using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using Calibrus.ErrorHandler;
using System.Diagnostics;
using System.Security;

namespace SparkTrendApparelDailyAgentTxt
{
    public class ReportTxt
    {
        #region Main
        public static void Main(string[] args)
        {
            string rootPath = string.Empty;//File we create this in this program and its location
            string hostName = string.Empty; //Server where we send the file
            string userName = string.Empty; //user account
            string password = string.Empty; //password

            //get report interval          
            DateTime StartDate = new DateTime();
            //DateTime EndDate = new DateTime();

            //start to  build the form pathing
            string csvFilename = string.Empty;
            string csvFilePath = string.Empty;

            if (args.Length > 0)
            {
                if (DateTime.TryParse(args[0], out StartDate))
                {
                    GetDates(out StartDate);//, out EndDate);
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
                GetDates(out StartDate);//, out EndDate);
            }

            //grab values from app.config
            rootPath = ConfigurationManager.AppSettings["rootPath"].ToString();

            hostName = ConfigurationManager.AppSettings["hostName"].ToString();
            userName = ConfigurationManager.AppSettings["userName"].ToString();
            password = ConfigurationManager.AppSettings["password"].ToString();
            try
            {
                #region Trend_Apparel_Agent_Daily_Activity_TPVYYYYMMDD.txt

                string strBuffer = string.Empty;//holds data to put into report for each column

                csvFilePath = rootPath;


                //Trend_Apparel_Agent_Daily_Activity_TPVYYYYMMDD.txt
                csvFilename = "Trend_Apparel_Agent_Daily_Activity_" + StartDate.ToString("yyyyMMdd") + ".txt";
                csvFilePath += csvFilename; //build filepath


                List<spTrendApparelDailyActivityReport_Result> activityList = GetAgentActivity(StartDate);

                //Write Report
                BuildAgentActivityCSV(activityList, csvFilePath, strBuffer);

                //FTP report
                FTPFile(ref rootPath, ref csvFilename, ref csvFilePath, hostName, userName, password);

                #endregion Trend_Apparel_Agent_Daily_Activity_TPVYYYYMMDD.txt

                #region Trend_Apparel_Agent_Daily_Status_ActiveOnly_TPVYYYYMMDD.txt

                strBuffer = string.Empty;

                csvFilePath = rootPath;

                //Trend_Apparel_Agent_Daily_Status_ActiveOnly_TPVYYYYMMDD.txt
                csvFilename = "Trend_Apparel_Agent_Daily_Status_ActiveOnly_" + StartDate.ToString("yyyyMMdd") + ".txt";
                csvFilePath += csvFilename; //build filepath


                List<spTrendApparelDailyAgentStatusActiveOnlyReport_Result> statusActiveOnlyList = GetAgentStatusActiveOnly();

                //Write Report
                BuildAgentStatusActiveOnlyCSV(statusActiveOnlyList, csvFilePath, strBuffer);

                //FTP report
                FTPFile(ref rootPath, ref csvFilename, ref csvFilePath, hostName, userName, password);

                #endregion Trend_Apparel_Agent_Daily_Status_ActiveOnly_TPVYYYYMMDD.txt


                #region Trend_Apparel_Agent_Daily_Status_TPVYYYYMMDD.txt


                csvFilePath = rootPath;

                strBuffer = string.Empty;
                //Trend_Apparel_Agent_Daily_Status_TPVYYYYMMDD.txt
                csvFilename = "Trend_Apparel_Agent_Daily_Status_" + StartDate.ToString("yyyyMMdd") + ".txt";
                csvFilePath += csvFilename; //build filepath

                List<spTrendApparelDailyAgentStatusReport_Result> statusList = GetAgentStatus(StartDate);

                //Write Report
                BuildAgentStatusCSV(statusList, csvFilePath, strBuffer);

                //FTP report
                FTPFile(ref rootPath, ref csvFilename, ref csvFilePath, hostName, userName, password);

                #endregion Trend_Apparel_Agent_Daily_Status_TPVYYYYMMDD.txt


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

        #region CSV (3 methods)
        private static void BuildAgentActivityCSV(List<spTrendApparelDailyActivityReport_Result> records, string csvFilePath, string strBuffer)
        {
            StreamWriter sw = File.CreateText(csvFilePath);

            //Used to hold content for report
            StringBuilder sb = new StringBuilder();


            //Vendor	
            strBuffer = "Vendor";
            sb.AppendFormat("{0}|", strBuffer);

            //Office	
            strBuffer = "Office";
            sb.AppendFormat("{0}|", strBuffer);

            //Spark_Agent_ID
            strBuffer = "Spark_Agent_ID";
            sb.AppendFormat("{0}|", strBuffer);

            //Total_Sale	
            strBuffer = "Total_Sale";
            sb.AppendFormat("{0}|", strBuffer);

            //Hire_Date
            strBuffer = "Hire_Date";
            sb.AppendFormat("{0}", strBuffer);

            strBuffer = string.Empty;
            sb.AppendFormat("{0}", Environment.NewLine);


            //Cursor to mark position when removing trailing control returns
            int recordCursor = 0;
            int recordCount = records.Count();

            if (recordCount != 0)
            {
                foreach (var item in records)
                {
                    recordCursor++;

                    //Vendor	
                    strBuffer = IsValueNull(item.Vendor) ? "" : item.Vendor.Trim();
                    sb.AppendFormat("{0}|", strBuffer);

                    //Office	
                    strBuffer = IsValueNull(item.Office) ? "" : item.Office.Trim();
                    sb.AppendFormat("{0}|", strBuffer);

                    //Spark_Agent_ID
                    strBuffer = IsValueNull(item.Spark_Agent_ID) ? "" : item.Spark_Agent_ID.Trim();
                    sb.AppendFormat("{0}|", strBuffer);

                    //Total_Sale	
                    int? total = null;
                    total = item.Total_Sale;
                    strBuffer = total.HasValue ? item.Total_Sale.ToString() : "";
                    sb.AppendFormat("{0}|", strBuffer);

                    //Hire_Date
                    DateTime? date = null;
                    date = item.Hire_Date;
                    strBuffer = date.HasValue ? item.Hire_Date.ToString() : "";
                    sb.AppendFormat("{0}", strBuffer);

                    strBuffer = string.Empty;
                    if (recordCursor < recordCount)
                    {
                        sb.AppendFormat("{0}", Environment.NewLine);
                    }

                }
            }

            //save file 
            sw.WriteLine(sb.ToString());
            sb.Remove(0, sb.Length);
            sw.Close();
            sw.Dispose();
        }

        private static void BuildAgentStatusActiveOnlyCSV(List<spTrendApparelDailyAgentStatusActiveOnlyReport_Result> records, string csvFilePath, string strBuffer)
        {
            StreamWriter sw = File.CreateText(csvFilePath);

            //Used to hold content for report
            StringBuilder sb = new StringBuilder();



            //Vendor	
            strBuffer = "Vendor_ID";
            sb.AppendFormat("{0}|", strBuffer);

            //Office	
            strBuffer = "Office_Name";
            sb.AppendFormat("{0}|", strBuffer);

            //Spark_Agent_ID
            strBuffer = "Spark_Agent_ID";
            sb.AppendFormat("{0}|", strBuffer);

            //Agent_First_Name	
            strBuffer = "Agent_First_Name";
            sb.AppendFormat("{0}|", strBuffer);

            //Agent_Last_Name
            strBuffer = "Agent_Last_Name";
            sb.AppendFormat("{0}|", strBuffer);

            //Gender
            strBuffer = "Gender";
            sb.AppendFormat("{0}|", strBuffer);

            //Shirt_Size	
            strBuffer = "Shirt_Size";
            sb.AppendFormat("{0}|", strBuffer);

            //Created_Date_Time	
            strBuffer = "Created_Date_Time";
            sb.AppendFormat("{0}|", strBuffer);

            //Status	
            strBuffer = "Status";
            sb.AppendFormat("{0}|", strBuffer);

            //Status_Date_Time	
            strBuffer = "Status_Date_Time";
            sb.AppendFormat("{0}|", strBuffer);

            //Office_Contact	
            strBuffer = "Office_Contact";
            sb.AppendFormat("{0}|", strBuffer);

            //Address1	
            strBuffer = "Address1";
            sb.AppendFormat("{0}|", strBuffer);

            //Address2	
            strBuffer = "Address2";
            sb.AppendFormat("{0}|", strBuffer);

            //City	
            strBuffer = "City";
            sb.AppendFormat("{0}|", strBuffer);

            //State	
            strBuffer = "State";
            sb.AppendFormat("{0}|", strBuffer);

            //Zip	
            strBuffer = "Zip";
            sb.AppendFormat("{0}|", strBuffer);

            //Office_Phone_Number
            strBuffer = "Office_Phone_Number";
            sb.AppendFormat("{0}", strBuffer);


            strBuffer = string.Empty;
            sb.AppendFormat("{0}", Environment.NewLine);


            //Cursor to mark position when removing trailing control returns
            int recordCursor = 0;
            int recordCount = records.Count();

            if (recordCount != 0)
            {
                foreach (var item in records)
                {
                    recordCursor++;

                    //Vendor	
                    strBuffer = IsValueNull(item.VendorNumber) ? "" : item.VendorNumber.Trim();
                    sb.AppendFormat("{0}|", strBuffer);

                    //Office	
                    strBuffer = IsValueNull(item.OfficeName) ? "" : item.OfficeName.Trim();
                    sb.AppendFormat("{0}|", strBuffer);

                    //Spark_Agent_ID
                    strBuffer = IsValueNull(item.Spark_Agent_ID) ? "" : item.Spark_Agent_ID.Trim();
                    sb.AppendFormat("{0}|", strBuffer);

                    //Agent_First_Name	
                    strBuffer = IsValueNull(item.FirstName) ? "" : item.FirstName.ToString();
                    sb.AppendFormat("{0}|", strBuffer);

                    //Agent_Last_Name
                    strBuffer = IsValueNull(item.LastName) ? "" : item.LastName.ToString();
                    sb.AppendFormat("{0}|", strBuffer);

                    //Gender
                    strBuffer = IsValueNull(item.Gender) ? "" : item.Gender.ToString();
                    sb.AppendFormat("{0}|", strBuffer);

                    //Shirt_Size	
                    strBuffer = IsValueNull(item.ShirtSize) ? "" : item.ShirtSize.ToString();
                    sb.AppendFormat("{0}|", strBuffer);

                    //Created_Date_Time	
                    DateTime? createdDate = null;
                    createdDate = item.CreatedDateTime;
                    strBuffer = createdDate.HasValue ? item.CreatedDateTime.ToString() : "";
                    sb.AppendFormat("{0}|", strBuffer);

                    //Status	
                    strBuffer = (item.IsActive == true) ? "Active" : "Inactive"; ;
                    sb.AppendFormat("{0}|", strBuffer);

                    //Status_Date_Time	
                    DateTime? statusDate = null;
                    statusDate = item.StatusDateTime;
                    strBuffer = statusDate.HasValue ? item.StatusDateTime.ToString() : "";
                    sb.AppendFormat("{0}|", strBuffer);


                    //Office_Contact	
                    strBuffer = IsValueNull(item.OfficeContact) ? "" : item.OfficeContact.ToString();
                    sb.AppendFormat("{0}|", strBuffer);

                    //Address1	
                    strBuffer = IsValueNull(item.Address1) ? "" : item.Address1.ToString();
                    sb.AppendFormat("{0}|", strBuffer);

                    //Address2	
                    strBuffer = IsValueNull(item.Address2) ? "" : item.Address2.ToString();
                    sb.AppendFormat("{0}|", strBuffer);

                    //City	
                    strBuffer = IsValueNull(item.City) ? "" : item.City.ToString();
                    sb.AppendFormat("{0}|", strBuffer);

                    //State	
                    strBuffer = IsValueNull(item.StateCode) ? "" : item.StateCode.ToString();
                    sb.AppendFormat("{0}|", strBuffer);

                    //Zip	
                    strBuffer = IsValueNull(item.ZipCode) ? "" : item.ZipCode.ToString();
                    sb.AppendFormat("{0}|", strBuffer);

                    //Office_Phone_Number
                    strBuffer = IsValueNull(item.OfficePhone) ? "" : string.Format("{0:###-###-####}", double.Parse(item.OfficePhone));
                    sb.AppendFormat("{0}", strBuffer);

                    strBuffer = string.Empty;
                    if (recordCursor < recordCount)
                    {
                        sb.AppendFormat("{0}", Environment.NewLine);
                    }

                }
            }

            //save file 
            sw.WriteLine(sb.ToString());
            sb.Remove(0, sb.Length);
            sw.Close();
            sw.Dispose();
        }

        private static void BuildAgentStatusCSV(List<spTrendApparelDailyAgentStatusReport_Result> records, string csvFilePath, string strBuffer)
        {
            StreamWriter sw = File.CreateText(csvFilePath);

            //Used to hold content for report
            StringBuilder sb = new StringBuilder();



            //Vendor	
            strBuffer = "Vendor_ID";
            sb.AppendFormat("{0}|", strBuffer);

            //Office	
            strBuffer = "Office_Name";
            sb.AppendFormat("{0}|", strBuffer);

            //Spark_Agent_ID
            strBuffer = "Spark_Agent_ID";
            sb.AppendFormat("{0}|", strBuffer);

            //Agent_First_Name	
            strBuffer = "Agent_First_Name";
            sb.AppendFormat("{0}|", strBuffer);

            //Agent_Last_Name
            strBuffer = "Agent_Last_Name";
            sb.AppendFormat("{0}|", strBuffer);

            //Gender
            strBuffer = "Gender";
            sb.AppendFormat("{0}|", strBuffer);

            //Shirt_Size	
            strBuffer = "Shirt_Size";
            sb.AppendFormat("{0}|", strBuffer);

            //Created_Date_Time	
            strBuffer = "Created_Date_Time";
            sb.AppendFormat("{0}|", strBuffer);

            //Status	
            strBuffer = "Status";
            sb.AppendFormat("{0}|", strBuffer);

            //Status_Date_Time	
            strBuffer = "Status_Date_Time";
            sb.AppendFormat("{0}|", strBuffer);

            //Office_Contact	
            strBuffer = "Office_Contact";
            sb.AppendFormat("{0}|", strBuffer);

            //Address1	
            strBuffer = "Address1";
            sb.AppendFormat("{0}|", strBuffer);

            //Address2	
            strBuffer = "Address2";
            sb.AppendFormat("{0}|", strBuffer);

            //City	
            strBuffer = "City";
            sb.AppendFormat("{0}|", strBuffer);

            //State	
            strBuffer = "State";
            sb.AppendFormat("{0}|", strBuffer);

            //Zip	
            strBuffer = "Zip";
            sb.AppendFormat("{0}|", strBuffer);

            //Office_Phone_Number
            strBuffer = "Office_Phone_Number";
            sb.AppendFormat("{0}", strBuffer);


            strBuffer = string.Empty;
            sb.AppendFormat("{0}", Environment.NewLine);


            //Cursor to mark position when removing trailing control returns
            int recordCursor = 0;
            int recordCount = records.Count();

            if (recordCount != 0)
            {
                foreach (var item in records)
                {
                    recordCursor++;

                    //Vendor	
                    strBuffer = IsValueNull(item.VendorNumber) ? "" : item.VendorNumber.Trim();
                    sb.AppendFormat("{0}|", strBuffer);

                    //Office	
                    strBuffer = IsValueNull(item.OfficeName) ? "" : item.OfficeName.Trim();
                    sb.AppendFormat("{0}|", strBuffer);

                    //Spark_Agent_ID
                    strBuffer = IsValueNull(item.Spark_Agent_ID) ? "" : item.Spark_Agent_ID.Trim();
                    sb.AppendFormat("{0}|", strBuffer);

                    //Agent_First_Name	
                    strBuffer = IsValueNull(item.FirstName) ? "" : item.FirstName.ToString();
                    sb.AppendFormat("{0}|", strBuffer);

                    //Agent_Last_Name
                    strBuffer = IsValueNull(item.LastName) ? "" : item.LastName.ToString();
                    sb.AppendFormat("{0}|", strBuffer);

                    //Gender
                    strBuffer = IsValueNull(item.Gender) ? "" : item.Gender.ToString();
                    sb.AppendFormat("{0}|", strBuffer);

                    //Shirt_Size	
                    strBuffer = IsValueNull(item.ShirtSize) ? "" : item.ShirtSize.ToString();
                    sb.AppendFormat("{0}|", strBuffer);

                    //Created_Date_Time	
                    DateTime? createdDate = null;
                    createdDate = item.CreatedDateTime;
                    strBuffer = createdDate.HasValue ? item.CreatedDateTime.ToString() : "";                
                    sb.AppendFormat("{0}|", strBuffer);

                    //Status	
                    strBuffer = (item.IsActive == true) ? "Active" : "Inactive"; ;
                    sb.AppendFormat("{0}|", strBuffer);

                    //Status_Date_Time	
                    DateTime? statusDate = null;
                    statusDate = item.StatusDatetime;
                    strBuffer = statusDate.HasValue ? item.StatusDatetime.ToString() : "";
                    sb.AppendFormat("{0}|", strBuffer);


                    //Office_Contact	
                    strBuffer = IsValueNull(item.OfficeContact) ? "" : item.OfficeContact.ToString();
                    sb.AppendFormat("{0}|", strBuffer);

                    //Address1	
                    strBuffer = IsValueNull(item.Address1) ? "" : item.Address1.ToString();
                    sb.AppendFormat("{0}|", strBuffer);

                    //Address2	
                    strBuffer = IsValueNull(item.Address2) ? "" : item.Address2.ToString();
                    sb.AppendFormat("{0}|", strBuffer);

                    //City	
                    strBuffer = IsValueNull(item.City) ? "" : item.City.ToString();
                    sb.AppendFormat("{0}|", strBuffer);

                    //State	
                    strBuffer = IsValueNull(item.StateCode) ? "" : item.StateCode.ToString();
                    sb.AppendFormat("{0}|", strBuffer);

                    //Zip	
                    strBuffer = IsValueNull(item.ZipCode) ? "" : item.ZipCode.ToString();
                    sb.AppendFormat("{0}|", strBuffer);

                    //Office_Phone_Number
                    strBuffer = IsValueNull(item.OfficePhone) ? "" : string.Format("{0:###-###-####}",double.Parse(item.OfficePhone));
                    sb.AppendFormat("{0}", strBuffer);

                    strBuffer = string.Empty;
                    if (recordCursor < recordCount)
                    {
                        sb.AppendFormat("{0}", Environment.NewLine);
                    }

                }
            }

            //save file 
            sw.WriteLine(sb.ToString());
            sb.Remove(0, sb.Length);
            sw.Close();
            sw.Dispose();
        }
        #endregion CSV (2 methods)

        #region Get Data (3 methods)

        /// <summary>
        /// Gets a list of Agent Activty for the day passed in, runTime is deprecated
        /// </summary>
        /// <param name="startdate"></param>
        /// <returns></returns>
        private static List<spTrendApparelDailyActivityReport_Result> GetAgentActivity(DateTime startdate)
        {
            List<spTrendApparelDailyActivityReport_Result> spResult = null;

            try
            {
                using (SparkEntities entities = new SparkEntities())
                {
                    spResult = entities.spTrendApparelDailyActivityReport(reportDate: startdate, runTime: null).ToList();
                }
            }
            catch (Exception ex)
            {
                SendErrorMessage(ex);
                //throw ex;
            }

            return spResult;
        }

        /// <summary>
        /// Gets a list of Agent Status for Active Only
        /// </summary>
        /// 
        /// <returns></returns>
        private static List<spTrendApparelDailyAgentStatusActiveOnlyReport_Result> GetAgentStatusActiveOnly()
        {
            List<spTrendApparelDailyAgentStatusActiveOnlyReport_Result> spResult = null;

            try
            {
                using (SparkEntities entities = new SparkEntities())
                {
                    spResult = entities.spTrendApparelDailyAgentStatusActiveOnlyReport().ToList();
                }
            }
            catch (Exception ex)
            {
                SendErrorMessage(ex);
                //throw ex;
            }

            return spResult;
        }

        /// <summary>
        /// Gets a list of Agent Status for the day passed in
        /// </summary>
        /// <param name="startdate"></param>
        /// <returns></returns>
        private static List<spTrendApparelDailyAgentStatusReport_Result> GetAgentStatus(DateTime startdate)
        {
            List<spTrendApparelDailyAgentStatusReport_Result> spResult = null;

            try
            {
                using (SparkEntities entities = new SparkEntities())
                {
                    spResult = entities.spTrendApparelDailyAgentStatusReport(reportDate: startdate).ToList();
                }
            }
            catch (Exception ex)
            {
                SendErrorMessage(ex);
                //throw ex;
            }

            return spResult;
        }

        #endregion Get Data (2 methods)

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
      
        private static void FTPFile(ref string reportPath, ref string filename, ref string putFilePath, string HostName, string UserName, string Password)
        {

            putFilePath = string.Format(reportPath + filename);
            try
            {
                Calibrus.Ftp.Upload ftp = new Calibrus.Ftp.Upload();
                ftp.Host = new Uri(string.Format("ftp://{0}/", HostName));
                ftp.UserName = UserName;
                ftp.Password = Password;
                ftp.UploadFile(putFilePath, filename);
            }
            catch (Exception ex)
            {
                SendErrorMessage(ex);
            }
        }


        private static void GetDates(out DateTime StartDate)//, out DateTime EndDate)
        {
            DateTime baseDate;
            DateTimeService.ReportingDateTimeService dts = null;
            try
            {
                dts = new DateTimeService.ReportingDateTimeService();
                baseDate = DateTime.Parse(dts.GetDateTime());

                //baseDate = new DateTime(2015, 6, 11); //ad hoc

                //baseDate = new DateTime(2014, 8, 1); //test for the first of the month to see if we get the previous months data
                //baseDate = new DateTime(2014, 8, 2);//test for the second of the month to see if we get the current months data
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

            baseDate = new DateTime(baseDate.Year, baseDate.Month, baseDate.Day, 0, 0, 0);//current date time  format time to default to midnight

            StartDate = new DateTime(baseDate.Year, baseDate.Month, baseDate.Day, 0, 0, 0).AddDays(-1);//Previous day
            /*              
             * EndDate is Not Used due to how the sproc is set up. 
             * It only takes a date and runs for that day, you cannot do a date range until that is rebuilt to do so              
             */
            //EndDate = new DateTime(baseDate.Year, baseDate.Month, baseDate.Day, 0, 0, 0);//current date time as this runs for the previous day
        }
        #endregion Utilities

        #region Error Handling
        private static void SendErrorMessage(Exception ex)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendFormat("ex:{0}, innerEx:{1}", ex.Message, ex.InnerException == null ? "" : ex.InnerException.Message);

            Calibrus.ErrorHandler.Alerting alert = new Calibrus.ErrorHandler.Alerting("SparkTrendApparelDailyAgentTxt");
            alert.SendAlert(ex.Source, sb.ToString(), Environment.MachineName, Environment.UserName, Environment.Version.ToString());
        }

        #endregion Error Handling
    }
}
