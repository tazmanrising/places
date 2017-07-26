using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Data.Entity;
using System.Data.Entity.Spatial;
using Microsoft.SqlServer.Types;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Reflection;
using Calibrus.ErrorHandler;
using Calibrus.Mail;
using Calibrus.Ftp;

namespace SparkNexxaLeadsImport
{
    public class FileImport
    {
        public enum ImportStatus
        {
            Success,
            NoFileToImport,
            Failed
        }

        #region Main
        public static void Main(string[] args)
        {
            string host = string.Empty;
            string username = string.Empty;
            string password = string.Empty;

            string CsvFileToImportFromPath = string.Empty;
            string CsvFileToImportArchivePath = string.Empty;

            string mailRecipientFailTO = string.Empty;
            string mailRecipientNoFileTO = string.Empty;

            string filenameToGrab = string.Empty;

            //Used for sql connection for the bulk inserts
            string SparkSqlConn = string.Empty;

            DateTime CurrentDate = new DateTime();

            try
            {
                host = ConfigurationManager.AppSettings["host"].ToString();
                username = ConfigurationManager.AppSettings["username"].ToString();
                password = ConfigurationManager.AppSettings["password"].ToString();

                CsvFileToImportFromPath = ConfigurationManager.AppSettings["CsvFileToImportFromPath"].ToString();
                CsvFileToImportArchivePath = ConfigurationManager.AppSettings["CsvFileToImportArchivePath"].ToString();

                mailRecipientFailTO = ConfigurationManager.AppSettings["mailRecipientFailTO"].ToString();
                mailRecipientNoFileTO = ConfigurationManager.AppSettings["mailRecipientNoFileTO"].ToString();

                SparkSqlConn = ConfigurationManager.ConnectionStrings["SparkSqlConn"].ToString();

                GetDate(out CurrentDate);

                //Build Filename
                //SE_Calibrus_yyyyMMdd.csv
                filenameToGrab = string.Format("SE_Calibrus_{0}.csv", CurrentDate.ToString("yyyy-MM-dd"));

                //Get File from sftp server
                DownloadFile(host, username, password, filenameToGrab, CsvFileToImportFromPath, mailRecipientFailTO, mailRecipientNoFileTO);

                //Look for files to process
                var ListImportFileNames = Directory.EnumerateFiles(CsvFileToImportFromPath, "*.*", System.IO.SearchOption.TopDirectoryOnly).Where(s => s.EndsWith(".csv"));
                if (ListImportFileNames.Count() > 0)
                {
                    foreach (var currentCsvFile in ListImportFileNames)
                    {
                        if (new FileInfo(currentCsvFile).Length != 0)
                        {
                            bool isError = false;

                            try
                            {
                                //Read Files to insert
                                ReadFiles(currentCsvFile, CsvFileToImportFromPath, SparkSqlConn, mailRecipientFailTO);

                                //Move Imported CSV to Archive
                                MoveFile(CsvFileToImportArchivePath, currentCsvFile);
                            }
                            catch (Exception ex)
                            {
                                LogError(ex, currentCsvFile);
                                isError = true;
                                continue;
                            }
                            finally
                            {
                                if (isError)// error send email import failed
                                {
                                    SendEmail(currentCsvFile, DateTime.Now, mailRecipientFailTO, ImportStatus.Failed);
                                }
                                //else//if no error send email import success
                                //{
                                //    //SendEmail(currentFile, DateTime.Now, mailRecipientTO, ImportStatus.Success);
                                //}
                            }
                        }
                        else
                        {
                            //Delete the file from the TempHold directory
                            DeleteFile(currentCsvFile);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                SendErrorMessage(ex);
            }

        }
        #endregion Main

        #region DataInserts

        private static void InsertCurrentLeadsFile(string currentFile, string connectionString)
        {
            DateTime now = DateTime.Now;
            try
            {
                #region Build CustomerRecord Object
                List<LeadsRecord> leadsFileRecordList = new List<LeadsRecord>();

                // read the entire file and store each line
                // as a new element in a string[]
                var lines = File.ReadAllLines(currentFile);

                // we can skip the first line because it's
                // just headings - if you need the headings
                // just grab them off the 0 index
                for (int i = 1; i < lines.Length; i++)
                {
                    // lines[i] = lines[i].Replace("\"", ""); //remove double quotes
                    var vals = lines[i].Split(',');

                    if (vals.Count() == 42)//IF we have the expected 42 fields of values
                    {
                        LeadsRecord leadsFileRecord = new LeadsRecord();

                        //leadsFileRecord.LeadsId = 0; //place holder for the identity field of the record
                        //leadsFileRecord.LoadDateTime = now;

                        //RECORD_LOCATOR 
                        leadsFileRecord.RecordLocator = StripAllNonNumerics(vals[0].ToString());

                        //FIRST_NAME 
                        leadsFileRecord.FirstName = vals[1].ToString().Trim();

                        //MIDDLE_NAME 
                        leadsFileRecord.MiddleName = vals[2].ToString().Trim();

                        //LAST_NAME 
                        leadsFileRecord.LastName = vals[3].ToString().Trim();

                        //PHONE
                        leadsFileRecord.Phone = StripAllNonNumerics(vals[4].ToString().Trim());

                        //ADDRESS_1
                        leadsFileRecord.Address = vals[5].ToString().Trim();

                        //ADDRESS_2
                        leadsFileRecord.Address2 = vals[6].ToString().Trim();

                        //CITY
                        leadsFileRecord.City = vals[7].ToString().Trim();

                        //COUNTY
                        leadsFileRecord.County = vals[8].ToString().Trim();

                        //STATE
                        leadsFileRecord.State = vals[9].ToString().Trim();

                        //ZIP
                        leadsFileRecord.Zip = vals[10].ToString().Trim();

                        //ZIP4
                        leadsFileRecord.Zip4 = vals[11].ToString().Trim();

                        //ZIP+4
                        leadsFileRecord.ZipPlus4 = vals[12].ToString().Trim();

                        //CONNECT_DATE
                        leadsFileRecord.ConnectDate = vals[13].ToString().Trim();

                        //UTILITY_ZONE                        
                        leadsFileRecord.UtilityZone = vals[14].ToString().Trim();

                        //HISPANIC_FLAG
                        leadsFileRecord.HispanicFlag = vals[15].ToString().Trim();

                        //HISPANIC_LANGPREF
                        leadsFileRecord.HispanicLangPref = vals[16].ToString().Trim();

                        //HOME_SQ_FT
                        leadsFileRecord.HomeSqFt = vals[17].ToString().Trim();

                        //DWELL_TYPE
                        leadsFileRecord.DwellType = vals[18].ToString().Trim();

                        //COMPANY_NAME                        
                        leadsFileRecord.CompanyName = vals[19].ToString().Trim();

                        //CONTACT_TITLE
                        leadsFileRecord.ContactTitle = vals[20].ToString().Trim();

                        //HOME_YR_BUILT
                        leadsFileRecord.HomeYrBuilt = vals[21].ToString().Trim();

                        //BUILDING_SQ_FT
                        leadsFileRecord.BuildingSqFt = vals[22].ToString().Trim();

                        //HISPANIC_ACULTURATION
                        leadsFileRecord.HispanicAculturation = vals[23].ToString().Trim();

                        //USAGE_THRESHOLD
                        leadsFileRecord.UsageThreshold = vals[24].ToString().Trim();

                        //INDIVIDUAL_CREDIT_SCORE
                        leadsFileRecord.IndividualCreditScore = vals[25].ToString().Trim();

                        //SIC_CODE
                        leadsFileRecord.SicCode = vals[26].ToString().Trim();

                        //EMPLOYEE_SIZE
                        leadsFileRecord.EmployeeSize = vals[27].ToString().Trim();

                        //CREDIT_RATING
                        leadsFileRecord.CreditRating = vals[28].ToString().Trim();

                        //YR_START_DATE
                        leadsFileRecord.YrStartDate = vals[29].ToString().Trim();

                        //SIC_DESC
                        leadsFileRecord.SicDesc = vals[30].ToString().Trim();

                        //CAMPAIGN_CODE
                        leadsFileRecord.CampaignCode = vals[31].ToString().Trim();

                        //RECORD_TYPE
                        leadsFileRecord.RecordType = vals[32].ToString().Trim();

                        //LDC_CODE
                        leadsFileRecord.Utility = vals[33].ToString().Trim();

                        //VENDOR                        
                        leadsFileRecord.Vendor = vals[34].ToString().Trim();

                        //VENDOR_CODE
                        leadsFileRecord.VendorNumber = vals[35].ToString().Trim();

                        //PROCESS_DATE
                        leadsFileRecord.ProcessDate = vals[36].ToString().Trim();

                        //ESIID	
                        leadsFileRecord.ESIID = vals[37].ToString().Trim();

                        //CARRIER_ROUTE	
                        leadsFileRecord.CarrierRoute = vals[38].ToString().Trim();

                        //SEQUENCE_NUMBER	
                        leadsFileRecord.SequenceNumber = StripAllNonNumerics(vals[39].ToString().Trim());

                        //LAT	
                        leadsFileRecord.Lat = IsValueNull(vals[40].ToString()) ? null : vals[40].ToString().Trim();

                        //LONG
                        leadsFileRecord.Long = IsValueNull(vals[41].ToString()) ? null : vals[41].ToString().Trim();

                        Microsoft.SqlServer.Types.SqlGeometry geolocation = null;
                        if (!IsValueNull(vals[40].ToString()) && !IsValueNull(vals[41].ToString()))
                        {
                          var pointString = string.Format("POINT({0} {1})", vals[40].ToString().Trim(), vals[41].ToString().Trim());
                          System.Data.SqlTypes.SqlChars sqlPointString = new System.Data.SqlTypes.SqlChars(pointString.ToCharArray());

                          geolocation = SqlGeometry.STGeomFromText(sqlPointString, 4326);
                        }
                        //Geolocation
                        leadsFileRecord.Geolocation = geolocation;


                        leadsFileRecordList.Add(leadsFileRecord);

                    }

                }
                #endregion Build CustomerRecord Object

                #region Insert into v1.Leads
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Create a table with rows from the leadsFileRecordList object. 
                    DataTable currentLoadFiles = GenericToDataTable.ConvertTo<LeadsRecord>(leadsFileRecordList);

                    // Create the SqlBulkCopy object. 
                    // Note that the column positions in the source DataTable 
                    // match the column positions in the destination table so 
                    // there is no need to map columns. 
                    using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection))
                    {
                        bulkCopy.DestinationTableName = "v1.Leads";

                        try
                        {
                            bulkCopy.BulkCopyTimeout = 120; //set timeout
                            // Write from the source to the destination.
                            bulkCopy.WriteToServer(currentLoadFiles);

                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                    }
                }
                #endregion Insert into v1.Leads


            }
            catch (Exception ex)
            {
                SendErrorMessage(ex, currentFile);
            }
        }
        #endregion DataInserts

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



        /// <summary>
        /// Reads all files in targeted path to determine which insert method to use
        /// </summary>
        /// <param name="FilesToReadPath"></param>
        public static void ReadFiles(string currentCsvFile, string FilesToReadPath, string sqlConn, string mailBadRecordListTo)
        {
            //var csvfileName = Path.GetFileName(currentCsvFile);//get the name of the current csv File we want to import from
            var ListFileNamesToRead = Directory.EnumerateFiles(FilesToReadPath, "*.*", System.IO.SearchOption.TopDirectoryOnly).Where(s => s.EndsWith(".csv"));
            try
            {
                if (ListFileNamesToRead.Count() > 0)
                {
                    foreach (var currentFile in ListFileNamesToRead)
                    {
                        var extension = Path.GetExtension(currentFile).ToLower();

                        if (extension == ".csv")
                        {
                            //This is a csv file to import                           
                            InsertCurrentLeadsFile(currentFile, sqlConn);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Uses Renci.sshnet to download file to a local path using sftp
        /// </summary>
        /// <param name="host"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="remoteDirectory"></param>
        /// <param name="RemoteFileName"></param>
        /// <param name="putToFilePath"></param>
        private static void DownloadFile(string host, string username, string password, string RemoteFileName, string putToFilePath, string mailRecipientFailTO, string mailRecipientNoFileTO)
        {
            //RemoteFileName = "Texas Pre-Screen Customer List_Appended_OUT_Revised.csv";
            string LocalDestinationFilename = string.Empty;
            LocalDestinationFilename = string.Format(@"{0}{1}", putToFilePath, RemoteFileName);


            LocalDestinationFilename = string.Format(putToFilePath + RemoteFileName);
            try
            {
                Calibrus.Ftp.Download ftp = new Calibrus.Ftp.Download();
                ftp.Host = new Uri(string.Format("ftp://{0}/", host));
                ftp.UserName = username;
                ftp.Password = password;
                //ftp.Timeout = 120;
                ftp.DownloadFile(LocalDestinationFilename, RemoteFileName);

            }
            catch (WebException wex)
            {
                //550 file not there       
                if (wex.Status == WebExceptionStatus.ProtocolError && ((FtpWebResponse)wex.Response).StatusCode == FtpStatusCode.ActionNotTakenFileUnavailable)
                {
                    LogError(wex, RemoteFileName);
                    SendEmail(RemoteFileName, DateTime.Now, mailRecipientNoFileTO, ImportStatus.NoFileToImport);
                }
            }
            catch (Exception ex)
            {
                SendErrorMessage(ex, RemoteFileName);
                SendEmail(RemoteFileName, DateTime.Now, mailRecipientFailTO, ImportStatus.Failed);
            }

        }

        /// <summary>
        /// Moves the Import file to the Archive folder
        /// </summary>
        /// <param name="MoveToFilePath"></param>
        /// <param name="currentFile"></param>
        private static void MoveFile(string MoveToFilePath, string currentFile)
        {
            try
            {
                //Move the original file to the Archive
                //build archive path for the file
                MoveToFilePath += Path.GetFileName(currentFile);
                bool oldFileExists = File.Exists(MoveToFilePath);

                //If the file exists in the Archive folder
                if (oldFileExists)
                {
                    //Delete the file
                    File.Delete(MoveToFilePath);
                }
                //Move it to the archive folder
                File.Move(currentFile, MoveToFilePath);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Deletes a specific file
        /// </summary>
        /// <param name="CurrentCSVFile"></param>
        private static void DeleteFile(string FileToDelete)
        {
            try
            {
                //Delete the file
                File.Delete(FileToDelete);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// Deletes all files in targeted path
        /// </summary>
        /// <param name="FileToImportPath"></param>
        private static void DeleteFiles(string FilesToDeletePath)
        {
            var ListFileNamesToDelete = Directory.EnumerateFiles(FilesToDeletePath, "*.*", System.IO.SearchOption.TopDirectoryOnly).Where(s => s.EndsWith(".txt") || s.EndsWith(".csv"));
            try
            {
                if (ListFileNamesToDelete.Count() > 0)
                    //Loop Through files to delete
                    foreach (var currentFile in ListFileNamesToDelete)
                    {
                        string FileToDelete = currentFile;

                        //Delete the file
                        File.Delete(FileToDelete);
                    }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Sends an email out to specific distro with name of file and status of the import: No File, Successful Import, or Failed Import
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="currentDate"></param>
        /// <param name="strToEmail"></param>
        /// <param name="importstatus"></param>
        private static void SendEmail(string filename, DateTime currentDate, String strToEmail, ImportStatus importstatus)
        {
            string strMsgBody = string.Empty;
            try
            {
                filename = Path.GetFileName(filename); //strip out path to just get filename

                StringBuilder sb = new StringBuilder();
                SmtpMail mail = new SmtpMail("TMPWEB1", false);

                switch (importstatus)
                {
                    //case ImportStatus.Success:
                    //    sb.AppendFormat("We received file {0} and imported it with no issues.  ", filename);
                    //    strMsgBody = sb.ToString();
                    //    mail.Subject = "Boss Telecom Zip File Import - Success";
                    //    break;

                    case ImportStatus.NoFileToImport:
                        sb.AppendFormat("There were no files to import today, {0}. ", currentDate.ToString("MMM") + " " + currentDate.ToString("dd") + " " + currentDate.ToString("yyyy"));
                        strMsgBody = sb.ToString();
                        mail.Subject = "Spark Nexxa Leads CSV File Import  - No Files to Import";
                        break;

                    case ImportStatus.Failed:
                        sb.AppendFormat("Failure trying to import file: {0} .", filename);
                        strMsgBody = sb.ToString();
                        mail.Subject = "Spark Nexxa Leads CSV File Import - Failed";
                        break;
                }

                //mail.AddAttachment(xlsFilePath);//Attach XLS report
                mail.AddRecipient(strToEmail, RecipientType.To);
                mail.From = "reports1@calibrus.com";
                mail.Body = strMsgBody;
                mail.SendMessage();
            }
            catch (Exception ex)
            {
                SendErrorMessage(ex);
            }
        }

        public static string StripAllNonNumerics(string input)
        {
            if (!string.IsNullOrEmpty(input))
            {
                input = Regex.Replace(input, @"[^\d]", "");// strip all non-numeric chars
                return input;
            }
            return string.Empty;
        }

        private static void GetDate(out DateTime CurrentDate)
        {

            DateTime baseDate;
            DateTimeService.ReportingDateTimeService dts = null;
            try
            {
                dts = new DateTimeService.ReportingDateTimeService();
                baseDate = DateTime.Parse(dts.GetDateTime());


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

            baseDate = new DateTime(baseDate.Year, baseDate.Month, baseDate.Day, 0, 0, 0);//current date time    
            CurrentDate = new DateTime(baseDate.Year, baseDate.Month, baseDate.Day, 0, 0, 0);

        }
        #endregion Utilities

        #region Error Handling
        private static void SendErrorMessage(Exception ex)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendFormat("ex:{0}, innerEx:{1}", ex.Message, ex.InnerException == null ? "" : ex.InnerException.Message);

            Calibrus.ErrorHandler.Alerting alert = new Calibrus.ErrorHandler.Alerting("SparkNexxaLeadsImport");
            alert.SendAlert(ex.Source, sb.ToString(), Environment.MachineName, Environment.UserName, Environment.Version.ToString());
        }
        private static void SendErrorMessage(Exception ex, string filename)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendFormat("ex:{0}, innerEx:{1}", ex.Message, ex.InnerException == null ? "" : ex.InnerException.Message);

            Calibrus.ErrorHandler.Alerting alert = new Calibrus.ErrorHandler.Alerting("SparkNexxaLeadsImport");
            alert.SendAlert(ex.Source, String.Format("CurrentFile: {0} -- {1}", filename, sb.ToString()), Environment.MachineName, Environment.UserName, Environment.Version.ToString());
        }

        private static void LogError(Exception ex, string filename)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendFormat("ex:{0}, innerEx:{1}", ex.Message, ex.InnerException == null ? "" : ex.InnerException.Message);

            Calibrus.ErrorHandler.Logging log = new Calibrus.ErrorHandler.Logging();
            log.LogError("SparkNexxaLeadsImport", Environment.Version.ToString(), Environment.MachineName, Environment.UserName, ex.Source,
                String.Format("File: {0} -- {1}", filename, sb.ToString()));
        }
        #endregion Error Handling
    }
}
