using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebApplication5.Models;

namespace WebApplication5.Controllers
{
    [Route("/")]
    [ApiController]
    public class HomeController : Controller
    {
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
        public IActionResult Index()
        {
           
            ViewBag.topFoods = getTopFood();
            ViewBag.foods = getFoods();
            ViewBag.foodTypes = getFoodTypes();
       
            return PartialView("Views/luigis/index.cshtml");
        }

        ArrayList getTopFood()
        {
            string value = "";
            if (!string.IsNullOrEmpty(Request.Query["search"]))
            {
                value = Request.Query["search"].ToString();
            }

            cnn.Open();
            string sql = "Select Food.Id,Food.FoodName,Food.Price," +
               "Food.Image,Food.Description,Food.FoodTypeId,FoodType.TypeName " +
               "from Food left join FoodType on Food.FoodTypeId = FoodType.Id " +
               "where Food.FoodName like '%"+ value + "%'";

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


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
