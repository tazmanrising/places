using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Calibrus.ErrorHandler;
using Renci.SshNet;
using System.Diagnostics;
using System.Security;

namespace ConstellationHomeServicesDailyCSV
{
    public class Report
    {
        #region Main
        public static void Main(string[] args)
        {
            string rootPath = string.Empty;//File we create this in this program and its location
            string hostName = string.Empty; //Server where we send the file
            string userName = string.Empty; //user account
            string password = string.Empty; //password
            string toDir = string.Empty; //Directory path to build for the file to put on the server


            //get report interval          
            DateTime StartDate = new DateTime();
            DateTime EndDate = new DateTime();

            //start to  build the form pathing
            string csvFilename = string.Empty;
            string csvFilePath = string.Empty;

            if (args.Length > 0)
            {
                if (DateTime.TryParse(args[0], out StartDate))
                {
                    GetDates(out StartDate, out EndDate);
                }
                else
                {
                    ArgumentException ex = new ArgumentException(String.Format("Invalid parameter", args[0]), "RunDate");
                    ex.Source = "Main(string[] args)";
                    SendErrorMessage(ex);
                    return;
                }
            }
            else
            {
                GetDates(out StartDate, out EndDate);
            }

            //grab values from app.config
            rootPath = ConfigurationManager.AppSettings["rootPath"].ToString();

            hostName = ConfigurationManager.AppSettings["hostName"].ToString();
            userName = ConfigurationManager.AppSettings["userName"].ToString();
            password = ConfigurationManager.AppSettings["password"].ToString();
            toDir = ConfigurationManager.AppSettings["toDir"].ToString();
            try
            {
                string strBuffer = "";//holds data to put into report for each column
                //Build Record Object
                List<Record> recordList = GetListOfRecords(StartDate, EndDate);


                csvFilePath = rootPath;
                csvFilename = "Calibrus_" + StartDate.ToString("yyyyMMdd") + ".csv";///Calibrus_YYYYMMDD.csv
                csvFilePath += csvFilename; //build filepath

                //Write Report
                WriteCSV(recordList, csvFilePath, ref strBuffer);


                //FTP report                    
                FTPFile(rootPath, csvFilename, csvFilePath, hostName, toDir, userName, password);


            }
            catch (Exception ex)
            {
                SendErrorMessage(ex);
                //throw ex;
            }
            finally
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }

        }
        #endregion Main


