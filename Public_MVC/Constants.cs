using System;
using System.Collections.Specialized;
using System.Web.Configuration;

namespace FairfaxCounty.JCAS_Public_MVC
{
    /// <summary>Constants used throughout the application.</summary>
    public static class Constants
    {
        /// <summary>Collection of possible roles to assign to users.</summary>
        public static NameValueCollection InternalRolesCollection = new NameValueCollection()
        {
           { Constants.InternalRoleReadOnly, Constants.InternalRoleReadOnly },
           { Constants.InternalRoleDataEntry, Constants.InternalRoleDataEntry },
           { Constants.InternalRoleSysAdmin, Constants.InternalRoleSysAdmin }
        };
        /// <summary>Comma separated internal roles.</summary>
        public const string InternalRolesAll = "ReadOnly,DataEntry,SysAdmin";
        /// <summary>Internal readonly role.</summary>
        public const string InternalRoleReadOnly = "ReadOnly";
        /// <summary>Internal dataentry role.</summary>
        public const string InternalRoleDataEntry = "DataEntry";
        /// <summary>Internal sysadmin role.</summary>
        public const string InternalRoleSysAdmin = "SysAdmin";
        /// <summary>External attorney role.</summary>
        public const string ExternalRoleAttorney = "Attorney";
        /// <summary>Indicates the password is expired and needs to be changed.</summary>
        public const string RoleExpired = "Expired";

        /// <summary>Display as first option in lists.</summary>
        public const string SelectOne = "--select one--";

        /// <summary>Abbreviation of the application.</summary>
        public static string AppAbbr { get; private set; }
        /// <summary>Name of the application.</summary>
        public static string AppName { get; private set; }
        /// <summary>Region of the application.</summary>
        public static string AppRegion { get; private set; }
        /// <summary>Whether to display full header.</summary>
        public static bool Headerfull { get; private set; }
        /// <summary>Smtp server to use for emailing.</summary>
        public static string SmtpServer { get; private set; }
        /// <summary>Whether to display breadcrumb.</summary>
        public static bool UseAppBreadcrumb { get; private set; }

        static Constants()
        {
            AppAbbr = WebConfigurationManager.AppSettings["appAbbr"].ToString();
            AppName = WebConfigurationManager.AppSettings["appName"].ToString();
            AppRegion = WebConfigurationManager.AppSettings["appRegion"].ToString();
            Headerfull = Boolean.Parse(WebConfigurationManager.AppSettings["headerfull"]);
            SmtpServer = WebConfigurationManager.AppSettings["smtpServer"].ToString();
            UseAppBreadcrumb = Boolean.Parse(WebConfigurationManager.AppSettings["useAppBreadcrumb"]);
        }
    }
}
