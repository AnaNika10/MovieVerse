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
    public class LikeController : ControllerBase
    {
        private readonly LikeStore _store;

        public LikeController(LikeStore store)
        {
            _store = store ?? throw new ArgumentNullException(nameof(store));
        }

        [HttpPost]
        [ProducesResponseType(typeof(Like), StatusCodes.Status201Created)]
        public async Task<ActionResult<Like>> CreateLike([FromBody] Like like)
        {
            await _store.CreateEntity(like);

            return CreatedAtRoute("GetLike", new { id = like.Id }, like);
        }

        [HttpGet("{id:length(24)}", Name = "GetLike")]
        [ProducesResponseType(typeof(Like), StatusCodes.Status200OK)]
        public async Task<ActionResult<Like>> GetLikeById(string id)
        {
            Like like = await _store.GetById(id);

            return like == null ? NotFound(null) : Ok(like);
        }

        [HttpPut]
        [ProducesResponseType(typeof(Like), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateLike([FromBody] Like like)
        {
            return Ok(await _store.UpdateEntity(like));
        }

        [HttpDelete("{id:length(24)}", Name = "DeleteLike")]
        [ProducesResponseType(typeof(Like), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteLike(string id)
        {
            return Ok(await _store.DeleteEntity(id));
        }
    }
}