        #region Write CSV
        public static void WriteCSV(List<Record> records, string csvFilePath, ref string strBuffer)
        {
            StreamWriter sw = File.CreateText(csvFilePath);

            //Used to hold content for report
            StringBuilder sb = new StringBuilder();

            #region Header
            //Enrollment ID	
            strBuffer = "Enrollment ID,";
            sb.AppendFormat(strBuffer);

            //Sales Channel
            strBuffer = "Sales Channel,";
            sb.AppendFormat(strBuffer);

            //Sales Vendor ID	
            strBuffer = "Sales Vendor ID,";
            sb.AppendFormat(strBuffer);

            //Sales Vendor
            strBuffer = "Sales Vendor,";
            sb.AppendFormat(strBuffer);

            //Sales Rep	
            strBuffer = "Sales Rep,";
            sb.AppendFormat(strBuffer);

            //Date of Sale	
            strBuffer = "Date of Sale,";
            sb.AppendFormat(strBuffer);

            //Utility	
            strBuffer = "Utility,";
            sb.AppendFormat(strBuffer);

            //Commmodity	
            strBuffer = "Commmodity,";
            sb.AppendFormat(strBuffer);

            //On-Bill Consent	
            strBuffer = "On-Bill Consent,";
            sb.AppendFormat(strBuffer);

            //Utility Account Number	
            strBuffer = "Utility Account Number,";
            sb.AppendFormat(strBuffer);

            //Product	
            strBuffer = "Product,";
            sb.AppendFormat(strBuffer);

            //First Name	
            strBuffer = "First Name,";
            sb.AppendFormat(strBuffer);

            //Last Name	
            strBuffer = "Last Name,";
            sb.AppendFormat(strBuffer);

            //Service Contact Email	
            strBuffer = "Service Contact Email,";
            sb.AppendFormat(strBuffer);

            //Service Address1	
            strBuffer = "Service Address1,";
            sb.AppendFormat(strBuffer);

            //Service Address2	
            strBuffer = "Service Address2,";
            sb.AppendFormat(strBuffer);

            //Service Address City	
            strBuffer = "Service Address City,";
            sb.AppendFormat(strBuffer);

            //Service Address State	
            strBuffer = "Service Address State,";
            sb.AppendFormat(strBuffer);

            //Service Address Zip Code	
            strBuffer = "Service Address Zip Code,";
            sb.AppendFormat(strBuffer);

            //Service Contact Phone	
            strBuffer = "Service Contact Phone,";
            sb.AppendFormat(strBuffer);

            //Billing Address1	
            strBuffer = "Billing Address1,";
            sb.AppendFormat(strBuffer);

            //Billing Address2	
            strBuffer = "Billing Address2,";
            sb.AppendFormat(strBuffer);

            //Billing Address City	
            strBuffer = "Billing Address City,";
            sb.AppendFormat(strBuffer);

            //Billing Address State	
            strBuffer = "Billing Address State,";
            sb.AppendFormat(strBuffer);

            //Billing Address Zip Code	
            strBuffer = "Billing Address Zip Code,";
            sb.AppendFormat(strBuffer);

            //Billing Contact Phone	
            strBuffer = "Billing Contact Phone,";
            sb.AppendFormat(strBuffer);

            //Language Preference	
            strBuffer = "Language Preference,";
            sb.AppendFormat(strBuffer);

            //Vendor
            strBuffer = "Vendor";
            sb.AppendFormat(strBuffer);

            #endregion Header

            #region Data
            foreach (Record item in records)
            {
                //Enrollment ID	
                strBuffer = item.EnrollmentId.ToString();
                sb.AppendFormat("\r\n{0},", strBuffer); //prepend carriage return on the data rows, so that an empty record doesn't create a blank record after the header.

                //Sales Channel	
                string salesChannel = string.Empty;
                switch (item.Dnis.Trim())
                {
                    case "2277":
                    case "2212":
                        salesChannel = "IB";
                        break;
                    default:
                        salesChannel = "OB";
                        break;
                }

                strBuffer = salesChannel;
                sb.AppendFormat("{0},", strBuffer);

                //Sales Vendor ID
                strBuffer = item.SalesVendorId.ToString();
                sb.AppendFormat("{0},", strBuffer);

                //Sales Vendor	
                strBuffer = item.SalesVendor;
                sb.AppendFormat("{0},", strBuffer);

                //Sales Rep	
                strBuffer = item.SalesRep;
                sb.AppendFormat("{0},", strBuffer);

                //Date of Sale	
                //strBuffer = String.Format("{0:MM/dd/yyyy hh:mm}", item.DateOfSale);
                strBuffer = String.Format("{0:MM/dd/yyyy}", item.DateOfSale);
                sb.AppendFormat("{0},", strBuffer);

                //Utility	
                strBuffer = item.Utility;
                sb.AppendFormat("{0},", strBuffer);

                //Commodity	
                strBuffer = item.Commodity;
                sb.AppendFormat("{0},", strBuffer);

                //On-Bill Consent	
                strBuffer = item.OnbillConsent;
                sb.AppendFormat("{0},", strBuffer);

                //Utility Account Number
                string UtilityAccountNumber = string.Empty;
                if (item.OnbillConsent == "Yes")
                {
                    if (!IsValueNull(item.UtilityAccountNumber))
                    {
                        UtilityAccountNumber = item.UtilityAccountNumber;
                    }
                    else
                    {
                        UtilityAccountNumber = item.ElectricChoiceId;
                    }
                }

                strBuffer = UtilityAccountNumber;
                sb.AppendFormat("{0},", strBuffer);

                //Product	         
                string input = item.Product;
                string product = string.Empty;
                if (input.Contains(" - HOU") || input.Contains(" - DFW"))
                {
                    product = input.Substring(0, input.Length - 6);
                }
                else
                {
                    product = input;
                }


                strBuffer = product;
                sb.AppendFormat("{0},", strBuffer);

                //First Name	
                strBuffer = item.FirstName;
                sb.AppendFormat("{0},", strBuffer);

                //Last Name	
                strBuffer = item.LastName;
                sb.AppendFormat("{0},", strBuffer);

                //Service Contact Email	
                strBuffer = item.Email;
                sb.AppendFormat("{0},", strBuffer);

                //Service Address1	
                strBuffer = item.ServiceAddress1;
                sb.AppendFormat("{0},", strBuffer);

                //Service Address2	
                strBuffer = item.ServiceAddress2;
                sb.AppendFormat("{0},", strBuffer);

                //Service Address City	
                strBuffer = item.ServiceCity;
                sb.AppendFormat("{0},", strBuffer);

                //Service Address State	
                strBuffer = item.ServiceState;
                sb.AppendFormat("{0},", strBuffer);

                //Service Address Zip Code	
                strBuffer = item.ServiceZip;
                sb.AppendFormat("{0},", strBuffer);

                //Service Contact Phone	
                strBuffer = item.ServicePhone;
                sb.AppendFormat("{0},", strBuffer);

                //Billing Address1	
                strBuffer = item.BillingAddress1;
                sb.AppendFormat("{0},", strBuffer);

                //Billing Address2	
                strBuffer = item.BillingAddress2;
                sb.AppendFormat("{0},", strBuffer);

                //Billing Address City	
                strBuffer = item.BillingCity;
                sb.AppendFormat("{0},", strBuffer);

                //Billing Address State	
                strBuffer = item.BillingState;
                sb.AppendFormat("{0},", strBuffer);

                //Billing Address Zip Code	
                strBuffer = item.BillingZip;
                sb.AppendFormat("{0},", strBuffer);

                //Billing Contact Phone	
                strBuffer = item.BillingPhone;
                sb.AppendFormat("{0},", strBuffer);

                //Language Preference	
                strBuffer = item.LanguagePreference;
                sb.AppendFormat("{0},", strBuffer);

                //Vendor
                strBuffer = item.Vendor;
                sb.AppendFormat("{0}", strBuffer);
            }
            //save file 
            sw.WriteLine(sb.ToString());
            sb.Remove(0, sb.Length);
            sw.Close();
            sw.Dispose();

            #endregion Data
        }
        #endregion Write CSV

