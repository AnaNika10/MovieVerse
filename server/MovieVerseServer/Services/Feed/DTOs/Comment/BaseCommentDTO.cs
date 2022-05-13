using System;

namespace Feed.DTOs.Comment
{
    public class BaseLikeDTO
    {
        public string UserId { get; set; }
        public int PostId { get; set; }
        public string Text { get; set; }
        public string[] Hashtags { get; set; }
        public int LikesNum { get; set; }
    }
}
