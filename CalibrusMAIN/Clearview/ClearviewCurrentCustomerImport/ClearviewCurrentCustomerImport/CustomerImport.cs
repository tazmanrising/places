using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Text.RegularExpressions;
using Calibrus.ErrorHandler;
using Calibrus.Mail;

namespace ClearviewCurrentCustomerImport
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

            string filenameToGrab = string.Empty;

            string mailRecipientFailTO = string.Empty;
            string mailRecipientNoFileTO = string.Empty;

            //Used for sql connection for the bulk inserts
            string ClearviewSqlConn = string.Empty;

            DateTime CurrentDate = new DateTime();
            try
            {
                GetDate(out CurrentDate);

                CsvFileToImportFromPath = ConfigurationManager.AppSettings["CsvFileToImportFromPath"].ToString();
                CsvFileToImportArchivePath = ConfigurationManager.AppSettings["CsvFileToImportArchivePath"].ToString();
                mailRecipientFailTO = ConfigurationManager.AppSettings["mailRecipientFailTO"].ToString();
                mailRecipientNoFileTO = ConfigurationManager.AppSettings["mailRecipientNoFileTO"].ToString();
                ClearviewSqlConn = ConfigurationManager.ConnectionStrings["ClearviewSqlConn"].ToString();

                //BELOW IS NOT NECESSARY SINCE WE ARE NOT LOOKING FOR A SPECIFIC FILE
                //ESuppressionList_yyyyMMdd.csv
                // filenameToGrab = string.Format("ESuppressionList_{0}.csv", string.Format("{0:yyyyMMdd}", CurrentDate));

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
                else
                {
                    //Send alert that there was no file
                    SendEmail(null, DateTime.Now, mailRecipientNoFileTO, ImportStatus.NoFileToImport);
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
                for (int i = 1; i < lines.Length; i++)
                {
                    id++;
                    lines[i] = lines[i].Replace("\"", "");//strip quotation marks
                    var vals = lines[i].Split(',');

                    if (vals.Count() == 9)//IF we have the expected 9 fields of values
                    {
                        //if the zipcode isn't in the standard 5 varchar Index starts at 0 through 4
                        //if the phone isn't in the standard 10 varchar Index starts at 0 through 9
                        //if the Commodity isn't in the standard 50 varchar Index starts at 0 through 49
                        //if ((vals[0].Count() <= 49)
                        //    || (vals[1].Count() <= 99)
                        //    || (vals[2].Count() <= 99)
                        //    || (vals[3].Count() <= 49)
                        //    || (vals[4].Count() == 2)
                        //    || (vals[5].Count() <= 4)
                        //    || (vals[6].Count() != 9)
                        //    || (vals[7].ToString().Trim() != "Gas" || vals[7].ToString().Trim() != "Electric")
                        //    || (vals[8].Count() <= 49))
                        //{
                        string zip = vals[5].ToString().Trim();
                        if (zip.Length == 5)
                        {
                            // do something with the vals because
                            // they are now in a zero-based array
                            CustomerRecord customerRecord = new CustomerRecord();

                            customerRecord.Id = id;//specified id to insert from this loop
                            customerRecord.InsertDateTime = DateTime.Now;

                            customerRecord.AccountNumber = vals[0].ToString().Trim();
                            customerRecord.Address1 = vals[1].ToString().Trim();
                            customerRecord.Address2 = vals[2].ToString().Trim();
                            customerRecord.City = vals[3].ToString().Trim();
                            customerRecord.State = vals[4].ToString().Trim();
                            customerRecord.Zip = vals[5].ToString().Trim();
                            customerRecord.Phone = vals[6].ToString().Trim();
                            customerRecord.Commodity = vals[7].ToString().Trim();
                            customerRecord.Utility = vals[8].ToString().Trim();

                            customerRecordList.Add(customerRecord);
                        }
                        //}
                    }
                    else
                    {
                        //Bad record
                        //id;
                    }


                }
                #endregion Build CustomerRecord Object

                using (ClearviewEntities entities = new ClearviewEntities())
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
                                bulkCopy.Close();
                            }
                            catch (Exception ex)
                            {
                                if (ex.Message.Contains("Received an invalid column length from the bcp client for colid"))
                                {
                                    string pattern = @"\d+";
                                    Match match = Regex.Match(ex.Message.ToString(), pattern);
                                    var index = Convert.ToInt32(match.Value) - 1;

                                    FieldInfo fi = typeof(SqlBulkCopy).GetField("_sortedColumnMappings", BindingFlags.NonPublic | BindingFlags.Instance);
                                    var sortedColumns = fi.GetValue(bulkCopy);
                                    var items = (Object[])sortedColumns.GetType().GetField("_items", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(sortedColumns);

                                    FieldInfo itemdata = items[index].GetType().GetField("_metadata", BindingFlags.NonPublic | BindingFlags.Instance);
                                    var metadata = itemdata.GetValue(items[index]);

                                    var column = metadata.GetType().GetField("column", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).GetValue(metadata);
                                    var length = metadata.GetType().GetField("length", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).GetValue(metadata);
                                    throw new Exception(String.Format("Column: {0} contains data with a length greater than: {1}", column, length));

                                }

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
        /// Strips all non numerics and returns numbers
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string StripAllNonNumerics(string input)
        {
            if (!string.IsNullOrEmpty(input))
            {
                input = Regex.Replace(input, @"[^\d]", "");// strip all non-numeric chars
                return input;
            }
            return string.Empty;
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
                if (!string.IsNullOrEmpty(filename))
                {
                    filename = Path.GetFileName(filename); //strip out path to just get filename
                }

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
                        mail.Subject = "Clearview Current Customer Import CSV File Import - No Files to Import";
                        break;

                    case ImportStatus.Failed:
                        sb.AppendFormat("Failure trying to import file: {0} .", filename);
                        strMsgBody = sb.ToString();
                        mail.Subject = "Clearview Current Customer Import CSV File Import - Failed";
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

            Calibrus.ErrorHandler.Alerting alert = new Calibrus.ErrorHandler.Alerting("ClearviewCurrentCustomerImport");
            alert.SendAlert(ex.Source, sb.ToString(), Environment.MachineName, Environment.UserName, Environment.Version.ToString());
        }
        private static void SendErrorMessage(Exception ex, string filename)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendFormat("ex:{0}, innerEx:{1}", ex.Message, ex.InnerException == null ? "" : ex.InnerException.Message);

            Calibrus.ErrorHandler.Alerting alert = new Calibrus.ErrorHandler.Alerting("ClearviewCurrentCustomerImport");
            alert.SendAlert(ex.Source, String.Format("CurrentFile: {0} -- {1}", filename, sb.ToString()), Environment.MachineName, Environment.UserName, Environment.Version.ToString());
        }

        private static void LogError(Exception ex, string filename)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendFormat("ex:{0}, innerEx:{1}", ex.Message, ex.InnerException == null ? "" : ex.InnerException.Message);

            Calibrus.ErrorHandler.Logging log = new Calibrus.ErrorHandler.Logging();
            log.LogError("ClearviewCurrentCustomerImport", Environment.Version.ToString(), Environment.MachineName, Environment.UserName, ex.Source,
                String.Format("File: {0} -- {1}", filename, sb.ToString()));
        }
        #endregion Error Handling
    }
}
