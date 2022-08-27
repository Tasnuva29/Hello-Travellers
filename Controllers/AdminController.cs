using Hello_Travellers.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Hello_Travellers.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin
        public ActionResult Index()
        {

            return View();

        }
        public ActionResult Dashboard()
        {
            return View();
        }
        public ActionResult Users()
        {
            Entities db = new Entities();
            var userList = db.Users.Where(temp => temp.Rank == "USERS").ToList();
            var count = db.Users.Where(temp => temp.Rank == "USERS").Count();
            ViewBag.userList = userList;
            ViewBag.count = count;
            // return Json(new { data = userList}, JsonRequestBehavior.AllowGet);
            return View();
        }
        public ActionResult Admins()
        {
            return View();
        }
        public ActionResult Posts()
        {
            return View();
        }
        public ActionResult Forums()
        {
            return View();
        }
        public ActionResult Reports()
        {
            return View();
        }
        public ActionResult Settings()
        {
            return View();
        }
    }
}