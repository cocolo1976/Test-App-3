using System;
using System.Data.Entity;
using System.Net;
using System.Web.Mvc;
using System.Collections.Generic;
using FairfaxCounty.JCAS_Public_MVC.Models;
namespace FairfaxCounty.JCAS_Public_MVC.Controllers
{

    /// <summary>Allows SysAdmin users to add/edit/delete the JcasCourtroom items.</summary>
    [Authorize(Roles = Constants.InternalRoleSysAdmin)]
    public class JcasCourtroomsController : Controller
    {
        private JcasEntities db = new JcasEntities();
        // GET: JcasCourtroomss
        /// <summary>Displays the list of Courtrooms.</summary>
        public ActionResult Index()
        {
            IEnumerable<JcasCourtroom> jcasCourtrooms = db.JcasCourtrooms;
            return View(jcasCourtrooms);
        }

        // GET: JcasCourtrooms/Create
        /// <summary>Displays a blank form for entering a new JcasCourtroom item.</summary>
        /// <returns>Blank form for entering a new JcasCourtroom item.</returns>
        public ActionResult Create()
        {

            return View();
        }

        // POST: JcasCourtrooms/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        /// <summary>Allows the user to save a new JcasCourtroom item.</summary>
        /// <param name="jcasCourtroom">Object that represents the new JcasCourtroom item to save.</param>
        /// <returns>If there are validation errors, redisplays the form with the error messages, otherwise returns to the Index page.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Courtroom,UserLoginIdUpdate,RowVersionId")] JcasCourtroom jcasCourtroom)
        {
            if (ModelState.IsValid)
            {
                db.JcasCourtrooms.Add(jcasCourtroom);
                db.SaveChanges(ModelState);
                if (ModelState.IsValid)
                {
                    return RedirectToAction("Index");
                }
            }

            return View(jcasCourtroom);
        }

        // GET: JcasCourtrooms/Edit/5
        /// <summary>Displays the specified JcasCourtroom item for editing.</summary>
        /// <param name="id">Identifies the JcasCourtroom item to edit.</param>
        /// <returns>The specified JcasCourtroom item for editing.</returns>
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            JcasCourtroom jcasCourtroom = db.JcasCourtrooms.Find(id);
            if (jcasCourtroom == null)
            {
                return HttpNotFound();
            }

            return View(jcasCourtroom);
        }

        // POST: JcasCourtrooms/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        /// <summary>Allows users to save changes to a JcasCourtroom item.</summary>
        /// <param name="jcasCourtroom">Object that represents the updated JcasCourtroom item to save.</param>
        /// <returns>If there are validation errors, redisplays the form with the error messages, otherwise returns to the Index page.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Courtroom,UserLoginIdUpdate,RowVersionId")] JcasCourtroom jcasCourtroom)
        {
            if (ModelState.IsValid)
            {
                db.Entry(jcasCourtroom).State = EntityState.Modified;
                db.SaveChanges(ModelState);
                if (ModelState.IsValid)
                {
                    return RedirectToAction("Index");
                }
            }

            return View(jcasCourtroom);
        }

        // GET: JcasCourtrooms/Delete/5
        /// <summary>Displays the specified JcasCourtroom item prior to deleting.</summary>
        /// <param name="id">Identifies the JcasCourtroom item to display prior to deleting.</param>
        /// <returns>The specified JcasCourtroom item and asks the user to confirm that it should be deleted.</returns>
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            JcasCourtroom jcasCourtroom = db.JcasCourtrooms.Find(id);
            if (jcasCourtroom == null)
            {
                return HttpNotFound();
            }
            return View(jcasCourtroom);
        }

        // POST: JcasCourtrooms/Delete/5
        /// <summary>Deletes the specified JcasCourtroom item.</summary>
        /// <param name="id">Identifies the JcasCourtroom item to delete.</param>
        /// <returns>After deleting, returns to the Index page.</returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            JcasCourtroom jcasCourtroom = db.JcasCourtrooms.Find(id);
            db.JcasCourtrooms.Remove(jcasCourtroom);
            db.SaveChanges(ModelState);
            if (ModelState.IsValid)
            {
                return RedirectToAction("Index");
            }
            db.Entry(jcasCourtroom).State = EntityState.Unchanged;
            return View(jcasCourtroom);
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
