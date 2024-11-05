using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DOTNET.Common.Reminders
{
    public class Reminder
    {
        public string? Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        //public long ReminderSecond { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime? LastNotified { get; set; }

        /// <summary>
        /// Time left until reminder
        /// </summary>
        public TimeSpan? TimeLeft { get; set; }

        public DateTime? NextNotify { get; set; }
        //public DateTime? NextNotify { get; internal set; }
        public RepeatReminder? RepeatReminder { get; set; }

        public string? DebugString { get; set; }

    }
}
