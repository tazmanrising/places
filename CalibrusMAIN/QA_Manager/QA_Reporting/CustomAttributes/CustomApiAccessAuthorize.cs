using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Net;
using System.Web.Http.Controllers;
using System.Net.Http;
using TokenApi.Core;

namespace QA_Reporting.CustomAttributes
{
    public class CustomApiAccessAuthorize : AuthorizeAttribute
    {
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            IEnumerable<string> header;
            actionContext.Request.Headers.TryGetValues("Auth-Token", out header);



            var headerToken = "";


            if (header != null)
            {
                headerToken = ((string[])header)[0];
            }

            var tokenManager = new TokenManager();
            var checkToken = tokenManager.CheckToken(headerToken);


            if (checkToken)
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