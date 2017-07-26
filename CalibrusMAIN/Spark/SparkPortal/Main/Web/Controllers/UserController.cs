using System;
using System.Text.RegularExpressions;
using Calibrus.SparkPortal.ViewModel;
using Calibrus.SparkPortal.Web.CustomAttributes;
using Microsoft.Ajax.Utilities;
using System.Web.Mvc;

namespace Calibrus.SparkPortal.Web.Controllers
{
    [CustomAccessAuthorize(AccessLevel = AccessLevel.OfficeAdministrator)]
    public class UserController : Controller
    {
        // GET: Vendor
        [CustomAccessAuthorize(AccessLevel = AccessLevel.OfficeAdministrator)]
        public ActionResult Index()
        {
            return View();
        }

        [CustomAccessAuthorize(AccessLevel = AccessLevel.OfficeAdministrator)]
        [ChildActionOnly]
        public ActionResult _Index()
        {
			return PartialView();
        }

		[Route("User/Create/{vendorId:int?}/{officeId:int?}")]
		[CustomAccessAuthorize(AccessLevel = AccessLevel.OfficeAdministrator)]
		public ActionResult Create(int? vendorId, int? officeId)
        {
			SessionVars.AccessedVendorId = vendorId.GetValueOrDefault(0);
			SessionVars.AccessedOfficeId = officeId.GetValueOrDefault(0);

			return View(new UserViewModel(SessionVars.LoggedInVendorId, SessionVars.LoggedInOfficeId, SessionVars.UserName));
        }

        // POST: User/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
		[CustomAccessAuthorize(AccessLevel = AccessLevel.OfficeAdministrator)]
        public ActionResult Create(UserViewModel user)
        {
            if (String.Compare(user.ShirtSize, "?") == 0)
            {
                user.ShirtSize = null;
            }

            if (String.Compare(user.Gender, "?") == 0)
            {
                user.Gender = null;
            }

            if (!string.IsNullOrEmpty(user.PhoneNumber))
	        {
		        user.PhoneNumber = Regex.Replace(user.PhoneNumber, @"[^\d]", ""); //strip off any masking from the UI
	        }

            if (!string.IsNullOrEmpty(user.MobilePhoneNumber))
            {
                user.MobilePhoneNumber = Regex.Replace(user.MobilePhoneNumber, @"[^\d]", ""); //strip off any masking from the UI
            }

            if (ModelState.IsValid)
            {
                user.Id = null;
                user.LoggedOnUser = SessionVars.UserName;
	            user.IsActive =  user.UserTypeId == 4 ? SessionVars.IsClientAdmin : true;
                user.SaveViewModel();

				TempData["Success"] = String.Format("{0} {1} was created successfully!", user.FirstName, user.LastName);
				return RedirectToAction("Create", new { controller = "User", id = user.Id });
            }

			user.LoggedOnUser = SessionVars.UserName;
            return View(new UserViewModel(user));
        }

        // GET: User/Edit
		[CustomAccessAuthorize(AccessLevel = AccessLevel.VendorAdministrator)]
        public ActionResult Edit(int id)
        {
			UserViewModel vm = new UserViewModel(id, SessionVars.UserName);
			SessionVars.AccessedOfficeId = vm.OfficeId.GetValueOrDefault(0);
            return View(vm);
        }

        // GET: User/Detail
        public ActionResult Detail(int id)
        {
			return View(new UserViewModel(id, SessionVars.UserName));
        }

        // POST: User/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
		[CustomAccessAuthorize(AccessLevel = AccessLevel.VendorAdministrator)]
        public ActionResult Edit(UserViewModel user)
        {
            if(String.Compare(user.ShirtSize, "?") == 0)
            {
                user.ShirtSize = null;
            }

            if (String.Compare(user.Gender, "?") == 0)
            {
                user.Gender = null;
            }

            if (!string.IsNullOrEmpty(user.PhoneNumber))
			{
				user.PhoneNumber = Regex.Replace(user.PhoneNumber, @"[^\d]", ""); //strip off any masking from the UI
			}

            if (!string.IsNullOrEmpty(user.MobilePhoneNumber))
            {
                user.MobilePhoneNumber = Regex.Replace(user.MobilePhoneNumber, @"[^\d]", ""); //strip off any masking from the UI
            }

            if (ModelState.IsValid)
            {
                user.LoggedOnUser = SessionVars.UserName;
                user.SaveViewModel();

				TempData["Success"] = "User updated successfully!";
                return RedirectToAction("Edit", new { controller = "User", id = user.Id });
            }

			user.LoggedOnUser = SessionVars.UserName;
            return View(new UserViewModel(user));
        }
    }
}