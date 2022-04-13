using Feed.Entities;

namespace Feed.Store.Interfaces
{
    public interface ILikeStore
    {
        Task<Like> GetLikeById(string id);
        Task CreateLike(Like like);

        Task<bool> UpdateLike(Like like);

        Task<bool> DeleteLike(string id);
    }
}
