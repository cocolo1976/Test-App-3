using FairfaxCounty.JCAS_Public_MVC.Models;
using FairfaxCounty.JCAS_Public_MVC.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace FairfaxCounty.JCAS_Public_MVC.Controllers
{
    /// <summary>Handles logging in and out of the application.</summary>
    public class AccountController : Controller
    {
        private JcasEntities db = new JcasEntities();

        //GET: Account/ForgotPassword
        /// <summary>Allows external user to reset password.</summary>
        /// <returns>Form that prompts for the user name to reset the password for.</returns>
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            //if there is an application message with application available equals false, display login screen
            DateTime currentDateTime = DateTime.Now;
            JcasMessage message = db.JcasMessages.Where(m => m.StartDateTime <= currentDateTime && m.EndDateTime >= currentDateTime
                            && m.ApplicationAvailable == false).FirstOrDefault();
            if (message != null)
            {
                return RedirectToAction("Login");
            }
            ForgotPasswordViewModel model = new ForgotPasswordViewModel();
            return View(model);
        }

        //POST: Account/ForgotPassword
        /// <summary>Allows external user to reset password.</summary>
        /// <param name="model">Object that holds the information about the password to reset.</param>
        /// <returns>If there are validation errors, redisplays the form with the error messages, 
        /// otherwise emails the new temporary password to the user and returns to the Login page.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public ActionResult ForgotPassword(ForgotPasswordViewModel model)
        {
            // if the page passes validation...
            if (ModelState.IsValid)
            {
                // get the user information
                JcasAttorney attorney = db.JcasAttorneys.Where(p => p.EmailAddress == model.EmailAddress).FirstOrDefault();
                // if User Login Id belongs to an internal FFX user, display message
                if (attorney == null)
                {
                    ModelState.AddModelError(string.Empty, "Attorney not found.");
                }
                //otherwise, generate an email confirmation code.
                else
                {
                    //if not active, set the error
                    if (!attorney.IsActive)
                    {
                        ModelState.AddModelError("", "This attorney currently has inactive status.  Please contact the JCAS system administrators for assistance.");
                    }
                    else
                    {
                        // generate a new confirmation code.
                        string strCode = Helpers.Utility.RandomCode(64);

                        // hash the new code.
                        SHA384CryptoServiceProvider objProvider = new SHA384CryptoServiceProvider();
                        byte[] arrCode = Encoding.ASCII.GetBytes(strCode);
                        arrCode = objProvider.ComputeHash(arrCode);

                        // store the new code.
                        attorney.EmailConfirmationCode = arrCode;
                        db.Entry(attorney).State = EntityState.Modified;
                        db.SaveChanges(ModelState);

                        // if there are no errors...
                        if (ModelState.IsValid)
                        {
                            // send the new email confirmation link to the user.
                            Helpers.Utility.ForgotPassword(db, attorney.EmailAddress, strCode);

                            // redirect to the login page
                            return RedirectToAction("Login");
                        }
                    }
                }
            }
            return View(model);
        }

        //GET: /ChangePassword
        /// <summary>When password has expired or an email confirmation link has been sent, allows user to set the new password.</summary>
        /// <param name="emailCode">If coming from an email confirmation link, the randomly generated confirmation code included in the link.</param>
        /// <returns>If there is no email code and the current user's password has not expired, redirects to logout.
        /// Otherwise, displays form for changing the password.</returns>
        [AllowAnonymous]
        public ActionResult ChangePassword(string emailCode)
        {
            //if there is an application message with application available equals false, display login screen
            DateTime currentDateTime = DateTime.Now;
            JcasMessage message = db.JcasMessages.Where(m => m.StartDateTime <= currentDateTime && m.EndDateTime >= currentDateTime
                            && m.ApplicationAvailable == false).FirstOrDefault();
            if (message != null)
            {
                return RedirectToAction("Login");
            }
            ChangePasswordViewModel model = new ChangePasswordViewModel(db);
            //if already logged in  or the user's password has expired, get the user login Id for the current user.
            if (User.IsInRole(Constants.ExternalRoleAttorney) || User.IsInRole(Constants.RoleExpired))
            {
                model.EmailAddress = User.Identity.Name.ToString();
            }
            else
            {
                // if there is no email code, redirect to the logout page.
                if (string.IsNullOrEmpty(emailCode))
                {
                    return RedirectToAction("Logout");
                }
                // get the email code from the query string (if any).
                model.EmailConfirmationCode = emailCode;
            }

            // display the change password form (if you have either the email code or the user login id)
            return View(model);
        }

        //POST: /ChangePassword
        /// <summary>When password has expired or an email confirmation link has been sent, allows user to set the new password.</summary>
        /// <param name="model">Object that represents the new password info entered by the user.</param>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public ActionResult ChangePassword(ChangePasswordViewModel model)
        {
            SHA384CryptoServiceProvider objProvider = new SHA384CryptoServiceProvider();
            JcasAttorney attorney = null;
            // if there are no validation errors, and the use is already logged in...
            if (ModelState.IsValid && !User.IsInRole(Constants.ExternalRoleAttorney))
            {
                // if the user is coming from an email confirmation link...
                if (!User.IsInRole(Constants.RoleExpired))
                {
                    // hash the email confirmation code included in the link.
                    byte[] arrCode = Encoding.ASCII.GetBytes(model.EmailConfirmationCode);
                    arrCode = objProvider.ComputeHash(arrCode);

                    // get the user information for the email address that was entered.
                    attorney = db.JcasAttorneys.Where(p => p.EmailAddress == model.EmailAddress).FirstOrDefault();

                    // if the confirmation code is not correct for the user (or the user shouldn't have a confirmation code), display message.
                    if (attorney == null || attorney.EmailIsConfirmed || attorney.EmailConfirmationCode == null || !attorney.EmailConfirmationCode.SequenceEqual(arrCode))
                    {
                        ModelState.AddModelError("", "The Email address and Email Confirmation Code do not match.  Please use the confirmation link from the most recent email and make sure that you have entered the Email address correctly below.");
                    }
                    else
                    {
                        // if the confirmation code has expired...
                        if (!attorney.EmailConfirmationExpires.HasValue || attorney.EmailConfirmationExpires.Value < System.DateTime.Now)
                        {
                            // generate a new confirmation code.
                            string strCode = Helpers.Utility.RandomCode(64);

                            // hash the new code.
                            arrCode = Encoding.ASCII.GetBytes(strCode);
                            arrCode = objProvider.ComputeHash(arrCode);

                            // store the new code.  The expiration date will be inserted by the trigger JcasAttorney_SetExpiration_InsUpdtrg
                            attorney.EmailConfirmationCode = arrCode;
                            db.Entry(attorney).State = EntityState.Modified;
                            db.SaveChanges(ModelState);

                            // if there are no errors...
                            if (ModelState.IsValid)
                            {
                                // send the new email confirmation link to the user.
                                Helpers.Utility.EmailConfirmationCode(db, attorney.EmailAddress, strCode);

                                // display message.
                                ModelState.AddModelError("", "The Email Confirmation Code has expired.  A new confirmation email has been sent.  Please use the confirmation link from the most recent email.");
                            }
                        }
                    }
                }
            }
            // if the confirmation code matches and hasn't expired, or the user logged in with an expired password and is being forced to select a new password...
            if (ModelState.IsValid)
            {
                // if the user logged in with an expired password, set the user name to the current user, regardless of what was entered as the email address.
                if (User.IsInRole(Constants.RoleExpired))
                {
                    model.EmailAddress = User.Identity.Name.ToString();
                }

                // hash the new password.
                byte[] arrPassword = Encoding.ASCII.GetBytes(model.NewPassword);
                arrPassword = objProvider.ComputeHash(arrPassword);

                // get the user information.
                if (attorney == null)
                {
                    attorney = db.JcasAttorneys.Where(p => p.EmailAddress == model.EmailAddress).FirstOrDefault();
                }

                // if the new password is the same as last time, add message.
                if (attorney.Password != null && attorney.Password.SequenceEqual(arrPassword))
                {
                    ModelState.AddModelError("", "Selected password was previously used. Please select a new password.");
                }
                // otherwise...
                else
                {
                    // store the new password.
                    attorney.Password = arrPassword;
                    db.Entry(attorney).State = EntityState.Modified;
                    db.SaveChanges(ModelState);
                }
            }

            // if there are no errors...
            if (ModelState.IsValid)
            {
                // force the app to get the roles again on the next request (this will get rid of the Expired role).
                Roles.DeleteCookie();

                // authenticate the user using the user name and new password and redirect to the index.
                if (Membership.ValidateUser(model.EmailAddress, model.NewPassword))
                {
                    FormsAuthentication.SetAuthCookie(model.EmailAddress, false);
                    return RedirectToAction("Index", "Home");
                }
            }
            // if there are error messages, redisplay the form with the error messages.
            model.RefreshLookups(db);
            return View(model);
        }

        // GET: Account/Login
        /// <summary>Allows the user to login to the application.</summary>
        /// <param name="returnUrl">If specified, this is the url to go to after successful login.</param>
        /// <returns>Form that prompts for the user name and password.</returns>
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            // if user is already logged in...
            if (User.Identity.IsAuthenticated)
            {
                // redirect to log the user out.
                return RedirectToAction("Logout");
            }

            if (!string.IsNullOrEmpty(returnUrl) && returnUrl.Contains("Logout"))
            {
                returnUrl = null;
            }

            // display the login form.
            ViewBag.ReturnUrl = returnUrl;
            LoginViewModel model = new LoginViewModel(db);

            //send scheduling reminder email
            model.EmailSchedulingReminder(db, ModelState);
            return View(model);
        }

        // POST: Account/Login
        /// <summary>Allows the user to login to the application.</summary>
        /// <param name="model">Object that represents the login information.</param>
        /// <param name="returnUrl">If specified, this is the url to go to after successful login.</param>
        /// <returns>If there are validation errors, redisplays the form with the error messages, 
        /// otherwise if a returnUrl is specified, returns to that url
        /// otherwise, returns to the Home page for the application.</returns>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model, string returnUrl)
        {
            // if there are no validation errors...
            if (ModelState.IsValid)
            {
                // if able to authenticate the user...
                if (Membership.ValidateUser(model.UserLoginId, model.Password))
                {
                    FormsAuthentication.SetAuthCookie(model.UserLoginId, model.RememberMe);

                    JcasUser user = db.JcasUsers.Where(u => u.UserLoginId == model.UserLoginId && !string.IsNullOrEmpty(u.UserRole)).FirstOrDefault();
                    JcasAttorney attorney = db.JcasAttorneys.Where(p => p.EmailAddress == model.UserLoginId).FirstOrDefault();
                    //if username cannot be found in both table user and table attorney
                    if (attorney == null && user == null)
                    {
                        ViewBag.ReturnUrl = returnUrl;
                        ModelState.AddModelError("", "User not found.  Please contact the JCAS system administrators for assistance.");
                        model.RefreshLookups(db);
                        return View(model);
                    }
                    else
                    {
                        // if user is in the attorney table and active
                        if (attorney != null)
                        {
                            if (Convert.ToBoolean(attorney.IsActive) == true)
                            {
                                // reset the failed attempts count for the user to 0
                                attorney.FailedLoginAttempts = 0;
                                db.Entry(attorney).State = EntityState.Modified;
                                db.SaveChanges(ModelState);
                                // if the user's password has expired,
                                // then redirect to the Change Password page.
                                if (!attorney.PasswordExpires.HasValue || attorney.PasswordExpires.Value <= DateTime.Today)
                                {
                                    return RedirectToAction("ChangePassword");
                                }
                            }
                            //else, currently inactive
                            else
                            {
                                ViewBag.ReturnUrl = returnUrl;
                                ModelState.AddModelError("", "Attorney currently has inactive status.  Please contact the JCAS system administrators for assistance.");
                                model.RefreshLookups(db);
                                return View(model);
                            }
                        }
                    }
                    // if a returnUrl was specified,
                    // redirect to the returnUrl.
                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                    && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                    {
                        return Redirect(returnUrl);
                    }
                    // if a returnUrl was not specified, go to the Session page.
                    else
                    {
                        return RedirectToAction("Calendar", "JcasSessions");
                    }
                }
                // if not able to authenticate the user, add error message.
                else
                {
                    ViewBag.ReturnUrl = returnUrl;
                    ModelState.AddModelError("", "The user name or password provided is incorrect.");
                }
            }
            // if there were errors, display messages.
            model.RefreshLookups(db);
            return View(model);
        }

        // GET: Account/Logout
        /// <summary>Allows the user to log out of the application.</summary>
        /// <returns>Clears the session and authentication and redirects to the Login page.</returns>
        [AllowAnonymous]
        public ActionResult Logout()
        {
            Session.Abandon();
            Session.Clear();
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
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
