using System;

namespace Feed.DTOs.Like
{
    public class BaseIdentityLikeDTO : BaseLikeDTO
    {
        public int Id { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
