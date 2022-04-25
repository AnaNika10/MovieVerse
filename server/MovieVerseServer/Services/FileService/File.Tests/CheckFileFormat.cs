using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Configuration;
using File.Controllers;
using File.Models;
using File.Utilities;
using File.Repositories.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Http;
using Moq;
using Xunit;

namespace File.Tests
{
    public class CheckFileFormat
    {
        private static readonly string _uploadPath = "Upload";
        private Dictionary<string, string> _confOptions = new Dictionary<string, string>
                        {
                            {"FileUploadDirectory", _uploadPath},
                        };    
        private static readonly string _ContentRootPath = 
                                "/home/marija/Desktop/MovieVerse/server/MovieVerseServer/Services/FileService/File";
        
        [Theory]
        [InlineData(0)]
        [InlineData(30000001)]
        public async void InvalidFileSize_Image(long fSize)
        {
            // given DI arguments for construction of FileController and file name
            var config = new ConfigurationBuilder()
                                    .AddInMemoryCollection(_confOptions)
                                    .Build();
            var env = new Mock<IHostEnvironment>();
            var logger = new Mock<ILogger<FileController>>();
            var repo = new Mock<IFileRepository>();
            env.Setup(e => e.ContentRootPath).Returns(_ContentRootPath);
            var originalFileName = "foo.mp4";
        
            // when posting image of invalid size
            var controller = new FileController(env.Object, logger.Object, config, repo.Object);
            var form = new Mock<IFormFile>();
            form.Setup(f => f.Length).Returns(fSize);
            var model = new FileModel();
            model.FormFile = form.Object;
            var res =  await controller.PostImage(model, "alice");
            var statusCodeResult = (IStatusCodeActionResult)res;
            
            // then request fails
            Assert.NotNull(res);
            Assert.Equal(statusCodeResult.StatusCode, StatusCodes.Status400BadRequest);
        }
        [Theory]
        [InlineData(0)]
        [InlineData(30000001)]
        public async void InvalidFileSize_IVideo(long fSize)
        {
            // given DI arguments for construction of FileController and file name
            var config = new ConfigurationBuilder()
                                    .AddInMemoryCollection(_confOptions)
                                    .Build();
            var env = new Mock<IHostEnvironment>();
            var logger = new Mock<ILogger<FileController>>();
            var repo = new Mock<IFileRepository>();
            env.Setup(e => e.ContentRootPath).Returns(_ContentRootPath);
            var originalFileName = "foo.mp4";
        
            // when posting image of invalid size
            var controller = new FileController(env.Object, logger.Object, config, repo.Object);
            var res =  await controller.PostVideo("alice");
            var statusCodeResult = (IStatusCodeActionResult)res;
            
            // then request fails
            Assert.NotNull(res);
            Assert.Equal(statusCodeResult.StatusCode, StatusCodes.Status400BadRequest);
        }


        [Theory]
        [InlineData(".pdf")]
        [InlineData("")]
        [InlineData(null)]
        [InlineData(".exe")]
        [InlineData(".doc")]
        [InlineData(".svg")]
        public async void NotSupportedImageExtension(string ext)
        {
            // given extension of uploaded file
        
            // when checking not supported extension
            var extValidation = FileFormatValidator.ValidFileExt(ext);
            
            // then extension check returns false
            Assert.False(extValidation);
        }
        [Theory]
        [InlineData(".pdf")]
        [InlineData("")]
        [InlineData(null)]
        [InlineData(".exe")]
        [InlineData(".doc")]
        [InlineData(".asf")]
        [InlineData(".flv")]
        
        public async void NotSupportedVideoExtension(string ext)
        {
            // given extension of uploaded file
        
            // when checking not supported extension
            var extValidation = FileFormatValidator.ValidFileExt(ext, true);
            
            // then extension check returns false
            Assert.False(extValidation);
        }
                
    }
}
