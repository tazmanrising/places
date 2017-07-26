using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web.Http;
using Calibrus.SparkPortal.DataAccess.Infrastructure;
using Calibrus.SparkPortal.DataAccess.Models;
using Calibrus.SparkPortal.Web.CustomAttributes;

namespace Calibrus.SparkPortal.Web.Controllers
{
	[CustomApiAccessAuthorize]
    public class ReportDataController : ApiController
    {
		[Route("api/report/dispositions/")]
		[HttpGet]
		public IEnumerable<Disposition> GetDispositions()
		{
			return Business.AppLogic.GetDispositions(true);
		}

		[Route("api/report/calls/")]
		[HttpPost]
		public IHttpActionResult GetResults(SearchContext ctx)
		{
			if (ctx == null)
			{
				return InternalServerError(new ArgumentNullException("ctx"));
			}

			if (ctx.StartDate.HasValue)
			{
				ctx.StartDate = ctx.StartDate.Value.Date;
			}
			if (ctx.EndDate.HasValue)
			{
				ctx.EndDate = ctx.EndDate.Value.Date.AddDays(1);
			}
			List<Main> calls = Business.AppLogic.GetCalls(ctx).ToList();
			return Ok(calls);
		}

	    
    }
}
