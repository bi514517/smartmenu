using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebApplication5.Models;

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

        [Route("food-create")]
        [HttpGet, HttpPost]
        public IActionResult createFood()
        {
            if (HttpContext.Request.Method == HttpMethod.Post.Method)
            {
                Database.cnn.Open();
                string sql = "INSERT INTO Food " +
                    "(FoodName, Price, Image, Description, FoodTypeId)" +
                    "VALUES ('" + Request.Form["FoodName"] + "', '" + Request.Form["Price"] + "', " +
                    "'" + Request.Form["Image"] + "','" + Request.Form["Description"] + "'," +
                    "'" + Request.Form["FoodTypeId"] + "')";
                Database.excuteQuery(sql);
                Database.cnn.Close();
                return Redirect("~/admin/food-create");
            }
            else
            {
                int Id = 1;
                if (!string.IsNullOrEmpty(Request.Query["id"]) && int.TryParse("123", out Id))
                {
                    Database.cnn.Open();
                    string sql1 = "Select Food.Id,Food.FoodName,Food.Price," +
                        "Food.Image,Food.Description,Food.FoodTypeId,FoodType.TypeName " +
                        "from Food left join FoodType on Food.FoodTypeId = FoodType.Id " +
                        "where Food.Id = '" + Request.Query["id"] + "'";
                    SqlDataReader dataReader1 = Database.excuteQuery(sql1);
                    while (dataReader1.Read())
                    {
                        string foodId = dataReader1.GetValue(0).ToString();
                        string foodName = dataReader1.GetValue(1).ToString();
                        int price = dataReader1.GetInt32(2);
                        string image = dataReader1.GetValue(3).ToString();
                        string description = dataReader1.GetValue(4).ToString();
                        int foodTypeId = dataReader1.GetInt32(5);
                        string foodTypeName = dataReader1.GetValue(6).ToString();
                        foodType foodType = new foodType(foodTypeId, foodTypeName);
                        food food = new food(foodId, foodName, price, image, description, foodType);
                        ViewBag.food = food;
                    }
                    Database.cnn.Close();

                }
                Database.cnn.Open();
                string sql = "select Id,TypeName from foodType";
                ArrayList foodTypes = new ArrayList();
                SqlDataReader dataReader = Database.excuteQuery(sql);
                while (dataReader.Read())
                {
                    int foodTypeId = dataReader.GetInt32(0);
                    string foodTypeName = dataReader.GetValue(1).ToString();
                    foodType foodType = new foodType(foodTypeId, foodTypeName);
                    foodTypes.Add(foodType);
                }
                ViewBag.foodTypes = foodTypes;
                Database.cnn.Close();
                return PartialView("Views/CoolAdmin-master/createFood.cshtml");
            }
        }


        [Route("food-managerment")]
        [HttpGet]
        public IActionResult foodList()
        {
            if(new user(Request.Cookies["user"]).islogged())
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

        [Route("food-delete")]
        [HttpGet]
        public String deleteFood()
        {
            int Id = 1;
            if (!string.IsNullOrEmpty(Request.Query["id"]) && int.TryParse("123", out Id))
            {
                Database.cnn.Open();
                Database.excuteQuery("DELETE FROM food_weather WHERE food_weather.FoodId = '" + Request.Query["id"] + "'");
                Database.cnn.Close();
                Database.cnn.Open();
                Database.excuteQuery("DELETE FROM Food WHERE Food.Id = '" + Request.Query["id"] + "'");
                Database.cnn.Close();
                return "success";
            }
            return "fail";
        }

        ArrayList getFoods(int page)
        {
            int amountOfPage = 20;
            int skip = amountOfPage * (page -1);
            Database.cnn.Open();
            string sql = "Select Food.Id,Food.FoodName,Food.Price," +
                "Food.Image,Food.Description,Food.FoodTypeId,FoodType.TypeName " +
                "from Food left join FoodType on Food.FoodTypeId = FoodType.Id " +
                "ORDER BY Food.Id " +
                "OFFSET "+skip+" ROWS FETCH NEXT "+amountOfPage+" ROWS ONLY ";
            ArrayList foods = new ArrayList();
            SqlDataReader dataReader = Database.excuteQuery(sql);
            while (dataReader.Read())
            {
                string foodId = dataReader.GetValue(0).ToString();
                string foodName = dataReader.GetValue(1).ToString();
                int price = dataReader.GetInt32(2);
                string image = dataReader.GetValue(3).ToString();
                string description = dataReader.GetValue(4).ToString();
                int foodTypeId = dataReader.GetInt32(5);
                string foodTypeName = dataReader.GetValue(6).ToString();
                foodType foodType = new foodType(foodTypeId, foodTypeName);
                food food = new food(foodId, foodName, price, image, description, foodType);
                foods.Add(food);
            }
            Database.cnn.Close();
            return foods;
        }

        ArrayList getFoodTypes(int page)
        {
            int amountOfPage = 20;
            int skip = amountOfPage * (page - 1);
            Database.cnn.Open();
            string sql = "Select Food.Id,Food.FoodName,Food.Price," +
                "Food.Image,Food.Description,Food.FoodTypeId,FoodType.TypeName " +
                "from Food left join FoodType on Food.FoodTypeId = FoodType.Id " +
                "ORDER BY Food.Id " +
                "OFFSET " + skip + " ROWS FETCH NEXT " + amountOfPage + " ROWS ONLY ";
            ArrayList foods = new ArrayList();
            SqlDataReader dataReader = Database.excuteQuery(sql);
            while (dataReader.Read())
            {
                string foodId = dataReader.GetValue(0).ToString();
                string foodName = dataReader.GetValue(1).ToString();
                int price = dataReader.GetInt32(2);
                string image = dataReader.GetValue(3).ToString();
                string description = dataReader.GetValue(4).ToString();
                int foodTypeId = dataReader.GetInt32(5);
                string foodTypeName = dataReader.GetValue(6).ToString();
                foodType foodType = new foodType(foodTypeId, foodTypeName);
                food food = new food(foodId, foodName, price, image, description, foodType);
                foods.Add(food);
            }
            Database.cnn.Close();
            return foods;
        }

        [Route("order-managerment")]
        [HttpGet]
        public IActionResult oderList()
        {
            return PartialView("Views/CoolAdmin-master/index.cshtml");
        }
    }
}