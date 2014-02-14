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
        [HttpPost]
        public ActionResult New(string description)
        {
            using (var container = new MeetingPlannerContainer())
            {
                var userProfile = container.UserProfile.FirstOrDefault(u => u.UserName == User.Identity.Name);
                var newMeeting = new Meeting()
                {
                    UserProfile = userProfile,
                    Description = description,
                    MeetingStatusId = 1
                };
                container.MeetingSet.Add(newMeeting);
                container.SaveChanges();
                return Json(newMeeting.Id);
            }
        }

        public ActionResult Edit(int? id)
        {
            if (id.HasValue)
            {
                using (var container = new MeetingPlannerContainer())
                {
                    var meeting = container.MeetingSet.FirstOrDefault(m => m.Id == id.Value);
                    if (meeting != null)
                    {
                        var model = new MeetingModel
                        {
                            Description = meeting.Description,
                            MeetingLink = @Url.Action("Edit", new {id = meeting.Id}),
                            OwnerName = meeting.UserProfile.UserName,
                            MeetingId = meeting.Id
                        };
                        return View(model);
                    }
                }
            }
            return View();
        }

        [HttpPost]
        public ActionResult SendResults(SimpleDate[] greenDays, SimpleDate[] redDays, int meetingId)
        {
            using (var container = new MeetingPlannerContainer())
            {
                var userProfile = container.UserProfile.FirstOrDefault(u => u.UserName == User.Identity.Name);
                if (userProfile != null)
                {
                    MakeEntityUserMeetingDay(greenDays, meetingId, container, userProfile, true);
                    MakeEntityUserMeetingDay(redDays, meetingId, container, userProfile, false);
                }
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

        private static void MakeEntityUserMeetingDay(SimpleDate[] days, int meetingId,
            MeetingPlannerContainer container,
            UserProfile userProfile, bool isAvaliable)
        {
            if (days != null)
            {
                foreach (var day in days)
                {
                    container.UserMeetingDatesSet.Add(new UserMeetingDates
                    {
                        MeetingId = meetingId,
                        UserProfileId = userProfile.Id,
                        IsAvaliable = isAvaliable,
                        Date = day.ToDateTime()
                    });
                    container.SaveChanges();
                }
            }
        }
    }
}
