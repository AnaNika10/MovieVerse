using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using File.Controllers;
using File.Repositories;
using File.Repositories.Interfaces;
using File.Data;
using File.Utilities.Antivirus;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using System.Security.Principal;
using System.Security.AccessControl;
using Moq;
using Xunit;
using Mono.Unix;

namespace File.Tests
{
    public class CreatePublicDirectory
    {
        private static readonly string _uploadPath = "Upload";
        private Dictionary<string, string> _confOptions = new Dictionary<string, string>
                        {
                            {"FileUploadDirectory", _uploadPath},
                        };    
        private static readonly string _ContentRootPath = 
                                "/home/marija/Desktop/MovieVerse/server/MovieVerseServer/Services/FileService/File";
        
        [Fact]
        public void CreationOfFileController_CreatesUploadDir()
        {
            // given DI arguments for construction of FileController
            var config = new ConfigurationBuilder()
                                    .AddInMemoryCollection(_confOptions)
                                    .Build();
            var env = new Mock<IHostEnvironment>();
            var logger = new Mock<ILogger<FileUploadController>>();
            var repo = new Mock<IFileRepository>();
            var antiVirus = new AntiVirusContext();
            env.Setup(e => e.ContentRootPath).Returns(_ContentRootPath);
        
            // when FileUploadController is created
            var controller = new FileUploadController(env.Object, logger.Object, config, repo.Object, antiVirus);

            // then upload dir is created 
            var dirPath = Path.Combine(_ContentRootPath, _uploadPath);
            DirectoryInfo dInfo = new DirectoryInfo(dirPath);

            Assert.True(dInfo.Exists);

        }
        
        [Fact]
        public void CreationOfMultipleFileUploadControllers_ResultsInUniqueUploadDirectory()
        {
            //given DI arguments for construction of FileCOntroller
            var config = new ConfigurationBuilder()
                                    .AddInMemoryCollection(_confOptions)
                                    .Build();
            var env = new Mock<IHostEnvironment>();
            var logger = new Mock<ILogger<FileUploadController>>();
            var repo = new Mock<IFileRepository>();
            var antiVirus = new AntiVirusContext();
            env.Setup(e => e.ContentRootPath).Returns(_ContentRootPath);
            
            // when multiple controllers are created
            var controller1 = new FileUploadController(env.Object, logger.Object, config, repo.Object, antiVirus);
            var controller2 = new FileUploadController(env.Object, logger.Object, config, repo.Object, antiVirus);
            

            //then unique upload directory exists
            var numOfUploadDirs = Directory.GetDirectories(_ContentRootPath)
                     .Where(dir => dir.Contains(_uploadPath))
                     .ToArray<string>()
                     .Length;
            Assert.Equal(numOfUploadDirs, 1);
        }

        [Fact]
        public void CreatedUploadDir_HasNoExecutePermissions_Unix()
        {
            //given DI arguments for construction of FileCOntroller
            var config = new ConfigurationBuilder()
                                    .AddInMemoryCollection(_confOptions)
                                    .Build();
            var env = new Mock<IHostEnvironment>();
            var logger = new Mock<ILogger<FileUploadController>>();
            var repo = new Mock<IFileRepository>();
            var antivirus = new AntiVirusContext();
            env.Setup(e => e.ContentRootPath).Returns(_ContentRootPath);
            
            // when upload dir is created
            var controller = new FileUploadController(env.Object, logger.Object, config, repo.Object, antivirus);
            

            //then it has no execute permissions
            var fileInfo = new Mono.Unix.UnixFileInfo(Path.Combine(_ContentRootPath, _uploadPath));
            var permissions = FileAccessPermissions.UserRead | FileAccessPermissions.UserWrite | FileAccessPermissions.UserExecute
                | FileAccessPermissions.GroupRead | FileAccessPermissions.GroupWrite
                | FileAccessPermissions.OtherRead | FileAccessPermissions.OtherWrite;
            
            Assert.Equal(permissions, fileInfo.FileAccessPermissions);
        }

        
    }
}
