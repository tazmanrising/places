using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace CalibrusTPV.Scripts.CustomAttributes
{
    public class CustomApiAccessAuthorize : AuthorizeAttribute
    {

        public override void OnAuthorization(HttpActionContext actionContext)
        {
            IEnumerable<string> header;
            actionContext.Request.Headers.TryGetValues("Auth-Token", out header);

            //var calibrusToken = ConfigurationManager.AppSettings["Auth-Token"];


            var headerToken = "";

            //if (header != null && header.First().Equals("U1BBUktUT0tFTg==", StringComparison.Ordinal))
            //{
            //    return;
            //}
            //else
            //{
            if (header != null)
            {
                headerToken = ((string[])header)[0];
            }

            //var tokenManager = new TokenManager();
            //var checkToken = tokenManager.CheckToken(headerToken);

            bool temp = true;

            if(temp)
            //if (checkToken)
            {
                return;
            }
            else
            {
                actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Forbidden);
            }

            //}






        }

    }

}