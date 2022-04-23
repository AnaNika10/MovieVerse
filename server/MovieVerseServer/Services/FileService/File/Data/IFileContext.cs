using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using File.DTO;
using MongoDB.Driver;

namespace File.Data
{
    public interface IFileContext
    {
        public IMongoCollection<FileDTO> Files {get;}
    }
}