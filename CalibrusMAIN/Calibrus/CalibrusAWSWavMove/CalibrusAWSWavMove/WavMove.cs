using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
//using System.IO;
using System.Linq;
using System.Text;
//using System.Text.RegularExpressions;
//using System.Reflection;
//using System.Threading.Tasks;
using Calibrus.Recordings;




namespace CalibrusAWSWavMove
{
    public class WavMove
    {

        #region Main
        public static void Main(string[] args)
        {
            //get WavFile List interval     
            DateTime StartDate = new DateTime();
            DateTime EndDate = new DateTime();

            string AWSAccessKey = string.Empty;
            string AWSSecretKey = string.Empty;
            string bucketName = string.Empty; //"*** bucket name ***";
            string mailRecipientFailTO = string.Empty;
            string CalibrusSqlConn = string.Empty;


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

            try
            {
                AWSAccessKey = ConfigurationManager.AppSettings["AWSAccessKey"].ToString();
                AWSSecretKey = ConfigurationManager.AppSettings["AWSSecretKey"].ToString();
                bucketName = ConfigurationManager.AppSettings["bucketName"].ToString();
                mailRecipientFailTO = ConfigurationManager.AppSettings["mailRecipientFailTO"].ToString();

                CalibrusSqlConn = ConfigurationManager.ConnectionStrings["CalibrusSqlConn"].ToString();

                //List of AWSWavLogRecords we intend to insert into our db
                List<AWSWavLogRecord> AWSWavLogRecordList = new List<AWSWavLogRecord>();

                #region Clearview
                //Build Record Object
                List<spAWSWaveFileListClearview_Result> clearviewWavFiles = GetClearviewWavFileList(StartDate, EndDate);

                //If we have records to send to AWS S3
                if (clearviewWavFiles.Count > 0)
                {
                    List<AWSWavLogRecord> clearviewList = S3MoveClearview(AWSAccessKey, AWSSecretKey, bucketName, clearviewWavFiles);
                    AWSWavLogRecordList.AddRange(clearviewList);
                }
                #endregion Clearview

                #region Spark

                #endregion Spark

                //bulk insert AWSWavLogRecordList
                InsertAWSWavLogRecord(CalibrusSqlConn, AWSWavLogRecordList);
            }
            catch (Exception ex)
            {
                SendErrorMessage(ex);
                //throw ex;
            }
        }


        #endregion Main

        #region EF Methods

        #region Get Data 
        private static List<spAWSWaveFileListClearview_Result> GetClearviewWavFileList(DateTime startDate, DateTime endDate)
        {
            List<spAWSWaveFileListClearview_Result> wavFiles = new List<spAWSWaveFileListClearview_Result>();
            try
            {
                using (ClearviewEntities entities = new ClearviewEntities())
                {

                    //The using statement should handle the open, close and dispose. So this is probably moot.
                    //entities.Database.CommandTimeout = 180;
                    //entities.Database.Connection.Open();
                    wavFiles = entities.spAWSWaveFileListClearview(startDate: startDate, endDate: endDate).ToList();
                    //entities.Database.Connection.Close();
                    //entities.Database.Connection.Dispose();
                }
            }
            catch (Exception ex)
            {
                SendErrorMessage(ex);
                //throw ex;
            }
            return wavFiles;
        }

        #endregion Get Data

