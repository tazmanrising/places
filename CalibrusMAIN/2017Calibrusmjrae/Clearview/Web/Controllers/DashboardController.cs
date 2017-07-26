using Calibrus.ClearviewPortal.DataAccess.Infrastructure;
using Calibrus.ClearviewPortal.Web.CustomAttributes;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace Calibrus.ClearviewPortal.Web.Controllers
{
	[CustomApiAccessAuthorize]
	public class DashboardController : ApiController
	{
		[Route("api/verifiedchart/{timeFrame:regex([dwmyDWMY]):length(1)}/{vendorId:int}/{officeId:int}")]
		[HttpGet]
		public IHttpActionResult GetVerifiedChartData(string timeFrame, int vendorId, int officeId)
		{
			DateTime sDate = GetStartDate(timeFrame.ToUpper());

			List<GetVerifiedChartSummary_Result> summary = Business.AppLogic.GetVerifiedChartData(sDate, vendorId, officeId);
			List<GetVerifiedChartDetail_Result> detail = Business.AppLogic.GetVerifiedChartDetailData(sDate, vendorId, officeId);
			List<Object> data = new List<Object> { summary, detail };

			return Ok(data);
		}

        [Route("api/topvendors/{timeFrame:regex([dwmyDWMY]):length(1)}/{vendorId:int}")]
        [HttpGet]
        public IHttpActionResult GetTopVendorsData(string timeFrame, int vendorId)
        {
            DateTime sDate = GetStartDate(timeFrame.ToUpper());

            List<GetTopVendorStats_Result> summary = Business.AppLogic.GetTopVendorsStats(sDate, vendorId);

            return Ok(summary);
        }

        [Route("api/topoffices/{timeFrame:regex([dwmyDWMY]):length(1)}/{vendorId:int}/{officeId:int}")]
        [HttpGet]
        public IHttpActionResult GetTopOfficesData(string timeFrame, int vendorId, int officeId)
        {
            DateTime sDate = GetStartDate(timeFrame.ToUpper());

            List<GetTopOfficeStats_Result> summary = Business.AppLogic.GetTopOfficeStats(sDate, vendorId, officeId);

            return Ok(summary);
        }

        [Route("api/topusers/{timeFrame:regex([dwmyDWMY]):length(1)}/{vendorId:int}/{officeId:int}")]
        [HttpGet]
        public IHttpActionResult GetTopUsersData(string timeFrame, int vendorId, int officeId)
        {
            DateTime sDate = GetStartDate(timeFrame.ToUpper());

            List<GetTopUserStats_Result> summary = Business.AppLogic.GetTopUsersStats(sDate, vendorId, officeId);

            return Ok(summary);
        }

        private DateTime GetStartDate(string timeFrame)
        {
            DateTime sDate;

            switch (timeFrame)
            {
                case "D":
                    sDate = DateTime.Today.Date;
                    break;

                case "W":

                    int diff = DateTime.Now.DayOfWeek - DayOfWeek.Monday;
                    if (diff < 0)
                    {
                        diff += 7;
                    }
                    sDate = DateTime.Now.AddDays(-1 * diff).Date;
                    break;

                case "M":
                    sDate = DateTime.Today.AddDays(1 - DateTime.Today.Day);
                    break;

                case "Y":
                    sDate = DateTime.Today.AddDays(1 - DateTime.Today.DayOfYear);
                    break;

                default:
                    sDate = DateTime.Today.Date;
                    break;
            }

            return sDate;
        }
    }
}