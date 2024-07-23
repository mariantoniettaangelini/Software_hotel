using System.Data.Common;

namespace Software_hotel.Services
{
    public abstract class ServiceBase
    {
        protected abstract DbConnection CreateConnection();
        protected abstract DbCommand GetCommand(string commandText, DbConnection connection);
    }
}
