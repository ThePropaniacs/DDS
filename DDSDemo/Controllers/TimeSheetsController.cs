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
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;
using System.Text;

namespace DDSDemo.Controllers
{
    [Authorize]
    public class TimeSheetsController : Controller
    {
        private DDSContext db = new DDSContext();

        private ApplicationUserManager _userManager = null;

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

        private string GetUserGreeting()
        {
            var greetingBuilder = new StringBuilder();
            greetingBuilder.Append("Hello");
            var userInfo = UserManager.FindById(User.Identity.GetUserId());
            if(userInfo != null)
            {
                greetingBuilder.Append($"{(!String.IsNullOrEmpty(userInfo.FirstName) ? $" {userInfo.FirstName}" : String.Empty)} {(!String.IsNullOrEmpty(userInfo.LastName) ? $" {userInfo.LastName}" : String.Empty)}"); 
            }
            greetingBuilder.Append("!");
            return greetingBuilder.ToString();
        }

        //http://localhost:8888/Timesheets/EmployeeIndex
        // GET: TimeSheets
        //Admin index
        [Authorize]
        public ActionResult Index(string sortBy, int? page, string clientFilter = "", string employeeFilter = "", string optionsStatus = "", string optionsProcessed = "", DateTime? fromDateFilter = null, DateTime? toDateFilter = null)
        {
            if (User.IsInRole("Admin"))
            {
                var tblTimeSheetMasters = db.TimeSheets.Include(t => t.Client).Include(t => t.Employee).AsQueryable();
                var data = tblTimeSheetMasters;

                ViewBag.SortClientParameter = sortBy == "Client" ? "Client Desc" : "Client";
                ViewBag.SortEmployeeParameter = sortBy == "Employee" ? "Employee Desc" : "Employee";
                ViewBag.SortStartTimeParameter = string.IsNullOrEmpty(sortBy) ? "Start Time Desc" : "";
                ViewBag.SortStopTimeParameter = sortBy == "Stop Time" ? "Stop Time Desc" : "Stop Time";
                ViewBag.SortElapsedTimeParameter = sortBy == "Elapsed Time" ? "Elapsed Time Desc" : "Elapsed Time";
                ViewBag.UserGreeting = GetUserGreeting();


                if (!String.IsNullOrEmpty(clientFilter.Trim()))
                {
                    data = data.Where(x => x.Client.CompanyName.StartsWith(clientFilter) || clientFilter == null);
                }
                if (!String.IsNullOrEmpty(employeeFilter.Trim()))
                {
                    data = data.Where(x => x.Employee.FirstName.Contains(employeeFilter) || x.Employee.LastName.Contains(employeeFilter) || employeeFilter.Contains(x.Employee.FirstName) || employeeFilter.Contains(x.Employee.LastName) || employeeFilter == null);
                }
                if (!String.IsNullOrEmpty(optionsStatus.Trim()))
                {
                    if(optionsStatus == "pending")
                    {
                        data = data.Where(x => x.Approved == null);
                    }
                    else if (optionsStatus == "approved")
                    {
                        data = data.Where(x => x.Approved == true);
                    }
                    else if (optionsStatus == "denied")
                    {
                        data = data.Where(x => x.Approved == false);
                    }
                    else
                    {
                        optionsStatus = String.Empty;
                    }
                }
                if (!String.IsNullOrEmpty(optionsProcessed.Trim()))
                {
                    if (optionsProcessed == "pending")
                    {
                        data = data.Where(x => x.Processed == false);
                    }
                    else if (optionsProcessed == "processed")
                    {
                        data = data.Where(x => x.Processed == true);
                    }
                    else
                    {
                        optionsProcessed = String.Empty;
                    }
                }
                if (fromDateFilter != null)
                {
                    var exactDate = fromDateFilter.Value.Date;
                    data = data.Where(x => x.StartTime.Value >= exactDate);
                }
                if(toDateFilter != null)
                {
                    var exactDate = toDateFilter.Value.Date.AddHours(23).AddMinutes(59).AddSeconds(59);
                    data = data.Where(x => x.StopTime.Value <= exactDate);
                    
                }
                switch (sortBy)
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
                        data = data.OrderBy(x => x.StartTime);
                        break;
                    case "Client":
                        data = data.OrderBy(x => x.Client.CompanyName);
                        break;
                    case "Stop Time Desc":
                        data = data.OrderByDescending(x => x.StopTime);
                        break;
                    case "Stop Time":
                        data = data.OrderBy(x => x.StopTime);
                        break;
                    default:
                        data = data.OrderByDescending(x => x.StartTime);
                        break;
                }

                return View(data.ToPagedList(page ?? 1, 10));
            }
            else if (User.IsInRole("Employee"))
            {
                return RedirectToAction("EmployeeIndex", "TimeSheets", new { page = page, sortBy = sortBy });
            }
            else if (User.IsInRole("Client"))
            {
                return RedirectToAction("ClientIndex", "TimeSheets", new { page = page, sortBy = sortBy });
            }
            else
            {
                return RedirectToAction("LogIn", "Account");
            }
        }

