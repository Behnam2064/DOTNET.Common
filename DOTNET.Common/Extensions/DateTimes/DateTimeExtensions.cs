using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DOTNET.Common.Extensions.DateTimes
{
    public static class DateTimeExtensions
    {
        #region Trimers

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

        #endregion

        public static bool IsToday(this DateTime dateTime)
        {
            return dateTime.Year == DateTime.Now.Year && dateTime.Month == DateTime.Now.Month && dateTime.Day == DateTime.Now.Day;
        }

        public static double GetMilliseconds(this DateTime start, DateTime end) => end.Subtract(start).TotalMilliseconds;

        public static double GetSeconds(this DateTime start, DateTime end) => end.Subtract(start).TotalSeconds;

        public static double GetMinutes(this DateTime start, DateTime end) => end.Subtract(start).TotalMinutes;

        public static double GetHours(this DateTime start, DateTime end) => end.Subtract(start).TotalHours;

        public static double GetDays(this DateTime start, DateTime end) => end.Subtract(start).TotalDays;


        /// <summary>
        /// https://stackoverflow.com/questions/902789/how-to-get-the-start-and-end-times-of-a-day
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static DateTime StartOfDay(this DateTime date) => date.Date;

        /// <summary>
        /// https://stackoverflow.com/questions/902789/how-to-get-the-start-and-end-times-of-a-day
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static DateTime EndOfDay(this DateTime date) => date.Date.AddHours(12).AddTicks(-1);

        #region Max and Min

        public static DateTime Max(this DateTime dateTime1, DateTime dateTime2)
        {
            return dateTime1 > dateTime2 ? dateTime1 : dateTime2;
        }

        public static DateTime Min(this DateTime dateTime1, DateTime dateTime2)
        {
            return dateTime1 < dateTime2 ? dateTime1 : dateTime2;
        }

        public static DateTime Max(params DateTime[] dates)
        {
            return dates.Max();
        }

        public static DateTime Min(params DateTime[] dates)
        {
            return dates.Max();
        }

        #endregion

    }
}
