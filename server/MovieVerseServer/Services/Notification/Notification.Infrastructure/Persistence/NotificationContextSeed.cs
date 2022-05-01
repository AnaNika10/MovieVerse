using Microsoft.Extensions.Logging;
using Notification.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notification.Infrastructure.Persistence
{
    public class NotificationContextSeed
    {
        public static async Task SeedAsync(NotificationContext notificationContext, ILogger<NotificationContextSeed> logger)
        {
            if(!notificationContext.Notifications.Any())
            {
                notificationContext.Notifications.AddRange(GetPreconfiguredNotifications());
                await notificationContext.SaveChangesAsync();
                logger.LogInformation("Seeding database associated with context {DbContextName}", nameof(NotificationContext));
            }
        }

        private static IEnumerable<UserNotification> GetPreconfiguredNotifications()
        {
            var notification = new UserNotification(1, "FOLLOW", false, "gojko", "nesto", "ide", "opet");

            return new List<UserNotification> { notification };
        }
    }
}
