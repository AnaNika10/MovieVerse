using Npgsql;

namespace Feed1.Data
{
    public interface IFeedContext
    {

        NpgsqlConnection GetConnection();

    }
}
