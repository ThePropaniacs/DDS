namespace DDSDemoDAL
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TimeSheet
    {
        public int Id { get; set; }

        public string CompanyName { get; set; }

        public int EmployeeId { get; set; }

        public int ClientId { get; set; }
        
        public DateTime? StartTime { get; set; }
        
        public DateTime? StopTime { get; set; }

        public string Note { get; set; }

        public string ClientFeedback { get; set; }

        public bool? Approved { get; set; }

        public string ApprovedBy { get; set; }

        public DateTime? ApprovedDate { get; set; }

        public bool Processed { get; set; }

        public virtual Client Client { get; set; }

        public virtual Employee Employee { get; set; }
    }
}
