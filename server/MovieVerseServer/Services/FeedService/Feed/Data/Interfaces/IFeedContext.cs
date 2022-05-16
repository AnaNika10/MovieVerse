using Npgsql;

namespace Feed.Data
{
    public interface IFeedContext
    {

        NpgsqlConnection GetConnection();

    }
}
