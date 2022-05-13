using Feed.DTOs.Comment;
using Feed.Repository.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Feed.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class CommentController : ControllerBase
    {
        private readonly ICommentRepository _repository;

        public CommentController(ICommentRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        [HttpPost]
        [ProducesResponseType(typeof(CommentDTO), StatusCodes.Status201Created)]
        public async Task<ActionResult<CommentDTO>> CreateComment([FromBody] CreateCommentDTO commentDTO)
        {
            var Id = await _repository.CreateComment(commentDTO);
            var comment = await _repository.GetById(Id);
            return CreatedAtRoute("GetComment", new { id = Id}, comment);
        }

        [HttpGet(Name = "GetComment")]
        [ProducesResponseType(typeof(CommentDTO), StatusCodes.Status200OK)]
        public async Task<ActionResult<CommentDTO>> GetCommentById(int id)
        {
            CommentDTO commentDTO = await _repository.GetById(id);

            return commentDTO == null ? NotFound(null) : Ok(commentDTO);
        }

        [HttpPut]
        [ProducesResponseType(typeof(CommentDTO), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateComment([FromBody] UpdateCommentDTO commentDTO)
        {
            return Ok(await _repository.UpdateComment(commentDTO));
        }

        [HttpDelete(Name = "DeleteComment")]
        [ProducesResponseType(typeof(CommentDTO), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteComment(int id)
        {
            return Ok(await _repository.DeleteComment(id));
        }
    }
}