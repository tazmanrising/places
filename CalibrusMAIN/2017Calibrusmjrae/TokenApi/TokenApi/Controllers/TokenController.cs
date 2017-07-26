using System;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using TokenApi.Core;
using TokenApi.CustomAttributes;
using TokenApi.Models;

namespace TokenApi.Controllers
{
    [CustomApiAccessAuthorize]
    public class TokenController : ApiController
    {

        [HttpPost]
        [AllowAnonymous]
        [Route("api/dataentry/logon/")]
        public IHttpActionResult Logon()
        {
            return Ok();
        }



        [Route("api/books/localoptional/{lcid:int?}")]
        public IHttpActionResult GetBooksByLocaleOptional(int lcid = 1033)
        {

            return Ok();

        }

        [Route("api/books/localdefault/{lcid:int=1033}")]
        public IHttpActionResult GetBooksByLocaleDefault(int lcid)
        {

            return Ok();

        }



        [HttpGet]
        [Route("api/test/")]
        public IHttpActionResult TestToken()
        {
            
            //var s = Request.GetOwinContext().Request.RemoteIpAddress;




            return Ok();

        }



        [HttpGet]
        [Route("api/CheckMainVerified/{mainid}")]
        public IHttpActionResult CheckIfVerified(int mainid)
        {
            try
            {
                return Ok();

            }
            catch (Exception e)
            {
                return BadRequest();
            }


        }


        [HttpGet]
        [Route("api/ip")]
        public IHttpActionResult GetHeaders()
        {

           // var apiLogHandler = new ApiLogHandler();
           // apiLogHandler.


            return Ok();
        }



        [HttpGet]
        [Route("GetQAByDateTime/date/{date}/time/{time}")]
        public IHttpActionResult GetQAFromDateTime(string date, string time)
        {



            return Ok();
        }





        [HttpGet]
        [Route("api/getheader/")]
        public IHttpActionResult RequestHeader()
        {

            //1.  directly get header
            var a = HttpContext.Current.Request.UserHostAddress;
            if (a == "::1")
            {
                a= "localhost";
            }

            var browser = HttpContext.Current.Request.UserAgent;
            var url = HttpContext.Current.Request.Url.OriginalString;




            //2. instantiate tokenManager when header call , pass in emtpy string 
            var tokenManager = new TokenManager();
            tokenManager.CheckToken("");




            return Ok("in api request");


        }



        [HttpGet]
        [Route("api/request/")]
        public IHttpActionResult RequestToken()
        {

            return Ok("in api request");

        }


        //[Route("api/reports/{securitylevel:int}")]
        //public IEnumerable<Report> GetReports(int securityLevel)
        //{
        //    List<Report> reports = Business.AppLogic.GetReports(securityLevel);
        //    return reports;
        //}






    }

}
