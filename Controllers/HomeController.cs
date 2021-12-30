using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Crud_Operation.Models;

namespace Crud_Operation.Controllers
{
    public class HomeController : Controller
    {
        public readonly string Username = "";

        public HomeController()
        {
            try
            {
                if (System.Web.HttpContext.Current.Session["UserId"] != null)
                {
                    int id = Convert.ToInt32(System.Web.HttpContext.Current.Session["UserId"]);                    
                    
                    ViewData["Username"] = System.Web.HttpContext.Current.Session["UserName"].ToString();
                }
                
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                ViewData["Username"] = "";
            }
            
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

    }
}