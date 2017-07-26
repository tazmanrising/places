using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using Calibrus.ErrorHandler;
using Calibrus.Mail;

namespace ConstellationNoSaleAlert
{
    public class NoSaleAlert
    {


        public static void Main(string[] args)
        {
            DateTime CurrentDate = new DateTime();


            List<Record> RecordsToSend = new List<Record>();
            try
            {

                GetDates(out CurrentDate);

                //Build a temporary list to hold the AlertId's and MainId's which we will use to build the Record Object
                List<int?[]> mainIdsToSend = new List<int?[]>();

                //Get Pending AlertId's and MainId's to Email
                foreach (tblAlert pendingAlerts in GetAlertRecords())
                {
                    mainIdsToSend.Add(new int?[2] { pendingAlerts.AlertId, pendingAlerts.MainId });
                }

                //Build Record Object
                foreach (var item in mainIdsToSend)
                {
                    Record recordData = null;
                    //get Record Data from tblMain joined with tblLoadFile
                    recordData = GetRecordData((int?)item[0], (int?)item[1]);

                    RecordsToSend.Add(recordData);
                }

                //Send Emails
                foreach (Record record in RecordsToSend)
                {
                    bool isErr = false;
                    //Send Email
                    SendEmail(record, ref isErr);

                    if (isErr)
                    {
                        //update Record as failed
                        UpdateAlertRecord(record.AlertId, false);
                    }
                    else
                    {
                        //update Record as sent
                        UpdateAlertRecord(record.AlertId, true);
                    }
                }

            }
            catch (Exception ex)
            {
                SendErrorMessage(ex);
            }

        }
        #region EF DataMethods

        /// <summary>
        /// Used to grab all pending records that need to be emailed
        /// </summary>
        /// <returns></returns>
        private static List<tblAlert> GetAlertRecords()
        {
            List<tblAlert> ctxRecords = new List<tblAlert>();

            try
            {
                using (ConstellationEntities entities = new ConstellationEntities())
                {
                    ctxRecords = entities.tblAlerts
                        .Where(x => x.Sent == "0" && x.MainId != 0 && x.MainId !=-1).ToList();
                }
            }
            catch (Exception ex)
            {
                SendErrorMessage(ex);
            }
            return ctxRecords;
        }

        /// <summary>
        /// Gets joined tblMain and tblLoadFile Record data based on mainid passed in
        /// </summary>
        /// <param name="mainId"></param>
        /// <returns>List of Records</returns>
        private static Record GetRecordData(int? alertId, int? mainId)
        {
            string MailToGlobal = string.Empty;//Global = 44
            string MailToProtocall = string.Empty;//Protocol = 86
            string MailToDistro = string.Empty;
            Record recordsList = null;
            try
            {
                MailToGlobal = ConfigurationManager.AppSettings["MailToGlobal"].ToString();
                MailToProtocall = ConfigurationManager.AppSettings["MailToProtocall"].ToString();

                using (ConstellationEntities entities = new ConstellationEntities())
                {
                    var query = (from m in entities.tblMains
                                 join a in entities.tblAlerts on m.MainId equals a.MainId
                                 where m.MainId == mainId
                                 select new { a.AlertId, m.MainId, a.Script, m.CallDateTime, m.VendorId, m.VendorAgentId, m.ResponseId, m.ConcernCode, m.Concern }
                                  );

                    foreach (var q in query)
                    {
                        switch (q.VendorId)
                        {
                            case "86":
                                MailToDistro = MailToGlobal;
                                break;
                            case "44":
                                MailToDistro = MailToProtocall;
                                break;
                        }


                        recordsList = new Record(q.AlertId, q.MainId, q.Script, q.CallDateTime, q.VendorId, q.VendorAgentId, q.ResponseId, q.ConcernCode, q.Concern, MailToDistro);
                    }
                }
            }
            catch (Exception ex)
            {
                SendErrorMessage(ex,alertId.ToString(),mainId.ToString());
            }
            return recordsList;
        }