        #region Get Data
        #region Method to Get RecordData (1 method)
        private static List<Record> GetListOfRecords(DateTime sDate, DateTime eDate)
        {

            //Select   hs.HomeServicesId, hsp.SalesChannel, hs.VendorAgentId, m.VendorAgentId, v.VendorName,hs.VendorAgentId, m.CallDateTime, hs.ResponseId,
            //         m.UDCCode, hpzclu.Jurisdiction, m.SignUpType, m.UDCAccountNumber, hsp.HomeServicesPlan, hs.AddOns, hpzclu.Jurisdiction,
            //         hs.ServiceFirstName, m.ServiceFirstName, hs.ServiceLastName, m.ServiceLastName, hs.ServiceEmail, m.ServiceEmail,
            //         hs.ServiceAddress1, m.ServiceAddress1, hs.ServiceAddress2 , m.ServiceAddress2, hs.ServiceCity , m.ServiceCity,
            //         hs.ServiceState ,m.ServiceState, hs.ServiceZipCode , m.ServiceZipCode, hs.ServicePhoneNumber , m.ServicePhoneNumber, hs.BillingAddress1,  m.BillingAddress1,hs.BillingAddress2, m.BillingAddress2,
            //         hs.BillingCity, m.BillingCity, hs.BillingState , m.BillingState, hs.BillingZipCode , m.BillingZipCode,  hs.ServicePhoneNumber , m.ServicePhoneNumber,
            //         hs.ElectricChoiceId, m.Dnis
            //FROM tblHomeServices hs
            //JOIN tblHomeServicesPlan hsp on hs.HomeServicesPlanId = hsp.HomeServicesPlanId
            //JOIN tblVendor v on hs.VendorId = v.VendorId
            //JOIN tblMain m on hs.HomeServicesId = m.HomeServicesId
            //JOIN tblHomeServicesZipCodeLookUp hpzclu on m.ServiceZipCode = hpzclu.ZipCode
            //where m.CallDateTime > '2/1/2016'
            //and m.CallDateTime < '4/1/2016'
            //and m.Verified = '1'
            List<Record> records = new List<Record>();
            try
            {
                using (ConstellationEntities entitites = new ConstellationEntities())
                {
                    var query = (from hs in entitites.tblHomeServices
                                 join hsp in entitites.tblHomeServicesPlans on hs.HomeServicesPlanId equals hsp.HomeServicesPlanId
                                 join v in entitites.tblVendors on hs.VendorId equals v.VendorId
                                 join m in entitites.tblMains on hs.HomeServicesId equals m.HomeServicesId
                                 join hspzclu in entitites.tblHomeServicesZipCodeLookUps on m.ServiceZipCode equals hspzclu.ZipCode
                                 where m.CallDateTime > sDate && m.CallDateTime < eDate
                                 && m.Verified == "1"
                                 select new
                                 {
                                     EnrollmentId = hs.HomeServicesId,
                                     SalesChannel = hsp.SalesChannel,
                                     SalesVendorId = v.PartnerId,   //hs.VendorId,  //hs.VendorAgentId ?? m.VendorAgentId, 
                                     // if vendorid = 44   then use parterid 2
                                     SalesVendor = v.VendorName ?? "",
                                     SalesRep = hs.VendorAgentId,
                                     DateOfSale = m.CallDateTime,
                                     ResponseId = hs.ResponseId,
                                     Utility = hspzclu.Jurisdiction ?? "",// m.UDCCode ?? "",
                                     Commodity = m.SignUpType == "HS" ? "N/A" : m.SignUpType, //m.SignUpType ?? "",
                                     UtilityAccountNumber = m.UDCAccountNumber ?? "",
                                     Product = hsp.HomeServicesPlan,
                                     AddOns = hs.AddOns,
                                     Jurisdiction = hspzclu.Jurisdiction,
                                     FirstName = hs.ServiceFirstName ?? m.ServiceFirstName,
                                     LastName = hs.ServiceLastName ?? m.ServiceLastName,
                                     Email = hs.ServiceEmail ?? m.ServiceEmail,
                                     ServiceAddress1 = hs.ServiceAddress1 ?? m.ServiceAddress1,
                                     ServiceAddress2 = hs.ServiceAddress2 ?? m.ServiceAddress2,
                                     ServiceCity = hs.ServiceCity ?? m.ServiceCity,
                                     ServiceState = hs.ServiceState ?? m.ServiceState,
                                     ServiceZip = hs.ServiceZipCode ?? m.ServiceZipCode,
                                     ServicePhone = hs.ServicePhoneNumber ?? m.ServicePhoneNumber,
                                     BillingAddress1 = hs.BillingAddress1 ?? m.BillingAddress1,
                                     BillingAddress2 = hs.BillingAddress2 ?? m.BillingAddress2,
                                     BillingCity = hs.BillingCity ?? m.BillingCity,
                                     BillingState = hs.BillingState ?? m.BillingState,
                                     BillingZip = hs.BillingZipCode ?? m.BillingZipCode,
                                     BillingPhone = hs.ServicePhoneNumber ?? m.ServicePhoneNumber,
                                     ElectricChoiceId = hs.ElectricChoiceId,
                                     Dnis = m.Dnis,
                                     OnBillConsent = (hs.IncludeOnBGEBill == true ? "Yes" : "No"),

                                 }).ToList();



                    foreach (var item in query)
                    {

                        string langpref = string.Empty;

                        switch (item.Dnis.Trim())
                        {
                            case "2277":
                            case "2278":
                            case "2298":
                                langpref = "English";
                                break;
                            case "2212":
                            case "2296":
                                langpref = "Spanish";
                                break;
                        }
                        Record record = new Record(item.EnrollmentId, item.SalesChannel, item.SalesVendorId.ToString(), item.SalesVendor, item.SalesRep,
                                                    item.DateOfSale, item.Utility, item.Commodity, item.UtilityAccountNumber, item.Product, item.AddOns,
                                                    item.Jurisdiction, item.FirstName, item.LastName, item.Email, item.ServiceAddress1, item.ServiceAddress2,
                                                    item.ServiceCity, item.ServiceState, item.ServiceZip, item.ServicePhone, item.BillingAddress1,
                                                    item.BillingAddress2, item.BillingCity, item.BillingState, item.BillingZip, item.BillingPhone,
                                                    item.ElectricChoiceId, item.Dnis, langpref, "Calibrus", item.OnBillConsent);


                        records.Add(record);
                    }
                }
            }
            catch (Exception ex)
            {
                SendErrorMessage(ex);
            }
            return records;
        }
        #endregion

