using System;
using System.Threading;
using System.Web;
using Microsoft.Ajax.Utilities;

namespace Calibrus.SparkPortal.Web
{
    public static class SessionVars
    {

        private const string KwUserName = "username";
        private const string KwPassword = "password";
        private const string KwVendorId = "vendornumber";
		private const string KwOfficeId = "officenumber";
        private const string KwIsClientAdmin = "isclientadmin";
        private const string KwIsVendorAdmin = "isvendoradmin";
        private const string KwIsSalesAdmin = "issalesadmin";
        private const string KwReturnUrl = "returnurl";
		private const string KwAccessedVendorId = "accessedvendorid";
		private const string KwAccessedOfficeId = "accessedofficeid";
        private const string KwSecurityLevel = "securitylevel";

        public static string UserName
        {
            get
            {
                return HttpContext.Current.Session[KwUserName].IfNotNull(x=>x.ToString());
            }
            set { HttpContext.Current.Session[KwUserName] = value; }
        }

        public static string Password
        {
            get { return HttpContext.Current.Session[KwPassword].IfNotNull(x => x.ToString()); }
            set { HttpContext.Current.Session[KwPassword] = value; }
        }

        public static int? LoggedInVendorId
        {
			get { return HttpContext.Current.Session[KwVendorId].IfNotNull(x => Convert.ToInt32(x)); }
            set { HttpContext.Current.Session[KwVendorId] = value; }
        }

		public static int? LoggedInOfficeId
		{
			get { return HttpContext.Current.Session[KwOfficeId].IfNotNull(x => Convert.ToInt32(x)); }
			set { HttpContext.Current.Session[KwOfficeId] = value; }
		}

        public static bool IsClientAdmin
        {
            get { return HttpContext.Current.Session[KwIsClientAdmin] == null ? false : (bool)HttpContext.Current.Session[KwIsClientAdmin]; }
            set { HttpContext.Current.Session[KwIsClientAdmin] = value; }
        }
        public static bool IsVendorAdmin
        {

            get { return HttpContext.Current.Session[KwIsVendorAdmin] == null ? false : (bool)HttpContext.Current.Session[KwIsVendorAdmin]; }
            set { HttpContext.Current.Session[KwIsVendorAdmin] = value; }
        }
        public static bool IsSalesAdmin
        {

            get { return HttpContext.Current.Session[KwIsSalesAdmin] == null ? false : (bool)HttpContext.Current.Session[KwIsSalesAdmin]; }
            set { HttpContext.Current.Session[KwIsSalesAdmin] = value; }
        }

        public static string ReturnUrl
        {
            get { return HttpContext.Current.Session[KwReturnUrl].IfNotNull(x => x.ToString()); }
            set { HttpContext.Current.Session[KwReturnUrl] = value; }
        }

		public static int AccessedVendorId
		{
			get
			{
				return HttpContext.Current.Session[KwAccessedVendorId] != null ? Convert.ToInt32(HttpContext.Current.Session[KwAccessedVendorId]) : 0;
				
			}
			set { HttpContext.Current.Session[KwAccessedVendorId] = value; }
		}

		public static int AccessedOfficeId
		{
			get
			{
				return HttpContext.Current.Session[KwAccessedOfficeId] != null ? Convert.ToInt32(HttpContext.Current.Session[KwAccessedOfficeId]) : 0;

			}
			set { HttpContext.Current.Session[KwAccessedOfficeId] = value; }
		}

		public static bool IsOfficeAdmin { get; set; }

        public static int SecurityLevel
        {
            get { return HttpContext.Current.Session[KwSecurityLevel] == null ? 0 : (int)HttpContext.Current.Session[KwSecurityLevel]; }
            set { HttpContext.Current.Session[KwSecurityLevel] = value; }
        }
    }
}