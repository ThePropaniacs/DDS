using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DDSDemoDAL;
using DDSDemo.Infrastructure.Authorization;
using System.Security.Claims;
using DDSDemo.Services;
using Microsoft.AspNet.Identity.Owin;
using System.Threading.Tasks;
using DDSDemo.Models;

namespace DDSDemo.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ClientsController : Controller
    {
        private DDSContext db = new DDSContext();
        private ApplicationDbContext dbb = ApplicationDbContext.Create();

        // GET: Clients
        public ActionResult Index()
        {
            return View(db.Clients.ToList());
        }

        // GET: Clients/Details/5
        public ActionResult Details(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Client client = db.Clients.Find(id);
            if (client == null)
            {
                return HttpNotFound();
            }
            return View(client);
        }

        // GET: Clients/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Clients/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "ID,CompanyName,EmployerID,EmployerName,Address1,Address2,City,State,Zip,Email,Phone")] Client client)
        {
            if (ModelState.IsValid)
            {
                ApplicationUserManager UserManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
                ApplicationUser exists = await UserManager.FindByEmailAsync(client.Email);
                if (exists == null)
                {
                    var new_client = db.Clients.Add(client);
                    db.SaveChanges();

                    var clientRegisterService = new ClientRegisterService();

                    var result = await clientRegisterService.RegisterClient(new_client, HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>());

                    if (result.Succeeded)
                    {
                        return RedirectToAction("Index");
                    }
                }
                return View(client);
            }
            return View(client);
        }

        // GET: Clients/AddUser/5
        public ActionResult AddUser(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Client client = db.Clients.Find(id);
            if (client == null)
            {
                return HttpNotFound();
            }
            return View(client);
        }

        // POST: Clients/AddUser/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddUser([Bind(Include = "ID, Email")]NewClientUserInputModel user)
        {
            if (ModelState.IsValid)
            {
                ApplicationUserManager UserManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
                ApplicationUser exists = await UserManager.FindByEmailAsync(user.Email);
                if (exists == null)
                {
                    var clientAddUserService = new ClientAddUserService();

                    var result = await clientAddUserService.AddUserClient(user, HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>());

                    if (result.Succeeded)
                    {
                        return RedirectToAction("Users/" + user.ID);
                    }
                }
                return View();
            }
            return View();
        }

        // GET: Clients/Edit/5
        public ActionResult Edit(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Client client = db.Clients.Find(id);
            if (client == null)
            {
                return HttpNotFound();
            }
            return View(client);
        }

        // POST: Clients/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,CompanyName,EmployerID,EmployerName,Address1,Address2,City,State,Zip,Email,Phone")] Client client)
        {
            if (ModelState.IsValid)
            {
                db.Entry(client).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(client);
        }

        // GET: Clients/Delete/5
        public ActionResult Delete(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Client client = db.Clients.Find(id);
            if (client == null)
            {
                return HttpNotFound();
            }
            return View(client);
        }

        // POST: Clients/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(decimal id)
        {
            //Client client = db.Clients.Find(id);
            ApplicationUserManager UserManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            ApplicationUser cliusers = dbb.Users.Where(u => u.Claims.Any(t => t.ClaimType == "ClientID" && t.ClaimValue == id.ToString())).FirstOrDefault();
            while (cliusers != null)
            {
                dbb.Users.Remove(cliusers);
                dbb.SaveChanges();
                cliusers = dbb.Users.Where(u => u.Claims.Any(t => t.ClaimType == "ClientID" && t.ClaimValue == id.ToString())).FirstOrDefault();
            }
            
            //db.Clients.Remove(client);
            //db.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: CLient/Users
        public ActionResult Users(decimal id)
        {
            var users = dbb.Users.ToList().Where(u => u.Claims.Any(t => t.ClaimType == "ClientID" && t.ClaimValue == id.ToString()));

            var currentClient = db.Clients.SingleOrDefault(e => e.ID == id);

            if (currentClient == null)
            {
                //TODO : Figure out more graceful way to handle this error
                throw new Exception("The client wasnt found");
            }

            var cliUsersViewModel = new ClientUsersViewModel
            {
                Client = currentClient,
                Users = users
            };

            return View(cliUsersViewModel);
        }

        // GET: Clients/DeleteUser/0bb0b0bb-0b0b-00bb-bb0b-00b000bb0000
        public ActionResult DeleteUser(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplicationUser user = dbb.Users.Find(id);
            var cliID = Convert.ToInt32(user.Claims.Where(u => u.ClaimType == "ClientID").Select(c => c.ClaimValue).SingleOrDefault());
            Client client = db.Clients.Find(cliID);
            if (user == null)
            {
                return HttpNotFound();
            }
            var userClientViewModel = new UserClientViewModel
            {
                Client = client,
                User = user
            };
            return View(userClientViewModel);
        }

        // POST: Clients/DeleteUser/0bb0b0bb-0b0b-00bb-bb0b-00b000bb0000
        [HttpPost, ActionName("DeleteUser")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteUserConfirmed(string id)
        {
            ApplicationUser user = dbb.Users.Find(id);
            var cliID = Convert.ToInt32(user.Claims.Where(u => u.ClaimType == "ClientID").Select(c => c.ClaimValue).SingleOrDefault());
            if (user != null)
            {
                dbb.Users.Remove(user);
                dbb.SaveChanges();
            }
            //db.Employees.Remove(employee);
            //db.SaveChanges();
            return RedirectToAction("Users/" + cliID);
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
                dbb.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
