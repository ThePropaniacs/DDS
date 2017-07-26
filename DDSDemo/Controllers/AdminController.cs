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
        public ActionResult Index(string searchBy, string search, int? page, string sortBy)
        {
            ViewBag.CurrentPage = page;
            var users = UserManager.Users.Where(u => u.Claims.Any(t => t.ClaimType == "Admin"));

            var data = users.AsQueryable();

            ViewBag.SortFirstNameParameter = string.IsNullOrEmpty(sortBy) ? "FirstName Desc" : "";
            ViewBag.SortLastNameParameter = sortBy == "LastName" ? "LastName Desc" : "LastName";

            if (searchBy == "FirstName")
            {
                data = data.Where(x => x.FirstName.StartsWith(search) || search == null);
            }
            else
            {
                data = data.Where(x => x.LastName.StartsWith(search) || search == null);
            }

            switch (sortBy)
            {
                case "FirstName Desc":
                    data = data.OrderByDescending(x => x.FirstName);
                    break;
                case "LastName":
                    data = data.OrderBy(x => x.LastName);
                    break;
                case "LastName Desc":
                    data = data.OrderByDescending(x => x.LastName);
                    break;
                default:
                    data = data.OrderBy(x => x.FirstName);
                    break;
            }




            return View(data.ToPagedList(page ?? 1, 10));
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
        public ActionResult Create(string searchBy, string search, int? page, string sortBy)
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
        public async Task<ActionResult> Create(string searchBy, string search, int? page, string sortBy, [Bind(Include = "FirstName,LastName,Email,PhoneNumber")] NewAdminUserInputModel admin)
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
        public ActionResult Edit(string id, string searchBy, string search, int? page, string sortBy)
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
        public ActionResult Edit(string searchBy, string search, int? page, string sortBy, [Bind(Include = "Id,FirstName,LastName,Email,EmailConfirmed,PasswordHash,SecurityStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEndDateUtc,LockoutEnabled,AccessFailedCount,UserName")] ApplicationUser user)
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
                            return RedirectToAction("Index", new { page = page, searchBy = Request.QueryString["searchBy"], search = Request.QueryString["search"], sortBy = Request.QueryString["sortBy"] });
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
        public ActionResult Delete(String id, string searchBy, string search, int? page, string sortBy)
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
        public ActionResult DeleteConfirmed(string id, string searchBy, string search, int? page, string sortBy)
        {
            ViewBag.CurrentPage = page;
            ApplicationUser user = db.Users.Find(id);
            db.Users.Remove(user);
            db.SaveChanges();
            return RedirectToAction("Index", new { page = page, searchBy = Request.QueryString["searchBy"], search = Request.QueryString["search"], sortBy = Request.QueryString["sortBy"] });
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

        
