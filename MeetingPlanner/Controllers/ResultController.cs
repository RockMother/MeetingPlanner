using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using DataAccess;
using MeetingPlanner.Enums;
using MeetingPlanner.Models;
using MeetingPlanner.Resources.Controllers.Result;

namespace MeetingPlanner.Controllers
{
    public class ResultController : BaseController
    {
        public ActionResult Index(int? id)
        {
            var model = new ResultModel();
            using (var container = new MeetingPlannerContainer())
            {
                var meeting = container.MeetingSet.FirstOrDefault(m => m.Id == id);
                if (meeting != null)
                {
                    if (meeting.MeetingStatusId == (int) MeetingStatusEnum.Open)
                    {
                        model.Message = Strings.PleaseReloadLater;
                    }
                    else if (meeting.MeetingStatusId == (int) MeetingStatusEnum.Closed)
                    {
                        model.Results = new Dictionary<string, ResultModel.DateCount>();
                        var userDates = container.UserMeetingDatesSet.Where(m => m.MeetingId == id).OrderBy(m => m.Date);
                        foreach (var userDate in userDates)
                        {
                            string date = userDate.Date.ToShortDateString();
                            if (!model.Results.ContainsKey(date))
                                model.Results.Add(date, new ResultModel.DateCount());
                            if (userDate.IsAvaliable)
                                model.Results[date].Convenient++;
                            else
                            {
                                model.Results[date].Inconvenient--;
                            }
                        }

                        var max = int.MinValue;
                        string finalDate = string.Empty;
                        foreach (var pair in model.Results)
                        {
                            int value = pair.Value.Convenient + (pair.Value.Inconvenient*2);
                            if (value > max)
                            {
                                max = value;
                                finalDate = pair.Key;
                            }
                        }
                        model.Message = string.Format(Strings.CountResult, finalDate);
                    }
                }
            }
            return View(model);
        }

    }
}
