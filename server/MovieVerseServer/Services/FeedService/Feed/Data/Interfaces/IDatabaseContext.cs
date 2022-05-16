using Npgsql;

namespace Feed.Data
{
    public interface IDatabaseContext
    {
        NpgsqlConnection GetConnection();
    }
}
