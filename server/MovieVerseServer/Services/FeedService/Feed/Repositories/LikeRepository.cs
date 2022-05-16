using AutoMapper;
using Dapper;
using Feed.Data;
using Feed.DTOs.Like;
using Feed.Entities;
using Feed.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Feed.Repository
{
    public class LikeRepository : ILikeRepository
    {
        private readonly IFeedContext _context;
        private readonly IMapper _mapper;

        public LikeRepository(IFeedContext context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<LikeDTO> GetById(int id)
        {
            using var connection = _context.GetConnection();

            var like = await connection.QueryFirstOrDefaultAsync<Like>(
                "SELECT * FROM FLike WHERE id = @Id", new { Id = id });

            return _mapper.Map<LikeDTO>(like);
        }

        public async Task<IEnumerable<LikeDTO>> GetPostLikes(int postId)
        {
            using var connection = _context.GetConnection();

            var postLikes = await connection.QueryAsync<Like>(
                "SELECT * FROM FLike WHERE postId = @postId", new { postId });

            return _mapper.Map<IEnumerable<LikeDTO>>(postLikes);
        }
        public async Task<IEnumerable<LikeDTO>> GetCommentLikes(int commentId)
        {
            using var connection = _context.GetConnection();

            var commentLikes = await connection.QueryAsync<Like>(
                "SELECT * FROM FLike WHERE postId = @postId", new { commentId });

            return _mapper.Map<IEnumerable<LikeDTO>>(commentLikes);
        }

        public async Task<int> CreateLike(CreateLikeDTO likeDTO)
        {
            using var connection = _context.GetConnection();

            var id = await connection.QueryFirstAsync<int>(
                "INSERT INTO FLike (UserId, PostId, CommentId)" +
                "VALUES (@UserId, @PostId, @CommentId)" +
                "RETURNING Id",
                new {
                    likeDTO.UserId, likeDTO.PostId, likeDTO.CommentId
                }
            );

            return id;
        }

        public async Task<bool> DeleteLike(int id)
        {
            using var connection = _context.GetConnection();

            var affected = await connection.ExecuteAsync(
                 "DELETE FROM FLike WHERE Id = @Id",
                new { Id = id }
            );

            return affected != 0;
        }

        public async Task<bool> UpdateLike(UpdateLikeDTO likeDTO)
        {
            using var connection = _context.GetConnection();

            var affected = await connection.ExecuteAsync(
                "UPDATE FLike SET UserId = @UserId, PostId = @PostId," +
                "CommentId = @CommentId, CreatedAt = @CreatedAt WHERE Id = @Id",
                new {likeDTO.UserId, likeDTO.PostId, likeDTO.CommentId, likeDTO.CreatedAt, likeDTO.Id }
            );

            return affected != 0;
        }
    }
}
