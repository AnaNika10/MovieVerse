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
using File.Utilities;
using Microsoft.Net.Http.Headers;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.WebUtilities;

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
        private string _path;
        private static readonly FormOptions _defaultFormOptions = new FormOptions();
        
        
        
        public FileController(IHostEnvironment env, ILogger<FileController> logger, 
                                IConfiguration configuration, IFileRepository repo)
        {
            _env = env;
            _logger = logger;
            _config = configuration;
            _repo = repo;
            // Path.Combine should work both on Windows and Unix based OS
            _path = Path.Combine(_env.ContentRootPath, _config.GetValue<string>("FileUploadDirectory"));
            if(!Directory.Exists(_path))
            {
                Directory.CreateDirectory(_path);
                // TODO: test on Windows: ls -l and see if x permissions are missing for every user
                FileAccessHandler.RemoveExecutePermissions(_path);
            }
        }

        private (string, string) GetUniqueFileNameAndPath()
        {
            var uniqueFileName = Path.GetRandomFileName();
            var uniqueFilePath = Path.Combine(_path, uniqueFileName);
            return (uniqueFileName, uniqueFilePath);
        }
        
        private (string, string) GetOriginalFileNameAndExtension(string path)
        {
            var originalFileName = Path.GetFileName(path);
            originalFileName = WebUtility.HtmlEncode(originalFileName);
            var originalFileExt = Path.GetExtension(path).ToLowerInvariant();
            return (originalFileName, originalFileExt);
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
            if(file.FormFile.Length > 0 && file.FormFile.Length < _config.GetValue<long>("FileSizeLimit"))
            {
                
                var fileSize = file.FormFile.Length;
                var (originalFileName, originalFileExt) = GetOriginalFileNameAndExtension(file.FormFile.FileName); 
                var (uniqueFileName, uniqueFilePath) =  GetUniqueFileNameAndPath();
                if(!FileFormatValidator.ValidFileExt(originalFileExt))
                {
                    return StatusCode(415, new {message = "Unsupported file format"});
                }
                
                
                if(!FileFormatValidator.VaildFileSignature(originalFileExt, file.FormFile.OpenReadStream())){
                    _logger.LogInformation("File ext signature failed");
                    return StatusCode(415, new {message = "File extension signature check failed"});
                }
                using (FileStream fileStream = new FileStream(uniqueFilePath, FileMode.Create, FileAccess.Write))
                {
                    
                    await file.FormFile.CopyToAsync(fileStream);
                    _logger.LogInformation("OK");    
                }
                var uploadedImg = new FileDTO(originalFileName, originalFileExt, fileSize, uniqueFileName, uniqueFilePath, userId); 
                _repo.UploadFile(uploadedImg);
                //TODO: change to CreatedAtRoute
                return Ok(new {uploadedImgId = uploadedImg.Id});
            }
            else
            {
                return BadRequest();
            }
        
        }

        [Route("[action]")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status415UnsupportedMediaType)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [DisableFormValueModelBinding]
        // [ValidateAntiForgeryToken]
        public async Task<IActionResult> PostVideo(string userId)
        {
            if (!MultipartRequestHelper.IsMultipartContentType(Request.ContentType))
            {
                ModelState.AddModelError("File", 
                    $"The request couldn't be processed.");

                return BadRequest(ModelState);
            }

            var boundary = MultipartRequestHelper.GetBoundary(
                MediaTypeHeaderValue.Parse(Request.ContentType),
                _defaultFormOptions.MultipartBoundaryLengthLimit);
            var reader = new MultipartReader(boundary, HttpContext.Request.Body);
            var section = await reader.ReadNextSectionAsync();
            
            var _fileSizeLimit = _config.GetValue<long>("FileSizeLimit");
            string originalFileName = System.String.Empty;
            string uniqueFileName = System.String.Empty;
            string uniqueFilePath = System.String.Empty;
            string originalFileExt = System.String.Empty;

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
                            $"The request couldn't be processed.");
                        return BadRequest(ModelState);
                    }
                    else
                    {
                        

                        (originalFileName, originalFileExt) = GetOriginalFileNameAndExtension(contentDisposition.FileName.Value); 
                        (uniqueFileName, uniqueFilePath) =  GetUniqueFileNameAndPath();
                        // **WARNING!**
                        // In the following example, the file is saved without
                        // scanning the file's contents. In most production
                        // scenarios, an anti-virus/anti-malware scanner API
                        // is used on the file before making the file available
                        // for download or for use by other systems. 
                        // For more information, see the topic that accompanies 
                        // this sample.
                        var streamedFileContent = await FileHelpers.ProcessStreamedFile(
                            section, contentDisposition, ModelState,  _fileSizeLimit);

                        if (!ModelState.IsValid)
                        {
                            return BadRequest(ModelState);
                        }

                        using (var targetStream = System.IO.File.Create(
                            Path.Combine(_path, uniqueFilePath)))
                        {
                            await targetStream.WriteAsync(streamedFileContent);
                        }
                    }
                }
                section = await reader.ReadNextSectionAsync();
            }
            var uploadedVideo = new FileDTO(originalFileName, originalFileExt, _fileSizeLimit, uniqueFileName, uniqueFilePath, userId);
            _repo.UploadFile(uploadedVideo);
            return Created("Video ID", new {uploadedVideoId = uploadedVideo.Id});
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
