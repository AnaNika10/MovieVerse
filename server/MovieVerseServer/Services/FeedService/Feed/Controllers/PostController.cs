using Feed.DTOs.Post;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using Feed.Repository.Interfaces;

namespace Feed.Controllers
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

        [HttpGet("user/{userId}")]
        [ProducesResponseType(typeof(IEnumerable<PostDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<PostDTO>>> GetPostsByUser(string userId)
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
        [HttpGet("{PostId}", Name = nameof(GetById))]
        [ProducesResponseType(typeof(PostDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PostDTO>> GetById(int PostId)
        {
            var post = await _repository.GetById(PostId);
            if (post == null)
            {
                return NotFound(null);
            }
            return Ok(post);
        }


        [HttpPost]
        [ProducesResponseType(typeof(PostDTO), StatusCodes.Status201Created)]
        public async Task<ActionResult<PostDTO>> CreatePost([FromBody] CreatePostDTO postDTO)
        {
            int Id = await _repository.CreatePost(postDTO);
            var post = await _repository.GetById(Id);
            return CreatedAtRoute("GetById", new { post.PostId },post);

        }
        [HttpPut]
        [ProducesResponseType(typeof(PostDTO), StatusCodes.Status200OK)]
        public async Task<ActionResult<PostDTO>> UpdatePost([FromBody] UpdatePostDTO postDTO)
        {
            await _repository.UpdatePost(postDTO);

            var post = await _repository.GetById(postDTO.PostId);
            return CreatedAtRoute("GetById", new { post.PostId }, post);
        }

        [HttpDelete("{postId}")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        public async Task<ActionResult<bool>> DeletePost(int postId)
        {
                var success = await _repository.DeletePost(postId);
                if (success)
                {
                    return Ok();
                }
                else
                {
                    return NotFound();
                }
                //var existingItem = await repository.GetItemAsync(id);

                //if (existingItem is null)
                //{
                //    return NotFound();
                //}

                //await repository.DeletePost(id);

                //return NoContent();
            
        }

    }
}
