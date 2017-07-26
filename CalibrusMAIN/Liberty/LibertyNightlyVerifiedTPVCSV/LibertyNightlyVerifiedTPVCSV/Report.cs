using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using Calibrus.ErrorHandler;
using Calibrus.Mail;
using System.Diagnostics;
using System.Security;

namespace LibertyNightlyVerifiedTPVCSV
{
    public class Report
    {
        #region Main
        public static void Main(string[] args)
        {
            string rootPath = string.Empty;
            string mailRecipientTO = string.Empty;
            string mailRecipientBCC = string.Empty;

            //get report interval
            DateTime CurrentDate = new DateTime();
            DateTime StartDate = new DateTime();
            DateTime EndDate = new DateTime();

            //start to  build the form pathing
            string csvFilename = string.Empty;
            string csvFilePath = string.Empty;

            if (args.Length > 0)
            {
                if (DateTime.TryParse(args[0], out CurrentDate))
                {
                    GetDates(out CurrentDate, out StartDate, out EndDate);
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
                GetDates(out CurrentDate, out StartDate, out EndDate);
            }

            //grab values from app.config
            rootPath = ConfigurationManager.AppSettings["rootPath"].ToString();
            mailRecipientTO = ConfigurationManager.AppSettings["mailRecipientTO"].ToString();
            mailRecipientBCC = ConfigurationManager.AppSettings["mailRecipientBCC"].ToString();

            try
            {
                string strBuffer = "";//holds data to put into report for each column

                //Build Record Object
                List<Record> Records = getRecords(StartDate, EndDate);

                //if we have records then we can build the report
                if (Records.Count() > 0)
                {
                    csvFilePath = rootPath;
                    csvFilename = "Admin Nightly File-All Verified TPVs-All Channels" + CurrentDate.ToString("yyyyMMddhhmmss") + ".csv";
                    csvFilePath += csvFilename; //build filepath

                    //Write Report
                    BuildCSV(Records, csvFilePath, ref strBuffer);

                    //Copy File to the FTP Folder
                    //CopyFileAndMove(rootPath, copyToPath, ref csvFilename);

                    //Email Report to Distro
                    SendEmail(ref csvFilePath, CurrentDate, mailRecipientTO, mailRecipientBCC);
                }

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

        #region CSV

        public static void BuildCSV(List<Record> records, string csvFilePath, ref string strBuffer)
        {
            StreamWriter sw = File.CreateText(csvFilePath);

            //Used to hold content for report
            StringBuilder sb = new StringBuilder();

            #region Header
            strBuffer = "LangCode,";
            sb.AppendFormat(strBuffer);

            strBuffer = "BTN,";
            sb.AppendFormat(strBuffer);

            strBuffer = "AgentID,";
            sb.AppendFormat(strBuffer);

            strBuffer = "SalesChannel,";
            sb.AppendFormat(strBuffer);

            strBuffer = "FirstName,";
            sb.AppendFormat(strBuffer);

            strBuffer = "LastName,";
            sb.AppendFormat(strBuffer);

            strBuffer = "AccountNumber,";
            sb.AppendFormat(strBuffer);

            strBuffer = "OfferCode,";
            sb.AppendFormat(strBuffer);

            strBuffer = "Rate,";
            sb.AppendFormat(strBuffer);

            strBuffer = "Term,";
            sb.AppendFormat(strBuffer);

            strBuffer = "EstDateEffect,";
            sb.AppendFormat(strBuffer);

            strBuffer = "PIN,";
            sb.AppendFormat(strBuffer);

            strBuffer = "ServiceAddress,";
            sb.AppendFormat(strBuffer);

            strBuffer = "ServiceCity,";
            sb.AppendFormat(strBuffer);

            strBuffer = "ServiceState,";
            sb.AppendFormat(strBuffer);

            strBuffer = "ServiceZip,";
            sb.AppendFormat(strBuffer);

            strBuffer = "BillingAddress,";
            sb.AppendFormat(strBuffer);

            strBuffer = "BillingCity,";
            sb.AppendFormat(strBuffer);

            strBuffer = "BillingState,";
            sb.AppendFormat(strBuffer);

            strBuffer = "BillingZip,";
            sb.AppendFormat(strBuffer);

            strBuffer = "VerificationNumber,";
            sb.AppendFormat(strBuffer);

            strBuffer = "TPV Code (R or C),";
            sb.AppendFormat(strBuffer);

            strBuffer = "FEIN\r\n";
            sb.AppendFormat(strBuffer);

            #endregion Header

            #region Data
            int recordCount = records.Count();
            int recordCursor = 0;
            foreach (Record item in records)
            {
                recordCursor++;

                //LangCode
                strBuffer = item.Language == "English" ? "E" : "S";
                sb.AppendFormat("{0},", strBuffer);

                //BTN	
                strBuffer = item.Btn;
                sb.AppendFormat("{0},", strBuffer);

                //AgentID	
                strBuffer = item.SalesAgentId;
                sb.AppendFormat("{0},", strBuffer);

                //SalesChannel	
                strBuffer = item.SalesChannel;
                sb.AppendFormat("{0},", strBuffer);

                //FirstName	
                strBuffer = item.AuthorizationFirstName.Replace(",", "");
                sb.AppendFormat("{0},", strBuffer);

                //LastName	
                strBuffer = item.AuthorizationLastName.Replace(",", "");
                sb.AppendFormat("{0},", strBuffer);

                //AccountNumber	
                strBuffer = item.AccountNumber;
                sb.AppendFormat("{0},", strBuffer);

                //OfferCode	
                strBuffer = item.OfferCode;
                sb.AppendFormat("{0},", strBuffer);

                //Rate	
                strBuffer = item.Rate;
                sb.AppendFormat("{0},", strBuffer);

                //Term	
                strBuffer = item.MonthlyTerm;
                sb.AppendFormat("{0},", strBuffer);

                //EstDateEffect	
                strBuffer = item.RateEffectiveDate;
                sb.AppendFormat("{0},", strBuffer);

                //PIN	
                strBuffer = "";
                sb.AppendFormat("{0},", strBuffer);

                //ServiceAddress	
                strBuffer = item.ServiceAddress.Replace(",", "") + " " + (string.IsNullOrEmpty(item.ServiceAddress2) ? "" : item.ServiceAddress2.Replace(",", ""));
                sb.AppendFormat("{0},", strBuffer);

                //ServiceCity	
                strBuffer = item.ServiceCity;
                sb.AppendFormat("{0},", strBuffer);

                //ServiceState	
                strBuffer = item.ServiceState;
                sb.AppendFormat("{0},", strBuffer);

                //ServiceZip	
                strBuffer = item.ServiceZip;
                sb.AppendFormat("{0},", strBuffer);

                //BillingAddress	
                strBuffer = item.BillingAddress.Replace(",", "") + " " + (string.IsNullOrEmpty(item.BillingAddress2) ? "" : item.BillingAddress2.Replace(",", ""));
                sb.AppendFormat("{0},", strBuffer);

                //BillingCity	
                strBuffer = item.BillingCity;
                sb.AppendFormat("{0},", strBuffer);

                //BillingState	
                strBuffer = item.BillingState;
                sb.AppendFormat("{0},", strBuffer);

                //BillingZip	
                strBuffer = item.BillingZip;
                sb.AppendFormat("{0},", strBuffer);

                //VerificationNumber	
                strBuffer = item.VerificationNumber.ToString();
                sb.AppendFormat("{0},", strBuffer);

                //TPV Code (R or C)	
                strBuffer = item.Commercial == true ? "C" : "R";
                sb.AppendFormat("{0},", strBuffer);

                //FEIN
                strBuffer = item.FEIN;

                if (recordCursor < recordCount)
                {
                    sb.AppendFormat("{0}{1}", strBuffer, Environment.NewLine);
                }
                else
                {
                    sb.AppendFormat("{0}", strBuffer); //otherwise end of records no newline to put
                }

                //sb.AppendLine(strBuffer);
            }

            //save file 
            sw.WriteLine(sb.ToString());
            sb.Remove(0, sb.Length);
            sw.Close();
            sw.Dispose();
            #endregion Data
        }

        #endregion CSV

        #region Get Data

        private static List<Record> getRecords(DateTime startDate, DateTime endDate)
        {
            //Select u.Language
            //        ,m.Btn
            //        ,m.SalesAgentId
            //        ,sc.Name
            //        ,m.AuthorizationFirstName	
            //        ,m.AuthorizationLastName	
            //        ,od.AccountNumber	
            //        ,mp.Product	
            //        ,m.Rate
            //        ,ct.MonthlyTerm	
            //        ,m.RateEffectiveDate
            //        ,od.ServiceAddress 
            //        ,od.ServiceAddress2	
            //        ,od.ServiceCity	
            //        ,od.ServiceState	
            //        ,od.ServiceZip
            //        ,od.BillingAddress 
            //        ,od.BillingAddress2	
            //        ,od.BillingCity	
            //        ,od.BillingState
            //        ,od.BillingZip
            //        ,m.MainId
            //        ,m.Verified
            //        ,m.BusinessTaxId
            //FROM [Liberty].[v1].[Main] as m
            //JOIN [Liberty].[v1].[OrderDetail] as od on m.MainId = od.MainId
            //JOIN [Liberty].[v1].[SalesChannel] as sc on sc.SalesChannelId = m.SalesChannelId
            //JOIN [Liberty].[v1].[ContractTerm] as ct on ct.ContractTermId = m.ContractTermId
            //JOIN [Liberty].[v1].[MarketProduct] as mp on mp.MarketProductId = m.MarketProductId
            //JOIN [Liberty].[v1].[User] as u on u.UserId = m.UserId
            //JOIN [Liberty].[v1].[Office] as o on u.OfficeId = o.OfficeId
            //Where m.CallDateTime > '01/01/2015' 
            //AND m.CallDateTime < '01/02/2015'
            //AND m.Verified ='1'


            List<Record> recordList = new List<Record>();
            try
            {
                using (LibertyEntities entities = new LibertyEntities())
                {
                    var query = (from m in entities.Mains
                                 join od in entities.OrderDetails on m.MainId equals od.MainId
                                 join sc in entities.SalesChannels on m.SalesChannelId equals sc.SalesChannelId
                                 join ct in entities.ContractTerms on m.ContractTermId equals ct.ContractTermId
                                 join mp in entities.MarketProducts on m.MarketProductId equals mp.MarketProductId
                                 join u in entities.Users on m.UserId equals u.UserId
                                 join o in entities.Offices on u.OfficeId equals o.OfficeId
                                 //join u in entities.Users on m.UserId equals u.UserId
                                 where m.CallDateTime > startDate
                                 && m.CallDateTime < endDate
                                 && m.Verified == "1"
                                 select new
                                 {
                                     MainId = m.MainId,
                                     DNIS = m.Dnis,
                                     BTN = m.Btn,
                                     AgentID = m.SalesAgentId,
                                     SalesChannel = o.OfficeName,
                                     FirstName = m.AuthorizationFirstName,
                                     LastName = m.AuthorizationLastName,
                                     AccountNumber = od.AccountNumber,
                                     ServiceNumber = od.ServiceNumber,
                                     OfferCode = mp.Product,
                                     Rate = m.Rate,
                                     Term = ct.MonthlyTerm,
                                     EstDateEffect = m.RateEffectiveDate,
                                     ServiceAddress = od.ServiceAddress,
                                     ServiceAddress2 = od.ServiceAddress2,
                                     ServiceCity = od.ServiceCity,
                                     ServiceState = od.ServiceState,
                                     ServiceZip = od.ServiceZip,
                                     BillingAddress = od.BillingAddress,
                                     BillingAddress2 = od.BillingAddress2,
                                     BillingCity = od.BillingCity,
                                     BillingState = od.BillingState,
                                     BillingZip = od.BillingZip,
                                     VerificationNumber = m.MainId,
                                     TPVCode = m.Verified,
                                     Commercial = mp.Commercial,
                                     FEIN = m.BusinessTaxId,
                                     Rate1 = od.SubTermRate1

                                 }).ToList();

                    foreach (var item in query)
                    {
                        string AccountNumber = string.Empty;
                        string Term = string.Empty;
                        string EstDateEffect = string.Empty;
                        DateTime DateFix = new DateTime();
                        string Rate = string.Empty;

                        string Language = "English";
                        if (item.DNIS == "4653")
                        {
                            Language = "Spanish";
                        }

                        if (!IsValueNull(item.ServiceNumber))//Use ServiceNumber if it exists otherwise use accountnumber 
                        {
                            AccountNumber = item.ServiceNumber;
                        }
                        else
                        {
                            AccountNumber = item.AccountNumber;
                        }

                        Term = item.Term.Replace(" Month", ""); //remove Month from Term

                        DateFix = DateTime.Parse(item.EstDateEffect);
                        EstDateEffect = string.Format("{0:M/d/yyyy}", DateFix);//Remove 0 from single months and days

                        if (IsValueNull(item.Rate))//Need to use SubTermRate1 if Rate is null in main
                        {
                            Rate = item.Rate1;
                        }
                        else
                        {
                            Rate = item.Rate;
                        }


                        Record record = new Record(item.MainId, Language, AccountNumber, item.BTN, item.AgentID, item.SalesChannel, item.FirstName, item.LastName, item.OfferCode,
                                                    Rate, Term, EstDateEffect, item.ServiceAddress, item.ServiceAddress2, item.ServiceCity, item.ServiceState,
                                                    item.ServiceZip, item.BillingAddress, item.BillingAddress2, item.BillingCity, item.BillingState, item.BillingZip, item.VerificationNumber,
                                                    item.TPVCode, item.Commercial, item.FEIN);

                        recordList.Add(record);
                    }
                }
            }
            catch (Exception ex)
            {
                SendErrorMessage(ex);
                //throw ex;
            }
            return recordList;
        }

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

        //private static void CopyFileAndMove(string getfile, string putfile, ref string csvFilename)
        //{
        //    getfile += csvFilename;
        //    putfile += csvFilename;
        //    try
        //    {
        //        bool fileExists = File.Exists(putfile);
        //        if (fileExists)
        //        {
        //            //delete it
        //            File.Delete(putfile);
        //        }
        //        //move the file to the processed directory
        //        File.Copy(String.Format(@"{0}", getfile), String.Format(@"{0}", putfile));
        //    }
        //    catch (Exception ex)
        //    { throw ex; }
        //}
        private static void SendEmail(ref string xlsFilePath, DateTime reportDate, String strToEmail, string strBccEmail)
        {
            //string strMsgBody = string.Empty;
            try
            {
                //StringBuilder sb = new StringBuilder();

                //sb.AppendLine("You have completed the Investor Verification process.  Attached is a summary report showing all data  ");
                //sb.AppendLine("entered into the investor verification website and a wav.file of your phone verification recording  ");
                //sb.AppendLine("indicating your verbal verification.   ");
                //sb.AppendLine(Environment.NewLine);
                //sb.AppendLine("If you have any questions please call us at (800) 222-2222.");
                //sb.AppendLine(Environment.NewLine);
                //sb.AppendLine("Sincerely,");
                //sb.AppendLine(Environment.NewLine);
                //sb.AppendLine("The Calibrus Verification Team");
                //strMsgBody = sb.ToString();

                SmtpMail mail = new SmtpMail("TMPWEB1", false);

                mail.AddAttachment(xlsFilePath);//Attach XLS report
                mail.AddRecipient(strToEmail, RecipientType.To);
                mail.AddRecipient(strBccEmail, RecipientType.Bcc);

                mail.From = "reports1@calibrus.com";

                mail.Subject = "Liberty Nightly Verified Report for " + reportDate.ToString("MMM") + " " + reportDate.ToString("dd") + " " + reportDate.ToString("yyyy") + ".";


                //mail.Body = strMsgBody;
                mail.SendMessage();

            }
            catch (Exception ex)
            {
                SendErrorMessage(ex);
            }
        }
        private static void GetDates(out DateTime CurrentDate, out DateTime StartDate, out DateTime EndDate)
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

            CurrentDate = new DateTime(baseDate.Year, baseDate.Month, baseDate.Day, baseDate.Hour, baseDate.Minute, baseDate.Second); //Date for when File runs
            StartDate = new DateTime(baseDate.Year, baseDate.Month, baseDate.Day, 0, 0, 0).AddDays(-1); //previous day  This will be the Start date for the day it runs
            EndDate = new DateTime(baseDate.Year, baseDate.Month, baseDate.Day, 0, 0, 0);  //current day this will be the End date
        }

        private static void SendErrorMessage(Exception ex)
        {
            Calibrus.ErrorHandler.Alerting alert = new Calibrus.ErrorHandler.Alerting("LibertyNightlyVerifiedTPVCSV");
            alert.SendAlert(ex.Source, ex.Message, Environment.MachineName, Environment.UserName, Environment.Version.ToString());
        }

        #endregion Utilities

    }
}
