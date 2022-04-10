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

        

        [HttpPost]
        // [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status415UnsupportedMediaType)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        
        public async Task<IActionResult>Post([FromForm]FileModel file, [FromQuery] string userID = "kajaa")
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
                _repo.UploadFile(new FileDTO(originalFileName, originalFileExt, fileSize, uniqueFileName, uniqueFilePath, userID));
                //TODO: change to CreatedAtRoute
                return Ok(new {fileSize = file.FormFile.Length});
            }
            else
            {
                return BadRequest();
            }
        
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
