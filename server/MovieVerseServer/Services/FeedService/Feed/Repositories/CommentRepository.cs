using AutoMapper;
using Dapper;
using Feed.Data;
using Feed.DTOs.Comment;
using Feed.Entities;
using Feed.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Feed.Repository
{
    public class CommentRepository : ICommentRepository
    {
        private readonly IDatabaseContext _context;
        private readonly IMapper _mapper;

        public CommentRepository(IDatabaseContext context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<CommentDTO> GetById(int id)
        {
            using var connection = _context.GetConnection();

            var comment = await connection.QueryFirstOrDefaultAsync<Comment>(
                "SELECT * FROM Comment WHERE id = @Id", new { Id = id });

            return _mapper.Map<CommentDTO>(comment);
        }

        public async Task<IEnumerable<CommentDTO>> GetPostComments(int postId)
        {
            using var connection = _context.GetConnection();

            var postComments = await connection.QueryAsync<Comment>(
                "SELECT * FROM Comment WHERE postId = @postId", new { postId });

            return _mapper.Map<IEnumerable<CommentDTO>>(postComments);
        }

        public async Task<IEnumerable<CommentDTO>> GetCommentsWithHashtag(string tag)
        {
            using var connection = _context.GetConnection();

            var comments = await connection.QueryAsync<Comment>(
                "SELECT * FROM Comment WHERE @tag = ANY(Hashtags)", new { tag });

            return _mapper.Map<IEnumerable<CommentDTO>>(comments);
        }

        public async Task<int> CreateComment(CreateCommentDTO comment)
        {
            using var connection = _context.GetConnection();

            var id = await connection.QueryFirstAsync<int>(
                "INSERT INTO Comment (UserId, PostId, Text, HashTags)" +
                "VALUES (@UserId, @PostId, @Text, @HashTags)" +
                "RETURNING Id",
                new {
                    comment.UserId, comment.PostId, comment.Text, comment.Hashtags,
                }
            );

            return id;
        }

        public async Task<bool> DeleteComment(int id)
        {
            using var connection = _context.GetConnection();

            var affected = await connection.ExecuteAsync(
                 "DELETE FROM Comment WHERE Id = @Id",
                new { Id = id }
            );

            return affected != 0;
        }

        public async Task<bool> UpdateComment(UpdateCommentDTO comment)
        {
            using var connection = _context.GetConnection();

            var affected = await connection.ExecuteAsync(
                "UPDATE Comment SET UserId = @UserId, PostId = @PostId," +
                "Text = @Text, Hashtags = @Hashtags" +
                "CreatedAt = @CreatedAt WHERE Id = @Id",
                new {
                    comment.UserId, comment.PostId, comment.Text,
                    comment.Hashtags, comment.CreatedAt,
                    comment.Id,
                }
            );

            return affected != 0;
        }
    }
}
