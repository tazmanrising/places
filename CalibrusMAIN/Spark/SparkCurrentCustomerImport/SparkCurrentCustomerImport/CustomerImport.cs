using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Calibrus.ErrorHandler;
using Calibrus.Ftp;
using Calibrus.Mail;

namespace SparkCurrentCustomerImport
{
    public class CustomerImport
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
            string CsvFileToImportFromPath = string.Empty;
            string CsvFileToImportArchivePath = string.Empty;
            string hostName = string.Empty;
            string userName = string.Empty;
            string password = string.Empty;

            string filenameToGrab = string.Empty;

            string mailRecipientFailTO = string.Empty;

            //Used for sql connection for the bulk inserts
            string SparkSqlConn = string.Empty;

            DateTime CurrentDate = new DateTime();

            try
            {
                GetDate(out CurrentDate);

                CsvFileToImportFromPath = ConfigurationManager.AppSettings["CsvFileToImportFromPath"].ToString();
                CsvFileToImportArchivePath = ConfigurationManager.AppSettings["CsvFileToImportArchivePath"].ToString();
                hostName = ConfigurationManager.AppSettings["hostName"].ToString();
                userName = ConfigurationManager.AppSettings["userName"].ToString();
                password = ConfigurationManager.AppSettings["password"].ToString();
                mailRecipientFailTO = ConfigurationManager.AppSettings["mailRecipientFailTO"].ToString();
                SparkSqlConn = ConfigurationManager.ConnectionStrings["SparkSqlConn"].ToString();

                //SparkBrands_EBR_MMddyyyy.csv
                filenameToGrab = string.Format("Sparkbrands_EBR_{0}.csv", string.Format("{0:MMddyyyy}", CurrentDate));
                //filenameToGrab = string.Format("SparkBrands_EBR_03112016.csv");
                //Grab the file from the FTP site
                GetFTPFile(filenameToGrab, CsvFileToImportFromPath, hostName, userName, password);

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
        
        private static void InsertCurrentCustomer(string currentFile, string connectionString)
        {
            int id = 0;
            try
            {

                #region Build CustomerRecord Object
                List<CustomerRecord> customerRecordList = new List<CustomerRecord>();

                // read the entire file and store each line
                // as a new element in a string[]
                var lines = File.ReadAllLines(currentFile);

                // we can skip the first line because it's
                // just headings - if you need the headings
                // just grab them off the 0 index
                for (int i = 0; i < lines.Length; i++)
                {
                    id++;
                    var vals = lines[i].Split(',');

                    if (vals.Count() == 4)//IF we have the expected 4 fields of values
                    {
                        // do something with the vals because
                        // they are now in a zero-based array
                        CustomerRecord customerRecord = new CustomerRecord();

                        customerRecord.Id = id;//specified id to insert from this loop

                        customerRecord.Utility = vals[0].ToString().Trim();
                        customerRecord.Commodity = vals[1].ToString().Trim();
                        customerRecord.AccountNumber = vals[2].ToString().Trim();
                        customerRecord.Phone = vals[3].ToString().Trim();

                        customerRecordList.Add(customerRecord);
                    }

                }
                #endregion Build CustomerRecord Object

                using (SparkEntities entities = new SparkEntities())
                {
                    entities.CommandTimeout = 10000000;

                    #region Delete all records from v1.CurrentCustomer

                    entities.ExecuteStoreCommand("TRUNCATE TABLE v1.CurrentCustomer");
                    entities.SaveChanges();
                    #endregion Delete all records from v1.CurrentCustomer


                    #region Insert into v1.CurrentCustomer
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();


                        // Create a table with some rows. 
                        DataTable currentCustomers = GenericToDataTable.ConvertTo<CustomerRecord>(customerRecordList);

                        // Create the SqlBulkCopy object. 
                        // Note that the column positions in the source DataTable 
                        // match the column positions in the destination table so 
                        // there is no need to map columns. 
                        using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection))
                        {
                            bulkCopy.DestinationTableName = "v1.CurrentCustomer";

                            try
                            {
                                bulkCopy.BulkCopyTimeout = 120; //set timeout
                                // Write from the source to the destination.
                                bulkCopy.WriteToServer(currentCustomers);
                            }
                            catch (Exception ex)
                            {
                                throw ex;
                            }
                        }


                    }
                    #endregion Insert into v1.CurrentCustomer
                }

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
                            InsertCurrentCustomer(currentFile, sqlConn);
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
        /// Gets a Specific FTP File from the FTP server we connect to and moves it to a specified directory
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="filePath"></param>
        /// <param name="HostName"></param>
        /// <param name="UserName"></param>
        /// <param name="Password"></param>
        private static void GetFTPFile(string filename, string filePath, string HostName, string UserName, string Password)
        {
            filePath = string.Format(filePath + filename);
            try
            {
                Calibrus.Ftp.Download ftp = new Calibrus.Ftp.Download();
                ftp.Host = new Uri(string.Format("ftp://{0}/", HostName));
                ftp.UserName = UserName;
                ftp.Password = Password;
                ftp.DownloadFile(filePath, filename);

            }
            catch (Exception ex)
            {
                SendErrorMessage(ex, filename);
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
                        mail.Subject = "Spark Current Customer Import CSV File Import - Failed";
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

            Calibrus.ErrorHandler.Alerting alert = new Calibrus.ErrorHandler.Alerting("SparkCurrentCustomerImport");
            alert.SendAlert(ex.Source, sb.ToString(), Environment.MachineName, Environment.UserName, Environment.Version.ToString());
        }
        private static void SendErrorMessage(Exception ex, string filename)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendFormat("ex:{0}, innerEx:{1}", ex.Message, ex.InnerException == null ? "" : ex.InnerException.Message);

            Calibrus.ErrorHandler.Alerting alert = new Calibrus.ErrorHandler.Alerting("SparkCurrentCustomerImport");
            alert.SendAlert(ex.Source, String.Format("CurrentFile: {0} -- {1}", filename, sb.ToString()), Environment.MachineName, Environment.UserName, Environment.Version.ToString());
        }

        private static void LogError(Exception ex, string filename)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendFormat("ex:{0}, innerEx:{1}", ex.Message, ex.InnerException == null ? "" : ex.InnerException.Message);

            Calibrus.ErrorHandler.Logging log = new Calibrus.ErrorHandler.Logging();
            log.LogError("SparkCurrentCustomerImport", Environment.Version.ToString(), Environment.MachineName, Environment.UserName, ex.Source,
                String.Format("File: {0} -- {1}", filename, sb.ToString()));
        }
        #endregion Error Handling
    }
}
