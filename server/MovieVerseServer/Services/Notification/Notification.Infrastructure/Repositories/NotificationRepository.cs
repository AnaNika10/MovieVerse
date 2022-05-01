using Microsoft.EntityFrameworkCore;
using Notification.Application.Contracts.Persistence;
using Notification.Domain.Entities;
using Notification.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Notification.Infrastructure.Repositories
{
    public class NotificationRepository : INotificationRepository
    {

        protected readonly NotificationContext _context;
        public NotificationRepository(NotificationContext context)
        {
            _context = context;
        }
        public async Task Add(UserNotification notification)
        {
            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var itemToRemove = await _context.Notifications.FindAsync(id);
            if (itemToRemove == null)
                throw new NullReferenceException();

            _context.Notifications.Remove(itemToRemove);
            await _context.SaveChangesAsync();
        }

        public async Task<UserNotification> Get(int id)
        {
            return await _context.Notifications.FindAsync(id);
        }

        public async Task<IEnumerable<UserNotification>> GetAll()
        {
            return await _context.Notifications.ToListAsync();
        }

        public async Task Update(UserNotification notification)
        {
            var itemToUpdate = await _context.Notifications.FindAsync(notification.Id);
            if (itemToUpdate == null)
                throw new NullReferenceException();
            itemToUpdate.IsRead = notification.IsRead;
            await _context.SaveChangesAsync();
        }
    }
}
