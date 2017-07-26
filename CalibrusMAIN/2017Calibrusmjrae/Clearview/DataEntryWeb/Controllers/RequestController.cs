using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Calibrus.ClearviewPortal.DataAccess.CodeFirst;
using Calibrus.ClearviewPortal.DataAccess.CodeFirst.Models;
using Calibrus.ClearviewPortal.DataAccess.Infrastructure;

namespace Calibrus.ClearviewPortal.DataEntryWeb.Controllers
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
                main = Business.AppLogic.CreateRequest(request);
                if (main == null)
                {
                    hasErrors = true;
                    //errorList.Add($"Record Locator '{recordLocator}' Not Found");
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

        [Route("api/getutilityprograms/{vendorid:int}/{officeid:int}/{state}/{zip}")]
        public IEnumerable<GetUtilityPrograms_Result> GetUtilityPrograms(int vendorid, int officeid, string state, string zip)
        {
            var utilityPrograms = Business.AppLogic.GetUtilityPrograms(vendorid, officeid, state, zip);
            return utilityPrograms;
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

        [Route("api/programs/{utilityId:int}/{vendorId:int}/{utilityType:regex(^(?i)(Gas)|(Electric)$)}/{accountType:regex(^(?i)(Residential)|(Business)$)}")]
        public IHttpActionResult GetProgramList(int utilityId, int vendorId, string utilityType, string accountType)
        {
            bool hasErrors = false;
            List<string> errorList = new List<string>();
            List<DataAccess.Infrastructure.Program> programs = null;

            try
            {
                programs = Business.AppLogic.GetPrograms(utilityId,
                                                        vendorId,
                                                        (Business.Enums.UtilityType)Enum.Parse(typeof(Business.Enums.UtilityType), utilityType),
                                                        (Business.Enums.PremiseType)Enum.Parse(typeof(Business.Enums.PremiseType), accountType));
                if (programs == null || programs.Count == 0)
                {
                    hasErrors = true;
                    errorList.Add("Programs Not Found");
                }
                else
                {
                    foreach (DataAccess.Infrastructure.Program p in programs)
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
                }
            }
            catch (Exception ex)
            {
                hasErrors = true;
                errorList.Add(ex.Message);
            }

            return Ok(new ViewModel.ApiMessage<List<DataAccess.Infrastructure.Program>>
            {
                Data = programs,
                HasErrors = hasErrors,
                ErrorList = errorList
            });

        }

        [Route("api/utilities/{vendorId:int}/{utilityType:regex(^(?i)(Gas)|(Electric)$)}/{accountType:regex(^(?i)(Residential)|(Business)$)}/{state:regex(^[A-Za-z]{2}$)}")]
        public IHttpActionResult GetUtilityList(int vendorId, string utilityType, string accountType, string state)
        {
            bool hasErrors = false;
            List<string> errorList = new List<string>();
            List<DataAccess.Infrastructure.Utility> utilities = null;

            try
            {
                utilities = Business.AppLogic.GetUtilities(vendorId,
                                                        (Business.Enums.UtilityType)Enum.Parse(typeof(Business.Enums.UtilityType), utilityType),
                                                        (Business.Enums.PremiseType)Enum.Parse(typeof(Business.Enums.PremiseType), accountType),
                                                        state);
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

        [Route("api/utilities/{vendorId:int}/{utilityType:regex(^(?i)(Gas)|(Electric)$)}/{accountType:regex(^(?i)(Residential)|(Business)$)}")]
        public IHttpActionResult GetUtilityList(int vendorId, string utilityType, string accountType)
        {
            bool hasErrors = false;
            List<string> errorList = new List<string>();
            List<DataAccess.Infrastructure.Utility> utilities = null;

            try
            {
                utilities = Business.AppLogic.GetUtilities(vendorId,
                                                        (Business.Enums.UtilityType)Enum.Parse(typeof(Business.Enums.UtilityType), utilityType),
                                                        (Business.Enums.PremiseType)Enum.Parse(typeof(Business.Enums.PremiseType), accountType));
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

        [HttpPost]
        [Route("api/dtdtrack/")]
        public IHttpActionResult DTDTrackAgent(DataAccess.Entities.TrackAgent TrackAgent)
        {
            bool hasErrors = false;
            bool agentTrack;
            List<string> errorList = new List<string>();
            List<DataAccess.Infrastructure.DtDAgentTrack> tracking = null;

            try
            {
                agentTrack = Business.AppLogic.AddAgentTrack(TrackAgent);
            }
            catch (Exception ex)
            {
                hasErrors = true;
                errorList.Add(ex.Message);
            }
            return Ok(new ViewModel.ApiMessage<List<DataAccess.Infrastructure.DtDAgentTrack>>
            {
                Data = tracking,
                HasErrors = hasErrors,
                ErrorList = errorList
            });
        }

        [HttpGet]
        [Route("api/getserviceablezip/{zip}")]
        public IHttpActionResult GetServiceableZipCode(string zip)
        {
            var zipService = new List<ServiceableZipCodes>();
            try
            {
                zipService = ZipCodeService.GetServiceableZipCodes(zip);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            
            return Ok(zipService);

        }



    }
}
