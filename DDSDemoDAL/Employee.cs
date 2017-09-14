namespace DDSDemoDAL
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Employee
    {
        public Employee()
        {
            TimeSheets = new HashSet<TimeSheet>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        //public int EmpID { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public bool? Sun { get; set; }

        public bool? Mon { get; set; }

        public bool? Tue { get; set; }

        public bool? Wed { get; set; }

        public bool? Thu { get; set; }

        public bool? Fri { get; set; }

        public bool? Sat { get; set; }

        public string AvailNotes { get; set; }
        
        public DateTime? AvailStart { get; set; }

        public DateTime? AvailExpires { get; set; }

        public int? AvailDuration { get; set; }

        public virtual ICollection<TimeSheet> TimeSheets { get; set; }
    }
}
