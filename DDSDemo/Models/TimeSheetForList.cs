using DDSDemoDAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DDSDemo.Models
{
    public class TimeSheetForList
    {
        public decimal Id { get; set; }

        public string CompanyName { get; set; }

        public decimal EmployeeId { get; set; }

        public decimal? ClientId { get; set; }

        public DateTimeOffset? StartTime { get; set; }

        public DateTimeOffset? StopTime { get; set; }

        public string Note { get; set; }

        public bool? Approved { get; set; }

        public string ApprovedBy { get; set; }

        public DateTimeOffset? ApprovedDate { get; set; }

        public bool? Processed { get; set; }

        public TimeSpan ElapsedTime { get; set; }

        public bool IsChecked { get; set; }

        public virtual Client Client { get; set; }

        public virtual Employee Employee { get; set; }
    }
}