using Feed.DTOs.Follow;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using Feed.Repositories;
using Feed.Repository.Interfaces;

namespace Feed.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class FollowController : ControllerBase
    {
        private readonly IFollowRepository _repository;

        public FollowController(IFollowRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        [HttpGet("/follow/{userId}")]
        [ProducesResponseType(typeof(IEnumerable<FollowDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<FollowDTO>>> GetUserFollowers(string userId)
        {
            var followers = await _repository.GetUserFollowers(userId);
            if (followers == null)
            {
                return NotFound();
            }
            return Ok(followers);
        }
        [HttpGet("/following/{userId}")]
        [ProducesResponseType(typeof(IEnumerable<FollowDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<FollowDTO>>> GetUserFollowing(string userId)
        {
            var following = await _repository.GetUserFollowing(userId);
            if (following == null)
            {
                return NotFound();
            }
            return Ok(following);
        }

        [HttpGet("{createdDate}/{FromUserId}/{ToUserId}", Name = nameof(GetFollowUsersAndDate))]
        [ProducesResponseType(typeof(FollowDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<FollowDTO>> GetFollowUsersAndDate(DateTime createdDate, string FromUserId, string ToUserId)
        {
            var follows = await _repository.GetFollowUsersAndDate(FromUserId, ToUserId,createdDate);
            if (follows == null)
            {
                return NotFound();
            }
            return Ok(follows);
        }

        [HttpGet("{FollowId}", Name = nameof(GetById))]
        [ProducesResponseType(typeof(FollowDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<FollowDTO>> GetById(int FollowId)
        {
            var f = await _repository.GetById(FollowId);
            if (f == null)
            {
                return NotFound(null);
            }
            return Ok(f);
        }
        [HttpPost]
        [ProducesResponseType(typeof(FollowDTO), StatusCodes.Status201Created)]
        public async Task<ActionResult<FollowDTO>> CreateFollow([FromBody] CreateFollowDTO followDTO)
        {
            int id = await _repository.CreateFollow(followDTO);
            var f = await _repository.GetById(id);
            return CreatedAtRoute("GetById", new { f.FollowId }, f);

        }
   

        [HttpDelete("{followId}")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        public async Task<ActionResult<bool>> DeleteFollow(int followId)
        {
            var success = await _repository.DeleteFollow(followId);
            if (success)
            {
                return Ok();
            }
            else
            {
                return NotFound();
            }
        }
    }
}
