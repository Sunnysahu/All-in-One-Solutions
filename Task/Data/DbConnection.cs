using Microsoft.Data.SqlClient;

namespace Task.Data
{
    public class DbConnection
    {
        private readonly string _connectionString;

        public DbConnection(string connectionString) => _connectionString = connectionString;

        public SqlConnection CreateConnection() => new SqlConnection(_connectionString);
    }
}
