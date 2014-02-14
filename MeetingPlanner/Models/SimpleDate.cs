using System;

namespace MeetingPlanner.Models
{
    public class SimpleDate
    {
        public int Month { get; set; }
        public int Day { get; set; }
        public int Year { get; set; }

        public DateTime ToDateTime()
        {
            return new DateTime(Year, Month, Day);
        }
    }
}