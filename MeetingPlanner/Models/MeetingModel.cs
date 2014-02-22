using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MeetingPlanner.Models
{
    public class MeetingModel
    {
        public string Description { get; set; }
        public bool IsOwner { get; set; }
        public int MeetingId { get; set; }
        public Dictionary<DateTime, bool> MarkedDays { get; set; }
        public string UserName { get; set; }
    }
}