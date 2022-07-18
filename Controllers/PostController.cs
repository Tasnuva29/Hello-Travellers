using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Hello_Travellers.Models;

namespace Hello_Travellers.Controllers
{
    public class PostController : Controller
    {
        Entities db = new Entities();

        // GET: UserProfile
        public ActionResult CreatePost()
        {
            ViewBag.Subforums = db.Subforums.ToList();
            return View();
        }

        [HttpPost]
        public ActionResult CreatePost(Post post)
        {
            ViewBag.Subforums = db.Subforums.ToList();
            var currentUsername = "bthor";
            post.CreatorUsername = currentUsername;
            db.Posts.Add(post);
            try
            {
                db.SaveChanges();
                Response.Redirect("~/Home/Index");
            }
            catch (DbEntityValidationException ex)
            {
                foreach (var entityValidationErrors in ex.EntityValidationErrors)
                {
                    foreach (var validationError in entityValidationErrors.ValidationErrors)
                    {
                        Response.Write("Property: " + validationError.PropertyName + " Error: " + validationError.ErrorMessage);
                    }
                }

            }
            return View();
        }
    }
}