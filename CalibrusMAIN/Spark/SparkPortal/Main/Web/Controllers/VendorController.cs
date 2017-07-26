using System.Linq;
using System.Net.Cache;
using Calibrus.SparkPortal.ViewModel;
using Calibrus.SparkPortal.Web.CustomAttributes;
using Microsoft.Ajax.Utilities;
using System.Web.Mvc;

namespace Calibrus.SparkPortal.Web.Controllers
{
    [CustomAccessAuthorize(AccessLevel = AccessLevel.VendorAdministrator)]
    public class VendorController : Controller
    {
        // GET: Vendor
        [CustomAccessAuthorize(AccessLevel = AccessLevel.ClientAdministrator)]
        public ActionResult Index()
        {
            return View();
        }

        [CustomAccessAuthorize(AccessLevel = AccessLevel.ClientAdministrator)]
        [ChildActionOnly]
        public ActionResult _Index()
        {
            return PartialView(new VendorIndexViewModel());
        }

        // GET: Vendor/Detail
        public ActionResult Detail(int id)
        {
			VendorViewModel vm = new VendorViewModel(id) { LoggedOnUser = SessionVars.UserName };

			//if (vm.OfficeList.All(x => x.Username != vm.LoggedOnUser))
			//{
			//	return RedirectToAction("AccessDenied", new { controller = "Error" });
			//}

            return View(vm);
        }

        // GET: Vendor/Create
        [CustomAccessAuthorize(AccessLevel = AccessLevel.ClientAdministrator)]
        public ActionResult Create()
        {
            SessionVars.ReturnUrl = Request.UrlReferrer.IfNotNull(u => u.AbsoluteUri, "//");
            return View(new VendorViewModel());
        }

        // POST: Vendor/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAccessAuthorize(AccessLevel = AccessLevel.ClientAdministrator)]
        public ActionResult Create(VendorViewModel vendor)
        {
            if (ModelState.IsValid)
            {
                vendor.LoggedOnUser = SessionVars.UserName;
                vendor.SaveViewModel();

				TempData["Success"] = "Vendor created successfully!";
                return RedirectToAction("Edit", new { controller = "Vendor", id = vendor.Id });
            }

            return View(vendor);
        }

        public ActionResult Edit(int id)
        {
	        SessionVars.AccessedVendorId = id;

	        VendorViewModel vm = new VendorViewModel(id) {LoggedOnUser = SessionVars.UserName};

	        if (!Calibrus.SparkPortal.Business.LoginLogic.IsClientAdmin(SessionVars.UserName) && vm.OfficeList.Any(o => o.Users.Any(x=>x.UserName != vm.LoggedOnUser))) 
			{
				return RedirectToAction("AccessDenied", new { controller = "Error" });
	        }

            return View(vm);
        }

        // POST: Vendor/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(VendorViewModel vendor)
        {
			if (!Calibrus.SparkPortal.Business.LoginLogic.IsClientAdmin(SessionVars.UserName) && vendor.OfficeList.Any(o => o.Users.Any(x=>x.UserName != SessionVars.UserName)))
			{
				return RedirectToAction("AccessDenied", new { controller = "Error" });
			}

            if (ModelState.IsValid)
            {
                vendor.LoggedOnUser = SessionVars.UserName;
                vendor.SaveViewModel();

				TempData["Success"] = "Program updated successfully!";
                return RedirectToAction("Edit", new { controller = "Vendor", id = vendor.Id });
            }

            return View(vendor);
        }
    }
}