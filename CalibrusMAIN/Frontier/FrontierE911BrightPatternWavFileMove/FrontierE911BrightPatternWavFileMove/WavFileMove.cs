using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using Calibrus.ErrorHandler;

namespace FrontierE911BrightPatternWavFileMove
{
    public class WavFileMove
    {

        #region Main
        public static void Main(string[] args)
        {
            string WavFileMoveFromPath = string.Empty;
            string WavFileMoveArchivePath = string.Empty;
            string mailRecipientFailTO = string.Empty;
            string filenameToGrab = string.Empty;

            try
            {
                WavFileMoveFromPath = ConfigurationManager.AppSettings["WavFileMoveFromPath"].ToString();
                WavFileMoveArchivePath = ConfigurationManager.AppSettings["WavFileMoveArchivePath"].ToString();
                mailRecipientFailTO = ConfigurationManager.AppSettings["mailRecipientFailTO"].ToString();

                //Look for files to process
                var ListWaveFileNames = Directory.EnumerateFiles(WavFileMoveFromPath, "*.*", System.IO.SearchOption.TopDirectoryOnly).Where(s => s.EndsWith(".wav"));
                if (ListWaveFileNames.Count() > 0)
                {
                    foreach (var currentWavFile in ListWaveFileNames)
                    {
                        filenameToGrab = currentWavFile;// used in case we need to log an error on this specific file

                        //Move Imported CSV to Archive
                        MoveFile(WavFileMoveArchivePath, currentWavFile);
                    }

                }
            }
            catch (Exception ex)
            {
                SendErrorMessage(ex, filenameToGrab);
            }
        }
        #endregion Main

        #region Utilities
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
        /// <param name="FileToDelete"></param>
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
            var ListFileNamesToDelete = Directory.EnumerateFiles(FilesToDeletePath, "*.*", System.IO.SearchOption.TopDirectoryOnly).Where(s => s.EndsWith(".wav"));
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
        #endregion Utilities

        #region Error Handling
        private static void SendErrorMessage(Exception ex)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendFormat("ex:{0}, innerEx:{1}", ex.Message, ex.InnerException == null ? "" : ex.InnerException.Message);

            Calibrus.ErrorHandler.Alerting alert = new Calibrus.ErrorHandler.Alerting("FrontierE911BrightPaternWavFileMove");
            alert.SendAlert(ex.Source, sb.ToString(), Environment.MachineName, Environment.UserName, Environment.Version.ToString());
        }
        private static void SendErrorMessage(Exception ex, string filename)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendFormat("ex:{0}, innerEx:{1}", ex.Message, ex.InnerException == null ? "" : ex.InnerException.Message);

            Calibrus.ErrorHandler.Alerting alert = new Calibrus.ErrorHandler.Alerting("FrontierE911BrightPaternWavFileMove");
            alert.SendAlert(ex.Source, String.Format("WavFile: {0} -- {1}", filename, sb.ToString()), Environment.MachineName, Environment.UserName, Environment.Version.ToString());
        }

        private static void LogError(Exception ex, string filename)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendFormat("ex:{0}, innerEx:{1}", ex.Message, ex.InnerException == null ? "" : ex.InnerException.Message);

            Calibrus.ErrorHandler.Logging log = new Calibrus.ErrorHandler.Logging();
            log.LogError("FrontierE911BrightPaternWavFileMove", Environment.Version.ToString(), Environment.MachineName, Environment.UserName, ex.Source,
                String.Format("WavFile: {0} -- {1}", filename, sb.ToString()));
        }

        #endregion Error Handling
    }
}
