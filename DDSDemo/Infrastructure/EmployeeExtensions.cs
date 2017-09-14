using DDSDemo.Models;
using DDSDemoDAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DDSDemo.Infrastructure
{
    public static class EmployeeExtensions
    {
        public static void EmployeeAccountVmToEmployee(this Employee employee, EmployeeAccountViewModel employeeVm)
        {
            employee.AvailDuration = employeeVm.AvailDuration;
            employee.AvailExpires = employeeVm.AvailExpires;
            employee.AvailNotes = employeeVm.AvailNotes;
            employee.AvailStart = employeeVm.AvailStart;
            //employee.EmployeeID = employeeVm.EmployeeID;
            employee.FirstName = employeeVm.FirstName;
            employee.Fri = employeeVm.Fri;
            employee.Id = employeeVm.Id;
            employee.LastName = employeeVm.LastName;
            employee.Mon = employeeVm.Mon;
            employee.Sat = employeeVm.Sat;
            employee.Sun = employeeVm.Sun;
            employee.Thu = employeeVm.Thu;
            employee.Tue = employeeVm.Tue;
            employee.Wed = employeeVm.Wed;
        }
    }
}