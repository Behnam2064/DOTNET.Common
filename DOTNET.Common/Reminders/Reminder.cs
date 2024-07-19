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
        public DateTime DateTime { get; set; }
        public DateTime? LastNotified { get; set; }
        public RepeatReminder? RepeatReminder { get; set; }

    }
}
