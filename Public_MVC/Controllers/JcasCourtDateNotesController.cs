using System;
using System.Data.Entity;
using System.Net;
using System.Web.Mvc;
using System.Collections.Generic;
using FairfaxCounty.JCAS_Public_MVC.Models;
using System.Linq;

namespace FairfaxCounty.JCAS_Public_MVC.Controllers
{
    /// <summary>Allows DataEntry users to add/edit/delete the JcasCourtDateNote items.</summary>
    [Authorize(Roles = Constants.InternalRoleDataEntry)]
    public class JcasCourtDateNotesController : Controller
    {
        private JcasEntities db = new JcasEntities();

        // GET: JcasCourtDateNotes
        /// <summary>Displays the list of CourtDateNotes.</summary>
        /// <param name="selectedYear">The year of all notes to list.</param>
        public ActionResult Index(int? selectedYear)
        {
            //populate the yearlist with this year and next 
            List<SelectListItem> yearList = new List<SelectListItem>();
            yearList.Add(new SelectListItem { Text = DateTime.Today.Year.ToString(), Value = DateTime.Today.Year.ToString() });
            yearList.Add(new SelectListItem { Text = DateTime.Today.AddYears(1).Year.ToString(), Value = DateTime.Today.AddYears(1).Year.ToString() });
            ViewBag.Years = yearList;

            selectedYear = (selectedYear.HasValue ? selectedYear : DateTime.Today.Year);
            ViewBag.SelectedYear = selectedYear;

            IEnumerable<JcasCourtDateNote> jcasCourtDateNotes = db.JcasCourtDateNotes.Where(c => c.FromDate.Year == selectedYear || c.ToDate.Year == selectedYear)
                .OrderBy(c => c.FromDate).ThenBy(c => c.ToDate);
            return View(jcasCourtDateNotes);
        }

        /// <summary>Gets the end date (end of next year) to display on the date calendar.</summary>
        public DateTime GetEndDate()
        {
            return DateTime.Parse("12/31/" + DateTime.Today.AddYears(1).Year.ToString());
        }

        // GET: JcasCourtDateNotes/Create
        /// <summary>Displays a blank form for entering a new JcasCourtDateNote item.</summary>
        /// <returns>Blank form for entering a new JcasCourtDateNote item.</returns>
        public ActionResult Create()
        {
            ViewBag.EndDate = GetEndDate().ToShortDateString();
            return View();
        }

        // POST: JcasCourtDateNotes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        /// <summary>Allows the user to save a new JcasCourtDateNote item.</summary>
        /// <param name="jcasCourtDateNote">Object that represents the new JcasCourtDateNote item to save.</param>
        /// <returns>If there are validation errors, redisplays the form with the error messages, otherwise returns to the Index page.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,FromDate,ToDate,Note,DisplayOrder,UserLoginIdUpdate,RowVersionId")] JcasCourtDateNote jcasCourtDateNote)
        {
            if (ModelState.IsValid)
            {
                db.JcasCourtDateNotes.Add(jcasCourtDateNote);
                db.SaveChanges(ModelState);
                if (ModelState.IsValid)
                {
                    return RedirectToAction("Index");
                }
            }
            ViewBag.EndDate = GetEndDate().ToShortDateString();
            return View(jcasCourtDateNote);
        }

        // GET: JcasCourtDateNotes/Edit/5
        /// <summary>Displays the specified JcasCourtDateNote item for editing.</summary>
        /// <param name="id">Identifies the JcasCourtDateNote item to edit.</param>
        /// <returns>The specified JcasCourtDateNote item for editing.</returns>
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            JcasCourtDateNote jcasCourtDateNote = db.JcasCourtDateNotes.Find(id);
            if (jcasCourtDateNote == null)
            {
                return HttpNotFound();
            }
            ViewBag.EndDate = GetEndDate().ToShortDateString();
            return View(jcasCourtDateNote);
        }

        // POST: JcasCourtDateNotes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        /// <summary>Allows users to save changes to a JcasCourtDateNote item.</summary>
        /// <param name="jcasCourtDateNote">Object that represents the updated JcasCourtDateNote item to save.</param>
        /// <returns>If there are validation errors, redisplays the form with the error messages, otherwise returns to the Index page.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,FromDate,ToDate,Note,DisplayOrder,UserLoginIdUpdate,RowVersionId")] JcasCourtDateNote jcasCourtDateNote)
        {
            if (ModelState.IsValid)
            {
                db.Entry(jcasCourtDateNote).State = EntityState.Modified;
                db.SaveChanges(ModelState);
                if (ModelState.IsValid)
                {
                    return RedirectToAction("Index");
                }
            }
            ViewBag.EndDate = GetEndDate().ToShortDateString();
            return View(jcasCourtDateNote);
        }

        // GET: JcasCourtDateNotes/Delete/5
        /// <summary>Displays the specified JcasCourtDateNote item prior to deleting.</summary>
        /// <param name="id">Identifies the JcasCourtDateNote item to display prior to deleting.</param>
        /// <returns>The specified JcasCourtDateNote item and asks the user to confirm that it should be deleted.</returns>
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            JcasCourtDateNote jcasCourtDateNote = db.JcasCourtDateNotes.Find(id);
            if (jcasCourtDateNote == null)
            {
                return HttpNotFound();
            }
            return View(jcasCourtDateNote);
        }

        // POST: JcasCourtDateNotes/Delete/5
        /// <summary>Deletes the specified JcasCourtDateNote item.</summary>
        /// <param name="id">Identifies the JcasCourtDateNote item to delete.</param>
        /// <returns>After deleting, returns to the Index page.</returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            JcasCourtDateNote jcasCourtDateNote = db.JcasCourtDateNotes.Find(id);
            db.JcasCourtDateNotes.Remove(jcasCourtDateNote);
            db.SaveChanges(ModelState);
            if (ModelState.IsValid)
            {
                return RedirectToAction("Index");
            }
            db.Entry(jcasCourtDateNote).State = EntityState.Unchanged;
            return View(jcasCourtDateNote);
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