        [ClaimsAccess(ClaimType = "EmployeeID")]
        public ActionResult EmployeeIndex(int? page, string sortBy)
        {
            ViewBag.SortStartTimeParameter = string.IsNullOrEmpty(sortBy) ? "Start Time Desc" : "";
            ViewBag.SortStopTimeParameter = sortBy == "Stop Time" ? "Stop Time Desc" : "Stop Time";
            ViewBag.UserGreeting = GetUserGreeting();

            var employeeID = Int32.Parse((this.HttpContext.User.Identity as ClaimsIdentity).Claims.FirstOrDefault(c => c.Type == "EmployeeID").Value);
            var timesheets = db.TimeSheets.Include(t => t.Client).Include(t => t.Employee).Where(t => t.Employee.Id == employeeID);

            var data = timesheets.AsQueryable();

            switch (sortBy)
            {
                case "Start Time Desc":
                    data = data.OrderBy(x => x.StartTime);
                    break;
                case "Stop Time Desc":
                    data = data.OrderByDescending(x => x.StopTime);
                    break;
                case "Stop Time":
                    data = data.OrderBy(x => x.StopTime);
                    break;
                default:
                    data = data.OrderByDescending(x => x.StartTime);
                    break;
            }
            return View(data.ToPagedList(page ?? 1, 10));
        }

        [ClaimsAccess(ClaimType = "ClientID")]
        public ActionResult ClientIndex(int? page, string sortBy)
        {
            ViewBag.SortStartTimeParameter = string.IsNullOrEmpty(sortBy) ? "Start Time Desc" : "";
            ViewBag.SortStopTimeParameter = sortBy == "Stop Time" ? "Stop Time Desc" : "Stop Time";
            ViewBag.UserGreeting = GetUserGreeting();

            var clientID = Int32.Parse((this.HttpContext.User.Identity as ClaimsIdentity).Claims.FirstOrDefault(c => c.Type == "ClientID").Value);
            var timesheets = db.TimeSheets.Include(t => t.Client).Include(t => t.Employee).Where(c => c.Client.Id == clientID);

            var timesheetsForList = new List<TimeSheetForList>();
            var data = timesheetsForList.AsQueryable();

            foreach (var ts in timesheets)
            {
                var timesheetForList = new TimeSheetForList();

                timesheetForList.TimesheetToTimesheetForList(ts);


                timesheetsForList.Add(timesheetForList);
            }
            switch (sortBy)
            {
                case "Start Time Desc":
                    data = data.OrderBy(x => x.StartTime);
                    break;
                case "Stop Time Desc":
                    data = data.OrderByDescending(x => x.StopTime);
                    break;
                case "Stop Time":
                    data = data.OrderBy(x => x.StopTime);
                    break;
                default:
                    data = data.OrderByDescending(x => x.StartTime);
                    break;
            }
            return View(data.ToPagedList(page ?? 1, 10));
            
        }

