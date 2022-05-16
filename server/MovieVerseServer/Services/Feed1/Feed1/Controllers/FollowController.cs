using Feed1.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using Feed1.Repositories;

namespace Feed1
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
        [HttpPost]
        [ProducesResponseType(typeof(FollowDTO), StatusCodes.Status201Created)]
        public async Task<ActionResult<FollowDTO>> CreateFollow([FromBody] CreateFollowDTO followDTO)
        {
            await _repository.CreateFollow(followDTO);
            var follow = await _repository.GetFollowUsersAndDate(followDTO.FollowFromUserId,followDTO.FollowToUserId,followDTO.CreatedDate);
            return CreatedAtRoute("GetFollowUsersAndDate", new { createdDate = follow.CreatedDate, FromUserId=follow.FollowFromUserId, ToUserId=follow.FollowToUserId }, follow);

        }
   

        [HttpDelete("{followId}")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        public async Task<ActionResult<bool>> DeleteFollow(int followId)
        {
            return Ok(await _repository.DeleteFollow(followId));
        }
    }
}
