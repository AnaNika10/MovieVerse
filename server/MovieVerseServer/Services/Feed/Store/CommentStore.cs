using Dapper;
using Feed.Data;
using Feed.Entities;
using Feed.Store.Interfaces;
using System;
using System.Threading.Tasks;

namespace Feed.Store
{
    public class CommentStore : IStore<Comment>
    {
        private readonly IDatabaseContext _context;

        public async Task<Comment> GetById(string id)
        {
            using var connection = _context.GetConnection();

            var comment = await connection.QueryFirstOrDefaultAsync<Comment>(
                "SELECT * FROM Comment WHERE id = @Id", new { Id = id });

            return comment;
        }

        public async Task<bool> CreateEntity(Comment comment)
        {
            using var connection = _context.GetConnection();

            var affected = await connection.ExecuteAsync(
                "INSERT INTO Comment (Id, UserId, PostId, Text, HashTags, LikesNum, CreatedAt)" +
                "VALUES (@Id, @UserId, @PostId, @Text, @HashTags, @LikesNum, @CreatedAt)",
                new {
                    comment.Id, comment.UserId, comment.PostId, comment.Text,
                    comment.Hashtags, comment.LikesNum, comment.CreatedAt
                }
            );

            return affected != 0;
        }

        public async Task<bool> DeleteEntity(string id)
        {
            using var connection = _context.GetConnection();

            var affected = await connection.ExecuteAsync(
                 "DELETE FROM Comment WHERE Id = @Id",
                new { Id = id }
            );

            return affected != 0;
        }

        public async Task<bool> UpdateEntity(Comment comment)
        {
            using var connection = _context.GetConnection();

            var affected = await connection.ExecuteAsync(
                "UPDATE Comment SET Id=@Id, UserId = @UserId, PostId = @PostId," +
                "Text = @Text, Hashtags = @Hashtags, LikesNum = @LikesNum," +
                "CreatedAt = @CreatedAt WHERE Id = @Id",
                new {
                    comment.Id, comment.UserId, comment.PostId, comment.Text,
                    comment.Hashtags, comment.LikesNum, comment.CreatedAt
                }
            );

            return affected != 0;
        }
    }
}
