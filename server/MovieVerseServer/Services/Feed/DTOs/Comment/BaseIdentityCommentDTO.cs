using System;

namespace Feed.DTOs.Comment
{
    public class BaseIdentityLikeDTO : BaseLikeDTO
    {
        public int Id { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
