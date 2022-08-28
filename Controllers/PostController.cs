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

        [HttpPost]
        public ActionResult UpdatePost(int PostID, String Content)
        {
            try
            {
                Entities db = new Entities();
                var post = db.Posts.Find(PostID);
                post.Content = Content;
                db.Entry(post).State = EntityState.Modified;
                db.SaveChanges();
                return Json("{\"Status\": \"Edited\"}");
            } catch (Exception ex)
            {
                return new HttpStatusCodeResult(401, ex.Message);
            }
        }

        [HttpPost]
        public ActionResult Report(String Reason, String Context, String ContextID)
        {
            try
            {
                Entities db = new Entities();
                string Username = (string)Session["Username"];

                Report existingReport = db.Reports.Where(temp => Context == temp.Context && ContextID == temp.ContextID && Username == temp.ReporterUsername).FirstOrDefault();
                if(existingReport != null)
                {
                    return Json("{\"Status\": \"Exists\"}");
                }
                Report report = new Report();
                report.ReporterUsername = Username;
                report.Context = Context;
                report.ContextID = ContextID;
                report.Reason = Reason;
                db.Reports.Add(report);
                db.SaveChanges();

                return Json("{\"Status\": \"Reported\"}");
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(401, ex.Message);
            }
        }

        [HttpPost]
        public ActionResult UpdateReact(int PostID, int ReactStatus)
        {
            try
            {
                Entities db = new Entities();
                string Username = (string) Session["Username"];

                React newReact = new React();
                newReact.PostID = PostID;
                newReact.ReactStatus = ReactStatus;
                newReact.Username = Username;

                React existingReact = db.Reacts.Where(temp => temp.PostID == newReact.PostID && temp.Username == newReact.Username).FirstOrDefault();
                if (existingReact == null)
                {
                    db.Reacts.Add(newReact);
                    Notification notification = new Notification();
                    notification.ForUsername = db.Posts.Where(t => t.PostID == PostID).First().CreatorUsername;
                    notification.HtmlContent = GenerateHTMLForVote(Username, PostID, ReactStatus);
                    notification.SeenStatus = "SENT";
                    notification.CreationTime = DateTime.Now;
                    db.Notifications.Add(notification);
                }
                else
                {
                    //Checking notification
                    string searchKeyword = "downvoted";
                    if (existingReact.ReactStatus == 1)
                    {
                        searchKeyword = "upvoted";
                    }
                    var existingNotif = db.Notifications.Where(temp => temp.HtmlContent.Contains(searchKeyword) && temp.HtmlContent.Contains(Username) && temp.HtmlContent.Contains(PostID.ToString())).First();

                    if (ReactStatus == 0)
                    {
                        db.Reacts.Remove(existingReact);
                        db.Notifications.Remove(existingNotif);
                    }
                    else
                    {
                        //Notification Modification here
                        existingNotif.HtmlContent = GenerateHTMLForVote(Username, PostID, ReactStatus);
                        existingNotif.SeenStatus = "SENT";
                        existingNotif.CreationTime = DateTime.Now;
                        
                        //Update db for react and notification
                        existingReact.ReactStatus = newReact.ReactStatus;
                        db.Entry(existingReact).State = EntityState.Modified;
                        db.Entry(existingNotif).State = EntityState.Modified;

                    }
                }
                db.SaveChanges();

                return Json("Success");
            } catch (Exception ex)
            {
                return new HttpStatusCodeResult(401, ex.Message);
            }
        }

        [HttpGet]
        public ActionResult ViewPost(int PostID)
        {
            TempData["PostID"] = PostID;
            try
            {
                Entities db = new Entities();

                var post = db.Posts.Where(t => t.PostID == PostID).FirstOrDefault();
                var replies = post.Replies.ToArray();
                var users = new User[replies.Length];
                for (int i = 0; i < replies.Length; i++)
                {
                    var currentUsername = replies[i].CreatorUsername;
                    users[i] = db.Users.Where(t => t.Username == currentUsername).Single();
                }

                //var writer = db.Users.Where(t => t.Username == post.CreatorUsername).Single();
                //var media = db.MediaItems.Where(t => t.PostID == post.PostID).ToArray();
                var writer = post.User;
                var media = post.MediaItems.ToArray();
                string Username = (string)Session["Username"];
                if(!String.IsNullOrEmpty(Username))
                {
                    var currentReact = db.Reacts.Where(t => t.PostID == post.PostID && t.Username == Username).FirstOrDefault();
                    if(currentReact != null)
                    {
                        ViewBag.currentReact = currentReact;

                    } else
                    {
                        ViewBag.currentReact = new React() { ReactStatus = 0, Username = Username, PostID = post.PostID };
                    }
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

                var post = db.Posts.Where(t => t.PostID == reply.PostID).FirstOrDefault();
                var writer = db.Users.Where(t => t.Username == post.CreatorUsername).FirstOrDefault();
                var media = post.MediaItems.ToArray();

                //Code for comment notification
                if(writer.Username != (string)Session["Username"])
                {
                    Notification notification = new Notification();
                    notification.ForUsername = writer.Username;
                    notification.HtmlContent = GenerateHTMLForComment((string)Session["Username"], post.PostID);
                    notification.CreationTime = DateTime.Now;
                    notification.SeenStatus = "SENT";
                    db.Notifications.Add(notification);
                }

                db.Replies.Add(reply);
                db.SaveChanges();
                var replies = db.Replies
                    .Where(temp => temp.PostID == reply.PostID)
                    .ToArray();
                var users = new User[replies.Length];
                for (int i = 0; i < replies.Length; i++)
                {
                    var currentUsername = replies[i].CreatorUsername;
                    users[i] = db.Users.Where(t => t.Username == currentUsername).First();
                }

                if (!String.IsNullOrEmpty(reply.CreatorUsername))
                {
                    var currentReact = db.Reacts.Where(t => t.PostID == post.PostID && t.Username == reply.CreatorUsername).FirstOrDefault();
                    if (currentReact != null)
                    {
                        ViewBag.currentReact = currentReact;

                    }
                    else
                    {
                        ViewBag.currentReact = new React() { ReactStatus = 0, Username = reply.CreatorUsername, PostID = post.PostID };
                    }
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

        private string GenerateHTMLForComment(string Username, int postID)
        {
            Entities db = new Entities();
            var user = db.Users.Where((t) => t.Username == Username).FirstOrDefault();
            return string.Format("<p class=\"notification-text\"><a class=\"notification-link\" " +
                "href=\"~/UserProfile?Username={0}\">{1}</a> commented on your " +
                "<a class=\"notification-link\" href=\"~/Post/ViewPost?PostID={2}\">post</a></p>", user.Username, user.Name, postID);
        }

        private string GenerateHTMLForVote(string Username, int postID, int voteStatus)
        {
            string voteAction = "";
            if(voteStatus == 0)
            {
                voteAction = "removed vote from";
            } else if(voteStatus == 1)
            {
                voteAction = "upvoted";
            } else
            {
                voteAction = "downvoted";
            }
            Entities db = new Entities();
            var user = db.Users.Where((t) => t.Username == Username).FirstOrDefault();
            return string.Format("<p class=\"notification-text\"><a class=\"notification-link\" " +
                "href=\"~/UserProfile?Username={0}\">{1}</a> {2} on your " +
                "<a class=\"notification-link\" href=\"~/Post/ViewPost?PostID={3}\">post</a></p>", user.Username, user.Name, voteAction, postID);
        }
    }
}
