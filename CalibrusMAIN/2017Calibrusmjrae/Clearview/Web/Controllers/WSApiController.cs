using System;
using Calibrus.ClearviewPortal.DataAccess.Infrastructure;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Calibrus.ClearviewPortal.Web.CustomAttributes;

namespace Calibrus.ClearviewPortal.Web.Controllers
{
    [CustomApiAccessAuthorize]
    public class WSApiController : ApiController
    {

        [Route("api/AWSWavUrl/id/{MainId:int}")]
        public IEnumerable<spGetAWSWavUrl_Result> Get(int MainId)
        {
            var aws = Business.AppLogic.GetAWSWavUrlByMainId(MainId);
            return aws;
        }


        [Route("api/AWSWavUrl/date/{DateToFind}")]
        public IEnumerable<spGetAWSWavUrl_Result> Get(DateTime DateToFind)
        {
            var aws = Business.AppLogic.GetAWSWavUrlByDate(DateToFind);
            return aws;
        }

        [Route("api/AWSWavUrl/vendor/{VendorNumber}")]
        public IEnumerable<spGetAWSWavUrl_Result> Get(string VendorNumber)
        {

            var aws = Business.AppLogic.GetAWSWavUrlByVendorNumber(VendorNumber);
            return aws;
        }

    }
}