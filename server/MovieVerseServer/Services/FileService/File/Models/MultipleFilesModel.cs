using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;

namespace File.Models
{
    public class MultipleFilesModel
    {
        public IEnumerable<IFormFile> formFiles {get; set;}
    }
}