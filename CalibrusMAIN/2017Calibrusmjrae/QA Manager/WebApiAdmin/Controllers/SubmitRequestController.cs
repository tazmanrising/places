using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WebApiAdmin.Controllers
{
    public class SubmitRequestController : ApiController
    {
        [HttpPost]
        [Route("api/request/")]
        public IHttpActionResult SubmitRequest(Models.Request request)
        {
            bool hasErrors = false;
            List<string> errorList = new List<string>();
            //DataAccess.Infrastructure.Main main = null;

            try
            {
                //check for existing phone number / check if lead id already verified
                if (Models.AppLogic.PhoneNumberExists(request.Phone)
                    //|| Business.AppLogic.LeadExists(request.Lead.LeadsId)
                    //|| request.OrderDetails.Any(x => Business.AppLogic.AccountNumberExists(x.AccountNumber)))
                    )
                {
                    errorList.Add("This Order cannot be submitted as entered. A combination of this information is already in use.");
                    //return Ok(new ViewModel.ApiMessage<DataAccess.Infrastructure.Main>
                    return Ok();
                    //{
                        //Data = null,
                        //HasErrors = true,
                        //ErrorList = errorList
                    //});
                }

                //create request
                //main = Business.AppLogic.CreateRequest(request);
                //if (main == null)
                //{
                //    hasErrors = true;
                //    errorList.Add($"Unable to save request.");
                //}
            }
            catch (Exception ex)
            {
                hasErrors = true;
                errorList.Add(ex.Message);
            }

            //main.OrderDetails = null;
            //main.User = null;
            //main.IpLocations = null;

            return Ok();
            //return Ok(new ViewModel.ApiMessage<DataAccess.Infrastructure.Main>
            //{
            //    //Data = main,
            //    HasErrors = hasErrors,
            //    ErrorList = errorList
            //});

        }

    }

}
