namespace DOTNET.Common.Reminders
{
    public class RepeatReminder
    {
        public string? Id { get; set; }

        public RepeatReminderType RepeatType { get; set; }

        /// <summary>
        /// Day of week
        /// </summary>
        public IList<int> Days { get; set; }

        /// <summary>
        /// Month of year
        /// </summary>
        public IList<int> Months { get; set; }

        public DateTime? EndDateTime { get; set; }



        /// <summary>
        /// Amount of repetition time
        /// Default value is 1 (Like 1 Minute, 1 Hour, 1 Day and ...)
        /// </summary>
        public double AmountTime { get; set; }

        public RepeatReminder()
        {
            AmountTime = 1;
            Days = new List<int>();
            Months = new List<int>();
        }
    }
}
