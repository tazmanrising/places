using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.Configuration;
using System.Net;
using System.IO;
using System.Web;
using Calibrus.ErrorHandler;


namespace ConstellationSecurityCheck
{
    public class SecurityCheck
    {
        public enum WhichSecurityCheck
        {
            Infutor,
            Experian
        }
        #region Main

        public static void Main(string[] args)
        {
            string InfutorEndpoint = string.Empty;
            string InfutorUsername = string.Empty;
            string InfutorPassword = string.Empty;

            string ExperianEndpoint = string.Empty;
            string ExperianUsername = string.Empty;
            string ExperianPassword = string.Empty;
            string ExperianDBHost = string.Empty;

            Int32 intRecordLocator = 0;

            //need to finish the ad hoc
            if (args.Length > 0) //if you pass in a recordlocator to run ad hoc 
            {
                if (!Int32.TryParse(args[0], out intRecordLocator))
                    throw new ArgumentException("Record Locator Parameter is Invalid");
            }
            else
            {
                int tblSecurityCheckResponseId = 0;//tblSecurityCheckResponseId.SecurityCheckResponseId
                int tblMainId = 0; //tblMain.MainId as found in tlbMain and tblResponse


                //***TO DO***
                //grab the record from command line Ad Hoc request
                //***TO DO***

                //or

                //grab records from the DB 
                //where ResponseSent==0(record hasn't been updated) 
                try
                {

                    //Get Values from Config File
                    InfutorEndpoint = ConfigurationManager.AppSettings["InfutorEndpoint"].ToString();
                    InfutorUsername = ConfigurationManager.AppSettings["InfutorUsername"].ToString();
                    InfutorPassword = ConfigurationManager.AppSettings["InfutorPassword"].ToString();

                    ExperianEndpoint = ConfigurationManager.AppSettings["ExperianEndpoint"].ToString();
                    ExperianUsername = ConfigurationManager.AppSettings["ExperianUsername"].ToString();
                    ExperianPassword = ConfigurationManager.AppSettings["ExperianPassword"].ToString();
                    ExperianDBHost = ConfigurationManager.AppSettings["ExperianDBHost"].ToString();

                    foreach (tblSecurityCheckResponse responseToSend in GetSecurityCheckResponseRecords())
                    {

                        //FK to tblSecurityCheckResponse
                        tblSecurityCheckResponseId = responseToSend.SecurityCheckResponseId;

                        string InfutorResult = string.Empty;
                        string ExperianResult = string.Empty;

                        string InfutorResponseColor = string.Empty;
                        string ExperianResponseColor = string.Empty;

                        #region tblMain values
                        //Values from tblMain
                        string ServiceFirstName = string.Empty;
                        string ServiceLastName = string.Empty;
                        string ServiceAddress1 = string.Empty;
                        string ServiceCity = string.Empty;
                        string ServiceState = string.Empty;
                        string ServiceZipCode = string.Empty;
                        string ServicePhoneNumber = string.Empty;


                        tblMainId = responseToSend.MainId;

                        tblMain mainRecord = GetMainrecord(tblMainId);//get the record from tblMain based on the mainId in tblSecurityCheckResponse

                        ServiceFirstName = mainRecord.ServiceFirstName;
                        ServiceLastName = mainRecord.ServiceLastName;
                        ServiceAddress1 = mainRecord.ServiceAddress1;
                        ServiceCity = mainRecord.ServiceCity;
                        ServiceState = mainRecord.ServiceState;
                        ServiceZipCode = mainRecord.ServiceZipCode.Substring(0, 5);//get first 5 chars in zip
                        ServicePhoneNumber = mainRecord.ServicePhoneNumber;

                        mainRecord = null; //cleanup
                        #endregion tblMain values

                        #region Test Data


                        //FName = "Eric";
                        //LName = "Robbins";
                        //Address1 = "6333 N 19th Dr";
                        //City = "Phoenix";
                        //State = "AZ";
                        //Zip = "85015";
                        //Phone = "6027952080";

                        //Test Environment Account                        
                        //ServiceFirstName = "JOHN";
                        //ServiceLastName = "BREEN";
                        //ServiceAddress1 = "PO BOX 445";
                        //ServiceCity = "APO";
                        //ServiceState = "AE";
                        //ServiceZipCode = "09061";
                        //ServicePhoneNumber = "7818945369";

                        //Production Environment Account
                        //ServiceFirstName = "EVCARRIE";
                        //ServiceLastName = "CONSUMER";
                        //ServiceAddress1 = "4437 Spruce Street";
                        //ServiceCity = "Philadelphia";
                        //ServiceState = "PA";
                        //ServiceZipCode = "19104";


                        #endregion Test Data

                        //build Infutor Response
                        InfutorResult = BuildInfutorResponse(ServiceFirstName, ServiceLastName, ServiceAddress1, ServiceCity, ServiceState, ServiceZipCode, ServicePhoneNumber, InfutorEndpoint, InfutorUsername, InfutorPassword);

                        //Insert the response into the tblInfutorResponse table
                        InfutorResponseColor = InsertInfutorResponse(tblSecurityCheckResponseId, InfutorResult);

                        //Update tblSecurityCheckResponse with the InfutorResult
                        UpdateSecurityResponseCheckRecord(tblSecurityCheckResponseId, "1", "Success - Infutor", InfutorResponseColor, WhichSecurityCheck.Infutor);


                        //if the Infutor Result isn't good enough (ResponseCode=3=Red) we need to do an Experian Call
                        if (InfutorResponseColor == "Red")
                        {
                            //build Experian Response
                            ExperianResult = BuildExperianResponse(tblSecurityCheckResponseId, ServiceFirstName, ServiceLastName, ServiceAddress1, ServiceCity, ServiceState, ServiceZipCode, ServicePhoneNumber, ExperianEndpoint, ExperianUsername, ExperianPassword, ExperianDBHost);

                            //Insert the response into the tblInfutorResponse table
                            ExperianResponseColor = InsertExperianResponse(tblSecurityCheckResponseId, ExperianResult);

                            //Update tblSecurityCheckResponse with the ExperianResult
                            UpdateSecurityResponseCheckRecord(tblSecurityCheckResponseId, "1", "Success - Experian", ExperianResponseColor, WhichSecurityCheck.Experian);
                        }

                    }

                }
                catch (Exception ex)
                {
                    ErrorHandler(ex, tblSecurityCheckResponseId.ToString(), tblMainId.ToString());
                    UpdateSecurityResponseCheckRecord(tblSecurityCheckResponseId, "9", "Failed record for MainId: " + tblMainId + " check error log");
                    InsertAlertDTDUponError(tblMainId); // Insert Alert to AlertDTD if there is a fatal error

                }
            }
        }

