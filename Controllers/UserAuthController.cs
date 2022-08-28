using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Hello_Travellers.Models;

namespace Hello_Travellers.Controllers
{
    public class UserAuthController : Controller
    {
      

        static int number = new Random().Next(1000, 9999);
        static User newUser;
        // GET

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(FormCollection form)
        {
            if (Session["Username"] != null)
            {
                Response.Redirect("~/Home/Index");
            }

            Entities db = new Entities();
            var email = form["Email"];
            var password = form["Password"];
          
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
            
            var name = user.Name;
            var username = user.Username;
            var email = user.Email;
            var password= user.Password;
            var confirmpassword= user.ConfirmPassword;

            if (ModelState.IsValid)
            {
                Entities db = new Entities();

                var result = db.Users.Where(t => t.Email.ToLower().Contains(email.ToLower())).ToList();
                if (result.Count <= 0)
                {


                    SendEmail sendEmail = new SendEmail();
                    sendEmail.sendmail(email, number.ToString());
                    newUser = user;
                    Response.Redirect("~/UserAuth/SendEmail");
                   
                    return View(user);

                }
          

            }
     
            return View(user);

        }

        public ActionResult SignOut()
        {
            Session["Username"] = null;
            Response.Redirect("~/Home/Index");
            return View();
        }





        public ActionResult SendEmail()
        {
            return View();
        }

        [HttpPost]
        public ActionResult SendEmail(string code, User user)
        {

            string unum = newUser.Name;
            //var alert = $"<script>alert('{newUser.Name}')</script>" ;
            //Response.Write(alert);

            if (number.ToString().Contains(code))
            {
                Entities db = new Entities();
                db.Users.Add(newUser);
                db.SaveChanges();
                db.Dispose();
                Session["Username"] = newUser.Username;
                Response.Redirect("~/Home/Index");

            }

            return View();
        }


        public ActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]

        public ActionResult ForgotPassword(string email)
        {

            Entities db = new Entities();
        
            var result = db.Users.Where(t => t.Email.ToLower().Contains(email.ToLower())).ToList();
            if (result.Count > 0)
            {
                //updateData();
            }
            
            db.Dispose();
            return View();
        }

    


    }

 
}