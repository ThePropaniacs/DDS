using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DDSDemoDAL;
using PagedList;
using PagedList.Mvc;
using DDSDemo.Infrastructure.Authorization;
using System.Security.Claims;
using DDSDemo.Models;
using DDSDemo.Infrastructure;

namespace DDSDemo.Controllers
{
    [Authorize]
    public class TimeSheetsController : Controller
    {
        private DDSContext db = new DDSContext();

        //http://localhost:8888/Timesheets/EmployeeIndex

        // GET: TimeSheets
        //Admin index
        [AllowAnonymous]
        public ActionResult Index(string searchBy, string search, int? page, string sortBy)
        {
            if (User.IsInRole("Admin"))
            {

                var tblTimeSheetMasters = db.TimeSheets.Include(t => t.Client).Include(t => t.Employee).AsQueryable();
                var data = tblTimeSheetMasters;

                ViewBag.SortClientParameter = string.IsNullOrEmpty(sortBy) ? "Client Desc" : "";
                ViewBag.SortEmployeeParameter = sortBy == "Employee" ? "Employee Desc" : "Employee";
                ViewBag.SortStartTimeParameter = sortBy == "Start Time" ? "Start Time Desc" : "Start Time";
                ViewBag.SortStopTimeParameter = sortBy == "Stop Time" ? "Stop Time Desc" : "Stop Time";
                ViewBag.SortElapsedTimeParameter = sortBy == "Elapsed Time" ? "Elapsed Time Desc" : "Elapsed Time";


                if (searchBy == "Client")
                {
                    data = data.Where(x => x.Client.CompanyName.StartsWith(search) || search == null);
                }
                else
                {
                    data = data.Where(x => x.Employee.FirstName.Contains(search) || x.Employee.LastName.Contains(search) || search.Contains(x.Employee.FirstName) || search.Contains(x.Employee.LastName) || search == null);
                }
                switch(sortBy)
                {
                    case "Client Desc":
                        data = data.OrderByDescending(x => x.Client.CompanyName);
                        break;
                    case "Employee Desc":
                        data = data.OrderByDescending(x => x.Employee.FirstName);
                        break;
                    case "Employee":
                        data = data.OrderBy(x => x.Employee.FirstName);
                        break;
                    case "Start Time Desc":
                        data = data.OrderByDescending(x => x.StartTime);
                        break;
                    case "Start Time":
                        data = data.OrderBy(x => x.StartTime);
                        break;
                    case "Stop Time Desc":
                        data = data.OrderByDescending(x => x.StopTime);
                        break;
                    case "Stop Time":
                        data = data.OrderBy(x => x.StopTime);
                        break;
                    default:
                        data = data.OrderBy(x => x.Client.CompanyName);
                        break;
                }

                return View(data.ToPagedList(page ?? 1, 10));
            }
            else if (User.IsInRole("Employee"))
            {
                return RedirectToAction("EmployeeIndex", "TimeSheets", new { page = page });
            }
            else if (User.IsInRole("Client"))
            {
                return RedirectToAction("ClientIndex", "TimeSheets", new { page = page });
            }
            else
            {
                return RedirectToAction("LogIn", "Account");
            }
        }

        [ClaimsAccess(ClaimType = "EmployeeID")]
        public ActionResult EmployeeIndex(int? page)
        {
            var employeeID = Int32.Parse((this.HttpContext.User.Identity as ClaimsIdentity).Claims.FirstOrDefault(c => c.Type == "EmployeeID").Value);
            var timesheets = db.TimeSheets.Include(t => t.Client).Include(t => t.Employee).Where(t => t.Employee.ID == employeeID);
            return View(timesheets.OrderByDescending(x => x.ID).ToPagedList(page ?? 1, 10));
        }

        [ClaimsAccess(ClaimType = "ClientID")]
        public ActionResult ClientIndex(int? page)
        {
            var clientID = Int32.Parse((this.HttpContext.User.Identity as ClaimsIdentity).Claims.FirstOrDefault(c => c.Type == "ClientID").Value);
            var timesheets = db.TimeSheets.Include(t => t.Client).Include(t => t.Employee).Where(c => c.Client.ID == clientID);

            var timesheetsForList = new List<TimeSheetForList>();

            foreach (var ts in timesheets)
            {
                var timesheetForList = new TimeSheetForList();

                timesheetForList.TimesheetToTimesheetForList(ts);


                timesheetsForList.Add(timesheetForList);
            }

            return View(timesheetsForList.OrderByDescending(x => x.StartTime).ToPagedList(page ?? 1, 10));
        }

