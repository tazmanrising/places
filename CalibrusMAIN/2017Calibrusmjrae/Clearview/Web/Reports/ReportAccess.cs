using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Calibrus.ClearviewPortal.DataAccess.Infrastructure;

namespace Calibrus.ClearviewPortal.Web.Reports
{
    public static class ReportAccess
    {
        public static bool IsAuthorized(HttpContext httpContext, AccessLevel accessLevel)
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

            switch (accessLevel)
            {
                case AccessLevel.ClientAdministrator:
                    isAuthorized = user.UserType.UserTypeName.Equals("Client Administrator", StringComparison.CurrentCultureIgnoreCase);
                    break;

                case AccessLevel.QaAdministrator:
                    isAuthorized =
                        user.UserType.UserTypeName.Equals("Client Administrator", StringComparison.CurrentCultureIgnoreCase) ||
                        (user.UserType.UserTypeName.Equals("QA Administrator", StringComparison.CurrentCultureIgnoreCase));
                    break;

                case AccessLevel.VendorAdministrator:
                    isAuthorized =
                        user.UserType.UserTypeName.Equals("Client Administrator", StringComparison.CurrentCultureIgnoreCase) ||
                        (user.UserType.UserTypeName.Equals("QA Administrator", StringComparison.CurrentCultureIgnoreCase)) ||
                        (user.UserType.UserTypeName.Equals("Vendor Administrator", StringComparison.CurrentCultureIgnoreCase));
                    break;

                case AccessLevel.OfficeAdministrator:
                    isAuthorized =
                        user.UserType.UserTypeName.Equals("Client Administrator", StringComparison.CurrentCultureIgnoreCase) ||
                        (user.UserType.UserTypeName.Equals("QA Administrator", StringComparison.CurrentCultureIgnoreCase)) ||
                        (user.UserType.UserTypeName.Equals("Vendor Administrator", StringComparison.CurrentCultureIgnoreCase)) ||
                        (user.UserType.UserTypeName.Equals("Office Administrator", StringComparison.CurrentCultureIgnoreCase));
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
    }
}
