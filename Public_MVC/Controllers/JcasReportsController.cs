using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using FairfaxCounty.JCAS_Public_MVC.Models;

namespace FairfaxCounty.JCAS_Public_MVC.Controllers
{
    /// <summary>Allows users to generate a report.</summary>
    [Authorize(Roles = Constants.ExternalRoleAttorney + "," + Constants.InternalRoleReadOnly)]
    public class JcasReportsController : Controller
    {
        private JcasEntities db = new JcasEntities();

        // GET: JcasReports
        /// <summary>Directly open some reports with the default parameters.</summary>
        /// <param name="id">Identify the report to run.</param>
        /// <param name="fromDate">Specify the begin of the report date range.</param>
        /// <param name="toDate">Specify the end of the report date range.</param>
        [HttpGet]
        public ActionResult Run(int id, DateTime? fromDate, DateTime? toDate)
        {
            IEnumerable<JcasAttorney> attorneys = db.JcasAttorneys;
            try
            {
                JcasReport jcasReport = db.JcasReports.Find(id);
                string reportHtml = string.Empty;
                if (jcasReport != null)
                {
                    int attorneyId = 0;
                    if (jcasReport.ReportParameter.Contains("AttorneyId") && User.IsInRole(Constants.ExternalRoleAttorney))
                    {
                        attorneyId = db.JcasAttorneys.Where(a => a.EmailAddress == User.Identity.Name.ToString()).Select(a => a.Id).FirstOrDefault();
                        ViewBag.AttorneyId = attorneyId;
                        attorneys = attorneys.Where(a => a.Id == attorneyId);
                    }
                    if (fromDate.HasValue && toDate.HasValue)
                    {
                        reportHtml = BuildReportHtml(
                                    id: id,
                                    fromDate: fromDate,
                                    toDate: toDate,
                                    attorneyId: attorneyId);
                        return File(Encoding.UTF8.GetBytes(reportHtml.ToString()), "application/msword", jcasReport.ReportName.ToString() + ".doc");
                    }
                    else
                    {
                        ViewBag.ReportName = jcasReport.ReportName.ToString();
                        ViewBag.ReportParameter = jcasReport.ReportParameter.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                if (!string.IsNullOrEmpty(ex.Message))
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }
            ViewBag.AttorneyList = new SelectList(attorneys, "Id", "FullName");
            return View();
        }

        // POST: JcasReports
        /// <summary>Run the selected report for the specified parameters.</summary>
        /// <param name="id">Identify the report to run.</param>
        /// <param name="attorneyId">Identify the attorney to filter the report to.</param>
        /// <param name="fromDate">Specify the begin of the report date range.</param>
        /// <param name="toDate">Specify the end of the report date range.</param>
        [HttpPost]
        public ActionResult Run(int id, int? attorneyId, DateTime? fromDate, DateTime? toDate)
        {
            StringBuilder errorMessage = new StringBuilder();
            IEnumerable<JcasAttorney> attorneys = db.JcasAttorneys;
            try
            {
                JcasReport jcasReport = db.JcasReports.Find(id);
                if (jcasReport != null)
                {
                    //if AttorneyId is one of the parameters for the selected report, make sure it is provided
                    if (jcasReport.ReportParameter.Contains("AttorneyId") && !attorneyId.HasValue)
                    {
                        errorMessage.Append("Attorney is required.");
                    }
                    //if FromDate is one of the parameters for the selected report, make sure it is provided
                    if (jcasReport.ReportParameter.Contains("FromDate") && !fromDate.HasValue)
                    {
                        errorMessage.Append("From Date is required.");
                    }
                    //if ToDate is one of the parameters for the selected report, make sure it is provided
                    if (jcasReport.ReportParameter.Contains("ToDate") && !toDate.HasValue)
                    {
                        errorMessage.AppendLine("To Date is required.");
                    }
                    //if FromDate and ToDate are the parameters for the selected report, make sure they are provided
                    if (jcasReport.ReportParameter.Contains("FromDate") && fromDate.HasValue
                        && jcasReport.ReportParameter.Contains("ToDate") && toDate.HasValue
                        && (fromDate.Value > toDate.Value || fromDate.Value.AddMonths(12) < toDate.Value))
                    {
                        errorMessage.AppendLine("From Date must be on or before To Date and not more than one year in between.");
                    }
                    //if there is a validation error, display it
                    if (!string.IsNullOrEmpty(errorMessage.ToString()))
                    {
                        ModelState.AddModelError(string.Empty, errorMessage.ToString());
                        ViewBag.ReportName = jcasReport.ReportName.ToString();
                        ViewBag.ReportParameter = jcasReport.ReportParameter.ToString();
                        if (User.IsInRole(Constants.ExternalRoleAttorney))
                        {
                            attorneyId = db.JcasAttorneys.Where(a => a.EmailAddress == User.Identity.Name.ToString()).Select(a => a.Id).FirstOrDefault();
                            ViewBag.AttorneyId = attorneyId;
                            attorneys = attorneys.Where(a => a.Id == attorneyId);
                        }
                        else
                        {
                            if (attorneyId.HasValue)
                            {
                                ViewBag.AttorneyId = attorneyId;
                            }
                        }
                        if (fromDate.HasValue)
                        {
                            ViewBag.FromDate = fromDate.Value.ToShortDateString();
                        }
                        if (toDate.HasValue)
                        {
                            ViewBag.ToDate = toDate.Value.ToShortDateString();
                        }
                    }
                    //else, no error
                    else
                    {
                        string reportHtml = BuildReportHtml(
                                        id: id,
                                        fromDate: fromDate,
                                        toDate: toDate,
                                        attorneyId: attorneyId);
                        return File(Encoding.UTF8.GetBytes(reportHtml.ToString()), "application/msword", jcasReport.ReportName.ToString() + ".doc");
                    }
                }
            }
            catch (Exception ex)
            {
                if (!string.IsNullOrEmpty(ex.Message))
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }
            ViewBag.AttorneyList = new SelectList(attorneys, "Id", "FullName");
            return View();
        }

        /// <summary>Gets html string from the stored procedure to be displayed in MS Word.</summary>
        /// <param name="id">Identify the selected report.</param>
        /// <param name="fromDate">The begin of the date range, if any.</param>
        /// <param name="toDate">The end of the date range, if any.</param>
        /// <param name="attorneyId">The selected attorney, if any.</param>
        /// <returns></returns>
        public string BuildReportHtml(int id, DateTime? fromDate, DateTime? toDate, int? attorneyId)
        {
            JcasReport jcasReport = db.JcasReports.Find(id);
            ObjectParameter reportHtml = new ObjectParameter("ReportHtml", typeof(string));
            switch (jcasReport.ReportName)
            {
                case "CourtCalendar":
                    db.rCourtCalendar(fromDate, toDate, reportHtml);
                    break;
                case "AttorneyCalendar":
                    db.rAttorneyCalendar(attorneyId, fromDate, toDate, reportHtml);
                    break;
                case "AttorneySchedule":
                    db.rAttorneySchedule(attorneyId, reportHtml);
                    break;
                default:
                    break;
            }
            return reportHtml.Value.ToString();
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
