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
using System.Security.Principal;
using System.Security.AccessControl;

namespace File.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FileController : ControllerBase
    {

        public static IHostEnvironment _env;
        private readonly ILogger<FileController> _logger;
        private string originalFileName;
        private string uniqueFileName = "file.png";
        private string uniqueFilePath;
        private readonly string uploadPath = "Upload";
        public FileController(IHostEnvironment env, ILogger<FileController> logger)
        {
            _env = env;
            _logger = logger;
        }


        private void removeExecutePermissions(string dirPath)
        {
            DirectoryInfo dInfo = new DirectoryInfo(dirPath);
            DirectorySecurity dSecurity = dInfo.GetAccessControl();

            dSecurity.RemoveAccessRule(new FileSystemAccessRule("Everyone", 
                                                                FileSystemRights.ExecuteFile, 
                                                                AccessControlType.Allow));
            dInfo.SetAccessControl(dSecurity);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult>Post([FromForm]FileModel file)
        {
            if(file.FormFile.Length > 0)
            {
                // Path.Combine should work both on Windows and Unix based OS
                string path = Path.Combine(_env.ContentRootPath, uploadPath);
                if(!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                    // TODO: test on Windows: ls -l and see if x permissions are missing for every user
                    // TODO: implement for linux
                    // removeExecutePermissions(path);
                }
                // TODO: construct uniqueFileName
                // TODO: HTML encode originalFileName
                using (FileStream fileStream = new FileStream(Path.Combine(path, uniqueFileName), FileMode.Create, FileAccess.Write))
                {
                    await file.FormFile.CopyToAsync(fileStream);
                }
            }

            return Ok(new {fileSize = file.FormFile.Length});
        
        }

    }
}
