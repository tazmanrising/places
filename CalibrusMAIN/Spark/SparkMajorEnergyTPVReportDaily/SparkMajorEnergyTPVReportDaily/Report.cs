using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using Excel = Microsoft.Office.Interop.Excel;
using Calibrus.ExcelFunctions;
using Calibrus.ErrorHandler;
using Calibrus.Mail;

namespace SparkMajorEnergyTPVReportDaily
{
    public class Report
    {
        public static object na = System.Reflection.Missing.Value;

        #region Main
        public static void Main(string[] args)
        {
            string rootPath = string.Empty;
            string mailRecipientTO = string.Empty;
            string mailRecipientBCC = string.Empty;

            //get report interval
            DateTime StartDate = new DateTime();
            DateTime EndDate = new DateTime();

            //start to  build the form pathing
            string xlsFilename = string.Empty;
            string xlsFilePath = string.Empty;

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

            //grab values from app.config
            rootPath = ConfigurationManager.AppSettings["rootPath"].ToString();
            mailRecipientTO = ConfigurationManager.AppSettings["mailRecipientTO"].ToString();
            mailRecipientBCC = ConfigurationManager.AppSettings["mailRecipientBCC"].ToString();

            //Get list of Vendors
            List<VendorObject> vendorList = GetVendorList();
            #region VendorObject Loop
            foreach (VendorObject vendor in vendorList)
            {
                //Get records based on Vendor and DateTime
                List<spMajorEnergyTPVDailyReport_Result> recordList = GetMajorEnergyTPVDailYReportRecords(StartDate, EndDate, vendor.VendorId);

                if (recordList.Count > 0)
                {
                    //start Excel
                    Excel.Application exApp = new Excel.Application();
                    Excel.Workbook exBook = null;
                    Excel.Worksheet exSheet = null;
                    Excel.Range exRange = null;

                    int sheetsAdded = 0;

                    sheetsAdded = 0;
                    try
                    {
                        //Set global attributes
                        exApp.StandardFont = "Calibri";
                        exApp.StandardFontSize = 11;

                        exBook = exApp.Workbooks.Add(na);
                        exApp.Visible = false;

                        if (sheetsAdded < exBook.Sheets.Count)
                        {
                            exSheet = (Excel.Worksheet)exBook.Sheets[sheetsAdded + 1];
                        }
                        else
                        {
                            exSheet = (Excel.Worksheet)exBook.Sheets.Add(na, exBook.ActiveSheet, na, na);
                        }

                        sheetsAdded++;

                        string sheetName = vendor.VendorName;
                        exSheet.Name = sheetName.Length > 30 ? sheetName.Substring(0, 30) : sheetName; //force length of sheet name due to excel constraints
                        exSheet.Select(na);
                        //write out Report
                        WriteReport(ref exApp, ref exRange, StartDate, EndDate, recordList);

                        //save report                        
                        SaveXlsDocument(ref rootPath, ref xlsFilename, ref xlsFilePath, exBook, StartDate, vendor.VendorName);

                    }
                    catch (Exception ex)
                    {
                        SendErrorMessage(ex);
                    }
                    finally
                    {
                        exApp.DisplayAlerts = false;

                        exBook.Close();
                        exApp.Quit();
                    }
                    SendEmail(ref xlsFilePath, StartDate, mailRecipientTO, mailRecipientBCC, vendor.VendorName);
                }
            }
            #endregion VendorObject Loop
        }
        #endregion Main

