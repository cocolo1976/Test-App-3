using System;
using System.Data.Entity;
using System.Net;
using System.Web.Mvc;
using System.Collections.Generic;
using FairfaxCounty.JCAS_Public_MVC.Models;
using System.Linq;
using System.Data.Entity.SqlServer;
using System.Data;
using System.Text;
using System.Globalization;
using FairfaxCounty.JCAS_Public_MVC.ViewModels;

namespace FairfaxCounty.JCAS_Public_MVC.Controllers
{
    /// <summary>Allows DataEntry and Attorney users to add/edit/delete the JcasSession items.</summary>
    [Authorize(Roles = Constants.ExternalRoleAttorney + "," + Constants.InternalRoleReadOnly)]
    public class JcasSessionsController : Controller
    {
        private JcasEntities db = new JcasEntities();

        /// <summary>Gets the end date to display calendar/sessions based on user role.</summary>
        public DateTime GetEndDate()
        {
            bool roleAttorney = User.IsInRole(Constants.ExternalRoleAttorney);
            string settingName = roleAttorney ? "AttorneySessionMonths" : "InternalSessionMonths";
            if (!int.TryParse(Helpers.Utility.GetSettingValue(settingName), out int outmonths))
            {
                outmonths = 4;
            }
            DateTime dateReturn = DateTime.Today;
            if (DateTime.TryParse(dateReturn.AddMonths(outmonths - 1).ToShortDateString(), out DateTime outDate))
            {
                dateReturn = outDate.AddMonths(1).AddDays(0 - outDate.Day);
            }
            return dateReturn;
        }

        /// <summary>Gets list of calendar dates between tday and end date allowable per user role.</summary>
        public List<DateTime> GetCalendarDates()
        {
            DateTime startDate = DateTime.Today;
            DateTime endDate = GetEndDate();
            List<DateTime> dates = Enumerable.Range(0, (endDate - startDate).Days + 1)
                            .Select(day => startDate.AddDays(day))
                            .Where(day => (!day.DayOfWeek.Equals(DayOfWeek.Sunday)
                                && !day.DayOfWeek.Equals(DayOfWeek.Saturday))).ToList();
            return dates;
        }

        /// <summary>Get the list of month and year to display in the dropdown list on the calendar view.</summary>
        public IEnumerable<SelectListItem> GetMonthYears()
        {
            bool roleAttorney = User.IsInRole(Constants.ExternalRoleAttorney);
            string settingName = roleAttorney ? "AttorneySessionMonths" : "InternalSessionMonths";
            if (!int.TryParse(Helpers.Utility.GetSettingValue(settingName), out int outmonths))
            {
                outmonths = 4;
            }
            List<string> dates = Enumerable.Range(0, (outmonths))
                            .Select(monthYear => DateTime.Today.Date.AddMonths(monthYear).Month.ToString().PadLeft(2, '0')
                            + "/" + DateTime.Today.Date.AddMonths(monthYear).Year.ToString()).ToList();
            IEnumerable<SelectListItem> monthYearList = from c in dates
                                                        select new SelectListItem { Text = c.ToString(), Value = c.ToString() };
            return monthYearList;
        }

