using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Notification.Domain.Entities;

namespace Notification.Infrastructure.Persistence.EntityConfigurations
{
    public class NotificationEntityTypeConfiguration : IEntityTypeConfiguration<UserNotification>
    {
        public void Configure(EntityTypeBuilder<UserNotification> builder)
        {
            builder.ToTable("Notifications");
            builder.HasKey(o => o.Id);
            builder.Property(o => o.Id).UseHiLo("notificationsec");
        }
    }
}
