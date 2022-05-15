using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using File.Repositories.Interfaces;

namespace File.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FileFetchController : ControllerBase
    {

        private readonly IFileRepository _repo;
        private readonly ILogger<FileFetchController> _logger;

        public FileFetchController(ILogger<FileFetchController> logger, IFileRepository repo)
        {
            _logger = logger;
            _repo = repo;
            
        }
        
        [Route("[action]/{id}")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> GetImage(string id)
        {
            
            var fileInfo = await _repo.GetFile(id);

            if (fileInfo == null){
                return NotFound();
            }
            
            var content = System.IO.File.ReadAllBytes(fileInfo.uniqueFilePath);
            
            return File(content, "image/" + fileInfo.originalFileExt.Substring(1), fileInfo.originalFileName);
        }

        [Route("[action]/{id}")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> GetVideo(string id)
        {
            
            var fileInfo = await _repo.GetFile(id);

            if (fileInfo == null){
                return NotFound();
            }
            
            var content = System.IO.File.ReadAllBytes(fileInfo.uniqueFilePath);
            var mimeType = "";
            if(fileInfo.originalFileExt.Substring(1) == "mp4"){
                mimeType = "mp4";
            }
            else if(fileInfo.originalFileExt.Substring(1) == "mkv"){
                mimeType = "x-matroska";
            }
            return File(content, "video/" + mimeType + fileInfo.originalFileExt.Substring(1), fileInfo.originalFileName);
        }

        
        [Route("[action]")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> GetMulltipleImages([FromQuery(Name = "ids[]")] string[] ids)
        {
            
            var files = new List<FileContentResult>();
            foreach(var id in ids){
                var fileInfo = await _repo.GetFile(id);

                if (fileInfo == null){
                    return NotFound(new {message = "Image id: " + id});
                }
                var content = System.IO.File.ReadAllBytes(fileInfo.uniqueFilePath);
                files.Add(File(content, "image/" + fileInfo.originalFileExt.Substring(1), fileInfo.originalFileName));
            }
            
            return Ok(new {fetchedFiles = files.ToArray()}); 
        }
    }
}