using System.Web.Mvc;
using Statki.Models;

namespace Statki.Controllers
{
    public class AuthenticationController : Controller
    {
        // GET: Authentication
        public ActionResult Login(string name1, string name2)
        {
            if (string.IsNullOrEmpty(name1) == false || string.IsNullOrEmpty(name1) == false)
            {
                var obj = new Session();
                using (var db = new BattleShipContext())
                {
                    obj =new Session();
                    db.Sessions.Add(obj);
                    db.SaveChanges();
                    db.Players.Add(new Player { Name = name1, SessionId = obj.Id });
                    db.Players.Add(new Player { Name = name2, SessionId = obj.Id });
                    db.SaveChanges();
                }
                return RedirectToAction("ShowMap", "Game", new { sessionId = obj.Id });
            }
            return View();
        }
    }
}