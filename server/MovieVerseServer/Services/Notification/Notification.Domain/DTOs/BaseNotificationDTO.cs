using Notification.Domain.Entities;

namespace Notification.Domain.DTOs
{
    public class BaseNotificationDTO
    {
        public string Type { get; set; }
        public bool IsRead { get; set; }
        public string PostCommentId { get; set; }
        public string PostLikeId { get; set; }
        public string FollowId { get; set; }
        public string UserId { get; set; }
    }
}
