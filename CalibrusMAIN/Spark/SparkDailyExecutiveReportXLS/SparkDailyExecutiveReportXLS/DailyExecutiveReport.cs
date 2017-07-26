using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Configuration;
using Calibrus.Mail;
using Calibrus.ErrorHandler;
using Calibrus.ExcelFunctions;
using Excel = Microsoft.Office.Interop.Excel;
namespace SparkDailyExecutiveReportXLS
{
    public class DailyExecutiveReport
    {

        public static object na = System.Reflection.Missing.Value;

        #region Main
        public static void Main(string[] args)
        {

            string rootPath = string.Empty;
            string mailRecipientTO = string.Empty;
            string mailRecipientBCC = string.Empty;


            //get report interval
            DateTime CurrentDate = new DateTime();

            //start to  build the form pathing
            string xlsFilename = string.Empty;
            string xlsFilePath = string.Empty;

            if (args.Length > 0)
            {
                if (DateTime.TryParse(args[0], out CurrentDate))
                {
                    GetDates(out CurrentDate);
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
                GetDates(out CurrentDate);
            }
            //grab values from app.config
            rootPath = ConfigurationManager.AppSettings["rootPath"].ToString();
            mailRecipientTO = ConfigurationManager.AppSettings["mailRecipientTO"].ToString();
            mailRecipientBCC = ConfigurationManager.AppSettings["mailRecipientBCC"].ToString();

            //start Excel
            Excel.Application exApp = new Excel.Application();
            Excel.Workbook exBook = null;
            Excel.Worksheet exSheet = null;
            Excel.Range exRange = null;

            int sheetsAdded = 0;

            exBook = exApp.Workbooks.Add(na);

            exApp.Visible = false;

            //Set global attributes
            exApp.StandardFont = "Calibri";
            exApp.StandardFontSize = 11;

            try
            {

                #region Get Data
                List<Vendor> VendorList = new List<Vendor>();
                List<UtilityType> UtilityTypeList = new List<UtilityType>();


                VendorList = GetVendors();
                UtilityTypeList = GetUtilityTypes();
                #endregion

                #region Write Report

                WriteReport(ref exApp, ref exRange, CurrentDate.AddDays(-1), CurrentDate, VendorList, UtilityTypeList);

                //Autosize the columns
                exRange = (Excel.Range)exApp.get_Range("A1", "H1");
                exRange.EntireColumn.AutoFit();
                #endregion

                //select the first tab in the workbook
                exSheet = (Excel.Worksheet)exApp.Worksheets[1];
                exSheet.Select(na);


                //Save the xls Report to represent the day prior to the current run date for proper identification of the data run
                SaveXlsDocument(ref rootPath, ref xlsFilename, ref xlsFilePath, exBook, CurrentDate.AddDays(-1));

            }
            catch (Exception ex)
            {
                SendErrorMessage(ex);
                //throw ex;
            }
            finally
            {
                exApp.DisplayAlerts = false;

                exBook.Close();
                exApp.Quit();
            }

            // Email File
            SendEmail(ref xlsFilePath, CurrentDate.AddDays(-1), mailRecipientTO, mailRecipientBCC);

        }

        #endregion

        #region Excel

        public static void WriteReport(ref Excel.Application exApp, ref Excel.Range exRange, DateTime startDate, DateTime endDate, List<Vendor> vendorList, List<UtilityType> utilityTypeList)
        {
            int rowInitialize = 1; //initial seed for the row data
            int row = 0;// where we start the row data

            int headerColumnInitialize = 66; //initial seed for column header - column  B
            int dataColumnInitialize = 67; //initial seed for column data - column  C
            int col = 0;

            row = rowInitialize;  //set the row for the data   
            col = dataColumnInitialize;//set the column for the data

            //Hiding the gridlines of the worksheet
            exApp.ActiveWindow.DisplayGridlines = false;
            Excel.Border exBorders = null;

            #region Header

            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn("A", row), new RangeColumn("H", row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                24, false, false, false);
            exRange.Merge(na);
            exRange.Interior.ColorIndex = 15;//grey
            exRange.Value2 = "Spark Energy - Res - DTD";

            row++;

            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn("A", row), new RangeColumn("H", row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                14, false, true, false);
            exRange.Merge(na);
            exBorders = exRange.Borders[Excel.XlBordersIndex.xlEdgeBottom];
            exBorders.LineStyle = Excel.XlLineStyle.xlContinuous;
            exBorders.Weight = Excel.XlBorderWeight.xlThin;
            exRange.Interior.ColorIndex = 15;//grey
            exRange.Value2 = string.Format("{0:MM/dd/yyyy}", startDate);

            row++;

            row++;
#endregion

            #region Vendor
            //used for overall total for all vendors at end of report
            int? grandTotal = 0;
            int verifiedGrandTotal = 0;
            int tmGrandTotal = 0;
            foreach (Vendor vendor in vendorList)
            {

                #region Vendor Headers
                //Vendor Header
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn("A", row), new RangeColumn("H", row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                   24, false, false, false);
                exRange.Merge(na);
                exBorders = exRange.Borders[Excel.XlBordersIndex.xlEdgeTop];
                exBorders.LineStyle = Excel.XlLineStyle.xlContinuous;
                exBorders.Weight = Excel.XlBorderWeight.xlThin;
                exRange.Interior.ColorIndex = 15;//grey
                exRange.Value2 = vendor.VendorName;
                row++;

                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn("A", row), new RangeColumn("H", row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                    14, false, false, false);
                exRange.Merge(na);
                exBorders = exRange.Borders[Excel.XlBordersIndex.xlEdgeBottom];
                exBorders.LineStyle = Excel.XlLineStyle.xlContinuous;
                exBorders.Weight = Excel.XlBorderWeight.xlThin;
                exRange.Interior.ColorIndex = 15;//grey
                exRange.Value2 = "";
                row++;


                //Fuel Type
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn("A", row), new RangeColumn("A", row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                    10, true, false, false);
                exRange.Merge(na);
                exBorders = exRange.Borders[Excel.XlBordersIndex.xlEdgeBottom];
                exBorders.LineStyle = Excel.XlLineStyle.xlContinuous;
                exBorders.Weight = Excel.XlBorderWeight.xlThin;
                exRange.Value2 = "Fuel Type";


                //LDC Code
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn("B", row), new RangeColumn("B", row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                    10, true, false, false);
                exRange.Merge(na);
                exBorders = exRange.Borders[Excel.XlBordersIndex.xlEdgeBottom];
                exBorders.LineStyle = Excel.XlLineStyle.xlContinuous;
                exBorders.Weight = Excel.XlBorderWeight.xlThin;
                exRange.Value2 = "LDC Code";


                //Total DTD
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn("C", row), new RangeColumn("C", row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                    10, true, false, false);
                exRange.Merge(na);
                exBorders = exRange.Borders[Excel.XlBordersIndex.xlEdgeBottom];
                exBorders.LineStyle = Excel.XlLineStyle.xlContinuous;
                exBorders.Weight = Excel.XlBorderWeight.xlThin;
                exRange.Value2 = "Total DTD";


                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn("D", row), new RangeColumn("D", row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                    10, true, false, false);
                exRange.Merge(na);
                exBorders = exRange.Borders[Excel.XlBordersIndex.xlEdgeBottom];
                exBorders.LineStyle = Excel.XlLineStyle.xlContinuous;
                exBorders.Weight = Excel.XlBorderWeight.xlThin;
                exRange.Value2 = "";


                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn("E", row), new RangeColumn("E", row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                    10, true, false, false);
                exRange.Merge(na);
                exBorders = exRange.Borders[Excel.XlBordersIndex.xlEdgeBottom];
                exBorders.LineStyle = Excel.XlLineStyle.xlContinuous;
                exBorders.Weight = Excel.XlBorderWeight.xlThin;
                exRange.Value2 = "Total TM";

                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn("F", row), new RangeColumn("F", row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                   10, true, false, false);
                exRange.Merge(na);
                exBorders = exRange.Borders[Excel.XlBordersIndex.xlEdgeBottom];
                exBorders.LineStyle = Excel.XlLineStyle.xlContinuous;
                exBorders.Weight = Excel.XlBorderWeight.xlThin;
                exRange.Value2 = "";

                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn("G", row), new RangeColumn("G", row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                   10, true, false, false);
                exRange.Merge(na);
                exBorders = exRange.Borders[Excel.XlBordersIndex.xlEdgeBottom];
                exBorders.LineStyle = Excel.XlLineStyle.xlContinuous;
                exBorders.Weight = Excel.XlBorderWeight.xlThin;
                exRange.Value2 = "";

                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn("H", row), new RangeColumn("H", row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                   10, true, false, false);
                exRange.Merge(na);
                exBorders = exRange.Borders[Excel.XlBordersIndex.xlEdgeBottom];
                exBorders.LineStyle = Excel.XlLineStyle.xlContinuous;
                exBorders.Weight = Excel.XlBorderWeight.xlThin;
                exRange.Value2 = "";
                row++;
                #endregion

                #region Vendor Data

                #region UtilityType Loop
                //used for totals across individual vendors
                int verifiedOverallTotal = 0;
                int tmOverallTotal = 0;
                //Need to loop thorugh UtilityType and get the LDCCOdE and Totals
                foreach (UtilityType utilityType in utilityTypeList)
                {

                    //Fuel Type
                    exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn("A", row), new RangeColumn("A", row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                        10, false, false, false);
                    exRange.Merge(na);
                    exRange.Value2 = utilityType.UtilityTypeName;

                    #region LDCcodes Loop
                    List<string> ldcCodes = GetLDCCodeList(startDate, endDate, vendor.VendorId, utilityType.UtilityTypeName);
                    if (ldcCodes.Count > 0)
                    {
                        foreach (string ldcCode in ldcCodes)
                        {

                            int TMTotal = 0;
                            int Verifiedtotal = 0;

                            TMTotal = getTotals(startDate, endDate, vendor.VendorId, vendor.VendorName, ldcCode, utilityType.UtilityTypeName, "Telesales");

                            Verifiedtotal = getTotals(startDate, endDate, vendor.VendorId, vendor.VendorName, ldcCode, utilityType.UtilityTypeName, "Door to Door");


                            //set the grand totals
                            verifiedOverallTotal += Verifiedtotal;
                            tmOverallTotal += TMTotal;

                            //LDC Code
                            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn("B", row), new RangeColumn("B", row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                                10, false, false, false);
                            exRange.Merge(na);
                            exRange.Value2 = ldcCode;

                            //Total DTD
                            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn("C", row), new RangeColumn("C", row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                                10, false, false, false);
                            exRange.Merge(na);
                            exRange.Value2 = Verifiedtotal;

                            
                            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn("D", row), new RangeColumn("D", row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                                10, false, false, false);
                            exRange.Merge(na);
                            exRange.Value2 = "";

                            //Total TM
                            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn("E", row), new RangeColumn("E", row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                                10, false, false, false);
                            exRange.Merge(na);
                            exRange.Value2 = TMTotal;

                            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn("F", row), new RangeColumn("F", row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                                10, false, false, false);
                            exRange.Merge(na);
                            exRange.Value2 = "";

                            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn("G", row), new RangeColumn("G", row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                                10, false, false, false);
                            exRange.Merge(na);
                            exRange.Value2 = "";

                            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn("H", row), new RangeColumn("H", row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                                10, false, false, false);
                            exRange.Merge(na);
                            exRange.Value2 = "";
                            row++;
                        }
                    }
                    else
                    {

                        //LDC Code
                        exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn("B", row), new RangeColumn("B", row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                            10, false, false, false);
                        exRange.Merge(na);
                        exRange.Value2 = "";

                        //Total DTD
                        exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn("C", row), new RangeColumn("C", row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                            10, false, false, false);
                        exRange.Merge(na);
                        exRange.Value2 = 0;

                        
                        exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn("D", row), new RangeColumn("D", row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                            10, false, false, false);
                        exRange.Merge(na);
                        exRange.Value2 = "";

                        //Total TM
                        exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn("E", row), new RangeColumn("E", row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                            10, false, false, false);
                        exRange.Merge(na);
                        exRange.Value2 = 0;

                        exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn("F", row), new RangeColumn("F", row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                            10, false, false, false);
                        exRange.Merge(na);
                        exRange.Value2 = "";

                        exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn("G", row), new RangeColumn("G", row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                            10, false, false, false);
                        exRange.Merge(na);
                        exRange.Value2 = "";

                        exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn("H", row), new RangeColumn("H", row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                            10, false, false, false);
                        exRange.Merge(na);
                        exRange.Value2 = "";
                        row++;
                    }

                    #endregion


                }
                #endregion

                #region Overall DTD & TM Totals
                // write OVerall totals row
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn("A", row), new RangeColumn("A", row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                    10, false, false, false);
                exRange.Merge(na);
                exBorders = exRange.Borders[Excel.XlBordersIndex.xlEdgeBottom];
                exBorders.LineStyle = Excel.XlLineStyle.xlContinuous;
                exBorders.Weight = Excel.XlBorderWeight.xlThin;
                exRange.Value2 = "";


                //Total Header
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn("B", row), new RangeColumn("B", row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                    10, true, false, false);
                exRange.Merge(na);
                exBorders = exRange.Borders[Excel.XlBordersIndex.xlEdgeBottom];
                exBorders.LineStyle = Excel.XlLineStyle.xlContinuous;
                exBorders.Weight = Excel.XlBorderWeight.xlThin;
                exRange.Value2 = "Total";


                //Total Verified
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn("C", row), new RangeColumn("C", row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                    10, false, false, false);
                exRange.Merge(na);
                exBorders = exRange.Borders[Excel.XlBordersIndex.xlEdgeBottom];
                exBorders.LineStyle = Excel.XlLineStyle.xlContinuous;
                exBorders.Weight = Excel.XlBorderWeight.xlThin;
                exRange.Value2 = verifiedOverallTotal;

                
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn("D", row), new RangeColumn("D", row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                    10, false, false, false);
                exRange.Merge(na);
                exBorders = exRange.Borders[Excel.XlBordersIndex.xlEdgeBottom];
                exBorders.LineStyle = Excel.XlLineStyle.xlContinuous;
                exBorders.Weight = Excel.XlBorderWeight.xlThin;
                exRange.Value2 = "";

                //Total TM
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn("E", row), new RangeColumn("E", row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                    10, false, false, false);
                exRange.Merge(na);
                exBorders = exRange.Borders[Excel.XlBordersIndex.xlEdgeBottom];
                exBorders.LineStyle = Excel.XlLineStyle.xlContinuous;
                exBorders.Weight = Excel.XlBorderWeight.xlThin;
                exRange.Value2 = tmOverallTotal;

                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn("F", row), new RangeColumn("F", row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                   10, false, false, false);
                exRange.Merge(na);
                exBorders = exRange.Borders[Excel.XlBordersIndex.xlEdgeBottom];
                exBorders.LineStyle = Excel.XlLineStyle.xlContinuous;
                exBorders.Weight = Excel.XlBorderWeight.xlThin;
                exRange.Value2 = "";

                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn("G", row), new RangeColumn("G", row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                   10, false, false, false);
                exRange.Merge(na);
                exBorders = exRange.Borders[Excel.XlBordersIndex.xlEdgeBottom];
                exBorders.LineStyle = Excel.XlLineStyle.xlContinuous;
                exBorders.Weight = Excel.XlBorderWeight.xlThin;
                exRange.Value2 = "";

                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn("H", row), new RangeColumn("H", row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                   10, false, false, false);
                exRange.Merge(na);
                exBorders = exRange.Borders[Excel.XlBordersIndex.xlEdgeBottom];
                exBorders.LineStyle = Excel.XlLineStyle.xlContinuous;
                exBorders.Weight = Excel.XlBorderWeight.xlThin;
                exRange.Value2 = "";
                row++;


                //Aggregate the GrandTotals
                tmGrandTotal += tmOverallTotal;
                verifiedGrandTotal += verifiedOverallTotal;

                row++;
                #endregion

                #endregion

            }
            #endregion

            #region Grand Totals

            #region Individual DTD and TM Grand Totals
            //write Individual Grand Totals  
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn("A", row), new RangeColumn("H", row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
              14, false, true, false);
            exRange.Merge(na);
            exBorders = exRange.Borders[Excel.XlBordersIndex.xlEdgeTop];
            exBorders.LineStyle = Excel.XlLineStyle.xlContinuous;
            exBorders.Weight = Excel.XlBorderWeight.xlThin;
            exRange.Interior.ColorIndex = 15;//grey
            exBorders = exRange.Borders[Excel.XlBordersIndex.xlEdgeBottom];
            exBorders.LineStyle = Excel.XlLineStyle.xlContinuous;
            exBorders.Weight = Excel.XlBorderWeight.xlThin;
            exRange.Interior.ColorIndex = 15;//grey
            exRange.Value2 = "";

            row++;
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn("A", row), new RangeColumn("A", row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                10, false, false, false);
            exRange.Merge(na);
            exRange.Value2 = "";

            //Total Header
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn("B", row), new RangeColumn("B", row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                10, true, false, false);
            exRange.Merge(na);
            exRange.Value2 = "Total";

            //Grand Total Verified
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn("C", row), new RangeColumn("C", row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                10, false, false, false);
            exRange.Merge(na);
            exRange.Value2 = verifiedGrandTotal;

           
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn("D", row), new RangeColumn("D", row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                10, false, false, false);
            exRange.Merge(na);
            exRange.Value2 = "";

            //Grand Total TM
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn("E", row), new RangeColumn("E", row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                10, false, false, false);
            exRange.Merge(na);
            exRange.Value2 = tmGrandTotal;

            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn("F", row), new RangeColumn("F", row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
               10, false, false, false);
            exRange.Merge(na);
            exRange.Value2 = "";

            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn("G", row), new RangeColumn("G", row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
               10, false, false, false);
            exRange.Merge(na);
            exRange.Value2 = "";

            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn("H", row), new RangeColumn("H", row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
               10, false, false, false);
            exRange.Merge(na);
            exRange.Value2 = "";
            row++;
            #endregion

            #region Total Grand Total
            //write Total Grandtotal   
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn("A", row), new RangeColumn("H", row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
              14, false, true, false);
            exRange.Merge(na);
            exBorders = exRange.Borders[Excel.XlBordersIndex.xlEdgeTop];
            exBorders.LineStyle = Excel.XlLineStyle.xlContinuous;
            exBorders.Weight = Excel.XlBorderWeight.xlThin;
            exRange.Interior.ColorIndex = 15;//grey
            exBorders = exRange.Borders[Excel.XlBordersIndex.xlEdgeBottom];
            exBorders.LineStyle = Excel.XlLineStyle.xlContinuous;
            exBorders.Weight = Excel.XlBorderWeight.xlThin;
            exRange.Interior.ColorIndex = 15;//grey
            exRange.Value2 = "";

            row++;

            //Total Header
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn("A", row), new RangeColumn("C", row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                10, true, false, false);
            exRange.Merge(na);
            exRange.Value2 = "Grand Total DTD & TM";

            //Total Verified
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn("D", row), new RangeColumn("E", row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                10, true, false, false);
            exRange.Merge(na);
            exRange.Value2 = verifiedGrandTotal + tmGrandTotal;
            #endregion

            #endregion
        }

            #endregion      
       
        #region Get Data

        #region Get Vendor List (1 method)
        /// <summary>
        /// Gets a list of Vendors
        /// </summary>
        /// <returns>All Vendors excluding the Administrator = 0</returns>
        private static List<Vendor> GetVendors()
        {
            List<Vendor> vendors = new List<Vendor>();
            using (SparkEntities data = new SparkEntities())
            {
                //exclude the Administrator = 0 for VendorId
                vendors = data.Vendors.Where(v => v.IsActive == true).ToList();
            }
            return vendors;
        }
        #endregion

        #region Get UtilityTypes List (1 method)
        /// <summary>
        /// Gets a list of UtilityTypes
        /// </summary>
        /// <returns>All active UtilityTypes</returns>
        private static List<UtilityType> GetUtilityTypes()
        {
            List<UtilityType> utilityTypes = new List<UtilityType>();
            using (SparkEntities data = new SparkEntities())
            {
                //exclude the Administrator = 0 for VendorId
                utilityTypes = data.UtilityTypes.Where(uty => uty.IsActive == true).ToList();
            }
            return utilityTypes;
        }

        #endregion

        #region Get LDCCode List (1 method)
        private static List<string> GetLDCCodeList(DateTime sDate, DateTime eDate, int vendorId, string utilityTypeName)
        {

            List<string> ldcCodeList = new List<string>();

            try
            {
                using (SparkEntities entities = new SparkEntities())
                {

                    //SELECT  ut.LdcCode				
                    //From spark.v1.Main m
                    //  JOIN spark.v1.OrderDetail od on od.MainId = m.MainId
                    //  JOIN spark.v1.Program p on p.ProgramId = od.ProgramId
                    //  JOIN spark.v1.Utility ut on ut.UtilityId = p.UtilityId
                    //  JOIN spark.v1.UtilityType uty on uty.UtilityTypeId = p.UtilityTypeId
                    //  JOIN spark.v1.[User] u on u.UserId = m.UserId
                    //  JOIN spark.v1.UserType usty on u.UserTypeId = usty.UserTypeId
                    //  JOIN spark.v1.Vendor v on v.VendorId = u.VendorId
                    //WHERE  m.Verified = '1'
                    //  AND m.CallDateTime > '8/17/2015' 
                    //  and m.CallDateTime < '8/18/2015'					  
                    //  AND v.VendorId = 11
                    //  AND uty.UtilityTypeName ='Electric'                    
                    //group by ut.LdcCode

                    var query = from m in entities.Mains
                                join od in entities.OrderDetails on m.MainId equals od.MainId
                                join p in entities.Programs on od.ProgramId equals p.ProgramId
                                join ut in entities.Utilities on p.UtilityId equals ut.UtilityId
                                join uty in entities.UtilityTypes on p.UtilityTypeId equals uty.UtilityTypeId
                                join u in entities.Users on m.UserId equals u.UserId
                                join usty in entities.UserTypes on u.UserTypeId equals usty.UserTypeId
                                join v in entities.Vendors on u.VendorId equals v.VendorId
                                where m.CallDateTime > sDate
                                && m.CallDateTime < eDate
                                && m.Verified == "1"
                                && v.VendorId == vendorId
                                && uty.UtilityTypeName == utilityTypeName

                                let k = new
                                {
                                    LDCCode = ut.LdcCode
                                }
                                group m by k into t
                                select new
                                {
                                    LDCCOde = t.Key.LDCCode
                                };

                    foreach (var item in query)
                    {
                        ldcCodeList.Add(item.LDCCOde);
                    }

                }


            }
            catch (Exception ex)
            {
                SendErrorMessage(ex);
                //throw ex;
            }
            return ldcCodeList;


        }
        #endregion

        #region Get Totals (1 method)

        private static int getTotals(DateTime sDate, DateTime eDate, int vendorId, string vendorName, string ldcCode, string utilityTypeName, string userType)
        {

            int total = 0;
            try
            {
                using (SparkEntities entities = new SparkEntities())
                {
                    //SELECT uty.UtilityTypeName, usty.UserTypeName, ut.LdcCode,count(od.MainId) as TotalVerified					
                    //From spark.v1.Main m
                    //  JOIN spark.v1.OrderDetail od on od.MainId = m.MainId
                    //  JOIN spark.v1.Program p on p.ProgramId = od.ProgramId
                    //  JOIN spark.v1.Utility ut on ut.UtilityId = p.UtilityId
                    //  JOIN spark.v1.UtilityType uty on uty.UtilityTypeId = p.UtilityTypeId
                    //  JOIN spark.v1.[User] u on u.UserId = m.UserId
                    //  JOIN spark.v1.UserType usty on u.UserTypeId = usty.UserTypeId
                    //  JOIN spark.v1.Vendor v on v.VendorId = u.VendorId
                    //WHERE  m.Verified = '1'
                    //  AND m.CallDateTime > '8/17/2015' 
                    //  and m.CallDateTime < '8/18/2015'                   
                    //  AND v.VendorId = 11
                    //  AND uty.UtilityTypeName ='Electric' 
                    //  and usty.UserTypeName = 'Door to Door' --and usty.UserTypeName = 'Telesales'
                    //group by uty.UtilityTypeName, usty.UserTypeName, ut.LdcCode

                    var query = (from od in entities.OrderDetails
                                 join m in entities.Mains on od.MainId equals m.MainId
                                 join p in entities.Programs on od.ProgramId equals p.ProgramId
                                 join ut in entities.Utilities on p.UtilityId equals ut.UtilityId
                                 join uty in entities.UtilityTypes on p.UtilityTypeId equals uty.UtilityTypeId
                                 join u in entities.Users on m.UserId equals u.UserId
                                 join usty in entities.UserTypes on u.UserTypeId equals usty.UserTypeId
                                 join v in entities.Vendors on u.VendorId equals v.VendorId
                                 where m.CallDateTime > sDate
                                 && m.CallDateTime < eDate
                                 && m.Verified == "1"
                                 && v.VendorId == vendorId
                                 && ut.LdcCode == ldcCode
                                 && uty.UtilityTypeName == utilityTypeName
                                 && usty.UserTypeName == userType
                                 select od).ToList();


                    total = query.Count();




                }


            }
            catch (Exception ex)
            {
                SendErrorMessage(ex);
                //throw ex;
            }
            return total;
        }
        #endregion

        #endregion

        #region Utilities

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
        private static void SendEmail(ref string xlsFilePath, DateTime reportDate, string strToEmail, string strBccEmail)
        {
            //string strMsgBody = string.Empty;
            try
            {
                //StringBuilder sb = new StringBuilder();

                //sb.AppendLine("You have completed the Investor Verification process.  Attached is a summary report showing all data  ");
                //sb.AppendLine("entered into the investor verification website and a wav.file of your phone verification recording  ");
                //sb.AppendLine("indicating your verbal verification.   ");
                //sb.AppendLine(Environment.NewLine);
                //sb.AppendLine("If you have any questions please call us at (800) 222-2222.");
                //sb.AppendLine(Environment.NewLine);
                //sb.AppendLine("Sincerely,");
                //sb.AppendLine(Environment.NewLine);
                //sb.AppendLine("The Calibrus Verification Team");
                //strMsgBody = sb.ToString();

                SmtpMail mail = new SmtpMail("TMPWEB1", false);

                mail.AddAttachment(xlsFilePath);//Attach XLS report
                mail.AddRecipient(strToEmail, RecipientType.To);
                mail.AddRecipient(strBccEmail, RecipientType.Bcc);


                mail.From = "reports1@calibrus.com";

                mail.Subject = "Spark Energy Daily Executive Report for " + reportDate.ToString("MMM") + " " + reportDate.ToString("dd") + " " + reportDate.ToString("yyyy") + ".";


                //mail.Body = strMsgBody;
                mail.SendMessage();

            }
            catch (Exception ex)
            {
                SendErrorMessage(ex);
            }

        }

        /// <summary>
        /// Saves XLS workbook document to a folder in the reportPath
        /// </summary>
        /// <param name="mainRecord"></param>
        /// <param name="reportPath"></param>
        /// <param name="xlsFilename"></param>
        /// <param name="xlsFilePath"></param>
        /// <param name="exBook"></param>
        private static void SaveXlsDocument(ref string reportPath, ref string xlsFilename, ref string xlsFilePath, Excel.Workbook exBook, DateTime reportDate)
        {
            //Build the file name
            xlsFilename = "SparkDailyExecutiveReport" + String.Format("{0:yyyyMMdd}", reportDate) + ".xlsx";

            xlsFilePath = string.Format(reportPath + xlsFilename);
            bool fileExists = File.Exists(xlsFilePath);
            if (fileExists)
            {
                //delete it
                File.Delete(xlsFilePath);
            }
            //save workbook
            exBook.SaveAs(Filename: xlsFilePath);
        }

        private static void CopyFileAndMove(string getfile, string putfile, ref string xlsFilename)
        {

            getfile += xlsFilename;
            putfile += xlsFilename;
            try
            {
                bool fileExists = File.Exists(putfile);
                if (fileExists)
                {
                    //delete it
                    File.Delete(putfile);
                }
                //move the file to the processed directory
                File.Copy(String.Format(@"{0}", getfile), String.Format(@"{0}", putfile));
            }
            catch (Exception ex)
            { throw ex; }
        }

        private static void GetDates(out DateTime CurrentDate)
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
            CurrentDate = new DateTime(baseDate.Year, baseDate.Month, baseDate.Day, 0, 0, 0);//current date time

        }

        private static void SendErrorMessage(Exception ex)
        {
            Calibrus.ErrorHandler.Alerting alert = new Calibrus.ErrorHandler.Alerting("SparkDailyExecutiveReport");
            alert.SendAlert(ex.Source, ex.Message, Environment.MachineName, Environment.UserName, Environment.Version.ToString());
        }
        #endregion
    }
}
