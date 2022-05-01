using Microsoft.EntityFrameworkCore;
using Notification.Application.Contracts.Persistence;
using Notification.Domain.Entities;
using Notification.Infrastructure.Persistence.EntityConfigurations;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Notification.Infrastructure.Persistence
{
    public class NotificationContext : DbContext
    {
        public NotificationContext(DbContextOptions<NotificationContext> options) : base(options)
        {
        }
        public DbSet<UserNotification> Notifications { get; set; }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            foreach (var entry in ChangeTracker.Entries<UserNotification>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedBy = "rs2";
                        entry.Entity.CreatedDate = DateTime.Now;
                        break;
                    case EntityState.Modified:
                        entry.Entity.LastModifiedBy = "rs2";
                        entry.Entity.LastModifiedDate = DateTime.Now;
                        break;
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new NotificationEntityTypeConfiguration());

            base.OnModelCreating(modelBuilder);
        }
    }
}
