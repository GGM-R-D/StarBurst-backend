using System.Data;
using Microsoft.Data.SqlClient;   // ← change this namespace

namespace SlotsGameEngine.BL.Helpers
{
    public class DBHelper
    {
        private readonly string _connectionString;
        public DBHelper(string connectionString) => _connectionString = connectionString;

        public IDbConnection CreateConnection()
            => new SqlConnection(_connectionString); // Microsoft.Data.SqlClient.SqlConnection
    }
}