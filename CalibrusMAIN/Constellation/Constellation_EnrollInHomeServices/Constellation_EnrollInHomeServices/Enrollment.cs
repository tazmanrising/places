using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using Calibrus.ErrorHandler;


namespace Constellation_EnrollInHomeServices
{
    public class Enrollment
    {

        #region Main

        public static void Main(string[] args)
        {
            string VendorName = string.Empty;
            string GlobalAPIKey = string.Empty;
            string ProtocallAPIKey = string.Empty;
            string BaseAddress = string.Empty;
            string Username = string.Empty;
            string Password = string.Empty;

            Int32 intRecordLocator = 0;

            //need to finish the ad hoc
            if (args.Length > 0) //if you pass in a recordlocator to run ad hoc
            {
                if (!Int32.TryParse(args[0], out intRecordLocator))
                    throw new ArgumentException("Record Locator Parameter is Invalid");
            }
            else
            {
                int tblHomeServicesResponseId = 0;//tblHomeServicesResponse.HomeServicesResponseId
                int tblHomeServicesId = 0;//tblHomeServices.HomeServicesId as found in tblHomeServices and tblHomeServicesResponse

                //***TO DO***
                //grab the record from command line Ad Hoc request
                //***TO DO***

                //or

                //grab records from the DB 
                //where ResponseSent==0(record hasn't been updated) 
                try
                {
                    //Get Values from Config File
                    GlobalAPIKey = ConfigurationManager.AppSettings["GlobalAPIKey"].ToString();
                    ProtocallAPIKey = ConfigurationManager.AppSettings["ProtocallAPIKey"].ToString();
                    BaseAddress = ConfigurationManager.AppSettings["BaseAddress"].ToString();
                    Username = ConfigurationManager.AppSettings["Username"].ToString();
                    Password = ConfigurationManager.AppSettings["Password"].ToString();

                    foreach (tblHomeServicesResponse responseToSend in GetHomeServicesResponseRecords())
                    {

                        tblHomeServicesResponseId = responseToSend.HomeServicesResponseId;
                        tblHomeServicesId = responseToSend.HomeServicesId;

                        Record homeServicesEnrollmentToSend = GetHomeServicesEnrollmentRecord(tblHomeServicesId, ref VendorName);

                        string apiKey = string.Empty;

                        switch (VendorName)
                        {
                            case "Global":
                                apiKey = GlobalAPIKey;
                                break;
                            case "Protocall":
                                apiKey = ProtocallAPIKey;
                                break;
                        }

                        //Build and Send the POST with Serialized JSON of Record object returns a Task<string>
                        var operationResult = BuildResponse(homeServicesEnrollmentToSend, apiKey, BaseAddress, Username, Password);


                        //var testSend = Newtonsoft.Json.JsonConvert.SerializeObject(homeServicesEnrollmentToSend);
                        //var testresponse = Newtonsoft.Json.JsonConvert.SerializeObject(operationResult);

                        var result = Newtonsoft.Json.JsonConvert.DeserializeObject<OperationResult>(operationResult.Result.ToString());

                        if (result.IsSuccess)
                        {
                            //Insert Response into tblHomeServicesResponse
                            UpdateHomeServicesResponseRecord(tblHomeServicesResponseId, "1", "Success", Newtonsoft.Json.JsonConvert.SerializeObject(homeServicesEnrollmentToSend), Newtonsoft.Json.JsonConvert.SerializeObject(result));
                        }
                        else
                        {
                            UpdateHomeServicesResponseRecord(tblHomeServicesResponseId, "9", "Failed - check missing data per business rules", Newtonsoft.Json.JsonConvert.SerializeObject(homeServicesEnrollmentToSend), Newtonsoft.Json.JsonConvert.SerializeObject(result));
                        }
                    }

                }
                catch (Exception ex)
                {
                    ErrorHandler(ex, tblHomeServicesResponseId.ToString(), tblHomeServicesId.ToString());
                    UpdateHomeServicesResponseRecord(tblHomeServicesResponseId, "9", "Failed record for HomeServicesId: " + tblHomeServicesId + " check error log");
                }

            }
        }

        #endregion Main

        #region BuildResponse

        private static async Task<string> BuildResponse(Record enrollmentRecord, string APIKey, string BaseAddress, string Username, string Password)
        {

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BaseAddress);

