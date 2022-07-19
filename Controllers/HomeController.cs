using Hello_Travellers.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Hello_Travellers.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            Entities db = new Entities();
            ViewBag.Posts = db.Posts.ToList();
            ViewBag.MediaItems = db.MediaItems.ToList();
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