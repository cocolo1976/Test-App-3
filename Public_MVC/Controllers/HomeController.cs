using FairfaxCounty.JCAS_Public_MVC.Models;
using System.Web.Mvc;

namespace FairfaxCounty.JCAS_Public_MVC.Controllers
{
    /// <summary>Displays the Home page of the application.</summary>
    [Authorize(Roles = Constants.InternalRoleReadOnly + "," + Constants.ExternalRoleAttorney)]
    public class HomeController : Controller
    {
        private JcasEntities db = new JcasEntities();

        /// <summary>Displays the Home page.</summary>
        [HttpGet]
        public ActionResult Index()
        {
            return RedirectToAction("Calendar", "JcasSessions");
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