using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication5.Controllers
{
    public class Database
    {
        static SqlCommand command;
        static SqlDataReader dataReader;
        static string connetionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=smartmenu;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
        public static SqlConnection cnn = new SqlConnection(connetionString);
        public static SqlDataReader excuteQuery(String sql)
        {
            command = new SqlCommand(sql, cnn);
            dataReader = command.ExecuteReader();
            return dataReader;
        }
        public static int executeScalar(String sql)
        {
            command = new SqlCommand(sql, cnn);
            int id = Convert.ToInt32(command.ExecuteScalar());
            return id;
        }
    }
}
