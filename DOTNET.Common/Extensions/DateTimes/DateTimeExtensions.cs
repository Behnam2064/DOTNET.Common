using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DOTNET.Common.Extensions.DateTimes
{
    public static class DateTimeExtensions
    {

        public static DateTime TrimMicroseconds(this DateTime dateTime)
        {
            return dateTime.AddMicroseconds(-dateTime.Microsecond);
        }


        public static DateTime TrimMilliseconds(this DateTime dateTime)
        {
            return dateTime.AddMilliseconds(-dateTime.Millisecond);
        }

        public static DateTime TrimSeconds(this DateTime dateTime)
        {
            return dateTime.AddSeconds(-dateTime.Second);
        }

        public static DateTime TrimMinutes(this DateTime dateTime)
        {
            return dateTime.AddMinutes(-dateTime.Minute);
        }


        public static DateTime TrimToSeconds(this DateTime dateTime)
        {
            return dateTime.TrimMicroseconds().TrimMilliseconds().TrimSeconds();
        }

        public static DateTime TrimToMinutes(this DateTime dateTime)
        {
            return dateTime.TrimToSeconds().TrimMinutes();
        }

        public static bool IsToday(this DateTime dateTime)
        {
            return dateTime.Year == DateTime.Now.Year && dateTime.Month == DateTime.Now.Month && dateTime.Day == DateTime.Now.Day;
        }

        public static double GetMilliseconds(this DateTime start, DateTime end) => end.Subtract(start).TotalMilliseconds;

        public static double GetSeconds(this DateTime start, DateTime end) => end.Subtract(start).TotalSeconds;

        public static double GetMinutes(this DateTime start, DateTime end) => end.Subtract(start).TotalMinutes;

        public static double GetHours(this DateTime start, DateTime end) => end.Subtract(start).TotalHours;

        public static double GetDays(this DateTime start, DateTime end) => end.Subtract(start).TotalDays;

    }
}
