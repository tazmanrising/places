using Script2_Liberty.Data.Models;
using Script2_Liberty.Data.Services;
using System.Collections.Generic;
using System.Web.Http;

namespace Script2_Liberty.Controllers
{
    public class TestController : ApiController
    {
        [HttpGet]
        public IHttpActionResult Get()
        {

            return Ok(new List<int>() { 1, 2, 3 });
        }

        [HttpGet]
        [Route("api/GetMarket/{marketId:int?}")]
        public IHttpActionResult GetMarket(int marketId)
        {
            var market = new Market();
            var contractterm = new List<ContractTerm>();
            contractterm = market.TestMarketUtilities(marketId);
            return Ok(contractterm);
        }
    }
}
