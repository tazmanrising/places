using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Calibrus.SparkPortal.DataAccess.Infrastructure;
using Calibrus.SparkPortal.Web.CustomAttributes;

namespace Calibrus.SparkPortal.Web.Controllers
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