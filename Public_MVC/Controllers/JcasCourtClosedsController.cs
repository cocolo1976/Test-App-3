using System;
using System.Data.Entity;
using System.Net;
using System.Web.Mvc;
using System.Collections.Generic;
using FairfaxCounty.JCAS_Public_MVC.Models;
using System.Linq;

namespace FairfaxCounty.JCAS_Public_MVC.Controllers
{
    /// <summary>Allows users to add/edit/delete the JcasCourtClosed items.</summary>
    [Authorize(Roles = Constants.InternalRoleDataEntry)]
    public class JcasCourtClosedsController : Controller
    {
        private JcasEntities db = new JcasEntities();

        /// <summary>Refreshes the dropdown list for Years selection.</summary>
        public void RefreshLists()
        {
            ViewBag.Years = new List<SelectListItem>(db.JcasCourtCloseds
                   .Select(c => c.CourtClosedDate.Year.ToString())
                   .Distinct()
                   .Select(d => new SelectListItem { Text = d, Value = d })
                   .OrderByDescending(d => d)
                   .ToList());
        }

        // GET: JcasCourtCloseds
        /// <summary>Displays the list of CourtCloseds.</summary>
        /// <param name="selectedYear">The year of all court closed dates to list.</param>
        public ActionResult Index(int? selectedYear)
        {
            RefreshLists();
            selectedYear = (selectedYear.HasValue ? selectedYear : System.DateTime.Today.Year);
            ViewBag.SelectedYear = selectedYear;
            IEnumerable<JcasCourtClosed> jcasCourtCloseds = db.JcasCourtCloseds.Where(c => c.CourtClosedDate.Year == selectedYear).OrderBy(c=>c.CourtClosedDate);
            return View(jcasCourtCloseds);
        }

        /// <summary>Gets the end date to display calendar/sessions.</summary>
        public DateTime GetEndDate()
        {
            if (!int.TryParse(Helpers.Utility.GetSettingValue("InternalSessionMonths"), out int outmonths))
            {
                outmonths = 12;
            }
            DateTime dateReturn = DateTime.Today;
            if (DateTime.TryParse(dateReturn.AddMonths(outmonths - 1).ToShortDateString(), out DateTime outDate))
            {
                dateReturn = outDate.AddMonths(1).AddDays(0 - outDate.Day);
            }
            return dateReturn;
        }

        /// <summary>Gets list of calendar dates between today and end date allowable per user role that are not already a court closed date
        /// or not have any scheduled sessions.</summary>
        public string GetCourtClosedDisabledDates()
        {
            DateTime startDate = DateTime.Today.Date;
            DateTime endDate = GetEndDate().Date;
            List<DateTime> dates = Enumerable.Range(0, (endDate - startDate).Days + 1)
                            .Select(day => startDate.AddDays(day))
                            .Where(day => (!day.DayOfWeek.Equals(DayOfWeek.Sunday)
                                && !day.DayOfWeek.Equals(DayOfWeek.Saturday))).ToList();
            string disabledDates = string.Join(",", 
                from c in dates
                where (db.JcasCourtCloseds.Where(cl => cl.CourtClosedDate == c.Date).Any()) 
                    || (db.JcasSessions.Where(s => s.SessionDate == c.Date && s.AttorneyId != null).Any())
                select c.Date.ToShortDateString()); 
            return disabledDates;
        }

        // GET: JcasCourtCloseds/Create
        /// <summary>Displays a blank form for entering a new JcasCourtClosed item.</summary>
        /// <returns>Blank form for entering a new JcasCourtClosed item.</returns>
        public ActionResult Create()
        {
            ViewBag.EndDate = GetEndDate().ToShortDateString();
            ViewBag.DisabledDates = GetCourtClosedDisabledDates();
            return View();
        }

