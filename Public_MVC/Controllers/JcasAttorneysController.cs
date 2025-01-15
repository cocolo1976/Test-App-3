using System;
using System.Net;
using System.Web.Mvc;
using System.Collections.Generic;
using FairfaxCounty.JCAS_Public_MVC.Models;
using FairfaxCounty.JCAS_Public_MVC.ViewModels;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Data.Entity;

namespace FairfaxCounty.JCAS_Public_MVC.Controllers
{
    /// <summary>Allows SysAdmin and DataEntry users to add/edit/delete the JcasAttorney items.</summary>
    public class JcasAttorneysController : Controller
    {
        private JcasEntities db = new JcasEntities();

        // GET: JcasAttorneys
        /// <summary>Displays the list of Attorneys.</summary>
        /// <param name="filter">If specified, only Attorneys that contain this text are included in the list.</param>
        /// <param name="orderBy">If specified, the list is sorted in this order.</param>
        /// <param name="page">The number of the page within the list to display.</param>
        /// <param name="pageSize">The number of records per page.</param>
        /// <returns>The filtered, sorted, paged list of Attorneys.</returns>
        [Authorize(Roles = Constants.InternalRoleReadOnly)]
        public ActionResult Index(string filter, string orderBy, int page = 1, int pageSize = 25)
        {
            IEnumerable<JcasAttorney> jcasAttorneys = db.JcasAttorneys;
            jcasAttorneys = JcasAttorney.FilteredSortedPagedList(db, ViewBag, filter, orderBy, page, pageSize);
            int totalPages = Convert.ToInt32(ViewBag.TotalPages);
            if (totalPages > 0)
            {
                if (page < 1)
                {
                    return RedirectToAction("Index", "JcasAttorneys", new { filter, orderBy, page = 1, pageSize });
                }
                if (page > totalPages)
                {
                    return RedirectToAction("Index", "JcasAttorneys", new { filter, orderBy, page = totalPages, pageSize });
                }
            }
            return View(jcasAttorneys);
        }

