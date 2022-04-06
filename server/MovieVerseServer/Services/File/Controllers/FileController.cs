using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using File.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;

namespace File.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FileController : ControllerBase
    {

        public static IHostEnvironment _env;
        private readonly ILogger<FileController> _logger;

        public FileController(IHostEnvironment env, ILogger<FileController> logger)
        {
            _env = env;
            _logger = logger;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult>Post([FromForm]FileModel file)
        {
            if(file.FormFile.Length > 0)
            {
                string path = _env.ContentRootPath + "/Upload/";

                if(!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                using (FileStream fileStream = new FileStream(Path.Combine(path, "file.png"), FileMode.Create, FileAccess.Write))
                {
                    await file.FormFile.CopyToAsync(fileStream);
                }
            }

            return Ok(new {fileSize = file.FormFile.Length});
        
        }

    }
}
