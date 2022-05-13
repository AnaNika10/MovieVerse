using System;

namespace Feed.Entities
{
    public class Comment
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int PostId { get; set; }
        public string Text { get; set; }
        public string[] Hashtags { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
