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
    public class MeetingController : BaseController
    {
        private const string MetingOwnerCookie = "MeetingOwner";
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

                Response.Cookies[MetingOwnerCookie].Value = newMeeting.Id.ToString();
                Response.Cookies[MetingOwnerCookie].Expires = DateTime.Now.AddDays(1);
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
                        var ownerCookie = Request.Cookies[MetingOwnerCookie];
                        if (ownerCookie != null)
                        {
                            model.IsOwner = Int32.Parse(ownerCookie.Value) == id;
                        }

                        var voteCookie = Request.Cookies[GetVoteCookieName(meeting.Id)];
                        if (voteCookie != null)
                        {
                            var dayIds = ParseVoteCookieAndGetIds(voteCookie);
                            model.MarkedDays = new Dictionary<DateTime, bool>();
                            container.UserMeetingDatesSet.Where(m => dayIds.Contains(m.Id))
                                     .ToList()
                                     .ForEach(d => model.MarkedDays.Add(d.Date, d.IsAvaliable));
                        }
                        return View(model);
                    }
                }
            }
            model.Description = "Встреча не найдена";
            return View(model);
        }

        [HttpPost]
        public ActionResult SendResults(DateTime[] greenDays, DateTime[] redDays, int meetingId, string userName)
        {
            if (greenDays != null || redDays != null)
            {
                int? userId = null;
                using (var container = new MeetingPlannerContainer())
                {
                    if (!string.IsNullOrEmpty(userName))
                    {
                        var user = container.CachedUserNames.FirstOrDefault(u => u.UserName == userName);
                        if (user == null)
                        {
                            user = new CachedUserName {UserName = userName};
                            container.CachedUserNames.Add(user);
                            container.SaveChanges();
                        }
                        userId = user.Id;
                    }

                    var voteCookieName = GetVoteCookieName(meetingId);
                    var cookie = Request.Cookies[voteCookieName];
                    if (cookie != null)
                    {
                        var dayIds = ParseVoteCookieAndGetIds(cookie);
                        var days = container.UserMeetingDatesSet.Where(d => dayIds.Contains(d.Id)).ToList();
                        days.ForEach(d => container.UserMeetingDatesSet.Remove(d));
                        container.SaveChanges();
                    }
                    cookie = new HttpCookie(voteCookieName) { Expires = DateTime.Now.AddDays(1) };

                    MakeEntityUserMeetingDay(greenDays, meetingId, container, true,userId, cookie);
                    MakeEntityUserMeetingDay(redDays, meetingId, container, false, userId, cookie);

                    if (Response.Cookies[voteCookieName] != null)
                        Response.Cookies.Remove(voteCookieName);
                    Response.Cookies.Add(cookie);
                }
            }

            return Json(SaveResult.Ok);
        }

        private string GetVoteCookieName(int meetingId)
        {
            return "VoteMeeting_" + meetingId.ToString();
        }

        private List<int> ParseVoteCookieAndGetIds(HttpCookie cookie)
        {
            return (cookie.Values.Cast<object>().Select(value => Int32.Parse(value.ToString()))).ToList();
        }

        private static void MakeEntityUserMeetingDay(DateTime[] days, int meetingId, MeetingPlannerContainer container, bool isAvaliable, int? userId, HttpCookie cookie)
        {
            if (days != null)
            {
                foreach (var day in days)
                {
                    var entity = new UserMeetingDates
                        {
                            MeetingId = meetingId,
                            IsAvaliable = isAvaliable,
                            Date = new DateTime(day.Ticks, DateTimeKind.Utc),
                            CachedUserNamesId = userId
                        };
                    container.UserMeetingDatesSet.Add(entity);
                    container.SaveChanges();
                    cookie[entity.Id.ToString()] = null;
                }
            }
        }
    }
}
