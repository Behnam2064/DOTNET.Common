using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DOTNET.Common.Reminders
{
    public class ReminderManagerArgs
    {
        /// <summary>
        /// Second
        /// </summary>
        public double Interval { get; set; }

        public Func<OnRefreshRemindersArgument, IList<Reminder>>? OnRefreshReminders { get; set; }

        public IList<Reminder>? Reminders { get; set; }

        public bool StartAtConstructor { get; set; }

        /// <summary>
        /// Do not invock OnRefreshReminders every time
        /// </summary>
        public bool IsConstReminderSource { get; set; }
    }
}
