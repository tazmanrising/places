using System.Linq;
using System.Web.Mvc;
using Calibrus.ClearviewPortal.ViewModel;

namespace Calibrus.ClearviewPortal.Web.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        public AccountController()
        {
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                bool success = model.ValidateUser();
                if (success)
                {
                    SessionVars.UserName = model.LoggedInUser.AgentId;
                    SessionVars.Password = model.LoggedInUser.Password;
                    SessionVars.LoggedInVendorId = model.LoggedInUser.VendorId;
					SessionVars.LoggedInOfficeId = model.LoggedInUser.OfficeId;
                    SessionVars.IsClientAdmin = model.LoggedInUser.UserType.SecurityLevel == 1000;
                    SessionVars.IsQaAdmin = model.LoggedInUser.UserType.SecurityLevel == 750;
                    SessionVars.IsVendorAdmin = model.LoggedInUser.UserType.SecurityLevel == 500;
					SessionVars.IsOfficeAdmin = model.LoggedInUser.UserType.SecurityLevel == 250;
                    SessionVars.SecurityLevel = model.LoggedInUser.UserType.SecurityLevel;

                    if (SessionVars.IsClientAdmin)
                    {
                        return RedirectToAction("ClientAdmin", "Home");
                    }
                    else if (SessionVars.IsQaAdmin)
                    {
                        return RedirectToAction("QaAdmin", "Home");
                    }
                    else if (SessionVars.IsVendorAdmin)
                    {
                        return RedirectToAction("VendorAdmin", "Home");
                    }
					else if (SessionVars.IsOfficeAdmin)
					{
						return RedirectToAction("OfficeAdmin", "Home");
					}
                    else
                    {
                        return RedirectToAction("AccessDenied", "Error");
                    }
                }

                Session.Abandon();
                ModelState.AddModelError("", "Invalid username or password.");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            //AuthenticationManager.SignOut();
            Session.Abandon();
            return RedirectToAction("Login", "Account");
        }
    }
}