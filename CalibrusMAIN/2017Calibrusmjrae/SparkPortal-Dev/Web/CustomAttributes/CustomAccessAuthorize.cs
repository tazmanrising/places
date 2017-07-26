using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Calibrus.SparkPortal.DataAccess.Infrastructure;

namespace Calibrus.SparkPortal.Web.CustomAttributes
{
    public class CustomAccessAuthorize : AuthorizeAttribute
    {
        public AccessLevel AccessLevel { get; set; }
		public int? VendorId { get; set; }
		public int? OfficeId { get; set; }

        protected override bool AuthorizeCore(System.Web.HttpContextBase httpContext)
        {
            bool isAuthorized = false;

            if (httpContext.Session == null || SessionVars.UserName == null)
            {
                return false;
            }

            User user = Business.LoginLogic.ValidateUser(SessionVars.UserName, SessionVars.Password);
            if (user == null)
            {
                return false;
            }

            switch (AccessLevel)
            {
                case AccessLevel.ClientAdministrator:
                    isAuthorized = user.UserType.UserTypeName.Equals("Client Administrator", StringComparison.CurrentCultureIgnoreCase);
                    break;

                case AccessLevel.VendorAdministrator:
					isAuthorized = user.UserType.UserTypeName.Equals("Client Administrator", StringComparison.CurrentCultureIgnoreCase) ||
						(user.UserType.UserTypeName.Equals("Vendor Administrator", StringComparison.CurrentCultureIgnoreCase));

					//if (isAuthorized)
					//{
					//	isAuthorized =
					//		user.UserType.UserTypeName.Equals("Client Administrator", StringComparison.CurrentCultureIgnoreCase) ||
					//		user.UserType.UserTypeName.Equals("Vendor Administrator", StringComparison.CurrentCultureIgnoreCase)
					//		 && user.VendorId.GetValueOrDefault(0) == SessionVars.AccessedVendorId;
					//}

                    break;

                case AccessLevel.OfficeAdministrator:
					isAuthorized = user.UserType.UserTypeName.Equals("Client Administrator", StringComparison.CurrentCultureIgnoreCase) ||
						(user.UserType.UserTypeName.Equals("Vendor Administrator", StringComparison.CurrentCultureIgnoreCase)) ||
                        (user.UserType.UserTypeName.Equals("Sales Administrator", StringComparison.CurrentCultureIgnoreCase)) ||
                        (user.UserType.UserTypeName.Equals("Office Administrator", StringComparison.CurrentCultureIgnoreCase));

					//if (isAuthorized)
					//{
					//	isAuthorized =
					//		user.UserType.UserTypeName.Equals("Client Administrator", StringComparison.CurrentCultureIgnoreCase) ||
					//		(user.UserType.UserTypeName.Equals("Vendor Administrator", StringComparison.CurrentCultureIgnoreCase)
					//		 && user.Vendor.Offices.Any(x=>x.OfficeId == SessionVars.AccessedOfficeId)) ||
					//		(user.UserType.UserTypeName.Equals("Office Administrator", StringComparison.CurrentCultureIgnoreCase)
					//		 && user.OfficeId.GetValueOrDefault(0) == SessionVars.AccessedOfficeId);
					//}

                    break;

                case AccessLevel.SalesAdministrator:
                    isAuthorized = user.UserType.UserTypeName.Equals("Client Administrator", StringComparison.CurrentCultureIgnoreCase) ||
                       // (user.UserType.UserTypeName.Equals("Vendor Administrator", StringComparison.CurrentCultureIgnoreCase)) ||
                        (user.UserType.UserTypeName.Equals("Office Administrator", StringComparison.CurrentCultureIgnoreCase));

                    //if (isAuthorized)
                    //{
                    //	isAuthorized =
                    //		user.UserType.UserTypeName.Equals("Client Administrator", StringComparison.CurrentCultureIgnoreCase) ||
                    //		(user.UserType.UserTypeName.Equals("Vendor Administrator", StringComparison.CurrentCultureIgnoreCase)
                    //		 && user.Vendor.Offices.Any(x=>x.OfficeId == SessionVars.AccessedOfficeId)) ||
                    //		(user.UserType.UserTypeName.Equals("Office Administrator", StringComparison.CurrentCultureIgnoreCase)
                    //		 && user.OfficeId.GetValueOrDefault(0) == SessionVars.AccessedOfficeId);
                    //}

                    break;

                case AccessLevel.Agent:
                    isAuthorized = false;
                    break;

                default:
                    isAuthorized = false;
                    break;
            }

            return isAuthorized;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Result = new RedirectResult("~/Error/AccessDenied");
        }
    }
}