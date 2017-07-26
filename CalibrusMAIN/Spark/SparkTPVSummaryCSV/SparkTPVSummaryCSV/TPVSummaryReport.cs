using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using Calibrus.ErrorHandler;
using Calibrus.Ftp;
using System.Diagnostics;
using System.Security;


namespace SparkTPVSummaryCSV
{
    public class TPVSummaryReport
    {
        #region Main

        public static void Main(string[] args)
        {

            string rootPath = string.Empty;
            string hostName = string.Empty;
            string userName = string.Empty;
            string password = string.Empty;


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

            hostName = ConfigurationManager.AppSettings["hostName"].ToString();
            userName = ConfigurationManager.AppSettings["userName"].ToString();
            password = ConfigurationManager.AppSettings["password"].ToString();

            try
            {
                string strBuffer = "";//holds data to put into report for each column


                //Build Record Object
                List<Record> Records = GetRecords(StartDate, EndDate);

                if (Records.Count > 0)
                {
                    csvFilePath = rootPath;
                    csvFilename = "Calibrus_TPVSummary_" + CurrentDate.ToString("MM_dd_yyyy") + ".csv";
                    csvFilePath += csvFilename; //build filepath

                    //Write Report
                    WriteCSV(Records, csvFilePath, ref strBuffer);


                    //FTP report                    
                    FTPFile(rootPath, csvFilename, csvFilePath, StartDate, hostName, userName, password);

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
        #endregion

        #region Write CSV
        public static void WriteCSV(List<Record> records, string csvFilePath, ref string strBuffer)
        {
            StreamWriter sw = File.CreateText(csvFilePath);

            //Used to hold content for report
            StringBuilder sb = new StringBuilder();

            #region Header

            strBuffer = "Date,";
            sb.AppendFormat(strBuffer);

            strBuffer = "VendorName,";
            sb.AppendFormat(strBuffer);

            strBuffer = "MarketingType,";
            sb.AppendFormat(strBuffer);

            strBuffer = "LDCCode,";
            sb.AppendFormat(strBuffer);

            strBuffer = "FuelType,";
            sb.AppendFormat(strBuffer);

            strBuffer = "TotalAccounts\r\n";
            sb.AppendFormat(strBuffer);

            #endregion Header
            #region Data

            foreach (Record item in records)
            {
                //Date
                strBuffer = item.Date;
                sb.AppendFormat("{0},", strBuffer);

                //VendorName
                strBuffer = item.VendorName;
                sb.AppendFormat("{0},", strBuffer);

                //MarketingType
                strBuffer = item.UserTypeName;
                sb.AppendFormat("{0},", strBuffer);

                //LDCCode
                strBuffer = item.LdcCode;
                sb.AppendFormat("{0},", strBuffer);

                //FuelType
                strBuffer = item.UtilityTypeName;
                sb.AppendFormat("{0},", strBuffer);

                //TotalAccounts
                strBuffer = item.TotalAccounts.ToString();
                sb.AppendFormat("{0}\r\n", strBuffer);
            }

            //save file 
            sw.WriteLine(sb.ToString());
            sb.Remove(0, sb.Length);
            sw.Close();
            sw.Dispose();

            #endregion Data
        }

        #endregion

        #region Get Data
        private static List<Record> GetRecords(DateTime sDate, DateTime eDate)
        {
            List<Record> records = new List<Record>();
            try
            {
                using (SparkEntities entities = new SparkEntities())
                {
                    //SELECT v.VendorName, utype.UserTypeName as MarketingTyp, ut.LdcCode, uty.UtilityTypeName as FuelType, count(od.OrderDetailId) as TotalAccounts
                    //FROM spark.v1.OrderDetail od 
                    //JOIN spark.v1.Main m on od.MainId = m.MainId
                    //JOIN spark.v1.Program p on p.ProgramId = od.ProgramId
                    //JOIN spark.v1.Utility ut on ut.UtilityId = p.UtilityId
                    //JOIN spark.v1.UtilityType uty on uty.UtilityTypeId = p.UtilityTypeId
                    //JOIN spark.v1.[User] u on u.UserId = m.UserId
                    //JOIN spark.v1.UserType utype on u.UserTypeId = utype.UserTypeId
                    //JOIN spark.v1.Vendor v on v.VendorId = u.VendorId
                    //WHERE   m.CallDateTime >'8/12/2015' 
                    //AND  m.CallDateTime <'8/13/2015' 
                    //AND m.Verified = '1'
                    //GROUP BY v.VendorName, utype.UserTypeName,uty.UtilityTypeName,ut.LdcCode

                    var query = from od in entities.OrderDetails
                                join m in entities.Mains on od.MainId equals m.MainId
                                join p in entities.Programs on od.ProgramId equals p.ProgramId
                                join ut in entities.Utilities on p.UtilityId equals ut.UtilityId
                                join uty in entities.UtilityTypes on p.UtilityTypeId equals uty.UtilityTypeId
                                join u in entities.Users on m.UserId equals u.UserId
                                join usty in entities.UserTypes on u.UserTypeId equals usty.UserTypeId
                                join v in entities.Vendors on u.VendorId equals v.VendorId
                                where m.CallDateTime > sDate
                                && m.CallDateTime < eDate
                                && m.Verified == "1"

                                let grpby = new
                                {
                                    //Date = m.CallDateTime,
                                    VendorName = v.VendorName,
                                    UserTypeName = usty.UserTypeName,
                                    UtilityTypeName = uty.UtilityTypeName,
                                    LDCCode = ut.LdcCode

                                }
                                group od by grpby into t
                                select new
                                {

                                    //Date = t.Key.Date,
                                    VendorName = t.Key.VendorName,
                                    UserTypeName = t.Key.UserTypeName,
                                    UtilityTypeName = t.Key.UtilityTypeName,
                                    LDCCode = t.Key.LDCCode,
                                    Total = t.Count()
                                };
                    foreach (var item in query)
                    {
                        Record record = new Record();
                        //record.Date = string.Format("{0:MM/dd/yyyy}", item.Date);
                        record.Date = string.Format("{0:MM/dd/yyyy}", sDate);
                        record.VendorName = item.VendorName;
                        record.UserTypeName = item.UserTypeName == "Telesales" ? "Res-TM" : "Res-D2D";
                        record.UtilityTypeName = item.UtilityTypeName;
                        record.LdcCode = item.LDCCode;
                        record.TotalAccounts = item.Total;

                        records.Add(record);
                    }


                }
            }
            catch (Exception ex)
            {
                SendErrorMessage(ex);
                //throw ex;
            }

            return records;

        }
        #endregion


        #region Utilities
        private static void FTPFile(string reportPath, string filename, string filePath, DateTime currentDate, string HostName, string UserName, string Password)
        {

            filePath = string.Format(reportPath + filename);
            try
            {
                Calibrus.Ftp.Upload ftp = new Calibrus.Ftp.Upload();
                ftp.Host = new Uri(string.Format("ftp://{0}/", HostName));
                ftp.UserName = UserName;
                ftp.Password = Password;
                ftp.UploadFile(filePath, filename);
               
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
            StartDate = new DateTime(baseDate.Year, baseDate.Month, baseDate.Day, 0, 0, 0); //Day before Current Date, this will be the Start date
            EndDate = new DateTime(baseDate.Year, baseDate.Month, baseDate.Day, 0, 0, 0).AddDays(1); //Current Date the report runs, this will be the End date
        }

        private static void SendErrorMessage(Exception ex)
        {
            Calibrus.ErrorHandler.Alerting alert = new Calibrus.ErrorHandler.Alerting("SparkTPVSummaryCSV");
            alert.SendAlert(ex.Source, ex.Message, Environment.MachineName, Environment.UserName, Environment.Version.ToString());
        }

        #endregion Utilities
    }
}
