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

namespace SparkDailyActivityReportXLS
{
    public class DailyActivityReport
    {
        public enum ReportType
        {
            Daily,
            MTD,
            YTD
        }

        public static Dictionary<string, string> Channel = new Dictionary<string, string>()
        {
            {"D2D English", "1324"},
            {"D2D Spanish", "1325"},
            {"Res Outbound English", "1322"},
            {"Res Outbound Spanish", "1323"}
        };

        public static object na = System.Reflection.Missing.Value;

        #region Main
        public static void Main(string[] args)
        {
            string rootPath = string.Empty;
            string mailRecipientTO = string.Empty;


            //get report interval
            DateTime CurrentDate = new DateTime();
            DateTime WeekStartDate = new DateTime();
            DateTime MonthStartDate = new DateTime();
            DateTime QuarterlyStartDate = new DateTime();
            DateTime YearStartDate = new DateTime();


            //start to  build the form pathing
            string xlsFilename = string.Empty;
            string xlsFilePath = string.Empty;

            if (args.Length > 0)
            {
                if (DateTime.TryParse(args[0], out CurrentDate))
                {
                    GetDates(out CurrentDate, out MonthStartDate, out YearStartDate);
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
                GetDates(out CurrentDate, out MonthStartDate, out YearStartDate);
            }

            //grab values from app.config
            rootPath = ConfigurationManager.AppSettings["rootPath"].ToString();
            mailRecipientTO = ConfigurationManager.AppSettings["mailRecipientTO"].ToString();

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
                #region WriteReportForReportTypeAndDates Tab
                foreach (ReportType reportType in Enum.GetValues(typeof(ReportType)))
                {
                    if (sheetsAdded < exBook.Sheets.Count)
                    {
                        exSheet = (Excel.Worksheet)exBook.Sheets[sheetsAdded + 1];
                    }
                    else
                    {
                        exSheet = (Excel.Worksheet)exBook.Sheets.Add(na, exBook.ActiveSheet, na, na);
                    }

                    string sheetName = reportType.ToString();
                    exSheet.Name = sheetName.Length > 30 ? sheetName.Substring(0, 30) : sheetName; //force length of sheet name due to excel constraints
                    exSheet.Select(na);

                    sheetsAdded++;

                    //write report based on Report type period and a starting date
                    string strHeader = string.Empty;
                    switch (reportType.ToString())
                    {
                        case "Daily":
                            strHeader = string.Format("TPV Daily Activity Report for {0:MMMM d, yyyy}", CurrentDate.AddDays(-1));
                            WriteReportForReportTypeAndDates(ref exApp, ref exRange, CurrentDate.AddDays(-1), CurrentDate, reportType.ToString(), strHeader);
                            break;
                        case "MTD":
                            strHeader = "Month to Date: TPV Daily Activity Report";
                            WriteReportForReportTypeAndDates(ref exApp, ref exRange, MonthStartDate, MonthStartDate.AddMonths(1), reportType.ToString(), strHeader);
                            break;
                        case "YTD":
                            strHeader = "Year to Date: TPV Daily Activity Report";
                            WriteReportForReportTypeAndDates(ref exApp, ref exRange, YearStartDate, YearStartDate.AddYears(1), reportType.ToString(), strHeader);
                            break;

                    }

                    //Autosize the columns, not sure if this will ever get to column Z, but this will ensure that the format is for all written columns
                    exRange = (Excel.Range)exApp.get_Range("A1", "Z1");
                    exRange.EntireColumn.AutoFit();
                }
                #endregion

                #region WriteReportForNoSalesMTDDetail Tab
                #endregion

                #region MTDSalesRepByDisposition Tab
                //Write One Tab of No Sales - MT Detail for the MTD date range.
                if (sheetsAdded < exBook.Sheets.Count)
                {
                    exSheet = (Excel.Worksheet)exBook.Sheets[sheetsAdded + 1];
                }
                else
                {
                    exSheet = (Excel.Worksheet)exBook.Sheets.Add(na, exBook.ActiveSheet, na, na);
                }


                exSheet.Name = "MTD Sales Rep by Disposition";
                exSheet.Select(na);

                sheetsAdded++;
                WriteReportForMTDSalesRepByDisposition(ref exApp, ref exRange, MonthStartDate, MonthStartDate.AddMonths(1));


                //Auto size the columns
                exRange = (Excel.Range)exApp.get_Range("A1", "Z1");
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
            SendEmail(ref xlsFilePath, CurrentDate.AddDays(-1), mailRecipientTO);

        }
        #endregion

        #region Excel

        #region WriteReportForReportTypeAndDates_EXCEL
        public static void WriteReportForReportTypeAndDates(ref Excel.Application exApp, ref Excel.Range exRange, DateTime startDate, DateTime endDate, string reportType, string header)
        {
            int rowInitialize = 1; //initial seed for the row data
            int row = 0;// where we start the row data

            int headerColumnInitialize = 66; //initial seed for column header - column  B
            int dataColumnInitialize = 67; //initial seed for column data - column  C
            int col = 0;

            row = rowInitialize;  //set the row for the data   
            col = dataColumnInitialize;//set the column for the data

            #region Header

            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn("A", row), new RangeColumn("C", row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                14, true, false, false);
            exRange.Merge(na);
            exRange.Font.Underline = true;
            exRange.Interior.ColorIndex = 15;//grey
            exRange.Value2 = header;

            row++;

            #endregion

            #region Sales Overall
            int TotalVerificationsPerformed_Overall = 0;
            int TotalGoodSale_Overall = 0;

            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn("A", row), new RangeColumn("C", row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                14, true, false, false);
            exRange.Merge(na);
            exRange.Interior.ColorIndex = 15;//grey
            exRange.Value2 = "Sales Overall";

            row++;

            //Total Verifications performed
            TotalVerificationsPerformed_Overall = GetTotalVerificationsPerformed_Overall(startDate, endDate);
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn("B", row), new RangeColumn("B", row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight,
                 11, true, false, false);
            exRange.Value2 = "Total Verifications performed";


            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn("C", row), new RangeColumn("C", row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = TotalVerificationsPerformed_Overall;
            row++;

            //Good Sale
            TotalGoodSale_Overall = GetTotalGoodSales_Overall(startDate, endDate);
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn("B", row), new RangeColumn("B", row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight,
                11, false, false, false);
            exRange.Value2 = "Good Sale";


            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn("C", row), new RangeColumn("C", row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, false, false, false);
            exRange.Value2 = TotalGoodSale_Overall;

            row++;

            //No Sale
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn("B", row), new RangeColumn("B", row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight,
                11, false, false, false);
            exRange.Value2 = "No Sale";
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn("C", row), new RangeColumn("C", row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, false, false, false);
            exRange.NumberFormat = "0";
            exRange.Formula = string.Format("=IFERROR({0}{1}-{0}{2}, \"0.00%\" )", "C", row - 2, row - 1);

            row++;

            //Good Sale %
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn("B", row), new RangeColumn("B", row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight,
                11, false, false, false);
            exRange.Value2 = "Good Sale %";

            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn("C", row), new RangeColumn("C", row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, false, false, false);
            exRange.NumberFormat = "0.00%";
            exRange.Formula = string.Format("=IFERROR({0}{1}/{0}{2}, \"0.00%\" )", "C", row - 2, row - 3);

            row++;

            //No Sale %
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn("B", row), new RangeColumn("B", row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight,
                11, false, false, false);
            exRange.Value2 = "No Sale %";

            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn("C", row), new RangeColumn("C", row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
               11, false, false, false);
            exRange.NumberFormat = "0.00%";
            exRange.Formula = string.Format("=IFERROR({0}{1}/{0}{2}, \"0.00%\" )", "C", row - 2, row - 4);

            row++;
            row++;
            #endregion

            #region Sales by Channel
            int TotalVerificationsPerformed_ByChannel = 0;
            int TotalGoodSale_ByChannel = 0;
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn("A", row), new RangeColumn("B", row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                14, true, false, false);
            exRange.Merge(na);
            exRange.Interior.ColorIndex = 15;//grey
            exRange.Value2 = "Sales by Channel";

            foreach (var pair in Channel)
            {
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                   14, true, false, false);
                exRange.Interior.ColorIndex = 15;//grey
                exRange.Value2 = pair.Key;

                col++;
            }

            col = dataColumnInitialize;//reset column back to C
            row++;

            //Total Verifications performed            
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn("B", row), new RangeColumn("B", row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight,
                 11, true, false, false);
            exRange.Value2 = "Total Verifications performed";

            foreach (var pair in Channel)
            {
                TotalVerificationsPerformed_ByChannel = GetTotalVerificationsPerformed_ByChannel(startDate, endDate, pair.Value);
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                  11, true, false, false);
                exRange.Value2 = TotalVerificationsPerformed_ByChannel;

                col++;
            }

            col = dataColumnInitialize;//reset column back to C
            row++;

            //Good Sale
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn("B", row), new RangeColumn("B", row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight,
                11, false, false, false);
            exRange.Value2 = "Good Sale";

            foreach (var pair in Channel)
            {
                TotalGoodSale_ByChannel = GetTotalGoodSales_ByChannel(startDate, endDate, pair.Value);
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = TotalGoodSale_ByChannel;

                col++;
            }

            col = dataColumnInitialize;//reset column back to C
            row++;

            //No Sale
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn("B", row), new RangeColumn("B", row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight,
                11, false, false, false);
            exRange.Value2 = "No Sale";

            foreach (var pair in Channel)
            {
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.NumberFormat = "0";
                exRange.Formula = string.Format("=IFERROR({0}{1}-{0}{2}, \"0.00%\" )", ConvertColumn(col), row - 2, row - 1);

                col++;
            }

            col = dataColumnInitialize;//reset column back to C
            row++;

            //Good Sale %
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn("B", row), new RangeColumn("B", row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight,
                11, false, false, false);
            exRange.Value2 = "Good Sale %";

            foreach (var pair in Channel)
            {
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.NumberFormat = "0.00%";
                exRange.Formula = string.Format("=IFERROR({0}{1}/{0}{2}, \"0.00%\" )", ConvertColumn(col), row - 2, row - 3);

                col++;
            }

            col = dataColumnInitialize;//reset column back to C
            row++;

            //No Sale %
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn("B", row), new RangeColumn("B", row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight,
                11, false, false, false);
            exRange.Value2 = "No Sale %";

            foreach (var pair in Channel)
            {
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                   11, false, false, false);
                exRange.NumberFormat = "0.00%";
                exRange.Formula = string.Format("=IFERROR({0}{1}/{0}{2}, \"0.00%\" )", ConvertColumn(col), row - 2, row - 4);

                col++;
            }

