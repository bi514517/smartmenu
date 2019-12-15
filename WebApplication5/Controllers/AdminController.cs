using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using smartmenu.Models;
using WebApplication5.Models;
using static smartmenu.Models.order;

namespace WebApplication5.Controllers
{
    [Route("admin")]
    [ApiController]
    public class AdminController : Controller
    {

        [HttpGet]
        public IActionResult Index()
        {
            user usr = new user(Request.Cookies["user"]);
            if (usr.islogged())
            return PartialView("Views/CoolAdmin-master/index.cshtml");
            else return PartialView("Views/CoolAdmin-master/index.cshtml");
        }

        [Route("food-managerment")]
        [HttpGet]
        public IActionResult foodList()
        {
            var a = Request.Cookies["user"];
            if (new user(Request.Cookies["user"]).islogged())
            {
                int page = 1;
                if (!string.IsNullOrEmpty(Request.Query["page"]) && int.TryParse(Request.Query["page"], out page))
                {
                    page = Int32.Parse(Request.Query["page"].ToString());
                }
                ViewBag.foods = getFoods(page);
                return PartialView("Views/CoolAdmin-master/foodManagerment.cshtml");
            }
            return Redirect("~/account/login");
        }

        [Route("food-create")]
        [HttpGet, HttpPost]
        public IActionResult createFood()
        {

            if (HttpContext.Request.Method == HttpMethod.Post.Method)
            {
                String sql = "";
                if (string.IsNullOrWhiteSpace(Request.Form["Id"]))
                {
                    sql = "INSERT INTO Food " +
                    "(FoodName, Price, Image, Description, FoodTypeId,SizeId)" +
                    "VALUES ('" + Request.Form["FoodName"] + "', '" + Request.Form["Price"] + "', " +
                    "'" + Request.Form["Image"] + "','" + Request.Form["Description"] + "'," +
                    "'" + Request.Form["FoodTypeId"] + "','"+ Request.Form["SizeId"] + "')";
                }
                else
                {
                    sql = "Update Food SET ";
                    String[] Keys = { "FoodName","Price","Image", "Description", "FoodTypeId", "SizeId" };
                    foreach (String key in Keys)
                    {
                        if(Request.Form[key].ToString().Length > 0)
                            sql += key +" = '"+ Request.Form[key] + "',";
                    }
                    sql = sql.Substring(0,sql.Length -1) + " where Food.Id ='"+ Request.Form["Id"] + "'";
                }
                Database.excuteQuery(sql);
                return Redirect("~/admin/food-create");
            }
            else
            {
                int Id = 1;
                if (!string.IsNullOrEmpty(Request.Query["id"]) && int.TryParse("123", out Id))
                {
                    string sql1 = "Select Food.Id,Food.FoodName,Food.Price," +
                        "Food.Image,Food.Description,Food.FoodTypeId,FoodType.TypeName, " +
                        "foodSize.Id as foodSizeId,foodSize.sizeName as foodSizeName " +
                        "from Food " +
                        "left join FoodType on Food.FoodTypeId = FoodType.Id " +
                        "left join foodSize on Food.SizeId = foodSize.Id " +
                        "where Food.Id = '" + Request.Query["id"] + "'";
                    DataTable dataReader1 = Database.excuteQuery(sql1);
                    foreach (DataRow row in dataReader1.Rows)
                    {
                        string foodId = row["Id"].ToString();
                        string foodName = row["FoodName"].ToString();
                        int price = Int32.Parse(row["Price"].ToString());
                        string image = row["Image"].ToString();
                        string description = row["Description"].ToString();
                        int foodTypeId = 0;
                        Int32.TryParse(row["FoodTypeId"].ToString(),out foodTypeId);
                        string foodTypeName = row["TypeName"].ToString();
                        int foodSizeId = 0;
                        Int32.TryParse(row["foodSizeId"].ToString(),out foodSizeId);
                        string foodSizeName = row["foodSizeName"].ToString();
                        foodType foodType = new foodType(foodTypeId, foodTypeName);
                        foodSize foodSize = new foodSize(foodSizeId, foodSizeName);
                        food food = new food(foodId, foodName, price, image, description, foodType,foodSize);
                        ViewBag.food = food;
                    }
                }
                string sql = "select Id,TypeName from foodType";
                ArrayList foodTypes = new ArrayList();
                DataTable dataReader = Database.excuteQuery(sql);
                foreach (DataRow row in dataReader.Rows)
                {
                    int foodTypeId = Int32.Parse(row["Id"].ToString());
                    string foodTypeName = row["TypeName"].ToString();
                    foodType foodType = new foodType(foodTypeId, foodTypeName);
                    foodTypes.Add(foodType);
                }
                ViewBag.foodTypes = foodTypes;
                sql = "select Id,sizeName from foodSize";
                ArrayList foodSizes = new ArrayList();
                dataReader = Database.excuteQuery(sql);
                foreach (DataRow row in dataReader.Rows)
                {
                    int foodSizeId = Int32.Parse(row["Id"].ToString());
                    string foodSizeName = row["sizeName"].ToString();
                    foodSize foodType = new foodSize(foodSizeId, foodSizeName);
                    foodSizes.Add(foodType);
                }
                ViewBag.foodSizes = foodSizes;
                return PartialView("Views/CoolAdmin-master/createFood.cshtml");
            }
        }