        // GET: JcasAttorneys/Details
        /// <summary>Displays the detail information about the selected attorney.</summary>
        [Authorize(Roles = Constants.InternalRoleReadOnly)]
        public ActionResult Details(int? id)
        {
            if (id.HasValue)
            {
                JcasAttorney attorney = db.JcasAttorneys.Find(id);
                if (attorney != null)
                {
                    return View(db.JcasAttorneys.Find(id));
                }
                else
                {
                    return HttpNotFound();
                }
            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }

        // GET: JcasAttorneys/Create
        /// <summary>Displays a blank form for entering a new JcasAttorney item.</summary>
        /// <returns>Blank form for entering a new JcasAttorney item.</returns>
        [AllowAnonymous]
        public ActionResult Create()
        {
            //if there is an application message with application available equals false, display login screen
            DateTime currentDateTime = DateTime.Now;
            JcasMessage message = db.JcasMessages.Where(m => m.StartDateTime <= currentDateTime && m.EndDateTime >= currentDateTime
                            && m.ApplicationAvailable == false).FirstOrDefault();
            if (message != null)
            {
                return RedirectToAction("Login");
            }
            AttorneyHearingTypeViewModel attorneyHearingTypeViewModel = new AttorneyHearingTypeViewModel(db, null);
            return View(attorneyHearingTypeViewModel);
        }

        // POST: JcasAttorneys/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        /// <summary>Allows the user to save a new JcasAttorney item.</summary>
        /// <param name="model">Object that represents the new JcasAttorney item to save.</param>
        /// <returns>If there are validation errors, redisplays the form with the error messages, otherwise displays success message.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public ActionResult Create(AttorneyHearingTypeViewModel model)
        {
            if (ModelState.IsValid)
            {
                model.Attorney.IsActive = false;
                try
                {
                    db.JcasAttorneys.Add(model.Attorney);
                    db.SaveChanges(ModelState);
                    if (ModelState.IsValid)
                    {
                        model.UpdateAttorneyHearingTypes(db, ModelState);
                        if (ModelState.IsValid)
                        {
                            Helpers.Utility.EmailRegistrationReceipt(db, model.Attorney.EmailAddress);
                            Helpers.Utility.EmailRegistrationApprovalNeeded(db, model.Attorney.Id);
                            ViewBag.MessageText = "The new attorney user has been created, pending the Court's approval.  Please check your email for registration receipt.";
                            model = new AttorneyHearingTypeViewModel(db, model.Attorney.Id);
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
            }
            model.RefreshLookups(db);
            return View(model);
        }

        // GET: JcasAttorneys/Edit/5
        /// <summary>Displays the specified JcasAttorney item for editing.</summary>
        /// <param name="id">Identifies the JcasAttorney item to edit.</param>
        /// <returns>The specified JcasAttorney item for editing.</returns>
        [Authorize(Roles = Constants.InternalRoleDataEntry + "," + Constants.ExternalRoleAttorney)]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                if (User.IsInRole(Constants.ExternalRoleAttorney))
                {
                    id = db.JcasAttorneys.Where(a => a.EmailAddress == User.Identity.Name.ToString()).Select(a => a.Id).FirstOrDefault();
                }
                else
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
            }
            AttorneyHearingTypeViewModel attorneyHearingTypeViewModel = new AttorneyHearingTypeViewModel(db, id);
            return View(attorneyHearingTypeViewModel);
        }

        // POST: JcasAttorneys/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        /// <summary>Allows users to save changes to a JcasAttorney item.</summary>
        /// <param name="model">Object that represents the updated JcasAttorney item to save.</param>
        /// <returns>If there are validation errors, redisplays the form with the error messages, otherwise returns to the Index page.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Constants.InternalRoleDataEntry + "," + Constants.ExternalRoleAttorney)]
        public ActionResult Edit(AttorneyHearingTypeViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    //if the attorney has at least one scheduled session with one of the unchecked hearing type(s), display error
                    string selectedHearingTypes = "," + model.AttorneyHearingTypes + ",";
                    JcasSession sessions = db.JcasSessions.Where(s => s.AttorneyId == model.Attorney.Id
                        && s.SessionDate >= DateTime.Today.Date
                        && !(selectedHearingTypes.Contains("," + s.HearingTypeId.ToString() + ","))).FirstOrDefault();
                    if (sessions != null)
                    {
                        ModelState.AddModelError("", "The attorney currently has at least one future scheduled session for one of the unchecked hearing type(s).  Please unschedule those session(s) before updating the attorney hearing types.");
                    }
                    else
                    {
                        JcasAttorney attorneyToUpdate = db.JcasAttorneys.Find(model.Attorney.Id);
                        if (attorneyToUpdate != null)
                        {
                            attorneyToUpdate.LastName = model.Attorney.LastName;
                            attorneyToUpdate.FirstName = model.Attorney.FirstName;
                            attorneyToUpdate.MiddleName = model.Attorney.MiddleName;
                            attorneyToUpdate.BarNumber = model.Attorney.BarNumber;
                            attorneyToUpdate.EmailAddress = model.Attorney.EmailAddress;
                            attorneyToUpdate.PhoneNumber = model.Attorney.PhoneNumber;
                            attorneyToUpdate.LawFirmName = model.Attorney.LawFirmName;
                            //if the attorney status is changed from inactive to active
                            // setup the email confirmation code;
                            string emailConfirmationCode = Helpers.Utility.RandomCode(64);
                            bool activated = false;
                            if (attorneyToUpdate.IsActive == false && model.Attorney.IsActive == true)
                            {
                                activated = true;
                                // hash the code.
                                SHA384CryptoServiceProvider objProvider = new SHA384CryptoServiceProvider();
                                byte[] arrEmailConfirmationCode = Encoding.ASCII.GetBytes(emailConfirmationCode);
                                arrEmailConfirmationCode = objProvider.ComputeHash(arrEmailConfirmationCode);
                                // add the hash to the model so it is stored in the database.
                                // the expiration date will be inserted by the trigger JcasAttorney_SetExpiration_InsUpdtrg
                                attorneyToUpdate.EmailConfirmationCode = arrEmailConfirmationCode;
                            }
                            attorneyToUpdate.IsActive = model.Attorney.IsActive;
                            db.Entry(attorneyToUpdate).State = EntityState.Modified;
                            db.SaveChanges(ModelState);
                            //update the hearing types
                            if (ModelState.IsValid)
                            {
                                model.UpdateAttorneyHearingTypes(db, ModelState);
                                if (ModelState.IsValid)
                                {
                                    if (activated == true)
                                    {
                                        Helpers.Utility.EmailConfirmationCode(db, model.Attorney.EmailAddress, emailConfirmationCode);
                                    }
                                    ViewBag.MessageText = "Attorney updated successful.";
                                    model = new AttorneyHearingTypeViewModel(db, model.Attorney.Id);
                                }
                            }
                            else
                            {
                                model.RefreshLookups(db);
                                return View();
                            }
                        }
                    }
                    //}
                }
                catch (Exception ex)
                {
                    // if there is an error, add it to the Model State.
                    string message = JcasDescription.GetDatabaseMessage(db, ex);
                    if (string.IsNullOrEmpty(message))
                    {
                        message = ex.Message.ToString();
                    }
                    ModelState.AddModelError("", message);
                }
            }
            model.RefreshLookups(db);
            return View(model);
        }

