namespace DDSDemoDAL
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TimeSheet
    {
        private DateTimeOffset? startTime = new DateTimeOffset?();
        private DateTimeOffset? stopTime = new DateTimeOffset?();

        public int Id { get; set; }

        public string CompanyName { get; set; }

        public int EmployeeId { get; set; }

        public int ClientId { get; set; }
        
        public DateTimeOffset? StartTime
        {
            get
            {
                return startTime;
                //if (this.startTime.HasValue)
                //{
                //    if (this.startTime.Value.Kind == DateTimeKind.Local)
                //    {
                //        return this.startTime;
                //    }
                //    var returnTime = DateTimeOperations.ConvertToLocalTime(this.startTime.Value);
                //    return returnTime;
                //}
                //return null;
            }

            set
            {
                startTime = value;
                //if (value.HasValue)
                //{
                //    if (value.Value.Kind == DateTimeKind.Local)
                //    {
                //        this.startTime = value.HasValue ? DateTimeOperations.ConvertToUTCTime(value.Value) : (DateTime?)null;
                //    }
                //    else
                //    {
                //        this.startTime = DateTime.SpecifyKind(value.Value, DateTimeKind.Utc);
                //    }
                //}
            }
        }


        public DateTimeOffset? StopTime
        {
            get
            {
                return stopTime;
                //if (this.stopTime.HasValue)
                //{
                //    if (this.stopTime.Value.Kind == DateTimeKind.Local)
                //    {
                //        return this.stopTime;
                //    }
                //    var returnTime = DateTimeOperations.ConvertToLocalTime(this.stopTime.Value);
                //    return returnTime;
                //}
                //return null;
            }

            set
            {
                stopTime = value;
                //if (value.HasValue)
                //{
                //    if (value.Value.Kind == DateTimeKind.Local)
                //    {
                //        this.stopTime = value.HasValue ? DateTimeOperations.ConvertToUTCTime(value.Value) : (DateTime?)null;
                //    }
                //    else
                //    {
                //        this.stopTime = DateTime.SpecifyKind(value.Value, DateTimeKind.Utc);
                //    }
                //}
            }
        }

        public string Note { get; set; }

        public string ClientFeedback { get; set; }

        public bool? Approved { get; set; }

        public string ApprovedBy { get; set; }

        public DateTimeOffset? ApprovedDate { get; set; }

        public bool Processed { get; set; }

        public virtual Client Client { get; set; }

        public virtual Employee Employee { get; set; }
    }
}
