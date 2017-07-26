using System.Linq;
using Calibrus.ClearviewPortal.ViewModel;
using Calibrus.ClearviewPortal.Web.CustomAttributes;
using Microsoft.Ajax.Utilities;
using System.Web.Mvc;

namespace Calibrus.ClearviewPortal.Web.Controllers
{
	[CustomAccessAuthorize(AccessLevel = AccessLevel.VendorAdministrator)]
	public class OfficeController : Controller
	{
		// GET: Office
		[CustomAccessAuthorize(AccessLevel = AccessLevel.ClientAdministrator)]
		public ActionResult Index()
		{
			return View();
		}

		[CustomAccessAuthorize(AccessLevel = AccessLevel.VendorAdministrator)]
		[ChildActionOnly]
		public ActionResult _Index(int? id)
		{
			return id.HasValue ? PartialView(new OfficeIndexViewModel(id.Value)) : PartialView(new OfficeIndexViewModel());
		}

		// GET: Office/Detail
		public ActionResult Detail(int id)
		{
			OfficeViewModel vm = new OfficeViewModel(id) { LoggedOnUser = SessionVars.UserName };

			if (vm.UserList.All(x => x.Username != vm.LoggedOnUser))
			{
				return RedirectToAction("AccessDenied", new { controller = "Error" });
			}

			return View(vm);
		}

		// GET: Office/Create
		[CustomAccessAuthorize(AccessLevel = AccessLevel.VendorAdministrator)]
		public ActionResult Create(int? id)
		{
			SessionVars.ReturnUrl = Request.UrlReferrer.IfNotNull(u => u.AbsoluteUri, "//");
			return View(new OfficeViewModel(id, SessionVars.UserName));
		}

		// POST: Office/Create
		[HttpPost]
		[ValidateAntiForgeryToken]
		[CustomAccessAuthorize(AccessLevel = AccessLevel.VendorAdministrator)]
		public ActionResult Create(OfficeViewModel office)
		{
			if (ModelState.IsValid)
			{
				office.Id = null;
				office.LoggedOnUser = SessionVars.UserName;
				office.SaveViewModel();

				TempData["Success"] = "Office created successfully!";
				return RedirectToAction("Edit", new { controller = "Office", id = office.Id });
			}

			return View(office);
		}

		public ActionResult Edit(int id)
		{
			SessionVars.ReturnUrl = Request.UrlReferrer.IfNotNull(u => u.AbsoluteUri, "//");
			OfficeViewModel vm = new OfficeViewModel(id) { LoggedOnUser = SessionVars.UserName };

			return View(vm);
		}

		// POST: Office/Edit
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit(OfficeViewModel office)
		{
			if (ModelState.IsValid)
			{
				office.LoggedOnUser = SessionVars.UserName;
				office.SaveViewModel();

				TempData["Success"] = "Office updated successfully!";
				return RedirectToAction("Edit", new { controller = "Office", id = office.Id });
			}

			return View(office);
		}
	}
}