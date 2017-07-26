using System.Collections.Generic;
using System.Web.Http;

namespace SparkScriptViewer.Controllers
{
    public class TestController : ApiController
    {

        // GET: api/test
        [HttpGet]
        public IHttpActionResult Get()
        {

            return Ok(new List<int>() { 1, 2, 3 });
        }
    }
}
