using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using WebApplication5.Controllers;

namespace WebApplication5.Models
{
    public class user
    {
        public String UserName, FullName, Password;

        public user(string userName, string fullName, string password)
        {
            UserName = userName;
            FullName = fullName;
            Password = password;
        }
        public user(string username)
        {
            String sql = "SELECT * FROM Users WHERE UserName = '" + username + "'";
            Database.cnn.Open();
            SqlDataReader data = Database.excuteQuery(sql);
            while (data.Read())
            {
                UserName = data.GetValue(0).ToString();
                FullName = data.GetValue(1).ToString();
                Password = data.GetValue(1).ToString();
            }
            Database.cnn.Close();
        }
        public Boolean islogged()
        {
            String sql = "SELECT * FROM Users WHERE UserName = '" + UserName + "'";
            Database.cnn.Open();
            SqlDataReader data = Database.excuteQuery(sql);
            Boolean tmp = data.HasRows;
            Database.cnn.Close();
            return tmp;
        }
    }

}
