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


namespace FrontierE911BrightPatternFileInsert
{
    public class FileInsert
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
            string CsvFileToImportBadFilePath = string.Empty;
            string CsvFileToImportBadFileArchivePath = string.Empty;

            string mailRecipientFailTO = string.Empty;

            string filenameToGrab = string.Empty;

            //Used for sql connection for the bulk inserts
            string FrontierSqlConn = string.Empty;

            try
            {

                CsvFileToImportFromPath = ConfigurationManager.AppSettings["CsvFileToImportFromPath"].ToString();
                CsvFileToImportArchivePath = ConfigurationManager.AppSettings["CsvFileToImportArchivePath"].ToString();
                CsvFileToImportBadFileArchivePath = ConfigurationManager.AppSettings["CsvFileToImportBadFileArchivePath"].ToString();


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

                                if (currentCsvFile.Contains("CallDetail"))
                                {
                                    //This is a csv Load file Call Detail to import                           
                                    InsertCurrentLoadFileCallDetailData(currentCsvFile, FrontierSqlConn, ref isError);
                                }
                                else
                                {
                                    //This is a csv Load file to import                           
                                    InsertCurrentLoadFileData(currentCsvFile, FrontierSqlConn, ref isError);

                                }

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

                                    //Move Imported CSV to Bad File Archive
                                    MoveFile(CsvFileToImportBadFileArchivePath, currentCsvFile);
                                }
                                else //if no error send email import success
                                {
                                    //SendEmail(currentFile, DateTime.Now, mailRecipientTO, ImportStatus.Success);

                                    //Move Imported CSV to Archive
                                    MoveFile(CsvFileToImportArchivePath, currentCsvFile);
                                }


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

        #region Data Insert
        /// <summary>
        /// Inserts data from the Load File csv
        /// </summary>
        /// <param name="currentFile"></param>
        /// <param name="connectionString"></param>
        /// <param name="isError"></param>
        private static void InsertCurrentLoadFileData(string currentFile, string connectionString, ref bool isError)
        {
            int badLine = 0;
            int BadLoadFileId = 0;
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
                    badLine = i;// need the line before this one, which is where it failed at
                    lines[i] = lines[i].Replace("\"", "");
                    var vals = lines[i].Split(','); //split on the comma


                    if (vals.Count() == 41)//IF we have the expected 41 fields of values
                    {

                        LoadFileRecord loadFileRecord = new LoadFileRecord();

                        loadFileRecord.E911BrightPatternLoadFileId = 0;//dummy placeholder for the identity field in the db
                        loadFileRecord.E911LoadFileId = Int32.Parse(vals[0].ToString().Trim()); 
                        loadFileRecord.SubscriberId = vals[1].ToString().Trim();
                        loadFileRecord.Name = vals[2].ToString().Trim();
                        loadFileRecord.Signature = vals[3].ToString().Trim();
                        loadFileRecord.BirthYear = vals[4].ToString().Trim();
                        loadFileRecord.TN = vals[5].ToString().Trim();
                        loadFileRecord.Email = vals[6].ToString().Trim();
                        loadFileRecord.GeneralAction = vals[7].ToString().Trim();
                        loadFileRecord.GeneralDate = string.IsNullOrEmpty(vals[8].ToString().Trim()) ? (DateTime?)null : DateTime.Parse(vals[8].ToString().Trim());
                        loadFileRecord.E911Action = vals[9].ToString().Trim();
                        loadFileRecord.E911Date = string.IsNullOrEmpty(vals[10].ToString().Trim()) ? (DateTime?)null : DateTime.Parse(vals[10].ToString().Trim());
                        loadFileRecord.IsData = vals[11].ToString().Trim();
                        loadFileRecord.IsVoip = vals[12].ToString().Trim();
                        loadFileRecord.User = vals[13].ToString().Trim();
                        loadFileRecord.State = vals[14].ToString().Trim();
                        loadFileRecord.DPIRegion = vals[15].ToString().Trim();
                        loadFileRecord.ThisPhonenumber = vals[16].ToString().Trim();
                        //ANI = vals[17].ToString().Trim(); //ANI is not USED but arrives in the file we import
                        loadFileRecord.IsCallAttempt = vals[18].ToString().Trim();
                        loadFileRecord.Completed = vals[19].ToString().Trim();
                        loadFileRecord.RecordDisposition = vals[20].ToString().Trim();
                        loadFileRecord.RecordDispositionCode = vals[21].ToString().Trim();
                        loadFileRecord.Outofquota = vals[22].ToString().Trim();
                        loadFileRecord.Quotagroup = vals[23].ToString().Trim();
                        loadFileRecord.CallDisposition = vals[24].ToString().Trim();
                        loadFileRecord.CallDispositionCode = vals[25].ToString().Trim();
                        loadFileRecord.CallNote = vals[26].ToString().Trim();
                        loadFileRecord.CallTime = string.IsNullOrEmpty(vals[27].ToString().Trim()) ? (DateTime?)null : DateTime.Parse(vals[27].ToString().Trim().Substring(0, 19));
                        loadFileRecord.DialingDuration = vals[28].ToString().Trim();
                        loadFileRecord.CPADuration = vals[29].ToString().Trim();
                        loadFileRecord.AnsweredDuration = vals[30].ToString().Trim();
                        loadFileRecord.Agent = vals[31].ToString().Trim();
                        loadFileRecord.Connected = vals[32].ToString().Trim();
                        loadFileRecord.CPAresult = vals[33].ToString().Trim();
                        loadFileRecord.CPArecordingfile = vals[34].ToString().Trim();
                        loadFileRecord.CPARTPserverid = vals[35].ToString().Trim();
                        loadFileRecord.Recordingfile = null; //vals[36].ToString().Trim();
                        loadFileRecord.RTPserverid = null; //vals[37].ToString().Trim();
                        loadFileRecord.GlobalInteractionID = vals[38].ToString().Trim();
                        loadFileRecord.RecordID = vals[39].ToString().Trim();
                        loadFileRecord.Listname = vals[40].ToString().Trim();
                       

                        loadFileRecordList.Add(loadFileRecord);

                    }

                }
                #endregion Build CustomerRecord Object

