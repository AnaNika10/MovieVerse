using Feed.Entities;
using Feed.Store.Interfaces;

namespace Feed.Store
{
    public class LikeStore : ILikeStore
    {
        public Task CreateLike(Like like)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteLike(string id)
        {
            throw new NotImplementedException();
        }

        public Task<Like> GetLikeById(string id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateLike(Like like)
        {
            throw new NotImplementedException();
        }
    }
}
