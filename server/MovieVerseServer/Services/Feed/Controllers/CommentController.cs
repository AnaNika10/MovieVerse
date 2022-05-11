using Feed.Entities;
using Feed.Store;
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
        private readonly CommentStore _store;

        public CommentController(CommentStore store)
        {
            _store = store ?? throw new ArgumentNullException(nameof(store));
        }

        [HttpPost]
        [ProducesResponseType(typeof(Comment), StatusCodes.Status201Created)]
        public async Task<ActionResult<Comment>> CreateComment([FromBody] Comment comment)
        {
            await _store.CreateEntity(comment);

            return CreatedAtRoute("GetComment", new { id = comment.Id }, comment);
        }

        [HttpGet("{id:length(24)}", Name = "GetComment")]
        [ProducesResponseType(typeof(Comment), StatusCodes.Status200OK)]
        public async Task<ActionResult<Comment>> GetCommentById(string id)
        {
            Comment comment = await _store.GetById(id);

            return comment == null ? NotFound(null) : Ok(comment);
        }

        [HttpPut]
        [ProducesResponseType(typeof(Comment), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateComment([FromBody] Comment comment)
        {
            return Ok(await _store.UpdateEntity(comment));
        }

        [HttpDelete("{id:length(24)}", Name = "DeleteComment")]
        [ProducesResponseType(typeof(Comment), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteComment(string id)
        {
            return Ok(await _store.DeleteEntity(id));
        }
    }
}