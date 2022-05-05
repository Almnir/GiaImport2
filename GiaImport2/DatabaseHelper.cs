using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GiaImport2
{
    public class DatabaseHelper
    {
        public static bool CheckConnection()
        {
            bool result = false;
            SqlConnection connection = null;
            try
            {
                using (connection = new SqlConnection(Globals.GetConnection()))
                {
                    var query = "select 1";
                    var command = new SqlCommand(query, connection);
                    connection.Open();
                    command.ExecuteScalar();
                    result = true;
                }
            }
            catch (Exception)
            {
                result = false;
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                    connection = null;
                }
            }
            return result;
        }

    }
}
