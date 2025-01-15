using FairfaxCounty.JCAS_Public_MVC.Helpers;
using System.Web;
using System.Web.Mvc;

namespace FairfaxCounty.JCAS_Public_MVC
{
    /// <summary>Configuration for global filters.</summary>
    public class FilterConfig
    {
        /// <summary>Register application filters.</summary>
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new HandleExceptionAttribute());
        }
    }
}
