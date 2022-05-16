using Feed.DTOs.Like;
using Feed.Repository.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Feed.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class LikeController : ControllerBase
    {
        private readonly ILikeRepository _repository;

        public LikeController(ILikeRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        [HttpPost]
        [ProducesResponseType(typeof(LikeDTO), StatusCodes.Status201Created)]
        public async Task<ActionResult<LikeDTO>> CreateLike([FromBody] CreateLikeDTO likeDTO)
        {
            var Id = await _repository.CreateLike(likeDTO);
            var like = await _repository.GetById(Id);
            return CreatedAtRoute("GetLike", new { id = Id }, like);
        }

        [HttpGet(Name = "GetLike")]
        [ProducesResponseType(typeof(LikeDTO), StatusCodes.Status200OK)]
        public async Task<ActionResult<LikeDTO>> GetLikeById(int id)
        {
            LikeDTO likeDTO = await _repository.GetById(id);

            return likeDTO == null ? NotFound(null) : Ok(likeDTO);
        }

        [HttpGet("/postLikes/{postId}")]
        [ProducesResponseType(typeof(IEnumerable<LikeDTO>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<LikeDTO>>> GetPostLikes(int postId)
        {
            IEnumerable<LikeDTO> likes = await _repository.GetPostLikes(postId);

            return likes == null ? NotFound(null) : Ok(likes);
        }

        [HttpGet("/commentLikes/{commentId}")]
        [ProducesResponseType(typeof(IEnumerable<LikeDTO>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<LikeDTO>>> GetCommentLikes(int commentId)
        {
            IEnumerable<LikeDTO> likes = await _repository.GetCommentLikes(commentId);

            return likes == null ? NotFound(null) : Ok(likes);
        }

        [HttpPut]
        [ProducesResponseType(typeof(LikeDTO), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateLike([FromBody] UpdateLikeDTO likeDTO)
        {
            return Ok(await _repository.UpdateLike(likeDTO));
        }

        [HttpDelete(Name = "DeleteLike")]
        [ProducesResponseType(typeof(LikeDTO), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteLike(int id)
        {
            return Ok(await _repository.DeleteLike(id));
        }
    }
}