        [Route("food-delete")]
        [HttpGet]
        public String deleteFood()
        {
            int Id = 1;
            if (!string.IsNullOrEmpty(Request.Query["id"]) && int.TryParse("123", out Id))
            {
                Database.excuteQuery("DELETE FROM food_weather WHERE food_weather.FoodId = '" + Request.Query["id"] + "'");
                Database.excuteQuery("DELETE FROM Food WHERE Food.Id = '" + Request.Query["id"] + "'");
                return "success";
            }
            return "fail";
        }

        ArrayList getFoods(int page)
        {
            int amountOfPage = 20;
            int skip = amountOfPage * (page -1);
            string sql = "Select Food.Id,Food.FoodName,Food.Price," +
                "Food.Image,Food.Description,Food.FoodTypeId,FoodType.TypeName " +
                "from Food left join FoodType on Food.FoodTypeId = FoodType.Id " +
                "ORDER BY Food.Id " +
                "OFFSET "+skip+" ROWS FETCH NEXT "+amountOfPage+" ROWS ONLY ";
            ArrayList foods = new ArrayList();
            DataTable dataReader = Database.excuteQuery(sql);
            foreach (DataRow row in dataReader.Rows)
            {
                string foodId = row["Id"].ToString();
                string foodName = row["FoodName"].ToString();
                int price = Int32.Parse(row["Price"].ToString());
                string image = row["Image"].ToString();
                string description = row["Description"].ToString();
                int foodTypeId = Int32.Parse(row["FoodTypeId"].ToString());
                string foodTypeName = row["TypeName"].ToString();
                foodType foodType = new foodType(foodTypeId, foodTypeName);
                food food = new food(foodId, foodName, price, image, description, foodType);
                foods.Add(food);
            }
            return foods;
        }

        ArrayList getFoodTypes(int page)
        {
            int amountOfPage = 20;
            int skip = amountOfPage * (page - 1);
            string sql = "Select Food.Id,Food.FoodName,Food.Price," +
                "Food.Image,Food.Description,Food.FoodTypeId,FoodType.TypeName " +
                "from Food left join FoodType on Food.FoodTypeId = FoodType.Id " +
                "ORDER BY Food.Id " +
                "OFFSET " + skip + " ROWS FETCH NEXT " + amountOfPage + " ROWS ONLY ";
            ArrayList foods = new ArrayList();
            DataTable dataReader = Database.excuteQuery(sql);
            foreach(DataRow row in dataReader.Rows)
            {
                string foodId = row["Id"].ToString();
                string foodName = row["FoodName"].ToString();
                int price = Int32.Parse(row["Price"].ToString());
                string image = row["Image"].ToString();
                string description = row["Description"].ToString();
                int foodTypeId = Int32.Parse(row["FoodTypeId"].ToString());
                string foodTypeName = row["TypeName"].ToString();
                foodType foodType = new foodType(foodTypeId, foodTypeName);
                food food = new food(foodId, foodName, price, image, description, foodType);
                foods.Add(food);
            }
            return foods;
        }

        [Route("order-managerment")]
        [HttpGet]
        public IActionResult orderList()
        {
            var a = Request.Cookies["user"];
            if (new user(Request.Cookies["user"]).islogged())
            {
                ViewBag.orders = getOrder();
                return PartialView("Views/CoolAdmin-master/orderManagerment.cshtml");
            }
            
            return Redirect("~/account/login");
        }

        [Route("order-notify")]
        [HttpGet]
        public IActionResult orderNotify()
        {
                ViewBag.orders = getOrder();
                ViewBag.count = ViewBag.orders.Count;
                return PartialView("Views/CoolAdmin-master/notify.cshtml");
        }

