using Calibrus.ClearviewPortal.DataAccess.Infrastructure;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace Calibrus.ClearviewPortal.DataEntryWeb.Controllers
{
    public class LogonController : ApiController
    {
        [HttpPost]
        [AllowAnonymous]
        [Route("api/dataentry/logon/")]
        public IHttpActionResult Logon(DataAccess.Entities.Logon logon)
        {
            bool hasErrors = false;
            List<string> errorList = new List<string>();
            User user = null;

            try
            {
                user = Business.LoginLogic.ValidateDataEntryUser(logon.ClearviewId, logon.Password);
                if (user == null)
                {
                    hasErrors = true;
                    errorList.Add("Invalid Clearview Id or Password.");
                }
                else if (!user.IsActive)
                {
                    hasErrors = true;
                    errorList.Add("Agent is inactive.");
                }
            }
            catch (Exception ex)
            {
                hasErrors = true;
                errorList.Add(ex.Message);
            }

            return Ok(new ViewModel.ApiMessage<DataAccess.Entities.User>
            {
                Data = user == null ? null : new DataAccess.Entities.User
                {
                    UserId = user.UserId,
                    AgentId = user.AgentId,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    ClearviewId = user.ClearviewId,
                    OfficeId = user.OfficeId.GetValueOrDefault(0),
                    OfficeName = user.Office?.OfficeName??"No Office Assigned",
                    VendorId = user.VendorId.GetValueOrDefault(0),
                    VendorName = user.Vendor?.VendorName??"No Vendor Assigned",
                    VendorNumber = user.Vendor?.VendorNumber ?? "000"
                },
                HasErrors = hasErrors,
                ErrorList = errorList
            });
        }
    }
}