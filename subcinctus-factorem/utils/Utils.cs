using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace subcinctus_factorem.utils
{
    public class Utils
    {
        public List<DateTime> GetCurrentWeekDates()
        {
            DateTime today = DateTime.Today;

            // Find the most recent Sunday
            int daysSinceSunday = (int)today.DayOfWeek;
            DateTime sunday = today.AddDays(-daysSinceSunday);

            // Generate 7 days (Sunday to Saturday)
            List<DateTime> weekDates = new List<DateTime>();
            for (int i = 0; i < 7; i++)
            {
                weekDates.Add(sunday.AddDays(i));
            }

            return weekDates;
        }
    }
}
