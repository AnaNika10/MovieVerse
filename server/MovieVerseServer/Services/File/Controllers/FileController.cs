using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using File.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using System.Net;
using System.Security.Principal;
using System.Security.AccessControl;
using System.Runtime.InteropServices;
using Mono.Unix;
using Microsoft.Extensions.Configuration;
using File.Repositories.Interfaces;
using File.DTO;
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
        private string[] permittedFileExtensions = {".jpg", ".jpeg", ".gif", ".png"};
        private static readonly Dictionary<string, List<byte[]>> _fileSignature = 
                                            new Dictionary<string, List<byte[]>>
                                            {
                                                { ".jpeg", new List<byte[]>
                                                    {
                                                        new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 },
                                                        new byte[] { 0xFF, 0xD8, 0xFF, 0xE2 },
                                                        new byte[] { 0xFF, 0xD8, 0xFF, 0xE3 },
                                                    }
                                                },
                                                { ".jpg", new List<byte[]>
                                                    {
                                                        new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 },  	 
                                                        new byte[] { 0xFF, 0xD8, 0xFF, 0xE1 },
                                                        new byte[] { 0xFF, 0xD8, 0xFF, 0xE8 },
                                                    }
                                                },
                                                { ".gif", new List<byte[]>
                                                    {
                                                        new byte[] { 0x47, 0x49, 0x46, 0x38 }
                                                    }
                                                },
                                                { ".png", new List<byte[]>
                                                    {
                                                        new byte[] { 0x89, 0x50, 0x4E, 0x47, 
                                                                     0x0D, 0x0A, 0x1A, 0x0A }
                                                    }
                                                },
                                            };
        public FileController(IHostEnvironment env, ILogger<FileController> logger, 
                                IConfiguration configuration, IFileRepository repo)
        {
            _env = env;
            _logger = logger;
            _config = configuration;
            _repo = repo;
        }


        private void removeExecutePermissionsWindows(string dirPath)
        {
            DirectoryInfo dInfo = new DirectoryInfo(dirPath);
                DirectorySecurity dSecurity = dInfo.GetAccessControl();

                dSecurity.RemoveAccessRule(new FileSystemAccessRule("Everyone", 
                                                                    FileSystemRights.ExecuteFile, 
                                                                    AccessControlType.Allow));
                dInfo.SetAccessControl(dSecurity);   
        }
        private void removeExecutePermissionsUnix(string dirPath)
        {
        
            var unixFileInfo = new Mono.Unix.UnixFileInfo(dirPath);
            unixFileInfo.FileAccessPermissions =
                FileAccessPermissions.UserRead | FileAccessPermissions.UserWrite | FileAccessPermissions.UserExecute
                | FileAccessPermissions.GroupRead | FileAccessPermissions.GroupWrite
                | FileAccessPermissions.OtherRead | FileAccessPermissions.OtherWrite;
        }
        private void removeExecutePermissions(string dirPath)
        {
            
            if(System.Runtime.InteropServices.RuntimeInformation
                                               .IsOSPlatform(OSPlatform.Windows))
            {
                removeExecutePermissionsWindows(dirPath);
            }
            else
            {
                removeExecutePermissionsUnix(dirPath);
            }        
            
        }
        
        private bool vaildFileSignature(string ext, FileModel file){
            using (var reader = new BinaryReader(file.FormFile.OpenReadStream()))
            {
                var signatures = _fileSignature[ext];
                var headerBytes = reader.ReadBytes(signatures.Max(m => m.Length));
                return signatures.Any(signature => 
                        headerBytes.Take(signature.Length).SequenceEqual(signature));
            }

        }
        private bool validFileExt(string ext){

            if (string.IsNullOrEmpty(ext) || !permittedFileExtensions.Contains(ext))
            {
                return false;
            }
            return true;
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
                    removeExecutePermissions(path);
                }
                //db info: 
                var originalFileName = Path.GetFileName(file.FormFile.FileName);
                originalFileName = WebUtility.HtmlEncode(originalFileName);
                var originalFileExt = Path.GetExtension(file.FormFile.FileName).ToLowerInvariant();
                if(!validFileExt(originalFileExt))
                {
                    return StatusCode(415, new {message = "Unsupported file format"});
                }
                var fileSize = file.FormFile.Length;
                // _logger.LogInformation("{} {} {}", originalFileName, originalFileExt, fileSize);
                
                var uniqueFileName = Path.GetRandomFileName();
                var uniqueFilePath = Path.Combine(path, uniqueFileName);
                
                if(!vaildFileSignature(originalFileExt, file)){
                    _logger.LogInformation("File ext signature failed");
                    return StatusCode(415, new {message = "File extension signature check failed"});
                }
                using (FileStream fileStream = new FileStream(uniqueFilePath, FileMode.Create, FileAccess.Write))
                {
                    
                    await file.FormFile.CopyToAsync(fileStream);
                    _logger.LogInformation("OK");    
                }
                _repo.UploadFile(new FileDTO(originalFileName, originalFileExt, fileSize, uniqueFileName, uniqueFilePath, userID));
                return Ok(new {fileSize = file.FormFile.Length});
            }
            else
            {
                return BadRequest();
            }
        
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<FileModel>>> GetFiles()
        {
            var files = _repo.GetFiles();

            return Ok(files);
        }

    }
}
