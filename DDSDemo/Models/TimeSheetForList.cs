using DDSDemoDAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DDSDemo.Models
{
    public class TimeSheetForList
    {
        public decimal ID { get; set; }

        public string CompanyName { get; set; }

        public decimal EmpID { get; set; }

        public decimal? AssocClientID { get; set; }

        public DateTime? StartTime { get; set; }

        public DateTime? StopTime { get; set; }

        public string Note { get; set; }

        public bool? Approved { get; set; }

        public int ApprovedBy { get; set; }

        public DateTime? ApprovedDate { get; set; }

        public bool? Processed { get; set; }

        public TimeSpan ElapsedTime { get; set; }

        public bool IsChecked { get; set; }

        public virtual Client Client { get; set; }

        public virtual Employee Employee { get; set; }
    }
}