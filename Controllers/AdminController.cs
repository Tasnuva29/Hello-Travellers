using Hello_Travellers.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace Hello_Travellers.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin
        public ActionResult Index()
        {

            return View();

        }
        public ActionResult Dashboard()
        {
            if (Session["Username"] == null)
            {
                return RedirectToAction("Login", "UserAuth");
            }
            else
            {
                if (Session["Rank"].Equals("ADMIN"))
                {
                    return View();
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }

        }

        public ActionResult Users()
        {
            if (Session["Username"] == null)
            {
                return RedirectToAction("Login", "UserAuth");
            }
            else
            {
                if (Session["Rank"].Equals("ADMIN"))
                {
                    Entities db = new Entities();
                    var count = db.Users.Where(temp => temp.Rank == "USER").Count();
                    var userList = db.Users.Where(temp => temp.Rank == "USER").ToList();
                    ViewBag.count = count;
                    ViewBag.userList = userList;
                    return View();
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }

        }
        public ActionResult Admins()
        {
            if (Session["Username"] == null)
            {
                return RedirectToAction("Login", "UserAuth");
            }
            else
            {
                if (Session["Rank"].Equals("ADMIN"))
                {
                    Entities db = new Entities();
                    var adminList = db.Users.Where(temp => temp.Rank == "ADMIN").ToList();
                    ViewBag.adminList = adminList;
                    var count = db.Users.Where(temp => temp.Rank == "ADMIN").Count();
                    ViewBag.count = count;
                    return View();
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }

        }

        public ActionResult Subforums()
        {
            if (Session["Username"] == null)
            {
                return RedirectToAction("Login", "UserAuth");
            }
            else
            {
                if (Session["Rank"].Equals("ADMIN"))
                {
                    Entities db = new Entities();
                    var subForums = db.Subforums.ToList();
                    ViewBag.subForum = subForums;
                    var count = db.Subforums.Count();
                    ViewBag.count = count;
                    return View();
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }

        }


        [HttpPost]
        public ActionResult CreateSubforum(string ForumName)
        {

            Entities db = new Entities();
            var subForum = new Subforum();
            var count = db.Subforums.Where(temp => temp.ForumName == ForumName).Count();
            if (count >= 1)
            {
                return Json("false", JsonRequestBehavior.AllowGet);
            }
            else
            {
                subForum.ForumName = ForumName.Trim();
                db.Subforums.Add(subForum);
                db.SaveChanges();
                return Json("true", JsonRequestBehavior.AllowGet);
            }

        }
        [HttpPost]
        public ActionResult DeleteSubforum(int ForumID)
        {
            Entities db = new Entities();
            var subForum = db.Subforums.Where(temp => temp.ForumID == ForumID).FirstOrDefault();
            db.Subforums.Remove(subForum);
            db.SaveChanges();
            return Json("true", JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult EditSubforum(string ForumName, int ForumID)
        {
            Entities db = new Entities();
            var subForum = new Subforum();
            var editedName = ForumName.Trim();
            var count = db.Subforums.Where(temp => temp.ForumName == editedName).Count();
            if (count >= 1)
            {
                return Json("false", JsonRequestBehavior.AllowGet);
            }
            else
            {
                subForum.ForumName = editedName;
                subForum.ForumID = ForumID;
                db.Entry(subForum).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return Json("true", JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Reports()
        {
            if (Session["Username"] == null)
            {
                return RedirectToAction("Login", "UserAuth");
            }
            else
            {
                if (Session["Rank"].Equals("ADMIN"))
                {
                    Entities db = new Entities();
                    var reportCount = db.Reports.Where(temp => temp.Status == "UNRESOLVED").Count();
                    var postReportCount = db.Reports.Where(temp => temp.Context == "POST" && temp.Status == "UNRESOLVED").Count();
                    var profileReportCount = db.Reports.Where(temp => temp.Context == "PROFILE" && temp.Status == "UNRESOLVED").Count();
                    var commentReportCount = db.Reports.Where(temp => temp.Context == "COMMENT" && temp.Status == "UNRESOLVED").Count();
                    var postReport = db.Reports.Where(temp => temp.Context == "POST" && temp.Status == "UNRESOLVED").ToList();
                    var profileReport = db.Reports.Where(temp => temp.Context == "PROFILE" && temp.Status == "UNRESOLVED").ToList();
                    var commentReport = db.Reports.Where(temp => temp.Context == "COMMENT" && temp.Status == "UNRESOLVED").ToList();
                    ViewBag.reportCount = reportCount;
                    ViewBag.postReportCount = postReportCount;
                    ViewBag.profileReportCount = profileReportCount;
                    ViewBag.commentReportCount = commentReportCount;
                    ViewBag.postReport = postReport;
                    ViewBag.profileReport = profileReport;
                    ViewBag.commentReport = commentReport;
                    var viewComment = new Reply[commentReport.Count];
                    for (int i = 0; i < commentReport.Count; i++)
                    {
                        var commentID = commentReport[i].ContextID;
                        var contextID = Convert.ToInt32(commentID);
                        viewComment[i] = db.Replies.Where(temp => temp.ReplyID == contextID).FirstOrDefault();
                    }
                    ViewBag.viewComment = viewComment;
                    return View();
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
        }
        [HttpPost]
        public ActionResult Ban()
        {
            return Json("true", JsonRequestBehavior.AllowGet);
        }
        public ActionResult Posts()
        {
            if (Session["Username"] == null)
            {
                return RedirectToAction("Login", "UserAuth");
            }
            else
            {
                if (Session["Rank"].Equals("ADMIN"))
                {
                    Entities db = new Entities();
                    var subforumList = db.Subforums.ToList();
                    ViewBag.subForumList = subforumList;
                    var postList = db.Posts.ToList();
                    ViewBag.postList = postList;
                    return View();
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }

        }
        void UpdateStatus(int ReportID)
        {
            Entities db = new Entities();
            var getReport = db.Reports.Where(temp => temp.ReportID == ReportID).FirstOrDefault();
            getReport.Status = "RESOLVED";
            db.Entry(getReport).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
        }

        [HttpPost]
        public ActionResult DeletePostOrComment(int ReportID, string ContextID, string Context)
        {
            Entities db = new Entities();
            //var deleteContextID = Convert.ToInt32(ContextID);
            if (Context.Equals("POST"))
            {

                var post = db.Posts.Where(temp => temp.PostID.ToString() == ContextID).FirstOrDefault();
                GenerateNotificationForDelete(post.CreatorUsername, post.Title);
                foreach (var item in post.Replies.ToList())
                {
                    db.Replies.Remove(item);
                }
                foreach (var item in post.MediaItems.ToList())
                {
                    db.MediaItems.Remove(item);
                }
                foreach (var item in post.Reacts.ToList())
                {
                    db.Reacts.Remove(item);
                }
                db.Posts.Remove(post);
                db.SaveChanges();
                UpdateStatus(ReportID);
                return Json("post", JsonRequestBehavior.AllowGet);
            }
            else
            {
                var reply = db.Replies.Where(temp => temp.ReplyID.ToString() == ContextID).FirstOrDefault();
                GenerateNotificationForDelete(reply.CreatorUsername, "");
                db.Replies.Remove(reply);
                db.SaveChanges();
                UpdateStatus(ReportID);
                return Json("comment", JsonRequestBehavior.AllowGet);
            }
        }

        private void GenerateNotificationForDelete(string Username, string PostTitle)
        {
            Entities db = new Entities();
            //var user = db.Users.Where((t) => t.Username == Username).FirstOrDefault();
            Notification notif = new Notification();
            notif.CreationTime = DateTime.Now;
            notif.SeenStatus = "SENT";
            notif.ForUsername = Username;
            if (PostTitle.Length > 0)
            {
                notif.HtmlContent = string.Format("<p class=\"notification-text\">" +
                    "Your post ({0}) was deleted by  " +
                    "the Admin for a report!</p>", PostTitle);
            }
            else
            {
                notif.HtmlContent = string.Format("<p class=\"notification-text\">" +
                    "Your comment was deleted by  " +
                    "the Admin for a report!</p>");
            }
            db.Notifications.Add(notif);
            db.SaveChanges();
        }

        [HttpPost]
        public ActionResult DismissReport(int ReportID)
        {
            UpdateStatus(ReportID);
            return Json("true", JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult BanUser(int ReportID, string ContextID, string Context, int Banvalue, string Banduration)
        {
            var sub = "Regarding your ban";
            var currentTime = DateTime.Now;
            switch (Banduration)
            {
                case "1":
                    currentTime = currentTime.AddHours(Banvalue);
                    break;
                case "2":
                    currentTime = currentTime.AddDays(Banvalue);
                    break;
                case "3":
                    currentTime = currentTime.AddMonths(Banvalue);
                    break;
                case "4":
                    currentTime = currentTime.AddYears(Banvalue);
                    break;
                default:
                    break;
            }
            Entities db = new Entities();
            if (Context == "PROFILE")
            {
                var setRank = db.Users.Where(temp => temp.Username == ContextID).FirstOrDefault();
                setRank.Rank = "BANNED," + currentTime.ToString() + "," + Context;
                var to = setRank.Email;
                setRank.ConfirmPassword = setRank.Password;
                db.Entry(setRank).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                var body = "We have noticed activities from your profile that goes" +
                           "against our standards and hence you have been banned for "
                           + Banvalue.ToString() + " " + Banduration;
                sendmail(to, body, sub);
                UpdateStatus(ReportID);
            }
            else if (Context == "POST")
            {
                var getPost = db.Posts.Where(temp => temp.PostID.ToString() == ContextID).FirstOrDefault();
                var setRank = getPost.User;
                setRank.ConfirmPassword = setRank.Password;
                var to = setRank.Email;
                setRank.Rank = "BANNED," + currentTime.ToString() + "," + Context;
                db.Entry(setRank).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                var body = "We have noticed activities from your profile that goes" +
                          "against our standards and hence you have been banned for "
                          + Banvalue.ToString() + " " + Banduration;
                sendmail(to, body, sub);
                UpdateStatus(ReportID);
            }
            else if (Context == "COMMENT")
            {
                var getComment = db.Replies.Where(temp => temp.ReplyID.ToString() == ContextID).FirstOrDefault();
                var setRank = getComment.User;
                var to = setRank.Email;
                setRank.ConfirmPassword = setRank.Password;
                setRank.Rank = "BANNED," + currentTime.ToString() + "," + Context;
                db.Entry(setRank).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                var body = "We have noticed activities from your profile that goes" +
                     "against our standards and hence you have been banned for "
                     + Banvalue.ToString() + " " + Banduration;
                sendmail(to, body, sub);
                UpdateStatus(ReportID);
            }
            return Json(currentTime, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Settings()
        {
            if (Session["Username"] == null)
            {
                return RedirectToAction("Login", "UserAuth");
            }
            else
            {
                if (Session["Rank"].Equals("ADMIN"))
                {
                    return View();
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }

        }
        public ActionResult BannedUser()
        {
            if (Session["Username"] == null)
            {
                return RedirectToAction("Login", "UserAuth");
            }
            else
            {
                if (Session["Rank"].Equals("ADMIN"))
                {
                    Entities db = new Entities();
                    var userList = db.Users.Where(temp => temp.Rank.Contains("BANNED"));
                    var bannedUser = userList.Where(temp => DateTime.Parse(temp.Rank.Split(',')[1]) < DateTime.Now);
                    ViewBag.bannedUser = userList;
                    return View();
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }

        }
        [HttpPost]
        public ActionResult RemoveUserBan(string Username)
        {
            Entities db = new Entities();
            var getUser = db.Users.Where(temp => temp.Username == Username).FirstOrDefault();
            getUser.Rank = "USER";
            var to = getUser.Email;
            var sub = "Regarding removing your ban";
            getUser.ConfirmPassword = getUser.Password;
            db.Entry(getUser).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            var body = "We have removed your ban." +
                          "You can now log in to your account.Please be careful next time";
            sendmail(to, body, sub);
            return Json("true", JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetReportInformation(int ReportID)
        {
            Entities db = new Entities();
            var report = db.Reports.Where(temp => temp.ReportID == ReportID).FirstOrDefault();
            var comment = db.Replies.Where(temp => temp.ReplyID.ToString().Contains(report.ContextID)).FirstOrDefault();
            var jsonString = string.Format("\"Name\": \"{0}\", " +
                "\"Username\": \"{1}\", " +
                "\"ProfilePictureLocation\": \"{2}\", " +
                "\"PostID\": {3}, " +
                "\"Content\": \"{4}\"", comment.User.Name, comment.User.Username, comment.User.DisplayPictureName, comment.PostID, comment.Content);
            return Json('{' + jsonString + '}', JsonRequestBehavior.AllowGet);
        }
        public void sendmail(string To, string body, string sub)
        {
            MailMessage mc = new MailMessage("190104113@aust.edu", To);
            mc.Subject = sub; mc.Body = body;
            mc.IsBodyHtml = false;
            SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
            smtp.EnableSsl = true;
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            NetworkCredential nc = new NetworkCredential("190104113@aust.edu", "Rohit1234");
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = nc;
            try
            {
                smtp.Send(mc);
            }
            catch (Exception ex)
            {
                Debug.Write(ex);
            }

        }

        

    }
}