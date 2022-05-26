using Dapper;
using Feed.Data;
using Feed.DTOs.Follow;
using System;
using Feed.Entities;
using System.Threading.Tasks;
using AutoMapper;
using System.Collections.Generic;
using System.Linq;
using Feed.Repository.Interfaces;

namespace Feed.Repositories
{
    public class FollowRepository : IFollowRepository
    {
        private readonly IFeedContext _context;
        private readonly IMapper _mapper;

        public FollowRepository(IFeedContext context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<int> CreateFollow(CreateFollowDTO followDTO)
        {
            using var connection = _context.GetConnection();

            var id = await connection.QueryFirstAsync<int>(
                  "INSERT INTO follow (\"FollowFromUserId\",\"FollowToUserId\",\"CreatedDate\") values (@FollowFromUserId, @FollowToUserId, @CreatedDate) RETURNING \"FollowId\"",
                  new { FollowFromUserId = followDTO.FollowFromUserId, FollowToUserId = followDTO.FollowToUserId, CreatedDate = followDTO.CreatedDate });
            return id;
        }

        public async Task<bool> DeleteFollow(int followId)
        {
            using var connection = _context.GetConnection();

            var affected = await connection.ExecuteAsync(
                "DELETE FROM follow WHERE \"FollowId\" = @followId",
                new { followId });

            if (affected == 0)
                return false;

            return true;
        }

        public async Task<FollowDTO> GetById(int id)
        {
            using var connection = _context.GetConnection();

            var f = await connection.QueryFirstOrDefaultAsync<Follow>(
                "SELECT * FROM \"Follow\" WHERE \"FollowId\" = @Id", new { Id = id });

            return _mapper.Map<FollowDTO>(f);
        }

        public async Task<FollowDTO> GetFollowUsersAndDate(string FromUserId, string ToUserId, DateTime CreatedDate)
        {
            using var connection = _context.GetConnection();
            var follow = await connection.QueryFirstOrDefaultAsync<Follow>(
                 "SELECT * FROM follow WHERE \"CreatedDate\" = @CreatedDate AND \"FollowToUserId\"=@FollowToUserId AND \"FollowFromUserId\"=@FollowFromUserId",
                 new { CreatedDate = CreatedDate, FollowToUserId = ToUserId, FollowFromUserId = FromUserId });

            return _mapper.Map<FollowDTO>(follow);
        }

        public async Task<IEnumerable<FollowDTO>> GetUserFollowers(string userId)
        {
            using var connection = _context.GetConnection();

            var follow = await connection.QueryAsync<Follow>(
              "SELECT * FROM follow WHERE \"FollowToUserId\"=@Id", new { Id = userId }

              );
            return _mapper.Map<IEnumerable<FollowDTO>>(follow);
        }

        public async Task<IEnumerable<FollowDTO>> GetUserFollowing(string userId)
        {
            using var connection = _context.GetConnection();

            var follow = await connection.QueryAsync<Follow>(
              "SELECT * FROM follow WHERE \"FollowFromUserId\"=@Id", new { Id = userId }

              );
            return _mapper.Map<IEnumerable<FollowDTO>>(follow);
        }
    }
}
