using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Calibrus.Recordings;
using Calibrus.Mail;


namespace CalibrusAWSBulkWavFileMove
{
    public class BulkWavMove
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

                for (DateTime date = StartDate; date.Date <= EndDate.Date; date = date.AddDays(1))//loop through date range day by day
                {
                    DateTime sDate = date;
                    DateTime eDate = date.AddDays(1);
                    //List of AWSWavLogRecords we intend to insert into our db
                    List<AWSWavLogRecord> AWSWavLogRecordList = new List<AWSWavLogRecord>();

                    #region LifeEnergy
                    //Build Record Object
                    List<spAWSWaveFileListLifeEnergy_Result> lifeEnergyWavFiles = GetLifeEnergyWavFileList(sDate, eDate);

                    //If we have records to send to AWS S3
                    if (lifeEnergyWavFiles.Count > 0)
                    {
                        List<WavFileRecord> wavFileRecordList = new List<WavFileRecord>();//Generic WavFile Record List

                        //Populate the Stored Procedure result to the generic WavFileRecord object
                        foreach (spAWSWaveFileListLifeEnergy_Result item in lifeEnergyWavFiles)
                        {
                            WavFileRecord record = new WavFileRecord();
                            record.MainId = item.MainId;
                            record.CallDateTime = item.CallDateTime;
                            record.WavName = item.WavName;
                            record.OutboundWavName = item.OutboundWavName;
                            record.Client = "lifeenergy";

                            wavFileRecordList.Add(record);
                        }
                        //Move the wavFileRecordList to S3
                        List<AWSWavLogRecord> awsWavLogRecordList = S3Move(AWSAccessKey, AWSSecretKey, bucketName, wavFileRecordList);

                        AWSWavLogRecordList.AddRange(awsWavLogRecordList);
                    }
                    #endregion LifeEnergy

                    if (AWSWavLogRecordList.Count > 0)
                    {
                        //bulk insert AWSWavLogRecordList
                        InsertAWSWavLogRecord(CalibrusSqlConn, AWSWavLogRecordList);
                    }
                }
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
        private static List<spAWSWaveFileListLifeEnergy_Result> GetLifeEnergyWavFileList(DateTime startDate, DateTime endDate)
        {
            List<spAWSWaveFileListLifeEnergy_Result> wavFiles = new List<spAWSWaveFileListLifeEnergy_Result>();
            try
            {
                using (LifeEnergyEntities entities = new LifeEnergyEntities())
                {

                    //The using statement should handle the open, close and dispose. So this is probably moot.
                    //entities.Database.CommandTimeout = 180;
                    //entities.Database.Connection.Open();
                    wavFiles = entities.spAWSWaveFileListLifeEnergy(startDate: startDate, endDate: endDate).ToList();
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

        private static List<AWSWavLogRecord> S3Move(string AWSAccessKey, string AWSSecretKey, string bucketName, List<WavFileRecord> WavFiles)
        {
            List<AWSWavLogRecord> AWSWavLogRecordList = new List<AWSWavLogRecord>();

            foreach (var item in WavFiles)
            {
                // preparing our file and directory names
                //string fileToBackup = @"d:\mybackupFile.zip"; // test file
                //string bucketName = "mys3bucketname"; //your s3 bucket name goes here
                //string s3DirectoryName = "justdemodirectory";
                //string s3FileName = @"mybackupFile uploaded in 12-9-2014.zip";
                string fileToBackup = string.Empty; // test file                              
                string s3DirectoryName = string.Format(@"{0}/{1:yyyyMMdd}", item.Client, item.CallDateTime);
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
                    OutFile = string.Format("{0}_{1}.wav", item.MainId, "out");//format s3FileName

                    //upload wav file to AWS S3
                    AmazonUploader myUploader = new AmazonUploader();
                    myUploader.sendMyFileToS3(outWavUnc, bucketName, s3DirectoryName, OutFile, AWSAccessKey, AWSSecretKey);
                }

                //if upload is successful build recordlist to store to Calibrus DB
                //Update Calibrus db with AWSWavLogRecordList
                AWSWavLogRecord awsWavLogRecord = new AWSWavLogRecord();
                awsWavLogRecord.Client = item.Client;
                awsWavLogRecord.MainId = item.MainId;
                awsWavLogRecord.AwsUrlIn = IsValueNull(inWavUnc) ? null : string.Format("https://s3-us-west-2.amazonaws.com/{0}/{1}/{2}", bucketName, s3DirectoryName, InFile);
                awsWavLogRecord.AwsUrlOut = IsValueNull(outWavUnc) ? null : string.Format("https://s3-us-west-2.amazonaws.com/{0}/{1}/{2}", bucketName, s3DirectoryName, OutFile);

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
            //StartDate = new DateTime(baseDate.Year, baseDate.Month, baseDate.Day, 0, 0, 0); //previous day  This will be the Start date for the day it runs
            //EndDate = new DateTime(baseDate.Year, baseDate.Month, baseDate.Day, 0, 0, 0);  //current day this will be the End date
            StartDate = new DateTime(2016, 7, 1, 0, 0, 0); //previous day  This will be the Start date for the day it runs
            EndDate = new DateTime(2017, 3, 1, 0, 0, 0);  //current day this will be the End date
        }
        #endregion Utilities

        #region Error Handling
        private static void SendErrorMessage(Exception ex)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendFormat("ex:{0}, innerEx:{1}", ex.Message, ex.InnerException == null ? "" : ex.InnerException.Message);

            Calibrus.ErrorHandler.Alerting alert = new Calibrus.ErrorHandler.Alerting("CalibrusAWSBulkWavFileMove");
            alert.SendAlert(ex.Source, sb.ToString(), Environment.MachineName, Environment.UserName, Environment.Version.ToString());
        }
        private static void SendErrorMessage(Exception ex, string filename)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendFormat("ex:{0}, innerEx:{1}", ex.Message, ex.InnerException == null ? "" : ex.InnerException.Message);

            Calibrus.ErrorHandler.Alerting alert = new Calibrus.ErrorHandler.Alerting("CalibrusAWSBulkWavFileMove");
            alert.SendAlert(ex.Source, String.Format("WavFile: {0} -- {1}", filename, sb.ToString()), Environment.MachineName, Environment.UserName, Environment.Version.ToString());
        }

        private static void LogError(Exception ex, string filename)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendFormat("ex:{0}, innerEx:{1}", ex.Message, ex.InnerException == null ? "" : ex.InnerException.Message);

            Calibrus.ErrorHandler.Logging log = new Calibrus.ErrorHandler.Logging();
            log.LogError("CalibrusAWSBulkWavFileMove", Environment.Version.ToString(), Environment.MachineName, Environment.UserName, ex.Source,
                String.Format("WavFile: {0} -- {1}", filename, sb.ToString()));
        }
        #endregion Error Handling
    }
}
