using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Statki.Models;

namespace Statki.Controllers
{
    public class AuthenticationController : Controller
    {
        // GET: Authentication
        public ActionResult Login(string name1, string name2)
        {
            if(string.IsNullOrEmpty(name1)==false && string.IsNullOrEmpty(name1) == false)
            {
                using (var db = new BattleShipContext())
                {
                    db.Database.ExecuteSqlCommand("TRUNCATE TABLE [Players]");
                    db.Database.ExecuteSqlCommand("TRUNCATE TABLE [Fields]");
                    db.Players.Add(new Player {Name = name1});
                    db.Players.Add(new Player { Name = name2 });
                    db.SaveChanges();
                }
                return RedirectToAction("ShowMap", "Game");
            }
            return View();
        }
    }
}