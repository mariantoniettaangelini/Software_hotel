using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data.Common;

namespace Software_hotel.Services
{
    public class SqlServiceSvcBase : ServiceBase
    {
        private readonly IConfiguration _config;
        public SqlServiceSvcBase(IConfiguration config)
        {
            _config = config;
        }
        protected override DbConnection CreateConnection()
        {
            return new SqlConnection(_config.GetConnectionString("Db"));
        }

        protected override DbCommand GetCommand(string commandText, DbConnection connection)
        {
            return new SqlCommand(commandText, connection as SqlConnection);
        }
    }
}