                client.DefaultRequestHeaders.Accept.Clear();
                byte[] useridPasswordArray = Encoding.ASCII.GetBytes(String.Format("{0}:{1}", Username, Password));

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(useridPasswordArray));
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                return client.PostAsJsonAsync(APIKey, enrollmentRecord).Result.Content.ReadAsStringAsync().Result;
            }

        }

        //private static async Task<string> BuildResponse(Record enrollmentRecord, string APIKey, string BaseAddress, string Username, string Password)
        //{

        //    using (var client = new HttpClient())
        //    {
        //        client.BaseAddress = new Uri(BaseAddress);

        //        client.DefaultRequestHeaders.Accept.Clear();
        //        byte[] useridPasswordArray = Encoding.ASCII.GetBytes(String.Format("{0}:{1}", Username, Password));

        //        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(useridPasswordArray));
        //        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        //        var response = client.PostAsJsonAsync(APIKey, enrollmentRecord).Result;
        //        var contents = await response.Content.ReadAsStringAsync();


        //        //var testSend = Newtonsoft.Json.JsonConvert.SerializeObject(enrollmentRecord);
        //        //var testresponse = Newtonsoft.Json.JsonConvert.SerializeObject(response);


        //        return contents;

        //    }

        //}

        #endregion BuildResponse

        #region Entity Framework Methods

        #region Get Data

        /// <summary>
        /// Gets a list of all pending records that have not been updated
        /// </summary>
        /// <returns>List of tblHomeServicesResponses</returns>
        private static List<tblHomeServicesResponse> GetHomeServicesResponseRecords()
        {
            List<tblHomeServicesResponse> ctxRecords = new List<tblHomeServicesResponse>();
            try
            {
                using (ConstellationEntities entities = new ConstellationEntities())
                {
                    ctxRecords = entities.tblHomeServicesResponses
                        .Where(x => x.ResponseSent == "0").ToList();
                }
            }
            catch (Exception ex)
            {
                ErrorHandler(ex);
            }
            return ctxRecords;
        }


        private static Record GetHomeServicesEnrollmentRecord(int homeServicesId, ref string VendorName)
        {
            Record homeServicesEnrollmentRecord = new Record();

            //SELECT hs.ResponseId, hs.VendorAgentId, m.Concern, m.MainId, m.UDCCode as UDC, m.PlanId, hsp.HomeServicesPlan as ServiceContractPromotion, m.PromoCode,  hs.HomeServicesId as ConfirmationNumber, v.VendorName, m.SignUpType as Commodity, hs.ServiceEmail, m.ServiceEmail,hs.ServiceFirstName, m.ServiceFirstName,
            //        hs.ServiceLastName, m.ServiceLastName,	m.UDCAccountNumber as UtilityAccountNumber, hs.ElectricChoiceId as UtilityAccountNumber2,hs.ServiceAddress1, m.ServiceAddress1, hs.ServiceAddress2, 
            //        m.ServiceAddress2, hs.ServiceCity , m.ServiceCity, hs.ServiceState, m.ServiceState, hs.ServiceZipCode, m.ServiceZipCode, hs.ServicePhoneNumber, m.ServicePhoneNumber, hs.BillingAddress1, 
            //        m.BillingAddress1, hs.BillingAddress2 , m.BillingAddress2, hs.BillingCity , m.BillingCity, hs.BillingState , m.BillingState, hs.BillingZipCode, 
            //        m.BillingZipCode
            //FROM [Constellation].[dbo].[tblHomeServices] hs
            //JOIN [Constellation].[dbo].[tblHomeServicesPlan] hsp on hs.HomeServicesPlanId = hsp.HomeServicesPlanId
            //JOIN [Constellation].[dbo].[tblVendor] v on v.VendorId = hs.VendorId
            //JOIN [Constellation].[dbo].[tblMain] m on m.HomeServicesId = hs.HomeServicesId
            //WHERE m.Verified ='1'

            //Possible check for UDC
            //select * 
            //FROM [Constellation].[dbo].[tblHomeServices] hs 
            //where IncludeOnBGEBill =1 --then udc code may be hardcoded as bge
            //order by HomeServicesId desc
            try
            {
                using (ConstellationEntities entities = new ConstellationEntities())
                {
                    var query = (from hs in entities.tblHomeServices
                                 join hsp in entities.tblHomeServicesPlans on hs.HomeServicesPlanId equals hsp.HomeServicesPlanId
                                 join v in entities.tblVendors on hs.VendorId equals v.VendorId
                                 join m in entities.tblMains on hs.HomeServicesId equals m.HomeServicesId
                                 where hs.HomeServicesId == homeServicesId
                                 && m.Verified == "1"
                                 select new
                                 {
                                     VendorName = v.VendorName,
                                     BillingAddress1 = hs.BillingAddress1 ?? m.BillingAddress1,
                                     BillingAddress2 = hs.BillingAddress2 ?? m.BillingAddress2,
                                     BillingCity = hs.BillingCity ?? m.BillingCity,
                                     BillingState = hs.BillingState ?? m.BillingState,
                                     BillingZip = hs.BillingZipCode ?? m.BillingZipCode,
                                     CellPhoneNumber = string.Empty,
                                     //Channel = "IBTM",
                                     Dnis = m.Dnis,
                                     Comment = string.Empty,
                                     Commodity = (m.SignUpType == "HS" ? string.Empty : m.SignUpType),
                                     ConfirmationNumber = hs.HomeServicesId,
                                     Email = hs.ServiceEmail ?? m.ServiceEmail,
                                     FirstName = hs.ServiceFirstName ?? m.ServiceFirstName,
                                     LastName = hs.ServiceLastName ?? m.ServiceLastName,
                                     HeatingAndCoolingAge = string.Empty,
                                     HeatingAndCoolingEquipment = string.Empty,
                                     HeatingAndCoolingMake = string.Empty,
                                     HeatingAndCoolingAge2 = string.Empty,
                                     HeatingAndCoolingEquipment2 = string.Empty,
                                     HeatingAndCoolingMake2 = string.Empty,
                                     HeatingAndCoolingAge3 = string.Empty,
                                     HeatingAndCoolingEquipment3 = string.Empty,
                                     HeatingAndCoolingMake3 = string.Empty,
                                     UDC = m.UDCCode ?? "",
                                     PhoneNumber = hs.ServicePhoneNumber ?? m.ServicePhoneNumber,
                                     PlanId = hsp.HomeServicesPlanValueId ?? "",
                                     PromoCode = m.PromoCode ?? "",
                                     SalesAgentId = hs.VendorAgentId ?? m.VendorAgentId,
                                     ServiceAddress1 = hs.ServiceAddress1 ?? m.ServiceAddress1,
                                     ServiceAddress2 = hs.ServiceAddress2 ?? m.ServiceAddress2,
                                     ServiceCity = hs.ServiceCity ?? m.ServiceCity,
                                     ServiceState = hs.ServiceState ?? m.ServiceState,
                                     ServiceZip = hs.ServiceZipCode ?? m.ServiceZipCode,
                                     ServiceContractPromotion = hsp.HomeServicesPlanValue,//"Water Heater Protection Plan", "Smart Service Home Comfort",//"Select Comfort",//hsp.HomeServicesPlanValue,
                                     UtilityAccountNumber = hs.ElectricChoiceId ?? "",
                                     OnBillConsent = (hs.IncludeOnBGEBill != null) ? true : false

                                 }).FirstOrDefault();

                    VendorName = query.VendorName;

                    string bAddress = string.Format("{0} {1}", query.BillingAddress1, query.BillingAddress2);
                    Record.Address billAddress = new Record.Address();
                    billAddress.AddressLine = bAddress.Trim();
                    billAddress.City = query.BillingCity;
                    billAddress.State = query.BillingState;
                    billAddress.Zip = query.BillingZip;

                    homeServicesEnrollmentRecord.BillingAddress = billAddress;

                    homeServicesEnrollmentRecord.OnBillConsent = query.OnBillConsent;

                    homeServicesEnrollmentRecord.CellPhoneNumber = query.PhoneNumber;

                    string salesChannel = string.Empty;
                    switch (query.Dnis.Trim())
                    {
                        case "2277":
                        case "2212":
                            salesChannel = "IBTM";
                            break;
                        default:
                            salesChannel = "OBTM";
                            break;
                    }
                    homeServicesEnrollmentRecord.Channel = salesChannel;
                    homeServicesEnrollmentRecord.Comment = query.Comment;
                    homeServicesEnrollmentRecord.Commodity = query.Commodity;
                    homeServicesEnrollmentRecord.ConfirmationNumber = query.ConfirmationNumber.ToString();
                    homeServicesEnrollmentRecord.Email = query.Email;
                    homeServicesEnrollmentRecord.FirstName = query.FirstName;
                    homeServicesEnrollmentRecord.LastName = query.LastName;
                    homeServicesEnrollmentRecord.HeatingAndCoolingAge = query.HeatingAndCoolingAge;
                    homeServicesEnrollmentRecord.HeatingAndCoolingEquipment = query.HeatingAndCoolingEquipment;
                    homeServicesEnrollmentRecord.HeatingAndCoolingMake = query.HeatingAndCoolingMake;
                    homeServicesEnrollmentRecord.HeatingAndCoolingAge2 = query.HeatingAndCoolingAge2;
                    homeServicesEnrollmentRecord.HeatingAndCoolingEquipment2 = query.HeatingAndCoolingEquipment2;
                    homeServicesEnrollmentRecord.HeatingAndCoolingMake2 = query.HeatingAndCoolingMake2;
                    homeServicesEnrollmentRecord.HeatingAndCoolingAge3 = query.HeatingAndCoolingAge3;
                    homeServicesEnrollmentRecord.HeatingAndCoolingEquipment3 = query.HeatingAndCoolingEquipment3;
                    homeServicesEnrollmentRecord.HeatingAndCoolingMake3 = query.HeatingAndCoolingMake3;
                    homeServicesEnrollmentRecord.UDC = query.UDC;
                    homeServicesEnrollmentRecord.PhoneNumber = query.PhoneNumber;
                    homeServicesEnrollmentRecord.PlanId = query.PlanId;
                    homeServicesEnrollmentRecord.PromoCode = query.PromoCode;
                    homeServicesEnrollmentRecord.SalesAgentId = query.SalesAgentId;

                    string sAddress = string.Format("{0} {1}", query.ServiceAddress1, query.ServiceAddress2);
                    Record.Address servAddress = new Record.Address();
                    servAddress.AddressLine = sAddress.Trim();
                    servAddress.City = query.ServiceCity;
                    servAddress.State = query.ServiceState;
                    servAddress.Zip = query.ServiceZip;

                    homeServicesEnrollmentRecord.ServiceAddress = servAddress;

                    homeServicesEnrollmentRecord.ServiceContractPromotion = query.ServiceContractPromotion;
                    homeServicesEnrollmentRecord.UtilityAccountNumber = query.UtilityAccountNumber;

                }
            }
            catch (Exception ex)
            {
                ErrorHandler(ex);
            }

            return homeServicesEnrollmentRecord;
        }



        #endregion Get Data

        #region Update Data

        /// <summary>
        /// Method to update tblHomeServicesResponse with a status of 1 with a success message
        /// </summary>
        /// <param name="tblHomeServicesResponseId"></param>
        /// <param name="responseSent"></param>
        /// <param name="status"></param>
        /// <param name="objectResult"></param>
        private static void UpdateHomeServicesResponseRecord(int tblHomeServicesResponseId, string responseSent, string status, string objectSent, string operationResult)
        {
            try
            {

                tblHomeServicesResponse homeServicesResponse = null;
                using (ConstellationEntities entities = new ConstellationEntities())
                {

                    homeServicesResponse = (from record in entities.tblHomeServicesResponses
                                            where record.HomeServicesResponseId == tblHomeServicesResponseId
                                            select record).FirstOrDefault();

                    homeServicesResponse.ResponseSent = responseSent;
                    homeServicesResponse.Status = status;
                    homeServicesResponse.ObjectSent = objectSent;
                    homeServicesResponse.OperationResult = operationResult;
                    homeServicesResponse.ResponseDateTime = DateTime.Now;
                    entities.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                ErrorHandler(ex, tblHomeServicesResponseId.ToString());

            }

        }

        /// <summary>
        /// Method to update tblHomeServicesResponse when the process fails via an exception thrown, this will update the record as a 9 and set the status to failed
        /// </summary>
        /// <param name="tblHomeServicesResponseId"></param>
        /// <param name="responseSent"></param>
        /// <param name="status"></param>
        private static void UpdateHomeServicesResponseRecord(int tblHomeServicesResponseId, string responseSent, string status)
        {
            try
            {
                tblHomeServicesResponse homeServicesResponse = null;
                using (ConstellationEntities entities = new ConstellationEntities())
                {

                    homeServicesResponse = (from record in entities.tblHomeServicesResponses
                                            where record.HomeServicesResponseId == tblHomeServicesResponseId
                                            select record).FirstOrDefault();

                    homeServicesResponse.ResponseSent = responseSent;
                    homeServicesResponse.Status = status;
                    homeServicesResponse.ResponseDateTime = DateTime.Now;
                    entities.SaveChanges();

                }
            }
            catch (Exception ex)
            {
                ErrorHandler(ex, tblHomeServicesResponseId.ToString());
            }

        }
        #endregion Update Data

        #endregion Entity Framework Methods

        #region Utilities

        //Base 64 convert routine
        public static string ConverToBase64String(string input)
        {
            //Encoding is in the System.Text Namespace
            byte[] info = Encoding.ASCII.GetBytes(input);
            //Convert the binary input into base 64 UUEncode output.
            //Each 3 byte sequence in the source data becomes a 4 byte
            //sequence in the character array.
            long dataLength = (long)((4.0d / 3.0d) * info.Length);
            //if length is not divisible by 4, go up to the next multiple of 4.
            if (dataLength % 4 != 0)
                dataLength += 4 - dataLength % 4;
            //Allocate the output buffer
            char[] base64CharArray = new char[dataLength];
            //converting.... (Convert is in the system namespace)
            Convert.ToBase64CharArray(info, 0, info.Length, base64CharArray, 0);
            //display the converted data
            return new string(base64CharArray);
        }
        #endregion Utilities

        #region ErrorHandling

        static void ErrorHandler(Exception ex)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("ex:{0}, innerEx:{1}", ex.Message, ex.InnerException == null ? "" : ex.InnerException.Message);

            Calibrus.ErrorHandler.Alerting alert = new Calibrus.ErrorHandler.Alerting("Constellation_EnrollInHomeServices");
            alert.SendAlert(ex.Source, sb.ToString(), Environment.MachineName, Environment.UserName, Environment.Version.ToString());
        }

        static void ErrorHandler(Exception ex, string tblHomeServicesResponseId)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("tblHomeServicesResponseId:{0}, ex:{1}, innerEx:{2}", tblHomeServicesResponseId, ex.Message, ex.InnerException == null ? "" : ex.InnerException.Message);

            Calibrus.ErrorHandler.Alerting alert = new Calibrus.ErrorHandler.Alerting("Constellation_EnrollInHomeServices");
            alert.SendAlert(ex.Source, sb.ToString(), Environment.MachineName, Environment.UserName, Environment.Version.ToString());
        }

        static void ErrorHandler(Exception ex, string tblHomeServicesResponseId, string tblHomeServicesId)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("tblHomeServicesResponseId:{0} tblHomeServicesId:{1}, ex:{2}, innerEx:{3}", tblHomeServicesResponseId, tblHomeServicesId, ex.Message, ex.InnerException == null ? "" : ex.InnerException.Message);
            
            Calibrus.ErrorHandler.Alerting alert = new Calibrus.ErrorHandler.Alerting("Constellation_EnrollInHomeServices");
            alert.SendAlert(ex.Source, sb.ToString(), Environment.MachineName, Environment.UserName, Environment.Version.ToString());
        }

        static void LogError(Exception ex)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("ex:{0}, innerEx:{1}", ex.Message, ex.InnerException == null ? "" : ex.InnerException.Message);

            Calibrus.ErrorHandler.Logging log = new Calibrus.ErrorHandler.Logging();
            log.LogError("Constellation_EnrollInHomeServices", Environment.Version.ToString(), Environment.MachineName, Environment.UserName, ex.Source, sb.ToString());
        }

        static void LogError(Exception ex, string tblHomeServicesResponseId)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("tblHomeServicesResponseId:{0}, ex:{1}, innerEx:{2}", tblHomeServicesResponseId, ex.Message, ex.InnerException == null ? "" : ex.InnerException.Message);


            Calibrus.ErrorHandler.Logging log = new Calibrus.ErrorHandler.Logging();
            log.LogError("Constellation_EnrollInHomeServices", Environment.Version.ToString(), Environment.MachineName, Environment.UserName, ex.Source, sb.ToString());
        }
        static void LogError(Exception ex, string tblHomeServicesResponseId, string tblHomeServicesId)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("tblHomeServicesResponseId:{0} tblHomeServicesId:{1}, ex:{2}, innerEx:{3}", tblHomeServicesResponseId, tblHomeServicesId, ex.Message, ex.InnerException == null ? "" : ex.InnerException.Message);

            Calibrus.ErrorHandler.Logging log = new Calibrus.ErrorHandler.Logging();
            log.LogError("Constellation_EnrollInHomeServices", Environment.Version.ToString(), Environment.MachineName, Environment.UserName, ex.Source, sb.ToString());
        }
        #endregion
    }
}