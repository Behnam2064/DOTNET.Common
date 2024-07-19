using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DOTNET.Common.Reminders
{
    public class OnRefreshRemindersArgument
    {
        /// <summary>
        /// The operation has been discontinued
        /// </summary>
        public bool IsCancel { get; set; }

        /// <summary>
        /// There is no need to update the reminder list
        /// </summary>
        public bool PreventUpdatesReminders { get; set; }

    }
}
