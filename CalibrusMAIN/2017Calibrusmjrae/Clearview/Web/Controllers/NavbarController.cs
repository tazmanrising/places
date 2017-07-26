using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Calibrus.ClearviewPortal.DataAccess.Infrastructure;
using Calibrus.ClearviewPortal.Web.CustomAttributes;

namespace Calibrus.ClearviewPortal.Web.Controllers
{
	[CustomApiAccessAuthorize]
	public class NavbarController : ApiController
	{
		[Route("api/reports/{securitylevel:int}")]
		public IEnumerable<Report> GetReports(int securityLevel)
		{
			List<Report> reports = Business.AppLogic.GetReports(securityLevel);
			return reports;
		}
	}
}