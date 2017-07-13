using DDSDemo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.Entity;
using System.Net;
using DDSDemoDAL;
using DDSDemo.Infrastructure.Authorization;
using System.Security.Claims;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using DDSDemo.Services;
using System.Threading.Tasks;

namespace DDSDemo.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public AdminController()
        {
        }

        public AdminController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        private ApplicationDbContext db = ApplicationDbContext.Create();
        private DDSContext dbb = new DDSContext();

        // GET: Admin
        public ActionResult Index()
        {
            var users = UserManager.Users.ToList().Where(u => u.Claims.Any(t => t.ClaimType == "Admin"));

            return View(users);
        }
        // GET: Admin/Details/0bb0b0bb-0b0b-00bb-bb0b-00b000bb0000
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplicationUser user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // GET: Admin/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "FirstName,LastName,Email,EmailConfirmed,PasswordHash,SecurityStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEndDateUtc,LockoutEnabled,AccessFailedCount,UserName")] ApplicationUser user)
        {
            if (ModelState.IsValid)
            {
                ApplicationUserManager UserManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
                ApplicationUser exists = await UserManager.FindByEmailAsync(user.Email);
                if (exists == null)
                {
                    var adminRegisterService = new AdminRegisterService();

                    var result = await adminRegisterService.RegisterAdmin(user, HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>());

                    if (result.Succeeded)
                    {
                        return RedirectToAction("Index");
                    }
                }
                return View(user);
            }
            return View(user);
        }

        // GET: Admin/Edit/0bb0b0bb-0b0b-00bb-bb0b-00b000bb0000
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplicationUser user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: Admin/Edit/0bb0b0bb-0b0b-00bb-bb0b-00b000bb0000
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "FirstName,LastName,Email,EmailConfirmed,PasswordHash,SecurityStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEndDateUtc,LockoutEnabled,AccessFailedCount,UserName")] ApplicationUser user)
        {
            if (ModelState.IsValid)
            {
                var _user = UserManager.FindByEmail(user.Email);

                if(_user == null)
                {
                    ModelState.AddModelError("I dont even know how there could have been an error here", "You suck");
                    return View(user);
                }

                _user.FirstName = user.FirstName;
                _user.LastName = user.LastName;
                _user.Email = user.Email;
                _user.PhoneNumber = user.PhoneNumber;
                _user.UserName = user.UserName;

                IdentityResult result = UserManager.Update(_user);

                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
            }

            ModelState.AddModelError("Something went wrong", "It wasnt me");
            return View(user);
        }

        // GET: Admin/Delete/0bb0b0bb-0b0b-00bb-bb0b-00b000bb0000
        public ActionResult Delete(String id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplicationUser user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: Admin/Delete/0bb0b0bb-0b0b-00bb-bb0b-00b000bb0000
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            ApplicationUser user = db.Users.Find(id);
            db.Users.Remove(user);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}

        
