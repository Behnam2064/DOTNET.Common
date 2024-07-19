using DOTNET.Common.Extensions.DateTimes;
using DOTNET.Common.Reminders;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.DOTNET.Common.Reminders
{
    [TestClass]
    public class ReminderManagerTest
    {

        [TestMethod]
        public void NormalReminder()
        {
            string Id = "123";
            var args = new ReminderManagerArgs()
            {
                Interval = 1,
                IsConstReminderSource = true,
                Reminders = new List<Reminder>() { new Reminder() { Id = Id, Title = "A normal reminder ", StartDateTime = DateTime.Now.AddSeconds(2) } },
                StartAtConstructor = true,
            };


            bool CanBreak = false;
            bool IsTrue = false;
            args.OnReminder += (s, r) =>
            {
                if (r.Id == Id)
                {
                    var sec = r.StartDateTime.GetSeconds(DateTime.Now);
                    Debug.WriteLine("Sec:" + sec);
                    IsTrue = args.Interval >= sec;
                    Assert.IsTrue(IsTrue);
                    CanBreak = true;
                }
            };

            ReminderManager rm = new ReminderManager(args);


            int Count = 0;
            while (++Count <= 6)
            {
                if (CanBreak)
                    break;
                Thread.Sleep(1000);

            }

            if (!IsTrue && !CanBreak)
                Assert.IsTrue(IsTrue);


            rm.Stop();

        }

        [TestMethod]
        public void MinutelyReminder()
        {
            string Id = "124";
            var args = new ReminderManagerArgs()
            {
                Interval = 1,
                IsConstReminderSource = true,
                Reminders = new List<Reminder>()
                {
                    new Reminder()
                    {
                        Id = Id,
                        LastNotified = DateTime.Now.AddMinutes(-1).AddSeconds(1),
                        Title = "A Minutely reminder ",
                        StartDateTime = DateTime.Now.AddSeconds(1),
                         RepeatReminder = new RepeatReminder()
                         {
                             AmountTime = 1,
                             RepeatType = RepeatReminderType.Minutely,
                         }

                    }
                },

                StartAtConstructor = true,
            };


            bool CanBreak = false;
            bool IsTrue = false;
            int CountTrue = 0;
            args.OnReminder += (s, r) =>
            {
                if (r.Id == Id)
                {
                    var sec = r.StartDateTime.GetSeconds(DateTime.Now);
                    IsTrue = args.Interval >= sec;
                    if (IsTrue)
                    {
                        Debug.WriteLine("Is true");
                        CountTrue++;
                    }

                    if (CountTrue >= 1)
                    {
                        CanBreak = true;
                    }

                }
            };

            


            ReminderManager rm = new ReminderManager(args);


            int Count = 0;
            while (++Count <= 5)
            {
                if (CanBreak)
                    break;
                Thread.Sleep(1000);

            }

            IsTrue = CountTrue >= 1;
            Assert.IsTrue(IsTrue);

            if (!IsTrue && !CanBreak)
                Assert.IsTrue(IsTrue);


            rm.Stop();

        }

        [TestMethod]
        public void HourlyReminder()
        {
            string Id = "124";
            var args = new ReminderManagerArgs()
            {
                Interval = 1,
                IsConstReminderSource = true,
                Reminders = new List<Reminder>()
                {
                    new Reminder()
                    {
                        Id = Id,
                        LastNotified = DateTime.Now.AddHours(-1).AddSeconds(1),
                        Title = "A Hourly reminder ",
                        StartDateTime = DateTime.Now.AddSeconds(1),
                         RepeatReminder = new RepeatReminder()
                         {
                             AmountTime = 1,
                             RepeatType = RepeatReminderType.Hourly,
                         }

                    }
                },

                StartAtConstructor = true,
            };


            bool CanBreak = false;
            bool IsTrue = false;
            int CountTrue = 0;
            args.OnReminder += (s, r) =>
            {
                if (r.Id == Id)
                {
                    var sec = r.StartDateTime.GetSeconds(DateTime.Now);
                    IsTrue = args.Interval >= sec;
                    if (IsTrue)
                    {
                        Debug.WriteLine("Is true");
                        CountTrue++;
                    }

                    if (CountTrue >= 1)
                    {
                        CanBreak = true;
                    }

                }
            };

            


            ReminderManager rm = new ReminderManager(args);


            int Count = 0;
            while (++Count <= 5)
            {
                if (CanBreak)
                    break;
                Thread.Sleep(1000);

            }

            IsTrue = CountTrue >= 1;
            Assert.IsTrue(IsTrue);

            if (!IsTrue && !CanBreak)
                Assert.IsTrue(IsTrue);


            rm.Stop();

        }

        [TestMethod]
        public void DaylyReminder()
        {
            string Id = "124";
            var args = new ReminderManagerArgs()
            {
                Interval = 1,
                IsConstReminderSource = true,
                Reminders = new List<Reminder>()
                {
                    new Reminder()
                    {
                        Id = Id,
                        LastNotified = DateTime.Now.AddDays(-1).AddSeconds(1),
                        Title = "A Dayly reminder ",
                        StartDateTime = DateTime.Now.AddSeconds(1),
                         RepeatReminder = new RepeatReminder()
                         {
                             AmountTime = 1,
                             RepeatType = RepeatReminderType.Daliy,
                         }

                    }
                },

                StartAtConstructor = true,
            };


            bool CanBreak = false;
            bool IsTrue = false;
            int CountTrue = 0;
            args.OnReminder += (s, r) =>
            {
                if (r.Id == Id)
                {
                    var sec = r.StartDateTime.GetSeconds(DateTime.Now);
                    IsTrue = args.Interval >= sec;
                    if (IsTrue)
                    {
                        Debug.WriteLine("Is true");
                        CountTrue++;
                    }

                    if (CountTrue >= 1)
                    {
                        CanBreak = true;
                    }

                }
            };




            ReminderManager rm = new ReminderManager(args);


            int Count = 0;
            while (++Count <= 5)
            {
                if (CanBreak)
                    break;
                Thread.Sleep(1000);

            }

            IsTrue = CountTrue >= 1;
            Assert.IsTrue(IsTrue);

            if (!IsTrue && !CanBreak)
                Assert.IsTrue(IsTrue);


            rm.Stop();

        }

        [TestMethod]
        public void WeeklyReminder()
        {
            string Id = "125";
            var args = new ReminderManagerArgs()
            {
                Interval = 1,
                IsConstReminderSource = true,
                Reminders = new List<Reminder>()
                {
                    new Reminder()
                    {
                        Id = Id,
                        LastNotified = DateTime.Now.AddDays(-7).AddSeconds(1),
                        Title = "A weekly reminder ",
                        StartDateTime = DateTime.Now.AddSeconds(1),
                         RepeatReminder = new RepeatReminder()
                         {
                             AmountTime = 1,
                             RepeatType = RepeatReminderType.Weekly,
                         }

                    }
                },

                StartAtConstructor = true,
            };


            bool CanBreak = false;
            bool IsTrue = false;
            int CountTrue = 0;
            args.OnReminder += (s, r) =>
            {
                if (r.Id == Id)
                {
                    var sec = r.StartDateTime.GetSeconds(DateTime.Now);
                    IsTrue = args.Interval >= sec;
                    if (IsTrue)
                    {
                        Debug.WriteLine("Is true");
                        CountTrue++;
                    }

                    if (CountTrue >= 1)
                    {
                        CanBreak = true;
                    }

                }
            };




            ReminderManager rm = new ReminderManager(args);


            int Count = 0;
            while (++Count <= 5)
            {
                if (CanBreak)
                    break;
                Thread.Sleep(1000);

            }

            IsTrue = CountTrue >= 1;
            Assert.IsTrue(IsTrue);

            if (!IsTrue && !CanBreak)
                Assert.IsTrue(IsTrue);


            rm.Stop();

        }

        [TestMethod]
        public void MonthlyReminder()
        {
            string Id = "125";
            var args = new ReminderManagerArgs()
            {
                Interval = 1,
                IsConstReminderSource = true,
                Reminders = new List<Reminder>()
                {
                    new Reminder()
                    {
                        Id = Id,
                        LastNotified = DateTime.Now.AddDays(-30).AddSeconds(1),
                        Title = "A Monthly reminder ",
                        StartDateTime = DateTime.Now.AddSeconds(1),
                         RepeatReminder = new RepeatReminder()
                         {
                             AmountTime = 1,
                             RepeatType = RepeatReminderType.Monthly,
                         }

                    }
                },

                StartAtConstructor = true,
            };


            bool CanBreak = false;
            bool IsTrue = false;
            int CountTrue = 0;
            args.OnReminder += (s, r) =>
            {
                if (r.Id == Id)
                {
                    var sec = r.StartDateTime.GetSeconds(DateTime.Now);
                    IsTrue = args.Interval >= sec;
                    if (IsTrue)
                    {
                        Debug.WriteLine("Is true");
                        CountTrue++;
                    }

                    if (CountTrue >= 1)
                    {
                        CanBreak = true;
                    }

                }
            };




            ReminderManager rm = new ReminderManager(args);


            int Count = 0;
            while (++Count <= 5)
            {
                if (CanBreak)
                    break;
                Thread.Sleep(1000);

            }

            IsTrue = CountTrue >= 1;
            Assert.IsTrue(IsTrue);

            if (!IsTrue && !CanBreak)
                Assert.IsTrue(IsTrue);


            rm.Stop();

        }


        [TestMethod]
        public void Yearlyeminder()
        {
            string Id = "125";
            var args = new ReminderManagerArgs()
            {
                Interval = 1,
                IsConstReminderSource = true,
                Reminders = new List<Reminder>()
                {
                    new Reminder()
                    {
                        Id = Id,
                        LastNotified = DateTime.Now.AddYears(-1).AddSeconds(1),
                        Title = "A Monthly reminder ",
                        StartDateTime = DateTime.Now.AddSeconds(1),
                         RepeatReminder = new RepeatReminder()
                         {
                             AmountTime = 1,
                             RepeatType = RepeatReminderType.Yearly,
                         }

                    }
                },

                StartAtConstructor = true,
            };


            bool CanBreak = false;
            bool IsTrue = false;
            int CountTrue = 0;
            args.OnReminder += (s, r) =>
            {
                if (r.Id == Id)
                {
                    var sec = r.StartDateTime.GetSeconds(DateTime.Now);
                    IsTrue = args.Interval >= sec;
                    if (IsTrue)
                    {
                        Debug.WriteLine("Is true");
                        CountTrue++;
                    }

                    if (CountTrue >= 1)
                    {
                        CanBreak = true;
                    }

                }
            };




            ReminderManager rm = new ReminderManager(args);


            int Count = 0;
            while (++Count <= 5)
            {
                if (CanBreak)
                    break;
                Thread.Sleep(1000);

            }

            IsTrue = CountTrue >= 1;
            Assert.IsTrue(IsTrue);

            if (!IsTrue && !CanBreak)
                Assert.IsTrue(IsTrue);


            rm.Stop();

        }

    }
}
