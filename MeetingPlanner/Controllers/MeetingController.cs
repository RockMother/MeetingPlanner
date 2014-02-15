using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Permissions;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using DataAccess;
using MeetingPlanner.Enums;
using MeetingPlanner.Models;
using UserProfile = DataAccess.UserProfile;

namespace MeetingPlanner.Controllers
{
    public class MeetingController : Controller
    {
        //
        // GET: /Meeting/
        public ActionResult New(string description)
        {
            using (var container = new MeetingPlannerContainer())
            {
                var newMeeting = new Meeting()
                {
                    Description = description,
                    MeetingStatusId = 1
                };
                container.MeetingSet.Add(newMeeting);
                container.SaveChanges();

                Response.Cookies["MeetingOwner"].Value = newMeeting.Id.ToString();
                Response.Cookies["MeetingOwner"].Expires = DateTime.Now.AddDays(1);
                return Json(newMeeting.Id);
            }
        }

        public ActionResult Edit(int? id)
        {
            var model = new MeetingModel();
            if (id.HasValue)
            {
                using (var container = new MeetingPlannerContainer())
                {
                    var meeting = container.MeetingSet.FirstOrDefault(m => m.Id == id.Value);
                    if (meeting != null)
                    {
                        model.Description = meeting.Description;
                        model.MeetingId = meeting.Id;
                        var cookie = Request.Cookies["MeetingOwner"];
                        if (cookie != null)
                        {
                            model.IsOwner = Int32.Parse(cookie.Value) == id;
                        }
                        return View(model);
                    }
                }
            }
            model.Description = "Встреча не найдена";
            return View(model);
        }

        [HttpPost]
        public ActionResult SendResults(SimpleDate[] greenDays, SimpleDate[] redDays, int meetingId)
        {
            using (var container = new MeetingPlannerContainer())
            {
                MakeEntityUserMeetingDay(greenDays, meetingId, container, true);
                MakeEntityUserMeetingDay(redDays, meetingId, container, false);
            }
            return Json(SaveResult.Ok);
        }

        public ActionResult CountResult(int meetingId)
        {
            using (var container = new MeetingPlannerContainer())
            {
                var meeting = container.MeetingSet.FirstOrDefault(m => m.Id == meetingId);
                if (meeting != null)
                {
                    meeting.MeetingStatusId = (int) MeetingStatusEnum.Closed;
                    container.SaveChanges();
                    return Json(SaveResult.Ok);
                }
            }
            return Json(SaveResult.None);
        }

        private static void MakeEntityUserMeetingDay(SimpleDate[] days, int meetingId, MeetingPlannerContainer container, bool isAvaliable)
        {
            if (days != null)
            {
                foreach (var day in days)
                {
                    container.UserMeetingDatesSet.Add(new UserMeetingDates
                    {
                        MeetingId = meetingId,
                        IsAvaliable = isAvaliable,
                        Date = day.ToDateTime()
                    });
                    container.SaveChanges();
                }
            }
        }
    }
}
