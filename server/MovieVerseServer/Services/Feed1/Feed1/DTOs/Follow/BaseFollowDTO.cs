using System;
namespace Feed1.DTOs
{
    public class BaseFollowDTO
    {
        public DateTime CreatedDate { get; set; }
        public string FollowFromUserId { get; set; }
        public string FollowToUserId { get; set; }

    }
}