            col = dataColumnInitialize;//reset column back to C
            row++;
            row++;

            #endregion

            #region Sales by Utility
            int TotalVerificationsPerformed_ByUtility = 0;
            int TotalGoodSale_ByUtility = 0;
            Dictionary<string, int> UtilitiesList = new Dictionary<string, int>(); //reused below for Utility Dispositions

            UtilitiesList = GetUtilities(startDate, endDate);

            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn("A", row), new RangeColumn("B", row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                14, true, false, false);
            exRange.Merge(na);
            exRange.Interior.ColorIndex = 15;//grey
            exRange.Value2 = "Sales by Utility";

            foreach (KeyValuePair<string, int> pair in UtilitiesList)
            {
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                  14, true, false, false);
                exRange.Interior.ColorIndex = 15;//grey
                exRange.Value2 = pair.Key.ToString();

                col++;
            }

            col = dataColumnInitialize;//reset column back to C
            row++;

            //Total Verifications performed            
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn("B", row), new RangeColumn("B", row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight,
                 11, true, false, false);
            exRange.Value2 = "Total Verifications performed";

            foreach (KeyValuePair<string, int> pair in UtilitiesList)
            {
                TotalVerificationsPerformed_ByUtility = GetTotalVerificationsPerformed_ByUtility(startDate, endDate, pair.Value);
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                  11, true, false, false);
                exRange.Value2 = TotalVerificationsPerformed_ByUtility;

