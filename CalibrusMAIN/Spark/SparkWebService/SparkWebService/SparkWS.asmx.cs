using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Xml;

namespace SparkWebService
{
    /// <summary>
    /// Summary description for Service1
    /// </summary>
    [WebService(Namespace = "https://wsssl.calibrus.com/Spark/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line.
    [System.Web.Script.Services.ScriptService]
    public class SparkWS : System.Web.Services.WebService
    {
        public enum FaultCode
        {
            Client = 0,
            Server = 1
        }

        public SparkWS()
        {
            //Uncomment the following line if using designed components
            //InitializeComponent();
        }

        [WebMethod(Description = "Creates a Spark Energy Record", MessageName = "SubmitInformation")]
        public string SubmitRecord(Record record)
        {
            string validationException = string.Empty;
            string recordId = "-1";
            if (!record.ValidateOrder(out validationException))
            {
                throw GenerateSoapException("ValidateOrder", "https://wssl.calibrus.com/SparkWS", validationException, "1000", "ValidateOrder", FaultCode.Client);
            }

            try
            {
                recordId = InsertRecord(ref record);
            }
            catch (Exception ex)
            {
                throw GenerateSoapException("InsertAccount", "https://wssl.calibrus.com/SparkWS", ex.Message, "1001", ex.Source, FaultCode.Server);
            }
            return recordId;
        }

        [WebMethod(Description = "Retrieves Spark Energy Record(s)", MessageName = "RetrieveRecords")]
        public List<TPVRecord> RetrieveRecord(string StartDate, string EndDate, string VendorNumber)
        {
            string validationException = string.Empty;

            List<TPVRecord> tpvRecordList = new List<TPVRecord>();
            if (!Validation(ref StartDate, ref EndDate, ref VendorNumber, out validationException))
            {
                throw GenerateSoapException("ValidateRetrieveRecord", "https://wsssl.calibrus.com/SparkWS", validationException, "1000", "ValidateRetrieveRecord", FaultCode.Client);
            }

            try
            {
                tpvRecordList = GetTPVRecords(StartDate, EndDate, VendorNumber);
            }
            catch (Exception ex)
            {
                throw GenerateSoapException("RetrieveRecord", "https://wsssl.calibrus.com/SparkWS", ex.Message, "2003", ex.Source, FaultCode.Server);
            }

            return (tpvRecordList);
        }

        private bool Validation(ref string sDate, ref string eDate, ref string vendorNumber, out string exception)
        {
            DateTime result;
            if (!DateTime.TryParse(sDate, out result))
            {
                exception = "Invalid StartDate supplied.";
                return false;
            }

            if (!DateTime.TryParse(eDate, out result))
            {
                exception = "Invalid EndDate supplied.";
                return false;
            }

            if (string.IsNullOrEmpty(vendorNumber))
            {
                exception = "Missing VendorNumber supplied.";
                return false;
            }
            exception = "";
            return true;
        }

        #region EntityFramework Methods

        private List<TPVRecord> GetTPVRecords(string sDate, string eDate, string vendorNumber)
        {
            List<TPVRecord> tpvRecords = new List<TPVRecord>();

            try
            {
                DateTime StartDate = Convert.ToDateTime(sDate);
                DateTime EndDate = Convert.ToDateTime(eDate);
                using (SparkEntities entitites = new SparkEntities())
                {
                    var query = (from m in entitites.Mains
                                 join od in entitites.OrderDetails on m.MainId equals od.MainId
                                 join p in entitites.Programs on od.ProgramId equals p.ProgramId
                                 join uty in entitites.Utilities on p.UtilityId equals uty.UtilityId
                                 join utytype in entitites.UtilityTypes on p.UtilityTypeId equals utytype.UtilityTypeId
                                 join ant in entitites.AccountNumberTypes on p.AccountNumberTypeId equals ant.AccountNumberTypeId
                                 join u in entitites.Users on m.UserId equals u.UserId
                                 join v in entitites.Vendors on u.VendorId equals v.VendorId
                                 join pt in entitites.PremiseTypes on p.PremiseTypeId equals pt.PremiseTypeId

                                 where m.CallDateTime > StartDate && m.CallDateTime < EndDate
                                 && v.VendorNumber == vendorNumber
                                 select new
                                 {
                                     CalibrusRecordLocator = m.MainId,
                                     CallDateTime = m.CallDateTime,
                                     WebDateTime = m.WebDateTime,
                                     Verified = m.Verified,
                                     Concern = m.Concern,
                                     ConcernCode = m.ConcernCode,
                                     TpvAgentName = m.TpvAgentName,
                                     TpvAgentId = m.TpvAgentId,
                                     AgentId = u.AgentId,
                                     AgentFirstName = u.FirstName,
                                     AgentLastName = u.LastName,
                                     Email = m.Email,
                                     AuthorizationFirstName = m.AuthorizationFirstName,
                                     AuthorizationLastName = m.AuthorizationLastName,
                                     AccountFirstName = m.AccountFirstName,
                                     AccountLastName = m.AccountLastName,
                                     Relation = m.Relation,
                                     Btn = m.Btn,
                                     AccountNumber = od.AccountNumber,
                                     NameKey = od.CustomerNameKey,
                                     ServiceAddress = od.ServiceAddress,
                                     ServiceCity = od.ServiceCity,
                                     ServiceState = od.ServiceState,
                                     ServiceCounty = od.ServiceCounty,
                                     ServiceZip = od.ServiceZip,
                                     BillingAddress = od.ServiceAddress,
                                     BillingCity = od.BillingCity,
                                     BillingState = od.BillingState,
                                     BillingCounty = od.BillingCounty,
                                     BillingZip = od.BillingZip,
                                     ProgramCode = p.ProgramCode,
                                     ProgramName = p.ProgramName,
                                     MSF = p.Msf,
                                     ETF = p.Etf,
                                     Rate = p.Rate,
                                     Term = p.Term,
                                     UtilityType = utytype.UtilityTypeName,
                                     PremiseType = pt.PremiseTypeName,
                                     State = p.State,
                                     LdcCode = uty.LdcCode,
                                     AccountNumType = ant.AccountNumberTypeName,
                                     BillingFirstName = od.BillingFirstName,
                                     BillingLastName = od.BillingLastName
                                 }).ToList();

                    foreach (var item in query)
                    { 

                        TPVRecord tpvrecord = new TPVRecord(item.CalibrusRecordLocator,
                                                            item.CallDateTime,
                                                            item.WebDateTime,
                                                            item.Verified,
                                                            item.Concern,
                                                            item.ConcernCode,
                                                            item.TpvAgentName,
                                                            item.TpvAgentId,
                                                            item.AgentId,
                                                            item.AgentFirstName,
                                                            item.AgentLastName,
                                                            item.Email,
                                                            item.AuthorizationFirstName,
                                                            item.AuthorizationLastName,
                                                            item.AccountFirstName,
                                                            item.AccountLastName,
                                                            item.Relation,
                                                            item.Btn,
                                                            item.AccountNumber,
                                                            item.NameKey,
                                                            item.ServiceAddress,
                                                            item.ServiceCity,
                                                            item.ServiceCounty,
                                                            item.ServiceState,
                                                            item.ServiceZip,
                                                            item.BillingAddress,
                                                            item.BillingCity,
                                                            item.BillingCounty,
                                                            item.BillingState,
                                                            item.BillingZip,
                                                            item.ProgramCode,
                                                            item.ProgramName,
                                                            item.MSF,
                                                            item.ETF,
                                                            item.Rate,
                                                            item.Term,
                                                            item.UtilityType,
                                                            item.PremiseType,
                                                            item.State,
                                                            item.LdcCode,
                                                            item.AccountNumType,
                                                            item.BillingFirstName,
                                                            item.BillingLastName);
                        tpvRecords.Add(tpvrecord);
                    }
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
                throw ex;
            }
            return tpvRecords;
        }

        private string InsertRecord(ref Record record)
        {
            int? MainId = null;

            try
            {
                Main main = null;
                OrderDetail orderdetail = null;
                using (SparkEntities data = new SparkEntities())
                {
                    //do a lookup for a match to get UserId
                    int userId = GetUserId(record.AgentId, record.VendorNumber);

                    main = new Main();

                    main.UserId = userId; //UserId from the GetUserId() method

                    main.Email = record.Email;
                    main.RecordLocator = record.RecordLocator;
                    main.SalesState = record.SalesState;
                    main.AuthorizationFirstName = record.AuthorizationFirstName;
                    main.AuthorizationMiddle = record.AuthorizationMiddle;
                    main.AuthorizationLastName = record.AuthorizationLastName;
                    main.Btn = StripAllNonNumerics(record.Btn);
                    main.CompanyName = record.CompanyName;
                    main.CompanyContactFirstName = record.CompanyContactFirstName;
                    main.CompanyContactLastName = record.CompanyContactLastName;
                    main.CompanyContactTitle = record.CompanyContactTitle;
                    main.Territory = record.Territory;
                    main.Relation = record.Relation;
                    main.NumberOfAccounts = record.NumberOfAccounts;
                    main.AccountFirstName = record.AuthorizationFirstName;
                    main.AccountLastName = record.AuthorizationLastName;
                    main.SourceId = 3; //API


                    //need to step through the RecordDetail data
                    foreach (RecordDetail recordDetail in record.RecordDetails)
                    {
                        //do a lookup for a match to get PlanId
                        int programId = GetProgramId(recordDetail.ProgramCode, record.VendorNumber);

                        //do a lookup for a match to get RateClass
                        string rateClass = GetRateClass(recordDetail.ProgramCode);

                        //do a lookup for County based on zip
                        string serviceCounty = CountyLookUp(recordDetail.ServiceZip);
                        string billingCounty = CountyLookUp(recordDetail.BillingZip);
                        orderdetail = new OrderDetail();

                        string nameKey = string.Empty;
                        //Get nameKey
                        if (string.IsNullOrEmpty(recordDetail.CustomerNameKey))
                        {
                            nameKey = recordDetail.BillingLastName.Substring(0, 4);
                        }
                        else 
                        {
                            nameKey = recordDetail.CustomerNameKey;
                        }


                        //orderdetail.MainId = MainId;//MainId from previous insert into Main table of the Record data
                        orderdetail.ProgramId = programId; //ProgramId from the GetProgramId() method

                        orderdetail.UtilityType = recordDetail.UtilityType;
                        orderdetail.AccountType = recordDetail.AccountType;
                        orderdetail.AccountNumber = recordDetail.AccountNumber.ToUpper();
                        orderdetail.MeterNumber = recordDetail.MeterNumber;
                        orderdetail.RateClass = recordDetail.RateClass;
                        orderdetail.CustomerNameKey = nameKey.ToUpper();
                        orderdetail.ServiceReferenceNumber = recordDetail.ServiceReferenceNumber;
                        orderdetail.ServiceAddress = recordDetail.ServiceAddress;
                        orderdetail.ServiceCity = recordDetail.ServiceCity;
                        orderdetail.ServiceState = recordDetail.ServiceState;
                        orderdetail.ServiceZip = recordDetail.ServiceZip;
                        orderdetail.ServiceCounty = serviceCounty;
                        orderdetail.BillingAddress = recordDetail.BillingAddress;
                        orderdetail.BillingCity = recordDetail.BillingCity;
                        orderdetail.BillingState = recordDetail.BillingState;
                        orderdetail.BillingZip = recordDetail.BillingZip;
                        orderdetail.BillingCounty = billingCounty;
                        orderdetail.InCityLimits = recordDetail.InCityLimits;
                        orderdetail.RateClass = rateClass;
                        orderdetail.BillingFirstName = recordDetail.BillingFirstName;
                        orderdetail.BillingLastName = recordDetail.BillingLastName;


                       

                        if (orderdetail != null)
                        {
                            main.OrderDetails.Add(orderdetail);
                        }
                    }
                    data.AddToMains(main);
                    data.SaveChanges();

                    MainId = main.MainId;//get mainId of record to use for the OrderDetail record insert below
                    data.Connection.Close();
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
                throw ex;
            }
            return string.Format("A{0}",MainId.Value);
        }

        private int GetUserId(string agentId, string vendorNumber)
        {
            //SELECT u.UserId, v.VendorId, v.VendorNumber
            //FROM [Spark].[v1].[User] as u
            //join [Spark].[v1].[Vendor] as v on v.VendorId = u.VendorId
            //where u.AgentId = 'mjrae2'
            //and v.VendorNumber = '91'
            //and u.IsActive = true

            int userid = 0;
            using (SparkEntities data = new SparkEntities())
            {
                userid = (from u in data.Users
                          join v in data.Vendors on u.VendorId equals v.VendorId
                          where u.AgentId == agentId
                          && v.VendorNumber == vendorNumber
                          && u.IsActive == true
                          select u.UserId).SingleOrDefault();
            }
            return userid;
        }

        private int GetProgramId(string programCode, string vendorNumber)
        {
            DateTime now = DateTime.Now;

            //SELECT p.programid, pv.VendorId, v.VendorId, v.VendorNumber
            //FROM [Spark].[v1].[Program] as p
            //join [Spark].[v1].[ProgramVendor] as pv on p.ProgramId = pv.ProgramId
            //join [Spark].[v1].[Vendor] as v on v.VendorId = pv.VendorId
            //where p.ProgramCode = 'A3'
            //and v.VendorNumber = '1001'
            //and (p.EffectiveStartDate < getdate() and p.EffectiveEndDate > getdate())

            int programid = 0;
            using (SparkEntities data = new SparkEntities())
            {
                programid = (from p in data.Programs
                             join pv in data.ProgramVendors on p.ProgramId equals pv.ProgramId
                             join v in data.Vendors on pv.VendorId equals v.VendorId
                             where p.ProgramCode == programCode
                             && v.VendorNumber == vendorNumber
                             && p.EffectiveStartDate  < now
                             && p.EffectiveEndDate > now
                             select p.ProgramId).SingleOrDefault();
            }
            return programid;
        }

        private string GetRateClass(string programCode)
        {
            //SELECT p.programid,p.ProgramCode,u.LdcCode, ut.UtilityTypeName
            //FROM [Spark].[v1].[Program] as p
            //Join [Spark].[v1].[Utility] as u on u.UtilityId = p.UtilityId
            //Join [Spark].[v1].[UtilityType] as ut on ut.UtilityTypeId = p.UtilityTypeId
            //where p.ProgramCode = '612'

            string rateclass = string.Empty;
            using (SparkEntities data = new SparkEntities())
            {
                var programlookup = (from p in data.Programs
                                     join u in data.Utilities on p.UtilityId equals u.UtilityId
                                     join ut in data.UtilityTypes on p.UtilityTypeId equals ut.UtilityTypeId
                                     where p.ProgramCode == programCode
                                     select new
                                     {
                                         LdcCode = u.LdcCode,
                                         UtilityTypeName = ut.UtilityTypeName
                                     }).FirstOrDefault();

                string rateclasslookup = (from rc in data.RateClassLookups
                                          where rc.LdcCode == programlookup.LdcCode
                                         && rc.UtilityTypeName == programlookup.UtilityTypeName
                                          select rc.RateClass).FirstOrDefault();

                if (!string.IsNullOrEmpty(rateclasslookup))
                {
                    rateclass = rateclasslookup.ToString();
                }
            }
            return rateclass;
        }

        #region EF Method to return County based on Zip Code (1 method)

        /// <summary>
        /// Looks up County based on ZipCode passed in
        /// </summary>
        /// <param name="zipcode"></param>
        /// <returns></returns>
        private string CountyLookUp(string zipcode)
        {
            //Select County
            //FROM [Spark].[v1].[ZipCodeLookup]
            //Where ZipCode = '85015'

            string county = string.Empty;
            using (SparkEntities data = new SparkEntities())
            {
                var countyLookUp = (from c in data.ZipCodeLookups
                                    where c.ZipCode == zipcode
                                    select c.County).SingleOrDefault();
                county = countyLookUp;
            }

            return county;
        }

        #endregion EF Method to return County based on Zip Code (1 method)

        #endregion EntityFramework Methods

        #region Soap Exception

        private SoapException GenerateSoapException(string uri, string webserviceNamespace, string errorMessage, string errorNumber, string errorSource, FaultCode code)
        {
            XmlQualifiedName faultCodeLocation = null;
            switch (code)
            {
                case FaultCode.Client:
                    faultCodeLocation = SoapException.ClientFaultCode;
                    break;

                case FaultCode.Server:
                    faultCodeLocation = SoapException.ServerFaultCode;
                    break;
            }

            XmlDocument xmlDoc = new XmlDocument();

            //Create the Detail node
            XmlNode rootNode = xmlDoc.CreateNode(XmlNodeType.Element, SoapException.DetailElementName.Name, SoapException.DetailElementName.Namespace);

            //Build specific details for the SoapException
            //Add first child of detail XML element.
            XmlNode errorNode = xmlDoc.CreateNode(XmlNodeType.Element, "Error", webserviceNamespace);

            //Create and set the value for the ErrorNumber node
            XmlNode errorNumberNode = xmlDoc.CreateNode(XmlNodeType.Element, "ErrorNumber", webserviceNamespace);
            errorNumberNode.InnerText = errorNumber;

            //Create and set the value for the ErrorMessage node
            XmlNode errorMessageNode = xmlDoc.CreateNode(XmlNodeType.Element, "ErrorMessage", webserviceNamespace);
            errorMessageNode.InnerText = errorMessage;

            //Create and set the value for the ErrorSource node
            XmlNode errorSourceNode = xmlDoc.CreateNode(XmlNodeType.Element, "ErrorSource", webserviceNamespace);
            errorSourceNode.InnerText = errorSource;

            //Append the Error child element nodes to the root detail node.
            errorNode.AppendChild(errorNumberNode);
            errorNode.AppendChild(errorMessageNode);
            errorNode.AppendChild(errorSourceNode);

            //Append the Detail node to the root node
            rootNode.AppendChild(errorNode);

            //Construct the exception
            SoapException soapEx = new SoapException(errorMessage, faultCodeLocation, uri, rootNode);

            //Raise the exception  back to the caller
            return soapEx;
        }

        #endregion Soap Exception

        #region Utilities

        public static string StripAllNonNumerics(string input)
        {
            if (!string.IsNullOrEmpty(input))
            {
                input = Regex.Replace(input, @"[^\d]", "");// strip all non-numeric chars
                return input;
            }
            return string.Empty;
        }

        #endregion Utilities

        #region ErrorHandling

        private static void ErrorHandler(Exception ex)
        {
            Calibrus.ErrorHandler.Alerting alert = new Calibrus.ErrorHandler.Alerting("SparkWebService");
            alert.SendAlert(ex.Source, ex.Message, Environment.MachineName, Environment.UserName, Environment.Version.ToString());
        }

        private static void ErrorHandler(Exception ex, string recId)
        {
            Calibrus.ErrorHandler.Alerting alert = new Calibrus.ErrorHandler.Alerting("SparkWebService");
            alert.SendAlert(ex.Source, String.Format("tblMain Record Locator: {0} -- {1}", recId, ex.Message), Environment.MachineName, Environment.UserName, Environment.Version.ToString());
        }

        private static void LogError(Exception ex)
        {
            Calibrus.ErrorHandler.Logging log = new Calibrus.ErrorHandler.Logging();
            log.LogError("SparkWebService", Environment.Version.ToString(), Environment.MachineName, Environment.UserName, ex.Source, ex.Message);
        }

        private static void LogError(Exception ex, string recId)
        {
            Calibrus.ErrorHandler.Logging log = new Calibrus.ErrorHandler.Logging();
            log.LogError("SparkWebService", Environment.Version.ToString(), Environment.MachineName, Environment.UserName, ex.Source,
                String.Format("tblMain Record Locator: {0} -- {1}", recId, ex.Message));
        }

        #endregion ErrorHandling
    }
}