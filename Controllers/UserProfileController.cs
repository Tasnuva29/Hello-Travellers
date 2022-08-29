using Hello_Travellers.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Hello_Travellers.Controllers
{
    public class UserProfileController : Controller
    {
        // GET: UserProfile
        Entities db = new Entities();
        //public ActionResult Index()
        //{

        //    if (Session["Username"] != null)
        //    {

        //        var currUser = Session["Username"].ToString();
        //        List<User> user = db.Users.Where(temp => temp.Username == currUser).ToList();
        //        ViewBag.user = user[0];
        //        var posts = db.Posts.Where(temp => temp.CreatorUsername == currUser).ToArray();
        //        ViewBag.posts = posts;
        //        var mediaItems = new MediaItem[posts.Length];
        //        for (int i = 0; i < posts.Length; i++)
        //        {
        //            var currentID = posts[i].PostID;
        //            mediaItems[i] = db.MediaItems.Where(temp => temp.PostID == currentID).FirstOrDefault();
        //        }
        //        ViewBag.mediaItems = mediaItems;
        //        return View();
        //    }
        //    else
        //    {
        //        return RedirectToAction("Login", "UserAuth");
        //    }

        //}

        [HttpGet]
        public ActionResult Index(string Username)
        {


            if (Username == null && Session["Username"] != null)
            {
                Username = (string)Session["Username"];
            }
            if (Username == null && Session["Username"] == null)
            {
                return RedirectToAction("Login", "UserAuth");
            }

            var currUser = Username.ToString();
            var user = db.Users.Where(temp => temp.Username == currUser).FirstOrDefault();
            ViewBag.user = user;
            var posts = user.Posts.ToArray();
            ViewBag.posts = posts;
            var mediaItems = new MediaItem[posts.Length];
            for (int i = 0; i < posts.Length; i++)
            {
                var currentID = posts[i].PostID;
                mediaItems[i] = db.MediaItems.Where(temp => temp.PostID == currentID).FirstOrDefault();
            }
            ViewBag.mediaItems = mediaItems;
            return View();
        }

        public ActionResult EditProfile()
        {
            if (Session["Username"] != null)
            {
                var currUser = Session["Username"].ToString();
                User existingUser = db.Users.Where(temp => temp.Username == currUser).FirstOrDefault();
                ViewBag.user = existingUser;
                return View();
            }
            else
            {
                return RedirectToAction("Login", "UserAuth");
            }

        }

        [HttpPost]
        public ActionResult EditProfile(String Name, String Email, String PhoneNumber, String About, String Password, String ConfirmPassword, IEnumerable<HttpPostedFileBase> postedImages)
        {

            string path = Server.MapPath("~/Images/ProfilePicture/");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            Entities db = new Entities();
            var currUser = Session["Username"].ToString();
            User existingUser = db.Users.Where(temp => temp.Username == currUser).FirstOrDefault();
            existingUser.ConfirmPassword = existingUser.Password;
            ViewBag.user = existingUser;
            if (!String.IsNullOrEmpty(Name))
            {
                existingUser.Name = Name;
            }
            if (!String.IsNullOrEmpty(Email))
            {
                existingUser.Email = Email;
            }
            if (!String.IsNullOrEmpty(PhoneNumber))
            {
                existingUser.PhoneNumber = PhoneNumber;
            }
            if (!String.IsNullOrEmpty(About))
            {
                existingUser.About = About;
            }
            if (!String.IsNullOrEmpty(Password))
            {
                existingUser.Password = Password;
                existingUser.ConfirmPassword = existingUser.Password;
            }

            System.Diagnostics.Debug.WriteLine(postedImages.ToList().Count);

            if (postedImages != null)
            {
                
                    foreach (var image in postedImages)
                    {
                        var ext = Path.GetExtension(image.FileName);
                        image.SaveAs(path + existingUser.Username + ext);
                        existingUser.DisplayPictureName = existingUser.Username + ext;
                    }
                
            }
            if (ModelState.IsValid)
            {
                db.Entry(existingUser).State = EntityState.Modified;
                db.SaveChanges();
            }        

            return RedirectToAction("Index");
            //try
            //{
            //    if (ModelState.IsValid)
            //    {

            //    }
            //    else
            //    {
            //        return View();
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine(ex.Message);
            //    return View();
            //}


        }

        [HttpPost]
        public ActionResult CreateUserReport(String Reason, String Username)
        {
            try
            {
                Report report = new Report();
                report.ReporterUsername = (string)Session["Username"];
                report.Status = "UNRESOLVED";
                report.Reason = Reason;
                report.Context = "PROFILE";
                report.ContextID = Username;

                var existingReport = db.Reports.Where(t => t.ReporterUsername == report.ReporterUsername && t.ContextID == report.ContextID && t.Context == report.Context).Count();
                if (existingReport > 0)
                {
                    return Json("Exists", JsonRequestBehavior.AllowGet);
                }

                db.Reports.Add(report);
                db.SaveChanges();

                return Json("Success", JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json("Error", JsonRequestBehavior.AllowGet);
            }
        }

    }
}