        /// <summary>Displays sessions for the selected month in a calendar view to allow schedule and unschedule sessions.</summary>
        /// <param name="selectedMonth">Identify the month and year to display the calendar.</param>
        /// <param name="view">Identify the view to display the calendar in.</param>
        /// <returns>The calendar for the selected month and year in html format.</returns>
        public ActionResult Calendar(string selectedMonth, string view = "Schedule")
        {
            //if Attorney user or not DataEntry user (Readonly user), set the view to Schedule
            if (User.IsInRole(Constants.ExternalRoleAttorney) || !User.IsInRole(Constants.InternalRoleDataEntry))
            {
                view = "Schedule";
            }
            ViewBag.View = view;

            DateTime endDate = GetEndDate();
            ViewBag.ReportToDate = endDate;
            ViewBag.MonthYears = GetMonthYears();
            selectedMonth = (string.IsNullOrEmpty(selectedMonth) ? DateTime.Today.Month.ToString().PadLeft(2, '0') + "/" + DateTime.Today.Year.ToString() : selectedMonth);
            ViewBag.SelectedMonth = selectedMonth;
            ViewBag.SelectedFrom = Convert.ToDateTime(selectedMonth).ToShortDateString();
            ViewBag.SelectedTo = Convert.ToDateTime(selectedMonth).AddMonths(1).AddDays(-1).ToShortDateString();
            //for downloading the calendar:
            //if attorney user, set the report ids to AttorneyCalendar and AttorneySchedule
            if (User.IsInRole(Constants.ExternalRoleAttorney))
            {
                ViewBag.CalendarReportId = db.JcasReports.Where(r => r.ReportName == "AttorneyCalendar").Select(r => r.Id).FirstOrDefault();
                ViewBag.ScheduleReportId = db.JcasReports.Where(r => r.ReportName == "AttorneySchedule").Select(r => r.Id).FirstOrDefault();
            }
            //else, set it to CourtCalendar
            else
            {
                ViewBag.CalendarReportId = db.JcasReports.Where(r => r.ReportName == "CourtCalendar").Select(r => r.Id).FirstOrDefault();
            }

            //if attorney user, get attorney id
            int attorneyId = 0;
            if (User.IsInRole(Constants.ExternalRoleAttorney))
            {
                attorneyId = (db.JcasAttorneys.Where(a => a.EmailAddress == User.Identity.Name.ToString()).Select(a => a.Id).FirstOrDefault());
            }

            //setup the calendar table
            int calendarWeek = 1;
            int calendarWeekday = 2;
            int calendarDay = 1;
            bool newDay = true;
            string tdStyle = "style=\"width: 20%;border-width:1pt;border-color:lightgray;border-style:solid\"";
            StringBuilder sbTable = new StringBuilder();
            sbTable.Append("<table class=\"table\" style=\"width:100%;border-width:1pt;border-color:lightgray;border-style:solid\">");
            //setup header row
            sbTable.AppendLine("<tr>");
            for (int i = 1; i < 6; i++)
            {
                sbTable.AppendLine(string.Format("<td {0}>", tdStyle));
                sbTable.Append(string.Format("<strong>{0}</strong></td>", Enum.Parse(typeof(DayOfWeek), i.ToString())));
            }
            sbTable.AppendLine("</tr>");
            sbTable.AppendLine("<tr>");
            sbTable.AppendLine(string.Format("<td {0}>", tdStyle));

            int monthsDisplay = User.IsInRole(Constants.ExternalRoleAttorney) ?
                Convert.ToInt32(Helpers.Utility.GetSettingValue("AttorneySessionMonths")) :
                Convert.ToInt32(Helpers.Utility.GetSettingValue("InternalSessionMonths"));
            DateTime dateToCompare = DateTime.Parse(DateTime.Today.AddMonths(monthsDisplay).ToString("MM/01/yyyy"));
            bool displayOnly = false;
            if (DateTime.Parse(selectedMonth) >= dateToCompare ||
                (User.IsInRole(Constants.ExternalRoleAttorney) && (Convert.ToBoolean(Helpers.Utility.GetSettingValue("AllowAttorneySchedule")) == false)))
            {
                displayOnly = true;
            }
            //loop through all rows for the selected months
            var resultCal = db.pJcasSessionGetCalendar(attorneyId, sessionMonth: Convert.ToDateTime(selectedMonth));
            //Added by DANG 03/19/2024----------
            var hearingType = "";
            //----------------------------------

            foreach (var r in resultCal.ToList())
            {
                //setup the table header row 
                if (r.CalendarWeek != (calendarWeek))
                {
                    calendarWeek++;
                    sbTable.AppendLine("</td>");
                    sbTable.AppendLine("</tr>");
                    sbTable.AppendLine("<tr>");
                    sbTable.AppendLine(string.Format("<td {0}>", tdStyle));
                }
                if (r.CalendarWeekday != calendarWeekday)
                {
                    calendarWeekday = (calendarWeekday > 5 || calendarWeekday < 2 ? 2 : calendarWeekday);
                    for (int i = calendarWeekday; r.CalendarWeekday > calendarWeekday;)
                    {
                        sbTable.AppendLine("</td>");
                        sbTable.AppendLine(string.Format("<td {0}>", tdStyle));
                        calendarWeekday++;
                    }
                    newDay = true;
                }
                //display the date if this is a new day
                if (newDay == true)
                {
                    //Added by DANG
                    hearingType = "";

                    //display the calendar day
                    calendarDay = r.CalendarDay.Value;
                    sbTable.Append("<strong class=\"highlight1\">" + r.CalendarDay.ToString() + "</strong>");
                    //if it is a court closed date, display reason
                    if (!string.IsNullOrEmpty(r.CourtClosed))
                    {
                        sbTable.AppendLine("<br/><strong>" + r.CourtClosed + "</strong>");
                    }
                    //else, not a court closed date, display the link to edit courtrooms
                    else
                    {
                        if (User.IsInRole(Constants.InternalRoleDataEntry) && displayOnly == false
                            && db.JcasSessions.Where(s => s.SessionDate == r.CalendarDate).Count() > 0)
                        {
                            sbTable.Append(string.Format("<a href=\"{0}\" class=\"pull-right\">Courtrooms</a>",
                            Url.Action("Courtrooms", "JcasSessions", new { selectedDate = r.CalendarDate.Value.ToShortDateString() })));
                        }
                    }
                    //get the court date notes for this date
                    StringBuilder sbNotes = new StringBuilder();
                    List<string> courtDateNotes = db.JcasCourtDateNotes.Where(a => a.FromDate <= r.CalendarDate && a.ToDate >= r.CalendarDate)
                        .OrderBy(a => a.DisplayOrder).Select(a => a.Note).ToList();
                    if (courtDateNotes != null)
                    {
                        foreach (string itm in courtDateNotes)
                        {
                            sbNotes.Append("<br />");
                            sbNotes.Append(itm);
                        }
                    }
                    //if court date notes exists, display them
                    if (sbNotes != null)
                        sbTable.Append("<strong>" + sbNotes.ToString() + "</strong>");

                    newDay = false;
                }
                if (r.SessionId != null)
                {
                    sbTable.Append("<br/>");
                    //if view is Schedule
                    if (view.Equals("Schedule"))
                    {
                        //if the selected month does not allow scheduling, display only
                        if (displayOnly == true)
                        {
                            sbTable.Append(r.HearingType.ToString());
                            if (User.IsInRole(Constants.InternalRoleReadOnly))
                            {
                                sbTable.Append(" - " + r.AttorneyName.ToString() + r.Courtroom.ToString());
                            }
                            if (User.IsInRole(Constants.ExternalRoleAttorney) && r.AttorneyId.HasValue && r.AttorneyId.Value == attorneyId)
                            {
                                sbTable.Append(" Confirmed");
                                sbTable.Append(r.Courtroom.ToString());
                            }
                        }
                        else
                        {
                            //if readonly user, display the session info and not allow editing
                            if (User.IsInRole(Constants.InternalRoleReadOnly) && !User.IsInRole(Constants.InternalRoleDataEntry))
                            {
                                sbTable.Append(r.HearingType.ToString() + " - " + r.AttorneyName.ToString() + r.Courtroom.ToString());
                            }
                            //else, the user has either Attorney or DataEntry role, allow schedule/unschedule based on role
                            else
                            {
                                if (r.AttorneyId.HasValue)
                                {
                                    if (User.IsInRole(Constants.InternalRoleDataEntry))
                                    {
                                        //sbTable.Append(string.Format("<a href=\"{0}\" class=\"unschedule\">{1}</a> - {2}{3}",
                                        //    Url.Action("Unschedule", "JcasSessions", new { id = r.SessionId.ToString() }),
                                        //    r.HearingType.ToString(),
                                        //    r.AttorneyName.ToString(),
                                        //    r.Courtroom.ToString()));

                                        //Added by DANG 03/18/2024
                                        if (String.IsNullOrEmpty(hearingType))
                                        {
                                            hearingType = r.HearingType.ToString();

                                            sbTable.Append(string.Format("<a href=\"{0}\" class=\"schedule\">{1}</a><br/>{2}",
                                              Url.Action("Schedule", "JcasSessions", new { id = r.SessionId.ToString() }),
                                              r.HearingType.ToString(),
                                               ""));
                                        }
                                        else
                                        {
                                            if (hearingType != r.HearingType.ToString())
                                            {
                                                sbTable.Append(string.Format("<a href=\"{0}\" class=\"schedule\">{1}</a><br/>{2}",
                                              Url.Action("Schedule", "JcasSessions", new { id = r.SessionId.ToString() }),
                                              r.HearingType.ToString(),
                                               ""));
                                            }
                                        }

                                        sbTable.Append(string.Format("<a href=\"{0}\" class=\"unschedule\">&nbsp;&nbsp;  - {1} - {2}</a>",
                                            Url.Action("Unschedule", "JcasSessions", new { id = r.SessionId.ToString(), attorneyId = r.AttorneyId }),
                                            r.AttorneyName.ToString(),
                                            r.Courtroom.ToString()));

                                    }
                                    else
                                    {
                                        sbTable.Append("<strong class=\"text-danger\">" + r.HearingType.ToString() + "</strong>");
                                        if (User.IsInRole(Constants.ExternalRoleAttorney) && r.AttorneyId.Value == attorneyId)
                                        {
                                            sbTable.Append(" Confirmed");
                                            sbTable.Append(r.Courtroom.ToString());
                                        }
                                    }
                                }
                                else
                                {
                                    if (String.IsNullOrEmpty(hearingType))
                                    {
                                        hearingType = r.HearingType.ToString();
                                    }
                                    sbTable.Append(string.Format("<a href=\"{0}\" class=\"schedule\">{1}</a>{2}",
                                           Url.Action("Schedule", "JcasSessions", new { id = r.SessionId.ToString() }),
                                           r.HearingType.ToString(),
                                            r.Courtroom.ToString()));
                                }
                            }
                        }
                    }
                    //else if DataEntry user and view is Delete, display the link for the selected action
                    else
                    {
                        if (User.IsInRole(Constants.InternalRoleDataEntry) && view.Equals("Delete"))
                        {
                            sbTable.Append(string.Format("<a href=\"{0}\" class=\"schedule\">{1}</a>",
                                               Url.Action("Delete", "JcasSessions", new { id = r.SessionId.ToString() }),
                                               view.ToString()));
                            sbTable.Append(" " + r.HearingType.ToString() + " - " + r.AttorneyName.ToString() + r.Courtroom.ToString());
                        }
                    }
                }
            }

            sbTable.AppendLine("</td>");
            sbTable.AppendLine("</tr>");
            sbTable.AppendLine("</table>");

            ViewBag.CalendarHtml = MvcHtmlString.Create("<html>" + sbTable.ToString() + "</html>");
            return View();
        }

