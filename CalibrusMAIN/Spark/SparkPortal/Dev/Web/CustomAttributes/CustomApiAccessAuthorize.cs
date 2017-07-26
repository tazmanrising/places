using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.UI.WebControls;
using Calibrus.SparkPortal.DataAccess.Infrastructure;

namespace Calibrus.SparkPortal.Web.CustomAttributes
{
	public class CustomApiAccessAuthorize : System.Web.Http.AuthorizeAttribute
	{
		public override void OnAuthorization(HttpActionContext actionContext)
		{
			IEnumerable<string> header;
			actionContext.Request.Headers.TryGetValues("Auth-Token", out header);

			if (header != null && header.First().Equals("U1BBUktUT0tFTg==", StringComparison.Ordinal))
			{
				return;
			}
			else
			{
				actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Forbidden);
			}
		}
	}
}