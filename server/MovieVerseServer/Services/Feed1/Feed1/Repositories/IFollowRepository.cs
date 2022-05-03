﻿using Feed1.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
namespace Feed1.Repositories
{
    public interface IFollowRepository
    {

  
        Task<bool> CreateFollow(CreateFollowDTO postDTO);
        Task<IEnumerable<FollowDTO>> GetUserFollowers(string userId);
        Task<IEnumerable<FollowDTO>> GetUserFollowing(string userId);
        Task<FollowDTO> GetFollowUsersAndDate(string FromUserId, string ToUserId, DateTime CreatedDate);

        Task<bool> DeleteFollow(int followId);
    }
}
