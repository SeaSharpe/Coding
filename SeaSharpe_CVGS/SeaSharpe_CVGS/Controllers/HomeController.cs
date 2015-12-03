using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SeaSharpe_CVGS.Models;

namespace SeaSharpe_CVGS.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            IQueryable<Game> listOfGames = db.Games.OrderByDescending(x=>x.Id).Take(8);
            return View(listOfGames.ToList());
        }

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

        public JsonResult SeedDatabase()
        {
            var db = new SeaSharpe_CVGS.Models.ApplicationDbContext();
            try
            {
                (new SeaSharpe_CVGS.Migrations.Configuration()).SeedDebug(db);
            }
            catch (Exception e)
            {
                return Json(new { Success = false, Exception = new { Message = e.Message, Stacktrace = e.StackTrace } }, JsonRequestBehavior.AllowGet);
            }
            return Json(new
            {
                Success = true,
                Users = db.Users.Count(),
                Employees = db.Employees.Count(),
                Members = db.Members.Count(),
                Events = db.Events.Count(),
                Addresses = db.Addresses.Count(),
                Orders = db.Orders.Count(),
                Games = db.Games.Count(),
                Platforms = db.Platforms.Count(),
                Categories = db.Catagories.Count(),
                Reviews = db.Reviews.Count(),
            }, JsonRequestBehavior.AllowGet);
        }
    }
}