        // GET: JcasSessions/Schedule
        /// <summary>Displays a form for scheduling a Session.</summary>
        /// <param name="id">Identify the session to schedule.</param>
        /// <returns>Form for scheduling a Session.</returns>
        [HttpGet]
        [Authorize(Roles = Constants.InternalRoleDataEntry + "," + Constants.ExternalRoleAttorney)]
        public ActionResult Schedule(int? id)
        {
            //if session id is null, return BadRequest
            if (id == null || !id.HasValue)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //if session id is provided
            else
            {
                //get session record from the database.  

                //Commented by DANG 03/18/2024-------------------------
                //JcasSession jcasSession = db.JcasSessions.Include(s => s.JcasAttorney)
                //    .Include(s => s.JcasCourtroom)
                //    .Include(s => s.JcasHearingType)
                //    .Where(s => s.Id == id && s.AttorneyId == null).FirstOrDefault();
                //End Commented by DANG 03/18/2024-------------------------

                JcasSession jcasSession = db.JcasSessions.Include(s => s.JcasAttorney)
                    .Include(s => s.JcasCourtroom)
                    .Include(s => s.JcasHearingType)
                    .Where(s => s.Id == id && s.CourtroomId != null).FirstOrDefault();

                //if session not found, display error
                if (jcasSession == null)
                {
                    //return HttpNotFound(); Commented by DANG 04/04/2024

                    //Added by DANG 04/04/2024----------------------------------------
                    //Check if in SessionRequest table already has a first session with courtRoom then we will disable courtRoom drop down. Only the first attorney can pick a courtroom
                    var queryJcasSessionRequest = (from jS in db.JcasSessions
                                                   join jSR in db.JcasSessionRequests on jS.Id equals jSR.SessionId
                                                   join jA in db.JcasAttorneys on jSR.AttorneyId equals jA.Id
                                                   join jC in db.JcasCourtrooms on jSR.CourtroomId equals jC.Id into cr
                                                   from x in cr.DefaultIfEmpty()
                                                   join jHR in db.JcasHearingTypes on jS.HearingTypeId equals jHR.Id
                                                   where jSR.SessionId == id
                                                   select new SessionContainerModel
                                                   {
                                                       Id = jS.Id,
                                                       SessionDate = jS.SessionDate,
                                                       AttorneyId = jA.Id,
                                                       AttorneyName = jA.FullName,
                                                       CourtroomId = x.Id,
                                                       Courtroom = x.Courtroom,
                                                       HearingTypeId = jHR.Id,
                                                       HearingTypeName = jHR.HearingTypeName,
                                                       UserLoginIdUpdate = jSR.UserLoginIdUpdate,
                                                       RowVersionId = jSR.RowVersionId,
                                                       SessionRequestId = jSR.Id
                                                   }).FirstOrDefault();

                    //if SessionRequest table is also empty That is meant there is no session yet. Then we will look for Session Table to display info
                    if (queryJcasSessionRequest == null)
                    {
                        jcasSession = db.JcasSessions.Include(s => s.JcasAttorney)
                           .Include(s => s.JcasCourtroom)
                           .Include(s => s.JcasHearingType)
                           .Where(s => s.Id == id).FirstOrDefault();

                        if (jcasSession == null)
                        {
                            return HttpNotFound();
                        }
                    }
                    //if SessionRequest table has data That is meant there is an attorney already booked a session. Then we will load the information and disable the courtroom dropdown
                    else
                    {
                        jcasSession = new JcasSession();
                        jcasSession.Id = queryJcasSessionRequest.Id;
                        jcasSession.SessionDate = queryJcasSessionRequest.SessionDate;
                        jcasSession.AttorneyId = queryJcasSessionRequest.AttorneyId;
                        jcasSession.CourtroomId = queryJcasSessionRequest.CourtroomId;
                        jcasSession.HearingTypeId = queryJcasSessionRequest.HearingTypeId;
                        jcasSession.JcasHearingType = db.JcasHearingTypes.FirstOrDefault(q => q.Id == jcasSession.HearingTypeId);

                    }
                    //End add by DANG --------------------------------------------------
                }

                ViewBag.SelectedMonth = jcasSession.SessionDate.ToString("MM/yyyy");
                //if this is an attorney user, default the attorney id to the current id
                if (User.IsInRole(Constants.ExternalRoleAttorney))
                {
                    jcasSession.AttorneyId = db.JcasAttorneys.Where(a => a.EmailAddress == User.Identity.Name.ToString()).Select(a => a.Id).FirstOrDefault();
                    //if the attorney is found, display the session record
                    if (jcasSession.AttorneyId != null)
                    {
                        jcasSession.JcasAttorney = db.JcasAttorneys.Find(jcasSession.AttorneyId);
                        return View(jcasSession);
                    }
                    //if the current attorney is not found, display error
                    else
                    {
                        return HttpNotFound();
                    }
                }
                //if not attorney user, refresh the attorney dropdown list and display the session record
                else
                {
                    //Added by DANG
                    //Get All Attorneys in SessionRequest
                    var items = db.JcasSessionRequests.Where(q => q.SessionId == id).Select(q => q.AttorneyId).ToList();

                    IEnumerable<JcasAttorney> attorneys = db.JcasAttorneys.Where(a => a.IsActive == true);
                    //only list attorneys who are certified for the provided hearing type and not already scheduled on the session date
                    attorneys = attorneys.Where(a => !(a.JcasSessions.Where(s => s.SessionDate == jcasSession.SessionDate && s.AttorneyId == a.Id).Any())
                      && !(items.Contains(a.Id))//Added by DANG
                      && string.Join(",", a.JcasAttorneyHearingTypes.Select(h => h.HearingTypeId.ToString()).ToList()).Contains(jcasSession.HearingTypeId.ToString()));
                    ViewBag.AttorneyList = new SelectList(attorneys ?? db.JcasAttorneys.Where(a => a.Id == 0), "Id", "FullName");
                    ViewBag.CourtroomList = new SelectList(db.JcasCourtrooms, "Id", "Courtroom");
                    return View(jcasSession);
                }

                //Commented by DANG 04/04/2024
                //if (jcasSession == null)
                //{
                //return HttpNotFound();
                //}
                ////if the specified session found
                //else
                //{
                //    ViewBag.SelectedMonth = jcasSession.SessionDate.ToString("MM/yyyy");
                //    //if this is an attorney user, default the attorney id to the current id
                //    if (User.IsInRole(Constants.ExternalRoleAttorney))
                //    {
                //        jcasSession.AttorneyId = db.JcasAttorneys.Where(a => a.EmailAddress == User.Identity.Name.ToString()).Select(a => a.Id).FirstOrDefault();
                //        //if the attorney is found, display the session record
                //        if (jcasSession.AttorneyId != null)
                //        {
                //            jcasSession.JcasAttorney = db.JcasAttorneys.Find(jcasSession.AttorneyId);
                //            return View(jcasSession);
                //        }
                //        //if the current attorney is not found, display error
                //        else
                //        {
                //            return HttpNotFound();
                //        }
                //    }
                //    //if not attorney user, refresh the attorney dropdown list and display the session record
                //    else
                //    {
                //        //Added by DANG
                //        //Get All Attorneys in SessionRequest
                //        var items = db.JcasSessionRequests.Where(q => q.SessionId == id).Select(q => q.AttorneyId).ToList();

                //        IEnumerable<JcasAttorney> attorneys = db.JcasAttorneys.Where(a => a.IsActive == true);
                //        //only list attorneys who are certified for the provided hearing type and not already scheduled on the session date
                //        attorneys = attorneys.Where(a => !(a.JcasSessions.Where(s => s.SessionDate == jcasSession.SessionDate && s.AttorneyId == a.Id).Any())
                //          && !(items.Contains(a.Id))//Added by DANG
                //          && string.Join(",", a.JcasAttorneyHearingTypes.Select(h => h.HearingTypeId.ToString()).ToList()).Contains(jcasSession.HearingTypeId.ToString()));
                //        ViewBag.AttorneyList = new SelectList(attorneys ?? db.JcasAttorneys.Where(a => a.Id == 0), "Id", "FullName");
                //        ViewBag.CourtroomList = new SelectList(db.JcasCourtrooms, "Id", "Courtroom");
                //        return View(jcasSession);
                //    }
                //}
            }
        }