        #region Write Data
        private static void InsertAWSWavLogRecord(string connectionString, List<AWSWavLogRecord> recordList)
        {
            // DateTime now = DateTime.Now;
            try
            {

                #region Insert into dbo.AWSWavLog
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Create a table with rows from the recordList object. 
                    DataTable dtRecordList = GenericToDataTable.ConvertTo<AWSWavLogRecord>(recordList);

                    // Create the SqlBulkCopy object. 
                    // Note that the column positions in the source DataTable 
                    // match the column positions in the destination table so 
                    // there is no need to map columns. 
                    using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection))
                    {
                        bulkCopy.DestinationTableName = "dbo.AWSWavLog";

                        try
                        {

                            bulkCopy.BulkCopyTimeout = 120; //set timeout
                            // Write from the source to the destination.
                            bulkCopy.WriteToServer(dtRecordList);

                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                    }
                }
                #endregion Insert into dbo.AWSWavLog


            }
            catch (Exception ex)
            {
                SendErrorMessage(ex);
            }
        }
        #endregion Write Data

        #endregion EF Methods

        #region Utilities

        private static List<AWSWavLogRecord> S3MoveClearview(string AWSAccessKey, string AWSSecretKey, string bucketName, List<spAWSWaveFileListClearview_Result> clearviewWavFiles)
        {
            List<AWSWavLogRecord> AWSWavLogRecordList = new List<AWSWavLogRecord>();

            foreach (var item in clearviewWavFiles)
            {
                // preparing our file and directory names
                //string fileToBackup = @"d:\mybackupFile.zip"; // test file
                //string bucketName = "mys3bucketname"; //your s3 bucket name goes here
                //string s3DirectoryName = "justdemodirectory";
                //string s3FileName = @"mybackupFile uploaded in 12-9-2014.zip";
                string fileToBackup = string.Empty; // test file   
                string client = "clearview";
                string s3DirectoryName = string.Format(@"{0}/{1:yyyyMMdd}", client, item.CallDateTime);
                string InFile = string.Empty;
                string OutFile = string.Empty;
                string inWavUnc = string.Empty;
                string outWavUnc = string.Empty;

                //need to do a check on the WavName to get UNC or null                         
                inWavUnc = getWavUnc(item.WavName.ToString());
                outWavUnc = getWavUnc(item.OutboundWavName.ToString());

                //if the file has been crunched and is available  
                if (!string.IsNullOrEmpty(inWavUnc))
                {
                    InFile = string.Format("{0}_{1}.wav", item.MainId, "in");//format s3FileName

                    //upload wav file to AWS S3
                    AmazonUploader myUploader = new AmazonUploader();
                    myUploader.sendMyFileToS3(inWavUnc, bucketName, s3DirectoryName, InFile, AWSAccessKey, AWSSecretKey);
                }

                //if the file has been crunched and is available  
                if (!string.IsNullOrEmpty(outWavUnc))
                {
                    //Make sure the Inbound and Outbound wav files are not the same, no need to duplicate outbound files
                    if (inWavUnc != outWavUnc)
                    {
                        OutFile = string.Format("{0}_{1}.wav", item.MainId, "out");//format s3FileName

                        //upload wav file to AWS S3
                        AmazonUploader myUploader = new AmazonUploader();
                        myUploader.sendMyFileToS3(outWavUnc, bucketName, s3DirectoryName, OutFile, AWSAccessKey, AWSSecretKey);
                    }
                }



                //if upload is successful build recordlist to store to Calibrus DB
                //Update Calibrus db with AWSWavLogRecordList
                AWSWavLogRecord awsWavLogRecord = new AWSWavLogRecord();
                awsWavLogRecord.Client = client;
                awsWavLogRecord.MainId = item.MainId;
                awsWavLogRecord.AwsUrlIn = IsValueNull(inWavUnc) ? null : string.Format("https://s3-us-west-2.amazonaws.com/{0}/{1}/{2}", bucketName, s3DirectoryName, InFile);

                if(!IsValueNull(outWavUnc))
                {
                    //Make sure the Inbound and Outbound wav files are not the same, no need to duplicate outbound files
                    if (inWavUnc != outWavUnc)
                    {
                        awsWavLogRecord.AwsUrlOut = string.Format("https://s3-us-west-2.amazonaws.com/{0}/{1}/{2}", bucketName, s3DirectoryName, OutFile);
                    }
                }
                

                AWSWavLogRecordList.Add(awsWavLogRecord);
            }

            return AWSWavLogRecordList;
        }


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

        /// <summary>
        /// Looks up to see if a Wav file exists for Calibrus recordings
        /// </summary>
        /// <param name="WavName">String WavName</param>
        /// <returns>NULL or valid WavUNC </returns>
        private static string getWavUnc(string WavName)
        {
            string wavUnc = string.Empty;
            RecordingLocator att1 = new RecordingLocator(WavName);
            if (att1.RecordingName != null)
            {
                wavUnc = att1.RecordingUnc;
            }
            return wavUnc;
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
            StartDate = new DateTime(baseDate.Year, baseDate.Month, baseDate.Day, 0, 0, 0).AddDays(-1); //previous day  This will be the Start date for the day it runs
            EndDate = new DateTime(baseDate.Year, baseDate.Month, baseDate.Day, 0, 0, 0);  //current day this will be the End date
        }
        #endregion Utilities

        #region Error Handling
        private static void SendErrorMessage(Exception ex)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendFormat("ex:{0}, innerEx:{1}", ex.Message, ex.InnerException == null ? "" : ex.InnerException.Message);

            Calibrus.ErrorHandler.Alerting alert = new Calibrus.ErrorHandler.Alerting("CalibrusAWSWavMove");
            alert.SendAlert(ex.Source, sb.ToString(), Environment.MachineName, Environment.UserName, Environment.Version.ToString());
        }
        private static void SendErrorMessage(Exception ex, string filename)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendFormat("ex:{0}, innerEx:{1}", ex.Message, ex.InnerException == null ? "" : ex.InnerException.Message);

            Calibrus.ErrorHandler.Alerting alert = new Calibrus.ErrorHandler.Alerting("CalibrusAWSWavMove");
            alert.SendAlert(ex.Source, String.Format("WavFile: {0} -- {1}", filename, sb.ToString()), Environment.MachineName, Environment.UserName, Environment.Version.ToString());
        }

        private static void LogError(Exception ex, string filename)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendFormat("ex:{0}, innerEx:{1}", ex.Message, ex.InnerException == null ? "" : ex.InnerException.Message);

            Calibrus.ErrorHandler.Logging log = new Calibrus.ErrorHandler.Logging();
            log.LogError("CalibrusAWSWavMove", Environment.Version.ToString(), Environment.MachineName, Environment.UserName, ex.Source,
                String.Format("WavFile: {0} -- {1}", filename, sb.ToString()));
        }
        #endregion Error Handling
    }
}
