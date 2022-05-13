using Feed.DTOs.Like;
using Feed.Entities;
using System.Threading.Tasks;

namespace Feed.Repository.Interfaces
{
    public interface ILikeRepository
    {
        Task<LikeDTO> GetById(int id);
        Task<int> CreateLike(CreateLikeDTO like);

        Task<bool> UpdateLike(UpdateLikeDTO like);

        Task<bool> DeleteLike(int id);
    }
}
