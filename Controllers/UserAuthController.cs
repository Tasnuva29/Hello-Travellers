using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Hello_Travellers.Models;

namespace Hello_Travellers.Controllers
{
    public class UserAuthController : Controller
    {
        // GET
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(FormCollection form)
        {
            var email = form["Email"];
            var password = form["Password"];
            Entities db = new Entities();
            List<User> users = db.Users.Where(x => x.Email.Equals(email) && x.Password.Equals(password)).ToList();
            if(users.Count > 0)
            {
                Session["Username"] = users[0].Username;
                Response.Redirect("~/Home/Index");
            }
            return View();
        }

        public ActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        public ActionResult SignUp(User user)
        {
            Entities db = new Entities();
            db.Users.Add(user);
            db.SaveChanges();
            db.Dispose();
            return View(user);
        }

        public ActionResult SignOut()
        {
            Session["Username"] = null;
            Response.Redirect("~/Home/Index");
            return View();
        }
    }
}