        [ClaimsAccess(ClaimType = "ClientID")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ClientIndex(int? page, string sortBy , List<TimeSheetForList> timesheetsFromList, String Approve, String Deny, [Bind(Include = "Id,CompanyName,EmployeeId,ClientId,StartTime,StopTime,Note,Approved,ApprovedBy,ApprovedDate,Processed")] TimeSheet timeSheet)
        {
            bool ActionToTake = true;

            if (String.IsNullOrEmpty(Approve))
            {
                ActionToTake = false;
            }

            foreach (var timesheet in timesheetsFromList)
            {
                var ts = db.TimeSheets.Find(timesheet.Id);

                if (timesheet.IsChecked)
                {
                    ts.Approved = ActionToTake;
                    ts.ApprovedDate = DateTime.Now;
                    ts.ApprovedBy = User.Identity.Name;
                }

                timesheet.TimesheetToTimesheetForList(ts);
            }
            db.SaveChanges();

            return RedirectToAction("ClientIndex", new { page = page, sortBy = sortBy });
        }
        [Authorize(Roles = "Admin, Client")]
        public ActionResult ClientEdit(decimal id, int? page, string sortBy)
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
        public ActionResult ClientEdit([Bind(Include = "Id,CompanyName,EmployeeId,ClientId,StartTime,StopTime,Note,Approved,ApprovedBy,ApprovedDate,Processed,ClientFeedback")] TimeSheet timeSheet, int? page, string sortBy)
        {
            if (ModelState.IsValid)
            {
                TimeSheet _timeSheet = db.TimeSheets.Find(timeSheet.Id);

                if (!_timeSheet.StopTime.HasValue)
                {
                    _timeSheet.StopTime = DateTime.Now;
                }
                _timeSheet.Approved = timeSheet.Approved;
                _timeSheet.Note = timeSheet.Note;
                _timeSheet.ApprovedDate = DateTime.Now;
                _timeSheet.ApprovedBy = User.Identity.Name;
                _timeSheet.ClientFeedback = timeSheet.ClientFeedback;
                db.SaveChanges();
                return RedirectToAction("ClientIndex", new { page = page, sortBy = sortBy });
            }
            return View(timeSheet);
        }
        

        //[Authorize(Roles = "Admin, Employee")]
        //public ActionResult EmployeeDetails(decimal id, int? page, string sortBy)
        //{
        //    ViewBag.CurrentPage = page;
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    TimeSheet timeSheet = db.TimeSheets.Find(id);
        //    if (timeSheet == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(timeSheet);
        //}
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

                var today = DateTime.Now.Date;

                var placements = db.Placements.ToList().Where(p => p.PlacementDate.Value.Date == DateTime.Now.Date 
                                        && p.EmployeeId == employeeID 
                                        && p.Cancelled == false);
                var clients = new List<Client>();
                if (placements.Count() != 0)
                {
                    foreach(var placement in placements)
                    {
                        clients.Add(db.Clients.FirstOrDefault(c => c.Id == placement.ClientId));
                    }
                    ViewBag.HasPlacements = true;
                }
                else
                {
                    ViewBag.HasPlacements = false;
                }

                ViewBag.ClientId = new SelectList(clients, "Id", "CompanyName");
                ViewBag.EmployeeId = new SelectList(db.Employees.Where(i => i.Id == employeeID), "Id", "FullName");
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
        public ActionResult EmployeeCreate([Bind(Include = "Id,CompanyName,EmployeeId,ClientId,StartTime,StopTime,Note,Approved,ApprovedDate,Processed")] TimeSheet timeSheet)
        {
            if (ModelState.IsValid)
            {
                timeSheet.StartTime = DateTime.Now;
                timeSheet = db.TimeSheets.Add(timeSheet);
                db.SaveChanges();
                if (User.IsInRole("Employee"))
                {
                    return RedirectToAction("EmployeeManage", new { id = timeSheet.Id });
                }
                else
                {
                    return RedirectToAction("Manage", new { id = timeSheet.Id });
                }
            }

            ViewBag.AssocClientID = new SelectList(db.Clients, "Id", "CompanyName", timeSheet.ClientId);
            ViewBag.EmpID = new SelectList(db.Employees, "Id", "FullName", timeSheet.EmployeeId);
            return View(timeSheet);
        }
        // GET: TimeSheets/Manage/5
        [Authorize(Roles = "Admin, Employee")]
        public ActionResult EmployeeManage(decimal id, int? page, string sortBy)
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
        public ActionResult EmployeeManage([Bind(Include = "Id,CompanyName,EmployeeId,ClientId,StartTime,StopTime,Note,Approved,ApprovedBy,ApprovedDate,Processed")] TimeSheet timeSheet, int? page, string sortBy)
        {
            if (ModelState.IsValid)
            {
                TimeSheet _timeSheet = db.TimeSheets.Find(timeSheet.Id);

                //if (!_timeSheet.StopTime.HasValue)
                //{
                //    _timeSheet.StopTime = DateTime.Now;
                //}

                _timeSheet.StopTime = timeSheet.StopTime;

                _timeSheet.Note = timeSheet.Note;
                db.SaveChanges();
                return RedirectToAction("EmployeeIndex", new { page = page, sortBy = sortBy });
            }
            return View(timeSheet);
        }

        //// GET: TimeSheets/Details/5
        //[Authorize(Roles = "Admin")]
        //public ActionResult Details(decimal id,string searchBy, string search, int? page, string sortBy)
        //{
        //    ViewBag.CurrentPage = page;
        //    ViewBag.searchBy = searchBy;
        //    ViewBag.search = search;
        //    ViewBag.sortBy = sortBy;
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    TimeSheet timeSheet = db.TimeSheets.Find(id);
        //    if (timeSheet == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(timeSheet);
        //}
        

        // GET: TimeSheets/Create
        [Authorize(Roles = "Admin, Employee")]
        public ActionResult Create()
        {
            if (User.IsInRole("Admin"))
            {
                ViewBag.ClientId = new SelectList(db.Clients, "Id", "CompanyName");
                ViewBag.EmployeeId = new SelectList(db.Employees, "Id", "FullName");
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
        public ActionResult Create([Bind(Include = "EmployeeId,ClientId")] TimeSheetForInsert timeSheetForInsert)
        {
            if (ModelState.IsValid)
            {
                var currentUniversalTime = DateTime.Now.ToUniversalTime();
                currentUniversalTime = DateTime.SpecifyKind(currentUniversalTime, DateTimeKind.Local);
                currentUniversalTime = currentUniversalTime.ToUniversalTime();
                var timeSheet = db.TimeSheets.Add(
                    new TimeSheet
                    {
                        EmployeeId = timeSheetForInsert.EmployeeId, ClientId = timeSheetForInsert.ClientId, StartTime = currentUniversalTime 
                    });

                db.SaveChanges();
                if (User.IsInRole("Employee"))
                {
                    return RedirectToAction("EmployeeManage", new { id = timeSheet.Id });
                }
                else
                {
                    return RedirectToAction("Manage", new { id = timeSheet.Id });
                }
            }

            ViewBag.ClientId = new SelectList(db.Clients, "Id", "CompanyName", timeSheetForInsert.ClientId);
            ViewBag.EmployeeId = new SelectList(db.Employees, "Id", "FullName", timeSheetForInsert.EmployeeId);
            return View(timeSheetForInsert);
        }

        


        // GET: TimeSheets/Manage/5
        [Authorize(Roles = "Admin")]
        public ActionResult Manage(decimal id, string searchBy, string search, int? page, string sortBy)
        {
            ViewBag.CurrentPage = page;
            ViewBag.searchBy = searchBy;
            ViewBag.search = search;
            ViewBag.sortBy = sortBy;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TimeSheet timeSheet = db.TimeSheets.Find(id);
            if(timeSheet == null)
            {
                return HttpNotFound();
            }
            ViewBag.ClientId = new SelectList(db.Clients, "Id", "CompanyName", timeSheet.ClientId);
            ViewBag.EmployeeId = new SelectList(db.Employees, "Id", "FullName", timeSheet.EmployeeId);
            return View(timeSheet);
        }

        // POST: TimeSheets/Manage
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Manage([Bind(Include = "Id,CompanyName,EmployeeId,ClientId,StartTime,StopTime,Note,Approved,ApprovedBy,ApprovedDate,Processed,ClientFeedback")] TimeSheet timeSheet, string searchBy, string search, int? page, string sortBy)
        {
            if (ModelState.IsValid)
            {
                TimeSheet _timeSheet = db.TimeSheets.Find(timeSheet.Id);

                //if (!_timeSheet.StopTime.HasValue)
                //{
                //    _timeSheet.StopTime = DateTime.Now;
                //}

                if (timeSheet.StopTime.HasValue)
                {
                    var convertedStopTime = timeSheet.StopTime?.ToUniversalTime();
                    convertedStopTime = DateTime.SpecifyKind(convertedStopTime.Value, DateTimeKind.Local);
                    convertedStopTime = convertedStopTime?.ToUniversalTime();
                    _timeSheet.StopTime = convertedStopTime;
                }
                
                _timeSheet.ClientFeedback = timeSheet.ClientFeedback;
                _timeSheet.Note = timeSheet.Note;
                db.SaveChanges();
                return RedirectToAction("Index", new { page = page, search = search, searchBy = searchBy, sortBy = sortBy });
            }
            return View(timeSheet);
        }
        

        // GET: TimeSheets/Edit/5
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(decimal id, string searchBy, string search, int? page, string sortBy)
        {
            ViewBag.CurrentPage = page;
            ViewBag.searchBy = searchBy;
            ViewBag.search = search;
            ViewBag.sortBy = sortBy;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TimeSheet timeSheet = db.TimeSheets.Find(id);
            if (timeSheet == null)
            {
                return HttpNotFound();
            }
            ViewBag.ClientId = new SelectList(db.Clients, "Id", "CompanyName", timeSheet.ClientId);
            ViewBag.EmployeeID = new SelectList(db.Employees, "Id", "FullName", timeSheet.EmployeeId);
            return View(timeSheet);
        }

        // POST: TimeSheets/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,CompanyName,EmployeeId,ClientId,StartTime,StopTime,Note,Approved,ApprovedBy,ApprovedDate,Processed")] TimeSheet timeSheet, string searchBy, string search, int? page, string sortBy)
        {
            if (ModelState.IsValid)
            {
                var _timeSheet = timeSheet;
                var old = db.TimeSheets.Find(_timeSheet.Id);

                if (old.Approved != timeSheet.Approved)
                {
                    old.ApprovedDate = DateTime.Now;
                    old.ApprovedBy = "Admin";
                }
                else
                {
                    old.ApprovedDate = timeSheet.ApprovedDate;
                    old.ApprovedBy = timeSheet.ApprovedBy;
                }

                old.Id = timeSheet.Id;
                old.CompanyName = timeSheet.CompanyName;
                old.EmployeeId = timeSheet.EmployeeId;
                old.ClientId = timeSheet.ClientId;

                if (timeSheet.StartTime.HasValue)
                {
                    var convertedStartTime = timeSheet.StartTime?.ToUniversalTime();
                    convertedStartTime = DateTime.SpecifyKind(convertedStartTime.Value, DateTimeKind.Local);
                    convertedStartTime = convertedStartTime?.ToUniversalTime();
                    old.StartTime = convertedStartTime;
                }

                if (timeSheet.StopTime.HasValue)
                {
                    var convertedStopTime = timeSheet.StopTime?.ToUniversalTime();
                    convertedStopTime = DateTime.SpecifyKind(convertedStopTime.Value, DateTimeKind.Local);
                    convertedStopTime = convertedStopTime?.ToUniversalTime();
                    old.StopTime = convertedStopTime;
                }
                
                old.Note = timeSheet.Note;
                old.Approved = timeSheet.Approved;
                old.Processed = timeSheet.Processed;

                
                db.SaveChanges();
                return RedirectToAction("Index", new {page = page, search = search, searchBy = searchBy, sortBy = sortBy });
            }
            ViewBag.ClientId = new SelectList(db.Clients, "ID", "CompanyName", timeSheet.ClientId);
            ViewBag.EmployeeId = new SelectList(db.Employees, "ID", "FullName", timeSheet.EmployeeId);
            return View(timeSheet);
        }
        // GET: TimeSheets/Manage/5
        

        // GET: TimeSheets/Delete/5
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(decimal id, string searchBy, string search, int? page, string sortBy)
        {
            ViewBag.CurrentPage = page;
            ViewBag.searchBy = searchBy;
            ViewBag.search = search;
            ViewBag.sortBy = sortBy;
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
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(decimal id, string searchBy, string search, int? page, string sortBy)
        {
            TimeSheet timeSheet = db.TimeSheets.Find(id);
            db.TimeSheets.Remove(timeSheet);
            db.SaveChanges();
            return RedirectToAction("Index", new {page = page, search = search, searchBy = searchBy, sortBy = sortBy });
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
