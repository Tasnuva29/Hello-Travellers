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
        Entities db = new Entities();
        // GET: UserProfile
        public ActionResult CreatePost()
        {
            if (Session["Username"] == null)
            {
                Response.Redirect("~/UserAuth/Login");
                return null;
            }
            Entities db = new Entities();
            ViewBag.Subforums = db.Subforums.ToList();
            db.Dispose();
            return View();
        }

        [HttpPost]
        public ActionResult CreatePost(Post post, IEnumerable<HttpPostedFileBase> postedImages)
        {
            if (Session["Username"] == null)
            {
                Response.Redirect("~/UserAuth/Login");
                return null;
            }

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
            try
            {
                Entities db = new Entities();

                var post = db.Posts.Where(t => t.PostID == PostID).Single();
                var replies = db.Replies
                    .Where(temp => temp.PostID == PostID)
                    .ToArray();
                var users = new User[replies.Length];
                for (int i = 0; i < replies.Length; i++)
                {
                    var currentUsername = replies[i].CreatorUsername;
                    users[i] = db.Users.Where(t => t.Username == currentUsername).Single();
                }

                var writer = db.Users.Where(t => t.Username == post.CreatorUsername).Single();
                var media = db.MediaItems.Where(t => t.PostID == post.PostID).ToArray();
                ViewBag.Replies = replies;
                ViewBag.Users = users;
                ViewBag.Post = post;
                ViewBag.Media = media;
                ViewBag.Writer = writer;
            }
            catch
            {
                return HttpNotFound();
            }
            return View();
        }

        [HttpPost]
        public ActionResult ViewPost(Reply reply)
        {
            reply.CreatorUsername = (string)Session["Username"];
            reply.CreationTime = DateTime.Now;
            reply.PostID = (int)TempData["PostID"];
            TempData["PostID"] = reply.PostID;

            if (Session["Username"] == null)
            {
                Response.Redirect("~/UserAuth/Login");
                return null;
            }

            try
            {
                Entities db = new Entities();

                var post = db.Posts.Where(t => t.PostID == reply.PostID).Single();
                var writer = db.Users.Where(t => t.Username == post.CreatorUsername).Single();
                var media = db.MediaItems.Where(t => t.PostID == post.PostID).ToArray();


                db.Replies.Add(reply);
                db.SaveChanges();
                var replies = db.Replies
                    .Where(temp => temp.PostID == reply.PostID)
                    .ToArray();
                var users = new User[replies.Length];
                for (int i = 0; i < replies.Length; i++)
                {
                    var currentUsername = replies[i].CreatorUsername;
                    users[i] = db.Users.Where(t => t.Username == currentUsername).Single();
                }
                ViewBag.Replies = replies;
                ViewBag.Users = users;
                ViewBag.Post = post;
                ViewBag.Media = media;
                ViewBag.Writer = writer;
            }
            catch
            {
                return HttpNotFound();
            }
            return View();
        }

        [HttpGet]
        public ActionResult Forum(int forumID)
        {

            var fetch = db.Posts.Where(m => m.ForumID == forumID).ToList();
            //var fetch = db.Posts.ToList();
            return View(fetch);
        }
    }
}
