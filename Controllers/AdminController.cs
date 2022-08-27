﻿using Hello_Travellers.Models;
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
            var count = db.Users.Where(temp => temp.Rank == "USER").Count();
            var userList = db.Users.Where(temp => temp.Rank == "USER").ToList();
            ViewBag.userList = userList;
            ViewBag.count = count;
            // return Json(new { data = userList}, JsonRequestBehavior.AllowGet);
            return View();
        }
        /*  [HttpGet]
          public ActionResult ShowUsers()
          {          
              Entities db = new Entities();
              var userList = db.Users.Where(temp => temp.Rank == "USER").ToList();
              ViewBag.userList = userList;
              return View();            
          }*/
        public ActionResult Admins()
        {
            Entities db = new Entities();
            var count = db.Users.Where(temp => temp.Rank == "ADMIN").Count();
            ViewBag.count = count;
            var adminList = db.Users.Where(temp => temp.Rank == "ADMIN").ToList();
            ViewBag.adminList = adminList;
            return View();
        }
        public ActionResult Posts()
        {
            return View();
        }
        
        public ActionResult Subforums()
        {
            Entities db = new Entities();
            var count = db.Subforums.Count();
            ViewBag.count = count;
            var subForums = db.Subforums.ToList();
            ViewBag.subForums = subForums;
            return View();
        }

        [HttpPost]
        public ActionResult CreateSubforum(string ForumName)
        {
            Entities db = new Entities();
            var subForum = new Subforum();
            subForum.ForumName = ForumName;
            db.Subforums.Add(subForum);
            db.SaveChanges();
            return Json("true", JsonRequestBehavior.AllowGet);


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