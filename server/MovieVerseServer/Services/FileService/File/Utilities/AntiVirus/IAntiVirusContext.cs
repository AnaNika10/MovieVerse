using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using File.Controllers;
using ClamAV.Net.Client;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace File.Utilities.Antivirus
{
    public interface IAntiVirusContext
    {
        public IClamAvClient _client {get;}

        public Task<bool> ScanFileForViruses(IFormFile file, ILogger<FileController> logger);
    }
}