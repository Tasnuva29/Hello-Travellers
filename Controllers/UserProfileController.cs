using Hello_Travellers.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Hello_Travellers.Controllers
{
    public class UserProfileController : Controller
    {
        Entities db = new Entities();
        // GET: UserProfile
        public ActionResult Index()
        {
            Entities db = new Entities();
            string currentUser = "bThor";
            List<User> user = db.Users.Where(temp => temp.Username == currentUser).ToList();
            ViewBag.user = user[0];
            return View();
        }

        [HttpPost]
        public ActionResult EditProfile([Bind(Include = "Name, Email, About, DisplayPictureName")] User user)
        {
            Entities db = new Entities();
            User existingUser = db.Users.Where(temp => temp.Username == "bThor").FirstOrDefault();
            if (ModelState.IsValid)
            {
                existingUser.Name = user.Name;
                existingUser.Email = user.Email;
                existingUser.About = user.About;
                existingUser.DisplayPictureName = user.DisplayPictureName;
                // existingUser.Password = user.Password;                     
                db.SaveChanges();
            }
            return PartialView(user);
        }

        public ActionResult messages()
        {
            return View();
        }
        [HttpPost]
        public ActionResult messages(string messages,User user, Message message)
        {
           
            
            var username = db.Users.Where(temp => temp.Username == user.Username);

            Message add = new Message { Content = messages,Sender = (User)username};
            return View();
        }


    }
}