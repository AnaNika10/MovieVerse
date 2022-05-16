using Dapper;
using Feed1.Data;
using Feed1.DTOs;
using System;
using Feed.Entities;
using System.Threading.Tasks;
using AutoMapper;
using System.Collections.Generic;
using System.Linq;

namespace Feed1.Repositories
{
    public class PostRepository: IPostRepository
    {
        private readonly IFeedContext _context;
        private readonly IMapper _mapper;

        public PostRepository(IFeedContext context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<bool> CreatePost(CreatePostDTO postDTO)
        {  
            using var connection = _context.GetConnection();

          var affected =   await connection.ExecuteAsync(
                "insert into post (\"PostText\",\"UserId\",\"CreatedDate\") values(@Text, @UserId, @CreatedAt)",
                new { Text = postDTO.PostText, UserId = postDTO.UserId, CreatedAt = postDTO.CreatedDate } );
            if (affected == 0)
                return false;
            return true;
             
            
        }

        public async Task<PostDTO> GetPostByDateAndUser(DateTime CreatedDate, string UserId)
        {
            using var connection = _context.GetConnection();
            var post = await connection.QueryFirstOrDefaultAsync<Post>(
                 "SELECT * FROM post WHERE \"CreatedDate\" = @CreatedDate AND \"UserId\"=@UserId", new { CreatedDate = CreatedDate, UserId = UserId });

            return _mapper.Map<PostDTO>(post);
        }

        public async Task<IEnumerable<PostDTO>> GetPostByUser(string userId)
        {
            using var connection = _context.GetConnection();

            var posts = await connection.QueryAsync<Post>(
              "SELECT * FROM post WHERE \"UserId\"=@Id", new { Id = userId }

              );
            return _mapper.Map<IEnumerable<PostDTO>>(posts);
  

      
        }
        public async Task<IEnumerable<PostDTO>> GetAllPosts()
        {
           
         
            using var connection = _context.GetConnection();

            var posts = await connection.QueryAsync<Post>(
                "SELECT * FROM post"

                );

            return _mapper.Map<IEnumerable<PostDTO>>(posts);
          
        }
      

        public async Task<bool> UpdatePost(UpdatePostDTO postDTO)
        {
            using var connection = _context.GetConnection();

            var affected = await connection.ExecuteAsync(

                "UPDATE post SET \"PostText\"=@PostText, \"Hashtags\" = @Hashtags, \"FilesUrls\" = @FilesUrls WHERE \"PostId\" = @PostId",
                new { postDTO.PostText, postDTO.Hashtags, postDTO.FilesUrls, postDTO.PostId });

            if (affected == 0)
                return false;

            return true;
        }
        public async Task<bool> DeletePost(int postId)
        {
            using var connection = _context.GetConnection();

            var affected = await connection.ExecuteAsync(
                "DELETE FROM post WHERE \"PostId\" = @postId",
                new { postId });

            if (affected == 0)
                return false;

            return true;
        }

    }
}
