using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DDSDemoDAL;

namespace DDSDemo.Controllers
{
    [Authorize]
    public class TimeSheetsController : Controller
    {
        private DDSContext db = new DDSContext();

        //http://localhost:8888/Timesheets/EmployeeIndex

        // GET: TimeSheets
        //Admin index
        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            var tblTimeSheetMasters = db.TimeSheets.Include(t => t.Client).Include(t => t.Employee);
            return View(tblTimeSheetMasters.ToList());
        }

        [Authorize(Roles = "Employee")]
        public ActionResult EmployeeIndex()
        {
            var timesheets = db.TimeSheets.Include(t => t.Client).Include(t => t.Employee);
            return View(timesheets.ToList());
        }

        // GET: TimeSheets/Details/5
        [Authorize]
        public ActionResult Details(decimal id)
        {

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
            ViewBag.AssocClientID = new SelectList(db.Clients, "ID", "CompanyName");
            ViewBag.EmpID = new SelectList(db.Employees, "ID", "FullName");
            return View();
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
                return RedirectToAction("Manage", new { id = timeSheet.ID });
            }

            ViewBag.AssocClientID = new SelectList(db.Clients, "ID", "CompanyName", timeSheet.AssocClientID);
            ViewBag.EmpID = new SelectList(db.Employees, "ID", "FullName", timeSheet.EmpID);
            return View(timeSheet);
        }

        // GET: TimeSheets/Manage/5
        [Authorize(Roles = "Employee, Client")]
        public ActionResult Manage(decimal id)
        {
            if(id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TimeSheet timeSheet = db.TimeSheets.Find(id);
            if(timeSheet == null)
            {
                return HttpNotFound();
            }

            return View(timeSheet);
        }

        // POST: TimeSheets/Manage
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Employee, Client")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Manage([Bind(Include = "ID,CompanyName,EmpID,AssocClientID,StartTime,StopTime,Note,Approved,ApprovedBy,ApprovedDate,Processed")] TimeSheet timeSheet)
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
                return RedirectToAction("Index");
            }
            return View(timeSheet);
        }

        // GET: TimeSheets/Edit/5
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(decimal id)
        {
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
        public ActionResult Edit([Bind(Include = "ID,CompanyName,EmpID,AssocClientID,StartTime,StopTime,Note,Approved,ApprovedBy,ApprovedDate,Processed")] TimeSheet timeSheet)
        {
            if (ModelState.IsValid)
            {
                db.Entry(timeSheet).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AssocClientID = new SelectList(db.Clients, "ID", "CompanyName", timeSheet.AssocClientID);
            ViewBag.EmpID = new SelectList(db.Employees, "ID", "CompanyName", timeSheet.EmpID);
            return View(timeSheet);
        }

        // GET: TimeSheets/Delete/5
        [Authorize]
        public ActionResult Delete(decimal id)
        {
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
        public ActionResult DeleteConfirmed(decimal id)
        {
            TimeSheet timeSheet = db.TimeSheets.Find(id);
            db.TimeSheets.Remove(timeSheet);
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
