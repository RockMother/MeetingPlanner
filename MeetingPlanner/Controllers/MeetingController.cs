using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Permissions;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using System.Web.Util;
using DataAccess;
using MeetingPlanner.Enums;
using MeetingPlanner.Helpers;
using MeetingPlanner.Models;
using UserProfile = DataAccess.UserProfile;

namespace MeetingPlanner.Controllers
{
    public class MeetingController : BaseController
    {
        private const string MetingOwnerCookie = "MeetingOwner";
        private const string UserNameCookie = "UserName";
        //
        // GET: /Meeting/
        public ActionResult New(string description, DateTime? from, DateTime? to)
        {
            using (var container = new MeetingPlannerContainer())
            {
                var newMeeting = new Meeting()
                {
                    Description = description,
                    MeetingStatusId = 1,
                    From = from,
                    To = to,
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
                    var userNameCookie = Request.Cookies[UserNameCookie];
                    if (userNameCookie != null)
                        model.UserName = HttpUtility.UrlDecode(userNameCookie.Value);
                    var meeting = container.MeetingSet.FirstOrDefault(m => m.Id == id.Value);
                    if (meeting != null)
                    {
                        model.Description = meeting.Description;
                        model.MeetingId = meeting.Id;
                        model.From = meeting.From;
                        model.To = meeting.To;
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
                using (var container = new MeetingPlannerContainer())
                {
                    var meeting = container.MeetingSet.First(m => m.Id == meetingId);
                    meeting.RowVersion = Guid.NewGuid();

                    CachedUserName user = null;
                    if (!string.IsNullOrEmpty(userName))
                    {
                        user = ProcessUserName(userName, container);
                    }

                    var voteCookieName = GetVoteCookieName(meetingId);
                    var cookie = Request.Cookies[voteCookieName];
                    if (cookie != null)
                    {
                        DeleteSetBeforeDates(cookie, container);
                    }

                    cookie = new HttpCookie(voteCookieName) { Expires = DateTime.Now.AddDays(1) };
                    var entities = new List<UserMeetingDates>();
                    entities.AddRange(MakeEntityUserMeetingDay(greenDays, meetingId, container, true, user));
                    entities.AddRange(MakeEntityUserMeetingDay(redDays, meetingId, container, false, user));
                    
                    container.SaveChanges();

                    entities.ForEach(e => cookie[e.Id.ToString()] = null);

                    if (Response.Cookies[voteCookieName] != null)
                        Response.Cookies.Remove(voteCookieName);
                    Response.Cookies.Add(cookie);

                    
                }
            }

            return Json(SaveResult.Ok);
        }

        private void DeleteSetBeforeDates(HttpCookie cookie, MeetingPlannerContainer container)
        {
            var dayIds = ParseVoteCookieAndGetIds(cookie);
            var days = container.UserMeetingDatesSet.Where(d => dayIds.Contains(d.Id)).ToList();
            days.ForEach(d => container.UserMeetingDatesSet.Remove(d));
        }

        private CachedUserName ProcessUserName(string userName, MeetingPlannerContainer container)
        {
            var user = container.CachedUserNames.FirstOrDefault(u => u.UserName == userName);
            if (user == null)
            {
                user = new CachedUserName {UserName = userName};
                container.CachedUserNames.Add(user);
            }

            Response.Cookies.Add(new HttpCookie(UserNameCookie, HttpUtility.UrlEncode(userName)));
            return user;
        }

        private string GetVoteCookieName(int meetingId)
        {
            return "VoteMeeting_" + meetingId.ToString();
        }

        private List<int> ParseVoteCookieAndGetIds(HttpCookie cookie)
        {
            return (cookie.Values.Cast<object>().Select(value => Int32.Parse(value.ToString()))).ToList();
        }

        private List<UserMeetingDates> MakeEntityUserMeetingDay(DateTime[] days, int meetingId, MeetingPlannerContainer container, bool isAvaliable, CachedUserName user)
        {
            var result = new List<UserMeetingDates>();
            if (days != null)
            {
                foreach (var day in days)
                {
                    var entity = new UserMeetingDates
                        {
                            MeetingId = meetingId,
                            IsAvaliable = isAvaliable,
                            Date = DateTimeHelper.ConvertToUtc(day),
                            CachedUserName = user
                        };
                    container.UserMeetingDatesSet.Add(entity);
                    result.Add(entity);
                }
            }
            return result;
        }
    }
}
