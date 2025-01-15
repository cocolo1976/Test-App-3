using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FairfaxCounty.JCAS_Public_MVC.Models;

namespace FairfaxCounty.JCAS_Public_MVC.Helpers
{
    /// <summary>Custom role provider for the application.</summary>
    public class ApplicationRoleProvider : FairfaxCounty.ADProviders.ActiveDirectoryRoleProvider
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>Gets a list of all the roles for the configured applicationName.</summary>
        /// <returns>A string array containing the names of all the roles for the configured applicationName.</returns>
        public override string[] GetAllRoles()
        {
            return GetRolesForUser(HttpContext.Current.User.Identity.Name);
        }
        /// <summary>Gets a list of the roles that a specified user is in for the configured applicationName.</summary>
        /// <param name="username">The user to return a list of roles for.</param>
        /// <returns>A string array containing the names of all the roles that the specified user is in for the configured applicationName.</returns>
        public override string[] GetRolesForUser(string username)
        {
            List<string> roles = new List<string>();
            using (JcasEntities db = new JcasEntities())
            {
                try
                {
                    // get info for the current user from the database.
                    JcasUser user = db.JcasUsers.Where(p => p.UserLoginId == username).FirstOrDefault();
                    // if the user is found...
                    if (user != null)
                    {
                        switch (user.UserRole)
                        {
                            // SysAdmin users are automatically granted DataEntry, and ReadOnly, in addition.
                            case Constants.InternalRoleSysAdmin:
                                roles.Add(Constants.InternalRoleSysAdmin);
                                roles.Add(Constants.InternalRoleDataEntry);
                                roles.Add(Constants.InternalRoleReadOnly);
                                break;
                            // DataEntry users are automatically granted ReadOnly, in addition.
                            case Constants.InternalRoleDataEntry:
                                roles.Add(Constants.InternalRoleDataEntry);
                                roles.Add(Constants.InternalRoleReadOnly);
                                break;
                            // ReadOnly users are only granted ReadOnly.
                            case Constants.InternalRoleReadOnly:
                                roles.Add(Constants.InternalRoleReadOnly);
                                break;
                            default:
                                break;
                        }
                    }
                    else
                    {
                        //else, if this is an attorney
                        JcasAttorney attorney = db.JcasAttorneys.Where(p => p.EmailAddress == username).FirstOrDefault();
                        if (attorney != null)
                        {
                            roles.Add(Constants.ExternalRoleAttorney);
                            // if this is an external user with an expired password...
                            if (!attorney.PasswordExpires.HasValue || attorney.PasswordExpires.Value <= System.DateTime.Today)
                            {
                                // grant the expired role (which only allows the user to select a new password).
                                roles.Add(Constants.RoleExpired);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    log.Info("Error trying to obtain user roles.", ex);
                }
                return roles.ToArray();
            }
        }
    }
}