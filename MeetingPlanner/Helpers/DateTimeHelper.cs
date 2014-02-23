using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MeetingPlanner.Helpers
{
    public class DateTimeHelper
    {
        public static DateTime ConvertToUtc(DateTime dateTime)
        {
            return new DateTime(dateTime.Ticks, DateTimeKind.Utc);
        }
    }
}