﻿using Hello_Travellers.Models;
using System;
using System.Collections.Generic;
using System.Linq;
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
            return View();
        }

        public ActionResult Users()
        {
            Entities db = new Entities();
            var count = db.Users.Where(temp => temp.Rank == "USER").Count();
            var userList = db.Users.Where(temp => temp.Rank == "USER").ToList();
            ViewBag.count = count;
            ViewBag.userList = userList;
            return View();
        }
        public ActionResult Admins()
        {
            Entities db = new Entities();
            var adminList = db.Users.Where(temp => temp.Rank == "ADMIN").ToList();
            ViewBag.adminList = adminList;
            var count = db.Users.Where(temp => temp.Rank == "ADMIN").Count();
            ViewBag.count = count;
            return View();
        }

        public ActionResult Subforums()
        {
            Entities db = new Entities();
            var subForums = db.Subforums.ToList();
            ViewBag.subForum = subForums;
            var count = db.Subforums.Count();
            ViewBag.count = count;
            return View();
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
            Entities db = new Entities();
            var reportCount = db.Reports.Count();
            var postReportCount = db.Reports.Where(temp => temp.Context == "POST").Count();
            var profileReportCount = db.Reports.Where(temp => temp.Context == "PROFILE").Count();
            var commentReportCount = db.Reports.Where(temp => temp.Context == "COMMENT").Count();
            var postReport = db.Reports.Where(temp => temp.Context == "POST").ToList();
            var profileReport = db.Reports.Where(temp => temp.Context == "PROFILE").ToList();
            var commentReport = db.Reports.Where(temp => temp.Context == "COMMENT").ToList();
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
        [HttpPost]
        public ActionResult Ban()
        {
            return Json("true", JsonRequestBehavior.AllowGet);
        }
        public ActionResult Posts()
        {
            Entities db = new Entities();
            var subforumList = db.Subforums.ToList();
            ViewBag.subForumList = subforumList;
            var postList = db.Posts.ToList();
            ViewBag.postList = postList;
            return View();
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
                foreach (var item in post.Replies.ToList())
                {
                    db.Replies.Remove(item);
                }
                foreach (var item in post.MediaItems.ToList())
                {
                    db.MediaItems.Remove(item);
                }
                db.Posts.Remove(post);
                db.SaveChanges();
                UpdateStatus(ReportID);
                return Json("post", JsonRequestBehavior.AllowGet);
            }
            else
            {
                var reply = db.Replies.Where(temp => temp.ReplyID.ToString() == ContextID).FirstOrDefault();
                db.Replies.Remove(reply);
                db.SaveChanges();
                UpdateStatus(ReportID);
                return Json("comment", JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public ActionResult DismissReport(int ReportID)
        {
            UpdateStatus(ReportID);
            return Json("true", JsonRequestBehavior.AllowGet);
        }

        public ActionResult Settings()
        {
            return View();
        }
        public ActionResult BannedUser()
        {
            Entities db = new Entities();
            var userList = db.Users.Where(temp => temp.Rank == "BANNED").ToList();
            ViewBag.userList = userList;
            return View();
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
                "\"Comment\": \"{4}\"", comment.User.Name, comment.User.Username, comment.User.DisplayPictureName, comment.PostID, comment.Content);
            return Json('{' + jsonString + '}', JsonRequestBehavior.AllowGet);
        }
    }
}