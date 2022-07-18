using System.Web.Mvc;
using Hello_Travellers.Models;

namespace Hello_Travellers.Controllers
{
    public class UserAuthController : Controller
    {
        // GET
        public ActionResult Login()
        {
            return View();
        }

        public ActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        public ActionResult SignUp(User user)
        {
            Entities db = new Entities();
            db.Users.Add(user);
            db.SaveChanges();
            db.Dispose();
            return View(user);
        }
    }
}