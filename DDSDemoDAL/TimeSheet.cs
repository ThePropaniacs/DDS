namespace DDSDemoDAL
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TimeSheet
    {
        private DateTime? startTime = new DateTime?();
        private DateTime? stopTime = new DateTime?();

        public int Id { get; set; }

        public string CompanyName { get; set; }

        public int EmployeeId { get; set; }

        public int ClientId { get; set; }
        
        public DateTime? StartTime
        {
            get
            {
                if (this.startTime.HasValue)
                {
                    if (this.startTime.Value.Kind == DateTimeKind.Local)
                    {
                        return this.startTime;
                    }
                    var returnTime = DateTimeOperations.ConvertToLocalTime(this.startTime.Value);
                    return returnTime;
                }
                return null;
            }

            set
            {
                if (value.HasValue)
                {
                    if (value.Value.Kind == DateTimeKind.Local)
                    {
                        this.startTime = value.HasValue ? DateTimeOperations.ConvertToUTCTime(value.Value) : (DateTime?)null;
                    }
                    else
                    {
                        this.startTime = DateTime.SpecifyKind(value.Value, DateTimeKind.Utc);
                    }
                }
            }
        }


        public DateTime? StopTime
        {
            get
            {
                if (this.stopTime.HasValue)
                {
                    if (this.stopTime.Value.Kind == DateTimeKind.Local)
                    {
                        return this.stopTime;
                    }
                    var returnTime = DateTimeOperations.ConvertToLocalTime(this.stopTime.Value);
                    return returnTime;
                }
                return null;
            }

            set
            {
                if (value.HasValue)
                {
                    if (value.Value.Kind == DateTimeKind.Local)
                    {
                        this.stopTime = value.HasValue ? DateTimeOperations.ConvertToUTCTime(value.Value) : (DateTime?)null;
                    }
                    else
                    {
                        this.stopTime = DateTime.SpecifyKind(value.Value, DateTimeKind.Utc);
                    }
                }
            }
        }

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
