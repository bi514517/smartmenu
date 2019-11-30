using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using WebApplication5.Models;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Http;
using DotNet.Cookies;

namespace WebApplication5.Controllers
{
    [Route("account")]
    [ApiController]
    public class AccountController : Controller
    {
        [Route("login")]
        [HttpPost, HttpGet]
        public IActionResult Login()
        {
            if (HttpContext.Request.Method == HttpMethod.Post.Method)
            { 
                String user = Request.Form["user"];
                String pass = Request.Form["pass"];
                String a = Request.Form["remember"];
                bool remember = false;
                if (a != null && a.Equals("on"))
                {
                    remember = true;
                }
                if (checkUserExist(user))
                {
                    saveUser(remember, user, pass);
                    return Redirect("~/admin/");
                }
                else
                {
                    TempData["message"] = "wrong !!!";
                    return RedirectToAction("Login");
                }

            }
            else

            {
                if (Request.Cookies["user"] != null )
                {
                    String user = Request.Cookies["user"];
                    String pass = Request.Cookies["pass"];
                    if (checkUserExist(user))
                    {
                        saveUser(true,user, pass);
                        return Redirect("~/admin/");
                    }
                    else
                    {
                        logoutCookie();
                        return PartialView("Views/CoolAdmin-master/login.cshtml");
                    }
                }
                else
                {
                    return PartialView("Views/CoolAdmin-master/login.cshtml");
                }
            }

        }
        private bool checkUserExist(string username)
        {
            String sql = "SELECT * FROM Users WHERE UserName = '" + username + "'";
            Database.cnn.Open();
            SqlDataReader data = Database.excuteQuery(sql);
            Boolean tmp = data.HasRows;
            Database.cnn.Close();
            return tmp;
        }
        public void saveUser(Boolean remember,string username, string password,string fullname = "",Boolean isCreate = false)
        {
            if (isCreate)
            {
                String sql = "INSERT INTO Users (UserName, FullName, Password) " +
                    " VALUES('"+username+"', '"+password+"', '"+fullname+"')";
                Database.cnn.Open();
                Database.excuteQuery(sql);
                Database.cnn.Close();
            }
            CookieOptions opt = new CookieOptions();
            Response.Cookies.Append("user", username, opt);
            Response.Cookies.Append("pass", password, opt);
        }

        public void logoutCookie()
        {
            if (Request.Cookies["user"] != null || Request.Cookies["pass"] != null)
            {
                Response.Cookies.Delete("user");
                Response.Cookies.Delete("pass");
            }
        }

        [Route("logout")]
        [HttpGet]
        public IActionResult logout()
        {
            logoutCookie();
            return Redirect("~/account/login");
        }

        [Route("register")]
        [HttpGet,HttpPost]
        public IActionResult register()
        {
            if (HttpContext.Request.Method == HttpMethod.Post.Method)
            {
                String user = Request.Form["user"];
                String pass = Request.Form["pass"];
                String fullname = Request.Form["fullname"];
                bool remember = Boolean.Parse(Request.Form["remember"]);
                if (!checkUserExist(user))
                {
                    saveUser(remember, user, pass,fullname,true);
                    return Redirect("~/admin/");
                }
                else
                {
                    TempData["message"] = "this account is already exist !!!";
                    return RedirectToAction("register");
                }
            }
            return PartialView("Views/CoolAdmin-master/register.cshtml");
        }
    }
}