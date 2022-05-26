using Feed.DTOs.Follow;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
namespace Feed.Repository.Interfaces
{
    public interface IFollowRepository
    {

  
        Task<int> CreateFollow(CreateFollowDTO postDTO);
        Task<IEnumerable<FollowDTO>> GetUserFollowers(string userId);
        Task<IEnumerable<FollowDTO>> GetUserFollowing(string userId);
        Task<FollowDTO> GetFollowUsersAndDate(string FromUserId, string ToUserId, DateTime CreatedDate);
        Task<FollowDTO> GetById(int id);
        Task<bool> DeleteFollow(int followId);
    }
}
