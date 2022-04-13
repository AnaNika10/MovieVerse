using Feed.Entities;
using Feed.Store.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Feed.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class CommentController : ControllerBase
    {
        private readonly ICommentStore _store;

        public CommentController(ICommentStore store)
        {
            _store = store ?? throw new ArgumentNullException(nameof(store));
        }

        [HttpPost]
        [ProducesResponseType(typeof(Comment), StatusCodes.Status201Created)]
        public async Task<ActionResult<Comment>> CreateComment([FromBody] Comment comment)
        {
            await _store.CreateComment(comment);

            return CreatedAtRoute("GetComment", new { id = comment.Id }, comment);
        }

        [HttpGet("{id:length(24)}", Name = "GetComment")]
        [ProducesResponseType(typeof(Comment), StatusCodes.Status200OK)]
        public async Task<ActionResult<Comment>> GetCommentById(string id)
        {
            Comment comment = await _store.GetCommentById(id);
            if (comment == null)
            {
                return NotFound(null);
            }
            return Ok(comment);
        }

        [HttpPut]
        [ProducesResponseType(typeof(Comment), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateComment([FromBody] Comment comment)
        {
            return Ok(await _store.UpdateComment(comment));
        }

        [HttpDelete("{id:length(24)}", Name = "DeleteComment")]
        [ProducesResponseType(typeof(Comment), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteComment(string id)
        {
            return Ok(await _store.DeleteComment(id));
        }
    }
}