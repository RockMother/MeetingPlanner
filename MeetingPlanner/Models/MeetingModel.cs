﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MeetingPlanner.Models
{
    public class MeetingModel
    {
        public string Description { get; set; }
        public string MeetingLink { get; set; }
        public string OwnerName { get; set; }
        public int MeetingId { get; set; }
    }
}