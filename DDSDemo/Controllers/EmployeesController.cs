using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DDSDemoDAL;
using DDSDemo.Models;
using Microsoft.AspNet.Identity;
using DDSDemo.Services;
using Microsoft.AspNet.Identity.Owin;
using System.Threading.Tasks;

namespace DDSDemo.Controllers
{
    [Authorize(Roles = "Admin")]
    public class EmployeesController : Controller
    {
        private ApplicationDbContext dbb = ApplicationDbContext.Create();
        private DDSContext db = new DDSContext();

        // GET: Employees
        public ActionResult Index()
        {
            return View(db.Employees.ToList());
        }

        // GET: Employees/Details/5
        public ActionResult Details(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employee employee = db.Employees.Find(id);
            if (employee == null)
            {
                return HttpNotFound();
            }
            return View(employee);
        }

        // GET: Employees/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Employees/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "ID,CompanyName,EmpID,FirstName,LastName,Sun,Mon,Tue,Wed,Thu,Fri,Sat,AvailNotes,AvailStart,AvailExpires,AvailDuration, Email")] Employee employee)
        {
            if (ModelState.IsValid)
            {
                ApplicationUserManager UserManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
                ApplicationUser exists = await UserManager.FindByEmailAsync(employee.Email);
                if (exists == null)
                {
                    var new_employee = db.Employees.Add(employee);
                    db.SaveChanges();

                    var employeeRegisterService = new EmployeeRegisterService();

                    var result = await employeeRegisterService.RegisterEmployee(new_employee, HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>());

                    if (result.Succeeded)
                    {
                        return RedirectToAction("Index");
                    }
                }
                return View(employee);                
            }
            return View(employee);
        }

        // GET: Employees/Edit/5
        public ActionResult Edit(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employee employee = db.Employees.Find(id);
            if (employee == null)
            {
                return HttpNotFound();
            }
            return View(employee);
        }

        // POST: Employees/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,CompanyName,EmpID,FirstName,LastName,Sun,Mon,Tue,Wed,Thu,Fri,Sat,AvailNotes,AvailStart,AvailExpires,AvailDuration")] Employee employee)
        {
            if (ModelState.IsValid)
            {
                db.Entry(employee).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(employee);
        }

        // GET: Employees/Delete/5
        public ActionResult Delete(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employee employee = db.Employees.Find(id);
            if (employee == null)
            {
                return HttpNotFound();
            }
            return View(employee);
        }

        // POST: Employees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(decimal id)
        {
            Employee employee = db.Employees.Find(id);
            ApplicationUserManager UserManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var user = dbb.Users.Where(u => u.Claims.Any(t => t.ClaimType == "EmployeeID" && t.ClaimValue == id.ToString()));
            ApplicationUser empuser = user.First();
            db.Employees.Remove(employee);
            dbb.Users.Remove(empuser);
            db.SaveChanges();
            dbb.SaveChanges();
            return RedirectToAction("Index");
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
