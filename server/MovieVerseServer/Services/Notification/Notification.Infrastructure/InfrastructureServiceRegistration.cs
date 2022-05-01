using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Notification.Application.Contracts.Persistence;
using Notification.Infrastructure.Persistence;
using Notification.Infrastructure.Repositories;

namespace Notification.Infrastructure
{
    public static class InfrastructureServiceRegistration
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<NotificationContext>(options => options.UseNpgsql(configuration.GetConnectionString("ConnectionString")));
            services.AddScoped<INotificationRepository, NotificationRepository>();

            return services;
        }
    }
}
