using log4net;
using System.Web.Mvc;

namespace FairfaxCounty.JCAS_Public_MVC.Helpers
{
    /// <summary>Helper to handle exceptions.</summary>
    public class HandleExceptionAttribute : HandleErrorAttribute
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>On exception, log the error.</summary>
        public override void OnException(ExceptionContext filterContext)
        {
            var e = filterContext.Exception;
            Log.Error(e.Message, e);
        }
    }
}
