using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Calibrus.SparkPortal.DataEntryWeb.Controllers
{
    public class RequestController : ApiController
    {
        [HttpPost]
        [Route("api/request/")]
        public IHttpActionResult SubmitRequest(DataAccess.Entities.Request request)
        {
            bool hasErrors = false;
            List<string> errorList = new List<string>();
            DataAccess.Infrastructure.Main main = null;

            try
            {
                //check for existing phone number / check if lead id already verified
                if (Business.AppLogic.PhoneNumberExists(request.Phone) 
                    || Business.AppLogic.LeadExists(request.Lead.LeadsId)
                    || request.OrderDetails.Any(x=> Business.AppLogic.AccountNumberExists(x.AccountNumber)))
                {
                    errorList.Add("This Order cannot be submitted as entered. A combination of this information is already in use.");
                    return Ok(new ViewModel.ApiMessage<DataAccess.Infrastructure.Main>
                    {
                        Data = null,
                        HasErrors = true,
                        ErrorList = errorList
                    });                    
                }

                //create request
                main = Business.AppLogic.CreateRequest(request);
                if (main == null)
                {
                    hasErrors = true;
                    errorList.Add($"Unable to save request.");
                }
            }
            catch (Exception ex)
            {
                hasErrors = true;
                errorList.Add(ex.Message);
            }

            main.OrderDetails = null;
            main.User = null;
            main.IpLocations = null;

            return Ok(new ViewModel.ApiMessage<DataAccess.Infrastructure.Main>
            {
                Data = main,
                HasErrors = hasErrors,
                ErrorList = errorList
            });

        }


        [Route("api/lead/{vendorNumber}/{recordLocator}")]
        public IHttpActionResult GetLead(string recordLocator, string vendorNumber)
        {
            bool hasErrors = false;
            List<string> errorList = new List<string>();
            DataAccess.Infrastructure.Lead lead = null;

            try
            {
                lead = Business.AppLogic.GetLead(recordLocator, vendorNumber);
                if (lead == null)
                {
                    hasErrors = true;
                    errorList.Add($"Record Locator '{recordLocator}' Not Found");
                }                
            }
            catch (Exception ex)
            {
                hasErrors = true;
                errorList.Add(ex.Message);
            }

            return Ok(new ViewModel.ApiMessage<DataAccess.Infrastructure.Lead>
            {
                Data = lead,
                HasErrors = hasErrors,
                ErrorList = errorList
            });
            
        }

        [Route("api/programs/{utilityId:int}/{vendorId:int}/{utilityType}/")]
        public IHttpActionResult GetProgramList(int utilityId, int vendorId, string utilityType)
        {
            bool hasErrors = false;
            List<string> errorList = new List<string>();
            List<DataAccess.Infrastructure.Program> programs = null;

            try
            {
                programs = Business.AppLogic.GetPrograms(utilityId, vendorId, utilityType);
                if (programs == null || programs.Count == 0)
                {
                    hasErrors = true;
                    errorList.Add("Programs Not Found");
                }
            }
            catch (Exception ex)
            {
                hasErrors = true;
                errorList.Add(ex.Message);
            }

            foreach(DataAccess.Infrastructure.Program p in programs)
            {
                p.AccountNumberType.Programs = null;
                p.Brand.Programs = null;
                p.PremiseType.Programs = null;
                p.UnitOfMeasure.Programs = null;
                p.Utility.Programs = null;
                p.UtilityType.Programs = null;
                foreach (DataAccess.Infrastructure.ProgramVendor pv in p.ProgramVendors)
                {
                    pv.Program = null;
                    pv.Vendor = null;
                }               
            }

            return Ok(new ViewModel.ApiMessage<List<DataAccess.Infrastructure.Program>>
            {
                Data = programs,
                HasErrors = hasErrors,
                ErrorList = errorList
            });

        }

        [Route("api/utilities/{state:regex(^[A-Za-z]{2}$)}")]
        public IHttpActionResult GetUtilityList(string state)
        {
            bool hasErrors = false;
            List<string> errorList = new List<string>();
            List<DataAccess.Infrastructure.Utility> utilities = null;

            try
            {
                utilities = Business.AppLogic.GetUtilities(state);
                if (utilities == null || utilities.Count == 0)
                {
                    hasErrors = true;
                    errorList.Add("Utilities Not Found");
                }
            }
            catch (Exception ex)
            {
                hasErrors = true;
                errorList.Add(ex.Message);
            }

            foreach (DataAccess.Infrastructure.Utility u in utilities)
            {
                u.Programs = null;               
            }

            return Ok(new ViewModel.ApiMessage<List<DataAccess.Infrastructure.Utility>>
            {
                Data = utilities,
                HasErrors = hasErrors,
                ErrorList = errorList
            });

        }

        [Route("api/utilities/")]
        public IHttpActionResult GetUtilityList()
        {
            bool hasErrors = false;
            List<string> errorList = new List<string>();
            List<DataAccess.Infrastructure.Utility> utilities = null;

            try
            {
                utilities = Business.AppLogic.GetUtilities();
                if (utilities == null || utilities.Count == 0)
                {
                    hasErrors = true;
                    errorList.Add("Utilities Not Found");
                }
            }
            catch (Exception ex)
            {
                hasErrors = true;
                errorList.Add(ex.Message);
            }

            foreach (DataAccess.Infrastructure.Utility u in utilities)
            {
                u.Programs = null;
            }

            return Ok(new ViewModel.ApiMessage<List<DataAccess.Infrastructure.Utility>>
            {
                Data = utilities,
                HasErrors = hasErrors,
                ErrorList = errorList
            });

        }

        [Route("api/relationships/")]
        public IHttpActionResult GetRelationshipList()
        {
            bool hasErrors = false;
            List<string> errorList = new List<string>();
            List<DataAccess.Infrastructure.Relationship> relationships = null;

            try
            {
                relationships = Business.AppLogic.GetRelationships();
                if (relationships == null || relationships.Count == 0)
                {
                    hasErrors = true;
                    errorList.Add("Relationships Not Found");
                }
            }
            catch (Exception ex)
            {
                hasErrors = true;
                errorList.Add(ex.Message);
            }

            return Ok(new ViewModel.ApiMessage<List<DataAccess.Infrastructure.Relationship>>
            {
                Data = relationships,
                HasErrors = hasErrors,
                ErrorList = errorList
            });

        }
    }
}
