using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;

using System.Reflection;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Calibrus.ErrorHandler;
using Calibrus.Mail;

namespace FrontierE911LaodFileInsert
{

    public class LoadFileInsert
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

            string mailRecipientFailTO = string.Empty;

            string filenameToGrab = string.Empty;

            //Used for sql connection for the bulk inserts
            string FrontierSqlConn = string.Empty;



            DateTime CurrentDate = new DateTime();
            try
            {
                GetDate(out CurrentDate);

                CsvFileToImportFromPath = ConfigurationManager.AppSettings["CsvFileToImportFromPath"].ToString();
                CsvFileToImportArchivePath = ConfigurationManager.AppSettings["CsvFileToImportArchivePath"].ToString();

                mailRecipientFailTO = ConfigurationManager.AppSettings["mailRecipientFailTO"].ToString();
                FrontierSqlConn = ConfigurationManager.ConnectionStrings["FrontierSqlConn"].ToString();

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
                                ReadFiles(currentCsvFile, CsvFileToImportFromPath, FrontierSqlConn, mailRecipientFailTO);

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

        private static void InsertCurrentLoadFileTotblE911LoadFileTempHold(string currentFile, string connectionString)
        {

            try
            {

                #region Build CustomerRecord Object
                List<LoadFileRecord> loadFileRecordList = new List<LoadFileRecord>();

                // read the entire file and store each line
                // as a new element in a string[]
                var lines = File.ReadAllLines(currentFile);

                // we can skip the first line because it's
                // just headings - if you need the headings
                // just grab them off the 0 index
                for (int i = 1; i < lines.Length; i++)
                {
                    var vals = lines[i].Split(',');

                    if (vals.Count() == 14)//IF we have the expected 14 fields of values
                    {
                        //-Filter records:
                        //1) General date after April 1st <-- deprecated
                        //2) isVoip = ‘Y’
                        //3) E911 Action = ‘N’ or ‘No Record
                        //DateTime dtCheck = new DateTime(2016, 4, 1, 0, 0, 0); <-- deprecated

                        if ((vals[10].ToString().Trim() == "Y")
                            && (vals[7].ToString().Trim() == "N" || vals[7].ToString().Trim() == "No Record"))
                        //&& (DateTime.Parse(vals[6].ToString().Trim()).Date >= dtCheck.Date)) <-- deprecated
                        {

                            LoadFileRecord loadFileRecord = new LoadFileRecord();

                            loadFileRecord.SubscriberID = vals[0].ToString().Trim();
                            loadFileRecord.Name = vals[1].ToString().Trim();
                            loadFileRecord.Signature = vals[2].ToString().Trim();
                            loadFileRecord.BirthYear = string.Empty;// no longer supplied in csv file but we need it for placement when doing a bulk insert will be empty
                            loadFileRecord.TN = vals[3].ToString().Trim();
                            loadFileRecord.Email = vals[4].ToString().Trim();
                            loadFileRecord.GeneralAction = vals[5].ToString().Trim();
                            loadFileRecord.GeneralDate = string.IsNullOrEmpty(vals[6].ToString().Trim()) ? (DateTime?)null : DateTime.Parse(vals[6].ToString().Trim());
                            loadFileRecord.E911Action = vals[7].ToString().Trim();
                            loadFileRecord.E911Date = string.IsNullOrEmpty(vals[8].ToString().Trim()) ? (DateTime?)null : DateTime.Parse(vals[8].ToString().Trim());
                            loadFileRecord.isData = vals[9].ToString().Trim();
                            loadFileRecord.isVoip = vals[10].ToString().Trim();
                            loadFileRecord.User = vals[11].ToString().Trim();
                            loadFileRecord.State = vals[12].ToString().Trim();
                            loadFileRecord.DPIRegion = vals[13].ToString().Trim();

                            loadFileRecordList.Add(loadFileRecord);
                        }
                    }



                }
                #endregion Build CustomerRecord Object

                using (FrontierEntities entities = new FrontierEntities())
                {
                    entities.CommandTimeout = 10000000;

                    #region Delete all records from dbo.tblE911LoadFileTempHold

                    entities.ExecuteStoreCommand("DELETE FROM dbo.tblE911LoadFileTempHold");
                    entities.SaveChanges();
                    #endregion Delete all records from dbo.tblE911LoadFileTempHold


                    #region Insert into dbo.tblE911LoadFileTempHold
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();


                        // Create a table with some rows. 
                        DataTable currentLoadFiles = GenericToDataTable.ConvertTo<LoadFileRecord>(loadFileRecordList);

                        // Create the SqlBulkCopy object. 
                        // Note that the column positions in the source DataTable 
                        // match the column positions in the destination table so 
                        // there is no need to map columns. 
                        using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection))
                        {
                            bulkCopy.DestinationTableName = "dbo.tblE911LoadFileTempHold";

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
                    #endregion Insert into dbo.tblE911LoadFileTempHold
                }

            }
            catch (Exception ex)
            {
                SendErrorMessage(ex, currentFile);
            }
        }

