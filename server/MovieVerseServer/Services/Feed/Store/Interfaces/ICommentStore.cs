using Feed.Entities;

namespace Feed.Store.Interfaces
{
    public interface ICommentStore
    {
        Task<Comment> GetCommentById(string id);
        Task CreateComment(Comment comment);

        Task<bool> UpdateComment(Comment comment);

        Task<bool> DeleteComment(string id);
    }
}
