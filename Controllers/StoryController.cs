using Hello_Travellers.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Hello_Travellers.Controllers
{
    public class StoryController : Controller
    {
        // GET: Story
        public ActionResult Story(int forumID)
        {
            Entities db = new Entities();
            var r = db.Posts.Where(m => m.ForumID == forumID).ToList();
            ViewBag.r = r;
            return View(r);
        }
           
    }
}

//@foreach(var item in Model)
//{
    
//    @*ViewBag.item = item;
//    Html.RenderPartial("_Post"); *@





    

//}