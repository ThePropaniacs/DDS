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
using static DDSDemo.Controllers.ManageController;
using PagedList;
using PagedList.Mvc;

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
        public ActionResult Index(int? page)
        {
            var users = UserManager.Users.ToList().AsQueryable().Where(u => u.Claims.Any(t => t.ClaimType == "Admin"));

            return View(users.ToPagedList(page ?? 1, 10));
        }
        //// GET: Admin/Details/0bb0b0bb-0b0b-00bb-bb0b-00b000bb0000
        //public ActionResult Details(string id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    ApplicationUser user = db.Users.Find(id);
        //    if (user == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(user);
        //}

        // GET: Admin/Create
        public ActionResult Create(int? page)
        {
            ViewBag.CurrentPage = page;
            ViewBag.EmailTaken = "";
            return View();
        }

        // POST: Admin/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(int? page,[Bind(Include = "FirstName,LastName,Email,PhoneNumber")] NewAdminUserInputModel admin)
        {
            ViewBag.CurrentPage = page;
            if (ModelState.IsValid)
            {
                ApplicationUserManager UserManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
                ApplicationUser exists = await UserManager.FindByEmailAsync(admin.Email);
                if (exists == null)
                {
                    var adminRegisterService = new AdminRegisterService();

                    var result = await adminRegisterService.RegisterAdmin(admin, HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>());

                    if (result.Succeeded)
                    {
                        return RedirectToAction("Index");
                    }
                }
                ViewBag.EmailTaken = "Email already in use";
                return View(admin);
            }
            return View(admin);
        }
        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }
        


        // GET: Admin/Edit/0bb0b0bb-0b0b-00bb-bb0b-00b000bb0000
        public ActionResult Edit(string id, int? page)
        {
            ViewBag.CurrentPage = page;
            ViewBag.EmailTaken = "";

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
        public ActionResult Edit(int? page,[Bind(Include = "Id,FirstName,LastName,Email,EmailConfirmed,PasswordHash,SecurityStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEndDateUtc,LockoutEnabled,AccessFailedCount,UserName")] ApplicationUser user)
        {
            ViewBag.CurrentPage = page;
            if (ModelState.IsValid)
            {
                if (user.Email != null)
                {
                    var _user = UserManager.FindById(user.Id);

                    if (_user == null)
                    {
                        ModelState.AddModelError("I dont even know how there could have been an error here", "You suck");
                        return View(user);
                    }

                    var exists = UserManager.FindByEmail(user.Email);

                    if (_user.Email == user.Email)
                    {
                        exists = null;
                    }

                    if (exists == null)
                    {
                        _user.FirstName = user.FirstName;
                        _user.LastName = user.LastName;
                        _user.Email = user.Email;
                        _user.PhoneNumber = user.PhoneNumber;
                        _user.UserName = user.Email;

                        IdentityResult result = UserManager.Update(_user);

                        if (result.Succeeded)
                        {
                            return RedirectToAction("Index", new { page = page});
                        }
                    }
                    else
                    {
                        ViewBag.EmailTaken = "Email already in use";
                        return View(user);
                    }
                }
                else
                {
                    ViewBag.EmailTaken = "This field is required";
                    return View(user);
                }
            }
            ModelState.AddModelError("Something went wrong", "It wasnt me");
            ViewBag.EmailTaken = "Invalid Email Address";
            return View(user);
        }

        // GET: Admin/Delete/0bb0b0bb-0b0b-00bb-bb0b-00b000bb0000
        public ActionResult Delete(String id, int? page)
        {
            ViewBag.CurrentPage = page;
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
        public ActionResult DeleteConfirmed(string id, int? page)
        {
            ViewBag.CurrentPage = page;
            ApplicationUser user = db.Users.Find(id);
            db.Users.Remove(user);
            db.SaveChanges();
            return RedirectToAction("Index", new { page = page });
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

        
