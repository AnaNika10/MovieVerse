using System;

namespace Notification.Domain.Entities
{
    public class UserNotification
    {
        public int Id { get; protected set; }
        public string Type { get; set; }
        public bool IsRead { get; set; }
        public string PostCommentId { get; set; }
        public string PostLikeId { get; set; }
        public string FollowId { get; set; }
        public string UserId { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime? LastModifiedDate { get; set; }

        public UserNotification(int id, string type, bool isRead, string postCommentId, string postLikeId, string followId, string userId)
        {
            Id = id;
            Type = type;
            IsRead = isRead;
            PostCommentId = postCommentId ?? throw new ArgumentNullException(nameof(postCommentId));
            PostLikeId = postLikeId ?? throw new ArgumentNullException(nameof(postLikeId));
            FollowId = followId ?? throw new ArgumentNullException(nameof(followId));
            UserId = userId ?? throw new ArgumentNullException(nameof(userId));
        }
    }
}
