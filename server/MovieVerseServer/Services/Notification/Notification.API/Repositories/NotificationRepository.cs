using AutoMapper;
using Notification.API.Data;
using Notification.API.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Notification.API.Repositories
{
    public class NotificationRepository : INotificationRepository
    {

        private readonly INotificationContext _context;
        private readonly IMapper _mapper;

        public NotificationRepository(INotificationContext context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public Task CreateNotification(CreateNotificationDTO notificationDTO)
        {
            throw new System.NotImplementedException();
        }

        public Task DeleteNotificationByUserId(int userId)
        {
            throw new System.NotImplementedException();
        }

        public Task<IEnumerable<Entities.Notification>> GetNotifications()
        {
            throw new System.NotImplementedException();
        }
    }
}
