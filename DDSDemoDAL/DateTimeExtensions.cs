using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DDSDemoDAL
{
    public static class DateTimeOffsetOperations
    {
        public static DateTimeOffset ConvertToLocalTime(DateTimeOffset dateTime)
        {
            //if(dateTime.Kind == DateTimeKind.Local)
            //{
            //    return dateTime;
            //}
            //string eastTimeZoneKey = "Eastern Standard Time";
            //TimeZoneInfo eastTimeZone = TimeZoneInfo.FindSystemTimeZoneById(eastTimeZoneKey);
            //DateTime eastDateTime = DateTime.SpecifyKind(new DateTime(), DateTimeKind.Local);
            //eastDateTime = TimeZoneInfo.ConvertTimeFromUtc(dateTime, eastTimeZone);
            //eastDateTime = DateTime.SpecifyKind(eastDateTime, DateTimeKind.Local);
            //return eastDateTime;
            return new DateTimeOffset();
        }

        public static DateTimeOffset ConvertToUTCTime(DateTimeOffset dateTime)
        {
            //string eastTimeZoneKey = "Eastern Standard Time";
            //TimeZoneInfo eastTimeZone = TimeZoneInfo.FindSystemTimeZoneById(eastTimeZoneKey);
            //DateTime utcDateTime = DateTime.SpecifyKind(new DateTime(), DateTimeKind.Utc);
            //utcDateTime = TimeZoneInfo.ConvertTimeToUtc(dateTime, eastTimeZone);
            //var utcDateTime = dateTime.ToUniversalTime();
            //return utcDateTime;
            return new DateTimeOffset();
        }
    }
}