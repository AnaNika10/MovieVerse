using System;
namespace Feed1.Entities
{
    public class Follow
    {
        public int FollowId { get; set; }
        public DateTime CreatedDate { get; set; }
        public string FollowFromUserId { get; set; }
        public string FollowToUserId { get; set; }

    }
}
