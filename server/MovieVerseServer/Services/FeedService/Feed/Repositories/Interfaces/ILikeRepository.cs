using Feed.DTOs.Like;
using Feed.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Feed.Repository.Interfaces
{
    public interface ILikeRepository
    {
        Task<LikeDTO> GetById(int id);
        Task<IEnumerable<LikeDTO>> GetPostLikes(int postId);
        Task<IEnumerable<LikeDTO>> GetCommentLikes(int commentId);
        Task<int> CreateLike(CreateLikeDTO like);

        Task<bool> UpdateLike(UpdateLikeDTO like);

        Task<bool> DeleteLike(int id);
    }
}
