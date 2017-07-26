using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Reflection;
using Calibrus.ErrorHandler;
using Calibrus.Mail;
using Renci.SshNet;
using Renci.SshNet.Common;
using Renci.SshNet.Sftp;

namespace ClearviewNexxaLeadsImport
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
            string remoteDirectory = string.Empty;

            string CsvFileToImportFromPath = string.Empty;
            string CsvFileToImportArchivePath = string.Empty;

            string mailRecipientFailTO = string.Empty;

            string filenameToGrab = string.Empty;

            //Used for sql connection for the bulk inserts
            string ClearviewSqlConn = string.Empty;

            DateTime CurrentDate = new DateTime();

            try
            {
                host = ConfigurationManager.AppSettings["host"].ToString();
                username = ConfigurationManager.AppSettings["username"].ToString();
                password = ConfigurationManager.AppSettings["password"].ToString();
                remoteDirectory = ConfigurationManager.AppSettings["remoteDirectory"].ToString();

                CsvFileToImportFromPath = ConfigurationManager.AppSettings["CsvFileToImportFromPath"].ToString();
                CsvFileToImportArchivePath = ConfigurationManager.AppSettings["CsvFileToImportArchivePath"].ToString();

                mailRecipientFailTO = ConfigurationManager.AppSettings["mailRecipientFailTO"].ToString();
                ClearviewSqlConn = ConfigurationManager.ConnectionStrings["ClearviewSqlConn"].ToString();

                GetDate(out CurrentDate);

                //Build Filename
                //CE_Calibrus_yyyyMMdd.csv
                filenameToGrab = string.Format("CE_Calibrus_{0}.csv", CurrentDate.ToString("yyyyMMdd"));

                //Get File from sftp server
                DownloadFile(host, username, password, remoteDirectory, filenameToGrab, CsvFileToImportFromPath);

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
                                ReadFiles(currentCsvFile, CsvFileToImportFromPath, ClearviewSqlConn, mailRecipientFailTO);

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
                    var vals = lines[i].Split(',');

                    if (vals.Count() == 18)//IF we have the expected 18 fields of values
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
                        leadsFileRecord.Phone = vals[4].ToString().Trim();

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

                        //CONNECT_DATE
                        leadsFileRecord.ConnectDate = vals[12].ToString().Trim();                        

                        //DWELL_TYPE
                        leadsFileRecord.DwellType = vals[13].ToString().Trim();                  

                        //CAMPAIGN_CODE
                        leadsFileRecord.CampaignCode = vals[14].ToString().Trim();
                       
                        //LDC_CODE
                        leadsFileRecord.Utility = vals[15].ToString().Trim();
                        
                        //VENDOR_CODE
                        leadsFileRecord.VendorNumber = vals[16].ToString().Trim();

                        //PROCESS_DATE
                        leadsFileRecord.ProcessDate = vals[17].ToString().Trim();

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
        private static void DownloadFile(string host, string username, string password, string remoteDirectory, string RemoteFileName, string putToFilePath)
        {
            string LocalDestinationFilename = string.Empty;
            LocalDestinationFilename = string.Format(@"{0}{1}", putToFilePath, RemoteFileName);
            using (var sftp = new SftpClient(host, 22, username, password))
            {
                sftp.ConnectionInfo.Timeout = TimeSpan.FromSeconds(60);
                sftp.Connect();
                var files = sftp.ListDirectory(remoteDirectory);// get list of files on sftp server

                if (files.Count() > 0) //if we have any files
                {
                    foreach (var file in files)
                    {
                        if (file.Name.ToString() == RemoteFileName)//if the file we are looping through matches the file we are looking for
                            using (var fileToGet = File.OpenWrite(LocalDestinationFilename))
                            {
                                sftp.ChangeDirectory(remoteDirectory);
                                sftp.DownloadFile(RemoteFileName, fileToGet);
                                sftp.DeleteFile(RemoteFileName);
                            }
                    }
                }
                sftp.Disconnect();
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

                    //case ImportStatus.NoFileToImport:
                    //    sb.AppendFormat("There were no files to import today, {0}. ", currentDate.ToString("MMM") + " " + currentDate.ToString("dd") + " " + currentDate.ToString("yyyy"));
                    //    strMsgBody = sb.ToString();
                    //    mail.Subject = "Boss Telecom Zip File Import - No Files to Import";
                    //    break;

                    case ImportStatus.Failed:
                        sb.AppendFormat("Failure trying to import file: {0} .", filename);
                        strMsgBody = sb.ToString();
                        mail.Subject = "Clearview Nexxa Leads CSV File Import - Failed";
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

            Calibrus.ErrorHandler.Alerting alert = new Calibrus.ErrorHandler.Alerting("ClearviewNexxaLeadsImport");
            alert.SendAlert(ex.Source, sb.ToString(), Environment.MachineName, Environment.UserName, Environment.Version.ToString());
        }
        private static void SendErrorMessage(Exception ex, string filename)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendFormat("ex:{0}, innerEx:{1}", ex.Message, ex.InnerException == null ? "" : ex.InnerException.Message);

            Calibrus.ErrorHandler.Alerting alert = new Calibrus.ErrorHandler.Alerting("ClearviewNexxaLeadsImport");
            alert.SendAlert(ex.Source, String.Format("CurrentFile: {0} -- {1}", filename, sb.ToString()), Environment.MachineName, Environment.UserName, Environment.Version.ToString());
        }

        private static void LogError(Exception ex, string filename)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendFormat("ex:{0}, innerEx:{1}", ex.Message, ex.InnerException == null ? "" : ex.InnerException.Message);

            Calibrus.ErrorHandler.Logging log = new Calibrus.ErrorHandler.Logging();
            log.LogError("ClearviewNexxaLeadsImport", Environment.Version.ToString(), Environment.MachineName, Environment.UserName, ex.Source,
                String.Format("File: {0} -- {1}", filename, sb.ToString()));
        }
        #endregion Error Handling
    }
}