        #region Excel
        private static void WriteReport(ref Excel.Application exApp, ref Excel.Range exRange, DateTime sDate, DateTime eDate, List<spMajorEnergyTPVDailyReport_Result> listOfRecords)
        {
            #region Variables

            Excel.Font exFont = null;
            //Placeholders as I move through the Excel sheet
            int rowInitialize = 1; //initial seed for the row data
            int colInitialize = 65; // column A
            int row = 0;// where we start the row data
            int col = 0;

            row = rowInitialize;  //set the row for the data
            col = colInitialize;//set the column for the data

            #endregion Variables

            #region Header
            //DATE and Time EST 	
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "DATE and Time EST";
            col++;

            //REP 	
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "REP";
            col++;

            //Conf No 	
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "Conf No";
            col++;

            //Application # 	
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "Application # ";
            col++;

            //RepID 	
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "RepID ";
            col++;

            //Rep Phone No 	
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "Rep Phone No ";
            col++;

            //First Name 	
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "First Name";
            col++;

            //Last Name 	
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "Last Name";
            col++;

            //Phone 	
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "Phone";
            col++;

            //Address 	
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "Address";
            col++;

            //City 	
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "City";
            col++;

            //State 	
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "State";
            col++;

            //ZipCode  	
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "ZipCode";
            col++;

            //Order   	
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "Order";
            col++;

            //VARIABLE/FIXED   	
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "VARIABLE/FIXED";
            col++;

            //E UTILITY  	
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "E UTILITY";
            col++;

            //DURATION ELECTRIC  	
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "DURATION ELECTRIC";
            col++;

            //G UTILITY   	
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "G UTILITY";
            col++;

            //DURATION GAS  	
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "DURATION GAS";
            col++;

            //Acct #   	
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "Acct #";
            col++;

            //Verified   	
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "Verified";
            col++;

            //Reason  	     	
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "Reason";
            col++;

            //Relationship to the Account Holder  	
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "Relationship to the Account Holder";
            col++;

            //Comments   	
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "Comments";
            col++;

            //Email Address  	
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "Email Address";
            col++;

            //Queue   	
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "Queue";
            col++;

            //Call Duration   	
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "Call Duration";
            col++;


            //Reset column
            col = colInitialize;
            row++;

            #endregion Header

            #region Data

            foreach (spMajorEnergyTPVDailyReport_Result record in listOfRecords)
            {
                //DATE and Time EST	
                DateTime dt = DateTime.Now;
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.NumberFormat = "m/d/yyyy h:mm AM/PM";
                exRange.Value2 = record.CallDateTime.HasValue ? ConvertFromArizonaToEST(dt = record.CallDateTime.Value) : (DateTime?)null;
                col++;

                //REP	
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = record.REP;
                col++;

                //Conf No  	   	
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = record.Conf_No;
                col++;

                //Application # 	
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = string.Empty;
                col++;

                //RepID  	 	
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = record.RepId;
                col++;

                //Rep Phone No 	
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = record.Rep_Phone_No;
                col++;

                //First Name  	
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = record.First_Name;
                col++;

                //Last Name 	
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = record.Last_Name;
                col++;

                //Phone  	
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = record.Phone;
                col++;

                //Address 	
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = record.Address;
                col++;

                //City  	
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = record.City;
                col++;

                //State  	
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = record.State;
                col++;

                //ZipCode 	
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = record.ZipCode;
                col++;

                //Order  		
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = record.UtilityOrder;
                col++;

                //VARIABLE/FIXED  	
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = string.Empty;
                col++;

                //E UTILITY  	
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = IsValueNull(record.E_Utility) ? string.Empty : ConvertLDCCode(record.E_Utility);
                col++;

                //DURATION ELECTRIC 	
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = record.Duration_Electric;
                col++;

                //G UTILITY  	
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = IsValueNull(record.G_Utility) ? string.Empty : ConvertLDCCode(record.G_Utility);                
                col++;

                //DURATION GAS  	
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = record.Duration_Gas;
                col++;

                //Acct #  	
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = record.AccountNumber;
                col++;

                //Verified	
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = record.Verified;
                col++;

                //Reason  	  	
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = record.Reason;
                col++;

                //Relationship to the Account Holder 	
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = record.Relationship_To_The_Account_Holder;
                col++;

                //Comments  	
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = string.Empty;
                col++;

                //Email Address 	
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = record.Email_Address;
                col++;

                //Queue 	
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = string.Empty;
                col++;

                //Call Duration	
                TimeSpan t = TimeSpan.FromSeconds(record.Call_Duration);
                string CallDuration = string.Format("{0:D2}:{1:D2}:{2:D2}",
                                t.Hours,
                                t.Minutes,
                                t.Seconds);
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = CallDuration.ToString();
                col++;


                //Reset column
                col = colInitialize;
                row++;
            }
            #endregion Data

            exRange = (Excel.Range)exApp.get_Range("A1", "AA1");
            exRange.EntireColumn.AutoFit();
        }
        #endregion Exel

