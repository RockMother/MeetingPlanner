using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using DataAccess;
using MeetingPlanner.Enums;
using MeetingPlanner.Helpers;
using MeetingPlanner.Models;
using MeetingPlanner.Resources.Controllers.Result;

namespace MeetingPlanner.Controllers
{
    public class ResultController : BaseController
    {
        public ActionResult GetResults(int? id, string currentRowVersion)
        {
            var model = new ResultModel();
            using (var container = new MeetingPlannerContainer())
            {
                var meeting = container.MeetingSet.FirstOrDefault(m => m.Id == id);
                if (meeting != null)
                {
                    model.RowVersion = meeting.RowVersion.ToString();
                    if (currentRowVersion != model.RowVersion)
                    {
                        model.Results = new List<ResultModel.DateCount>();
                        var userDates =
                            from user in container.UserMeetingDatesSet.Where(m => m.MeetingId == id)
                            join userName in container.CachedUserNames on user.CachedUserNameId equals userName.Id into
                                results
                            from result in results.DefaultIfEmpty()
                            select new {userDate = user, userName = result == null ? string.Empty : result.UserName};
                        userDates = userDates.OrderBy(key => key.userDate.Date);

                        foreach (var userDate in userDates)
                        {
                            string date = DateTimeHelper.ConvertToUtc(userDate.userDate.Date).ToShortDateString();
                            string userName = userDate.userName;
                            if (!model.HasKey(date))
                                model.Results.Add(new ResultModel.DateCount(date));
                            if (userDate.userDate.IsAvaliable)
                            {
                                if (!string.IsNullOrEmpty(userName) &&
                                    !model.GetDateCountByKey(date).UserNamesConvenient.Contains(userDate.userName))
                                    model.GetDateCountByKey(date).UserNamesConvenient.Add(userDate.userName);
                                model.GetDateCountByKey(date).Convenient++;
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(userName) &&
                                    !model.GetDateCountByKey(date).UserNamesInconvenient.Contains(userDate.userName))
                                    model.GetDateCountByKey(date).UserNamesInconvenient.Add(userDate.userName);
                                model.GetDateCountByKey(date).Inconvenient++;
                            }
                        }

                        var max = int.MinValue;
                        string finalDate = string.Empty;
                        foreach (var result in model.Results)
                        {
                            int value = result.Convenient - (result.Inconvenient*2);
                            if (value > max)
                            {
                                max = value;
                                finalDate = result.Key;
                            }
                        }
                        var maxConv = model.Results.Max(p => p.Convenient);
                        var maxInconv = model.Results.Max(p => p.Inconvenient);
                        model.MaxValue = Math.Max(maxConv, maxInconv);
                        model.Message = string.Format(Strings.CountResult, finalDate);

                    }
                }
            }
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Index(int? id)
        {
            Meeting meeting = null;
            using (var container = new MeetingPlannerContainer())
            {
                meeting = container.MeetingSet.First(m => m.Id == id);
            }
            return View(new ResultIndexModel { MeetingId = meeting.Id});
        }

    }
}
