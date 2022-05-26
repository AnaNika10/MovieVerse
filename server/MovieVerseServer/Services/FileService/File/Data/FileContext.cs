using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using File.DTO;
using MongoDB.Driver;

namespace File.Data
{
    public class FileContext : IFileContext
    {
        public IMongoCollection<FileDTO> Files {get;}
        
        public FileContext()
        {
            var client = new MongoClient("mongodb://host.docker.internal:27017");
            var database = client.GetDatabase("FileDB");

            Files = database.GetCollection<FileDTO>("Files");
        }
    }
}