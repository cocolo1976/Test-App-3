using System;
using System.Data.Entity;
using System.Net;
using System.Web.Mvc;
using System.Collections.Generic;
using FairfaxCounty.JCAS_Public_MVC.Models;
namespace FairfaxCounty.JCAS_Public_MVC.Controllers
{
    /// <summary>Allows SysAdmin users to add/edit/delete the JcasRule items.</summary>
    [Authorize(Roles = Constants.InternalRoleSysAdmin)]
    public class JcasRulesController : Controller
    {
        private JcasEntities db = new JcasEntities();
     
        /// <summary>refreshes the datetype list.</summary>
        public void RefreshLists()
        {
            ViewBag.DateTypeList = new SelectList(db.JcasDateTypes, "Id", "DateTypeName");
        }

        // GET: JcasRuless
        /// <summary>Displays the list of Rules.</summary>
        public ActionResult Index()
        {
            IEnumerable<JcasRule> jcasRules = db.JcasRules.Include(a => a.JcasSubmitFromDateType)
                .Include(a => a.JcasSubmitToDateType).Include(a => a.JcasSessionFromDateType).Include(a => a.JcasSessionToDateType);
            return View(jcasRules);
        }

        // GET: JcasRules/Create
        /// <summary>Displays a blank form for entering a new JcasRule item.</summary>
        /// <returns>Blank form for entering a new JcasRule item.</returns>
        public ActionResult Create()
        {
            RefreshLists();
            return View();
        }

        // POST: JcasRules/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        /// <summary>Allows the user to save a new JcasRule item.</summary>
        /// <param name="jcasRule">Object that represents the new JcasRule item to save.</param>
        /// <returns>If there are validation errors, redisplays the form with the error messages, otherwise returns to the Index page.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,RuleName,RuleDescription,SubmitFromDateTypeId,SubmitFromSequence,SubmitToDateTypeId,SubmitToSequence,SessionMonthSequence,SessionFromDateTypeId,SessionFromSequence,SessionToDateTypeId,SessionToSequence,HearingTypeCount,UserGroup,UserLoginIdUpdate,RowVersionId")] JcasRule jcasRule)
        {
            if (ModelState.IsValid)
            {
                db.JcasRules.Add(jcasRule);
                db.SaveChanges(ModelState);
                if (ModelState.IsValid)
                {
                    return RedirectToAction("Index");
                }
            }
            RefreshLists();
            return View(jcasRule);
        }

        // GET: JcasRules/Edit/5
        /// <summary>Displays the specified JcasRule item for editing.</summary>
        /// <param name="id">Identifies the JcasRule item to edit.</param>
        /// <returns>The specified JcasRule item for editing.</returns>
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            JcasRule jcasRule = db.JcasRules.Find(id);
            if (jcasRule == null)
            {
                return HttpNotFound();
            }
            RefreshLists();
            return View(jcasRule);
        }

        // POST: JcasRules/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        /// <summary>Allows users to save changes to a JcasRule item.</summary>
        /// <param name="jcasRule">Object that represents the updated JcasRule item to save.</param>
        /// <returns>If there are validation errors, redisplays the form with the error messages, otherwise returns to the Index page.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,RuleName,RuleDescription,SubmitFromDateTypeId,SubmitFromSequence,SubmitToDateTypeId,SubmitToSequence,SessionMonthSequence,SessionFromDateTypeId,SessionFromSequence,SessionToDateTypeId,SessionToSequence,HearingTypeCount,UserGroup,UserLoginIdUpdate,RowVersionId")] JcasRule jcasRule)
        {
            if (ModelState.IsValid)
            {
                db.Entry(jcasRule).State = EntityState.Modified;
                db.SaveChanges(ModelState);
                if (ModelState.IsValid)
                {
                    return RedirectToAction("Index");
                }
            }
            RefreshLists();
            return View(jcasRule);
        }

        // GET: JcasRules/Delete/5
        /// <summary>Displays the specified JcasRule item prior to deleting.</summary>
        /// <param name="id">Identifies the JcasRule item to display prior to deleting.</param>
        /// <returns>The specified JcasRule item and asks the user to confirm that it should be deleted.</returns>
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            JcasRule jcasRule = db.JcasRules.Find(id);
            if (jcasRule == null)
            {
                return HttpNotFound();
            }
            return View(jcasRule);
        }

        // POST: JcasRules/Delete/5
        /// <summary>Deletes the specified JcasRule item.</summary>
        /// <param name="id">Identifies the JcasRule item to delete.</param>
        /// <returns>After deleting, returns to the Index page.</returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            JcasRule jcasRule = db.JcasRules.Find(id);
            db.JcasRules.Remove(jcasRule);
            db.SaveChanges(ModelState);
            if (ModelState.IsValid)
            {
                return RedirectToAction("Index");
            }
            db.Entry(jcasRule).State = EntityState.Unchanged;
            return View(jcasRule);
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