        /// <summary>Assign an attorney to a session.</summary>
        /// <param name="jcasSession">Object to represent the session to schedule.</param>
        /// <returns>Form for scheduling a Session.</returns>
        [HttpPost]
        [Authorize(Roles = Constants.InternalRoleDataEntry + "," + Constants.ExternalRoleAttorney)]
        public ActionResult Schedule(JcasSession jcasSession)
        {
            IEnumerable<JcasAttorney> attorneys = db.JcasAttorneys.Where(a => a.IsActive == true);
            //only list attorneys who are certified for the provided hearing type and not already scheduled on the session date
            attorneys = attorneys.Where(a => !(a.JcasSessions.Where(s => s.SessionDate == jcasSession.SessionDate && s.AttorneyId == a.Id).Any())
                       && string.Join(",", a.JcasAttorneyHearingTypes.Select(h => h.HearingTypeId.ToString()).ToList()).Contains(jcasSession.HearingTypeId.ToString()));
            ViewBag.AttorneyList = new SelectList(attorneys ?? db.JcasAttorneys.Where(a => a.Id == 0), "Id", "FullName");
            ViewBag.CourtroomList = new SelectList(db.JcasCourtrooms, "Id", "Courtroom");
            jcasSession.JcasHearingType = db.JcasHearingTypes.Find(jcasSession.HearingTypeId);
            ViewBag.SelectedMonth = jcasSession.SessionDate.ToString("MM/yyyy");

            if (ModelState.IsValid)
            {
                try
                {
                    if (jcasSession.AttorneyId.HasValue && jcasSession.AttorneyId.Value > 0)
                    {
                        //Added by DANG---------------------------------------------------------
                        var jcasSessionDB = db.JcasSessions.FirstOrDefault(q => q.Id == jcasSession.Id && q.AttorneyId > 0);
                        if (jcasSessionDB != null)
                        {
                            var jcasSessionRequest = new JcasSessionRequest();
                            jcasSessionRequest.SessionId = jcasSession.Id;
                            jcasSessionRequest.AttorneyId = jcasSession.AttorneyId;
                            jcasSessionRequest.CourtroomId = jcasSession.CourtroomId;
                            jcasSessionRequest.SubmittedDate = DateTime.Today;
                            db.JcasSessionRequests.Add(jcasSessionRequest);
                            db.SaveChanges();
                            if (ModelState.IsValid)
                            {
                                //if the assigned attorney is not public defender, send email
                                if (jcasSession.AttorneyId != Convert.ToInt32(Helpers.Utility.GetSettingValue("PDAttorneyId")))
                                {
                                    Helpers.Utility.EmailSessionScheduled(db
                                        , db.JcasAttorneys.Find(jcasSession.AttorneyId).EmailAddress.ToString()
                                        , "Date: " + jcasSession.SessionDate.ToShortDateString()
                                            + " Hearing Type: " + db.JcasHearingTypes.Find(jcasSession.HearingTypeId).HearingTypeName.ToString()
                                        , jcasSession.SessionDate);
                                }
                                ViewBag.MessageText = "Session scheduled successful.";

                                jcasSession.JcasHearingType = db.JcasHearingTypes.Find(jcasSession.HearingTypeId);
                                jcasSession.JcasCourtroom = db.JcasCourtrooms.Find(jcasSession.CourtroomId);
                                if (jcasSession.AttorneyId.HasValue && jcasSession.AttorneyId.Value > 0)
                                {
                                    jcasSession.JcasAttorney = db.JcasAttorneys.Find(jcasSession.AttorneyId);
                                }
                                return View(jcasSession);
                            }
                        }

                        //End By DANG------------------------------------------------------------
                        else
                        {
                            //try to schedule the session
                            jcasSession.SubmittedDate = DateTime.Today;
                            db.Entry(jcasSession).State = EntityState.Modified;
                            db.SaveChanges(ModelState);
                            if (ModelState.IsValid)
                            {
                                //if the assigned attorney is not public defender, send email
                                if (jcasSession.AttorneyId != Convert.ToInt32(Helpers.Utility.GetSettingValue("PDAttorneyId")))
                                {
                                    Helpers.Utility.EmailSessionScheduled(db
                                        , db.JcasAttorneys.Find(jcasSession.AttorneyId).EmailAddress.ToString()
                                        , "Date: " + jcasSession.SessionDate.ToShortDateString()
                                            + " Hearing Type: " + db.JcasHearingTypes.Find(jcasSession.HearingTypeId).HearingTypeName.ToString()
                                        , jcasSession.SessionDate);
                                }
                                ViewBag.MessageText = "Session scheduled successful.";
                            }
                        }

                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Please select an attorney to schedule.");
                        return View(jcasSession);
                    }
                }
                catch (Exception ex)
                {
                    // get error message.
                    string message = JcasDescription.GetDatabaseMessage(db, ex);
                    message = (string.IsNullOrEmpty(message) ? ex.Message : message);
                    ModelState.AddModelError(string.Empty, message);
                }
            }

            jcasSession = db.JcasSessions.Include(s => s.JcasAttorney)
                           .Include(s => s.JcasCourtroom)
                           .Include(s => s.JcasHearingType)
                           .Where(s => s.Id == jcasSession.Id).FirstOrDefault();
            if (jcasSession != null)
            {
                jcasSession.JcasHearingType = db.JcasHearingTypes.Find(jcasSession.HearingTypeId);
                jcasSession.JcasCourtroom = db.JcasCourtrooms.Find(jcasSession.CourtroomId);
                if (jcasSession.AttorneyId.HasValue && jcasSession.AttorneyId.Value > 0)
                {
                    jcasSession.JcasAttorney = db.JcasAttorneys.Find(jcasSession.AttorneyId);
                }
            }
            return View(jcasSession);
        }

