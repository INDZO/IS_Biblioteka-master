using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IS_Biblioteka.DB
{
    public abstract class DBConnection
    {
        private readonly string connectionString;

        public DBConnection()
        {
            connectionString = "Server=(local); DataBase=Biblioteka; Integrated Security=true";
        }

        protected SqlConnection GetConnection()
        {
            return new SqlConnection(connectionString);
        }
    }
}