        // GET: JcasAttorneys/Delete/5
        /// <summary>Displays the specified JcasAttorney item prior to deleting.</summary>
        /// <param name="id">Identifies the JcasAttorney item to display prior to deleting.</param>
        /// <returns>The specified JcasAttorney item and asks the user to confirm that it should be deleted.</returns>
        [Authorize(Roles = Constants.InternalRoleDataEntry)]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AttorneyHearingTypeViewModel attorneyHearingTypeViewModel = new AttorneyHearingTypeViewModel(db, id);
            if (attorneyHearingTypeViewModel == null)
            {
                return HttpNotFound();
            }
            return View(attorneyHearingTypeViewModel);
        }

        // POST: JcasAttorneys/Delete/5
        /// <summary>Deletes the specified JcasAttorney item.</summary>
        /// <param name="id">Identifies the JcasAttorney item to delete.</param>
        /// <returns>After deleting, returns to the Index page.</returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Constants.InternalRoleDataEntry)]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                //make sure the attorney is not scheduled for any session
                JcasSession sessions = db.JcasSessions.Where(s => s.AttorneyId == id).FirstOrDefault();
                if (sessions != null)
                {
                    ModelState.AddModelError("", "The attorney is or has been scheduled for at least one court session. Please delete upcoming sessions and make the attorney inactive.");
                }
                else
                {
                    JcasAttorney jcasAttorney = db.JcasAttorneys.Find(id);
                    db.JcasAttorneys.Remove(jcasAttorney);
                    db.SaveChanges(ModelState);
                    if (ModelState.IsValid)
                    {
                        ViewBag.MessageText = "Attorney has been deleted successfully.";
                        return View();
                    }
                }
            }
            catch (Exception ex)
            {
                // if there is an error, add it to the Model State.
                string message = JcasDescription.GetDatabaseMessage(db, ex);
                if (string.IsNullOrEmpty(message))
                {
                    message = ex.Message.ToString();
                }
                ModelState.AddModelError("", message);
            }
            AttorneyHearingTypeViewModel attorneyHearingTypeViewModel = new AttorneyHearingTypeViewModel(db, id);
            return View(attorneyHearingTypeViewModel);
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
