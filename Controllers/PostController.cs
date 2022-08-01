using System;
using System.Collections.Generic;
using System.Data.Entity;
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

        [HttpGet]
        public ActionResult ViewPost(int PostID)
        {
            TempData["PostID"] = PostID;
            Entities db = new Entities();
            ViewBag.Replies = db.Replies
                .Where(temp => temp.PostID == PostID)
                .Include(temp => temp.User)
                .ToList();
            db.Dispose();
            return View();
        }

        [HttpPost]
        public ActionResult ViewPost(Reply reply)
        {
            reply.CreatorUsername = (string)Session["Username"];
            reply.CreationTime = DateTime.Now;

            reply.PostID = (int)TempData["PostID"];
            Entities db = new Entities();
            db.Replies.Add(reply);
            db.SaveChanges();
            
            db.Dispose();

            return View();
        }
    }
}
