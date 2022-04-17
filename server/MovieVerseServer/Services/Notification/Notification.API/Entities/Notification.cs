using System;

namespace Notification.API.Entities
{
    public enum NotificationType
    {
        POST_COMMENT,
        POST_LIKE,
        FOLLOW
    }
    public class Notification
    {
        public int Id { get; protected set; }
        public NotificationType Type { get; set; }
        public bool IsRead { get; set; }
        public string PostCommentId { get; set; }
        public string PostLikeId { get; set; }
        public string FollowId { get; set; }
        public string UserId { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime? LastModifiedDate { get; set; }
    }
}