                col++;
            }

            col = dataColumnInitialize;//reset column back to C
            row++;

            //Good Sale
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn("B", row), new RangeColumn("B", row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight,
                11, false, false, false);
            exRange.Value2 = "Good Sale";

            foreach (KeyValuePair<string, int> pair in UtilitiesList)
            {
                TotalGoodSale_ByUtility = GetTotalGoodSales_ByUtility(startDate, endDate, pair.Value);
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = TotalGoodSale_ByUtility;

                col++;
            }

            col = dataColumnInitialize;//reset column back to C
            row++;

            //No Sale
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn("B", row), new RangeColumn("B", row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight,
                11, false, false, false);
            exRange.Value2 = "No Sale";

            foreach (KeyValuePair<string, int> pair in UtilitiesList)
            {
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.NumberFormat = "0";
                exRange.Formula = string.Format("=IFERROR({0}{1}-{0}{2}, \"0.00%\" )", ConvertColumn(col), row - 2, row - 1);

                col++;
            }

            col = dataColumnInitialize;//reset column back to C
            row++;

            //Good Sale %
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn("B", row), new RangeColumn("B", row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight,
                11, false, false, false);
            exRange.Value2 = "Good Sale %";

            foreach (KeyValuePair<string, int> pair in UtilitiesList)
            {
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.NumberFormat = "0.00%";
                exRange.Formula = string.Format("=IFERROR({0}{1}/{0}{2}, \"0.00%\" )", ConvertColumn(col), row - 2, row - 3);

                col++;
            }

            col = dataColumnInitialize;//reset column back to C
            row++;

            //No Sale %
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn("B", row), new RangeColumn("B", row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight,
                11, false, false, false);
            exRange.Value2 = "No Sale %";

            foreach (KeyValuePair<string, int> pair in UtilitiesList)
            {
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                   11, false, false, false);
                exRange.NumberFormat = "0.00%";
                exRange.Formula = string.Format("=IFERROR({0}{1}/{0}{2}, \"0.00%\" )", ConvertColumn(col), row - 2, row - 4);

                col++;
            }

            col = dataColumnInitialize;//reset column back to C
            row++;
            row++;
            #endregion

            #region Disposition by Utility
            int DispositionCount_ByUtility = 0;
            int DispositionCountTotal_ByUtility = 0;
            List<Disposition> DispositionList = GetDispositionList(startDate, endDate); //reused below for Vendor Dispositions
            //int dispositionByUtilityTotal = DispositionList.Select(c => c.Count).Sum();

            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn("A", row), new RangeColumn("B", row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
               14, true, false, false);
            exRange.Merge(na);
            exRange.Interior.ColorIndex = 15;//grey
            exRange.Value2 = "Disposition by Utility";

            foreach (KeyValuePair<string, int> pair in UtilitiesList)
            {
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                   14, true, false, false);
                exRange.Interior.ColorIndex = 15;//grey
                exRange.Value2 = pair.Key.ToString();

                col++;
            }

            col = dataColumnInitialize;//reset column back to C
            row++;

            foreach (var disposition in DispositionList)
            {
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn("B", row), new RangeColumn("B", row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight,
                    11, false, false, false);
                exRange.Value2 = disposition.Concern;
                row++;
            }
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn("B", row), new RangeColumn("B", row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight,
                    11, true, false, false);
            exRange.Value2 = "Total";


            //roll back the rows to start for the data write
            foreach (var disposition in DispositionList)
            {
                row--;
            }

            foreach (KeyValuePair<string, int> pair in UtilitiesList)
            {
                foreach (var disposition in DispositionList)
                {
                    //pass in date state and disposition
                    DispositionCount_ByUtility = GetDispositionCount_ByUtility(startDate, endDate, pair.Value, disposition.Concern);
                    DispositionCountTotal_ByUtility += DispositionCount_ByUtility;
                    exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                       11, false, false, false);
                    exRange.Value2 = DispositionCount_ByUtility;
                    row++;
                }

                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, true, false, false);
                exRange.Value2 = DispositionCountTotal_ByUtility;
                //roll back the rows to start for the data write
                foreach (var disposition in DispositionList)
                {
                    row--;
                }

                DispositionCountTotal_ByUtility = 0; //reset count to 0
                col++;
            }
            //move forward the rows to start for the next section
            foreach (var disposition in DispositionList)
            {
                row++;
            }


            col = dataColumnInitialize;//reset column back to C
            row++;
            row++;

            #endregion

            #region Sales by Vendor
            int TotalVerificationsPerformed_ByVendor = 0;
            int TotalGoodSale_ByVendor = 0;

            List<Vendor> VendorList = new List<Vendor>(); //reused below for Vendor Dispositions

            VendorList = GetVendors();

            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn("A", row), new RangeColumn("B", row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
             14, true, false, false);
            exRange.Merge(na);
            exRange.Interior.ColorIndex = 15;//grey
            exRange.Value2 = "Sales by Vendor";

            foreach (Vendor vendor in VendorList)
            {
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                   14, true, false, false);
                exRange.Interior.ColorIndex = 15;//grey
                exRange.Value2 = vendor.VendorName;

                col++;
            }

            col = dataColumnInitialize;//reset column back to C
            row++;

            //Total Verifications performed            
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn("B", row), new RangeColumn("B", row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight,
                 11, true, false, false);
            exRange.Value2 = "Total Verifications performed";

            foreach (Vendor vendor in VendorList)
            {
                TotalVerificationsPerformed_ByVendor = GetTotalVerificationsPerformed_ByVendor(startDate, endDate, vendor.VendorId);
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                  11, true, false, false);
                exRange.Value2 = TotalVerificationsPerformed_ByVendor;

                col++;
            }

            col = dataColumnInitialize;//reset column back to C
            row++;

            //Good Sale
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn("B", row), new RangeColumn("B", row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight,
                11, false, false, false);
            exRange.Value2 = "Good Sale";

            foreach (Vendor vendor in VendorList)
            {
                TotalGoodSale_ByVendor = GetTotalGoodSales_ByVendor(startDate, endDate, vendor.VendorId);
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = TotalGoodSale_ByVendor;

                col++;
            }

            col = dataColumnInitialize;//reset column back to C
            row++;

            //No Sale
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn("B", row), new RangeColumn("B", row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight,
                11, false, false, false);
            exRange.Value2 = "No Sale";

            foreach (Vendor vendor in VendorList)
            {
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.NumberFormat = "0";
                exRange.Formula = string.Format("=IFERROR({0}{1}-{0}{2}, \"0.00%\" )", ConvertColumn(col), row - 2, row - 1);

                col++;
            }

            col = dataColumnInitialize;//reset column back to C
            row++;

            //Good Sale %
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn("B", row), new RangeColumn("B", row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight,
                11, false, false, false);
            exRange.Value2 = "Good Sale %";

            foreach (Vendor vendor in VendorList)
            {
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.NumberFormat = "0.00%";
                exRange.Formula = string.Format("=IFERROR({0}{1}/{0}{2}, \"0.00%\" )", ConvertColumn(col), row - 2, row - 3);

                col++;
            }

            col = dataColumnInitialize;//reset column back to C
            row++;

            //No Sale %
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn("B", row), new RangeColumn("B", row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight,
                11, false, false, false);
            exRange.Value2 = "No Sale %";

            foreach (Vendor vendor in VendorList)
            {
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                   11, false, false, false);
                exRange.NumberFormat = "0.00%";
                exRange.Formula = string.Format("=IFERROR({0}{1}/{0}{2}, \"0.00%\" )", ConvertColumn(col), row - 2, row - 4);

                col++;
            }

            col = dataColumnInitialize;//reset column back to C
            row++;
            row++;
            #endregion

            #region Disposition by Vendor
            int DispositionCount_ByVendor = 0;
            int DispositionCountTotal_ByVendor = 0;

            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn("A", row), new RangeColumn("B", row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
             14, true, false, false);
            exRange.Merge(na);
            exRange.Interior.ColorIndex = 15;//grey
            exRange.Value2 = "Disposition by Vendor";

            foreach (Vendor vendor in VendorList)
            {
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                   14, true, false, false);
                exRange.Interior.ColorIndex = 15;//grey
                exRange.Value2 = vendor.VendorName;

                col++;
            }

            col = dataColumnInitialize;//reset column back to C
            row++;

            foreach (var disposition in DispositionList)
            {
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn("B", row), new RangeColumn("B", row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight,
                    11, false, false, false);
                exRange.Value2 = disposition.Concern;
                row++;
            }
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn("B", row), new RangeColumn("B", row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight,
                    11, true, false, false);
            exRange.Value2 = "Total";


            //roll back the rows to start for the data write
            foreach (var disposition in DispositionList)
            {
                row--;
            }

            foreach (Vendor vendor in VendorList)
            {
                foreach (var disposition in DispositionList)
                {
                    //pass in date state and disposition
                    DispositionCount_ByVendor = GetDispositionCount_ByVendor(startDate, endDate, vendor.VendorId, disposition.Concern);
                    DispositionCountTotal_ByVendor += DispositionCount_ByVendor;
                    exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                       11, false, false, false);
                    exRange.Value2 = DispositionCount_ByVendor;
                    row++;
                }

                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, true, false, false);
                exRange.Value2 = DispositionCountTotal_ByVendor;
                //roll back the rows to start for the data write
                foreach (var disposition in DispositionList)
                {
                    row--;
                }

                DispositionCountTotal_ByVendor = 0; //reset count to 0
                col++;
            }
            //move forward the rows to start for the next section
            foreach (var disposition in DispositionList)
            {
                row++;
            }


            col = dataColumnInitialize;//reset column back to C
            row++;
            row++;
            #endregion

            #region Non-Verified By Disposition (Overall)
            //int DispositionCountTotal_Overall = 0;
            List<Disposition> NonVerifiedDispositionList = GetNonVerifiedDispositionList(startDate, endDate);
            int DispositionCountTotal_Overall = NonVerifiedDispositionList.Select(c => c.Count).Sum();

            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn("A", row), new RangeColumn("B", row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
             14, true, false, false);
            exRange.Merge(na);
            exRange.Interior.ColorIndex = 15;//grey
            exRange.Value2 = "Non-Verified By Disposition (Overall)";

            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn("C", row), new RangeColumn("C", row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                14, true, false, false);
            exRange.Merge(na);
            exRange.Interior.ColorIndex = 15;//grey
            exRange.Value2 = "Total";

            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn("D", row), new RangeColumn("D", row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                14, true, false, false);
            exRange.Merge(na);
            exRange.Interior.ColorIndex = 15;//grey
            exRange.Value2 = "Percentage";
            row++;

            foreach (var nonverifieddisposition in NonVerifiedDispositionList)
            {
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn("B", row), new RangeColumn("B", row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight,
                    11, false, false, false);
                exRange.Value2 = nonverifieddisposition.Concern;
                row++;
            }
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn("B", row), new RangeColumn("B", row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight,
                    11, true, false, false);
            exRange.Value2 = "Total";

            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn("C", row), new RangeColumn("C", row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, true, false, false);
            exRange.Formula = DispositionCountTotal_Overall;

            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn("D", row), new RangeColumn("D", row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, true, false, false);
            exRange.NumberFormat = "0.00%";
            exRange.Formula = string.Format("=IFERROR({0}/{1}, \"0.00%\" )", DispositionCountTotal_Overall, DispositionCountTotal_Overall);

            //roll back the rows to start for the data write
            foreach (var nonverifieddisposition in NonVerifiedDispositionList)
            {
                row--;
            }


            foreach (var nonverifieddisposition in NonVerifiedDispositionList)
            {
                //DispositionCountTotal_Overall += nonverifieddisposition.Count;
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                   11, false, false, false);
                exRange.Value2 = nonverifieddisposition.Count;
                col++;
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                   11, false, false, false);
                exRange.NumberFormat = "0.00%";
                exRange.Formula = string.Format("=IFERROR({0}/{1}, \"0.00%\" )", nonverifieddisposition.Count, DispositionCountTotal_Overall);
                col--;
                row++;
            }

            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = DispositionCountTotal_Overall;
            //roll back the rows to start for the data write
            foreach (var nonverifieddisposition in NonVerifiedDispositionList)
            {
                row--;
            }

            //DispositionCountTotal_ByVendor = 0; //reset count to 0
            col++;

            //move forward the rows to start for the next section
            foreach (var nonverifieddisposition in NonVerifiedDispositionList)
            {
                row++;
            }


            col = dataColumnInitialize;//reset column back to C
            row++;
            row++;
            #endregion
        }
        #endregion


        #region WriteReportForMTDSalesRepByDisposition_EXCEL
        public static void WriteReportForMTDSalesRepByDisposition(ref Excel.Application exApp, ref Excel.Range exRange, DateTime startDate, DateTime endDate)
        {
            int rowInitialize = 1; //initial seed for the row data
            int row = 0;// where we start the row data  

            int dataColumnInitialize = 65; //initial seed for column data - column  A
            int col = 0;

            row = rowInitialize;  //set the row for the data   
            col = dataColumnInitialize;//set the column for the data

            List<Vendor> VendorList = GetVendors();

            List<Disposition> NonVerifiedDispositionList = GetNonVerifiedDispositionList(startDate, endDate);

            foreach (Vendor vendor in VendorList)
            {
                #region Header
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn("A", row), new RangeColumn("A", row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                      18, true, false, false);
                exRange.Merge(na);
                exRange.Interior.ColorIndex = 15;//grey
                exRange.Value2 = vendor.VendorName;

                col++;

                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                     11, true, false, false);
                exRange.Merge(na);
                exRange.Interior.ColorIndex = 15;//grey
                exRange.Value2 = "Grand Total";

                col++;

                foreach (var nonverifieddisposition in NonVerifiedDispositionList.OrderBy(nd => nd.Concern))
                {
                    exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                        11, true, false, false);
                    exRange.Merge(na);
                    exRange.Interior.ColorIndex = 15;//grey
                    exRange.Value2 = nonverifieddisposition.Concern;

                    col++;
                }

                col = dataColumnInitialize;//reset column back to A
                row++;
                #endregion

                #region Sales Rep Data Totals
                //Get list of Sales Reps for a Vendor and DateRange
                List<SalesRep> SalesRepList = GetMTDSalesRepByDisposition(startDate, endDate, vendor.VendorId, NonVerifiedDispositionList);
                if (SalesRepList.Count() > 0)
                {
                    foreach (SalesRep mySalesRep in SalesRepList.OrderByDescending(sr => sr.TotalConcernCount))
                    {
                        //output data for SalesRep
                        exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                               11, false, false, false);
                        exRange.Merge(na);
                        exRange.Value2 = mySalesRep.FirstName + " " + mySalesRep.LastName;

                        col++;

                        //Grand Total Column
                        exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                              11, true, false, false);
                        exRange.Merge(na);
                        exRange.Value2 = mySalesRep.TotalConcernCount;

                        col++;

                        foreach (var nonverifieddisposition in NonVerifiedDispositionList.OrderBy(nd => nd.Concern))
                        {
                            int? dispositionCount = null;

                            foreach (Disposition salesRepDispositions in mySalesRep.Dispositions)
                            {

                                if (salesRepDispositions.Concern == nonverifieddisposition.Concern)
                                {
                                    dispositionCount = salesRepDispositions.Count;
                                }

                            }
                            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                                    11, false, false, false);
                            exRange.Merge(na);
                            exRange.Value2 = dispositionCount;

                            col++;
                        }

                        col = dataColumnInitialize;//reset column back to A
                        row++;
                    }
                #endregion
                    #region Grand Totals
                    exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                        11, true, false, false);
                    exRange.Merge(na);
                    exRange.Value2 = "Grand Total";

                    col++;

                    //Grand Total
                    exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                        11, true, false, false);
                    exRange.Merge(na);
                    exRange.NumberFormat = "0";
                    exRange.Formula = string.Format("=SUM({0}{1}:{0}{2})", ConvertColumn(col), row - SalesRepList.Count(), row - 1);

                    col++;

                    foreach (var nonverifieddisposition in NonVerifiedDispositionList.OrderBy(nd => nd.Concern))
                    {
                        //Grand Totals for Dispositions
                        exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                        11, true, false, false);
                        exRange.Merge(na);
                        exRange.NumberFormat = "0";
                        exRange.Formula = string.Format("=SUM({0}{1}:{0}{2})", ConvertColumn(col), row - SalesRepList.Count(), row - 1);

                        col++;
                    }


                }
                row++;

                col = dataColumnInitialize;//reset column back to A
                row++;

                    #endregion

            }
        }
        #endregion

        #endregion

        #region Get Data

        #region Get Utility List (1 method)

        private static Dictionary<string, int> GetUtilities(DateTime startDate, DateTime endDate)
        {
            Dictionary<string, int> utilities = new Dictionary<string, int>();
            using (SparkEntities entities = new SparkEntities())
            {

                var query = (from u in entities.Utilities
                             where u.IsActive == true
                             select u).Distinct().OrderBy(m => m.LdcCode).ToList();

                foreach (var item in query)
                {
                    utilities.Add(item.LdcCode, item.UtilityId);
                }
            }
            return utilities;
        }

        #endregion

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

        #region Get Disposition List (2 methods)
        private static List<Disposition> GetDispositionList(DateTime startDate, DateTime endDate)
        {

            //SELECT COUNT(MainId), Concern
            //FROM [Spark].[v1].[Main]
            //WHERE CallDateTime > startDate and CallDateTime < endDate
            //AND Concern is not null
            //GROUP BY Concern
            //ORDER BY Concern

            List<Disposition> dispositionList = new List<Disposition>();

            try
            {
                using (SparkEntities entities = new SparkEntities())
                {
                    var query = (from m in entities.Mains
                                 where m.CallDateTime > startDate
                                  && m.CallDateTime < endDate
                                  && m.Concern != null
                                 group m by m.Concern into concernList
                                 select new
                                 {
                                     Concern = concernList.Key,
                                     Count = concernList.Count()
                                 }).OrderByDescending(group => group.Concern);

                    foreach (var concernItem in query)
                    {
                        Disposition myDisposition = new Disposition();

                        myDisposition.Concern = concernItem.Concern;
                        myDisposition.Count = concernItem.Count;

                        dispositionList.Add(myDisposition);
                    }

                }

            }
            catch (Exception ex)
            {
                SendErrorMessage(ex);
                //throw ex;
            }
            return dispositionList;
        }

        private static List<Disposition> GetNonVerifiedDispositionList(DateTime startDate, DateTime endDate)
        {

            //SELECT COUNT(MainId), Concern
            //FROM [Spark].[v1].[Main]
            //WHERE CallDateTime > startDate and CallDateTime < endDate
            //AND Concern is not null
            //AND Verified != '1'
            //GROUP BY Concern
            //ORDER BY Concern

            List<Disposition> nonVerifiedDispositionList = new List<Disposition>();

            try
            {
                using (SparkEntities entities = new SparkEntities())
                {
                    var query = (from m in entities.Mains
                                 where m.CallDateTime > startDate
                                  && m.CallDateTime < endDate
                                  && m.Concern != null
                                  && m.Verified != "1"
                                 group m by m.Concern into concernList
                                 select new
                                 {
                                     Concern = concernList.Key,
                                     Count = concernList.Count()
                                 }).OrderByDescending(group => group.Concern);

                    foreach (var concernItem in query)
                    {
                        Disposition myDisposition = new Disposition();

                        myDisposition.Concern = concernItem.Concern;
                        myDisposition.Count = concernItem.Count;

                        nonVerifiedDispositionList.Add(myDisposition);
                    }

                }

            }
            catch (Exception ex)
            {
                SendErrorMessage(ex);
                //throw ex;
            }
            return nonVerifiedDispositionList;
        }
        #endregion

        #region Overall Data (2 methods)

        #region Gets Total Verifications performed for a date range
        private static int GetTotalVerificationsPerformed_Overall(DateTime startDate, DateTime endDate)
        {
            //SELECT count(mainid) AS [Total Verifications Performed]
            //FROM [Spark].[v1].[Main]
            //WHERE calldatetime > startDate and calldatetime < endDate
            //AND concern is not null
            int total = 0;
            try
            {
                using (SparkEntities entities = new SparkEntities())
                {
                    var query = (from m in entities.Mains
                                 where m.CallDateTime > startDate
                                  && m.CallDateTime < endDate
                                  && m.Concern != null
                                 select m);

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

        #region Gets All Good Sales performed for a date range
        private static int GetTotalGoodSales_Overall(DateTime startDate, DateTime endDate)
        {
            //SELECT count(mainid) AS [Good Sale]
            //FROM [Spark].[v1].[Main]
            //WHERE calldatetime > startDate and calldatetime < endDate
            //AND concern ='1'
            //AND verified ='1'
            int total = 0;
            try
            {
                using (SparkEntities entities = new SparkEntities())
                {
                    var query = (from m in entities.Mains
                                 where m.CallDateTime > startDate
                                 && m.CallDateTime < endDate
                                 && m.Concern != null
                                 && m.Verified == "1"
                                 select m);

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

        #region By Channel Data (2 methods)
        #region Gets Total Verifications performed for a date range by channel
        private static int GetTotalVerificationsPerformed_ByChannel(DateTime startDate, DateTime endDate, string dnis)
        {
            //SELECT count(mainid) AS [Total Verifications Performed]
            //FROM [Spark].[v1].[Main]
            //WHERE calldatetime > startDate and calldatetime < endDate
            //AND dnis = dnis
            //AND concern is not null
            int total = 0;
            try
            {
                using (SparkEntities entities = new SparkEntities())
                {
                    var query = (from m in entities.Mains
                                 where m.CallDateTime > startDate
                                  && m.CallDateTime < endDate
                                  && m.Dnis == dnis
                                  && m.Concern != null
                                 select m);

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


        #region Gets All Good Sales performed for a date range by channel
        private static int GetTotalGoodSales_ByChannel(DateTime startDate, DateTime endDate, string dnis)
        {
            //SELECT count(mainid) AS [Good Sale]
            //FROM [Spark].[v1].[Main]
            //WHERE calldatetime > startDate and calldatetime < endDate
            //AND dnis = dnis
            //AND concern ='1'
            //AND verified ='1'
            int total = 0;
            try
            {
                using (SparkEntities entities = new SparkEntities())
                {
                    var query = (from m in entities.Mains
                                 where m.CallDateTime > startDate
                                 && m.CallDateTime < endDate
                                 && m.Dnis == dnis
                                 && m.Concern != null
                                 && m.Verified == "1"
                                 select m);

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

        #region By Utility Data (2 methods)
        #region Gets Total Verifications performed for a date range by Utility
        private static int GetTotalVerificationsPerformed_ByUtility(DateTime startDate, DateTime endDate, int utilityId)
        {
            //SELECT count(m.mainid) AS [Total Verifications Performed]
            //FROM [Spark].[v1].[Main] m
            //join [Spark].[v1].[OrderDetail] od on od.MainId = m.MainId
            //join [Spark].[v1].[Program] p on od.ProgramId = p.ProgramId
            //WHERE m.calldatetime > startDate  and m.calldatetime < endDate 
            //AND m.concern is not null
            //and p.UtilityId = utilityId
            int total = 0;
            try
            {
                using (SparkEntities entities = new SparkEntities())
                {
                    var query = (from m in entities.Mains
                                 join od in entities.OrderDetails on m.MainId equals od.MainId
                                 join p in entities.Programs on od.ProgramId equals p.ProgramId
                                 where m.CallDateTime > startDate
                                  && m.CallDateTime < endDate
                                  && m.Concern != null
                                  && p.UtilityId == utilityId
                                 select m);

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


        #region Gets All Good Sales performed for a date range by Utility
        private static int GetTotalGoodSales_ByUtility(DateTime startDate, DateTime endDate, int utilityId)
        {
            //SELECT count(m.mainid) AS [Total Verifications Performed]
            //FROM [Spark].[v1].[Main] m
            //join [Spark].[v1].[OrderDetail] od on od.MainId = m.MainId
            //join [Spark].[v1].[Program] p on od.ProgramId = p.ProgramId
            //WHERE m.calldatetime > startDate  and m.calldatetime < endDate 
            //AND m.concern ='1'
            //AND m.verified ='1'
            //and p.UtilityId = utilityId

            int total = 0;
            try
            {
                using (SparkEntities entities = new SparkEntities())
                {
                    var query = (from m in entities.Mains
                                 join od in entities.OrderDetails on m.MainId equals od.MainId
                                 join p in entities.Programs on od.ProgramId equals p.ProgramId
                                 where m.CallDateTime > startDate
                                  && m.CallDateTime < endDate
                                  && m.Concern != null
                                  && m.Verified == "1"
                                  && p.UtilityId == utilityId
                                 select m);

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

        #region Disposition By Utility (1 method)
        private static int GetDispositionCount_ByUtility(DateTime startDate, DateTime endDate, int utilityId, string concern)
        {
            //Not a true SQL representation, just used 
            //to quickly check all dispositions for a state and date range

            //SELECT m.concern, count(m.mainid) AS [Count]
            //FROM [Spark].[v1].[Main] m
            //join [Spark].[v1].[OrderDetail] od on od.MainId = m.MainId
            //join [Spark].[v1].[Program] p on od.ProgramId = p.ProgramId
            //WHERE m.calldatetime > startDate  and m.calldatetime < endDate 
            //AND m.concern is not null
            //and p.UtilityId = utilityId
            //group by m.concern
            //order by m.concern desc

            int total = 0;
            try
            {
                using (SparkEntities entities = new SparkEntities())
                {
                    var query = (from m in entities.Mains
                                 join od in entities.OrderDetails on m.MainId equals od.MainId
                                 join p in entities.Programs on od.ProgramId equals p.ProgramId
                                 where m.CallDateTime > startDate
                                 && m.CallDateTime < endDate
                                 && m.Concern == concern
                                 && p.UtilityId == utilityId
                                 select m);

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

        #region By Vendor Data (1 method)
        #region Gets Total Verifications performed for a date range by Vendor
        private static int GetTotalVerificationsPerformed_ByVendor(DateTime startDate, DateTime endDate, int vendorid)
        {
            //SELECT count(m.mainid) AS [Total Verifications Performed]
            //FROM [Spark].[v1].[Main] m
            //join [Spark].[v1].[User] u on u.UserId = m.UserId
            //join [Spark].[v1].[Vendor] v on v.VendorId = u.VendorId
            //WHERE m.calldatetime > startDate and m.calldatetime < endDate
            //AND v.VendorId = vendorid
            //AND m.concern is not null
            int total = 0;
            try
            {
                using (SparkEntities entities = new SparkEntities())
                {
                    var query = (from m in entities.Mains
                                 join u in entities.Users on m.UserId equals u.UserId
                                 join v in entities.Vendors on u.VendorId equals v.VendorId
                                 where m.CallDateTime > startDate
                                  && m.CallDateTime < endDate
                                  && m.Concern != null
                                  && v.VendorId == vendorid
                                 select m);

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

        #region Gets All Good Sales performed for a date range by Vendor (1 method)
        private static int GetTotalGoodSales_ByVendor(DateTime startDate, DateTime endDate, int vendorId)
        {
            //SELECT count(m.mainid) AS [Total Verifications Performed]
            //FROM [Spark].[v1].[Main] m
            //join [Spark].[v1].[User] u on u.UserId = m.UserId
            //join [Spark].[v1].[Vendor] v on v.VendorId = u.VendorId
            //WHERE m.calldatetime > startDate and m.calldatetime < endDate
            //AND v.VendorId = vendorid
            //AND m.concern ='1'
            //AND m.verified ='1'
            int total = 0;
            try
            {
                using (SparkEntities entities = new SparkEntities())
                {
                    var query = (from m in entities.Mains
                                 join u in entities.Users on m.UserId equals u.UserId
                                 join v in entities.Vendors on u.VendorId equals v.VendorId
                                 where m.CallDateTime > startDate
                                 && m.CallDateTime < endDate
                                 && m.Concern != null
                                 && m.Verified == "1"
                                 && v.VendorId == vendorId
                                 select m);

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

        #region Disposition By Vendor (1 method)
        private static int GetDispositionCount_ByVendor(DateTime startDate, DateTime endDate, int vendorid, string concern)
        {
            //Not a true SQL representation, just used 
            //to quickly check all dispositions for a state and date range

            //SELECT m.concern, count(m.mainid) AS [Count]
            //FROM [Spark].[v1].[Main] m
            //join [Spark].[v1].[User] u on u.UserId = m.UserId
            //join [Spark].[v1].[Vendor] v on v.VendorId = u.VendorId
            //WHERE m.CallDateTime > startdate and m.CallDateTime < enddate
            //AND v.VendorId = vendorid
            //AND m.Concern is not null
            //group by m.concern
            //order by m.concern desc

            int total = 0;
            try
            {
                using (SparkEntities entities = new SparkEntities())
                {
                    var query = (from m in entities.Mains
                                 join u in entities.Users on m.UserId equals u.UserId
                                 join v in entities.Vendors on u.VendorId equals v.VendorId
                                 where m.CallDateTime > startDate
                                 && m.CallDateTime < endDate
                                 && m.Concern == concern
                                 && v.VendorId == vendorid
                                 select m);

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

        #region MTD Sales Rep by Disposition (1 method)
        private static List<SalesRep> GetMTDSalesRepByDisposition(DateTime startDate, DateTime endDate, int vendorId, List<Disposition> nonVerifiedDispositionList)
        {

            //Select u.FirstName +' ' + u.LastName as Name, m.Concern, count(m.Concern) as Total
            //FROM [Spark].[v1].[Main] m
            //join [Spark].[v1].[User] u on u.UserId = m.UserId
            //join [Spark].[v1].[Vendor] v on v.VendorId = u.VendorId
            // where m.CallDateTime > endDate and m.CallDateTime < endDate
            // and v.VendorId = vendorId
            // and m.concern in (nonVerifiedDispositionList)
            // group by m.Concern, u.FirstName +' ' + u.LastName
            List<SalesRep> salesRep = new List<SalesRep>();


            try
            {

                List<string> myfilter = new List<string>();
                foreach (Disposition disp in nonVerifiedDispositionList)
                {
                    myfilter.Add(disp.Concern);
                }

                using (SparkEntities entities = new SparkEntities())
                {
                    //Get all sales reps for a vendorId and date range where verified is not 1
                    var salesRepQuery = (from m in entities.Mains
                                         join u in entities.Users on m.UserId equals u.UserId
                                         join v in entities.Vendors on u.VendorId equals v.VendorId
                                         where m.CallDateTime > startDate
                                         && m.CallDateTime < endDate
                                         && v.VendorId == vendorId
                                         && m.Verified != "1"
                                         group u by new { u.UserId, u.FirstName, u.LastName } into uGroup
                                         select new
                                         {
                                             uGroup.Key.FirstName,
                                             uGroup.Key.LastName,
                                             uGroup.Key.UserId
                                         }).OrderBy(group => group.UserId);


                    foreach (var salerepitem in salesRepQuery)
                    {

                        //Get all dispositions for a sales rep with concern list for a date range and vendorId
                        var dispositionQuery = (from m in entities.Mains
                                                join u in entities.Users on m.UserId equals u.UserId
                                                join v in entities.Vendors on u.VendorId equals v.VendorId
                                                where m.CallDateTime > startDate
                                                && m.CallDateTime < endDate
                                                && v.VendorId == vendorId
                                                && u.UserId == salerepitem.UserId
                                                && myfilter.Contains(m.Concern)
                                                group m by m.Concern into concernList
                                                select new
                                                {
                                                    Concern = concernList.Key,
                                                    Count = concernList.Count()
                                                }).OrderByDescending(group => group.Concern);

                        int totalcount = 0;
                        List<Disposition> salesRepDispositions = new List<Disposition>();
                        foreach (var concernItem in dispositionQuery)
                        {
                            Disposition myDisposition = new Disposition();

                            myDisposition.Concern = concernItem.Concern;
                            myDisposition.Count = concernItem.Count;
                            totalcount += concernItem.Count;
                            salesRepDispositions.Add(myDisposition);
                        }
                        SalesRep mysalesrep = new SalesRep(salerepitem.FirstName, salerepitem.LastName, totalcount, salesRepDispositions);


                        salesRep.Add(mysalesrep);
                    }

                }
            }
            catch (Exception ex)
            {
                SendErrorMessage(ex);
                //throw ex;
            }
            return salesRep;

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
        private static void SendEmail(ref string xlsFilePath, DateTime reportDate, String strToEmail)
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


                mail.From = "reports1@calibrus.com";

                mail.Subject = "Spark Energy Daily Activity Report for " + reportDate.ToString("MMM") + " " + reportDate.ToString("dd") + " " + reportDate.ToString("yyyy") + ".";


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
            xlsFilename = "SparkDailyActivityReport" + String.Format("{0:yyyyMMdd}", reportDate) + ".xlsx";

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

        private static void GetDates(out DateTime CurrentDate, out DateTime MonthStartDate, out DateTime YearStartDate)
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

            //since we run this report for data that is on a 24 hour cycle, we run this report on the day after to get the full days data
            //on the first of the month we must ensure that we still get the previous months data and not start on the current month
            if (baseDate.Day == 1)
            {
                MonthStartDate = new DateTime(baseDate.Year, baseDate.Month, 1, 0, 0, 0).AddMonths(-1); //Beginning of the previous month   
            }
            else
            {
                MonthStartDate = new DateTime(baseDate.Year, baseDate.Month, 1, 0, 0, 0); //Beginning of current month  
            }
            YearStartDate = new DateTime(baseDate.Year, 1, 1, 0, 0, 0);   //Beginning of current year
        }

        private static void SendErrorMessage(Exception ex)
        {
            Calibrus.ErrorHandler.Alerting alert = new Calibrus.ErrorHandler.Alerting("SparkDailyActivityReport");
            alert.SendAlert(ex.Source, ex.Message, Environment.MachineName, Environment.UserName, Environment.Version.ToString());
        }
        #endregion

    }
}
