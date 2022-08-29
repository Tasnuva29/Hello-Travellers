using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;
using Hello_Travellers.Models;


namespace Hello_Travellers.Controllers
{
    public class UserAuthController : Controller
    {

        static string name;
        static int number = new Random().Next(1000, 9999);
        static int number_forPass = new Random().Next(1000, 9999);
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
            if (users.Count > 0)
            {
                Session["Username"] = users[0].Username;
                Session["Rank"] = users[0].Rank;
                if (Session["Rank"].ToString().Contains("BANNED"))
                {
                    var rank = Session["Rank"].ToString();
                    if (DateTime.Parse(rank.Split(',')[1]) < DateTime.Now)
                    {

                    }
                }
                else if (Session["Rank"].ToString().Equals("ADMIN"))
                {
                    Response.Redirect("~/Admin/Users");
                }
                else
                {
                    Response.Redirect("~/Home/Index");
                }
            }
            else
            {
                ViewBag.ValidID = "This Email address has been already registerd !";

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
            var password = user.Password;
            var confirmpassword = user.ConfirmPassword;

            if (ModelState.IsValid)
            {
                Entities db = new Entities();


                var chkUsername = db.Users.Where(t => t.Username.ToLower().Contains(username.ToLower())).ToList();
                if (chkUsername.Count > 0)
                {
                    ViewBag.chkuse = "no";
                }
                else
                {
                    var result = db.Users.Where(t => t.Email.ToLower().Contains(email.ToLower())).ToList();

                    if (result.Count <= 0)
                    {
                        SendEmail sendEmail = new SendEmail();
                        sendEmail.sendmail(email, number.ToString(), "Activation Mail");
                        newUser = user;
                        Response.Redirect("~/UserAuth/SendEmail");
                        return View(user);


                    }

                    else
                    {
                        ViewBag.flag = "This Email address has been already registerd !";

                    }
                }



            }

            return View(user);

        }

        public ActionResult SignOut()
        {
            Session["Username"] = null;
            Session["Rank"] = null;
            Response.Redirect("~/Home/Index");
            return View();
        }









        public ActionResult SendEmail()
        {
            Session["CodePending"] = "YES";
            return View();
        }

        [HttpPost]
        public ActionResult SendEmail(string code)
        {

            //string unum = newUser.Name;
            Session["CodePending"] = "YES";

            if (number.ToString().Contains(code))
            {
                Entities db = new Entities();
                db.Users.Add(newUser);
                db.SaveChanges();
                db.Dispose();
                Session["Username"] = newUser.Username;

                Response.Redirect("~/Home/Index");

            }
            else
            {
                ViewBag.correct = "false";
            }


            return View();
        }

        [HttpPost]
        public ActionResult SendMailForPass()
        {
            return View();
        }

        [HttpPost]

        public ActionResult SendMailForPass(string code)
        {
            if (number_forPass.ToString().Contains(code))
            {

                Response.Redirect("~/UserAuth/InputNewPass");

            }
            else
            {
                ViewBag.correct = "no";
            }
            return View();
        }


        public ActionResult InputNewPass()
        {
            return View();
        }

        [HttpPost]
        public ActionResult InputNewPass(string newpass)
        {
            Entities db = new Entities();
            string e = (string)Session["Email"];

            try
            {
                var data = db.Users.Where(x => x.Email.Equals(e)).FirstOrDefault();

                data.Password = newpass;
                data.ConfirmPassword = newpass;
                db.Entry(data).State = EntityState.Modified;
                db.SaveChanges();
                ViewBag.newpassvalid = "success";
                Response.Redirect("~/UserAuth/Login");
            }
            catch (Exception ex)
            {

            }
            return View();
        }



        public ActionResult SearchUser()
        {

            return View();
        }

        [HttpPost]

        public ActionResult SearchUser(string tf_username)
        {
            name = tf_username;
            Response.Redirect("~/UserAuth/SearchResult");

            return View();
        }

        public ActionResult SearchResult()
        {


            Entities db = new Entities();

            var userData = db.Users.Where(m => m.Username.Contains(name)).ToList();
            ViewBag.userData = userData;
            var postData = db.Posts.Where(m => m.Title.Contains(name)).ToList();
            ViewBag.postData = postData;

            //var alert = $"<script>alert('{name}')</script>";
            //Response.Write(alert);
            return View();

        }



        public ActionResult ForgotPassword()
        {

            return View();
        }

        [HttpPost]

        public ActionResult ForgotPassword(string Email, User user)
        {
            {
                Entities db = new Entities();
                var email = Email;

                List<User> users = db.Users.Where(x => x.Email.Equals(email)).ToList();

                if (users.Count > 0)
                {
                    newUser = user;
                    SendEmail sendEmail = new SendEmail();
                    sendEmail.sendmail(email, number.ToString(), "Activation Mail");


                    Session["Email"] = Email;

                    Response.Redirect("~/UserAuth/SendMailForPass");

                }
                else
                {
                    ViewBag.forgotPassValid = "not";
                }
            }
            return View(user);





        }



    }


}