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
using PagedList;
using PagedList.Mvc;

namespace DDSDemo.Controllers
{
    [Authorize(Roles = "Admin")]
    public class EmployeesController : Controller
    {
        private ApplicationDbContext dbb = ApplicationDbContext.Create();
        private DDSContext db = new DDSContext();

        // GET: Employees
        public ActionResult Index(string searchBy, string search, int? page, string sortBy)
        {
            ViewBag.CurrentPage = page;
            var data = db.Employees.AsQueryable();

            ViewBag.SortFirstNameParameter = string.IsNullOrEmpty(sortBy) ? "FirstName Desc" : "";
            ViewBag.SortLastNameParameter = sortBy == "LastName" ? "LastName Desc" : "LastName";

            if(searchBy == "FirstName")
            {
                data = data.Where(x => x.FirstName.StartsWith(search) || search == null);
            }
            else
            {
                data = data.Where(x => x.LastName.StartsWith(search) || search == null);
            }

            switch(sortBy)
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
        public ActionResult Users(decimal id, string searchBy, string search, int? page, string sortBy)
        {
            ViewBag.CurrentPage = page;
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
        public ActionResult Create(string searchBy, string search, int? page, string sortBy)
        {
            ViewBag.CurrentPage = page;
            return View();
        }

        // POST: Employees/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(string searchBy, string search, int? page, string sortBy, [Bind(Include = "ID,CompanyName,EmpID,FirstName,LastName,Sun,Mon,Tue,Wed,Thu,Fri,Sat,AvailNotes,AvailStart,AvailExpires,AvailDuration, Email")] EmployeeAccountViewModel employeeVm)
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
        public ActionResult AddUser(decimal id, string searchBy, string search, int? page, string sortBy)
        {
            ViewBag.CurrentPage = page;
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
        public async Task<ActionResult> AddUser(string searchBy, string search, int? page, string sortBy, [Bind(Include = "ID, Email")]NewEmployeeUserInputModel user)
        {
            ViewBag.CurrentPage = page;
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
                        return RedirectToAction("Users/" + user.ID, new { page = page, searchBy = Request.QueryString["searchBy"], search = Request.QueryString["search"], sortBy = Request.QueryString["sortBy"] });
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
        public ActionResult Edit(decimal id, string searchBy, string search, int? page, string sortBy)
        {
            ViewBag.CurrentPage = page;
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
        public ActionResult Edit(string searchBy, string search, int? page, string sortBy, [Bind(Include = "ID,CompanyName,EmpID,FirstName,LastName,Sun,Mon,Tue,Wed,Thu,Fri,Sat,AvailNotes,AvailStart,AvailExpires,AvailDuration")] Employee employee)
        {
            if (ModelState.IsValid)
            {
                db.Entry(employee).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", new { page = page, searchBy = Request.QueryString["searchBy"], search = Request.QueryString["search"], sortBy = Request.QueryString["sortBy"] });
            }
            return View(employee);
        }

        // GET: Employees/Delete/5
        public ActionResult Delete(decimal id, string searchBy, string search, int? page, string sortBy)
        {
            ViewBag.CurrentPage = page;
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
        public ActionResult DeleteConfirmed(decimal id, string searchBy, string search, int? page, string sortBy)
        {
            ViewBag.CurrentPage = page;
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
            return RedirectToAction("Index", new { page = page, searchBy = Request.QueryString["searchBy"], search = Request.QueryString["search"], sortBy = Request.QueryString["sortBy"] });
        }

        // GET: Employees/DeleteUser/0bb0b0bb-0b0b-00bb-bb0b-00b000bb0000
        public ActionResult DeleteUser(string id, string searchBy, string search, int? page, string sortBy)
        {
            ViewBag.CurrentPage = page;
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
        public ActionResult DeleteUserConfirmed(string id, string searchBy, string search, int? page, string sortBy)
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
            return RedirectToAction("Users/" + empID, new { page = page, searchBy = Request.QueryString["searchBy"], search = Request.QueryString["search"], sortBy = Request.QueryString["sortBy"] });
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
