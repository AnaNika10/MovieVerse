using System;

namespace Feed.Entities
{
    public class Like
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int PostId { get; set; }
        public int CommentId { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
