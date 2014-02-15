using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DataAccess;
using MeetingPlanner.Enums;
using MeetingPlanner.Resources.Controllers.Result;

namespace MeetingPlanner.Controllers
{
    public class ResultController : BaseController
    {
        public ActionResult Index(int? id)
        {
            var message = string.Empty;
            using (var container = new MeetingPlannerContainer())
            {
                var meeting = container.MeetingSet.FirstOrDefault(m => m.Id == id);
                if (meeting != null)
                {
                    if (meeting.MeetingStatusId == (int) MeetingStatusEnum.Open)
                    {
                        message = Strings.PleaseReloadLater;
                    }
                    else if (meeting.MeetingStatusId == (int) MeetingStatusEnum.Closed)
                    {
                        var result = new Dictionary<DateTime, int>();
                        var userDates = container.UserMeetingDatesSet.Where(m => m.MeetingId == id);
                        foreach (var userDate in userDates)
                        {
                            if (!result.ContainsKey(userDate.Date))
                                result.Add(userDate.Date, 0);
                            if (userDate.IsAvaliable)
                                result[userDate.Date]++;
                            else
                            {
                                result[userDate.Date] = result[userDate.Date] - 2;
                            }
                        }
                        var max = int.MinValue;
                        DateTime finalDate = DateTime.MaxValue;
                        foreach (var pair in result)
                        {
                            if (pair.Value > max)
                            {
                                max = pair.Value;
                                finalDate = pair.Key;
                            }
                        }
                        message = string.Format(Strings.CountResult, finalDate.ToShortDateString());
                    }
                }
            }
            ViewBag.Message = message;
            return View();
        }

    }
}
