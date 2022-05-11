using System;

namespace Feed.Entities
{
    public class Like
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string PostId { get; set; }
        public string CommentId { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
