using Hello_Travellers.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Hello_Travellers.Controllers
{
    public class HomeController : Controller
    {

        public ActionResult Index()
        {
            Entities db = new Entities();
            var posts = db.Posts.ToList();
            ViewBag.Subforums = db.Subforums.ToList();
            return View(posts);
        }

        [HttpPost]
        public ActionResult RenderBottom(int Count, String Criteria)
        {
            Entities db = new Entities();
            List<Post> posts;
            if (Criteria.Contains("new"))
            {
                posts = db.Posts.OrderByDescending(temp => temp.CreationTime).Take(Math.Min(db.Posts.Count(), Count)).ToList();
            }
            else if (Criteria.Contains("commented"))
            {
                posts = db.Posts.OrderByDescending(temp => temp.Replies.Count()).Take(Math.Min(db.Posts.Count(), Count)).ToList();
            }
            else
            {
                posts = db.Posts.OrderByDescending(temp => temp.Reacts.Sum(x => x.ReactStatus)).Take(Math.Min(db.Posts.Count(), Count)).ToList();
            }
            return PartialView("_HomePageBottom", posts);
        }

        //public ActionResult Index()
        //{

        //    Entities db = new Entities();
        //    var topPosts = db.Posts.OrderByDescending(temp => temp.CreationTime).Skip(Math.Max(0, db.Posts.Count() - 3)).ToArray();
        //    var topWriters = new User[topPosts.Length];
        //    for (int i = 0; i < topPosts.Length; i++)
        //    {
        //        var currWriter = topPosts[i].CreatorUsername;
        //        topWriters[i] = db.Users.Where(temp => temp.Username == currWriter).Single();
        //    }
        //    var topMedias = new MediaItem[topPosts.Length];
        //    for (int i = 0; i < topPosts.Length; i++)
        //    {
        //        var currPost = topPosts[i].PostID;
        //        topMedias[i] = db.MediaItems.Where(temp => temp.PostID == currPost).FirstOrDefault();
        //    }
        //    ViewBag.TopPosts = topPosts;
        //    ViewBag.TopWriters = topWriters;
        //    ViewBag.TopMedias = topMedias;

        //    var selectedPost = db.Posts.OrderByDescending(temp => temp.CreationTime).Skip(Math.Max(0, db.Posts.Count() - 9)).ToArray();
        //    var selectedAuthor = new User[selectedPost.Length];
        //    for (int i = 0; i < selectedPost.Length; ++i)
        //    {
        //        var currUsername = selectedPost[i].CreatorUsername;
        //        selectedAuthor[i] = db.Users.Where(temp => temp.Username == currUsername).FirstOrDefault();
        //    }
        //    var selectedMedia = new MediaItem[selectedPost.Length];
        //    for (int i = 0; i < selectedPost.Length; ++i)
        //    {
        //        var currPostID = selectedPost[i].PostID;
        //        selectedMedia[i] = db.MediaItems.Where(temp => temp.PostID == currPostID).FirstOrDefault();
        //    }

        //    ViewBag.selectedPost = selectedPost;
        //    ViewBag.selectedAuthor = selectedAuthor;
        //    ViewBag.selectedMedia = selectedMedia;
        //    return View();

        //}

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";
            return View();
        }

        public ActionResult Error()
        {
            return View();
        }

        [HttpGet]
        public ActionResult GetNotificationCount()
        {
            if (Session["Username"] == null)
            {
                return Json(0, JsonRequestBehavior.AllowGet);
            }
            var currUsername = (string)Session["Username"];
            Entities db = new Entities();
            int count = db.Users.Where(temp => temp.Username.Equals(currUsername)).FirstOrDefault().Notifications.Where(temp => !temp.SeenStatus.Equals("SEEN")).Count();
            return Json(count, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetNotificationResult()
        {
            var currUsername = (string)Session["Username"];
            Entities db = new Entities();
            var User = db.Users.Where(temp => temp.Username.Equals(currUsername)).FirstOrDefault();
            var notificationList = User.Notifications.OrderByDescending(temp => temp.CreationTime).Take(Math.Min(User.Notifications.Count(), 8)).ToList();
            foreach (var item in notificationList)
            {
                item.SeenStatus = "SEEN";
            }
            db.SaveChanges();
            return PartialView("_NotificationBlock", notificationList);
        }
    }
}