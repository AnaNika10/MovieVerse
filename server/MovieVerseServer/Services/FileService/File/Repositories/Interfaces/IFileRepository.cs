using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using File.DTO;

namespace File.Repositories.Interfaces
{
    public interface IFileRepository
    {
        // Task<IEnumerable<FileDTO>> GetFiles();
        void UploadFile(FileDTO file);
        public Task<FileDTO> GetAvatar(string userId);
        public Task<bool> DeleteAvatar(string fileId);
    }
}