        #region GetData

        /// <summary>
        /// Gets a list of Vendors for the Major Energy - BrandId = 6
        /// </summary>
        /// <returns></returns>
        private static List<VendorObject> GetVendorList()
        {
            //Select distinct v.VendorId
            //        ,v.VendorNumber
            //        ,v.VendorName
            //FROM [Spark].[v1].[Vendor] v
            //JOIN [Spark].[v1].[ProgramVendor] pv on pv.VendorId = v.VendorId
            //JOIN[Spark].[v1].[Program] p on p.ProgramId = pv.ProgramId
            //WHERE p.BrandId = 6 --Major Energy            
            //ORDER By v.VendorName

            List<VendorObject> vendors = new List<VendorObject>();
            using (SparkEntities entities = new SparkEntities())
            {
                var query = (from v in entities.Vendors
                             join pv in entities.ProgramVendors on v.VendorId equals pv.VendorId
                             join p in entities.Programs on pv.ProgramId equals p.ProgramId
                             where p.BrandId == 6
                             select new
                             {
                                 VendorId = v.VendorId,
                                 VendorNumber = v.VendorNumber,
                                 VendorName = v.VendorName
                             }).Distinct();

                foreach (var item in query.OrderBy(v => v.VendorId))
                {
                    VendorObject vendor = new VendorObject();
                    vendor.VendorId = item.VendorId;
                    vendor.VendorNumber = item.VendorNumber;
                    vendor.VendorName = item.VendorName;
                    vendors.Add(vendor);
                }
            }

            return vendors;
        }

        /// <summary>
        /// Gets a list of TPV Report records for a date range for Major Energy - BrandId = 6
        /// </summary>
        /// <param name="sDate"></param>
        /// <param name="eDate"></param>
        /// <param name="vendorId"></param>
        /// <returns></returns>
        private static List<spMajorEnergyTPVDailyReport_Result> GetMajorEnergyTPVDailYReportRecords(DateTime sDate, DateTime eDate, int vendorId)
        {
            List<spMajorEnergyTPVDailyReport_Result> result = null;
            try
            {
                using (SparkEntities entities = new SparkEntities())
                {
                    result = entities.spMajorEnergyTPVDailyReport(startDate: sDate, endDate: eDate, vendorId: vendorId).ToList();
                }
            }
            catch (Exception ex)
            {
                SendErrorMessage(ex);
                //throw ex;
            }

            return result;
        }
        #endregion GetData

