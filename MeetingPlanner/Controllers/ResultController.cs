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
                        var userDates =
                            from user in container.UserMeetingDatesSet.Where(m => m.MeetingId == id).OrderBy(m => m.Date)
                            join userName in container.CachedUserNames on user.CachedUserNamesId equals userName.Id into
                                results
                            from result in results.DefaultIfEmpty()
                            select new {userDate = user, userName = result == null ? string.Empty : result.UserName}; 


                        foreach (var userDate in userDates)
                        {
                            string date = userDate.userDate.Date.ToShortDateString();
                            string userName = userDate.userName;
                            if (!model.Results.ContainsKey(date))
                                model.Results.Add(date, new ResultModel.DateCount());
                            if (userDate.userDate.IsAvaliable)
                            {
                                if (!string.IsNullOrEmpty(userName) && !model.Results[date].UserNamesConvenient.Contains(userDate.userName))
                                    model.Results[date].UserNamesConvenient.Add(userDate.userName);
                                model.Results[date].Convenient++;
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(userName) && !model.Results[date].UserNamesInconvenient.Contains(userDate.userName))
                                    model.Results[date].UserNamesInconvenient.Add(userDate.userName);
                                model.Results[date].Inconvenient++;
                            }
                        }

                        var max = int.MinValue;
                        string finalDate = string.Empty;
                        foreach (var pair in model.Results)
                        {
                            int value = pair.Value.Convenient - (pair.Value.Inconvenient*2);
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
