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
           
            cnn.Open();
            String sql = "Select * from Food";

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
                food food = new food(foodId,foodName,price,image,description);
                foods.Add(food);
            }
            ViewBag.foods = foods; 
            cnn.Close();
            return PartialView("Views/luigis/index.cshtml");
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
