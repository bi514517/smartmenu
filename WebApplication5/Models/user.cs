using System;
using System.Collections.Generic;
using System.Data;
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
            String sql = "SELECT UserName,FullName,Password FROM Users WHERE UserName = '" + username + "'";
            DataTable data = Database.excuteQuery(sql);
            if (data.Rows.Count > 0)
            {
                UserName = data.Rows[0]["UserName"].ToString();
                FullName = data.Rows[0]["FullName"].ToString();
                Password = data.Rows[0]["Password"].ToString();
            }
        }
        public Boolean islogged()
        {
            String sql = "SELECT * FROM Users WHERE UserName = '" + UserName + "'";
            DataTable data = Database.excuteQuery(sql);
            Boolean tmp = data.Rows.Count > 0;
            return true;
        }
    }

}
