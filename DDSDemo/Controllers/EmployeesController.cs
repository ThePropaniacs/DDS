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
using DDSDemo.Infrastructure;

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

        //// GET: Employees/Details/5
        //public ActionResult Details(decimal id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Employee employee = db.Employees.Find(id);
        //    if (employee == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(employee);
        //}

        // GET: Employee/Users
        public ActionResult Users(decimal id)
        {
            var users = dbb.Users.ToList().Where(u => u.Claims.Any(t => t.ClaimType == "EmployeeID" && t.ClaimValue == id.ToString()));

            var currentEmployee = db.Employees.SingleOrDefault(e => e.ID == id);

            if(currentEmployee == null)
            {
                //TODO : Figure out more graceful way to handle this error
                throw new Exception("The employee wasnt found");
            }

            var empUsersViewModel = new EmployeeUsersViewModel
            {
                Employee = currentEmployee,
                Users = users
            };

            return View(empUsersViewModel);
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
        public async Task<ActionResult> Create([Bind(Include = "ID,CompanyName,EmpID,FirstName,LastName,Sun,Mon,Tue,Wed,Thu,Fri,Sat,AvailNotes,AvailStart,AvailExpires,AvailDuration, Email")] EmployeeAccountViewModel employeeVm)
        {
            if (ModelState.IsValid)
            {
                var employee = new Employee();

                employee.EmployeeAccountVmToEmployee(employeeVm);

                ApplicationUserManager UserManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
                ApplicationUser exists = await UserManager.FindByEmailAsync(employeeVm.Email);

                if (exists == null)
                {
                    var new_employee = db.Employees.Add(employee);
                    db.SaveChanges();

                    employeeVm.ID = new_employee.ID;

                    var employeeRegisterService = new EmployeeRegisterService();

                    var result = await employeeRegisterService.RegisterEmployee(employeeVm, HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>());

                    if (result.Succeeded)
                    {
                        return RedirectToAction("Index");
                    }
                }
                else
                {
                    ViewBag.EmailTaken = "Email already in use";
                    return View(employeeVm);
                }              
            }
            return View(employeeVm);
        }

        // GET: Employee/AddUser/5
        public ActionResult AddUser(decimal id)
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

            EmployeeAccountViewModel employeeVm = new EmployeeAccountViewModel();

            employeeVm.ID = employee.ID;
            employeeVm.FirstName = employee.FirstName;
            employeeVm.LastName = employee.LastName;
            

            
            return View(employeeVm);
        }

        // POST: Employee/AddUser/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddUser([Bind(Include = "ID, Email")]NewEmployeeUserInputModel user)
        {
            Employee employee = db.Employees.Find(user.ID);
            EmployeeAccountViewModel employeeVm = new EmployeeAccountViewModel();
            employeeVm.ID = employee.ID;
            employeeVm.FirstName = employee.FirstName;
            employeeVm.LastName = employee.LastName;

            if (ModelState.IsValid)
            {
                ApplicationUserManager UserManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
                ApplicationUser exists = await UserManager.FindByEmailAsync(user.Email);
                if (exists == null)
                {
                    var employeeAddUserService = new EmployeeAddUserService();

                    var result = await employeeAddUserService.AddUserEmployee(user, HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>());

                    if (result.Succeeded)
                    {
                        return RedirectToAction("Users/" + user.ID);
                    }
                }
                else
                {
                    ViewBag.EmailTaken = "Email already in use";
                    return View(employeeVm);
                }
            }
            return View(employeeVm);
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
            //Employee employee = db.Employees.Find(id);
            ApplicationUserManager UserManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            ApplicationUser empuser = dbb.Users.Where(u => u.Claims.Any(t => t.ClaimType == "EmployeeID" && t.ClaimValue == id.ToString())).FirstOrDefault();
            while (empuser != null)
            {
                dbb.Users.Remove(empuser);
                dbb.SaveChanges();
                empuser = dbb.Users.Where(u => u.Claims.Any(t => t.ClaimType == "EmployeeID" && t.ClaimValue == id.ToString())).FirstOrDefault();

            }
            //db.Employees.Remove(employee);
            //db.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: Employees/DeleteUser/0bb0b0bb-0b0b-00bb-bb0b-00b000bb0000
        public ActionResult DeleteUser(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplicationUser user = dbb.Users.Find(id);
            var empID = Convert.ToInt32(user.Claims.Where(u => u.ClaimType == "EmployeeID").Select(c => c.ClaimValue).SingleOrDefault());
            Employee employee = db.Employees.Find(empID);
            if (user == null)
            {
                return HttpNotFound();
            }
            var userEmployeeViewModel = new UserEmployeeViewModel
            {
                Employee = employee,
                User = user
            };
            return View(userEmployeeViewModel);
        }

        // POST: Employees/DeleteUser/0bb0b0bb-0b0b-00bb-bb0b-00b000bb0000
        [HttpPost, ActionName("DeleteUser")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteUserConfirmed(string id)
        {
            ApplicationUser user = dbb.Users.Find(id);
            var empID = Convert.ToInt32(user.Claims.Where(u => u.ClaimType == "EmployeeID").Select(c => c.ClaimValue).SingleOrDefault());
            if (user != null)
            {
                dbb.Users.Remove(user);
                dbb.SaveChanges();
            }
            //db.Employees.Remove(employee);
            //db.SaveChanges();
            return RedirectToAction("Users/" + empID);
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