        /// <summary>
        /// Takes records from the temphold table and compres SubscriberId in the Destination table
        /// If there is no existing SubscriberId it will insert the record into the Destination table
        /// </summary>
        private static void CopyRecordstoMainTable()
        {

            tblE911LoadFileTempHold origin = new tblE911LoadFileTempHold();

            using (FrontierEntities entities = new FrontierEntities())
            {
                //Grab all the records from the temphold table
                var originQuery = (from temphold in entities.tblE911LoadFileTempHold
                                   select temphold).ToList();

                foreach (var tempitem in originQuery)
                {
                    //Look for a matching SubscriberId
                    var destQuery = (from lf in entities.tblE911LoadFile
                                     where lf.SubscriberId == tempitem.SubscriberId
                                     select lf).Any();

                    if (!destQuery)//If there isn't any insert the record
                    {

                        tblE911LoadFile destination = new tblE911LoadFile();

                        destination.SubscriberId = tempitem.SubscriberId;
                        destination.Name = tempitem.Name;
                        destination.Signature = tempitem.Signature;
                        destination.BirthYear = tempitem.BirthYear;
                        destination.TN = tempitem.TN;
                        destination.Email = tempitem.Email;
                        destination.GeneralAction = tempitem.GeneralAction;
                        destination.GeneralDate = tempitem.GeneralDate;
                        destination.E911Action = tempitem.E911Action;
                        destination.E911Date = tempitem.E911Date;
                        destination.IsData = tempitem.IsData;
                        destination.IsVoip = tempitem.IsVoip;
                        destination.User = tempitem.User;
                        destination.State = tempitem.State;
                        destination.DPIRegion = tempitem.DPIRegion;

                        entities.AddTotblE911LoadFile(destination);
                        entities.SaveChanges();
                    }

                }

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
                            InsertCurrentLoadFileTotblE911LoadFileTempHold(currentFile, sqlConn);

                            //Copy records from tempholdtable to the actual table
                            CopyRecordstoMainTable();
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
                        mail.Subject = "Frontier E911 LoadFile Import CSV File Import - Failed";
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

            Calibrus.ErrorHandler.Alerting alert = new Calibrus.ErrorHandler.Alerting("FrontierE911LoadFileInsert");
            alert.SendAlert(ex.Source, sb.ToString(), Environment.MachineName, Environment.UserName, Environment.Version.ToString());
        }
        private static void SendErrorMessage(Exception ex, string filename)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendFormat("ex:{0}, innerEx:{1}", ex.Message, ex.InnerException == null ? "" : ex.InnerException.Message);

            Calibrus.ErrorHandler.Alerting alert = new Calibrus.ErrorHandler.Alerting("FrontierE911LoadFileInsert");
            alert.SendAlert(ex.Source, String.Format("CurrentFile: {0} -- {1}", filename, sb.ToString()), Environment.MachineName, Environment.UserName, Environment.Version.ToString());
        }

        private static void LogError(Exception ex, string filename)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendFormat("ex:{0}, innerEx:{1}", ex.Message, ex.InnerException == null ? "" : ex.InnerException.Message);

            Calibrus.ErrorHandler.Logging log = new Calibrus.ErrorHandler.Logging();
            log.LogError("FrontierE911LoadFileInsert", Environment.Version.ToString(), Environment.MachineName, Environment.UserName, ex.Source,
                String.Format("File: {0} -- {1}", filename, sb.ToString()));
        }
        #endregion Error Handling
    }
}