        #endregion Get Data


        #region Utilities
        /// <summary>
        /// Takes the value passed in and tests to see if it is a NULL type, 
        /// that being empty string, white space or a string type of NULL
        /// </summary>
        /// <param name="value">string</param>
        /// <returns>true or false</returns>
        private static bool IsValueNull(string value)
        {
            bool status = false;
            if (string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value) || value.ToUpper() == "NULL")
            {
                status = true;
            }
            return status;
        }


        private static void FTPFile(string reportPath, string filename, string putFilePath, string HostName, string ToDir, string UserName, string Password)
        {

            putFilePath = string.Format(reportPath + filename);
            try
            {
                //Renci.sshNet
                UploadFileRenciSshNet(HostName, UserName, Password, ToDir, putFilePath);

            }
            catch (Exception ex)
            {
                SendErrorMessage(ex);
            }
        }


        /// <summary>
        /// This sample will upload a file on your local machine to the remote system. 
        /// Using the Renci.SshNet dll found on \\Tmpdev2\Production\CalibrusFramework\2012\RenciSshNet which is written in .net 4.0 and is a rewrite of the Tamir.SharpSSH
        /// http://sshnet.codeplex.com/wikipage?title=Draft%20for%20Documentation%20page
        /// </summary>
        private static void UploadFileRenciSshNet(string host, string username, string password, string toDir, string localFileName)
        {
            //string host = "";
            //string username = "";
            //string password = "";
            //string localFileName = "";
            string remoteFileName = System.IO.Path.GetFileName(localFileName);

            using (var sftp = new SftpClient(host, username, password))
            {

                sftp.ConnectionInfo.Timeout = TimeSpan.FromSeconds(60);
                sftp.Connect();

                using (Stream file = File.OpenRead(localFileName))
                {
                    sftp.ChangeDirectory(toDir);
                    sftp.UploadFile(file, remoteFileName);
                }

                sftp.Disconnect();
            }
        }
        private static void GetDates(out DateTime StartDate, out DateTime EndDate)
        {
            DateTime baseDate;
            DateTimeService.ReportingDateTimeService dts = null;
            try
            {
                dts = new DateTimeService.ReportingDateTimeService();
                baseDate = DateTime.Parse(dts.GetDateTime());
            }
            catch (Exception)
            {
                baseDate = DateTime.Now;
            }
            finally
            {
                dts.Dispose();
            }

            //int baseHour = baseDate.Hour;
            //int baseMinute = -1;

            //if (baseDate.Minute >= 0 && baseDate.Minute < 30)
            //    baseMinute = 0;
            //else
            //    baseMinute = 30;

            baseDate = new DateTime(baseDate.Year, baseDate.Month, baseDate.Day, baseDate.Hour, baseDate.Minute, baseDate.Second);//current date time


            StartDate = new DateTime(baseDate.Year, baseDate.Month, baseDate.Day, 0, 0, 0).AddDays(-1); //Day before Current Date, this will be the Start date
            EndDate = new DateTime(baseDate.Year, baseDate.Month, baseDate.Day, 0, 0, 0); //Current Date the report runs, this will be the End date

        }

        private static void SendErrorMessage(Exception ex)
        {
            Calibrus.ErrorHandler.Alerting alert = new Calibrus.ErrorHandler.Alerting("ConstellationHomeServiceDailyCSV");
            alert.SendAlert(ex.Source, ex.Message, Environment.MachineName, Environment.UserName, Environment.Version.ToString());
        }

        #endregion Utilities


    }
}
