using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using File.DTO;
using File.Data;
using MongoDB.Driver;
using File.Repositories.Interfaces;

namespace File.Repositories
{
    public class FileRepository : IFileRepository
    {
        private readonly IFileContext _context;

        public FileRepository(IFileContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<FileDTO>> GetFiles()
        {
            return await _context.Files.Find<FileDTO>(p => true).ToListAsync();
        }

        public async void UploadFile(FileDTO file)
        {
            await _context.Files.InsertOneAsync(file);
        }
    }
}