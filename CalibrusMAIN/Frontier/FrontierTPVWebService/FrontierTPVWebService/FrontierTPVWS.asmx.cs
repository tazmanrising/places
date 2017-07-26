using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Xml;
using System.Transactions;
using System.Text;
using System.Text.RegularExpressions;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace FrontierTPVWebService
{

    [WebService(Namespace = "http://ws.calibrus.com/FrontierTPVWebService")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class FrontierTPVWS : System.Web.Services.WebService
    {
        public enum FaultCode
        {
            Client = 0,
            Server = 1
        }

        public FrontierTPVWS()
        {
            //Uncomment the following line if using designed components 
            //InitializeComponent(); 
        }

        [WebMethod(Description = "Submits a Frontier Order", MessageName = "SubmitOrder")]
        public int SubmitOrder(Account account)
        {
            string validationException = string.Empty;
            int accountId = -1;

            if (!account.ValidateOrder(out validationException))
            {
                throw GenerateSoapException("ValidateOrder", "http://ws.calibrus.com/FrontierTPVWebService", validationException, "1000", "ValidateOrder", FaultCode.Client);
            }

            try
            {
                accountId = InsertAccount(ref account);
            }
            catch (Exception ex)
            {
                throw GenerateSoapException("InsertAccount", "http://ws.calibrus.com/FrontierTPVWebService", ex.Message, "1001", ex.Source, FaultCode.Server);
            }

            return accountId;
        }

        [WebMethod(Description = "Retrieve information about a specific record using the Telephone Number.", MessageName = "RetrieveDataTelephoneNumber")]
        public DataSet RetrieveDataTelephoneNumber(string PhoneNumber)
        {
            string validationException = "";
            DataSet result = null;

            if (!ValidationPhoneNumber(ref PhoneNumber, out validationException))
            {
                throw GenerateSoapException("ValidateOrder", "http://ws.calibrus.com/FrontierTPVWebService", validationException, "2000", "PhoneAccountValidation", FaultCode.Client);
            }

            try
            {
                result = RunReportPhoneNumber(PhoneNumber);
            }
            catch (Exception ex)
            {
                throw GenerateSoapException("RetieveDataRecordLocator", "http://ws.calibrus.com/FrontierTPVWebService", ex.Message, "2001", ex.Source, FaultCode.Server);
            }

            return (result);
        }

        [WebMethod(Description = "Retrieve information about a specific record using the Record Locator.", MessageName = "RetrieveDataRecordLocator")]
        public DataSet RetrieveDataRecordLocator(string RecordLocator)
        {
            string validationException = "";
            DataSet result = null;

            if (!ValidationRecordLocator(ref RecordLocator, out validationException))
            {
                throw GenerateSoapException("ValidateOrder", "http://ws.calibrus.com/FrontierTPVWebService", validationException, "2002", "RecordLocatorValidation", FaultCode.Client);
            }

            try
            {
                result = RunReportRecordLocator(RecordLocator);
            }
            catch (Exception ex)
            {
                throw GenerateSoapException("RetieveDataRecordLocator", "http://ws.calibrus.com/FrontierTPVWebService", ex.Message, "2003", ex.Source, FaultCode.Server);
            }

            return (result);
        }

        private int InsertAccount(ref Account account)
        {
            int? id = null;

            try
            {
                tblMain main = null;
                tblTn tn = null;
                using (FrontierEntities ftrData = new FrontierEntities())
                {
                    ftrData.Connection.Open();

                    main = new tblMain();

                    main.SalesAgentId = account.SalesAgentId;
                    main.State = account.State.ToUpper();
                    main.CustFirstName = account.CustFirstName;
                    main.CustLastName = account.CustLastName;
                    main.DecisionMaker = account.DecisionMaker;
                    main.CompanyName = account.CompanyName == null ? string.Empty : account.CompanyName;
                    main.Product = account.Product;
                    main.Business = account.Business == true ? "1" : "0";
                    main.Verified = "9";
                    main.Concern = "No TPV Call";
                    main.WebDateTime = DateTime.Now;


                    foreach (PhoneNumber phoneNumber in account.PhoneNumbers)
                    {
                        tn = new tblTn();

                        tn.Tn = phoneNumber.Tn;
                        tn.DialTone = phoneNumber.PLOCChange == true ? "1" : "0";
                        tn.DialToneFreeze = phoneNumber.PLOCFreeze == true ? "1" : "0";
                        tn.LocalToll = phoneNumber.ILPIntra == true ? "1" : "0";
                        tn.LocalTollFreeze = phoneNumber.ILPIntraFreeze == true ? "1" : "0";
                        tn.Ld = phoneNumber.PICInter == true ? "1" : "0";
                        tn.LdFreeze = phoneNumber.PICInterFreeze == true ? "1" : "0";

                        if (tn != null)
                        {
                            main.tblTns.Add(tn);
                        }
                    }
                    ftrData.AddTotblMains(main);
                    ftrData.SaveChanges();

                    id = main.MainId;

                    ftrData.Connection.Close();
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return id.Value;
        }

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

        private bool ValidationPhoneNumber(ref string phoneNumber, out string exception)
        {

            if (phoneNumber.Trim().Length == 0)
            {
                //throw (new ApplicationException("No Phone Number supplied."));
                exception = "No Phone Number supplied.";
                return false;
            }

            //strip out any non digits they might send over.
            Regex rPhoneStrip = new Regex(@"\D");
            rPhoneStrip.Replace(phoneNumber, "");

            //check the phone number to make sure it is 10 digits
            Regex rPhoneCheck = new Regex(@"^\d{10}$");
            if (rPhoneCheck.IsMatch(phoneNumber) == false)
            {
                //throw (new SoapException("test", new System.Xml.XmlQualifiedName("test qualified")));
                //throw (new ApplicationException("Invalid Phone Number supplied."));
                exception = "Invalid Phone Number supplied.";
                return false;
            }

            exception = "";
            return true;
        }

        private bool ValidationRecordLocator(ref string recordLocator, out string exception)
        {
            int result = 0;
            if (!int.TryParse(recordLocator, out result))
            {
                //throw (new ApplicationException("Invalid Record Locator supplied."));
                exception = "Invalid Record Locator supplied.";
                return false;
            }

            exception = "";
            return true;
        }

        private DataSet RunReportPhoneNumber(string phoneNumber)
        {
            SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["Frontier_cnString"].ToString());
            SqlDataAdapter da = new SqlDataAdapter(SqlStringPhone(phoneNumber), cn);
            DataSet ds = new DataSet();
            try
            {
                da.Fill(ds);
            }
            catch (SqlException ex)
            {
                throw (new ApplicationException(ex.Message));
            }
            finally
            {
                cn.Dispose();
                da.Dispose();
            }
            return ds;
        }

        private string SqlStringPhone(string phoneNumber)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT  top 1 m.DateTime, m.Concern, m.Verified ");
            sb.Append("FROM tblMain m ");
            sb.Append("JOIN tblTn tn ");
            sb.Append("ON   m.MainId = tn.MainId ");
            sb.AppendFormat("WHERE tn.tn = '{0}' ", phoneNumber);
            sb.Append("order by tn.MainId desc  ");

            return sb.ToString();
        }

        private DataSet RunReportRecordLocator(string recordLocator)
        {

            SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["Frontier_cnString"].ToString());
            SqlDataAdapter da = new SqlDataAdapter(SqlStringRecordLocator(recordLocator), cn);
            DataSet ds = new DataSet();
            try
            {
                da.Fill(ds);
            }
            catch (SqlException ex)
            {
                throw (new ApplicationException(ex.Message));
            }
            finally
            {
                cn.Dispose();
                da.Dispose();
            }
            return ds;
        }

        private string SqlStringRecordLocator(string recordLocator)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT Datetime, Concern, Verified ");
            sb.Append("FROM tblMain ");
            sb.AppendFormat("Where MainId = '{0}' ", recordLocator);

            return sb.ToString();
        }



    }
}