using System;

namespace Feed.DTOs.Like
{
    public class BaseLikeDTO
    {
        public string UserId { get; set; }
        public int PostId { get; set; }
        public int CommentId { get; set; }
    }
}
