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

namespace Frontier911FailedReportExport
{
    public class ReportExport
    {

        #region Main
        public static void Main(string[] args)
        {
            string rootPath = string.Empty;
            string mailRecipientTO = string.Empty;

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

            try
            {
                string strBuffer = "";//holds data to put into report for each column

                //Build Record Object
                //List<FailedRecord> failedRecords = GetFailedRecords(StartDate, EndDate);
                List<sp911BrightPatternFailedReport_Result> failedRecords = GetFailedRecords(StartDate, EndDate);

                //if we have records then we can build the report
                if (failedRecords.Count() > 0)
                {
                    csvFilePath = rootPath;
                    //E911_TPVYYYYMMDD.txt
                    csvFilename = "E911_TPV" + CurrentDate.ToString("yyyyMMdd") + ".txt";
                    csvFilePath += csvFilename; //build filepath

                    //Write Report
                    BuildCSV(failedRecords, csvFilePath, ref strBuffer);

                    //Email Report to Distro
                    SendEmail(ref csvFilePath, CurrentDate, mailRecipientTO);
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

        public static void BuildCSV(List<sp911BrightPatternFailedReport_Result> failedRecords, string csvFilePath, ref string strBuffer)
        {
            StreamWriter sw = File.CreateText(csvFilePath);

            //Used to hold content for report
            StringBuilder sb = new StringBuilder();

            #region Header
            strBuffer = "Subscriber Id|";
            sb.AppendFormat(strBuffer);

            strBuffer = "Name|";
            sb.AppendFormat(strBuffer);

            strBuffer = "Signature|";
            sb.AppendFormat(strBuffer);

            strBuffer = "Birth Year|";
            sb.AppendFormat(strBuffer);

            strBuffer = "TN|";
            sb.AppendFormat(strBuffer);

            strBuffer = "Email|";
            sb.AppendFormat(strBuffer);

            strBuffer = "General Action|";
            sb.AppendFormat(strBuffer);

            strBuffer = "General Date|";
            sb.AppendFormat(strBuffer);

            strBuffer = "E911Action|";
            sb.AppendFormat(strBuffer);

            strBuffer = "E911Date|";
            sb.AppendFormat(strBuffer);

            strBuffer = "Is Data|";
            sb.AppendFormat(strBuffer);

            strBuffer = "Is Voip|";
            sb.AppendFormat(strBuffer);

            strBuffer = "User|";
            sb.AppendFormat(strBuffer);

            strBuffer = "Number Of Call Attempts|";
            sb.AppendFormat(strBuffer);

            strBuffer = "Last Call Attempt|";
            sb.AppendFormat(strBuffer);

            strBuffer = "Customer Accepted|";
            sb.AppendFormat(strBuffer);

            strBuffer = "Customer To Be Disconnected|";
            sb.AppendFormat(strBuffer);

            strBuffer = "Reasons for Failed Attempts\r\n";
            sb.AppendFormat(strBuffer);

            #endregion Header

            #region Data
            int recordCount = failedRecords.Count();
            int recordCursor = 0;
            foreach (var item in failedRecords)
            {
                recordCursor++;



                //Subscriber Id
                strBuffer = item.SubscriberId;
                sb.AppendFormat("{0}|", strBuffer);

                //Name
                strBuffer = item.Name;
                sb.AppendFormat("{0}|", strBuffer);

                //Signature
                strBuffer = item.Signature;
                sb.AppendFormat("{0}|", strBuffer);

                //Birth Year
                strBuffer = item.BirthYear;
                sb.AppendFormat("{0}|", strBuffer);

                //TN    
                strBuffer = item.TN;
                sb.AppendFormat("{0}|", strBuffer);

                //Email
                strBuffer = item.Email;
                sb.AppendFormat("{0}|", strBuffer);

                //General Action
                strBuffer = item.GeneralAction;
                sb.AppendFormat("{0}|", strBuffer);

                //General Date
                strBuffer = string.Format("{0: dd MMM yyyy hh:mm tt}", item.GeneralDate.ToString());
                sb.AppendFormat("{0}|", strBuffer);

                //E911Action
                strBuffer = item.E911Action;
                sb.AppendFormat("{0}|", strBuffer);

                //E911Date
                strBuffer = string.Format("{0: dd MMM yyyy hh:mm tt}", item.E911Date.ToString());
                sb.AppendFormat("{0}|", strBuffer);

                //Is Data
                strBuffer = item.IsData;
                sb.AppendFormat("{0}|", strBuffer);

                //Is Voip
                strBuffer = item.IsVoip;
                sb.AppendFormat("{0}|", strBuffer);

                //User
                strBuffer = item.User;
                sb.AppendFormat("{0}|", strBuffer);

                //Number of Call Attempts
                strBuffer = item.Attempts.ToString();
                sb.AppendFormat("{0}|", strBuffer);

                //Last Call Attempt  (Date & Time)
                strBuffer = string.Format("{0: dd MMM yyyy hh:mm tt}", item.CallTime.ToString());
                sb.AppendFormat("{0}|", strBuffer);

                //Customer Accepted
                strBuffer = item.Customer_Accepted;
                sb.AppendFormat("{0}|", strBuffer);

                //Customer To Be Disconnected
                strBuffer = item.Customer_To_Be_Disconnected;
                sb.AppendFormat("{0}|", strBuffer);

                //Reasons for Failed Attempts
                strBuffer = item.CallDispositionCode;

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

        private static List<sp911BrightPatternFailedReport_Result> GetFailedRecords(DateTime startDate, DateTime endDate)
        {
            List<sp911BrightPatternFailedReport_Result> failedRecordList = new List<sp911BrightPatternFailedReport_Result>();
            try
            {
                using (FrontierEntities entities = new FrontierEntities())
                {
                    failedRecordList = entities.sp911BrightPatternFailedReport(startDate: startDate, endDate: endDate).ToList();
                }
            }
            catch (Exception ex)
            {
                SendErrorMessage(ex);
                //throw ex;
            }
            return failedRecordList;
        }
        //private static List<FailedRecord> GetFailedRecords(DateTime startDate, DateTime endDate)
        //{
        //    //SELECT [SubscriberId]
        //    //      ,[Name]
        //    //      ,[Signature]
        //    //      ,[BirthYear]
        //    //      ,[TN]
        //    //      ,[Email]
        //    //      ,[GeneralAction]
        //    //      ,[GeneralDate]
        //    //      ,[E911Action]
        //    //      ,[E911Date]
        //    //      ,[IsData]
        //    //      ,[IsVoip]
        //    //      ,[User]
        //    //      ,[Attempts]
        //    //      ,[LastAttemptDate]	  
        //    //      ,case  when [LastDisposition] = 'Verified' THEN 'Y'
        //    //            ELSE 'N'   
        //    //            END as Customer_Accepted
        //    //      ,case  when [Processed] <> '0' AND [LastDisposition] <> 'Verified' THEN 'Y'
        //    //            ELSE 'N'   
        //    //            END as Customer_To_Be_Disconnected
        //    //      ,[LastDispositionCode]
        //    //FROM [Frontier].[dbo].[tblE911LoadFile]
        //    //WHERE [LastDisposition] <> 'Verified'
        //    //AND [Processed] <> '0'
        //    //    AND [LastAttemptDate] > @StartDate 
        //    //    AND [LastAttemptDate] < DATEADD(day, 1, @EndDate)
        //    //    ORDER BY [LastAttemptDate]


        //    List<FailedRecord> failedRecordList = new List<FailedRecord>();
        //    try
        //    {
        //        using (FrontierEntities entities = new FrontierEntities())
        //        {
        //            var query = (from lf in entities.tblE911LoadFile
        //                         where lf.LastAttemptDate > startDate
        //                         && lf.LastAttemptDate < endDate
        //                         && lf.LastDisposition != "Verified"
        //                         && lf.Processed != "0"
        //                         select lf).OrderBy(x => x.LastAttemptDate).ToList();

        //            foreach (var item in query)
        //            {
        //                FailedRecord failedRecord = new FailedRecord();
        //                failedRecord.SubscriberId = item.SubscriberId;
        //                failedRecord.Name = item.Name;
        //                failedRecord.Signature = item.Signature;
        //                failedRecord.BirthYear = item.BirthYear;
        //                failedRecord.TN = item.TN;
        //                failedRecord.Email = item.Email;
        //                failedRecord.GeneralAction = item.GeneralAction;
        //                failedRecord.GeneralDate = item.GeneralDate;
        //                failedRecord.E911Action = item.E911Action;
        //                failedRecord.E911Date = item.E911Date;
        //                failedRecord.IsData = item.IsData;
        //                failedRecord.IsVoip = item.IsVoip;
        //                failedRecord.User = item.User;
        //                failedRecord.Attempts = item.Attempts;
        //                failedRecord.LastAttemptDate = item.LastAttemptDate;
        //                failedRecord.CustomerAccepted = item.LastDisposition == "Verified" ? "Y" : "N";
        //                failedRecord.CustomerToBeDisconnected = item.Processed != "0" && item.LastDisposition != "Verified" ? "Y" : "N";
        //                failedRecord.LastDispositionCode = item.LastDispositionCode;

        //                failedRecordList.Add(failedRecord);

        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        SendErrorMessage(ex);
        //        //throw ex;
        //    }
        //    return failedRecordList;
        //}

        #endregion Get Data

        #region Utilities

        private static void SendEmail(ref string xlsFilePath, DateTime reportDate, String strToEmail)
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


                mail.From = "reports1@calibrus.com";

                mail.Subject = "E911 Failures ToS for " + reportDate.ToString("MMM") + " " + reportDate.ToString("dd") + " " + reportDate.ToString("yyyy") + ".";


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
            Calibrus.ErrorHandler.Alerting alert = new Calibrus.ErrorHandler.Alerting("Frontier911FailedReportExport");
            alert.SendAlert(ex.Source, ex.Message, Environment.MachineName, Environment.UserName, Environment.Version.ToString());
        }

        #endregion Utilities
    }
}
