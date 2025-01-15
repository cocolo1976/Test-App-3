using System;
using System.Data.Entity;
using System.Net;
using System.Web.Mvc;
using System.Collections.Generic;
using FairfaxCounty.JCAS_Public_MVC.Models;
namespace FairfaxCounty.JCAS_Public_MVC.Controllers
{
    /// <summary>Allows SysAdmin users to add/edit/delete the JcasMessage items.</summary>
    [Authorize(Roles = Constants.InternalRoleSysAdmin)]
    public class JcasMessagesController : Controller
    {
        private JcasEntities db = new JcasEntities();
        // GET: JcasMessagess
        /// <summary>Displays the list of Messages.</summary>
        public ActionResult Index()
        {
            IEnumerable<JcasMessage> jcasMessages = db.JcasMessages;
            return View(jcasMessages);
        }

        // GET: JcasMessages/Create
        /// <summary>Displays a blank form for entering a new JcasMessage item.</summary>
        /// <returns>Blank form for entering a new JcasMessage item.</returns>
        public ActionResult Create()
        {
            return View();
        }

        // POST: JcasMessages/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        /// <summary>Allows the user to save a new JcasMessage item.</summary>
        /// <param name="jcasMessage">Object that represents the new JcasMessage item to save.</param>
        /// <returns>If there are validation errors, redisplays the form with the error messages, otherwise returns to the Index page.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,MessageText,DisplayAsError,ApplicationAvailable,StartDateTime,EndDateTime,UserLoginIdUpdate,RowVersionId")] JcasMessage jcasMessage)
        {
            if (ModelState.IsValid)
            {
                db.JcasMessages.Add(jcasMessage);
                db.SaveChanges(ModelState);
                if (ModelState.IsValid)
                {
                    return RedirectToAction("Index");
                }
            }

            return View(jcasMessage);
        }

        // GET: JcasMessages/Edit/5
        /// <summary>Displays the specified JcasMessage item for editing.</summary>
        /// <param name="id">Identifies the JcasMessage item to edit.</param>
        /// <returns>The specified JcasMessage item for editing.</returns>
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            JcasMessage jcasMessage = db.JcasMessages.Find(id);
            if (jcasMessage == null)
            {
                return HttpNotFound();
            }

            return View(jcasMessage);
        }

        // POST: JcasMessages/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        /// <summary>Allows users to save changes to a JcasMessage item.</summary>
        /// <param name="jcasMessage">Object that represents the updated JcasMessage item to save.</param>
        /// <returns>If there are validation errors, redisplays the form with the error messages, otherwise returns to the Index page.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,MessageText,DisplayAsError,ApplicationAvailable,StartDateTime,EndDateTime,UserLoginIdUpdate,RowVersionId")] JcasMessage jcasMessage)
        {
            if (ModelState.IsValid)
            {
                db.Entry(jcasMessage).State = EntityState.Modified;
                db.SaveChanges(ModelState);
                if (ModelState.IsValid)
                {
                    return RedirectToAction("Index");
                }
            }

            return View(jcasMessage);
        }

        // GET: JcasMessages/Delete/5
        /// <summary>Displays the specified JcasMessage item prior to deleting.</summary>
        /// <param name="id">Identifies the JcasMessage item to display prior to deleting.</param>
        /// <returns>The specified JcasMessage item and asks the user to confirm that it should be deleted.</returns>
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            JcasMessage jcasMessage = db.JcasMessages.Find(id);
            if (jcasMessage == null)
            {
                return HttpNotFound();
            }
            return View(jcasMessage);
        }

        // POST: JcasMessages/Delete/5
        /// <summary>Deletes the specified JcasMessage item.</summary>
        /// <param name="id">Identifies the JcasMessage item to delete.</param>
        /// <returns>After deleting, returns to the Index page.</returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            JcasMessage jcasMessage = db.JcasMessages.Find(id);
            db.JcasMessages.Remove(jcasMessage);
            db.SaveChanges(ModelState);
            if (ModelState.IsValid)
            {
                return RedirectToAction("Index");
            }
            db.Entry(jcasMessage).State = EntityState.Unchanged;
            return View(jcasMessage);
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
