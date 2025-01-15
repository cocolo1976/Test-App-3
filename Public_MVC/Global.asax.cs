using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;

[assembly: log4net.Config.XmlConfigurator(ConfigFile = "Web.config", Watch = true)]
namespace FairfaxCounty.JCAS_Public_MVC
{
    /// <summary>Defines the methods, properties, and events that are common to all application objects in this application.</summary>
    public class MvcApplication : System.Web.HttpApplication
    {
        /// <summary>Register configurations when starting the application.</summary>
        protected void Application_Start()
        {
            log4net.Config.XmlConfigurator.Configure();
            RouteTable.Routes.MapMvcAttributeRoutes();
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AntiForgeryConfig.SuppressXFrameOptionsHeader = true;
        }

        /// <summary>Redirect all requests to SSL, if required.</summary>
        protected void Application_BeginRequest()
        {
            if (FormsAuthentication.RequireSSL && !Request.IsSecureConnection)
            {
                Response.Redirect(Request.Url.AbsoluteUri.Replace("http://", "https://"));
            }
        }
    }
}
