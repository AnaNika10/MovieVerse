using Notification.API.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Notification.API.Repositories
{
    public interface INotificationRepository
    {
        Task CreateNotification(CreateNotificationDTO notificationDTO);
        Task<IEnumerable<Entities.Notification>> GetNotifications();
        Task DeleteNotificationByUserId(int userId);
    }
}
