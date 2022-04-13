using Feed.Entities;
using Feed.Store.Interfaces;

namespace Feed.Store
{
    public class CommentStore : ICommentStore
    {
        public Task CreateComment(Comment comment)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteComment(string id)
        {
            throw new NotImplementedException();
        }

        public Task<Comment> GetCommentById(string id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateComment(Comment comment)
        {
            throw new NotImplementedException();
        }
    }
}
