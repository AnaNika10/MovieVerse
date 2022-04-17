using Notification.API.Entities;

namespace Notification.API.DTOs
{
    public class BaseNotificationDTO
    {
        public NotificationType Type { get; set; }
        public bool IsRead { get; set; }
        public string PostCommentId { get; set; }
        public string PostLikeId { get; set; }
        public string FollowId { get; set; }
        public string UserId { get; set; }
    }
}
