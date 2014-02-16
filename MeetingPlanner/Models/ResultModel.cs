using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MeetingPlanner.Models
{
    public class ResultModel
    {
        public Dictionary<string, DateCount> Results { get; set; }
        public string Message { get; set; }
        

        public class DateCount
        {
            public DateCount()
            {
                UserNamesConvenient = new List<string>();
                UserNamesInconvenient = new List<string>();
            }

            public List<string> UserNamesConvenient { get; set; }
            public List<string> UserNamesInconvenient { get; set; }
            public int Convenient { get; set; }
            public int Inconvenient { get; set; }
        }
    }
}