        #endregion Main

        #region Infutor

        /// <summary>
        /// Builds the Infutor Web Request with passed in values
        /// </summary>
        /// <param name="FName"></param>
        /// <param name="LName"></param>
        /// <param name="Address1"></param>
        /// <param name="City"></param>
        /// <param name="State"></param>
        /// <param name="Zip"></param>
        /// <param name="Phone"></param>
        /// <param name="endpoint"></param>
        /// <param name="user"></param>
        /// <param name="pass"></param>
        /// <returns>Infutor Web Response as a string containing XML</returns>
        private static string BuildInfutorResponse(string FName, string LName, string Address1, string City, string State, string Zip, string Phone, string endpoint, string user, string pass)
        {

            string response = string.Empty;

            try
            {

                string InfutorRequest = string.Empty;
                InfutorRequest = string.Format("{0}login={1}&password={2}&FName={3}&LName={4}&Address1={5}&City={6}&State={7}&zip={8}&phone={9}&DOB=&SSN4=&version=4", endpoint, user, pass, FName, LName, Address1, City, State, Zip, Phone);

                WebRequest webRequest = WebRequest.Create(InfutorRequest);
                WebResponse webResp = webRequest.GetResponse();


                WebHeaderCollection header = webResp.Headers;

                var encoding = ASCIIEncoding.ASCII;

                string responseText = string.Empty;
                using (var reader = new System.IO.StreamReader(webResp.GetResponseStream(), encoding))
                {
                    response = reader.ReadToEnd();
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return response;

        }

        /// <summary>
        /// Parses out the Infutor String to XML and inserts into tblInfutorResponse
        /// </summary>
        /// <param name="securitycheckresponseId"></param>
        /// <param name="infutorResponseText"></param>
        /// <returns>Returns an Infutor Color Result based on the values in the parsed XML </returns>
        private static string InsertInfutorResponse(int securitycheckresponseId, string infutorResponseText)
        {

            string color = string.Empty;

            string ResponseCode = string.Empty;
            string ResponseMsg = string.Empty;
            string PersonLevel = string.Empty;
            string PersonCategoryDescription = string.Empty;
            string PersonScore = string.Empty;
            string PersonCategory = string.Empty;
            string PersonSSN4Reason = string.Empty;
            string PersonDOBReason = string.Empty;
            string PersonSSN4Match = string.Empty;
            string PersonDOBMatch = string.Empty;
            string PersonRecType = string.Empty;
            string PersonSource = string.Empty;
            string PersonConfidenceCode = string.Empty;
            string PersonCassErrorCode = string.Empty;
            string PersonFirstSummary = string.Empty;
            string PersonLastSummary = string.Empty;
            string PersonAddressSummary = string.Empty;
            string PersonZipSummary = string.Empty;
            string PersonPhoneSummary = string.Empty;
            string PersonSSN4Summary = string.Empty;
            string PersonDOBSummary = string.Empty;
            string PersonTotalSummary = string.Empty;
            string PersonLive = string.Empty;

            try
            {

                //Convert the string to XML
                XmlDocument xmlResponse = new XmlDocument();
                xmlResponse.LoadXml(infutorResponseText);

                //Example of the XML Response from text conversion
                //<?xml version="1.0"?>
                //-<xml xmlns:i="http://www.w3.org/2001/XMLSchema-instance" xmlns="https://xml.yourdatadelivery.com">
                //    <ResponseCode>0</ResponseCode>
                //    <ResponseMsg>Successful</ResponseMsg>
                //    -<Response>
                //    -<Detail xmlns="" transaction="LeadValidationCASS">
                //        -<Person>
                //        <Level>4</Level>
                //        <CategoryDescription>HouseHold with phone</CategoryDescription>
                //        <Score>202</Score>
                //        <Category>HP</Category>
                //        <SSN4Reason>Not Provided</SSN4Reason>
                //        <DOBReason>Not Provided</DOBReason>
                //        <SSN4Match>N</SSN4Match>
                //        <DOBMatch>N</DOBMatch>
                //        <RecType>R</RecType>
                //        <Source>C</Source>
                //        <ConfidenceCode>1</ConfidenceCode>
                //        <CassErrorCode>1</CassErrorCode>
                //        <FirstSummary>0</FirstSummary>
                //        <LastSummary>1</LastSummary>
                //        <AddressSummary>1</AddressSummary>
                //        <ZipSummary>1</ZipSummary>
                //        <PhoneSummary>1</PhoneSummary>
                //        <SSN4Summary>1</SSN4Summary>
                //        <DOBSummary>0</DOBSummary>
                //        <DOBMSummary>0</DOBMSummary>
                //        <TotalSummary>5</TotalSummary>
                //        <Live>False</Live>
                //        </Person>
                //    </Detail>
                //    </Response>
                //</xml>

                //Prepare to parse out the XML node values
                XmlNodeList elemlist = null;

                elemlist = xmlResponse.GetElementsByTagName("ResponseCode");
                ResponseCode = elemlist[0].InnerXml;

                elemlist = xmlResponse.GetElementsByTagName("ResponseMsg");
                ResponseMsg = elemlist[0].InnerXml;

                elemlist = xmlResponse.GetElementsByTagName("Level");
                PersonLevel = elemlist[0].InnerXml;

                elemlist = xmlResponse.GetElementsByTagName("CategoryDescription");
                PersonCategoryDescription = elemlist[0].InnerXml;

                elemlist = xmlResponse.GetElementsByTagName("Score");
                PersonScore = elemlist[0].InnerXml;

                elemlist = xmlResponse.GetElementsByTagName("Category");
                PersonCategory = elemlist[0].InnerXml;

                elemlist = xmlResponse.GetElementsByTagName("SSN4Reason");
                PersonSSN4Reason = elemlist[0].InnerXml;

                elemlist = xmlResponse.GetElementsByTagName("DOBReason");
                PersonDOBReason = elemlist[0].InnerXml;

                elemlist = xmlResponse.GetElementsByTagName("SSN4Match");
                PersonSSN4Match = elemlist[0].InnerXml;

                elemlist = xmlResponse.GetElementsByTagName("DOBMatch");
                PersonDOBMatch = elemlist[0].InnerXml;

                elemlist = xmlResponse.GetElementsByTagName("RecType");
                PersonRecType = elemlist[0].InnerXml;

                elemlist = xmlResponse.GetElementsByTagName("Source");
                PersonSource = elemlist[0].InnerXml;

                elemlist = xmlResponse.GetElementsByTagName("ConfidenceCode");
                PersonConfidenceCode = elemlist[0].InnerXml;

                elemlist = xmlResponse.GetElementsByTagName("CassErrorCode");
                PersonCassErrorCode = elemlist[0].InnerXml;

                elemlist = xmlResponse.GetElementsByTagName("FirstSummary");
                PersonFirstSummary = elemlist[0].InnerXml;

                elemlist = xmlResponse.GetElementsByTagName("LastSummary");
                PersonLastSummary = elemlist[0].InnerXml;

                elemlist = xmlResponse.GetElementsByTagName("AddressSummary");
                PersonAddressSummary = elemlist[0].InnerXml;

                elemlist = xmlResponse.GetElementsByTagName("ZipSummary");
                PersonZipSummary = elemlist[0].InnerXml;

                elemlist = xmlResponse.GetElementsByTagName("PhoneSummary");
                PersonPhoneSummary = elemlist[0].InnerXml;

                elemlist = xmlResponse.GetElementsByTagName("SSN4Summary");
                PersonSSN4Summary = elemlist[0].InnerXml;

                elemlist = xmlResponse.GetElementsByTagName("DOBSummary");
                PersonDOBSummary = elemlist[0].InnerXml;

                elemlist = xmlResponse.GetElementsByTagName("TotalSummary");
                PersonTotalSummary = elemlist[0].InnerXml;

                elemlist = xmlResponse.GetElementsByTagName("Live");
                PersonLive = elemlist[0].InnerXml;


                //Insert into tblInfutorResponse
                tblInfutorResponse InfutorRecord = null;
                using (ConstellationEntities entities = new ConstellationEntities())
                {
                    InfutorRecord = new tblInfutorResponse();

                    entities.Connection.Open();

                    InfutorRecord.SecurityCheckResponseId = securitycheckresponseId; //FK to tblSecurityCheckResponse

                    InfutorRecord.ResponseCode = int.Parse(ResponseCode);
                    InfutorRecord.ResponseMsg = ResponseMsg;
                    InfutorRecord.PersonLevel = int.Parse(PersonLevel);
                    InfutorRecord.PersonCategoryDescription = PersonCategoryDescription;
                    InfutorRecord.PersonScore = int.Parse(PersonScore);
                    InfutorRecord.PersonCategory = PersonCategory;
                    InfutorRecord.PersonSSN4Reason = PersonSSN4Reason;
                    InfutorRecord.PersonDOBReason = PersonDOBReason;
                    InfutorRecord.PersonSSN4Match = PersonSSN4Match;
                    InfutorRecord.PersonDOBMatch = PersonDOBMatch;
                    InfutorRecord.PersonRecType = PersonRecType;
                    InfutorRecord.PersonSource = PersonSource;
                    InfutorRecord.PersonConfidenceCode = int.Parse(PersonConfidenceCode);
                    InfutorRecord.PersonCassErrorCode = int.Parse(PersonCassErrorCode);
                    InfutorRecord.PersonFirstSummary = int.Parse(PersonFirstSummary);
                    InfutorRecord.PersonLastSummary = int.Parse(PersonLastSummary);
                    InfutorRecord.PersonAddressSummary = int.Parse(PersonAddressSummary);
                    InfutorRecord.PersonZipSummary = int.Parse(PersonZipSummary);
                    InfutorRecord.PersonPhoneSummary = int.Parse(PersonPhoneSummary);
                    InfutorRecord.PersonSSN4Summary = int.Parse(PersonSSN4Summary);
                    InfutorRecord.PersonDOBSummary = int.Parse(PersonDOBSummary);
                    InfutorRecord.PersonTotalSummary = int.Parse(PersonTotalSummary);
                    InfutorRecord.PersonLive = bool.Parse(PersonLive);

                    entities.AddTotblInfutorResponses(InfutorRecord);
                    entities.SaveChanges();
                    entities.Connection.Close();
                }

                switch (PersonLevel)
                {
                    case "3":
                        color = "Red";
                        break;
                    case "4":
                        color = "Green";
                        break;
                    case "5":
                        color = "Blue";
                        break;
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return color;
        }

        #endregion Infutor

        #region Experian
        /// <summary>
        /// Builds the Experian Web Request with passed in values
        /// </summary>
        /// <param name="tblSecurityCheckResponseId"></param>
        /// <param name="FName"></param>
        /// <param name="LName"></param>
        /// <param name="Address1"></param>
        /// <param name="City"></param>
        /// <param name="State"></param>
        /// <param name="Zip"></param>
        /// <param name="Phone"></param>
        /// <param name="endpoint"></param>
        /// <param name="user"></param>
        /// <param name="pass"></param>
        /// <returns>Experian Web Response as a string containing XML</returns>
        private static string BuildExperianResponse(int tblSecurityCheckResponseId, string FName, string LName, string Address1, string City, string State, string Zip, string Phone, string endpoint, string user, string pass, string dbhost)
        {

            string response = string.Empty;

            //Build XML To Send
            StringBuilder sb = new StringBuilder();

            sb.AppendFormat("<?xml version=\"{0}\" encoding=\"{1}\"?>", "1.0", "UTF-8");
            sb.AppendFormat("<NetConnectRequest xmlns=\"{0}\" xmlns:xsi=\"{1}\" xsi:schemaLocation=\"{2}\">", @"http://www.experian.com/NetConnect", @"http://www.w3.org/2001/XMLSchema-instance", @"http://www.experian.com/NetConnect NetConnect.xsd");
            sb.AppendFormat("   <EAI>{0}</EAI>", "11111111");
            sb.AppendFormat("   <DBHost>{0}</DBHost>", dbhost);
            sb.AppendFormat("   <ReferenceId>{0}</ReferenceId>", tblSecurityCheckResponseId.ToString());
            sb.AppendFormat("   <Request xmlns=\"{0}\" version=\"{1}\">", @"http://www.experian.com/WebDelivery/", "1.0");
            sb.AppendFormat("   <Products>");
            sb.AppendFormat("   <PreciseIDServer>");
            sb.AppendFormat("	    <XMLVersion>5.0</XMLVersion>");
            sb.AppendFormat("	    <Subscriber>");
            sb.AppendFormat("		    <Preamble>{0}</Preamble>", "TBD2");
            sb.AppendFormat("		    <OpInitials>{0}</OpInitials>", "CS");
            sb.AppendFormat("		    <SubCode>{0}</SubCode>", "1907637");
            sb.AppendFormat("	    </Subscriber>");
            sb.AppendFormat("	    <PrimaryApplicant>");
            sb.AppendFormat("		    <Name>");
            sb.AppendFormat("			    <Surname>{0}</Surname>", LName);
            sb.AppendFormat("			    <First>{0}</First>", FName);
            sb.AppendFormat("		    </Name>");
            sb.AppendFormat("		    <SSN/>");
            sb.AppendFormat("		    <CurrentAddress>");
            sb.AppendFormat("			    <Street>{0}</Street>", Address1);
            sb.AppendFormat("			    <City>{0}</City>", City);
            sb.AppendFormat("			    <State>{0}</State>", State);
            sb.AppendFormat("			    <Zip>{0}</Zip>", Zip);
            sb.AppendFormat("		    </CurrentAddress>");
            sb.AppendFormat("		    <Phone>");
            sb.AppendFormat("			    <Number>{0}</Number>", Phone);
            sb.AppendFormat("			    <Type>R</Type>");
            sb.AppendFormat("		    </Phone>");
            sb.AppendFormat("	    </PrimaryApplicant>");
            sb.AppendFormat("       <Options>");
            sb.AppendFormat("           <ReferenceNumber>{0}</ReferenceNumber>", tblSecurityCheckResponseId.ToString());
            sb.AppendFormat("           <PreciseIDType>{0}</PreciseIDType>", "20");
            sb.AppendFormat("           <DetailRequest>{0}</DetailRequest>", "S");
            sb.AppendFormat("           <InquiryChannel>{0}</InquiryChannel>", "INTE");
            sb.AppendFormat("       </Options>");
            sb.AppendFormat("   </PreciseIDServer>");
            sb.AppendFormat("   </Products>");
            sb.AppendFormat("   </Request>");
            sb.AppendFormat("</NetConnectRequest>");


            try
            {
                //1. Build a string of UrlEncoded XML
                string postData = null;
                postData = "NETCONNECT_TRANSACTION=" + System.Web.HttpUtility.UrlEncode(sb.ToString());

                //2. Instantiate an HTTPWebRequest Object (see function below)
                HttpWebRequest experianRequest = (HttpWebRequest)WebRequest.Create(endpoint);

                //3. Set .NET to “POST” data
                experianRequest.Method = "POST";

                //4. Set Content Type per Experian
                experianRequest.ContentType = "application/x-www-form-urlencoded";

                //5. Format User ID and password in Experian-defined format (includes “:”)
                string UserIDFormated = string.Format("{0}:{1}", user, pass);

                //6. Set up communication protocols to match JAVA implementation
                //experianRequest.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes(UserIDFormated)));
                experianRequest.Headers.Add("Authorization", "Basic " + ConverToBase64String(UserIDFormated));
                experianRequest.Timeout = 100000;
                experianRequest.KeepAlive = false;
                experianRequest.Credentials = System.Net.CredentialCache.DefaultCredentials;
                //7. ASCII-encode the XML string we built (postData) above
                System.Text.ASCIIEncoding encoding = new ASCIIEncoding();
                byte[] byteData;
                byteData = encoding.GetBytes(postData);

                //8. Allow redirects
                experianRequest.AllowAutoRedirect = true;

                //9. Build the request string
                experianRequest.ContentLength = byteData.Length;
                Stream newStream = experianRequest.GetRequestStream();

                //10. Send the request stream to Experian
                newStream.Write(byteData, 0, byteData.Length);
                newStream.Close();

                //11. Capture the response
                HttpWebResponse experianResponse = (HttpWebResponse)experianRequest.GetResponse();



                using (var reader = new System.IO.StreamReader(experianResponse.GetResponseStream(), encoding))
                {
                    response = reader.ReadToEnd();
                }


            }
            catch (Exception ex)
            {
                throw ex;
            }

            return response;
        }

        /// <summary>
        ///  Parses out the Experian String to XML and inserts into tblInfutorResponse
        /// </summary>
        /// <param name="securitycheckresponseId"></param>
        /// <param name="experianResponseText"></param>
        /// <returns>Returns an Experian Color Result based on the values in the parsed XML </returns>
        private static string InsertExperianResponse(int securitycheckresponseId, string experianResponseText)
        {
            string color = string.Empty;

            string ReportDate = string.Empty;
            string ReportTime = string.Empty;
            string Preamble = string.Empty;
            string ReferenceNumber = string.Empty;
            string ReviewReferenceID = string.Empty;
            string PreciseIDType = string.Empty;
            string ComplianceIndicator = string.Empty;
            string ComplianceDescription = string.Empty;
            string InitialDecision = string.Empty;
            string FinalDecision = string.Empty;


            try
            {
                //Convert the string to XML
                XmlDocument xmlResponse = new XmlDocument();
                xmlResponse.LoadXml(experianResponseText);



                //Example of the XML Response from text conversion
                //<NetConnectResponse xmlns="http://www.experian.com/NetConnectResponse">
                //    <CompletionCode>0000</CompletionCode>
                //    <ReferenceId>1</ReferenceId>
                //    <Products>
                //        <PreciseIDServer>
                //            <XMLVersion>5.0</XMLVersion>
                //            <SessionID>1FE1036AB86F44F6B36021600F9EAAF5.pidd2v-1512041546140210529333057</SessionID>
                //            <Header>
                //                <ReportDate>12042015</ReportDate>
                //                <ReportTime>154615</ReportTime>
                //                <Preamble>TBD2</Preamble>
                //                <ReferenceNumber>1</ReferenceNumber>
                //            </Header>
                //            <Messages/>
                //            <Summary>
                //                <ReviewReferenceID>10644586</ReviewReferenceID>
                //                <PreciseIDType>20</PreciseIDType>
                //                <ComplianceIndicator>3 </ComplianceIndicator>
                //                <ComplianceDescription>See Final Decision</ComplianceDescription>
                //                <InitialResults>
                //                    <InitialDecision>3  </InitialDecision>
                //                    <FinalDecision>3  </FinalDecision>
                //                </InitialResults>
                //                <CrossReferenceIndicatorsGrid>
                //                    <FullNameVerifiesToAddress code="NM"/>
                //                    <FullNameVerifiesToSSN code="NM"/>
                //                    <FullNameVerifiesToDL code="NM"/>
                //                    <FullNameVerifiesToPhone code="NM"/>
                //                    <SurnameOnlyVerifiesToAddress code="NM"/>
                //                    <SurnameOnlyVerifiesToSSN code="NM"/>
                //                    <SurnameOnlyVerifiesToDL code="NM"/>
                //                    <SurnameOnlyVerifiesToPhone code="NM"/>
                //                    <AddressVerifiesToFullName code="NM"/>
                //                    <AddressVerifiesToSurnameOnly code="NM"/>
                //                    <AddressVerifiesToSSN code="NM"/>
                //                    <AddressVerifiesToDL code="NM"/>
                //                    <AddressVerifiesToPhone code="NM"/>
                //                    <SSNVerifiesToFullName code="NM"/>
                //                    <SSNVerifiesToSurnameOnly code="NM"/>
                //                    <SSNVerifiesToAddress code="NM"/>
                //                    <DLVerifiesToFullName code="NM"/>
                //                    <DLVerifiesToSurnameOnly code="NM"/>
                //                    <DLVerifiesToAddress code="NM"/>
                //                    <PhoneVerifiesToFullName code="NM"/>
                //                    <PhoneVerifiesToSurnameOnly code="NM"/>
                //                    <PhoneVerifiesToAddress code="NM"/>
                //                </CrossReferenceIndicatorsGrid>
                //                <DateOfBirthMatch code="6"/>
                //            </Summary>
                //            <CCNumerics/>
                //            <CCStrings>
                //                <CCString5>V;;;</CCString5>
                //            </CCStrings>
                //        </PreciseIDServer>
                //    </Products>
                //</NetConnectResponse>

                //Prepare to parse out the XML node values
                XmlNodeList elemlist = null;

                elemlist = xmlResponse.GetElementsByTagName("ReportDate");
                ReportDate = elemlist[0].InnerXml.Trim();

                elemlist = xmlResponse.GetElementsByTagName("ReportTime");
                ReportTime = elemlist[0].InnerXml.Trim();

                elemlist = xmlResponse.GetElementsByTagName("Preamble");
                Preamble = elemlist[0].InnerXml.Trim();

                elemlist = xmlResponse.GetElementsByTagName("ReferenceNumber");
                ReferenceNumber = elemlist[0].InnerXml.Trim();

                elemlist = xmlResponse.GetElementsByTagName("ReviewReferenceID");
                ReviewReferenceID = elemlist[0].InnerXml.Trim();

                elemlist = xmlResponse.GetElementsByTagName("PreciseIDType");
                PreciseIDType = elemlist[0].InnerXml.Trim();

                elemlist = xmlResponse.GetElementsByTagName("ComplianceIndicator");
                ComplianceIndicator = elemlist[0].InnerXml.Trim();

                elemlist = xmlResponse.GetElementsByTagName("ComplianceDescription");
                ComplianceDescription = elemlist[0].InnerXml.Trim();

                elemlist = xmlResponse.GetElementsByTagName("InitialDecision");
                InitialDecision = elemlist[0].InnerXml.Trim();

                elemlist = xmlResponse.GetElementsByTagName("FinalDecision");
                FinalDecision = elemlist[0].InnerXml.Trim();

                //Insert into tblExperian
                tblExperianResponse ExperianRecord = null;
                using (ConstellationEntities entities = new ConstellationEntities())
                {
                    ExperianRecord = new tblExperianResponse();

                    entities.Connection.Open();

                    ExperianRecord.SecurityCheckResponseId = securitycheckresponseId; //FK to tblSecurityCheckResponse                   

                    ExperianRecord.HeaderReportDate = ReportDate;
                    ExperianRecord.HeaderReportTime = ReportTime;
                    ExperianRecord.HeaderPreamble = Preamble;
                    ExperianRecord.HeaderReferenceNumber = int.Parse(ReferenceNumber);
                    ExperianRecord.ReviewReferenceId = int.Parse(ReviewReferenceID);
                    ExperianRecord.PreciseIDType = int.Parse(PreciseIDType);
                    ExperianRecord.ComplianceIndicator = int.Parse(ComplianceIndicator);
                    ExperianRecord.ComplianceDescription = ComplianceDescription;
                    ExperianRecord.InitialResultsInitialDecision = int.Parse(InitialDecision);
                    ExperianRecord.InitialResultsFinalDecision = int.Parse(FinalDecision);

                    entities.AddTotblExperianResponses(ExperianRecord);
                    entities.SaveChanges();
                    entities.Connection.Close();
                }

                string ExperianLevel = string.Empty;
                //Level 3
                //ComplianceIndicator =3
                //InitialDecision = 3
                //FinalDecision = 3

                //Level 4
                //ComplianceIndicator =3
                //InitialDecision = 4
                //FinalDecision = 4

                //Level 5
                //ComplianceIndicator = 5
                //InitialDecision = 3
                //FinalDecision = 3

                if (ComplianceIndicator == "3" && InitialDecision == "3" && FinalDecision == "3")
                {
                    ExperianLevel = "3";
                }
                else if (ComplianceIndicator == "3" && InitialDecision == "4" && FinalDecision == "4")
                {
                    ExperianLevel = "4";
                }
                else if (ComplianceIndicator == "5" && InitialDecision == "3" && FinalDecision == "3")
                {
                    ExperianLevel = "5";
                }


                switch (ExperianLevel)
                {
                    case "3":
                        color = "Red";
                        break;
                    case "4":
                        color = "Green";
                        break;
                    case "5":
                        color = "Blue";
                        break;
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return color;
        }
        #endregion Experian

        #region Entity Framework Methods

        /// <summary>
        /// Gets a list of all pending records that have not been updated
        /// </summary>
        /// <returns>List of tblSecurityCheckResponses</returns>
        private static List<tblSecurityCheckResponse> GetSecurityCheckResponseRecords()
        {
            List<tblSecurityCheckResponse> ctxRecords = new List<tblSecurityCheckResponse>();
            try
            {
                using (ConstellationEntities entities = new ConstellationEntities())
                {
                    ctxRecords = entities.tblSecurityCheckResponses
                        .Where(x => x.ResponseSent == "0").ToList();
                }
            }
            catch (Exception ex)
            {
                ErrorHandler(ex);
            }
            return ctxRecords;
        }

        /// <summary>
        /// Used to get the record we need to update from tblMain
        /// </summary>
        /// <param name="mainid"></param>
        /// <returns>A tblMain record</returns>
        private static tblMain GetMainrecord(int mainid)
        {
            tblMain ctxRecords = new tblMain();
            try
            {
                using (ConstellationEntities entities = new ConstellationEntities())
                {
                    ctxRecords = entities.tblMains
                     .Where(x => x.MainId == mainid).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                ErrorHandler(ex);
            }

            return ctxRecords;
        }

        /// <summary>
        /// Overloaded method to update tblSecurityCheckResponse with Color value for the passed in Security Check and a status of 1 with a success message
        /// </summary>
        /// <param name="tblSecurityCheckResponseId"></param>
        /// <param name="responseSent"></param>
        /// <param name="color"></param>
        /// <param name="securityCheck"></param>
        private static void UpdateSecurityResponseCheckRecord(int tblSecurityCheckResponseId, string responseSent, string status, string color, WhichSecurityCheck securityCheck)
        {
            try
            {

                tblSecurityCheckResponse securityCheckResponse = null;
                using (ConstellationEntities entities = new ConstellationEntities())
                {

                    securityCheckResponse = (from record in entities.tblSecurityCheckResponses
                                             where record.SecurityCheckResponseId == tblSecurityCheckResponseId
                                             select record).FirstOrDefault();

                    securityCheckResponse.ResponseSent = responseSent;
                    securityCheckResponse.Status = status;
                    switch (securityCheck)
                    {
                        case WhichSecurityCheck.Infutor:
                            securityCheckResponse.InfutorResult = color;
                            break;
                        case WhichSecurityCheck.Experian:
                            securityCheckResponse.ExperianResult = color;
                            break;
                    }

                    securityCheckResponse.ResponseDateTime = DateTime.Now;
                    entities.SaveChanges();

                }
            }
            catch (Exception ex)
            {
                ErrorHandler(ex, tblSecurityCheckResponseId.ToString());

            }

        }

        /// <summary>
        /// Overloaded method to update tblSecurityCheckResponse when the process fails via an exception thrown, this will update the record as a 9 and set the status to failed
        /// </summary>
        /// <param name="tblSecurityCheckResponseId"></param>
        /// <param name="responseSent"></param>
        /// <param name="status"></param>
        private static void UpdateSecurityResponseCheckRecord(int tblSecurityCheckResponseId, string responseSent, string status)
        {
            try
            {

                tblSecurityCheckResponse securityCheckResponse = null;
                using (ConstellationEntities entities = new ConstellationEntities())
                {

                    securityCheckResponse = (from record in entities.tblSecurityCheckResponses
                                             where record.SecurityCheckResponseId == tblSecurityCheckResponseId
                                             select record).FirstOrDefault();

                    securityCheckResponse.ResponseSent = responseSent;
                    securityCheckResponse.Status = status;
                    securityCheckResponse.ResponseDateTime = DateTime.Now;
                    entities.SaveChanges();

                }
            }
            catch (Exception ex)
            {
                ErrorHandler(ex, tblSecurityCheckResponseId.ToString());

            }

        }

        #region AlertDTD
        /// <summary>
        /// Inserts an Alert to the AlertDTD table if there is a critical error
        /// </summary>
        /// <param name="mainId"></param>
        private static void InsertAlertDTDUponError(int? mainId)
        {
            int alertTypeId = 0;
            try
            {
                AlertDTD alertDTDrecord = null;
                AlertTypeDTD alertTypeDTD = null;
                using (ConstellationEntities entities = new ConstellationEntities())
                {
                    alertTypeDTD = new AlertTypeDTD();

                    //Get the AlertTypeDTD.Id for Pos ID Errors
                    alertTypeId = (from at in entities.AlertTypeDTDs
                                   where at.Type == "POS ID Errors"
                                   select at.Id).FirstOrDefault();

                    alertDTDrecord = new AlertDTD();

                    alertDTDrecord.AlertTypeId = alertTypeId;
                    alertDTDrecord.MainId = mainId;
                    entities.AddToAlertDTDs(alertDTDrecord);
                    entities.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                ErrorHandler(ex);
            }
        }
        #endregion AlertDTD

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
            Calibrus.ErrorHandler.Alerting alert = new Calibrus.ErrorHandler.Alerting("ConstellationSecurityCheck");
            alert.SendAlert(ex.Source, ex.Message, Environment.MachineName, Environment.UserName, Environment.Version.ToString());
        }

        static void ErrorHandler(Exception ex, string tblSecurityCheckRespoinseId)
        {
            Calibrus.ErrorHandler.Alerting alert = new Calibrus.ErrorHandler.Alerting("ConstellationSecurityCheck");
            alert.SendAlert(ex.Source, String.Format("tblSecurityCheckRespoinseId: {0} -- {1}", tblSecurityCheckRespoinseId, ex.Message), Environment.MachineName, Environment.UserName, Environment.Version.ToString());
        }

        static void ErrorHandler(Exception ex, string tblSecurityCheckRespoinseId, string tblMainId)
        {
            Calibrus.ErrorHandler.Alerting alert = new Calibrus.ErrorHandler.Alerting("ConstellationSecurityCheck");
            alert.SendAlert(ex.Source, String.Format("tblSecurityCheckRespoinseId: {0} tblMainId: {1} -- {2}", tblSecurityCheckRespoinseId, tblMainId, ex.Message), Environment.MachineName, Environment.UserName, Environment.Version.ToString());
        }

        static void LogError(Exception ex)
        {
            Calibrus.ErrorHandler.Logging log = new Calibrus.ErrorHandler.Logging();
            log.LogError("ConstellationSecurityCheck", Environment.Version.ToString(), Environment.MachineName, Environment.UserName, ex.Source, ex.Message);
        }

        static void LogError(Exception ex, string tblSecurityCheckRespoinseId)
        {
            Calibrus.ErrorHandler.Logging log = new Calibrus.ErrorHandler.Logging();
            log.LogError("ConstellationSecurityCheck", Environment.Version.ToString(), Environment.MachineName, Environment.UserName, ex.Source,
                String.Format("tblSecurityCheckRespoinseId Record Locator: {0} -- {1}", tblSecurityCheckRespoinseId, ex.Message));
        }
        static void LogError(Exception ex, string tblSecurityCheckRespoinseId, string tblMainId)
        {
            Calibrus.ErrorHandler.Logging log = new Calibrus.ErrorHandler.Logging();
            log.LogError("ConstellationSecurityCheck", Environment.Version.ToString(), Environment.MachineName, Environment.UserName, ex.Source,
                String.Format("tblSecurityCheckRespoinseId: {0} tblMainId: {1} -- {2}", tblSecurityCheckRespoinseId, tblMainId, ex.Message));
        }
        #endregion
    }
}