                if (loadFileRecordList.Count > 0)
                {
                    using (FrontierEntities entities = new FrontierEntities())
                    {
                        entities.CommandTimeout = 10000000;

                        #region Insert into dbo.tblE911BrightPatternLoadFile
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
                                bulkCopy.DestinationTableName = "dbo.tblE911BrightPatternLoadFile";

                                try
                                {
                                    bulkCopy.BulkCopyTimeout = 120; //set timeout
                                    // Write from the source to the destination.
                                    bulkCopy.WriteToServer(currentLoadFiles);

                                }
                                catch (Exception ex)
                                {

                                    isError = true;
                                    throw ex;
                                }
                            }

                        }

                        #endregion Insert into dbo.tblE911BrightPatternLoadFile
                    }
                }

            }
            catch (Exception ex)
            {
                isError = true;
                LogError(ex, currentFile, badLine);
                throw ex;
            }
        }

        /// <summary>
        /// Inserts data from the Load File Call Detail csv
        /// </summary>
        /// <param name="currentFile"></param>
        /// <param name="connectionString"></param>
        /// <param name="isError"></param>
        private static void InsertCurrentLoadFileCallDetailData(string currentFile, string connectionString, ref bool isError)
        {

            try
            {

                #region Build CustomerRecord Object
                List<LoadFileCallDetailRecord> loadFileCallDetailRecordList = new List<LoadFileCallDetailRecord>();

                // read the entire file and store each line
                // as a new element in a string[]
                var lines = File.ReadAllLines(currentFile);

                // we can skip the first line because it's
                // just headings - if you need the headings
                // just grab them off the 0 index
                for (int i = 1; i < lines.Length; i++)
                {
                    var vals = lines[i].Split(','); //split on the commas

                    if (vals.Count() == 22)//IF we have the expected 22 fields of values
                    {

                        LoadFileCallDetailRecord loadFileCallDetailRecord = new LoadFileCallDetailRecord();

                        loadFileCallDetailRecord.E911BrightPatternLoadFileCallDetailId = 0;//dummy placeholder for the identity field in the db

                        string strDateTime = vals[0].ToString().Trim() + " " + vals[1].ToString().Trim(); //Concatentate Date and Time

                        loadFileCallDetailRecord.CallDetailDateTime = string.IsNullOrEmpty(strDateTime) ? (DateTime?)null : DateTime.Parse(strDateTime);
                        loadFileCallDetailRecord.Type = vals[2].ToString().Trim();
                        loadFileCallDetailRecord.IVR = string.IsNullOrEmpty(vals[3].ToString().Trim()) ? 0 : ConvertTimeToSeconds(vals[3].ToString().Trim());
                        loadFileCallDetailRecord.QueueTime = string.IsNullOrEmpty(vals[4].ToString().Trim()) ? 0 : ConvertTimeToSeconds(vals[4].ToString().Trim());
                        loadFileCallDetailRecord.DialingRinging = string.IsNullOrEmpty(vals[5].ToString().Trim()) ? 0 : ConvertTimeToSeconds(vals[5].ToString().Trim());
                        loadFileCallDetailRecord.Talk = string.IsNullOrEmpty(vals[6].ToString().Trim()) ? 0 : ConvertTimeToSeconds(vals[6].ToString().Trim());
                        loadFileCallDetailRecord.Hold = string.IsNullOrEmpty(vals[7].ToString().Trim()) ? 0 : ConvertTimeToSeconds(vals[7].ToString().Trim());
                        loadFileCallDetailRecord.WrapUpTime = string.IsNullOrEmpty(vals[8].ToString().Trim()) ? 0 : ConvertTimeToSeconds(vals[8].ToString().Trim());
                        loadFileCallDetailRecord.Duration = string.IsNullOrEmpty(vals[9].ToString().Trim()) ? 0 : ConvertTimeToSeconds(vals[9].ToString().Trim());
                        loadFileCallDetailRecord.FromLocation = vals[10].ToString().Trim();
                        loadFileCallDetailRecord.OriginalDestination = vals[11].ToString().Trim();
                        loadFileCallDetailRecord.ConnectedTo = vals[12].ToString().Trim();
                        loadFileCallDetailRecord.ConnectedToNumber = vals[13].ToString().Trim();
                        loadFileCallDetailRecord.ServiceCampaign = vals[14].ToString().Trim();
                        loadFileCallDetailRecord.AgentDisposition = vals[15].ToString().Trim();
                        loadFileCallDetailRecord.Notes = vals[16].ToString().Trim();
                        loadFileCallDetailRecord.Disposition = vals[17].ToString().Trim();
                        loadFileCallDetailRecord.MediaType = vals[18].ToString().Trim();
                        loadFileCallDetailRecord.InSL = vals[19].ToString().Trim();
                        loadFileCallDetailRecord.GloablID = vals[20].ToString().Trim();
                        loadFileCallDetailRecord.InteractionStepID = vals[21].ToString().Trim();

                        string WavName = string.Empty;
                        if (!string.IsNullOrEmpty(vals[21].ToString().Trim()))
                        {
                            WavName = vals[21].ToString().Trim();
                            int pos = WavName.LastIndexOf("/") + 1;
                            WavName = WavName.Substring(pos, WavName.Length - pos);
                        }

                        loadFileCallDetailRecord.WavName = Path.GetFileNameWithoutExtension(WavName); //cut off the .wav

                        loadFileCallDetailRecordList.Add(loadFileCallDetailRecord);

                    }

                }
                #endregion Build CustomerRecord Object


                using (FrontierEntities entities = new FrontierEntities())
                {
                    entities.CommandTimeout = 10000000;

                    #region Insert into dbo.tblE911BrightPatternLoadFileCallDetail
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();


                        // Create a table with some rows. 
                        DataTable currentLoadCallDetailFiles = GenericToDataTable.ConvertTo<LoadFileCallDetailRecord>(loadFileCallDetailRecordList);

                        // Create the SqlBulkCopy object. 
                        // Note that the column positions in the source DataTable 
                        // match the column positions in the destination table so 
                        // there is no need to map columns. 
                        using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection))
                        {
                            bulkCopy.DestinationTableName = "dbo.tblE911BrightPatternLoadFileCallDetail";

                            try
                            {
                                bulkCopy.BulkCopyTimeout = 120; //set timeout
                                // Write from the source to the destination.
                                bulkCopy.WriteToServer(currentLoadCallDetailFiles);

                            }
                            catch (Exception ex)
                            {
                                isError = true;
                                throw ex;
                            }
                        }

                    }

                    #endregion Insert into dbo.tblE911BrightPatternLoadFileCallDetail
                }

            }
            catch (Exception ex)
            {
                isError = true;
                LogError(ex, currentFile);
                throw ex;
            }
        }

        #endregion Data Insert

        #region Utilities

        /// <summary>
        /// Takes in a HH:MM:SS and converts it to seconds as an int rounded up
        /// </summary>
        /// <param name="TimeToConvert"></param>
        /// <returns></returns>
        private static int? ConvertTimeToSeconds(string TimeToConvert)
        {
            double seconds = TimeSpan.Parse(TimeToConvert).TotalSeconds;
            return Convert.ToInt32(seconds);
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
                        mail.Subject = "Frontier E 911  Bright Pattern File Insert - Failed";
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


        #endregion Utilities

        #region Error Handling
        private static void SendErrorMessage(Exception ex)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendFormat("ex:{0}, innerEx:{1}", ex.Message, ex.InnerException == null ? "" : ex.InnerException.Message);

            Calibrus.ErrorHandler.Alerting alert = new Calibrus.ErrorHandler.Alerting("FrontierE911BrightPatternFileInsert");
            alert.SendAlert(ex.Source, sb.ToString(), Environment.MachineName, Environment.UserName, Environment.Version.ToString());
        }
        private static void SendErrorMessage(Exception ex, string filename)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendFormat("ex:{0}, innerEx:{1}", ex.Message, ex.InnerException == null ? "" : ex.InnerException.Message);

            Calibrus.ErrorHandler.Alerting alert = new Calibrus.ErrorHandler.Alerting("FrontierE911BrightPatternFileInsert");
            alert.SendAlert(ex.Source, String.Format("CurrentFile: {0} -- {1}", filename, sb.ToString()), Environment.MachineName, Environment.UserName, Environment.Version.ToString());
        }

        private static void LogError(Exception ex, string filename)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendFormat("ex:{0}, innerEx:{1}", ex.Message, ex.InnerException == null ? "" : ex.InnerException.Message);

            Calibrus.ErrorHandler.Logging log = new Calibrus.ErrorHandler.Logging();
            log.LogError("FrontierE911BrightPatternFileInsert", Environment.Version.ToString(), Environment.MachineName, Environment.UserName, ex.Source,
                String.Format("File: {0} -- {1}", filename, sb.ToString()));
        }
        private static void LogError(Exception ex, string filename, int recordLineFaileAt)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendFormat("ex:{0}, innerEx:{1}", ex.Message, ex.InnerException == null ? "" : ex.InnerException.Message);

            Calibrus.ErrorHandler.Logging log = new Calibrus.ErrorHandler.Logging();
            log.LogError("FrontierE911BrightPatternFileInsert", Environment.Version.ToString(), Environment.MachineName, Environment.UserName, ex.Source,
                String.Format("File: {0} - Bad Line In File: - {1} - {2} ", filename, recordLineFaileAt, sb.ToString()));
        }
        #endregion Error Handling

    }
}