        ArrayList getOrder()
        {
            string sql = "select orders.id as orderId,orders.tableIp,orders.time,[table].name as tableName " +
                "from orders " +
                "inner join [table] on [table].[IP] = orders.tableIp " +
                "where approve IS NULL or approve = 0";
            ArrayList orderList = new ArrayList();
            DataTable dataReader = Database.excuteQuery(sql);
            foreach (DataRow row in dataReader.Rows)
            {
                int orderId = Int32.Parse(row["orderId"].ToString());
                string tableIp = row["tableIp"].ToString();
                string tableName = row["tableName"].ToString();
                DateTime time = new DateTime();
                if (!(row["time"] is DBNull))
                    time = Convert.ToDateTime(row["time"]);
                order order = new order(orderId, new table(tableIp, tableName), time);
                orderList.Add(order);
            }
            for (int i = 0;i < orderList.Count;i++)
            {
                ArrayList orderItemList = new ArrayList();
                sql = "select Food.Id,Food.FoodName,Food.Price, " +
                "Food.Image,Food.Description,Food.FoodTypeId,FoodType.TypeName, " +
                "orderItem.amount ,orderItem.weatherId from orderItem " +
                "left join Food on Food.Id = orderItem.foodId  " +
                "left join foodType on [Food].[FoodTypeId] = [foodType].[Id]  " +
                "where orderId = '" + ((order)orderList[i]).id + "' " +
                "ORDER BY Food.Id ";
                dataReader = Database.excuteQuery(sql);
                foreach (DataRow row in dataReader.Rows)
                {
                    string foodId = row["Id"].ToString();
                    string foodName = row["FoodName"].ToString();
                    int price = 0; Int32.TryParse(row["Price"].ToString(),out price);
                    string image = row["Image"].ToString();
                    string description = row["Description"].ToString();
                    int foodTypeId = 0; Int32.TryParse(row["FoodTypeId"].ToString(),out foodTypeId);
                    string foodTypeName = row["TypeName"].ToString();
                    int amount = 0; Int32.TryParse(row["amount"].ToString(),out amount);
                    int weatherId = 0; Int32.TryParse(row["weatherId"].ToString(),out weatherId);
                    foodType foodType = new foodType(foodTypeId, foodTypeName);
                    food food = new food(foodId, foodName, price, image, description, foodType);
                    orderItem orderItem = new orderItem(food, amount, weatherId);
                    orderItemList.Add(orderItem);
                }
                ((order)orderList[i]).ordered = orderItemList;
            }
            return orderList;
        }

        [Route("order-delete")]
        [HttpGet]
        public String deleteOrder()
        {
            int Id = 1;
            if (!string.IsNullOrEmpty(Request.Query["id"]) && int.TryParse("123", out Id))
            {
                Database.excuteQuery("DELETE FROM orders WHERE orders.id = '" + Request.Query["id"] + "'");
                Database.excuteQuery("DELETE FROM orderItem WHERE orderItem.orderId = '" + Request.Query["id"] + "'");
                return "success";
            }
            return "fail";
        }


        [Route("order-approve")]
        [HttpGet]
        public String approveOrder()
        {
            int Id = 1;
            if (!string.IsNullOrEmpty(Request.Query["id"]) && int.TryParse("123", out Id))
            {
                Database.excuteQuery("UPDATE orders SET approve = '1' WHERE orders.id = '" + Request.Query["id"] + "'");
                return "success";
            }
            return "fail";
        }

        [Route("table-managerment")]
        [HttpGet]
        public IActionResult tableList()
        {
            var a = Request.Cookies["user"];
            if (new user(Request.Cookies["user"]).islogged())
            {
                ViewBag.tables = getTables();
                return PartialView("Views/CoolAdmin-master/tableManagerment.cshtml");
            }
            return Redirect("~/account/login");
        }

        private ArrayList getTables()
        {
            string sql = "Select [IP],[name] from [Table] ";
            ArrayList tables = new ArrayList();
            DataTable dataReader = Database.excuteQuery(sql);
            foreach (DataRow row in dataReader.Rows)
            {
                string IP = row["IP"].ToString();
                string name = row["name"].ToString();
                table food = new table(IP,name);
                tables.Add(food);
            }
            return tables;
        }

        [Route("table-edit")]
        [HttpPost]
        public String createTable()
        {
            if (HttpContext.Request.Method == HttpMethod.Post.Method)
            {
                var ip = Request.HttpContext.Connection.RemoteIpAddress;
                if (!string.IsNullOrEmpty(Request.Query["name"]))
                {
                    String name = Request.Query["name"];
                    string sql = "select * from [table] where [IP] ='" + ip + "'";
                    DataTable dataReader = Database.excuteQuery(sql);

                    if (dataReader.Rows.Count > 0)
                    {
                        sql = "UPDATE [table] SET " +
                            "[name]='" + name + "' where [IP]='" + ip + "'";
                    }
                    else
                    {
                        sql = "INSERT INTO [table] " +
                            "([IP],[name])" +
                            "VALUES ('" + ip + "','" + name + "')";
                    }
                    Database.excuteQuery(sql);
                }
                return "success";
            }
            else
            {
                return "notthing to show";
            }
        }


        [Route("table-delete")]
        [HttpGet]
        public IActionResult deleteTable()
        {
            return PartialView("Views/CoolAdmin-master/index.cshtml");
        }
    }
}