        #region Utilities
        private static string ConvertLDCCode(string ldcCode)
        {
            string convertedLdcCode = string.Empty;
            //MECO = Masselec
            //WMECO = Westmass
            //NSTARB = NSTAR
            //NSTARC = NSTAR
            switch (ldcCode.ToUpper())
            {
                case "MECO":
                    convertedLdcCode = "Masselec";
                    break;
                case "WMECO":
                    convertedLdcCode = "Westmass";
                    break;

                case "NSTARB":
                    convertedLdcCode = "NSTAR";
                    break;

                case "NSTARC":
                    convertedLdcCode = "NSTAR";
                    break;
                default:
                    convertedLdcCode = ldcCode;//leave it as is
                    break;

            }
            return convertedLdcCode;
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
        /// Converts Arizona US Mountain Standard Time to Eastern Standard Time
        /// </summary>
        /// <param name="azTime"></param>
        /// <returns></returns>
        public static DateTime ConvertFromArizonaToEST(DateTime azTime)
        {
            DateTime timeUtc = azTime.ToUniversalTime();//Convert to UTC first
            TimeZoneInfo easternZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            DateTime easternTime = TimeZoneInfo.ConvertTimeFromUtc(timeUtc, easternZone);

            return easternTime;
        }

        //Converts 1 to a Yes, all else No
        private static string ConvertToYN(string character)
        {
            return character == "1" ? "YES" : "NO";
        }

        //Moving through the excel spreadsheet
        private static string ConvertColumn(int columnNumber)
        {
            //ASCII Decimal to Alphabet  65=A 90=Z
            int y = 0;
            string finalcol = "";

            if (columnNumber >= 65 && columnNumber <= 90) //single column
            {
                char col = (char)columnNumber;
                finalcol = col.ToString();
            }
            else //double letter column
            {
                y = (columnNumber - 65) / 26;
                int f = 65 + y - 1;
                char firstcol = (char)f;
                int s = columnNumber - (26 * y);
                char secondcol = (char)s;

                finalcol = firstcol.ToString() + secondcol.ToString();
            }

            return finalcol;
        }

        private static void SaveXlsDocument(ref string reportPath, ref string xlsFilename, ref string xlsFilePath, Excel.Workbook exBook, DateTime currentDate, string vendorName)
        {

            //Major_Energy_[Vendor]_Enrollment_[MMddYYYY].xlsx
            xlsFilename = "Major_Energy_" + vendorName + "_TPV_Report_" + String.Format("{0:MMddyyyy}", currentDate) + ".xls";

            xlsFilePath = string.Format(reportPath + xlsFilename);
            bool fileExists = File.Exists(xlsFilePath);
            if (fileExists)
            {
                //delete it
                File.Delete(xlsFilePath);
            }
            //save workbook
            exBook.SaveAs(Filename: xlsFilePath, FileFormat: Excel.XlFileFormat.xlWorkbookNormal);
        }

        private static void SendEmail(ref string xlsFilePath, DateTime currentDate, string strToEmail, string strBccEmail, string vendorName)
        {
            //string strMsgBody = string.Empty;
            try
            {

                //StringBuilder sb = new StringBuilder();

                //sb.AppendLine("");
                //strMsgBody = sb.ToString();

                SmtpMail mail = new SmtpMail("TMPWEB1", false);

                mail.AddAttachment(xlsFilePath);//Attach XLS report
                mail.AddRecipient(strToEmail, RecipientType.To);
                mail.AddRecipient(strBccEmail, RecipientType.Bcc);

                mail.From = "reports1@calibrus.com";

                mail.Subject = "Spark Major Energy TPV Report for " + vendorName + " " + currentDate.ToString("dddd, dd MMMM yyyy") + ".";

                //mail.Body = strMsgBody;
                mail.SendMessage();

            }
            catch (Exception ex)
            {
                SendErrorMessage(ex);
            }

        }

        private static void GetDates(out DateTime StartDate, out DateTime EndDate)
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
            EndDate = new DateTime(baseDate.Year, baseDate.Month, baseDate.Day, 0, 0, 0);//current date time as this runs for the previous day

        }
        #endregion Utilities

        #region Error Handling
        private static void SendErrorMessage(Exception ex)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendFormat("ex:{0}, innerEx:{1}", ex.Message, ex.InnerException == null ? "" : ex.InnerException.Message);

            Calibrus.ErrorHandler.Alerting alert = new Calibrus.ErrorHandler.Alerting("SparkMajorEnergyTPVReportDaily");
            alert.SendAlert(ex.Source, sb.ToString(), Environment.MachineName, Environment.UserName, Environment.Version.ToString());
        }
        #endregion Error Handling
    }
}
