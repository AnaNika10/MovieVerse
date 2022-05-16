using Microsoft.Extensions.Configuration;
using Npgsql;
using System;

namespace Feed.Data
{
    public class DatabaseContext : IDatabaseContext
    {
        protected IConfiguration _configuration;

        public DatabaseContext(IConfiguration config)
        {
            _configuration = config ?? throw new ArgumentNullException(nameof(config));
        }

        public NpgsqlConnection GetConnection()
        {
            return new NpgsqlConnection(_configuration.GetValue<string>("DatabaseSettings:ConnectionString"));
        }
    }
}
