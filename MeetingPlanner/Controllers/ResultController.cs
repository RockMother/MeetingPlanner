using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DataAccess;
using MeetingPlanner.Enums;

namespace MeetingPlanner.Controllers
{
    public class ResultController : Controller
    {
        public ActionResult Index(int? id)
        {
            var message = string.Empty;
            using (var container = new MeetingPlannerContainer())
            {
                var userProfile = container.UserProfile.FirstOrDefault(u => u.UserName == User.Identity.Name);
                if (userProfile != null)
                {
                    var meeting = container.MeetingSet.FirstOrDefault(m => m.Id == id);
                    if (meeting != null)
                    {
                        if (meeting.MeetingStatusId == (int) MeetingStatusEnum.Open)
                        {
                            message = "Пожалуйста, обновите страницу позже, результат появится, когда создатель встречи отправит запрос на расчет";
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
                            message = string.Format("Предлагаю встретится {0}, это устроит большинство.", finalDate.ToShortDateString());
                        }
                    }

                }
            }
            ViewBag.Message = message;
            return View();
        }

    }
}
