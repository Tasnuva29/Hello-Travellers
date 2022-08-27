using Hello_Travellers.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Hello_Travellers.Controllers
{
    public class ChatController : Controller
    {

        private void SetChatList(String Username)
        {
            Entities db = new Entities();
            var sentUsers = db.Messages.Where(temp => temp.SenderUsername == Username).Select(temp => temp.Receiver).ToList();
            var receivedUsers = db.Messages.Where(temp => temp.ReceiverUsername == Username).Select(temp => temp.Sender).ToList();

            var chatList = sentUsers.Union(receivedUsers).ToList();
            ViewBag.ChatList = chatList;
        }

        public ActionResult Index()
        {
            if (Session["Username"] == null)
            {
                RedirectToAction("/");
            }
            SetChatList((string)Session["Username"]);
            ViewBag.Load = false;
            return View();
        }

        [HttpGet]
        public ActionResult Index(String Username)
        {
            if (Username == null)
            {
                return Index();
            }

            Entities db = new Entities();
            var currUsername = (string)Session["Username"];
            SetChatList(currUsername);
            var Messages = db.Messages
                .Where(temp => (temp.SenderUsername == currUsername && temp.ReceiverUsername == Username) || (temp.SenderUsername == Username && temp.ReceiverUsername == currUsername))
                .OrderByDescending(temp => temp.SentTime)
                .ToList();
            ViewBag.Load = true;
            ViewBag.Receiver = db.Users.Where(temp => temp.Username == Username).FirstOrDefault();
            ViewBag.Messages = Messages;
            return View();
        }

        [HttpGet]
        public ActionResult GetNewMessages(String Username, int CurrentCount)
        {
            Entities db = new Entities();
            var currUsername = (string)Session["Username"];
            var messageCount = db.Messages
                .Where(temp => (temp.SenderUsername == currUsername && temp.ReceiverUsername == Username) || 
                (temp.SenderUsername == Username && temp.ReceiverUsername == currUsername))
                .Count();
            if (messageCount > CurrentCount)
            {
                Hello_Travellers.Models.Message message = db.Messages.
                    OrderBy(temp => temp.SentTime).
                    Skip(CurrentCount).Take(1).FirstOrDefault();
                return PartialView("_MessageBox", message);
            }
            else
            {
                return Json("Empty", JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult SendMessage(String Username, String Content)
        {
            try
            {
                Entities db = new Entities();
                var currentUsername = (string)Session["Username"];
                Message message = new Message();
                message.SenderUsername = currentUsername;
                message.ReceiverUsername = Username;
                message.Content = Content;
                message.SentTime = DateTime.Now;
                db.Messages.Add(message);
                db.SaveChanges();
                return Json("Success", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json("Error", JsonRequestBehavior.AllowGet);
            }
        }

    }
}