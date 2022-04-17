using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;

namespace Notification.API.Data
{
    public class NotificationContext : INotificationContext
    {
        public NotificationContext(IConfiguration configuration)
        {
            var connection = new SqlConnection(configuration.GetValue<string>("DatabaseSettings:ConnectionString"));
            connection.Open();
        }
        public SqlConnection Connection => throw new NotImplementedException();
    }
}