using System;
using System.Data.Entity;
using System.Net;
using System.Web.Mvc;
using System.Collections.Generic;
using FairfaxCounty.JCAS_Public_MVC.Models;
using System.Linq;

namespace FairfaxCounty.JCAS_Public_MVC.Controllers
{
    /// <summary>Allows SysAdmin users to add/edit/delete the JcasSetting items.</summary>
    [Authorize(Roles = Constants.InternalRoleSysAdmin)]
    public class JcasSettingsController : Controller
    {
        private JcasEntities db = new JcasEntities();
        // GET: JcasSettingss
        /// <summary>Displays the list of Settings.</summary>
        public ActionResult Index()
        {
            IEnumerable<JcasSetting> jcasSettings = db.JcasSettings.Where(s => s.SettingVisible == true);
            return View(jcasSettings);
        }
        
        // GET: JcasSettings/Edit/5
        /// <summary>Displays the specified JcasSetting item for editing.</summary>
        /// <param name="id">Identifies the JcasSetting item to edit.</param>
        /// <returns>The specified JcasSetting item for editing.</returns>
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            JcasSetting jcasSetting = db.JcasSettings.Find(id);
            if (jcasSetting == null)
            {
                return HttpNotFound();
            }
            return View(jcasSetting);
        }

        // POST: JcasSettings/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        /// <summary>Allows users to save changes to a JcasSetting item.</summary>
        /// <param name="jcasSetting">Object that represents the updated JcasSetting item to save.</param>
        /// <returns>If there are validation errors, redisplays the form with the error messages, otherwise returns to the Index page.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,SettingName,SettingValue,SettingLabelText,SettingDataType,SettingMaxLength,SettingRequired,SettingTooltip,SettingOrder,SettingVisible,UserLoginIdUpdate,RowVersionId")] JcasSetting jcasSetting)
        {
            if (ModelState.IsValid)
            {
                db.Entry(jcasSetting).State = EntityState.Modified;
                db.SaveChanges(ModelState);
                if (ModelState.IsValid)
                {
                    return RedirectToAction("Index");
                }
            }
            return View(jcasSetting);
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
