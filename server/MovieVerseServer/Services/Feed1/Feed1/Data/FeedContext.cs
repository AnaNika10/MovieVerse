using Microsoft.Extensions.Configuration;
using Npgsql;
using System;

namespace Feed1.Data
{
    public class FeedContext : IFeedContext
    {

        private readonly IConfiguration _configuration;

        public FeedContext(IConfiguration configuration)
        {
            this._configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public NpgsqlConnection GetConnection()
        {
            return new NpgsqlConnection(_configuration.GetValue<string>("DatabaseSettings:ConnectionString"));
        }
    }
}
