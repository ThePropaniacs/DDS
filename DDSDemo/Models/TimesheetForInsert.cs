using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DDSDemo.Models
{
    public class TimeSheetForInsert
    {
        public int EmployeeId { get; set; }

        public int ClientId { get; set; }

        public DateTimeOffset? StartTime { get; set; }
    }
}