using Microsoft.Data.SqlClient;

namespace FinstarTest
{
    public class SqlConnectionManager
    {
        public SqlConnection OpenSqlConnection()
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
            builder.DataSource = "N371-PC";
            builder.IntegratedSecurity = true;
            builder.InitialCatalog = "Finstar";
            builder.TrustServerCertificate = true;
            SqlConnection connection = new SqlConnection(builder.ConnectionString);
            return connection;
        }
    }
}