using Microsoft.Data.SqlClient;

namespace Notification.API.Data
{
    public interface INotificationContext
    {
        SqlConnection Connection { get; }
    }
}
