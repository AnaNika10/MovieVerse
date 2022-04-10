using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace File.DTO
{
    public class FileDTO
    {
        public string originalFileName {get; set;}
        public string originalFileExt {get; set;}
        public long fileSize {get; set;}
        public string uniqueFileName {get; set;}
        public string uniqueFilePath {get; set;}
        public string userID {get; set;}

        public FileDTO(string originalFileName, string originalFileExt, long fileSize, string uniqueFileName, string uniqueFilePath, string userID)
        {
            this.originalFileName = originalFileName;
            this.originalFileExt = originalFileExt;
            this.fileSize = fileSize;
            this.uniqueFileName = uniqueFileName;
            this.uniqueFilePath = uniqueFilePath;
            this.userID = userID;
        }
    }
}