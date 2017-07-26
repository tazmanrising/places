using System.Net;
using System.Security.Principal;

namespace Calibrus.ClearviewPortal.Web.Reports
{
	public class CustomReportCredentials : Microsoft.Reporting.WebForms.IReportServerCredentials
	{
		// local variable for network credential.
		private readonly string _username;

		private readonly string _password;
		private readonly string _domainName;

		public CustomReportCredentials(string username, string password, string domainName)
		{
			_username = username;
			_password = password;
			_domainName = domainName;
		}

		public WindowsIdentity ImpersonationUser
		{
			get
			{
				return null;  // not use ImpersonationUser
			}
		}

		public ICredentials NetworkCredentials
		{
			get
			{
				// use NetworkCredentials
				return new NetworkCredential(_username, _password, _domainName);
			}
		}

		public bool GetFormsCredentials(out Cookie authCookie, out string user, out string password, out string authority)
		{
			// not use FormsCredentials unless you have implements a custom autentication.
			authCookie = null;
			user = password = authority = null;
			return false;
		}
	}
}