using DOTNET.Common.Extensions.DateTimes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DOTNET.Common.Reminders
{
    public class ReminderManager
    {
        #region Properties

        /// <summary>
        /// Today's reminders
        /// </summary>
        //public int TodayReminders { get; private set; }

        /// <summary>
        /// Tomorrow's reminders
        /// </summary>
        //public int TomorrowReminders { get; private set; }
        public double Interval { get; }
        private bool _isRunning { get; set; }
        public bool IsRunning { get => _isRunning; }

        private bool _InvokeAtConstructor { get; set; }
        public bool InvokeAtConstructor { get => _InvokeAtConstructor; }

        /// <summary>
        /// Reminders
        /// </summary>
        public IList<Reminder> Reminders { get; private set; }

        /// <summary>
        /// Do not invock OnRefreshReminders func every time
        /// </summary>
        public bool IsConstReminderSource { get; set; }

        #endregion

        #region Func

        public Func<OnRefreshRemindersArgument, IList<Reminder>> OnRefreshReminders { get; set; }

        #endregion

        #region Events

        public EventHandler<Reminder> OnReminder { get; set; }

        #endregion

        public ReminderManager(ReminderManagerArgs args)
        {
            Reminders = new List<Reminder>();

            if (args != null)
            {
                if (args.OnRefreshReminders != null)
                    this.OnRefreshReminders = args.OnRefreshReminders;

                if (args.Interval != 0)
                    Interval = TimeSpan.FromSeconds(args.Interval).TotalMilliseconds;
                else
                    Interval = TimeSpan.FromSeconds(30).TotalMilliseconds;

                if (args.Reminders != null)
                    this.Reminders = args.Reminders;


                if (args.OnReminder != null)
                    this.OnReminder += args.OnReminder;

                this.IsConstReminderSource = args.IsConstReminderSource;



                if (args.StartAtConstructor)
                {
                    Start();

                    if (args.InvokeAtConstructor)
                        Task.Run(() => TimerElapsed(Timer, null));

                }

            }
            else
            {
                Interval = TimeSpan.FromSeconds(30).TotalMilliseconds;

            }

        }

        public ReminderManager(int intervalSecond) : this(new ReminderManagerArgs() { Interval = intervalSecond })
        {


        }
        public ReminderManager() : this(new ReminderManagerArgs() { Interval = 30 })
        {

        }

        private System.Timers.Timer Timer;

        public void Start()
        {
            lock (this)
            {
                if (this.Timer != null)
                    throw new InvalidOperationException("The Timer is not null (The services was already started.)");

                if ((IsConstReminderSource && Reminders == null) || (!IsConstReminderSource && OnRefreshReminders == null))
                    throw new ArgumentNullException($"The '{nameof(OnRefreshReminders)}' property is null");

                this.Timer = new System.Timers.Timer();
                this.Timer.Interval = this.Interval;
                this.Timer.AutoReset = true;
                this.Timer.Enabled = true;
                _isRunning = true;
                this.Timer.Elapsed += TimerElapsed;
                this.Timer.Start();
            }

        }

        public void Stop()
        {
            lock (this)
            {
                if (this.Timer == null)
                    throw new InvalidOperationException("The Timer is null (The services was't start yet.)");

                this.Timer.AutoReset = false;
                this.Timer.Enabled = false;
                this.Timer.Stop();
                this.Timer.Dispose();
                _isRunning = false;
            }

        }

        private bool IsInTimerElapsed { get; set; }

        private void TimerElapsed(object? sender, System.Timers.ElapsedEventArgs? e)
        {

            if (IsInTimerElapsed)
                return;

            lock (this)
            {
                IsInTimerElapsed = true;

                if (!IsConstReminderSource)
                {
                    OnRefreshRemindersArgument args = new OnRefreshRemindersArgument();

                    IList<Reminder> resultReminders = OnRefreshReminders.Invoke(args);
                    //The operation has been discontinued
                    if (args.IsCancel)
                        return;

                    //No need to update the list
                    if (!args.PreventUpdatesReminders)
                        Reminders = resultReminders;
                }

                foreach (var item in Reminders)
                {
                    if (item.RepeatReminder == null || item.RepeatReminder.Days == null)
                        continue;


                    if (item.RepeatReminder.Days.GroupBy(x => x).Any(x => x.Count() > 1))
                    {
                        if (item.RepeatReminder.RepeatType == RepeatReminderType.WeeklySelective)
                            throw new InvalidOperationException("It is not possible to register a day of the week as a reminder multiple times");
                        else if (item.RepeatReminder.RepeatType == RepeatReminderType.MonthlySelective)
                        {
                            //Can not sort days and month.Their indexed to each other

                            List<(int day, int month)> daysList = new List<(int, int)>();
                            for (int y = 0; y < item.RepeatReminder.Days.Count; y++)// The Days and Month is same length and index
                                daysList.Add(new(item.RepeatReminder.Days[y], item.RepeatReminder.Months[y]));

                            var duplicateDays = daysList
                                    .GroupBy(d => d)
                                    .Where(g => g.Count() > 1)
                                    .Select(g => g.Key);

                            if (duplicateDays.Any())
                                throw new InvalidOperationException("Cannot record a reminder with duplicate day and month");

                        }
                    }
                }

                using (BackgroundWorker bw = new BackgroundWorker())
                {
                    bw.DoWork += (s1, e1) =>
                    {

                        if (Reminders == null)
                            Reminders = new List<Reminder>();



                        foreach (var item in Reminders)
                        {
                            #region Time check

                            //  2/2/2024 > 1/1/2024
                            // The time to remember has not yet come
                            if (item.StartDateTime > DateTime.Now)
                            {
                                item.TimeLeft = item.StartDateTime.Subtract(DateTime.Now);
                                item.NextNotify = DateTime.Now.AddTicks(item.TimeLeft.Value.Ticks);
                                continue;
                            }

                            //  1/1/2024 < 2/2/2024
                            //Reminder time is over  
                            if (item.RepeatReminder != null && item.RepeatReminder.EndDateTime != null && item.RepeatReminder.EndDateTime < DateTime.Now)
                            {
                                //item.NextNotify = null;
                                //Calculate
                                //Elapsed time
                                continue;
                            }

                            if (item.RepeatReminder != null)
                            {
                                item.TimeLeft = DateTime.Now.Max(item.StartDateTime).Subtract(DateTime.Now.Min(item.StartDateTime));
                            }
                            #endregion

                            if (item.RepeatReminder == null)
                            {
                                if (item.LastNotified == null)
                                {
                                    if (item.StartDateTime.TrimToSeconds() <= DateTime.Now.TrimToSeconds())
                                    {
                                        OnReminder.Invoke(this, item);
                                        item.LastNotified = DateTime.Now;
                                    }
                                }
                                else if (!item.LastNotified.Value.IsToday())
                                {
                                    if (item.StartDateTime.TrimToSeconds() <= DateTime.Now.TrimToSeconds())
                                    {
                                        OnReminder.Invoke(this, item);
                                        item.LastNotified = DateTime.Now;
                                    }
                                }

                            }
                            else
                            {

                                DateTime LastNotified = DateTime.MinValue;
                                DateTime LDateTime = DateTime.MinValue;
                                if (item.LastNotified == null)
                                {
                                    LastNotified = DateTime.Now;
                                    LDateTime = item.StartDateTime;

                                }
                                else
                                {
                                    LastNotified = DateTime.Now;
                                    LDateTime = (DateTime)item.LastNotified;
                                }


                                /*DateTime LastNotified = item.LastNotified ?? DateTime.Now; //item.DateTime;
                                DateTime LDateTime = item.LastNotified ?? item.StartDateTime;*/
                                switch (item.RepeatReminder.RepeatType)
                                {
                                    case RepeatReminderType.Minutely:

                                        #region Calculate next notify and time left

                                        item.NextNotify = LDateTime.AddMinutes(item.RepeatReminder.AmountTime);
                                        item.TimeLeft = item.NextNotify.Value.Subtract(DateTime.Now);

                                        #endregion

                                        if (LDateTime.GetMinutes(LastNotified) >= item.RepeatReminder.AmountTime)
                                        {
                                            OnReminder.Invoke(this, item);
                                            item.LastNotified = DateTime.Now;
                                        }

                                        break;
                                    case RepeatReminderType.Hourly:

                                        #region Calculate next notify and time left
                                        item.NextNotify = LDateTime.AddHours(item.RepeatReminder.AmountTime);
                                        item.TimeLeft = item.NextNotify.Value.Subtract(DateTime.Now);
                                        #endregion

                                        if (LDateTime.GetHours(LastNotified) >= item.RepeatReminder.AmountTime)
                                        {
                                            OnReminder.Invoke(this, item);
                                            item.LastNotified = DateTime.Now;
                                        }

                                        break;
                                    case RepeatReminderType.Daily:

                                        #region Calculate next notify and time left

                                        item.NextNotify = LDateTime.AddDays(item.RepeatReminder.AmountTime);
                                        item.TimeLeft = item.NextNotify.Value.Subtract(DateTime.Now);

                                        #endregion

                                        if (LDateTime.GetDays(LastNotified) >= item.RepeatReminder.AmountTime)
                                        {
                                            OnReminder.Invoke(this, item);
                                            item.LastNotified = DateTime.Now;
                                        }

                                        break;
                                    case RepeatReminderType.Weekly:

                                        #region Calculate next notify and time left

                                        item.NextNotify = LDateTime.AddDays(item.RepeatReminder.AmountTime * 7);
                                        item.TimeLeft = item.NextNotify.Value.Subtract(DateTime.Now);

                                        #endregion

                                        var days = LDateTime.GetDays(LastNotified);
                                        double AmountWeek = days / 7;


                                        if (AmountWeek >= item.RepeatReminder.AmountTime)
                                        {
                                            OnReminder.Invoke(this, item);
                                            item.LastNotified = DateTime.Now;
                                        }

                                        break;
                                    case RepeatReminderType.WeeklySelective:

                                        int IntDayOfWeek = (int)DateTime.Now.DayOfWeek;
                                        bool IsIncloudToday = item.RepeatReminder.Days.Any(x => x == IntDayOfWeek);
                                        if (IsIncloudToday)
                                        {

                                            DateTime ConvertedToNow = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, item.StartDateTime.Hour, item.StartDateTime.Minute, item.StartDateTime.Second);
                                            var ConvertedSecondTrimed = ConvertedToNow.TrimToSeconds();
                                            var NowSecondTrimed = DateTime.Now.TrimToSeconds();
                                            //ConvertedSecondTrimed == NowSecondTrimed || dateTimeConvertedToNow.GetSeconds(DateTime.Now) <= TimeSpan.FromMilliseconds(this.Interval).TotalSeconds)
                                            var Max = ConvertedToNow.Max(DateTime.Now);
                                            var Min = ConvertedToNow.Min(DateTime.Now);
                                            var diff = Min.GetSeconds(Max);

                                            #region Recently displayed
                                            //Has an event recently been displayed? (Less than or equal to one minute ago)
                                            //If there is no code below, each event will be displayed with two characters
                                            bool HasRecenctlyDisplayed = false; //If the event was displayed less than a minute ago, do not display it again
                                            if (item.LastNotified == null)
                                            {
                                                //It has never been displayed
                                                HasRecenctlyDisplayed = false; 
                                            }
                                            else
                                            {
                                                //Has the current event happened in the last minute?
                                                HasRecenctlyDisplayed = DateTime.Now.Subtract(item.LastNotified.Value).TotalMinutes <= 1;
                                            }

                                            #endregion

                                            if (
                                            (item.LastNotified == null || 
                                            DateTime.Now.Subtract(item.LastNotified.Value).TotalDays >= 1) //If a notification has not been displayed or more than a day has passed since the last notification was displayed
                                            && 
                                             NowSecondTrimed == ConvertedSecondTrimed
                                            || (!HasRecenctlyDisplayed && diff <= TimeSpan.FromMilliseconds(this.Interval).TotalSeconds)
                                            )
                                            {
                                                OnReminder.Invoke(this, item);
                                                item.LastNotified = DateTime.Now;
                                                //Do not use break to find next notify date time like tomorrow
                                            }
                                            else
                                            {
                                                #region Calculate next notify and time left
                                                item.NextNotify = ConvertedToNow;
                                                item.TimeLeft = item.NextNotify.Value.Subtract(DateTime.Now);
                                                #endregion
                                                if (item.NextNotify > DateTime.Now)
                                                    break; //It includes today and not raised event so still next notify is today || do not go next line (find next day)
                                                else //Time has passed
                                                {
                                                    //Unti next weeek just same day
                                                    item.NextNotify = item.NextNotify.Value.AddDays(7); // Next week
                                                    item.TimeLeft = item.NextNotify.Value.Subtract(DateTime.Now);

                                                    // Do not use break || Not happening today
                                                    // Maybe next day of week exist 
                                                }
                                            }

                                            #region Debug

                                            item.DebugString +=
                                                            Environment.NewLine + $"IsOnReminderInvockedDebug =>{IsOnReminderInvockedDebug}"
                                                            + Environment.NewLine + $"LastNotified is null =>{item.LastNotified == null}"
                                                            + Environment.NewLine + $"DateTime.Now.Subtract(item.LastNotified.Value).TotalDays >= 1 =>{DateTime.Now.Subtract(item.LastNotified.Value).TotalDays >= 1}"
                                                            + Environment.NewLine + $"NowSecondTrimed == ConvertedSecondTrimed =>{NowSecondTrimed == ConvertedSecondTrimed}"
                                                            + Environment.NewLine + $"diff <= TimeSpan.FromMilliseconds(this.Interval).TotalSeconds =>{diff <= TimeSpan.FromMilliseconds(this.Interval).TotalSeconds}"
                                                            + Environment.NewLine + $"NowSecondTrimed =>{NowSecondTrimed}"
                                                            + Environment.NewLine + $"ConvertedSecondTrimed =>{ConvertedSecondTrimed}"
                                                            + Environment.NewLine + $"diff (TotalSeconds) =>{diff}"
                                                            + Environment.NewLine + $"DateTime.Now.Subtract(item.LastNotified.Value).TotalDays =>{DateTime.Now.Subtract(item.LastNotified.Value).TotalDays}"
                                                            + Environment.NewLine + $"TimeSpan.FromMilliseconds(this.Interval).TotalSeconds =>{TimeSpan.FromMilliseconds(this.Interval).TotalSeconds}"
                                                            .TrimStart();

                                            #endregion
                                        }

                                        #region Calculate next notify and time left

                                        //This mean event is now and was invoked before got to next day of week
                                        //Next day of week if exist

                                        /*
                                                                                if (IntDayOfWeek > (int)DayOfWeek.Saturday)
                                                                                    IntDayOfWeek = 0;*/

                                        if (item.RepeatReminder.Days.Any(x => x > IntDayOfWeek))
                                        {
                                            //Like Tomorrow
                                            item.NextNotify = new DateTime
                                            (DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, item.StartDateTime.Hour, item.StartDateTime.Minute, item.StartDateTime.Second)
                                            .NextDayOfWeek((DayOfWeek)item.RepeatReminder.Days.Where(x => x > IntDayOfWeek).FirstOrDefault());

                                            //(DayOfWeek)IntDayOfWeek;
                                            item.TimeLeft = item.NextNotify.Value.Subtract(DateTime.Now);
                                        }
                                        else if (item.RepeatReminder.Days.Any(x => x < IntDayOfWeek))
                                        {
                                            //For example, next week but one day before today
                                            item.NextNotify = new DateTime
                                            (DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, item.StartDateTime.Hour, item.StartDateTime.Minute, item.StartDateTime.Second)
                                            .NextDayOfWeek((DayOfWeek)item.RepeatReminder.Days.Where(x => x < IntDayOfWeek).FirstOrDefault());

                                            //(DayOfWeek)IntDayOfWeek;
                                            item.TimeLeft = item.NextNotify.Value.Subtract(DateTime.Now);
                                        }

                                        #endregion


                                        break;
                                    case RepeatReminderType.Monthly:

                                        #region Calculate next notify and time left

                                        item.NextNotify = LDateTime.AddDays(item.RepeatReminder.AmountTime * 30);
                                        item.TimeLeft = item.NextNotify.Value.Subtract(DateTime.Now);

                                        #endregion

                                        double AmountMonth = LDateTime.GetDays(LastNotified) / 30;


                                        if (AmountMonth >= item.RepeatReminder.AmountTime)
                                        {
                                            OnReminder.Invoke(this, item);
                                            item.LastNotified = DateTime.Now;
                                        }

                                        break;

                                    case RepeatReminderType.MonthlySelective:
                                        //case RepeatReminderType.YearlySelective:
                                        int CurrentMonth = DateTime.Now.Month;
                                        int CurrentDay = DateTime.Now.Day;
                                        // Maybe LastNotified will be null
                                        /*
                                                                                if (*//*item.LastNotified == null || !item.LastNotified.Value.IsToday() && *//*item.RepeatReminder.Months.Any(x => x == CurrentMonth))
                                                                                {
                                                                                    for (int i = 0; i < item.RepeatReminder.Months.Count; i++)
                                                                                    {
                                                                                        if (item.RepeatReminder.Months[i] == CurrentMonth)
                                                                                        {
                                                                                            if (item.RepeatReminder.Days[i] == DateTime.Now.Day)
                                                                                            {
                                                                                                DateTime dateTimeConvertedToNow = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, item.StartDateTime.Hour, item.StartDateTime.Minute, item.StartDateTime.Second).TrimToSeconds();
                                                                                                DateTime DateTimeNowTrimToMinutes = DateTime.Now.TrimToSeconds();

                                                                                                var Max = dateTimeConvertedToNow.Max(DateTime.Now);
                                                                                                var Min = dateTimeConvertedToNow.Min(DateTime.Now);
                                                                                                var diff = Min.GetSeconds(Max);

                                                                                                if (dateTimeConvertedToNow == DateTimeNowTrimToMinutes || diff <= TimeSpan.FromMilliseconds(this.Interval).TotalSeconds)
                                                                                                {
                                                                                                    OnReminder.Invoke(this, item);
                                                                                                    item.LastNotified = DateTime.Now;
                                                                                                }
                                                                                            }
                                                                                        }
                                                                                    }
                                                                                }
                                        */
                                        //bool IsIncloudToday2 = item.RepeatReminder.Months.Any(x => x == CurrentMonth) && item.RepeatReminder.Days.Any(x => x == CurrentDay);

                                        List<(int day, int month)> daysList = new List<(int, int)>();
                                        for (int y = 0; y < item.RepeatReminder.Days.Count; y++)// The Days and Month is same length and index
                                            daysList.Add(new(item.RepeatReminder.Days[y], item.RepeatReminder.Months[y]));

                                        bool IsIncloudToday2 = daysList.Any(x => x.month == CurrentMonth && x.day == CurrentDay);


                                        if (IsIncloudToday2)
                                        {
                                            var iff = daysList.First(x => x.month == CurrentMonth && x.day == CurrentDay);
                                            int day = iff.day; // item.RepeatReminder.Days.FirstOrDefault(x => x == CurrentDay);
                                            int month = iff.month; // item.RepeatReminder.Months.FirstOrDefault(x => x == CurrentMonth);

                                            DateTime dateTimeConvertedToNow = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, item.StartDateTime.Hour, item.StartDateTime.Minute, item.StartDateTime.Second).TrimToSeconds();
                                            DateTime DateTimeNowTrimToMinutes = DateTime.Now.TrimToSeconds();

                                            var Max = dateTimeConvertedToNow.Max(DateTime.Now);
                                            var Min = dateTimeConvertedToNow.Min(DateTime.Now);
                                            var diff = Min.GetSeconds(Max);



                                            #region Recently displayed
                                            //Has an event recently been displayed? (Less than or equal to one minute ago)
                                            //If there is no code below, each event will be displayed with two characters
                                            bool HasRecenctlyDisplayed = false; //If the event was displayed less than a minute ago, do not display it again
                                            if (item.LastNotified == null)
                                            {
                                                //It has never been displayed
                                                HasRecenctlyDisplayed = false;
                                            }
                                            else
                                            {
                                                //Has the current event happened in the last minute?
                                                HasRecenctlyDisplayed = DateTime.Now.Subtract(item.LastNotified.Value).TotalMinutes <= 1;
                                            }

                                            #endregion

                                            if (
                                            (item.LastNotified == null || 
                                            DateTime.Now.Subtract(item.LastNotified.Value).TotalDays >= 1) //If a notification has not been displayed or more than a day has passed since the last notification was displayed
                                            && 
                                            dateTimeConvertedToNow == DateTimeNowTrimToMinutes ||
                                            (!HasRecenctlyDisplayed && diff <= TimeSpan.FromMilliseconds(this.Interval).TotalSeconds))
                                            {
                                                OnReminder.Invoke(this, item);
                                                item.LastNotified = DateTime.Now;
                                            }
                                            else
                                            {

                                                #region Calculate next notify and time left
                                                item.NextNotify = dateTimeConvertedToNow;
                                                item.TimeLeft = item.NextNotify.Value.Subtract(DateTime.Now);
                                                #endregion
                                                if (item.NextNotify > DateTime.Now)
                                                    break; //It includes today and not raised event so still next notify is today || do not go next line (find next day)
                                                else //Time has passed
                                                {
                                                    //Unti next weeek just same day
                                                    item.NextNotify = item.NextNotify.Value.AddDays(30); // Next month
                                                    item.TimeLeft = item.NextNotify.Value.Subtract(DateTime.Now);

                                                    // Do not use break || Not happening today
                                                    // Maybe next day of week exist 
                                                }
                                            }
                                        }



                                        if (item.RepeatReminder.Days.Any(x => x > CurrentDay))
                                        {
                                            var d1 = item.RepeatReminder.Days.FirstOrDefault(x => x > CurrentDay);
                                            //Like Tomorrow
                                            try
                                            {

                                                item.NextNotify = new DateTime
                                                (DateTime.Now.Year,
                                                DateTime.Now.Month, //Next day of current month : like today is 10 and Tomorrow is 11
                                                d1,
                                                item.StartDateTime.Hour,
                                                item.StartDateTime.Minute,
                                                item.StartDateTime.Second);

                                            }
                                            catch (System.ArgumentOutOfRangeException)
                                            {

                                                item.NextNotify = new DateTime
                                                (DateTime.Now.Year,
                                                DateTime.Now.Month, 
                                                1, // 29,30,31 not exist
                                                item.StartDateTime.Hour,
                                                item.StartDateTime.Minute,
                                                item.StartDateTime.Second);
                                            }
                                            //(DayOfWeek)IntDayOfWeek;
                                            item.TimeLeft = item.NextNotify.Value.Subtract(DateTime.Now);
                                        }
                                        else if (item.RepeatReminder.Days.Any(x => x < CurrentDay))
                                        {
                                            try
                                            {

                                                //For example, next week but one day before today
                                                item.NextNotify = new DateTime
                                                (DateTime.Now.Year,
                                                DateTime.Now.Month,
                                                item.RepeatReminder.Days.FirstOrDefault(x => x < CurrentDay),
                                                item.StartDateTime.Hour,
                                                item.StartDateTime.Minute,
                                                item.StartDateTime.Second);

                                            }
                                            catch (System.ArgumentOutOfRangeException)
                                            {
                                                //For example, next week but one day before today
                                                item.NextNotify = new DateTime
                                                (DateTime.Now.Year,
                                                DateTime.Now.Month,
                                                1, // 29,30,31 not exist
                                                item.StartDateTime.Hour,
                                                item.StartDateTime.Minute,
                                                item.StartDateTime.Second);

                                            }
                                            //(DayOfWeek)IntDayOfWeek;
                                            item.TimeLeft = item.NextNotify.Value.Subtract(DateTime.Now);
                                        }



                                        break;
                                    case RepeatReminderType.Yearly:

                                        #region Calculate next notify and time left

                                        item.NextNotify = LDateTime.AddDays(item.RepeatReminder.AmountTime * 360);
                                        item.TimeLeft = item.NextNotify.Value.Subtract(DateTime.Now);

                                        #endregion

                                        double AmountYear = LDateTime.GetDays(LastNotified) / 365;


                                        if (AmountYear >= item.RepeatReminder.AmountTime)
                                        {
                                            OnReminder.Invoke(this, item);
                                            item.LastNotified = DateTime.Now;
                                        }
                                        break;
                                    default:
                                        break;
                                }
                            }
                        }
                    };

                    bw.RunWorkerCompleted += (s1, e1) =>
                    {
                        IsInTimerElapsed = false;
                    };
                    bw.RunWorkerAsync();


                }
            }
        }
        public DateTime StartOfWeek(DateTime dt, DayOfWeek startOfWeek)
        {
            int diff = (7 + (dt.DayOfWeek - startOfWeek)) % 7;
            return dt.AddDays(-1 * diff).Date;
        }

    }
}

