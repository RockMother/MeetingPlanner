using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MeetingPlanner.Models
{
    public class ResultModel
    {
        public List<DateCount> Results { get; set; }
        public int MaxValue { get; set; }
        public string RowVersion { get; set; }
        public string Message { get; set; }

        public class DateCount
        {
            public DateCount(string key)
            {
                Key = key;
                UserNamesConvenient = new List<string>();
                UserNamesInconvenient = new List<string>();
            }

            public string Key { get; set; }
            public List<string> UserNamesConvenient { get; set; }
            public List<string> UserNamesInconvenient { get; set; }
            public int Convenient { get; set; }
            public int Inconvenient { get; set; }
        }

        public bool HasKey(string key)
        {
            return GetDateCountByKey(key) != null;
        }

        public DateCount GetDateCountByKey(string key)
        {
            return Results.Find(r => r.Key == key);
        }
    }
}