using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Calibrus.SparkPortal.DataAccess.Infrastructure;
using Calibrus.SparkPortal.DataEntryWeb.CustomAttributes;


namespace Calibrus.SparkPortal.DataEntryWeb.Controllers
{
    [CustomApiAccessAuthorize]
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
                 || request.OrderDetails.Any(x => Business.AppLogic.AccountNumberExists(x.AccountNumber)))
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

        [Route("api/main/{mainId}")]
        public IEnumerable<spGetMainClone_Result> GetMain(int mainId)
        {
            var mains = Business.AppLogic.GetMainClone(mainId);
            return mains;
        }

        [HttpGet]
        [Route("api/CheckMainVerified/{mainid}")]
        public IHttpActionResult CheckIfVerified(int mainid)
        {
            var hasErrors = false;
            var errorList = new List<string>();
            
            try
            {
                var main = Business.AppLogic.MainVerified(mainid);

                if (main == null)
                {
                        return Ok(new ViewModel.ApiMessage<DataAccess.Entities.Verification>
                        {
                            Data = null,
                            HasErrors = true,
                            ErrorList = new List<string> {"Invalid ID"}
                        });
                }
                else
                {

                    var verification = new DataAccess.Entities.Verification
                    {
                        MainId = main.MainId,
                        Verified = main.Verified,
                        Concern = main.Concern,
                        ConcernCode = main.ConcernCode
                    };

                    return Ok(new ViewModel.ApiMessage<DataAccess.Entities.Verification>
                    {
                        Data = verification,
                        HasErrors = hasErrors,
                        ErrorList = errorList
                    });


                    }

         
                }
            catch (Exception ex)
            {
                hasErrors = true;
                errorList.Add(ex.Message);
            }

            return Ok(new ViewModel.ApiMessage<DataAccess.Entities.Verification>
            {
                Data = null,
                HasErrors = true,
                
            });



        }

        /// <summary>
        /// Postman  
        /// Call with POST 
        /// http://localhost:13497/api/products
        /// Header token send as that is required 
        /// Body -  Raw  - JSON 
        /// However,  do NOT wrap in  { "mainId": 33 } 
        /// Instead ONLY put the value   e.g.    1615    with nothing wrapped around it 
        /// </summary>
        /// <param name="mainId"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/products")]
        public IHttpActionResult AddProduct([FromBody] int mainId)
        {
            // Add product
            return Ok();
        }



        [HttpPost]
        [Route("api/UpdateMainReversed")]
        public IHttpActionResult UpdateMainVerified(DataAccess.Entities.RequestMain mainValues)
        {
            bool hasErrors = false;
            List<string> errorList = new List<string>();
            DataAccess.Entities.Verification verification = new DataAccess.Entities.Verification();

            try
            {
                verification = Business.AppLogic.UpdateMainVerifiedToReverse(mainValues.MainId);
            }
            catch (Exception ex)
            {
                hasErrors = true;
                errorList.Add(ex.Message);
            }
            return Ok(new ViewModel.ApiMessage<DataAccess.Entities.Verification>
            {
                Data = verification,
                HasErrors = hasErrors,
                ErrorList = errorList
            });
        }

        [Route("api/getutilityprograms/{vendorid:int}/{officeid:int}/{state}/{zip}/{creditcheck}/{premisetype:int?}")]
        public IEnumerable<GetUtilityPrograms_Result> GetUtilityPrograms(int vendorid, int officeid, string state, string zip,bool creditcheck,int premisetype=1)
        {
            var utilityPrograms = Business.AppLogic.GetUtilityPrograms(vendorid, officeid, state, zip, creditcheck,premisetype);
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

        [Route("api/vendorzipcode/{vendorNumber}/{zipcode}")]
        public IHttpActionResult GetLeadsByZip(string vendorNumber, string zipcode)
        {
            bool hasErrors = false;
            List<string> errorList = new List<string>();
            List<Lead> lead = null;

            try
            {
                lead = Business.AppLogic.GetLeadsByZip(vendorNumber, zipcode);
                if (lead == null)
                {
                    hasErrors = true;
                    errorList.Add($"Record Locator '{vendorNumber}' Not Found");
                }
            }
            catch (Exception ex)
            {
                hasErrors = true;
                errorList.Add(ex.Message);
            }

            return Ok(new ViewModel.ApiMessage<List<Lead>>
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

        [Route("api/titles/")]
        public IHttpActionResult GetTitlesList()
        {
            bool hasErrors = false;
            List<string> errorList = new List<string>();
            List<DataAccess.Infrastructure.Title> titles = null;

            try
            {
                titles = Business.AppLogic.GetTitles();
                if (titles == null || titles.Count == 0)
                {
                    hasErrors = true;
                    errorList.Add("Titles Not Found");
                }
            }
            catch (Exception ex)
            {
                hasErrors = true;
                errorList.Add(ex.Message);
            }

            return Ok(new ViewModel.ApiMessage<List<DataAccess.Infrastructure.Title>>
            {
                Data = titles,
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
        [Route("api/esiid/{vendorNumber}/{esiid}")]
        public IHttpActionResult GetEsiid(string esiid, string vendorNumber)
        {
            bool hasErrors = false;
            List<string> errorList = new List<string>();
            DataAccess.Infrastructure.Lead lead = null;

            try
            {
                lead = Business.AppLogic.GetEsiid(esiid, vendorNumber);
                if (lead == null)
                {
                    hasErrors = true;
                    errorList.Add($"Record Locator '{esiid}' Not Found");
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
    }

  

    }
