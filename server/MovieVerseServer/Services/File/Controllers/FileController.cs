using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using File.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using System.Net;
using File.Repositories.Interfaces;
using File.DTO;
using File.Utillities;
namespace File.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FileController : ControllerBase
    {

        private readonly IFileRepository _repo;
        public static IHostEnvironment _env;
        private readonly ILogger<FileController> _logger;
        public IConfiguration _config { get; }
        public FileController(IHostEnvironment env, ILogger<FileController> logger, 
                                IConfiguration configuration, IFileRepository repo)
        {
            _env = env;
            _logger = logger;
            _config = configuration;
            _repo = repo;
        }

        

        [Route("[action]")]
        [HttpPost]
        // [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status415UnsupportedMediaType)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        
        public async Task<IActionResult>PostImage([FromForm]FileModel file, string userId)
        {
            if(file.FormFile.Length > 0)
            {
                // Path.Combine should work both on Windows and Unix based OS
                string path = Path.Combine(_env.ContentRootPath, _config.GetValue<string>("FileUploadDirectory"));
                if(!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                    // TODO: test on Windows: ls -l and see if x permissions are missing for every user
                    FileAccessHandler.RemoveExecutePermissions(path);
                }
                var originalFileName = Path.GetFileName(file.FormFile.FileName);
                originalFileName = WebUtility.HtmlEncode(originalFileName);
                var originalFileExt = Path.GetExtension(file.FormFile.FileName).ToLowerInvariant();
                if(!FileFormatValidator.ValidFileExt(originalFileExt))
                {
                    return StatusCode(415, new {message = "Unsupported file format"});
                }
                var fileSize = file.FormFile.Length;
                // _logger.LogInformation("{} {} {}", originalFileName, originalFileExt, fileSize);
                
                var uniqueFileName = Path.GetRandomFileName();
                var uniqueFilePath = Path.Combine(path, uniqueFileName);
                
                if(!FileFormatValidator.VaildFileSignature(originalFileExt, file)){
                    _logger.LogInformation("File ext signature failed");
                    return StatusCode(415, new {message = "File extension signature check failed"});
                }
                using (FileStream fileStream = new FileStream(uniqueFilePath, FileMode.Create, FileAccess.Write))
                {
                    
                    await file.FormFile.CopyToAsync(fileStream);
                    _logger.LogInformation("OK");    
                }
                _repo.UploadFile(new FileDTO(originalFileName, originalFileExt, fileSize, uniqueFileName, uniqueFilePath, userId));
                //TODO: change to CreatedAtRoute
                return Ok(new {fileSize = file.FormFile.Length});
            }
            else
            {
                return BadRequest();
            }
        
        }

        [Route("[action]")]
        [HttpPost]
        // [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status415UnsupportedMediaType)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [DisableFormValueModelBinding]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PostVideo(string userId)
        {
            if (!MultipartRequestHelper.IsMultipartContentType(Request.ContentType))
            {
                ModelState.AddModelError("File", 
                    $"The request couldn't be processed (Error 1).");
                // Log error

                return BadRequest(ModelState);
            }

            var boundary = MultipartRequestHelper.GetBoundary(
                MediaTypeHeaderValue.Parse(Request.ContentType),
                _defaultFormOptions.MultipartBoundaryLengthLimit);
            var reader = new MultipartReader(boundary, HttpContext.Request.Body);
            var section = await reader.ReadNextSectionAsync();

            while (section != null)
            {
                var hasContentDispositionHeader = 
                    ContentDispositionHeaderValue.TryParse(
                        section.ContentDisposition, out var contentDisposition);

                if (hasContentDispositionHeader)
                {
                    // This check assumes that there's a file
                    // present without form data. If form data
                    // is present, this method immediately fails
                    // and returns the model error.
                    if (!MultipartRequestHelper
                        .HasFileContentDisposition(contentDisposition))
                    {
                        ModelState.AddModelError("File", 
                            $"The request couldn't be processed (Error 2).");
                        // Log error

                        return BadRequest(ModelState);
                    }
                    else
                    {
                        // Don't trust the file name sent by the client. To display
                        // the file name, HTML-encode the value.
                        var trustedFileNameForDisplay = WebUtility.HtmlEncode(
                                contentDisposition.FileName.Value);
                        var trustedFileNameForFileStorage = Path.GetRandomFileName();

                        // **WARNING!**
                        // In the following example, the file is saved without
                        // scanning the file's contents. In most production
                        // scenarios, an anti-virus/anti-malware scanner API
                        // is used on the file before making the file available
                        // for download or for use by other systems. 
                        // For more information, see the topic that accompanies 
                        // this sample.

                        var streamedFileContent = await FileHelpers.ProcessStreamedFile(
                            section, contentDisposition, ModelState, 
                            _permittedExtensions, _fileSizeLimit);

                        if (!ModelState.IsValid)
                        {
                            return BadRequest(ModelState);
                        }

                        using (var targetStream = System.IO.File.Create(
                            Path.Combine(_targetFilePath, trustedFileNameForFileStorage)))
                        {
                            await targetStream.WriteAsync(streamedFileContent);

                            _logger.LogInformation(
                                "Uploaded file '{TrustedFileNameForDisplay}' saved to " +
                                "'{TargetFilePath}' as {TrustedFileNameForFileStorage}", 
                                trustedFileNameForDisplay, _targetFilePath, 
                                trustedFileNameForFileStorage);
                        }
                    }
                }

                // Drain any remaining section body that hasn't been consumed and
                // read the headers for the next section.
                section = await reader.ReadNextSectionAsync();
            }

            return Created(nameof(StreamingController), null);
        }

        
        // [HttpGet]
        // [ProducesResponseType(StatusCodes.Status200OK)]
        // public async Task<ActionResult<IEnumerable<FileModel>>> GetFiles()
        // {
        //     var files = _repo.GetFiles();

        //     return Ok(files);
        // }

    }
}
