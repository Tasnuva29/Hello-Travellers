using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.IO;
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
            Entities db = new Entities();
            ViewBag.Subforums = db.Subforums.ToList();
            db.Dispose();
            return View();
        }

        [HttpPost]
        public ActionResult CreatePost(Post post, IEnumerable<HttpPostedFileBase> postedImages)
        {
            string path = Server.MapPath("~/Images/MediaContent/");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            

            using (Entities db = new Entities()) 
            {
                post.CreatorUsername = (string)Session["Username"];
                post.CreationTime = DateTime.Now;
                db.Posts.Add(post);
                db.SaveChanges();

                if (postedImages != null)
                {
                    foreach (var image in postedImages)
                    {
                        MediaItem mediaItem = new MediaItem();
                        mediaItem.PostID = post.PostID;
                        mediaItem.UploaderUsername = post.CreatorUsername;
                        mediaItem.CreationTime = post.CreationTime;
                        mediaItem.Type = Path.GetExtension(image.FileName);
                        db.MediaItems.Add(mediaItem);
                        db.SaveChanges();
                        var fileName = mediaItem.MediaID + mediaItem.Type;
                        image.SaveAs(path + fileName);
                    }
                }



                ViewBag.Subforums = db.Subforums.ToList();
            }

            return View();
        }

        public ActionResult ViewPost()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Forum(int forumID)
        {

            var fetch = db.Posts.Where(m => m.ForumID == forumID).ToList();
            //var fetch = db.Posts.ToList();
            return View(fetch);
        }

        public ActionResult TravelPlans(String submit)
        {

            string s = ViewBag.submit;
            var fetch = db.Posts.ToList();
            return View(fetch);
        }

       
 public ActionResult ShortStories(String submit)
        {

            string s = ViewBag.submit;
            var fetch = db.Posts.ToList();
            return View(fetch);
        }






    }
}