        // POST: JcasCourtCloseds/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        /// <summary>Allows the user to save a new JcasCourtClosed item.</summary>
        /// <param name="jcasCourtClosed">Object that represents the new JcasCourtClosed item to save.</param>
        /// <returns>If there are validation errors, redisplays the form with the error messages, otherwise returns to the Index page.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,CourtClosedDate,CourtClosedReason,UserLoginIdUpdate,RowVersionId")] JcasCourtClosed jcasCourtClosed)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    //if there exists a session with a scheduled attorney on the same date, display error
                    JcasSession jcasSession = db.JcasSessions.Where(s => s.SessionDate == jcasCourtClosed.CourtClosedDate && s.AttorneyId != null).FirstOrDefault();
                    if (jcasSession != null)
                    {
                        ModelState.AddModelError(string.Empty,
                            "Court Closed Date must not be on a weekend and must not already exist a session on the same date with a scheduled attorney.");
                    }
                    else
                    {
                        //remove the sessions first
                        IEnumerable<JcasSession> jcasSessions = db.JcasSessions.Where(s => s.SessionDate == jcasCourtClosed.CourtClosedDate);
                        foreach (JcasSession sessionRemove in jcasSessions)
                        {
                            db.JcasSessions.Remove(sessionRemove);
                        }
                        db.SaveChanges(ModelState);

                        //remove the court closed date
                        db.JcasCourtCloseds.Add(jcasCourtClosed);
                        db.SaveChanges(ModelState);
                        if (ModelState.IsValid)
                        {
                            return RedirectToAction("Index");
                        }
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }
            ViewBag.EndDate = GetEndDate().ToShortDateString();
            ViewBag.DisabledDates = GetCourtClosedDisabledDates();
            return View(jcasCourtClosed);
        }

        // GET: JcasCourtCloseds/Edit/5
        /// <summary>Displays the specified JcasCourtClosed item for editing.</summary>
        /// <param name="id">Identifies the JcasCourtClosed item to edit.</param>
        /// <returns>The specified JcasCourtClosed item for editing.</returns>
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ViewBag.EndDate = GetEndDate().ToShortDateString();
            ViewBag.DisabledDates = GetCourtClosedDisabledDates();
            JcasCourtClosed jcasCourtClosed = db.JcasCourtCloseds.Find(id);
            if (jcasCourtClosed == null)
            {
                return HttpNotFound();
            }

            return View(jcasCourtClosed);
        }

        // POST: JcasCourtCloseds/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        /// <summary>Allows users to save changes to a JcasCourtClosed item.</summary>
        /// <param name="jcasCourtClosed">Object that represents the updated JcasCourtClosed item to save.</param>
        /// <returns>If there are validation errors, redisplays the form with the error messages, otherwise returns to the Index page.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,CourtClosedDate,CourtClosedReason,UserLoginIdUpdate,RowVersionId")] JcasCourtClosed jcasCourtClosed)
        {
            if (ModelState.IsValid)
            {
                //if there exists a session with a scheduled attorney on the same date, display error
                JcasSession jcasSession = db.JcasSessions.Where(s => s.SessionDate == jcasCourtClosed.CourtClosedDate && s.AttorneyId != null).FirstOrDefault();
                if (jcasSession != null)
                {
                    ModelState.AddModelError(string.Empty,
                        "Court Closed Date must not be on a weekend and must not already exist a session on the same date with a scheduled attorney.");
                }
                else
                {
                    //remove the sessions first
                    IEnumerable<JcasSession> jcasSessions = db.JcasSessions.Where(s => s.SessionDate == jcasCourtClosed.CourtClosedDate);
                    foreach (JcasSession sessionRemove in jcasSessions)
                    {
                        db.JcasSessions.Remove(sessionRemove);
                    }
                    db.SaveChanges(ModelState);
                    //save the court closed date
                    db.Entry(jcasCourtClosed).State = EntityState.Modified;
                    db.SaveChanges(ModelState);
                    if (ModelState.IsValid)
                    {
                        return RedirectToAction("Index");
                    }
                }
            }
            ViewBag.EndDate = GetEndDate().ToShortDateString();
            ViewBag.DisabledDates = GetCourtClosedDisabledDates();
            return View(jcasCourtClosed);
        }

        // GET: JcasCourtCloseds/Delete/5
        /// <summary>Displays the specified JcasCourtClosed item prior to deleting.</summary>
        /// <param name="id">Identifies the JcasCourtClosed item to display prior to deleting.</param>
        /// <returns>The specified JcasCourtClosed item and asks the user to confirm that it should be deleted.</returns>
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            JcasCourtClosed jcasCourtClosed = db.JcasCourtCloseds.Find(id);
            if (jcasCourtClosed == null)
            {
                return HttpNotFound();
            }
            return View(jcasCourtClosed);
        }

        // POST: JcasCourtCloseds/Delete/5
        /// <summary>Deletes the specified JcasCourtClosed item.</summary>
        /// <param name="id">Identifies the JcasCourtClosed item to delete.</param>
        /// <returns>After deleting, returns to the Index page.</returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            JcasCourtClosed jcasCourtClosed = db.JcasCourtCloseds.Find(id);
            db.JcasCourtCloseds.Remove(jcasCourtClosed);
            db.SaveChanges(ModelState);
            if (ModelState.IsValid)
            {
                return RedirectToAction("Index");
            }
            db.Entry(jcasCourtClosed).State = EntityState.Unchanged;
            return View(jcasCourtClosed);
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
