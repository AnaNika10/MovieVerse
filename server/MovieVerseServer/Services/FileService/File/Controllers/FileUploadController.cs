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
using File.Utilities.Antivirus;
using Microsoft.Net.Http.Headers;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.WebUtilities;

namespace File.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FileUploadController : ControllerBase
    {

        private readonly IFileRepository _repo;
        public static IHostEnvironment _env;
        private readonly ILogger<FileUploadController> _logger;
        public IConfiguration _config { get; }
        public IAntiVirusContext _antiVirus;
        private string _path {get;}
        private static readonly FormOptions _defaultFormOptions = new FormOptions();
        
        
        
        public FileUploadController(IHostEnvironment env, ILogger<FileUploadController> logger, 
                                IConfiguration configuration, IFileRepository repo,
                                IAntiVirusContext antiVirus)
        {
            _env = env;
            _logger = logger;
            _config = configuration;
            _repo = repo;
            _antiVirus = antiVirus;
            // Path.Combine should work both on Windows and Unix based OS
            _path = Path.Combine(_env.ContentRootPath, _config.GetValue<string>("FileUploadDirectory"));
            if(!Directory.Exists(_path))
            {
                Directory.CreateDirectory(_path);
                // TODO: test on Windows: ls -l and see if x permissions are missing for every user
                FileAccessHandler.RemoveExecutePermissions(_path);
            }
        }


        [Route("[action]")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status415UnsupportedMediaType)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        
        public async Task<IActionResult>PostImage([FromForm]FileModel file, string userId, bool isAvatar=false)
        {
            (int code, string msg) = FileHelpers.ValidateFileProperties(file.FormFile, 
                                                    _config.GetValue<long>("FileSizeLimit"),
                                                    _logger);
            if(code >= 400){
                return StatusCode(code, new {message = msg});
            }
            
            var scanResult = await _antiVirus.ScanFileForViruses(file.FormFile, _logger); 
            
            if(!scanResult){
                return BadRequest(new {message = "Antivirus scan failed."});
            }

            if(isAvatar){
                var existingAvatar = await _repo.GetAvatar(userId);
                if(existingAvatar!=null){
                    System.IO.File.Delete(existingAvatar.uniqueFilePath);
                    bool isDeleteSuccessful = await _repo.DeleteAvatar(existingAvatar.Id);
                    if(!isDeleteSuccessful){
                        return StatusCode(500);
                    }
                }
            }
            var fileSize = file.FormFile.Length;
            var (originalFileName, originalFileExt) = FileFormatValidator.GetOriginalFileNameAndExtension(file.FormFile.FileName); 
            var (uniqueFileName, uniqueFilePath) =  FileFormatValidator.GetUniqueFileNameAndPath(_path);
            using (FileStream fileStream = new FileStream(uniqueFilePath, FileMode.Create, FileAccess.Write))
            {
                
                await file.FormFile.CopyToAsync(fileStream);
                _logger.LogInformation("OK");    
            }
            
            var uploadedImg = new FileDTO(originalFileName, originalFileExt, fileSize, uniqueFileName, uniqueFilePath, userId, isAvatar); 
            _repo.UploadFile(uploadedImg);
            
            return Created("Image ID", new {uploadedImgId = uploadedImg.Id});
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
                        (originalFileName, originalFileExt) = FileFormatValidator.GetOriginalFileNameAndExtension(contentDisposition.FileName.Value); 
                        (uniqueFileName, uniqueFilePath) =  FileFormatValidator.GetUniqueFileNameAndPath(_path);
                        
                        
                        var streamedFileContent = await FileHelpers.ProcessStreamedFile(
                            section, contentDisposition, ModelState,  _fileSizeLimit);

                        var scanResult = await _antiVirus.ScanFileForViruses(streamedFileContent, _logger); 
                        if(!scanResult){
                            return BadRequest(new {message = "Antivirus scan failed."});
                        }
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

        
        [Route("[action]")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status415UnsupportedMediaType)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        
        public async Task<IActionResult>PostMultipleImages([FromForm]MultipleFilesModel files, string userId)
        {
            if(files.formFiles == null){
                return BadRequest(new {message = "No images were sent"});
            }
            
            
            foreach(var file in files.formFiles){
                
                (int code, string msg) = FileHelpers.ValidateFileProperties(file, 
                                                    _config.GetValue<long>("FileSizeLimit"),
                                                    _logger);
                if(code >= 400){
                    return StatusCode(code, new {message = msg});
                }
             
                var scanResult = await _antiVirus.ScanFileForViruses(file, _logger); 
                if(!scanResult){
                    return BadRequest(new {message = "Antivirus scan failed."});
                }

            }

            List<string> imageIDs = new List<string>();
            foreach (var file in files.formFiles)
            {
                var fileSize = file.Length;
                var (originalFileName, originalFileExt) = FileFormatValidator.GetOriginalFileNameAndExtension(file.FileName); 
                var (uniqueFileName, uniqueFilePath) =  FileFormatValidator.GetUniqueFileNameAndPath(_path);

                using (FileStream fileStream = new FileStream(uniqueFilePath, FileMode.Create, FileAccess.Write))
                {
                    
                    await file.CopyToAsync(fileStream);
                    _logger.LogInformation("OK");    
                }

                var uploadedImg = new FileDTO(originalFileName, originalFileExt, fileSize, uniqueFileName, uniqueFilePath, userId, false); 
                _repo.UploadFile(uploadedImg);
                imageIDs.Add(uploadedImg.Id);
            }
            return Created("Image IDs", new {listOfImageIds = imageIDs.ToArray()});
        }
    }
}