        /// <summary>
        /// Updates the tblAlert with the passed in AlertId
        /// </summary>
        /// <param name="AlertId"></param>
        private static void UpdateAlertRecord(int? AlertId, bool isSent)
        {
            string sent = string.Empty;
            
            try
            {
                if (isSent)
                {
                    sent = "1";
                }
                else
                {
                    sent = "9";
                }

                tblAlert alert = null;
                using (ConstellationEntities data = new ConstellationEntities())
                {
                    alert = (from a in data.tblAlerts
                             where a.AlertId == AlertId
                             select a).FirstOrDefault();

                    
                    alert.Sent = sent;
                    if (isSent)
                    {
                        alert.SentDateTime = DateTime.Now;
                    }
                    data.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                SendErrorMessage(ex);
            }
        }
        #endregion

        #region Utilities
        private static void SendEmail(Record recordData, ref bool isErr)
        {

            string strMsgBody = string.Empty;
            string strSubject = string.Empty;
            try
            {
                StringBuilder sb = new StringBuilder();

                sb.AppendFormat("ATTENTION!!! Verification was No Saled by the CALIBRUS Agent.");
                sb.AppendLine(Environment.NewLine);
                sb.AppendFormat("Date: {0}\r", recordData.CallDateTime);
                sb.AppendFormat("Vendor ID: {0}\r", recordData.VendorId);
                sb.AppendFormat("Sales Rep ID: {0}\r", recordData.VendorAgentId);
                sb.AppendLine(Environment.NewLine);
                sb.AppendFormat("Response ID: {0}\r", recordData.ResponseId);
                sb.AppendFormat("Verification Code: {0}\r", recordData.ConcernCode);
                sb.AppendFormat("No Sale Reason: No Sale –  {0}\r", recordData.Concern);

                sb.AppendLine(Environment.NewLine);

                strMsgBody = sb.ToString();


                SmtpMail mail = new SmtpMail("TMPWEB1", false);


                mail.AddRecipient(recordData.MailToDistro, RecipientType.To);
                mail.From = "noreply@calibrus.com";
                mail.Subject = string.Format("Constellation - {0} - Verification No Saled!", recordData.Script);
                mail.Body = strMsgBody;
                mail.SendMessage();
            }
            catch (Exception ex)
            {
                isErr = true;
                SendErrorMessage(ex, recordData.AlertId.ToString(), recordData.MainId.ToString());
            }
        }
        private static void GetDates(out DateTime CurrentDate)
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

            CurrentDate = new DateTime(baseDate.Year, baseDate.Month, baseDate.Day, baseDate.Hour, baseDate.Minute, baseDate.Second);//current date time   

        }
        private static void SendErrorMessage(Exception ex)
        {
            Calibrus.ErrorHandler.Alerting alert = new Calibrus.ErrorHandler.Alerting("ConstellationNoSaleAlert");
            alert.SendAlert(ex.Source, ex.Message, Environment.MachineName, Environment.UserName, Environment.Version.ToString());
        }
        private static void SendErrorMessage(Exception ex, string tblAlertId)
        {
            Calibrus.ErrorHandler.Alerting alert = new Calibrus.ErrorHandler.Alerting("ConstellationNoSaleAlert");
            alert.SendAlert(ex.Source, String.Format("tblAlertId: {0} -- {1}", tblAlertId, ex.Message), Environment.MachineName, Environment.UserName, Environment.Version.ToString());
        }
        private static void SendErrorMessage(Exception ex, string tblAlertId, string tblMainId)
        {
            Calibrus.ErrorHandler.Alerting alert = new Calibrus.ErrorHandler.Alerting("ConstellationNoSaleAlert");
            alert.SendAlert(ex.Source, String.Format("tblResponseId: {0} tblMainId: {1} -- {2}", tblAlertId, tblMainId, ex.Message), Environment.MachineName, Environment.UserName, Environment.Version.ToString());
        }
        #endregion
    }
}
