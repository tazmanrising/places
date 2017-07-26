using Calibrus.SparkPortal.Web.CustomAttributes;
using System.Web.Mvc;

namespace Calibrus.SparkPortal.Web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

		[CustomAccessAuthorize(AccessLevel = AccessLevel.OfficeAdministrator)]
		[ChildActionOnly]
		public ActionResult _Dashboard()
		{
			return PartialView();
		}

        [AllowAnonymous]
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        [AllowAnonymous]
		public ActionResult Logout()
        {
            Session.Abandon();
	        return RedirectToAction("Login", "Account");
        }

        [CustomAccessAuthorize(AccessLevel = AccessLevel.ClientAdministrator)]
        public ActionResult ClientAdmin()
        {
            ViewBag.Message = "Client Administrator Dashboard";

            return View();
        }

        [CustomAccessAuthorize(AccessLevel = AccessLevel.VendorAdministrator)]
        public ActionResult VendorAdmin()
        {
            ViewBag.Message = "Vendor Administrator Dashboard";

            return View();
        }

		[CustomAccessAuthorize(AccessLevel = AccessLevel.OfficeAdministrator)]
		public ActionResult OfficeAdmin()
		{
			ViewBag.Message = "Office Administrator Dashboard";

			return View();
		}
    }
}