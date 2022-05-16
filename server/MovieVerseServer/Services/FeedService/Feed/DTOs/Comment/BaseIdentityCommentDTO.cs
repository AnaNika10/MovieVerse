using System;

namespace Feed.DTOs.Comment
{
    public class BaseIdentityCommentDTO : BaseCommentDTO
    {
        public int Id { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
