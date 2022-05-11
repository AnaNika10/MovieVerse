using Dapper;
using Feed.Data;
using Feed.Entities;
using Feed.Store.Interfaces;
using System;
using System.Threading.Tasks;

namespace Feed.Store
{
    public class LikeStore : IStore<Like>
    {
        private readonly IDatabaseContext _context;

        public LikeStore(IDatabaseContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Like> GetById(string id)
        {
            using var connection = _context.GetConnection();

            var like = await connection.QueryFirstOrDefaultAsync<Like>(
                "SELECT * FROM Like WHERE id = @Id", new { Id = id });

            return like;
        }

        public async Task<bool> CreateEntity(Like like)
        {
            using var connection = _context.GetConnection();

            var affected = await connection.ExecuteAsync(
                "INSERT INTO Like (Id, UserId, PostId, CommentId, CreatedAt)" +
                "VALUES (@Id, @UserId, @PostId, @CommentId, @CreatedAt)",
                new { 
                    like.Id, like.UserId, like.PostId, like.CommentId, like.CreatedAt
                }
            );

            return affected != 0;
        }

        public async Task<bool> DeleteEntity(string id)
        {
            using var connection = _context.GetConnection();

            var affected = await connection.ExecuteAsync(
                 "DELETE FROM Like WHERE Id = @Id",
                new { Id = id }
            );

            return affected != 0;
        }

        public async Task<bool> UpdateEntity(Like like)
        {
            using var connection = _context.GetConnection();

            var affected = await connection.ExecuteAsync(
                "UPDATE Like SET Id=@Id, UserId = @UserId, PostId = @PostId," +
                "CommentId = @CommentId, CreatedAt = @CreatedAt WHERE Id = @Id",
                new { like.Id, like.UserId, like.PostId, like.CommentId, like.CreatedAt }
            );

            return affected != 0;
        }
    }
}
