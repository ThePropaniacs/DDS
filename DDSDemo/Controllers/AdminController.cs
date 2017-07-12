using DDSDemo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DDSDemo.Controllers
{
    public class AdminController : Controller
    {
        private ApplicationDbContext db = ApplicationDbContext.Create();

        // GET: Admin
        public ActionResult Index()
        {
            var users = db.Users.ToList();
            return View(users);
        }
    }
}