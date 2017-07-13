using DDSDemo.Models;
using DDSDemoDAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DDSDemo.Infrastructure
{
    public static class TimesheetExtensions
    {
        public static void TimesheetToTimesheetForList(this TimeSheetForList tsForList, TimeSheet ts)
        {
            tsForList.Approved = ts.Approved;
            tsForList.Client = ts.Client;
            tsForList.ID = ts.ID;
            tsForList.CompanyName = ts.CompanyName;
            tsForList.ElapsedTime = ts.ElapsedTime;
            tsForList.Employee = ts.Employee;
            tsForList.Note = ts.Note;
            tsForList.StartTime = ts.StartTime;
            tsForList.StopTime = ts.StopTime;
        }
    }
}