        /// <summary>Unschedule the attorney from the scheduled session.</summary>
        /// <param name="id">The session id to unschedule.</param>
        /// <returns>Message text of the unschedule.</returns>
        [HttpGet]
        [Authorize(Roles = Constants.InternalRoleDataEntry)]
        public ActionResult Unschedule(int? id, int? attorneyId)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            else
            {
                //JcasSession jcasSession = db.JcasSessions.Include(s => s.JcasAttorney)
                //    .Include(s => s.JcasCourtroom)
                //    .Include(s => s.JcasHearingType)
                //    .Where(s => s.Id == id).FirstOrDefault();
                //if (jcasSession != null)
                //{
                //    ViewBag.SelectedMonth = jcasSession.SessionDate.ToString("MM/yyyy");
                //    return View(jcasSession);
                //}
                //else
                //{
                //    return HttpNotFound();
                //}

                var sessionContainerModel = new SessionContainerModel();

                var queryJcasSession = (from jS in db.JcasSessions
                                        join jA in db.JcasAttorneys on jS.AttorneyId equals jA.Id
                                        join jC in db.JcasCourtrooms on jS.CourtroomId equals jC.Id into cr
                                        from x in cr.DefaultIfEmpty()
                                        join jHR in db.JcasHearingTypes on jS.HearingTypeId equals jHR.Id
                                        where jS.Id == id && jA.Id == attorneyId
                                        select new SessionContainerModel
                                        {
                                            Id = jS.Id,
                                            SessionDate = jS.SessionDate,
                                            AttorneyId = jA.Id,
                                            AttorneyName = jA.FullName,
                                            CourtroomId = x.Id,
                                            Courtroom = x.Courtroom,
                                            HearingTypeId = jHR.Id,
                                            HearingTypeName = jHR.HearingTypeName,
                                            UserLoginIdUpdate = jS.UserLoginIdUpdate,
                                            RowVersionId = jS.RowVersionId,
                                            SessionRequestId = 0
                                        }).FirstOrDefault();



                var queryJcasSessionRequest = (from jS in db.JcasSessions
                                               join jSR in db.JcasSessionRequests on jS.Id equals jSR.SessionId
                                               join jA in db.JcasAttorneys on jSR.AttorneyId equals jA.Id
                                               join jC in db.JcasCourtrooms on jSR.CourtroomId equals jC.Id into cr
                                               from x in cr.DefaultIfEmpty()
                                               join jHR in db.JcasHearingTypes on jS.HearingTypeId equals jHR.Id
                                               where jSR.SessionId == id && jSR.AttorneyId == attorneyId
                                               select new SessionContainerModel
                                               {
                                                   Id = jS.Id,
                                                   SessionDate = jS.SessionDate,
                                                   AttorneyId = jA.Id,
                                                   AttorneyName = jA.FullName,
                                                   CourtroomId = x.Id,
                                                   Courtroom = x.Courtroom,
                                                   HearingTypeId = jHR.Id,
                                                   HearingTypeName = jHR.HearingTypeName,
                                                   UserLoginIdUpdate = jSR.UserLoginIdUpdate,
                                                   RowVersionId = jSR.RowVersionId,
                                                   SessionRequestId = jSR.Id
                                               }).FirstOrDefault();


                if (queryJcasSession != null)
                {
                    sessionContainerModel = queryJcasSession;
                }
                else
                {
                    sessionContainerModel = queryJcasSessionRequest;
                }

                if (sessionContainerModel != null)
                {
                    sessionContainerModel.SessionDateString = sessionContainerModel.SessionDate.ToShortDateString();
                    ViewBag.SelectedMonth = sessionContainerModel.SessionDate.ToString("MM/yyyy");
                    return View(sessionContainerModel);
                }
                else
                {
                    return HttpNotFound();
                }


            }
        }

