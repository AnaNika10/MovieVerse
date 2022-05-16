using Feed1.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;

namespace Feed1
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class PostController : ControllerBase
    {

        private readonly IPostRepository _repository;

        public PostController(IPostRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        [HttpGet("{userId}")]
        [ProducesResponseType(typeof(IEnumerable<PostDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<PostDTO>>> GetPostByUser(string userId)
        {
            var posts = await _repository.GetPostByUser(userId);
            if (posts == null)
            {
                return NotFound();
            }
            return Ok(posts);
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<PostDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<PostDTO>>> GetAllPosts()
        {
            var post = await _repository.GetAllPosts();
            if (post == null)
            {
                return NotFound();
            }
            return Ok(post);
        }
        [HttpGet("{createdDate}/{userId}", Name = nameof(GetPostByDateAndUser))]
        [ProducesResponseType(typeof(PostDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<PostDTO>> GetPostByDateAndUser(DateTime createdDate, string userId)
        {
            var post = await _repository.GetPostByDateAndUser(createdDate, userId);
            if (post == null)
            {
                return NotFound();
            }
            return Ok(post);
        }


        [HttpPost]
        [ProducesResponseType(typeof(PostDTO), StatusCodes.Status201Created)]
        public async Task<ActionResult<PostDTO>> CreatePost([FromBody] CreatePostDTO postDTO)
        {
            await _repository.CreatePost(postDTO);
            var post = await _repository.GetPostByDateAndUser(postDTO.CreatedDate,postDTO.UserId);
            return CreatedAtRoute("GetPostByDateAndUser", new { createdDate = post.CreatedDate, userId = post.UserId}, post);
        }
        [HttpPut]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        public async Task<ActionResult<PostDTO>> UpdatePost([FromBody] UpdatePostDTO postDTO)
        {
            await _repository.UpdatePost(postDTO);

            var post = await _repository.GetPostByDateAndUser(postDTO.CreatedDate, postDTO.UserId);
            return CreatedAtRoute("GetPostByDateAndUser", new { createdDate = post.CreatedDate, userId = post.UserId }, post);
        }

        [HttpDelete("{postId}")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        public async Task<ActionResult<bool>> DeletePost(int postId)
        {
            return Ok(await _repository.DeletePost(postId));
        }

    }
}
