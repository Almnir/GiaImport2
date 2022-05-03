using System.Data.SqlClient;

namespace GiaImport2.Services
{
    public interface IDatabase
    {
        SqlConnection GetConnection();
    }
}
