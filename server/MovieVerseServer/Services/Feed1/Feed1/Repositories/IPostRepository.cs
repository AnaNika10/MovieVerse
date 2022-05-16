using Feed1.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
namespace Feed1
{
    public interface IPostRepository
    {
        Task<IEnumerable<PostDTO>> GetAllPosts();
        Task<bool> UpdatePost(UpdatePostDTO postDTO);
        Task<bool> CreatePost(CreatePostDTO postDTO);
        Task<IEnumerable<PostDTO>> GetPostByUser(string userId);
        Task<PostDTO> GetPostByDateAndUser(DateTime CreatedDate, string UserId);
        Task<bool> DeletePost(int postId);
    }
}