        [ClaimsAccess(ClaimType = "ClientID")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ClientIndex(int? page, List<TimeSheetForList> timesheetsFromList, String Approve, String Deny, [Bind(Include = "ID,CompanyName,EmpID,AssocClientID,StartTime,StopTime,Note,Approved,ApprovedBy,ApprovedDate,Processed")] TimeSheet timeSheet)
        {
            bool ActionToTake = true;

            if (String.IsNullOrEmpty(Approve))
            {
                ActionToTake = false;
            }

            foreach (var timesheet in timesheetsFromList)
            {
                var ts = db.TimeSheets.Find(timesheet.ID);

                if (timesheet.IsChecked)
                {
                    ts.Approved = ActionToTake;
                    ts.ApprovedDate = DateTime.Now;
                }

                timesheet.TimesheetToTimesheetForList(ts);
            }
            db.SaveChanges();

            return RedirectToAction("ClientIndex", new { page = page });
        }
        [Authorize(Roles = "Admin, Client")]
        public ActionResult ClientEdit(decimal id, int? page)
        {
            ViewBag.CurrentPage = page;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TimeSheet timeSheet = db.TimeSheets.Find(id);
            if (timeSheet == null)
            {
                return HttpNotFound();
            }
            return View(timeSheet);
        }

        // POST: TimeSheets/Manage
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin, Client")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ClientEdit([Bind(Include = "ID,CompanyName,EmpID,AssocClientID,StartTime,StopTime,Note,Approved,ApprovedBy,ApprovedDate,Processed")] TimeSheet timeSheet, int? page)
        {
            if (ModelState.IsValid)
            {
                TimeSheet _timeSheet = db.TimeSheets.Find(timeSheet.ID);

                if (!_timeSheet.StopTime.HasValue)
                {
                    _timeSheet.StopTime = DateTime.Now;
                }
                _timeSheet.Approved = timeSheet.Approved;
                _timeSheet.Note = timeSheet.Note;
                _timeSheet.ApprovedDate = DateTime.Now;
                db.SaveChanges();
                return RedirectToAction("ClientIndex", new { page = page });
            }
            return View(timeSheet);
        }
        

        [Authorize(Roles = "Admin, Employee")]
        public ActionResult EmployeeDetails(decimal id, int? page)
        {
            ViewBag.CurrentPage = page;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TimeSheet timeSheet = db.TimeSheets.Find(id);
            if (timeSheet == null)
            {
                return HttpNotFound();
            }
            return View(timeSheet);
        }
        // GET: TimeSheets/EmployeeCreate
        [Authorize(Roles = "Admin, Employee")]
        public ActionResult EmployeeCreate()
        {
            if (User.IsInRole("Admin"))
            {
                return RedirectToAction("Create", "TimeSheets");
            }
            else if (User.IsInRole("Employee"))
            {
                var employeeID = Int32.Parse((this.HttpContext.User.Identity as ClaimsIdentity).Claims.FirstOrDefault(c => c.Type == "EmployeeID").Value);
                ViewBag.AssocClientID = new SelectList(db.Clients, "ID", "CompanyName");
                ViewBag.EmpID = new SelectList(db.Employees.Where(i => i.ID == employeeID), "ID", "FullName");
                return View();
            }
            else
            {
                return RedirectToAction("Index", "TimeSheets");
            }
        }

        // POST: TimeSheets/EmployeeCreate
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin, Employee")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EmployeeCreate([Bind(Include = "ID,CompanyName,EmpID,AssocClientID,StartTime,StopTime,Note,Approved,ApprovedBy,ApprovedDate,Processed")] TimeSheet timeSheet)
        {
            if (ModelState.IsValid)
            {
                timeSheet.StartTime = DateTime.Now;
                timeSheet = db.TimeSheets.Add(timeSheet);
                db.SaveChanges();
                if (User.IsInRole("Employee"))
                {
                    return RedirectToAction("EmployeeManage", new { id = timeSheet.ID });
                }
                else
                {
                    return RedirectToAction("Manage", new { id = timeSheet.ID });
                }
            }

            ViewBag.AssocClientID = new SelectList(db.Clients, "ID", "CompanyName", timeSheet.AssocClientID);
            ViewBag.EmpID = new SelectList(db.Employees, "ID", "FullName", timeSheet.EmpID);
            return View(timeSheet);
        }
        // GET: TimeSheets/Manage/5
        [Authorize(Roles = "Admin, Employee")]
        public ActionResult EmployeeManage(decimal id, int? page)
        {
            ViewBag.CurrentPage = page;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TimeSheet timeSheet = db.TimeSheets.Find(id);
            if (timeSheet == null)
            {
                return HttpNotFound();
            }

            return View(timeSheet);
        }

        // POST: TimeSheets/Manage
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin, Employee")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EmployeeManage([Bind(Include = "ID,CompanyName,EmpID,AssocClientID,StartTime,StopTime,Note,Approved,ApprovedBy,ApprovedDate,Processed")] TimeSheet timeSheet, int? page)
        {
            if (ModelState.IsValid)
            {
                TimeSheet _timeSheet = db.TimeSheets.Find(timeSheet.ID);

                if (!_timeSheet.StopTime.HasValue)
                {
                    _timeSheet.StopTime = DateTime.Now;
                }

                _timeSheet.Note = timeSheet.Note;
                db.SaveChanges();
                return RedirectToAction("EmployeeIndex", new { page = page });
            }
            return View(timeSheet);
        }

        // GET: TimeSheets/Details/5
        [Authorize(Roles = "Admin")]
        public ActionResult Details(decimal id, int? page)
        {
            ViewBag.CurrentPAge = page;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TimeSheet timeSheet = db.TimeSheets.Find(id);
            if (timeSheet == null)
            {
                return HttpNotFound();
            }
            return View(timeSheet);
        }
        

        // GET: TimeSheets/Create
        [Authorize(Roles = "Admin, Employee")]
        public ActionResult Create()
        {
            if (User.IsInRole("Admin"))
            {
                ViewBag.AssocClientID = new SelectList(db.Clients, "ID", "CompanyName");
                ViewBag.EmpID = new SelectList(db.Employees, "ID", "FullName");
                return View();
            }
            else if (User.IsInRole("Employee"))
            {
                return RedirectToAction("EmployeeCreate", "TimeSheets");
            }
            else
            {
                return RedirectToAction("Index", "TimeSheets");
            }
        }

        // POST: TimeSheets/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin, Employee")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,CompanyName,EmpID,AssocClientID,StartTime,StopTime,Note,Approved,ApprovedBy,ApprovedDate,Processed")] TimeSheet timeSheet)
        {
            if (ModelState.IsValid)
            {
                timeSheet.StartTime = DateTime.Now;
                timeSheet = db.TimeSheets.Add(timeSheet);
                db.SaveChanges();
                if (User.IsInRole("Employee"))
                {
                    return RedirectToAction("EmployeeManage", new { id = timeSheet.ID });
                }
                else
                {
                    return RedirectToAction("Manage", new { id = timeSheet.ID });
                }
            }

            ViewBag.AssocClientID = new SelectList(db.Clients, "ID", "CompanyName", timeSheet.AssocClientID);
            ViewBag.EmpID = new SelectList(db.Employees, "ID", "FullName", timeSheet.EmpID);
            return View(timeSheet);
        }

        


        // GET: TimeSheets/Manage/5
        [Authorize(Roles = "Admin")]
        public ActionResult Manage(decimal id, int? page)
        {
            ViewBag.CurrentPage = page;
            if(id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TimeSheet timeSheet = db.TimeSheets.Find(id);
            if(timeSheet == null)
            {
                return HttpNotFound();
            }
            ViewBag.AssocClientID = new SelectList(db.Clients, "ID", "CompanyName", timeSheet.AssocClientID);
            ViewBag.EmpID = new SelectList(db.Employees, "ID", "FullName", timeSheet.EmpID);
            return View(timeSheet);
        }

        // POST: TimeSheets/Manage
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Manage([Bind(Include = "ID,CompanyName,EmpID,AssocClientID,StartTime,StopTime,Note,Approved,ApprovedBy,ApprovedDate,Processed")] TimeSheet timeSheet, int? page)
        {
            if (ModelState.IsValid)
            {
                TimeSheet _timeSheet = db.TimeSheets.Find(timeSheet.ID);

                if (!_timeSheet.StopTime.HasValue)
                {
                    _timeSheet.StopTime = DateTime.Now;
                }
                
                _timeSheet.Note = timeSheet.Note;
                db.SaveChanges();
                return RedirectToAction("Index", new { page = page });
            }
            return View(timeSheet);
        }
        

        // GET: TimeSheets/Edit/5
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(decimal id, int? page)
        {
            ViewBag.CurrentPage = page;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TimeSheet timeSheet = db.TimeSheets.Find(id);
            if (timeSheet == null)
            {
                return HttpNotFound();
            }
            ViewBag.AssocClientID = new SelectList(db.Clients, "ID", "CompanyName", timeSheet.AssocClientID);
            ViewBag.EmpID = new SelectList(db.Employees, "ID", "CompanyName", timeSheet.EmpID);
            return View(timeSheet);
        }

        // POST: TimeSheets/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,CompanyName,EmpID,AssocClientID,StartTime,StopTime,Note,Approved,ApprovedBy,ApprovedDate,Processed")] TimeSheet timeSheet, int? page)
        {
            if (ModelState.IsValid)
            {
                db.Entry(timeSheet).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", new {page = page });
            }
            ViewBag.AssocClientID = new SelectList(db.Clients, "ID", "CompanyName", timeSheet.AssocClientID);
            ViewBag.EmpID = new SelectList(db.Employees, "ID", "CompanyName", timeSheet.EmpID);
            return View(timeSheet);
        }
        // GET: TimeSheets/Manage/5
        

        // GET: TimeSheets/Delete/5
        [Authorize]
        public ActionResult Delete(decimal id, int? page)
        {
            ViewBag.CurrentPage = page;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TimeSheet timeSheet = db.TimeSheets.Find(id);
            if (timeSheet == null)
            {
                return HttpNotFound();
            }
            return View(timeSheet);
        }

        // POST: TimeSheets/Delete/5
        [Authorize]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(decimal id, int? page)
        {
            TimeSheet timeSheet = db.TimeSheets.Find(id);
            db.TimeSheets.Remove(timeSheet);
            db.SaveChanges();
            return RedirectToAction("Index", new {page = page });
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