        /// <summary>Unschedule the attorney from the scheduled session.</summary>
        /// <param name="id">The session id to unschedule.</param>
        /// <returns>Message text of the unschedule.</returns>
        [HttpPost, ActionName("Unschedule")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Constants.InternalRoleDataEntry)]
        public ActionResult UnscheduleConfirmed(int id, int attorneyId)
        {
            //JcasSession jcasSession = db.JcasSessions.Find(id);
            //if (jcasSession == null)
            //{
            //    return HttpNotFound();
            //}

            try
            {
                JcasSession jcasSession = db.JcasSessions.FirstOrDefault(q => q.Id == id && q.AttorneyId == attorneyId);
                if (jcasSession != null)
                {
                    var queryJcasSession = (from jS in db.JcasSessions
                                            join jA in db.JcasAttorneys on jS.AttorneyId equals jA.Id
                                            join jC in db.JcasCourtrooms on jS.CourtroomId equals jC.Id into cr
                                            from x in cr.DefaultIfEmpty()
                                            join jHR in db.JcasHearingTypes on jS.HearingTypeId equals jHR.Id
                                            where jS.Id == id && jA.Id == attorneyId
                                            select new SessionContainerModel
                                            {
                                                Id = jS.Id,
                                                SessionDate = jS.SessionDate,
                                                AttorneyId = jA.Id,
                                                AttorneyName = jA.FullName,
                                                CourtroomId = x.Id,
                                                Courtroom = x.Courtroom,
                                                HearingTypeId = jHR.Id,
                                                HearingTypeName = jHR.HearingTypeName,
                                                UserLoginIdUpdate = jS.UserLoginIdUpdate,
                                                RowVersionId = jS.RowVersionId,
                                                SessionRequestId = 0
                                            }).FirstOrDefault();
                    queryJcasSession.SessionDateString = queryJcasSession.SessionDate.ToShortDateString();
                    ViewBag.SelectedMonth = jcasSession.SessionDate.ToString("MM/yyyy");
                    jcasSession.AttorneyId = null;
                    jcasSession.SubmittedDate = null;
                    jcasSession.CourtroomId = null;//Added by DANG 03/19/2024. This should be empty as well. 
                    db.Entry(jcasSession).State = EntityState.Modified;
                    db.SaveChanges(ModelState);
                    if (ModelState.IsValid)
                    {
                        ViewBag.MessageText = "Session unscheduled successful.";
                        //if the assigned attorney is not public defender, send email
                        if (attorneyId != Convert.ToInt32(Helpers.Utility.GetSettingValue("PDAttorneyId")))
                        {
                            Helpers.Utility.EmailSessionUnscheduled(db
                            , db.JcasAttorneys.Find(attorneyId).EmailAddress.ToString()
                            , "Date: " + jcasSession.SessionDate.ToShortDateString() + " Hearing Type: " + db.JcasHearingTypes.Find(jcasSession.HearingTypeId).HearingTypeName.ToString());
                        }

                        return View(queryJcasSession);
                    }

                }
                else
                {
                    JcasSessionRequest jcasSessionRequest = db.JcasSessionRequests.FirstOrDefault(q => q.SessionId == id && q.AttorneyId == attorneyId);
                    var jcasSessionDB = db.JcasSessions.Find(id);
                    if (jcasSessionRequest != null)
                    {

                        var queryJcasSessionRequest = (from jS in db.JcasSessions
                                                       join jSR in db.JcasSessionRequests on jS.Id equals jSR.SessionId
                                                       join jA in db.JcasAttorneys on jSR.AttorneyId equals jA.Id
                                                       join jC in db.JcasCourtrooms on jSR.CourtroomId equals jC.Id into cr
                                                       from x in cr.DefaultIfEmpty()
                                                       join jHR in db.JcasHearingTypes on jS.HearingTypeId equals jHR.Id
                                                       where jSR.SessionId == id && jSR.AttorneyId == attorneyId
                                                       select new SessionContainerModel
                                                       {
                                                           Id = jS.Id,
                                                           SessionDate = jS.SessionDate,
                                                           AttorneyId = jA.Id,
                                                           AttorneyName = jA.FullName,
                                                           CourtroomId = x.Id,
                                                           Courtroom = x.Courtroom,
                                                           HearingTypeId = jHR.Id,
                                                           HearingTypeName = jHR.HearingTypeName,
                                                           UserLoginIdUpdate = jSR.UserLoginIdUpdate,
                                                           RowVersionId = jSR.RowVersionId,
                                                           SessionRequestId = jSR.Id
                                                       }).FirstOrDefault();
                        queryJcasSessionRequest.SessionDateString = queryJcasSessionRequest.SessionDate.ToShortDateString();
                        var jcasSessionRequestDB = db.JcasSessionRequests.Find(jcasSessionRequest.Id);
                        db.JcasSessionRequests.Remove(jcasSessionRequestDB);
                        jcasSessionRequestDB.SessionId = id;
                        //db.Entry(jcasSessionRequest).State = EntityState.Deleted;
                        db.SaveChanges();

                        ViewBag.MessageText = "Session unscheduled successful.";
                        //if the assigned attorney is not public defender, send email
                        if (attorneyId != Convert.ToInt32(Helpers.Utility.GetSettingValue("PDAttorneyId")))
                        {
                            Helpers.Utility.EmailSessionUnscheduled(db
                            , db.JcasAttorneys.Find(attorneyId).EmailAddress.ToString()
                            , "Date: " + jcasSessionDB.SessionDate.ToShortDateString() + " Hearing Type: " + db.JcasHearingTypes.Find(jcasSessionDB.HearingTypeId).HearingTypeName.ToString());
                        }


                        return View(queryJcasSessionRequest);
                    }
                }



                //ViewBag.SelectedMonth = jcasSession.SessionDate.ToString("MM/yyyy");
                //int attorneyId = jcasSession.AttorneyId.Value;
                //jcasSession.AttorneyId = null;
                //jcasSession.SubmittedDate = null;
                //jcasSession.CourtroomId = null;//Added by DANG 03/19/2024. This should be empty as well. 
                //db.Entry(jcasSession).State = EntityState.Modified;
                //db.SaveChanges(ModelState);
                //if (ModelState.IsValid)
                //{
                //    ViewBag.MessageText = "Session unscheduled successful.";
                //    //if the assigned attorney is not public defender, send email
                //    if (attorneyId != Convert.ToInt32(Helpers.Utility.GetSettingValue("PDAttorneyId")))
                //    {
                //        Helpers.Utility.EmailSessionUnscheduled(db
                //        , db.JcasAttorneys.Find(attorneyId).EmailAddress.ToString()
                //        , "Date: " + jcasSession.SessionDate.ToShortDateString() + " Hearing Type: " + db.JcasHearingTypes.Find(jcasSession.HearingTypeId).HearingTypeName.ToString());
                //    }
                //    jcasSession = db.JcasSessions.Include(s => s.JcasAttorney)
                //                        .Include(s => s.JcasCourtroom)
                //                        .Include(s => s.JcasHearingType)
                //                        .Where(s => s.Id == jcasSession.Id).FirstOrDefault();
                //}


            }
            catch (Exception ex)
            {
                // if there is an error, add it to the Model State.
                string message = JcasDescription.GetDatabaseMessage(db, ex);
                message = (string.IsNullOrEmpty(message) ? ex.Message : message);
                ModelState.AddModelError("", message);
            }
            return View();

        }

        // GET: JcasSessions/Create
        /// <summary>Displays a blank form for entering a new JcasSession item.</summary>
        /// <returns>Blank form for entering a new JcasSession item.</returns>
        [HttpGet]
        [Authorize(Roles = Constants.InternalRoleDataEntry)]
        public ActionResult Create(string selectedMonth)
        {
            //disable the dates the court are closed or dates that already have sessions for all hearing types
            ViewBag.DisabledDates = string.Join(",", from c in GetCalendarDates()
                                                     where db.JcasCourtCloseds.Where(s => s.CourtClosedDate == c.Date).Any()
                                                     || db.JcasHearingTypes.Count() == db.JcasSessions.Where(s => s.SessionDate == c.Date).Count()
                                                     select c.ToShortDateString());
            ViewBag.EndDate = GetEndDate().ToShortDateString();
            ViewBag.CourtroomList = new SelectList(db.JcasCourtrooms, "Id", "Courtroom");
            ViewBag.SelectedMonth = (string.IsNullOrEmpty(selectedMonth) ? DateTime.Today.ToString("MM/yyyy") : selectedMonth);
            return View();
        }

        // POST: JcasSessions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        /// <summary>Allows the user to save a new JcasSession item.</summary>
        /// <param name="jcasSession">Object that represents the new JcasSession item to save.</param> 
        /// <returns>If there are validation errors, redisplays the form with the error messages, otherwise returns to the Index page.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Constants.InternalRoleDataEntry)]
        public ActionResult Create([Bind(Include = "Id,SessionDate,AttorneyId,HearingTypeId,CourtroomId,SubmittedDate,UserLoginIdUpdate,RowVersionId")] JcasSession jcasSession)
        {
            if (ModelState.IsValid)
            {
                if (jcasSession.AttorneyId != null)
                {
                    jcasSession.SubmittedDate = DateTime.Today;
                    //make sure the selected attorney is certified for the selected hearing type
                    JcasAttorneyHearingType hearingType = db.JcasAttorneyHearingTypes.Where(t => t.AttorneyId == jcasSession.AttorneyId && t.HearingTypeId == jcasSession.HearingTypeId).FirstOrDefault();
                    if (hearingType == null)
                    {
                        ModelState.AddModelError(string.Empty, "The selected attorney is not certified for the selected hearing type.");
                    }
                }
                if (ModelState.IsValid)
                {
                    db.JcasSessions.Add(jcasSession);
                    db.SaveChanges(ModelState);
                    if (ModelState.IsValid)
                    {
                        ViewBag.MessageText = "Session created successful.";
                        //if an attorney is assigned, email the attorney
                        if (jcasSession.AttorneyId != null && jcasSession.AttorneyId != Convert.ToInt32(Helpers.Utility.GetSettingValue("PDAttorneyId")))
                        {
                            Helpers.Utility.EmailSessionScheduled(db
                                , db.JcasAttorneys.Find(jcasSession.AttorneyId).EmailAddress.ToString()
                                , "Date: " + jcasSession.SessionDate.ToShortDateString()
                                    + " Hearing Type: " + db.JcasHearingTypes.Find(jcasSession.HearingTypeId).HearingTypeName.ToString()
                                , jcasSession.SessionDate);
                        }
                    }
                }
            }
            jcasSession.JcasHearingType = db.JcasHearingTypes.Find(jcasSession.HearingTypeId);
            jcasSession.JcasCourtroom = db.JcasCourtrooms.Find(jcasSession.CourtroomId);
            //disable the dates the court are closed or dates that already have sessions for all hearing types
            ViewBag.DisabledDates = string.Join(",", from c in GetCalendarDates()
                                                     where db.JcasCourtCloseds.Where(s => s.CourtClosedDate == c.Date).Any()
                                                     || db.JcasHearingTypes.Count() == db.JcasSessions.Where(s => s.SessionDate == c.Date).Count()
                                                     select c.ToShortDateString());
            ViewBag.EndDate = GetEndDate().ToShortDateString();
            ViewBag.CourtroomList = new SelectList(db.JcasCourtrooms, "Id", "Courtroom");
            ViewBag.SelectedMonth = jcasSession.SessionDate.ToString("MM/yyyy");

            return View(jcasSession);
        }

        /// <summary>Get hearing types that are not already exists for the selected session date.</summary>
        /// <param name="sessionDate">Date to list the hearing types.</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetHearingTypes(DateTime sessionDate)
        {
            if (sessionDate != null && DateTime.TryParse(sessionDate.ToShortDateString(), out DateTime outDate) == true)
            {
                IEnumerable<SelectListItem> hearingTypes = (from h in db.JcasHearingTypes
                                                            where !h.JcasSessions.Where(s => s.SessionDate == sessionDate && s.HearingTypeId == h.Id).Any()
                                                            select new SelectListItem { Value = h.Id.ToString(), Text = h.HearingTypeCode }).ToList();
                return Json(new SelectList(hearingTypes, "Value", "Text"), JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        /// <summary>Get attorneys certified for the selected hearing type who are not currently scheduled for the selected session date.</summary>
        /// <param name="sessionDate">The selected session date.</param>
        /// <param name="hearingTypeId">The selected hearing type.</param>
        /// <returns>List of attorneys certified for the selected hearing type who are not currently scheduled for the selected session date</returns>
        [HttpGet]
        public ActionResult GetAttorneys(DateTime sessionDate, int hearingTypeId)
        {
            if (sessionDate != null && DateTime.TryParse(sessionDate.ToShortDateString(), out DateTime outDate) == true && hearingTypeId > 0)
            {
                IEnumerable<SelectListItem> attorneys =
                    (from a in db.JcasAttorneys
                     where a.IsActive == true
                         && a.JcasAttorneyHearingTypes.Where(h => h.HearingTypeId == hearingTypeId).Any()
                         && !a.JcasSessions.Where(s => s.SessionDate == sessionDate
                                                 && s.HearingTypeId != hearingTypeId
                                                 && s.AttorneyId == a.Id).Any()
                     select new SelectListItem { Value = a.Id.ToString(), Text = a.FullName }).ToList();
                return Json(new SelectList(attorneys, "Value", "Text"), JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        /// <summary>Display courtrooms to update for the selected session date.</summary>
        /// <param name="selectedDate">The selected session date to display courtrooms.</param>
        /// <returns>List of courtrooms for the selected session date.</returns>
        [Authorize(Roles = Constants.InternalRoleDataEntry)]
        [HttpGet]
        public ActionResult Courtrooms(DateTime? selectedDate)
        {
            selectedDate = (selectedDate ?? DateTime.Today.Date);
            ViewBag.SelectedMonth = selectedDate.Value.ToString("MM/yyyy");
            List<JcasSession> sessions = db.JcasSessions.Where(s => s.SessionDate == selectedDate).ToList();
            if (sessions != null)
            {
                //if no session found for the selected date, redirect to Calendar screen
                if (sessions.Count() == 0)
                    return RedirectToAction("Calendar", "JcasSessions", new { selectedMonth = selectedDate.Value.ToString("MM/yyyy") });
                //if at least one session found, refresh lists and display the Courtrooms screen
                else
                {
                    ViewBag.CourtroomList = new SelectList(db.JcasCourtrooms, "Id", "Courtroom");
                    //disable the dates that the court is closed or that are not in session table
                    ViewBag.DisabledDates = string.Join(",", from c in GetCalendarDates()
                                                             where db.JcasCourtCloseds.Where(s => s.CourtClosedDate == c.Date).Any()
                                                             || !db.JcasSessions.Where(s => s.SessionDate == c.Date).Any()
                                                             select c.ToShortDateString());
                    ViewBag.EndDate = GetEndDate().ToShortDateString();
                    ViewBag.SelectedDate = selectedDate.Value.ToShortDateString();
                    return View(sessions);
                }
            }
            else
            {
                return HttpNotFound();
            }
        }

        /// <summary>Update all courtrooms for the selected session date.</summary>
        /// <param name="jcasSessions">The list of courtrooms for the selected session date to update.</param>
        /// <returns>List of courtrooms for the selected session date.</returns>
        [Authorize(Roles = Constants.InternalRoleDataEntry)]
        [HttpPost]
        public ActionResult Courtrooms(List<JcasSession> jcasSessions)
        {
            DateTime selectedDate = DateTime.Today.Date;
            if (jcasSessions.Count() > 0)
            {
                try
                {
                    selectedDate = db.JcasSessions.Find(jcasSessions.FirstOrDefault().Id).SessionDate;

                    //either all courtroom assignments will be saved or none at all
                    foreach (JcasSession session in jcasSessions)
                    {
                        if (!ModelState.IsValid)
                        {
                            break;
                        }
                        var sessionToUpdate = db.JcasSessions.Find(session.Id);
                        if (sessionToUpdate != null)
                        {
                            sessionToUpdate.CourtroomId = session.CourtroomId;
                            db.Entry(sessionToUpdate).State = EntityState.Modified;
                            db.SaveChanges(ModelState);
                        }
                    }
                    if (ModelState.IsValid)
                        ViewBag.MessageText = "Courtroom(s) updated sucessful.";
                }
                catch (Exception ex)
                {
                    // if there is an error, add it to the Model State.
                    string message = JcasDescription.GetDatabaseMessage(db, ex);
                    message = string.IsNullOrEmpty(message) ? ex.Message : message;
                    ModelState.AddModelError("", message);
                }
            }
            List<JcasSession> sessions = db.JcasSessions.Where(s => s.SessionDate == selectedDate).Include(r => r.JcasCourtroom).ToList();
            ViewBag.SelectedDate = selectedDate.ToShortDateString();
            ViewBag.SelectedMonth = selectedDate.ToString("MM/yyyy");
            ViewBag.CourtroomList = new SelectList(db.JcasCourtrooms, "Id", "Courtroom");
            //disable the dates that the court is closed or that are not in session table
            ViewBag.DisabledDates = string.Join(",", from c in GetCalendarDates()
                                                     where db.JcasCourtCloseds.Where(s => s.CourtClosedDate == c.Date).Any()
                                                     || !db.JcasSessions.Where(s => s.SessionDate == c.Date).Any()
                                                     select c.ToShortDateString());
            ViewBag.EndDate = GetEndDate().ToShortDateString();

            return View(sessions);
        }

        // GET: JcasSessions/Delete/5
        /// <summary>Displays the specified JcasSession item prior to deleting.</summary>
        /// <param name="id">Identifies the JcasSession item to display prior to deleting.</param>
        /// <returns>The specified JcasSession item and asks the user to confirm that it should be deleted.</returns>
        [Authorize(Roles = Constants.InternalRoleDataEntry)]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            JcasSession jcasSession = db.JcasSessions.Find(id);
            if (jcasSession == null)
            {
                return HttpNotFound();
            }
            ViewBag.SelectedMonth = jcasSession.SessionDate.ToString("MM/yyyy");
            return View(jcasSession);
        }

        // POST: JcasSessions/Delete/5
        /// <summary>Deletes the specified JcasSession item.</summary>
        /// <param name="id">Identifies the JcasSession item to delete.</param>
        /// <returns>After deleting, returns to the Index page.</returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Constants.InternalRoleDataEntry)]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                JcasSession jcasSession = db.JcasSessions.Find(id);
                ViewBag.SelectedMonth = jcasSession.SessionDate.ToString("MM/yyyy");
                string attorneyEmail = string.Empty;
                if (jcasSession.AttorneyId != null && jcasSession.AttorneyId != Convert.ToInt32(Helpers.Utility.GetSettingValue("PDAttorneyId")))
                {
                    attorneyEmail = db.JcasAttorneys.Find(jcasSession.AttorneyId).EmailAddress.ToString();
                }
                string sessionDate = jcasSession.SessionDate.ToShortDateString();
                string hearingType = db.JcasHearingTypes.Find(jcasSession.HearingTypeId).HearingTypeName.ToString();
                db.JcasSessions.Remove(jcasSession);
                db.SaveChanges(ModelState);

                if (ModelState.IsValid)
                {
                    ViewBag.MessageText = "Session deleted successful.";
                    if (!string.IsNullOrEmpty(attorneyEmail))
                    {
                        Helpers.Utility.EmailSessionDeleted(db
                            , attorneyEmail
                            , "Date: " + sessionDate + " Hearing Type: " + hearingType);
                    }
                }
            }
            catch (Exception ex)
            {
                // if there is an error, add it to the Model State.
                string message = JcasDescription.GetDatabaseMessage(db, ex);
                if (string.IsNullOrEmpty(message))
                {
                    message = ex.Message;
                }
                ModelState.AddModelError("", message);
            }
            return View();
        }

        /// <summary>Releases unmanaged resources.</summary>
        /// <param name="disposing">Indicates whether the method call comes from a Dispose method (its value is true) or from a finalizer (its value is false).</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
