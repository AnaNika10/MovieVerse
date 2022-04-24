using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using File.Controllers;
using File.Repositories.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using Moq;
using Xunit;
using Mono.Unix;

namespace File.Tests
{
    public class CheckOfUploadedFile
    {
        private static readonly string _uploadPath = "Upload";
        private Dictionary<string, string> _confOptions = new Dictionary<string, string>
                        {
                            {"FileUploadDirectory", _uploadPath},
                        };    
        private static readonly string _ContentRootPath = 
                                "/home/marija/Desktop/MovieVerse/server/MovieVerseServer/Services/FileService/File";
        
        [Fact]
        public void UploadedFile_IsHTMLEncoded()
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
        
            // when preprocessing file
            var controller = new FileController(env.Object, logger.Object, config, repo.Object);
            var (htmlEncodedName, _) = controller.GetOriginalFileNameAndExtension(originalFileName);

            // then file name is html encoded
            Assert.Equal(HttpUtility.HtmlDecode(htmlEncodedName), originalFileName);

        }
        
    }
}
