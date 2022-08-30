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
                return Redirect("~/UserAuth/Login");
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
                report.Status = "UNRESOLVED";
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
            try { 
                Entities db = new Entities();
                string Username = (string)Session["Username"];

                var post = db.Posts.Where(temp => temp.PostID == PostID).FirstOrDefault();
                var existingReact = db.Reacts.Where(temp => temp.PostID == PostID && temp.Username == Username).FirstOrDefault();
                var existingNotif = db.Notifications.
                            Where(temp => temp.ForUsername == post.CreatorUsername && temp.HtmlContent.Contains(Username) &&
                            temp.HtmlContent.Contains("voted") && temp.HtmlContent.Contains(PostID.ToString())).
                            FirstOrDefault();

                //If react status is 0 is passed, then there was already a react before
                //First we remove that then we remove the notification for that
                if (ReactStatus == 0)
                {
                    db.Reacts.Remove(existingReact);
                    if (Username != post.CreatorUsername && existingNotif != null)
                    {
                        db.Notifications.Remove(existingNotif);
                    }
                    db.SaveChanges();
                    return Json("Success", JsonRequestBehavior.AllowGet);
                }

                //If react status is 1 or -1, we check if there was a react before
                //If there was a react we update that
                //Otherwise enter new react
                if (existingReact == null)
                {
                    React react = new React();
                    react.PostID = PostID;
                    react.ReactStatus = ReactStatus;
                    react.Username = Username;
                    db.Reacts.Add(react);
                }
                else
                {
                    existingReact.ReactStatus = ReactStatus;
                    db.Entry(existingReact).State = EntityState.Modified;
                }
                db.SaveChanges();


                //if the username and creatorUsername isn't same that means there should be a corresponding notifcation entry
                //We find that entry and update it
                //If we are unable to retrieve that entry, we create a new one
                if (Username != post.CreatorUsername)
                {
                    if (existingNotif == null)
                    {
                        Notification notification = new Notification()
                        {
                            ForUsername = post.CreatorUsername,
                            HtmlContent = GenerateHTMLForVote(Username, PostID, ReactStatus),
                            SeenStatus = "SENT",
                            CreationTime = DateTime.Now
                        };
                        db.Notifications.Add(notification);
                    }
                    else
                    {
                        existingNotif.HtmlContent = GenerateHTMLForVote(Username, PostID, ReactStatus);
                        existingNotif.SeenStatus = "SENT";
                        existingNotif.CreationTime = DateTime.Now;
                        db.Entry(existingNotif).State = EntityState.Modified;
                    }
                    db.SaveChanges();
                }

                return Json("Success", JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                return new HttpStatusCodeResult(401, ex.Message);
            }
        }

        [HttpGet]
        public ActionResult ViewPost(int? PostID)
        {
            if(PostID == default)
            {
                return Redirect("~/Home/Error");
            }
            TempData["PostID"] = PostID;
            try
            {
                Entities db = new Entities();

                var post = db.Posts.Where(t => t.PostID == PostID).FirstOrDefault();
                if (post == null)
                {
                    return Redirect("~/Home/Error");
                }
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

            //var fetch = db.Posts.Where(m => m.ForumID == forumID).ToList();
           
            return View();
        }

        [HttpPost]
        public ActionResult GetForumPost(int forumID)
        {
            var fetch = db.Posts.Where(m => m.ForumID == forumID).ToList();
            return PartialView("_ForumPost", fetch);
        }

        [HttpPost]
        public ActionResult GetUserPost(string Username)
        {
            var fetch = db.Users.Where(m => m.Username == Username).FirstOrDefault().Posts.ToList();
            return PartialView("_ForumPost", fetch);
        }

        

        private string GenerateHTMLForComment(string Username, int postID)
        {
            Entities db = new Entities();
            var user = db.Users.Where((t) => t.Username == Username).FirstOrDefault();
            return string.Format("<p class=\"notification-text\"><a class=\"notification-link\" " +
                "href=\"/UserProfile?Username={0}\">{1}</a> commented on your " +
                "<a class=\"notification-link\" href=\"/Post/ViewPost?PostID={2}\">post</a></p>", user.Username, user.Name, postID);
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
                "href=\"/UserProfile?Username={0}\">{1}</a> {2} on your " +
                "<a class=\"notification-link\" href=\"/Post/ViewPost?PostID={3}\">post</a></p>", user.Username, user.Name, voteAction, postID);
        }

        [HttpPost]
        public ActionResult CreateReplyReport(String Reason, int ReplyID)
        {
            try { 
                Report report = new Report();
                report.ReporterUsername = (string)Session["Username"];
                report.Status = "UNRESOLVED";
                report.Reason = Reason;
                report.Context = "COMMENT";
                report.ContextID = ReplyID.ToString();


                Entities db = new Entities();

                var existingReport = db.Reports.Where(t => t.ReporterUsername == report.ReporterUsername && t.ContextID == report.ContextID && t.Context == report.Context).Count();
                if(existingReport > 0)
                {
                    return Json("Exists", JsonRequestBehavior.AllowGet);
                }

                db.Reports.Add(report);
                db.SaveChanges();

                return Json("Success", JsonRequestBehavior.AllowGet);

            } catch
            {
                return Json("Error", JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult DeletePost(int PostID)
        {
            try
            {
                Entities db = new Entities();
                var post = db.Posts.Where(t => t.PostID == PostID).FirstOrDefault();
                foreach (var replies in post.Replies)
                {
                    db.Replies.Remove(replies);
                }
                foreach (var media in post.MediaItems)
                {
                    db.MediaItems.Remove(media);
                }
                foreach (var react in post.Reacts)
                {
                    db.Reacts.Remove(react);
                }
                db.Posts.Remove(post);
                db.SaveChanges();
                return Json("Success", JsonRequestBehavior.AllowGet);
            } catch (Exception ex)
            {
                return Json("Error", JsonRequestBehavior.AllowGet);
            }
        }
    }
}
