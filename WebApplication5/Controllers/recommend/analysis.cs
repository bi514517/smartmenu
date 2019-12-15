using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using WebApplication5.Controllers;
using static smartmenu.Models.order;
using WebApplication5.Models;

namespace smartmenu.Controllers.recommend
{
    public class analysis
    {
        public static ArrayList getData()
        {
            var sql = "select * from orderItem";
            ArrayList orderItemList = new ArrayList();
            DataTable dt = Database.excuteQuery(sql);
            foreach (DataRow row in dt.Rows)
            {
                string foodId = row["foodId"].ToString();
                int amount = 0; Int32.TryParse(row["amount"].ToString(), out amount);
                int weatherId = 0; Int32.TryParse(row["weatherId"].ToString(), out weatherId);
                food food = new food(foodId);
                orderItem orderItem = new orderItem(food, amount, weatherId);
            }
            return orderItemList;
        }
        public static void clearData()
        {
            var sql = "DELETE FROM [food_weather] ";
            ArrayList orderItemList = new ArrayList();
            DataTable dt = Database.excuteQuery(sql);
        }

        public static void startAnalys()
        {
            clearData();
            ArrayList orderItemList = getData();
            foreach(orderItem item in orderItemList)
            {
                var sql = "select * from food_weather where foodId = '"+ item.food.foodId + "' and weatherId = '"+ item.weatherCode +"'";
                if(Database.excuteQuery(sql).Rows.Count == 0)
                {
                    sql = "insert into food_weather  set foodId = '" + item.food.foodId + "' and weatherId = '" + item.weatherCode + "'";

                }
            }
        }

    }
}
