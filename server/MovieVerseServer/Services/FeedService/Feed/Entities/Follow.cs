using System;
namespace Feed.Entities
{
    public class Follow
    {
        public int FollowId { get; set; }
        public DateTime CreatedDate { get; set; }
        public string FollowFromUserId { get; set; }
        public string FollowToUserId { get; set; }

    }
}
