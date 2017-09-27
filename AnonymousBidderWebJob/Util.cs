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

        /// <summary>
        /// Get current local TimeZone Name of running system
        /// </summary>
        public static string TimeZoneName
        {
            get
            {
                return ConfigurationManager.AppSettings["TimeZoneName"];
            }
        }

        /// <summary>
        /// A conversion from DateTime.Now to current local TimeZone DateTime
        /// </summary>
        public static DateTime LocalDateTimeNow
        {
            get
            {
                return ConvertUCTDateTimeToLocal(DateTime.Now);
                //return DateTime.Now;
            }
        }

        /// <summary>
        /// Convert current local TimeZone DateTime to UTC DateTime
        /// </summary>
        /// <param name="localTime"></param>
        /// <returns></returns>
        public static DateTime ConvertLocalDateTimeToUTC(DateTime localTime)
        {
            TimeZoneInfo localTimeZone = TimeZoneInfo.FindSystemTimeZoneById(TimeZoneName);
            DateTime uctTime = TimeZoneInfo.ConvertTimeToUtc(localTime, localTimeZone);

            return uctTime;
        }

        /// <summary>
        /// Convert Current UTC DateTime to current local TimeZone
        /// </summary>
        /// <param name="utcTime"></param>
        /// <returns></returns>
        public static DateTime ConvertUCTDateTimeToLocal(DateTime utcTime)
        {
            TimeZoneInfo localTimeZone = TimeZoneInfo.FindSystemTimeZoneById(TimeZoneName);
            DateTime uctTime = TimeZoneInfo.ConvertTime(utcTime, localTimeZone);

            return uctTime;
        }
    }
}
