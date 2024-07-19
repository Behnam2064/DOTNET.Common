﻿using DOTNET.Common.Extensions.DateTimes;
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

        /// <summary>
        /// Reminders
        /// </summary>
        public IList<Reminder> Reminders { get; private set; }

        #endregion

        #region Func

        public required Func<OnRefreshRemindersArgument, IList<Reminder>> OnRefreshReminders { get; set; }

        #endregion

        #region Events

        public EventHandler<Reminder> OnReminder { get; set; }

        #endregion

        public ReminderManager(int intervalSecond)
        {
            Reminders = new List<Reminder>();
            Interval = TimeSpan.FromSeconds(intervalSecond).TotalMilliseconds;
            _isRunning = false;

        }
        public ReminderManager() : this(30)
        {

        }

        private System.Timers.Timer Timer;

        public void Start()
        {
            lock (this)
            {
                if (this.Timer != null)
                    throw new InvalidOperationException("The Timer is not null (The services was already started.)");

                if (OnRefreshReminders == null)
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

        private void TimerElapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {

            if (IsInTimerElapsed)
                return;

            lock (this)
            {
                IsInTimerElapsed = true;

                var args = new OnRefreshRemindersArgument();

                var resultReminders = OnRefreshReminders.Invoke(args);
                //The operation has been discontinued
                if (args.IsCancel)
                    return;

                //No need to update the list
                if (!args.PreventUpdatesReminders)
                    Reminders = resultReminders;

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
                                continue;

                            //  1/1/2024 < 2/2/2024
                            //Reminder time is over  
                            if (item.RepeatReminder != null && item.RepeatReminder.EndDateTime != null && item.RepeatReminder.EndDateTime < DateTime.Now)
                            {
                                //item.NextNotify = null;
                                continue;
                            }


                            #endregion

                            if (item.RepeatReminder == null)
                            {
                                if (item.LastNotified == null)
                                {
                                    if (item.StartDateTime.TrimToMinutes() <= DateTime.Now.TrimToMinutes())
                                    {
                                        OnReminder.Invoke(this, item);
                                        item.LastNotified = DateTime.Now;
                                    }
                                }
                                else if (!item.LastNotified.Value.IsToday())
                                {
                                    if (item.StartDateTime.TrimToMinutes() <= DateTime.Now.TrimToMinutes())
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
                                    LastNotified = DateTime.Now; //item.DateTime;
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
                                        if (LDateTime.GetMinutes(LastNotified) >= item.RepeatReminder.AmountTime)
                                        {
                                            OnReminder.Invoke(this, item);
                                            item.LastNotified = DateTime.Now;
                                        }

                                        break;
                                    case RepeatReminderType.Hourly:

                                        if (LDateTime.GetHours(LastNotified) >= item.RepeatReminder.AmountTime)
                                        {
                                            OnReminder.Invoke(this, item);
                                            item.LastNotified = DateTime.Now;
                                        }

                                        break;
                                    case RepeatReminderType.Daliy:

                                        if (LDateTime.GetDays(LastNotified) >= item.RepeatReminder.AmountTime)
                                        {
                                            OnReminder.Invoke(this, item);
                                            item.LastNotified = DateTime.Now;
                                        }

                                        break;
                                    case RepeatReminderType.Weekly:

                                        var days = LDateTime.GetDays(LastNotified);
                                        double AmountWeek = days / 7;
                                        if (AmountWeek >= item.RepeatReminder.AmountTime)
                                        {
                                            OnReminder.Invoke(this, item);
                                            item.LastNotified = DateTime.Now;
                                        }

                                        break;
                                    case RepeatReminderType.WeeklySelective:

                                        if (item.RepeatReminder.Days.Any(x => (DayOfWeek)x == DateTime.Now.DayOfWeek))
                                        {
                                            // Maybe LastNotified will be null
                                            if (item.LastNotified == null || !item.LastNotified.Value.IsToday())
                                            //if (!LastNotified.IsToday())
                                            {
                                                DateTime dateTimeConvertedToNow = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, item.StartDateTime.Hour, item.StartDateTime.Minute, item.StartDateTime.Second);
                                                if (dateTimeConvertedToNow <= DateTime.Now)
                                                {
                                                    OnReminder.Invoke(this, item);
                                                    item.LastNotified = DateTime.Now;
                                                }
                                            }
                                        }

                                        break;
                                    case RepeatReminderType.Monthly:
                                        double AmountMonth = LDateTime.GetDays(LastNotified) / 30;
                                        if (AmountMonth >= item.RepeatReminder.AmountTime)
                                        {
                                            OnReminder.Invoke(this, item);
                                            item.LastNotified = DateTime.Now;
                                        }

                                        break;

                                    case RepeatReminderType.MonthlySelective:
                                    case RepeatReminderType.YearlySelective:
                                        int CurrentMonth = DateTime.Now.Month;
                                        // Maybe LastNotified will be null
                                        if (item.LastNotified == null || !item.LastNotified.Value.IsToday() && item.RepeatReminder.Months.Any(x => x == CurrentMonth))
                                        {
                                            for (int i = 0; i < item.RepeatReminder.Months.Count; i++)
                                            {
                                                if (item.RepeatReminder.Months[i] == CurrentMonth)
                                                {
                                                    if (item.RepeatReminder.Days[i] == DateTime.Now.Day)
                                                    {
                                                        DateTime dateTimeConvertedToNow = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, item.StartDateTime.Hour, item.StartDateTime.Minute, item.StartDateTime.Second);
                                                        if (dateTimeConvertedToNow <= DateTime.Now)
                                                        {
                                                            OnReminder.Invoke(this, item);
                                                            item.LastNotified = DateTime.Now;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        break;
                                    case RepeatReminderType.Yearly:

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

    }
}
