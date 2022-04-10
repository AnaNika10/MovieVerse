using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using File.DTO;

namespace File.Repositories.Interfaces
{
    public interface IFileRepository
    {
        Task<IEnumerable<FileDTO>> GetFiles();
        void UploadFile(FileDTO file);
    }
}