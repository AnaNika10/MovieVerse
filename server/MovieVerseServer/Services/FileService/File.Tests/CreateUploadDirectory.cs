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
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using Moq;
using Xunit;

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
        public void CreationOfMultipleFileControllers_ResultsInUniqueUploadDirectory()
        {
            //given DI arguments for construction of FileCOntroller
            var config = new ConfigurationBuilder()
                                    .AddInMemoryCollection(_confOptions)
                                    .Build();
            var env = new Mock<IHostEnvironment>();
            var logger = new Mock<ILogger<FileController>>();
            var repo = new Mock<IFileRepository>();
            env.Setup(e => e.ContentRootPath).Returns(_ContentRootPath);
            
            // when multiple controllers are created
            var controller1 = new FileController(env.Object, logger.Object, config, repo.Object);
            var controller2 = new FileController(env.Object, logger.Object, config, repo.Object);
            

            //then unique upload directory exists
            var numOfUploadDirs = Directory.GetDirectories(_ContentRootPath)
                     .Where(dir => dir.Contains(_uploadPath))
                     .ToArray<string>()
                     .Length;
            Console.WriteLine(numOfUploadDirs);
            Assert.Equal(numOfUploadDirs, 1);
        }
    }
}
