﻿using System;
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
namespace File.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FileController : ControllerBase
    {

        public static IHostEnvironment _env;
        private readonly ILogger<FileController> _logger;
        private string originalFileName;
        private string originalFileExt;
        private long fileSize;
        private string uniqueFileName;
        private string uniqueFilePath;
        public IConfiguration _config { get; }
        public FileController(IHostEnvironment env, ILogger<FileController> logger, IConfiguration configuration)
        {
            _env = env;
            _logger = logger;
            _config = configuration;
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

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult>Post([FromForm]FileModel file)
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
                originalFileName = Path.GetFileName(file.FormFile.FileName);
                originalFileExt = originalFileName.Substring(originalFileName.LastIndexOf("."));
                originalFileName = WebUtility.HtmlEncode(originalFileName);
                fileSize = file.FormFile.Length;
                
                uniqueFileName = Path.GetRandomFileName();
                uniqueFilePath = Path.Combine(path, uniqueFileName);
                
                using (FileStream fileStream = new FileStream(uniqueFilePath, FileMode.Create, FileAccess.Write))
                {
                    await file.FormFile.CopyToAsync(fileStream);
                }
            }

            return Ok(new {fileSize = file.FormFile.Length});
        
        }

    }
}
