using FairfaxCounty.JCAS_Public_MVC.Models;
using System;
using System.Data.Entity;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace FairfaxCounty.JCAS_Public_MVC.Helpers
{
    /// <summary>Custom membership provider for the application.</summary>
    public class ApplicationMembershipProvider : FairfaxCounty.ADProviders.ActiveDiretoryMembershipProvider
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>Authenticates the user.</summary>
        /// <param name="username">Name used by the application to identify the user.</param>
        /// <param name="password">Password for the user.</param>
        /// <returns>True if the user is a valid user who has not exceeded the max failed login attempts, 
        /// otherwise returns false and increments the failed login attempt count.</returns>
        public override bool ValidateUser(string username, string password)
        {
            // assume the login is not valid by default.
            bool blnValidLogin = false;

            using (JcasEntities db = new JcasEntities())
            {
                try
                {
                    //try to validate the user against active directory using the FairfaxCounty.ADProviders.ActiveDiretoryMembershipProvider
                    blnValidLogin = base.ValidateUser(username, password);
                    //if the user cannot be validated against active directory...
                    if (!blnValidLogin)
                    {
                        //get the max number of failed login attempts that is allowed.
                        int intMaxFailedLoginAttempts = Convert.ToInt32(Utility.GetSettingValue("MaxFailedLoginAttempts"));
                        //try to get the user's password from the Attorney table and decrypt it.
                        JcasAttorney user = db.JcasAttorneys.Where(p => p.EmailAddress == username).FirstOrDefault();
                        if (user != null)
                        {
                            // hash the password that was provided.
                            SHA384CryptoServiceProvider objProvider = new SHA384CryptoServiceProvider();
                            byte[] arrPassword = Encoding.ASCII.GetBytes(password);
                            arrPassword = objProvider.ComputeHash(arrPassword);

                            // this is a valid login if the user has not maxed out on failed login attempts and the password hashes match.
                            blnValidLogin = (user.FailedLoginAttempts <= intMaxFailedLoginAttempts && user.Password != null && user.Password.SequenceEqual(arrPassword));
                            // if this is not a valid login and the user has not maxed out on failed login attempts...
                            if (!blnValidLogin && user.FailedLoginAttempts <= intMaxFailedLoginAttempts)
                            {
                                // add a failed login attempt to the count.
                                user.FailedLoginAttempts = user.FailedLoginAttempts + 1;
                                db.Entry(user).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    // write the error message to the log.
                    log.Info("Error trying to validate user.", ex);
                    //if there's an error with the FairfaxCounty.ADProviders.ActiveDiretoryMembershipProvider, log the error, 
                    //but don't email the error to developers, because there are intermittent problems with the provider from E-Gov that resolve themselves.
                    //Microsoft.Practices.EnterpriseLibrary.ExceptionHandling.ExceptionPolicy.HandleException(ex, "Default")
                }
            }
            return blnValidLogin;
        }
    }
}