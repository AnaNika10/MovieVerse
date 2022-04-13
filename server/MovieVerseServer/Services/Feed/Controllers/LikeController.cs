using Feed.Entities;
using Feed.Store.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Feed.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class LikeController : ControllerBase
    {
        private readonly ILikeStore _store;

        public LikeController(ILikeStore store)
        {
            _store = store ?? throw new ArgumentNullException(nameof(store));
        }

        [HttpPost]
        [ProducesResponseType(typeof(Like), StatusCodes.Status201Created)]
        public async Task<ActionResult<Like>> CreateLike([FromBody] Like like)
        {
            await _store.CreateLike(like);

            return CreatedAtRoute("GetLike", new { id = like.Id }, like);
        }

        [HttpGet("{id:length(24)}", Name = "GetLike")]
        [ProducesResponseType(typeof(Like), StatusCodes.Status200OK)]
        public async Task<ActionResult<Like>> GetLikeById(string id)
        {
            Like like = await _store.GetLikeById(id);
            if (like == null)
            {
                return NotFound(null);
            }
            return Ok(like);
        }

        [HttpPut]
        [ProducesResponseType(typeof(Like), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateLike([FromBody] Like like)
        {
            return Ok(await _store.UpdateLike(like));
        }

        [HttpDelete("{id:length(24)}", Name = "DeleteLike")]
        [ProducesResponseType(typeof(Like), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteLike(string id)
        {
            return Ok(await _store.DeleteLike(id));
        }
    }
}