using System;
using System.Data.Entity;
using System.Net;
using System.Web.Mvc;
using System.Collections.Generic;
using FairfaxCounty.JCAS_Public_MVC.Models;
namespace FairfaxCounty.JCAS_Public_MVC.Controllers
{
    /// <summary>Allows SysAdmin users to add/edit/delete the JcasHearingType items.</summary>
    [Authorize(Roles = Constants.InternalRoleSysAdmin)]
    public class JcasHearingTypesController : Controller
    {
        private JcasEntities db = new JcasEntities();
        // GET: JcasHearingTypess
        /// <summary>Displays the list of HearingTypes.</summary>
        public ActionResult Index()
        {
            IEnumerable<JcasHearingType> jcasHearingTypes = db.JcasHearingTypes;
            return View(jcasHearingTypes);
        }

        // GET: JcasHearingTypes/Create
        /// <summary>Displays a blank form for entering a new JcasHearingType item.</summary>
        /// <returns>Blank form for entering a new JcasHearingType item.</returns>
        public ActionResult Create()
        {

            return View();
        }

        // POST: JcasHearingTypes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        /// <summary>Allows the user to save a new JcasHearingType item.</summary>
        /// <param name="jcasHearingType">Object that represents the new JcasHearingType item to save.</param>
        /// <returns>If there are validation errors, redisplays the form with the error messages, otherwise returns to the Index page.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,HearingTypeCode,HearingTypeName,DefaultCourtroom,UserLoginIdUpdate,RowVersionId")] JcasHearingType jcasHearingType)
        {
            if (ModelState.IsValid)
            {
                db.JcasHearingTypes.Add(jcasHearingType);
                db.SaveChanges(ModelState);
                if (ModelState.IsValid)
                {
                    return RedirectToAction("Index");
                }
            }

            return View(jcasHearingType);
        }

        // GET: JcasHearingTypes/Edit/5
        /// <summary>Displays the specified JcasHearingType item for editing.</summary>
        /// <param name="id">Identifies the JcasHearingType item to edit.</param>
        /// <returns>The specified JcasHearingType item for editing.</returns>
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            JcasHearingType jcasHearingType = db.JcasHearingTypes.Find(id);
            if (jcasHearingType == null)
            {
                return HttpNotFound();
            }

            return View(jcasHearingType);
        }

        // POST: JcasHearingTypes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        /// <summary>Allows users to save changes to a JcasHearingType item.</summary>
        /// <param name="jcasHearingType">Object that represents the updated JcasHearingType item to save.</param>
        /// <returns>If there are validation errors, redisplays the form with the error messages, otherwise returns to the Index page.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,HearingTypeCode,HearingTypeName,DefaultCourtroom,UserLoginIdUpdate,RowVersionId")] JcasHearingType jcasHearingType)
        {
            if (ModelState.IsValid)
            {
                db.Entry(jcasHearingType).State = EntityState.Modified;
                db.SaveChanges(ModelState);
                if (ModelState.IsValid)
                {
                    return RedirectToAction("Index");
                }
            }

            return View(jcasHearingType);
        }

        // GET: JcasHearingTypes/Delete/5
        /// <summary>Displays the specified JcasHearingType item prior to deleting.</summary>
        /// <param name="id">Identifies the JcasHearingType item to display prior to deleting.</param>
        /// <returns>The specified JcasHearingType item and asks the user to confirm that it should be deleted.</returns>
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            JcasHearingType jcasHearingType = db.JcasHearingTypes.Find(id);
            if (jcasHearingType == null)
            {
                return HttpNotFound();
            }
            return View(jcasHearingType);
        }

        // POST: JcasHearingTypes/Delete/5
        /// <summary>Deletes the specified JcasHearingType item.</summary>
        /// <param name="id">Identifies the JcasHearingType item to delete.</param>
        /// <returns>After deleting, returns to the Index page.</returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            JcasHearingType jcasHearingType = db.JcasHearingTypes.Find(id);
            db.JcasHearingTypes.Remove(jcasHearingType);
            db.SaveChanges(ModelState);
            if (ModelState.IsValid)
            {
                return RedirectToAction("Index");
            }
            db.Entry(jcasHearingType).State = EntityState.Unchanged;
            return View(jcasHearingType);
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
