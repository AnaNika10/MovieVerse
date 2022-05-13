using System;

namespace Feed.DTOs.Comment
{
    public class BaseCommentDTO
    {
        public string UserId { get; set; }
        public int PostId { get; set; }
        public string Text { get; set; }
        public string[] Hashtags { get; set; }
    }
}
