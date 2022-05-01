using Notification.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Notification.Application.Contracts.Persistence
{
    public interface INotificationRepository
    {
        Task<UserNotification> Get(int id);
        Task<IEnumerable<UserNotification>> GetAll();
        Task Add(UserNotification notification);
        Task Delete(int id);
        Task Update(UserNotification notification);
    }
}
