using System;
using System.Configuration;

namespace AnonymousBidderWebJob
{
    public static class Util
    {
        public enum ReminderType
        {
            AuctionEnd = 1
        }

        public static string JobKeyFormat = "{0}###{1}"; //ID###EmailAddress
        
        public static string TimeZoneName
        {
            get
            {
                return ConfigurationManager.AppSettings["TimeZoneName"];
            }
        }
        
        public static DateTime LocalDateTimeNow
        {
            get
            {
                return ConvertUCTDateTimeToLocal(DateTime.Now);
                //return DateTime.Now;
            }
        }
        public static DateTime ConvertLocalDateTimeToUTC(DateTime localTime)
        {
            TimeZoneInfo localTimeZone = TimeZoneInfo.FindSystemTimeZoneById(TimeZoneName);
            DateTime uctTime = TimeZoneInfo.ConvertTimeToUtc(localTime, localTimeZone);

            return uctTime;
        }
        
        public static DateTime ConvertUCTDateTimeToLocal(DateTime utcTime)
        {
            TimeZoneInfo localTimeZone = TimeZoneInfo.FindSystemTimeZoneById(TimeZoneName);
            DateTime uctTime = TimeZoneInfo.ConvertTime(utcTime, localTimeZone);

            return uctTime;
        }
    }
}
