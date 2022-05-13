using Feed.DTOs.Comment;
using Feed.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Feed.Repository.Interfaces
{
    public interface ICommentRepository
    {
        Task<CommentDTO> GetById(int id);
        Task<int> CreateComment(CreateCommentDTO comment);

        Task<bool> UpdateComment(UpdateCommentDTO comment);

        Task<bool> DeleteComment(int id);

        Task<IEnumerable<CommentDTO>> getPostComments(int postId);
    }
}
