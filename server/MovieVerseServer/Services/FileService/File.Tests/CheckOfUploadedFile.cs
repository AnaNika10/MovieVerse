using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using File.Controllers;
using File.Repositories.Interfaces;
using File.Utilities.Antivirus;
using File.Utilities;
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
            // given file name
            var originalFileName = "foo.mp4";

            // when preprocessing file
            (string htmlEncodedName, string _) = FileFormatValidator.GetOriginalFileNameAndExtension(originalFileName);

            // then file name is html encoded
            Assert.Equal(HttpUtility.HtmlDecode(htmlEncodedName), originalFileName);

        }

        [Fact]
        public void SafeFileNameAndPath_AreGenerated()
        {
            // given file name and upload path

            var originalFileName = "foo.mp4";
            var uploadPath = "/home/marija/Desktop/MovieVerse/server/MovieVerseServer/Services/FileService/File/Upload";
        
            // when preprocessing file
            (string newFileName, string newFilePath) = FileFormatValidator.GetUniqueFileNameAndPath(uploadPath);

            // then random file name and unique file path are generated
            Assert.False(newFileName == originalFileName);
            Assert.True(Path.Combine(_ContentRootPath, _uploadPath, newFileName) == newFilePath);

        }
        
    }
}
