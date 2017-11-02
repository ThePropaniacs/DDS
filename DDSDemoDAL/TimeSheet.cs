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
                if (this.startTime.HasValue)
                {
                    var regionalStartTime = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(this.startTime.Value, "Eastern Standard Time");
                    return regionalStartTime;
                }

                return (DateTimeOffset?)null;
            }

            set
            {
                startTime = value?.ToUniversalTime();
            }
        }


        public DateTimeOffset? StopTime
        {
            get
            {
                if (this.stopTime.HasValue)
                {
                    var regionalStopTime = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(this.stopTime.Value, "Eastern Standard Time");
                    return regionalStopTime;
                }

                return (DateTimeOffset?)null;
            }

            set
            {
                stopTime = value?.ToUniversalTime();
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
