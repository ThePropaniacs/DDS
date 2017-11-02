using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DDSDemoDAL
{
    public static class DateTimeOperations
    {
        public static DateTime ConvertToLocalTime(DateTime dateTime)
        {
            if(dateTime.Kind == DateTimeKind.Local)
            {
                return dateTime;
            }
            string eastTimeZoneKey = "Eastern Standard Time";
            TimeZoneInfo eastTimeZone = TimeZoneInfo.FindSystemTimeZoneById(eastTimeZoneKey);
            DateTime eastDateTime = DateTime.SpecifyKind(new DateTime(), DateTimeKind.Local);
            eastDateTime = TimeZoneInfo.ConvertTimeFromUtc(dateTime, eastTimeZone);
            eastDateTime = DateTime.SpecifyKind(eastDateTime, DateTimeKind.Local);
            return eastDateTime;
        }

        public static DateTime ConvertToUTCTime(DateTime dateTime)
        {
            //string eastTimeZoneKey = "Eastern Standard Time";
            //TimeZoneInfo eastTimeZone = TimeZoneInfo.FindSystemTimeZoneById(eastTimeZoneKey);
            //DateTime utcDateTime = DateTime.SpecifyKind(new DateTime(), DateTimeKind.Utc);
            //utcDateTime = TimeZoneInfo.ConvertTimeToUtc(dateTime, eastTimeZone);
            var utcDateTime = dateTime.ToUniversalTime();
            return utcDateTime;
        }
    }
}