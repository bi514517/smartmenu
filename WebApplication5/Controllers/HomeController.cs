﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using WebApplication5.Models;

namespace WebApplication5.Controllers
{
    [Route("/")]
    [ApiController]
    public class HomeController : Controller
    {
        //location
        string weatherAPIUrl = "https://api.openweathermap.org/data/2.5/weather";
        string waetherAPIKey = "6448df283427548ea46ab0a6698d6218";
        private void addParams( String key, String value)
        {
            if (weatherAPIUrl.IndexOf("?") == -1)
                weatherAPIUrl+=  "?" + key + "=" + value;
            weatherAPIUrl+=  "&" + key + "=" + value;
        }
        //
        SqlCommand command;
        SqlDataReader dataReader;
        string connetionString;
        SqlConnection cnn;
        public HomeController()
        {
            connetionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=smartmenu;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            cnn = new SqlConnection(connetionString);
        }
        [HttpGet]
        public async Task<ActionResult> Index()
        {
            weather weather = await GetWeatherAsync();
            ViewBag.weather = weather;
            ViewBag.topFoods = getRcmdFood(weather.id);
            ViewBag.foods = getFoods();
            ViewBag.foodTypes = getFoodTypes();
            return PartialView("Views/luigis/index.cshtml");
        }

        ArrayList getRcmdFood(int weatherId)
        {
            string value = "";
            if (!string.IsNullOrEmpty(Request.Query["search"]))
            {
                value = Request.Query["search"].ToString();
            }

            cnn.Open();
            string sql = "Select Food.Id,Food.FoodName,Food.Price," +
               "Food.Image,Food.Description,Food.FoodTypeId,FoodType.TypeName " +
               "from Food " +
               "left join FoodType on Food.FoodTypeId = FoodType.Id ";
            if(value.Length > 0)
                sql+= "where Food.FoodName like '%" + value + "%' ";
            else
                sql += "left join food_weather on Food.Id = food_weather.foodId " +
                    "where food_weather.weatherId = " + weatherId;   
            command = new SqlCommand(sql, cnn);
            dataReader = command.ExecuteReader();
            ArrayList foods = new ArrayList();
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
            cnn.Close();
            return foods;
        }

        ArrayList getFoods()
        {
            cnn.Open();
            string sql = "Select Food.Id,Food.FoodName,Food.Price," +
                "Food.Image,Food.Description,Food.FoodTypeId,FoodType.TypeName " +
                "from Food left join FoodType on Food.FoodTypeId = FoodType.Id";

            command = new SqlCommand(sql, cnn);

            dataReader = command.ExecuteReader();
            ArrayList foods = new ArrayList();
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
            cnn.Close();
            return foods;
        }

        async Task<weather> GetWeatherAsync()
        {
            addParams("lat", "16.0608215");
            addParams("lon", "108.1941091");
            addParams("appid", waetherAPIKey);
            addParams("lang", "vi");
    
            weather weather=new weather();
            using (HttpClient clients = new HttpClient())
            {
                clients.BaseAddress = new Uri(weatherAPIUrl);
                clients.DefaultRequestHeaders.Accept.Clear();
                clients.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await clients.GetAsync(weatherAPIUrl);
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    JObject json = JObject.Parse(data);
                    int id = Int32.Parse((string)json["weather"][0]["id"]);
                    string describe = (string)json["weather"][0]["description"],
                        icon = (string)json["weather"][0]["icon"];
                    float temperature = float.Parse((string)json["main"]["temp"]),
                        humidity = float.Parse((string)json["main"]["humidity"]);
                    weather = new weather(id,describe, icon, temperature, humidity);
                }
            }
            return weather;
        }

        ArrayList getFoodTypes()
        {
            cnn.Open();
            string sql = "Select foodType.Id,foodType.TypeName from foodType";

            command = new SqlCommand(sql, cnn);

            dataReader = command.ExecuteReader();
            ArrayList foodTypes = new ArrayList();
            while (dataReader.Read())
            {
                int foodTypeId = dataReader.GetInt32(0);
                string foodTypeName = dataReader.GetValue(1).ToString();
                foodType foodType = new foodType(foodTypeId, foodTypeName);
                foodTypes.Add(foodType);
            }
            cnn.Close();
            return foodTypes;
        }

        [Route("about-us")]
        [HttpGet]
        public IActionResult AboutUs()
        {
            return PartialView("Views/luigis/02_about_us.cshtml");
        }

        [Route("menu")]
        [HttpGet]
        public IActionResult Menu()
        {
            return PartialView("Views/luigis/03_menu.cshtml");
        }

        [Route("blog")]
        [HttpGet]
        public IActionResult Blog()
        {
             return PartialView("Views/luigis/04_blog.cshtml");
        }

        [Route("contact")]
        [HttpGet]
        public IActionResult Contact()
        {
             return PartialView("Views/luigis/05_contact.cshtml");
        }

        [Route("elements")]
        [HttpGet]
        public IActionResult Elements()
        {
             return PartialView("Views/luigis/06_elements.cshtml");
        }

        [Route("food-create")]
        [HttpGet]
        public IActionResult createFood()
        {
            return PartialView("Views/luigis/CreateFood.cshtml");
        }

        [Route("ordered")]
        [HttpGet]
        public IActionResult ordered()
        {
            cnn.Open();
            string sql = "Select Food.Id,Food.FoodName,Food.Price," +
                "Food.Image,Food.Description,Food.FoodTypeId,FoodType.TypeName " +
                "from Food left join FoodType on Food.FoodTypeId = FoodType.Id ";
            String tam = "";
            string[] keys = Request.Cookies.Keys.ToArray();
            int id;
            for (int i = 0; i < Request.Cookies.Count; i++)
            {
                if (Request.Cookies[keys[i]] != null && int.TryParse(keys[i], out id))
                {
                    tam += "'" + id + "',";
                }
            }
            if (tam.Length > 0)
            {
                sql += "WHERE Food.Id IN (" + tam;
                sql = sql.Substring(0, sql.Length - 1) + ")";
            }
            else
            {
                sql += "WHERE 0 > 1";
            }

            command = new SqlCommand(sql, cnn);
            dataReader = command.ExecuteReader();
            ArrayList foods = new ArrayList();
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
            cnn.Close();
            ViewBag.foods = foods;
            return PartialView("Views/luigis/order.cshtml");
        }

        [Route("ordered")]
        [HttpPost]
        public String postOrdered()
        {
            var ip = Request.HttpContext.Connection.RemoteIpAddress;
            String sql = "INSERT INTO [orders] (tableIp) VALUES ('"+ip+ "') ;SELECT SCOPE_IDENTITY();";
            Database.cnn.Open();
            int orderId = Database.executeScalar(sql);
            Database.cnn.Close();
            sql = "INSERT INTO [orderItem] (orderId,foodId,amount) VALUES ";
            int count = 0;
            foreach(String key in Request.Form.Keys)
            {
                count++;
                sql += "('"+ orderId + "','"+key+"','"+Request.Form[key]+"')";
            }
            if (count > 0)
            {
                foreach (String key in Request.Form.Keys)
                {
                    Response.Cookies.Delete(key);
                }
                Database.cnn.Open();
                Database.excuteQuery(sql);
                Database.cnn.Close();
                return "success";
            }
            else return "fail";
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
