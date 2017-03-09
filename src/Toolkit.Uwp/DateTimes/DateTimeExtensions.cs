using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Globalization.DateTimeFormatting;

namespace Toolkit.Uwp.DateTimes
{
    public static class DateTimeExtensions
    {
        private const string IsoDateTimeFormatter = "yyyy-MM-ddTHH:mm:ss.FFFFFFFK";
        private const int QuarterHoursInDay = 96;
        private static DateTimeFormatter _dayOfWeekFormatter = null;

        public static string ToDayOfWeekString(this DateTime dateTime)
        {
            if (_dayOfWeekFormatter == null)
            {
                _dayOfWeekFormatter = new DateTimeFormatter("dayofweek.full");
            }

            return _dayOfWeekFormatter.Format(dateTime);
        }

        public static string ToShortTimeString(this DateTime dateTime)
        {
            return DateTimeFormatter.ShortTime.Format(dateTime);
        }

        public static string ToShortDateString(this DateTime dateTime)
        {
            return DateTimeFormatter.ShortDate.Format(dateTime);
        }

        public static bool IsToday(this DateTime dateTime)
        {
            return dateTime.ToLocalTime().Date.Equals(DateTime.Today);
        }

        public static bool IsTomorrow(this DateTime dateTime)
        {
            return dateTime.ToLocalTime().Date.Equals(DateTime.Today.AddDays(1));
        }

        public static DateTime Tomorrow(this DateTime dateTime)
        {
            return dateTime.AddDays(1);
        }

        public static DateTime SetDate(this DateTime dateTime, DateTime targetDate)
        {
            return new DateTime(targetDate.Year, targetDate.Month, targetDate.Day, dateTime.Hour, dateTime.Minute, dateTime.Second, dateTime.Millisecond, dateTime.Kind);
        }

        public static DateTime SetTime(this DateTime dateTime, DateTime targetTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, targetTime.Hour, targetTime.Minute, targetTime.Second, targetTime.Millisecond, dateTime.Kind);
        }

        public static DateTime RoundDownToQuarterHour(this DateTime dateTime)
        {
            var newMinute = dateTime.Minute - (dateTime.Minute % 15);

            return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, 0, 0, 0, dateTime.Kind).AddMinutes(newMinute);
        }

        public static DateTime RoundUpToQuarterHour(this DateTime dateTime)
        {
            var newMinute = dateTime.Minute - (dateTime.Minute % 15);
            if (newMinute != 0)
            {
                newMinute += 15;
            }

            return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, 0, 0, 0, dateTime.Kind).AddMinutes(newMinute);
        }

        public static IEnumerable<DateTime> GetDaysOfWeekAfter(this DateTime dateTime, uint days = 6, uint daysToSkip = 1, bool wrap = false)
        {
            var daysOfWeek = new List<DateTime>((int)days);
            for (uint i = daysToSkip; i < days + daysToSkip; i++)
            {
                var newDateTime = dateTime.AddDays(i);
                if (wrap == false && dateTime.DayOfWeek == newDateTime.DayOfWeek)
                {
                    break;
                }

                daysOfWeek.Add(newDateTime);
            }

            return daysOfWeek;
        }

        public static IEnumerable<DateTime> GetQuarterHourIncrementsAfter(this DateTime dateTime, bool wrap = true)
        {
            var quarterHours = new List<DateTime>(QuarterHoursInDay);
            for (int i = 1; i <= QuarterHoursInDay; i++)
            {
                var newDateTime = dateTime.AddMinutes(i * 15);
                if (!wrap && newDateTime.Date.CompareTo(dateTime.Date) > 0)
                {
                    break;
                }

                quarterHours.Add(newDateTime);
            }

            return quarterHours;
        }

        /// <summary>
        /// Returns an ISO formatted date time string for use in JSON.
        /// Ex. 2009-06-15T13:45:30.0000000Z
        /// Format: "yyyy-MM-ddTHH:mm:ss.FFFFFFFK"
        /// </summary>
        public static string ToIsoString(this DateTime dateTime)
        {
            return dateTime.ToUniversalTime().ToString(IsoDateTimeFormatter);
        }
    }
}
