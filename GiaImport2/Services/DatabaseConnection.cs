using System.Data.SqlClient;

namespace GiaImport2.Services
{
    internal class DatabaseConnection : IDatabase
    {
        public SqlConnection GetConnection()
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
            builder.DataSource = @"DESKTOP-RH75GMH\SQLEXPRESS";
            builder.InitialCatalog = "datatest";
            builder.IntegratedSecurity = false;
            builder.UserID = "sa";
            builder.Password = "Njkmrjcdjb";
            SqlConnection con = new SqlConnection(builder.ConnectionString);
            return con;
        }
    }
}
