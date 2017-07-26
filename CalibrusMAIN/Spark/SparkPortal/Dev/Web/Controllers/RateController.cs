using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using Calibrus.SparkPortal.ViewModel;
using Calibrus.SparkPortal.Web.CustomAttributes;
using Microsoft.Ajax.Utilities;
using System.Web.Mvc;

namespace Calibrus.SparkPortal.Web.Controllers
{
    [CustomAccessAuthorize(AccessLevel = AccessLevel.ClientAdministrator)]
    public class RateController : Controller
    {
        // GET: Rate
        public ActionResult Index()
        {
            return View();
        }

        // GET: Rate
        [ChildActionOnly]
        public ActionResult _Index(int? id)
        {
            return PartialView(new RateIndexViewModel(id));
        }

        // GET: Rate/Create/
        public ActionResult Create()
        {
            if (SessionVars.UserName.IsNullOrWhiteSpace())
            {
                return RedirectToAction("Login", new { controller = "Account" });
            }

            SessionVars.ReturnUrl = Request.UrlReferrer.IfNotNull(u => u.AbsoluteUri, "//");
            return View(new RateViewModel());
        }

        // POST: Rate/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(RateViewModel newProgram)
        {
            if (SessionVars.UserName.IsNullOrWhiteSpace())
            {
                return RedirectToAction("Login", new { controller = "Account" });
            }

            if (ModelState.IsValid)
            {
                newProgram.Id = null;
                newProgram.UpdatedBy = SessionVars.UserName;
                newProgram.SaveViewModel();

				TempData["Success"] = "Program created successfully!";
                return RedirectToAction("Edit", new { controller = "Rate", id = newProgram.Id });
            }

			//if selected vendors or SalesChannels is null initialize it
			newProgram.SelectedVendors = newProgram.SelectedVendors ?? new List<int>();
            newProgram.SelectedSalesChannels = newProgram.SelectedSalesChannels ?? new List<int>();
            newProgram.SetDropdowns();
            return View(newProgram);
        }

        // GET: User/Edit
        public ActionResult Edit(int id)
        {
            if (SessionVars.UserName.IsNullOrWhiteSpace())
            {
                return RedirectToAction("Login", new { controller = "Account" });
            }

            SessionVars.ReturnUrl = Request.UrlReferrer.IfNotNull(u => u.AbsoluteUri, "//");
            return View(new RateViewModel(id));
        }

		// GET: User/Copy
		public ActionResult Copy(int id)
		{
			int newProgramId = Business.AppLogic.CopyProgram(id, SessionVars.UserName);
			TempData["Success"] = "Program copied successfully!";

			return RedirectToAction("Edit", "Rate", new { id = newProgramId });
		}

        // POST: Rate/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(RateViewModel model)
        {
            if (SessionVars.UserName.IsNullOrWhiteSpace())
            {
                return RedirectToAction("Login", new { controller = "Account" });
            }

            if (ModelState.IsValid)
            {
                model.UpdatedBy = SessionVars.UserName;
                model.SaveViewModel();
				TempData["Success"] = "Program updated successfully!";
                return RedirectToAction("Edit", new { controller = "Rate", id = model.Id });
            }

			//if selected vendors is null initialize it
			model.SelectedVendors = model.SelectedVendors ?? new List<int>();
            model.SelectedSalesChannels = model.SelectedSalesChannels ?? new List<int>();

            model.SetDropdowns();
            return View(model);
        }
    }
}