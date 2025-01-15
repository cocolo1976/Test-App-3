using System;
using System.Data.Entity;
using System.Net;
using System.Web.Mvc;
using System.Collections.Generic;
using FairfaxCounty.JCAS_Public_MVC.Models;
namespace FairfaxCounty.JCAS_Public_MVC.Controllers
{
    /// <summary>Allows SysAdmin users to add/edit/delete the JcasUser items.</summary>
    [Authorize(Roles = Constants.InternalRoleSysAdmin)]
    public class JcasUsersController : Controller
    {
        private JcasEntities db = new JcasEntities();

        // GET: JcasUserss
        /// <summary>Displays the list of Users.</summary>
        /// <param name="filter">If specified, only Users that contain this text are included in the list.</param>
        /// <param name="orderBy">If specified, the list is sorted in this order.</param>
        /// <param name="page">The number of the page within the list to display.</param>
        /// <param name="pageSize">The number of records per page.</param>
        /// <returns>The filtered, sorted, paged list of Users.</returns>
        public ActionResult Index(string filter, string orderBy, int page = 1, int pageSize = 25)
        {
            IEnumerable<JcasUser> jcasUsers = db.JcasUsers;
            jcasUsers = JcasUser.FilteredSortedPagedList(db, ViewBag, filter, orderBy, page, pageSize);
            int totalPages = Convert.ToInt32(ViewBag.TotalPages);
            if (totalPages > 0)
            {
                if (page < 1)
                {
                    return RedirectToAction("Index", "JcasUsers", new { filter = filter, orderBy = orderBy, page = 1, pageSize = pageSize });
                }
                if (page > totalPages)
                {
                    return RedirectToAction("Index", "JcasUsers", new { filter = filter, orderBy = orderBy, page = totalPages, pageSize = pageSize });
                }
            }
            return View(jcasUsers);
        }


        // GET: JcasUsers/Create
        /// <summary>Displays a blank form for entering a new JcasUser item.</summary>
        /// <returns>Blank form for entering a new JcasUser item.</returns>
        public ActionResult Create()
        {
            return View();
        }

        // POST: JcasUsers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        /// <summary>Allows the user to save a new JcasUser item.</summary>
        /// <param name="jcasUser">Object that represents the new JcasUser item to save.</param>
        /// <returns>If there are validation errors, redisplays the form with the error messages, otherwise returns to the Index page.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,UserLoginId,UserName,UserRole,UserLoginIdUpdate,RowVersionId")] JcasUser jcasUser)
        {
            if (ModelState.IsValid)
            {
                db.JcasUsers.Add(jcasUser);
                db.SaveChanges(ModelState);
                if (ModelState.IsValid)
                {
                    return RedirectToAction("Index");
                }
            }
            return View(jcasUser);
        }

        // GET: JcasUsers/Edit/5
        /// <summary>Displays the specified JcasUser item for editing.</summary>
        /// <param name="id">Identifies the JcasUser item to edit.</param>
        /// <returns>The specified JcasUser item for editing.</returns>
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            JcasUser jcasUser = db.JcasUsers.Find(id);
            if (jcasUser == null)
            {
                return HttpNotFound();
            }
            return View(jcasUser);
        }

        // POST: JcasUsers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        /// <summary>Allows users to save changes to a JcasUser item.</summary>
        /// <param name="jcasUser">Object that represents the updated JcasUser item to save.</param>
        /// <returns>If there are validation errors, redisplays the form with the error messages, otherwise returns to the Index page.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,UserLoginId,UserName,UserRole,UserLoginIdUpdate,RowVersionId")] JcasUser jcasUser)
        {
            if (ModelState.IsValid)
            {
                db.Entry(jcasUser).State = EntityState.Modified;
                db.SaveChanges(ModelState);
                if (ModelState.IsValid)
                {
                    return RedirectToAction("Index");
                }
            }
            return View(jcasUser);
        }

        // GET: JcasUsers/Delete/5
        /// <summary>Displays the specified JcasUser item prior to deleting.</summary>
        /// <param name="id">Identifies the JcasUser item to display prior to deleting.</param>
        /// <returns>The specified JcasUser item and asks the user to confirm that it should be deleted.</returns>
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            JcasUser jcasUser = db.JcasUsers.Find(id);
            if (jcasUser == null)
            {
                return HttpNotFound();
            }
            return View(jcasUser);
        }

        // POST: JcasUsers/Delete/5
        /// <summary>Deletes the specified JcasUser item.</summary>
        /// <param name="id">Identifies the JcasUser item to delete.</param>
        /// <returns>After deleting, returns to the Index page.</returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            JcasUser jcasUser = db.JcasUsers.Find(id);
            db.JcasUsers.Remove(jcasUser);
            db.SaveChanges(ModelState);
            if (ModelState.IsValid)
            {
                return RedirectToAction("Index");
            }
            db.Entry(jcasUser).State = EntityState.Unchanged;
            return View(jcasUser);
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
