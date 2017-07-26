using Calibrus.ExcelFunctions;
using Calibrus.Mail;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using Excel = Microsoft.Office.Interop.Excel;

namespace ConstellationDailyActivityReport
{
    public class DailyActivityReport
    {
        public enum ReportType
        {
            Daily,
            MTD,
            YTD
        }

        //public static Dictionary<string, int> Vendor = new Dictionary<string, int>()
        //{
        //    {"Global", 86},
        //    {"Protocall", 44}
        //};

        public static Dictionary<string, string> Channel = new Dictionary<string, string>()
        {
            {"Res Inbound English", "2277"},
            {"Res Inbound Spanish", "2212"},
            {"Res Outbound English", "2278"},
            {"Res Outbound Spanish", "2296"}
        };

        public static object na = System.Reflection.Missing.Value;

        #region Main

        public static void Main(string[] args)
        {
            string rootPath = string.Empty;

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

            //Get The Vendors to report on
            List<Vendors> vendorsList = GetVendorList();

            //start Excel
            Excel.Application exApp = new Excel.Application();
            Excel.Workbook exBook = null;
            Excel.Worksheet exSheet = null;
            Excel.Range exRange = null;

            int sheetsAdded = 0;

            #region VendorForLoop - Breakout for Individual Vendors

            foreach (Vendors vendor in vendorsList)
            {
                sheetsAdded = 0;

                try
                {
                    exBook = exApp.Workbooks.Add(na);
                    exApp.Visible = false;

                    //Set global attributes
                    exApp.StandardFont = "Calibri";
                    exApp.StandardFontSize = 11;

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
                                WriteReportForReportTypeAndDates(ref exApp, ref exRange, CurrentDate.AddDays(-1), CurrentDate, reportType.ToString(), strHeader, vendor);
                                break;

                            case "MTD":
                                strHeader = "Month to Date: TPV Daily Activity Report";
                                WriteReportForReportTypeAndDates(ref exApp, ref exRange, MonthStartDate, MonthStartDate.AddMonths(1), reportType.ToString(), strHeader, vendor);
                                break;

                            case "YTD":
                                strHeader = "Year to Date: TPV Daily Activity Report";
                                WriteReportForReportTypeAndDates(ref exApp, ref exRange, YearStartDate, YearStartDate.AddYears(1), reportType.ToString(), strHeader, vendor);
                                break;
                        }

                        //Autosize the columns, not sure if this will ever get to column Z, but this will ensure that the format is for all written columns
                        exRange = (Excel.Range)exApp.get_Range("A1", "Z1");
                        exRange.EntireColumn.AutoFit();
                    }

                    #endregion WriteReportForReportTypeAndDates Tab

                    #region WriteReportForNoSalesMTDDetail Tab

                    //Write One Tab of No Sales - MT Detail for the MTD date range.
                    if (sheetsAdded < exBook.Sheets.Count)
                    {
                        exSheet = (Excel.Worksheet)exBook.Sheets[sheetsAdded + 1];
                    }
                    else
                    {
                        exSheet = (Excel.Worksheet)exBook.Sheets.Add(na, exBook.ActiveSheet, na, na);
                    }

                    exSheet.Name = "No Sales - MTD Details";
                    exSheet.Select(na);

                    sheetsAdded++;
                    WriteReportForNoSalesMTDDetail(ref exApp, ref exRange, MonthStartDate, MonthStartDate.AddMonths(1), string.Format("Detail for for {0:MMMM d, yyyy}", MonthStartDate), vendor);

                    //Autosize the columns
                    exRange = (Excel.Range)exApp.get_Range("A1", "Z1");
                    exRange.EntireColumn.AutoFit();

                    #endregion WriteReportForNoSalesMTDDetail Tab

                    #region MTDSalesRepByDisposition  Tab

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
                    WriteReportForMTDSalesRepByDisposition(ref exApp, ref exRange, MonthStartDate, MonthStartDate.AddMonths(1), vendor);

                    //Autosize the columns
                    exRange = (Excel.Range)exApp.get_Range("A1", "Z1");
                    exRange.EntireColumn.AutoFit();

                    #endregion MTDSalesRepByDisposition  Tab

                    //select the first tab in the workbook
                    exSheet = (Excel.Worksheet)exApp.Worksheets[1];
                    exSheet.Select(na);

                    //Save the xls Report to represent the day prior to the current run date for proper identification of the data run
                    SaveXlsDocument(ref rootPath, ref xlsFilename, ref xlsFilePath, exBook, CurrentDate.AddDays(-1), vendor.VendorName);
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
                SendEmail(ref xlsFilePath, CurrentDate.AddDays(-1), vendor.VendorName);
            }

            #endregion VendorForLoop - Breakout for Individual Vendors

            #region All Vendors

            //Then write out for All Vendors
            sheetsAdded = 0;

            try
            {
                exBook = exApp.Workbooks.Add(na);
                exApp.Visible = false;

                //Set global attributes
                exApp.StandardFont = "Calibri";
                exApp.StandardFontSize = 11;

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
                            WriteReportForReportTypeAndDates(ref exApp, ref exRange, CurrentDate.AddDays(-1), CurrentDate, reportType.ToString(), strHeader, vendorsList);
                            break;

                        case "MTD":
                            strHeader = "Month to Date: TPV Daily Activity Report";
                            WriteReportForReportTypeAndDates(ref exApp, ref exRange, MonthStartDate, MonthStartDate.AddMonths(1), reportType.ToString(), strHeader, vendorsList);
                            break;

                        case "YTD":
                            strHeader = "Year to Date: TPV Daily Activity Report";
                            WriteReportForReportTypeAndDates(ref exApp, ref exRange, YearStartDate, YearStartDate.AddYears(1), reportType.ToString(), strHeader, vendorsList);
                            break;
                    }

                    //Autosize the columns, not sure if this will ever get to column Z, but this will ensure that the format is for all written columns
                    exRange = (Excel.Range)exApp.get_Range("A1", "Z1");
                    exRange.EntireColumn.AutoFit();
                }

                #endregion WriteReportForReportTypeAndDates Tab

                #region WriteReportForNoSalesMTDDetail Tab

                //Write One Tab of No Sales - MT Detail for the MTD date range.
                if (sheetsAdded < exBook.Sheets.Count)
                {
                    exSheet = (Excel.Worksheet)exBook.Sheets[sheetsAdded + 1];
                }
                else
                {
                    exSheet = (Excel.Worksheet)exBook.Sheets.Add(na, exBook.ActiveSheet, na, na);
                }

                exSheet.Name = "No Sales - MTD Details";
                exSheet.Select(na);

                sheetsAdded++;
                WriteReportForNoSalesMTDDetail(ref exApp, ref exRange, MonthStartDate, MonthStartDate.AddMonths(1), string.Format("Detail for for {0:MMMM d, yyyy}", MonthStartDate), vendorsList);

                //Autosize the columns
                exRange = (Excel.Range)exApp.get_Range("A1", "Z1");
                exRange.EntireColumn.AutoFit();

                #endregion WriteReportForNoSalesMTDDetail Tab

                #region MTDSalesRepByDisposition  Tab

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
                WriteReportForMTDSalesRepByDisposition(ref exApp, ref exRange, MonthStartDate, MonthStartDate.AddMonths(1), vendorsList);

                //Autosize the columns
                exRange = (Excel.Range)exApp.get_Range("A1", "Z1");
                exRange.EntireColumn.AutoFit();

                #endregion MTDSalesRepByDisposition  Tab

                //select the first tab in the workbook
                exSheet = (Excel.Worksheet)exApp.Worksheets[1];
                exSheet.Select(na);

                //Save the xls Report to represent the day prior to the current run date for proper identification of the data run
                SaveXlsDocument(ref rootPath, ref xlsFilename, ref xlsFilePath, exBook, CurrentDate.AddDays(-1), "Total");
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
            SendEmail(ref xlsFilePath, CurrentDate.AddDays(-1), "Total");

            #endregion All Vendors
        }

        #endregion Main

        #region Excel

        #region WriteReportForReportTypeAndDates_EXCEL (2 methods)

        private static void WriteReportForReportTypeAndDates(ref Excel.Application exApp, ref Excel.Range exRange, DateTime startDate, DateTime endDate, string reportType, string header, Vendors vendor)
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

            #endregion Header

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
            TotalVerificationsPerformed_Overall = GetTotalVerificationsPerformed_Overall(startDate, endDate, vendor.VendorId.ToString());
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn("B", row), new RangeColumn("B", row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight,
                 11, true, false, false);
            exRange.Value2 = "Total Verifications performed";

            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn("C", row), new RangeColumn("C", row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = TotalVerificationsPerformed_Overall;
            row++;

            //Good Sale
            TotalGoodSale_Overall = GetTotalGoodSales_Overall(startDate, endDate, vendor.VendorId.ToString());
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

            #endregion Sales Overall

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
                TotalVerificationsPerformed_ByChannel = GetTotalVerificationsPerformed_ByChannel(startDate, endDate, pair.Value, vendor.VendorId.ToString());
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
                TotalGoodSale_ByChannel = GetTotalGoodSales_ByChannel(startDate, endDate, pair.Value, vendor.VendorId.ToString());
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

            #endregion Sales by Channel

            #region Sales by State

            int TotalVerificationsPerformed_ByState = 0;
            int TotalGoodSale_ByState = 0;
            List<string> StatesList = new List<string>(); //reused below for State Dispositions

            StatesList = GetStates(startDate, endDate, vendor.VendorId.ToString());

            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn("A", row), new RangeColumn("B", row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
               14, true, false, false);
            exRange.Merge(na);
            exRange.Interior.ColorIndex = 15;//grey
            exRange.Value2 = "Sales by State";

            foreach (string state in StatesList)
            {
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                   14, true, false, false);
                exRange.Interior.ColorIndex = 15;//grey
                exRange.Value2 = state;

                col++;
            }

            col = dataColumnInitialize;//reset column back to C
            row++;

            //Total Verifications performed
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn("B", row), new RangeColumn("B", row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight,
                 11, true, false, false);
            exRange.Value2 = "Total Verifications performed";

            foreach (string state in StatesList)
            {
                TotalVerificationsPerformed_ByState = GetTotalVerificationsPerformed_ByState(startDate, endDate, state, vendor.VendorId.ToString());
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                  11, true, false, false);
                exRange.Value2 = TotalVerificationsPerformed_ByState;

                col++;
            }

            col = dataColumnInitialize;//reset column back to C
            row++;

            //Good Sale
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn("B", row), new RangeColumn("B", row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight,
                11, false, false, false);
            exRange.Value2 = "Good Sale";

            foreach (string state in StatesList)
            {
                TotalGoodSale_ByState = GetTotalGoodSales_ByState(startDate, endDate, state, vendor.VendorId.ToString());
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = TotalGoodSale_ByState;

                col++;
            }

            col = dataColumnInitialize;//reset column back to C
            row++;

            //No Sale
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn("B", row), new RangeColumn("B", row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight,
                11, false, false, false);
            exRange.Value2 = "No Sale";

            foreach (string state in StatesList)
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

            foreach (string state in StatesList)
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

            foreach (string state in StatesList)
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

            #endregion Sales by State

            #region Disposition by State

            int DispositionCount_ByState = 0;
            int DispositionCountTotal_ByState = 0;
            List<Disposition> DispositionList = GetDispositionList(startDate, endDate); //reused below for Vendor Dispositions
            //int dispositionByStateTotal = DispositionList.Select(c => c.Count).Sum();

            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn("A", row), new RangeColumn("B", row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
               14, true, false, false);
            exRange.Merge(na);
            exRange.Interior.ColorIndex = 15;//grey
            exRange.Value2 = "Disposition by State";

            foreach (string state in StatesList)
            {
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                   14, true, false, false);
                exRange.Interior.ColorIndex = 15;//grey
                exRange.Value2 = state;

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

            foreach (string state in StatesList)
            {
                foreach (var disposition in DispositionList)
                {
                    //pass in date state and disposition
                    DispositionCount_ByState = GetDispositionCount_ByState(startDate, endDate, state, disposition.Concern, vendor.VendorId.ToString());
                    DispositionCountTotal_ByState += DispositionCount_ByState;
                    exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                       11, false, false, false);
                    exRange.Value2 = DispositionCount_ByState;
                    row++;
                }

                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, true, false, false);
                exRange.Value2 = DispositionCountTotal_ByState;
                //roll back the rows to start for the data write
                foreach (var disposition in DispositionList)
                {
                    row--;
                }

                DispositionCountTotal_ByState = 0; //reset count to 0
                col++;
            }
            //moveforward the rows to start for the next section
            foreach (var disposition in DispositionList)
            {
                row++;
            }

            col = dataColumnInitialize;//reset column back to C
            row++;
            row++;

            #endregion Disposition by State

            #region Sales by Vendor

            int TotalVerificationsPerformed_ByVendor = 0;
            int TotalGoodSale_ByVendor = 0;

            //List<tblVendor> VendorList = new List<tblVendor>(); //reused below for Vendor Dispositions

            //VendorList = GetVendors();

            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn("A", row), new RangeColumn("B", row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
             14, true, false, false);
            exRange.Merge(na);
            exRange.Interior.ColorIndex = 15;//grey
            exRange.Value2 = "Sales by Vendor";

            //foreach (Vendors vendor in vendorList)
            //{
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
               14, true, false, false);
            exRange.Interior.ColorIndex = 15;//grey
            exRange.Value2 = vendor.VendorName;

            col++;
            //}

            col = dataColumnInitialize;//reset column back to C
            row++;

            //Total Verifications performed
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn("B", row), new RangeColumn("B", row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight,
                 11, true, false, false);
            exRange.Value2 = "Total Verifications performed";

            //foreach (Vendors vendor in vendorList)
            //{
            TotalVerificationsPerformed_ByVendor = GetTotalVerificationsPerformed_ByVendor(startDate, endDate, vendor.VendorId.ToString());
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
              11, true, false, false);
            exRange.Value2 = TotalVerificationsPerformed_ByVendor;

            col++;
            //}

            col = dataColumnInitialize;//reset column back to C
            row++;

            //Good Sale
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn("B", row), new RangeColumn("B", row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight,
                11, false, false, false);
            exRange.Value2 = "Good Sale";

            //foreach (Vendors vendor in vendorList)
            //{
            TotalGoodSale_ByVendor = GetTotalGoodSales_ByVendor(startDate, endDate, vendor.VendorId.ToString());
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, false, false, false);
            exRange.Value2 = TotalGoodSale_ByVendor;

            col++;
            //}

            col = dataColumnInitialize;//reset column back to C
            row++;

            //No Sale
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn("B", row), new RangeColumn("B", row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight,
                11, false, false, false);
            exRange.Value2 = "No Sale";

            //foreach (Vendors vendor in vendorList)
            //{
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, false, false, false);
            exRange.NumberFormat = "0";
            exRange.Formula = string.Format("=IFERROR({0}{1}-{0}{2}, \"0.00%\" )", ConvertColumn(col), row - 2, row - 1);

            col++;
            //}

            col = dataColumnInitialize;//reset column back to C
            row++;

            //Good Sale %
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn("B", row), new RangeColumn("B", row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight,
                11, false, false, false);
            exRange.Value2 = "Good Sale %";

            //foreach (Vendors vendor in vendorList)
            //{
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, false, false, false);
            exRange.NumberFormat = "0.00%";
            exRange.Formula = string.Format("=IFERROR({0}{1}/{0}{2}, \"0.00%\" )", ConvertColumn(col), row - 2, row - 3);

            col++;
            //}

            col = dataColumnInitialize;//reset column back to C
            row++;

            //No Sale %
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn("B", row), new RangeColumn("B", row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight,
                11, false, false, false);
            exRange.Value2 = "No Sale %";

            //foreach (Vendors vendor in vendorList)
            //{
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
               11, false, false, false);
            exRange.NumberFormat = "0.00%";
            exRange.Formula = string.Format("=IFERROR({0}{1}/{0}{2}, \"0.00%\" )", ConvertColumn(col), row - 2, row - 4);

            col++;
            //}

            col = dataColumnInitialize;//reset column back to C
            row++;
            row++;

            #endregion Sales by Vendor

            #region Disposition by Vendor

            int DispositionCount_ByVendor = 0;
            int DispositionCountTotal_ByVendor = 0;

            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn("A", row), new RangeColumn("B", row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
             14, true, false, false);
            exRange.Merge(na);
            exRange.Interior.ColorIndex = 15;//grey
            exRange.Value2 = "Disposition by Vendor";

            //foreach (Vendors vendor in vendorList)
            //{
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
               14, true, false, false);
            exRange.Interior.ColorIndex = 15;//grey
            exRange.Value2 = vendor.VendorName;

            col++;
            //}

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

            //foreach (Vendors vendor in vendorList)
            //{
            foreach (var disposition in DispositionList)
            {
                //pass in date state and disposition
                DispositionCount_ByVendor = GetDispositionCount_ByVendor(startDate, endDate, vendor.VendorId.ToString(), disposition.Concern);
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
            //}
            //moveforward the rows to start for the next section
            foreach (var disposition in DispositionList)
            {
                row++;
            }

            col = dataColumnInitialize;//reset column back to C
            row++;
            row++;

            #endregion Disposition by Vendor

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

            //moveforward the rows to start for the next section
            foreach (var nonverifieddisposition in NonVerifiedDispositionList)
            {
                row++;
            }

            col = dataColumnInitialize;//reset column back to C
            row++;
            row++;

            #endregion Non-Verified By Disposition (Overall)
        }

        private static void WriteReportForReportTypeAndDates(ref Excel.Application exApp, ref Excel.Range exRange, DateTime startDate, DateTime endDate, string reportType, string header, List<Vendors> vendorList)
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

            #endregion Header

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

            #endregion Sales Overall

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

            #endregion Sales by Channel

            #region Sales by State

            int TotalVerificationsPerformed_ByState = 0;
            int TotalGoodSale_ByState = 0;
            List<string> StatesList = new List<string>(); //reused below for State Dispositions

            StatesList = GetStates(startDate, endDate);

            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn("A", row), new RangeColumn("B", row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
               14, true, false, false);
            exRange.Merge(na);
            exRange.Interior.ColorIndex = 15;//grey
            exRange.Value2 = "Sales by State";

            foreach (string state in StatesList)
            {
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                   14, true, false, false);
                exRange.Interior.ColorIndex = 15;//grey
                exRange.Value2 = state;

                col++;
            }

            col = dataColumnInitialize;//reset column back to C
            row++;

            //Total Verifications performed
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn("B", row), new RangeColumn("B", row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight,
                 11, true, false, false);
            exRange.Value2 = "Total Verifications performed";

            foreach (string state in StatesList)
            {
                TotalVerificationsPerformed_ByState = GetTotalVerificationsPerformed_ByState(startDate, endDate, state);
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                  11, true, false, false);
                exRange.Value2 = TotalVerificationsPerformed_ByState;

                col++;
            }

            col = dataColumnInitialize;//reset column back to C
            row++;

            //Good Sale
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn("B", row), new RangeColumn("B", row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight,
                11, false, false, false);
            exRange.Value2 = "Good Sale";

            foreach (string state in StatesList)
            {
                TotalGoodSale_ByState = GetTotalGoodSales_ByState(startDate, endDate, state);
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = TotalGoodSale_ByState;

                col++;
            }

            col = dataColumnInitialize;//reset column back to C
            row++;

            //No Sale
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn("B", row), new RangeColumn("B", row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight,
                11, false, false, false);
            exRange.Value2 = "No Sale";

            foreach (string state in StatesList)
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

            foreach (string state in StatesList)
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

            foreach (string state in StatesList)
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

            #endregion Sales by State

            #region Disposition by State

            int DispositionCount_ByState = 0;
            int DispositionCountTotal_ByState = 0;
            List<Disposition> DispositionList = GetDispositionList(startDate, endDate); //reused below for Vendor Dispositions
            //int dispositionByStateTotal = DispositionList.Select(c => c.Count).Sum();

            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn("A", row), new RangeColumn("B", row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
               14, true, false, false);
            exRange.Merge(na);
            exRange.Interior.ColorIndex = 15;//grey
            exRange.Value2 = "Disposition by State";

            foreach (string state in StatesList)
            {
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                   14, true, false, false);
                exRange.Interior.ColorIndex = 15;//grey
                exRange.Value2 = state;

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

            foreach (string state in StatesList)
            {
                foreach (var disposition in DispositionList)
                {
                    //pass in date state and disposition
                    DispositionCount_ByState = GetDispositionCount_ByState(startDate, endDate, state, disposition.Concern);
                    DispositionCountTotal_ByState += DispositionCount_ByState;
                    exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                       11, false, false, false);
                    exRange.Value2 = DispositionCount_ByState;
                    row++;
                }

                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, true, false, false);
                exRange.Value2 = DispositionCountTotal_ByState;
                //roll back the rows to start for the data write
                foreach (var disposition in DispositionList)
                {
                    row--;
                }

                DispositionCountTotal_ByState = 0; //reset count to 0
                col++;
            }
            //moveforward the rows to start for the next section
            foreach (var disposition in DispositionList)
            {
                row++;
            }

            col = dataColumnInitialize;//reset column back to C
            row++;
            row++;

            #endregion Disposition by State

            #region Sales by Vendor

            int TotalVerificationsPerformed_ByVendor = 0;
            int TotalGoodSale_ByVendor = 0;

            //List<tblVendor> VendorList = new List<tblVendor>(); //reused below for Vendor Dispositions

            //VendorList = GetVendors();

            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn("A", row), new RangeColumn("B", row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
             14, true, false, false);
            exRange.Merge(na);
            exRange.Interior.ColorIndex = 15;//grey
            exRange.Value2 = "Sales by Vendor";

            foreach (Vendors vendor in vendorList)
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

            foreach (Vendors vendor in vendorList)
            {
                TotalVerificationsPerformed_ByVendor = GetTotalVerificationsPerformed_ByVendor(startDate, endDate, vendor.VendorId.ToString());
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

            foreach (Vendors vendor in vendorList)
            {
                TotalGoodSale_ByVendor = GetTotalGoodSales_ByVendor(startDate, endDate, vendor.VendorId.ToString());
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

            foreach (Vendors vendor in vendorList)
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

            foreach (Vendors vendor in vendorList)
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

            foreach (Vendors vendor in vendorList)
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

            #endregion Sales by Vendor

            #region Disposition by Vendor

            int DispositionCount_ByVendor = 0;
            int DispositionCountTotal_ByVendor = 0;

            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn("A", row), new RangeColumn("B", row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
             14, true, false, false);
            exRange.Merge(na);
            exRange.Interior.ColorIndex = 15;//grey
            exRange.Value2 = "Disposition by Vendor";

            foreach (Vendors vendor in vendorList)
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

            foreach (Vendors vendor in vendorList)
            {
                foreach (var disposition in DispositionList)
                {
                    //pass in date state and disposition
                    DispositionCount_ByVendor = GetDispositionCount_ByVendor(startDate, endDate, vendor.VendorId.ToString(), disposition.Concern);
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
            //moveforward the rows to start for the next section
            foreach (var disposition in DispositionList)
            {
                row++;
            }

            col = dataColumnInitialize;//reset column back to C
            row++;
            row++;

            #endregion Disposition by Vendor

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

            //moveforward the rows to start for the next section
            foreach (var nonverifieddisposition in NonVerifiedDispositionList)
            {
                row++;
            }

            col = dataColumnInitialize;//reset column back to C
            row++;
            row++;

            #endregion Non-Verified By Disposition (Overall)
        }

        #endregion WriteReportForReportTypeAndDates_EXCEL (2 methods)

        #region WriteReportForNoSalesMTDDetail_EXCEL (2 methods)

        private static void WriteReportForNoSalesMTDDetail(ref Excel.Application exApp, ref Excel.Range exRange, DateTime startDate, DateTime endDate, string header, Vendors vendor)
        {
            int rowInitialize = 1; //initial seed for the row data
            int row = 0;// where we start the row data

            int dataColumnInitialize = 65; //initial seed for column data - column  A
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

            row++;

            //vendor_name
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "vendor_name";
            col++;

            //tsr_id
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                 11, true, false, false);
            exRange.Value2 = "tsr_id";
            col++;

            //tsr_name
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "tsr_namel";
            col++;

            //status_txt
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "status_txt";
            col++;

            //reason
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "reason";
            col++;

            //p_date
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "p_date";
            col++;

            //sales_state
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "sales_state";
            col++;

            //fuel_type
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "fuel_type";
            col++;

            //dual_fuel
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "dual_fuel";
            col++;

            //btn
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "btn";
            col++;

            //acct_num
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "acct_num";
            col++;

            //auth_fname
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "auth_fname";
            col++;

            //auth_lname
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "auth_lname";
            col++;

            //bill_fname
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "bill_fname";
            col++;

            //bill_lname
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "bill_lname";
            col++;

            //addr1
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "addr1";
            col++;

            //addr2
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "addr2";
            col++;

            //city
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "city";
            col++;

            //state
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "state";
            col++;

            //zip
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "zip";
            col++;

            //ldc_code
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "ldc_code";
            col++;

            //rate
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "rate";
            col++;

            //term
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "term";
            col++;

            //ver_code
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "ver_code";
            col++;

            //response_id
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "response_id";
            col++;

            //sales_method
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "sales_method";

            col = dataColumnInitialize;//reset col to A
            row++;

            #endregion Header

            #region Data

            IList<spMTDCallDetailByVendor_Result> mtdCallDeatilResult = GetNoSalesMTDDetail(startDate, endDate, vendor.VendorId);

            #endregion Data

            #region Write Out Data

            //Data map from the stored procedure

            //VendorName = vendor_name
            //VendorAgentId = tsr_id
            //AgentName = tsr_name
            //VerifiedFormattedExport = status_txt
            //Concern = reason
            //CallDateTime = DataFormatString="{0:MM/dd/yyyy} = p_date
            //ServiceState = sales_state
            //SignUpType = fuel_type
            //DualSignUp = dual_fuel
            //ServicePhoneNumber = btn
            //UDCAccountNumber =  acct_num
            //ServiceFirstName = auth_fname
            //ServiceLastName = auth_lname
            //BillingFirstName = bill_fname
            //BillingLastName = bill_lname
            //BillingAddress1 = addr1
            //BillingAddress2 = addr2
            //BillingCity = city
            //BillingState = state
            //BillingZipCode = zip
            //UDCCode = ldc_code
            //Rate = rate
            //Term = term
            //MainId = ver_code
            //ResponseId = response_id

            foreach (var item in mtdCallDeatilResult.OrderBy(s => s.CallDateTime))
            {
                if (mtdCallDeatilResult.Count != 0)
                {
                    //vendor_name
                    exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                        11, false, false, false);
                    exRange.Value2 = item.VendorName;
                    col++;

                    //tsr_id
                    exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                        11, false, false, false);
                    exRange.Value2 = item.VendorAgentId;
                    col++;

                    //tsr_name
                    exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                        11, false, false, false);
                    exRange.Value2 = item.AgentName;
                    col++;

                    //status_txt
                    exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                        11, false, false, false);
                    exRange.Value2 = (item.Verified == "1" ? "Yes" : "No");
                    col++;

                    //reason
                    exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                        11, false, false, false);
                    exRange.Value2 = item.Concern;
                    col++;

                    //p_date
                    exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                        11, false, false, false);
                    exRange.NumberFormat = "mm/dd/yyyy";
                    exRange.Value2 = item.CallDateTime;

                    col++;
                    //sales_state
                    exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                        11, false, false, false);
                    exRange.Value2 = item.ServiceState;
                    col++;

                    //fuel_type
                    exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                        11, false, false, false);
                    exRange.Value2 = item.SignUpType;
                    col++;

                    //dual_fuel
                    exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                        11, false, false, false);
                    exRange.Value2 = item.DualSignUp;
                    col++;

                    //btn
                    exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                        11, false, false, false);
                    exRange.Value2 = item.ServicePhoneNumber;
                    col++;

                    //acct_num
                    exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                        11, false, false, false);
                    exRange.NumberFormat = "@";
                    exRange.Value2 = item.UDCAccountNumber;
                    col++;

                    //auth_fname
                    exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                        11, false, false, false);
                    exRange.Value2 = item.ServiceFirstName;
                    col++;

                    //auth_lname
                    exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                        11, false, false, false);
                    exRange.Value2 = item.ServiceLastName;
                    col++;

                    //bill_fname
                    exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                        11, false, false, false);
                    exRange.Value2 = item.BillingFirstName;
                    col++;

                    //bill_lname
                    exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                        11, false, false, false);
                    exRange.Value2 = item.BillingLastName;
                    col++;

                    //addr1
                    exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                        11, false, false, false);
                    exRange.Value2 = item.BillingAddress1;
                    col++;

                    //addr2
                    exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                        11, false, false, false);
                    exRange.Value2 = item.BillingAddress2;
                    col++;

                    //city
                    exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                        11, false, false, false);
                    exRange.Value2 = item.BillingCity;
                    col++;

                    //state
                    exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                        11, false, false, false);
                    exRange.Value2 = item.BillingState;
                    col++;

                    //zip
                    exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                        11, false, false, false);
                    exRange.Value2 = item.BillingZipCode;
                    col++;

                    //ldc_code
                    exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                        11, false, false, false);
                    exRange.Value2 = item.UDCCode;
                    col++;

                    //rate
                    exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                        11, false, false, false);
                    exRange.Value2 = item.Rate;
                    col++;

                    //term
                    exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                        11, false, false, false);
                    exRange.Value2 = item.Term;
                    col++;

                    //ver_code
                    exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                        11, false, false, false);
                    exRange.Value2 = item.MainId;
                    col++;

                    //response_id
                    exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                        11, false, false, false);
                    exRange.Value2 = item.ResponseId;
                    col++;

                    //sales_method
                    exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                        11, false, false, false);
                    exRange.Value2 = ""; //intentionally blank

                    col = dataColumnInitialize;//reset col to A
                    row++;
                }
            }

            #endregion Write Out Data
        }

        private static void WriteReportForNoSalesMTDDetail(ref Excel.Application exApp, ref Excel.Range exRange, DateTime startDate, DateTime endDate, string header, List<Vendors> vendorList)
        {
            int rowInitialize = 1; //initial seed for the row data
            int row = 0;// where we start the row data

            int dataColumnInitialize = 65; //initial seed for column data - column  A
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

            row++;

            //vendor_name
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "vendor_name";
            col++;

            //tsr_id
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                 11, true, false, false);
            exRange.Value2 = "tsr_id";
            col++;

            //tsr_name
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "tsr_namel";
            col++;

            //status_txt
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "status_txt";
            col++;

            //reason
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "reason";
            col++;

            //p_date
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "p_date";
            col++;

            //sales_state
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "sales_state";
            col++;

            //fuel_type
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "fuel_type";
            col++;

            //dual_fuel
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "dual_fuel";
            col++;

            //btn
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "btn";
            col++;

            //acct_num
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "acct_num";
            col++;

            //auth_fname
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "auth_fname";
            col++;

            //auth_lname
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "auth_lname";
            col++;

            //bill_fname
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "bill_fname";
            col++;

            //bill_lname
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "bill_lname";
            col++;

            //addr1
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "addr1";
            col++;

            //addr2
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "addr2";
            col++;

            //city
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "city";
            col++;

            //state
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "state";
            col++;

            //zip
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "zip";
            col++;

            //ldc_code
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "ldc_code";
            col++;

            //rate
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "rate";
            col++;

            //term
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "term";
            col++;

            //ver_code
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "ver_code";
            col++;

            //response_id
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "response_id";
            col++;

            //sales_method
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "sales_method";

            col = dataColumnInitialize;//reset col to A
            row++;

            #endregion Header

            #region Data

            IList<spMTDCallDetailByVendor_Result> mtdCallDeatilResult = GetNoSalesMTDDetail(startDate, endDate);

            #endregion Data

            #region Write Out Data

            //Data map from the stored procedure

            //VendorName = vendor_name
            //VendorAgentId = tsr_id
            //AgentName = tsr_name
            //VerifiedFormattedExport = status_txt
            //Concern = reason
            //CallDateTime = DataFormatString="{0:MM/dd/yyyy} = p_date
            //ServiceState = sales_state
            //SignUpType = fuel_type
            //DualSignUp = dual_fuel
            //ServicePhoneNumber = btn
            //UDCAccountNumber =  acct_num
            //ServiceFirstName = auth_fname
            //ServiceLastName = auth_lname
            //BillingFirstName = bill_fname
            //BillingLastName = bill_lname
            //BillingAddress1 = addr1
            //BillingAddress2 = addr2
            //BillingCity = city
            //BillingState = state
            //BillingZipCode = zip
            //UDCCode = ldc_code
            //Rate = rate
            //Term = term
            //MainId = ver_code
            //ResponseId = response_id

            foreach (var item in mtdCallDeatilResult)
            {
                if (mtdCallDeatilResult.Count != 0)
                {
                    //vendor_name
                    exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                        11, false, false, false);
                    exRange.Value2 = item.VendorName;
                    col++;

                    //tsr_id
                    exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                        11, false, false, false);
                    exRange.Value2 = item.VendorAgentId;
                    col++;

                    //tsr_name
                    exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                        11, false, false, false);
                    exRange.Value2 = item.AgentName;
                    col++;

                    //status_txt
                    exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                        11, false, false, false);
                    exRange.Value2 = (item.Verified == "1" ? "Yes" : "No");
                    col++;

                    //reason
                    exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                        11, false, false, false);
                    exRange.Value2 = item.Concern;
                    col++;

                    //p_date
                    exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                        11, false, false, false);
                    exRange.NumberFormat = "mm/dd/yyyy";
                    exRange.Value2 = item.CallDateTime;

                    col++;
                    //sales_state
                    exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                        11, false, false, false);
                    exRange.Value2 = item.ServiceState;
                    col++;

                    //fuel_type
                    exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                        11, false, false, false);
                    exRange.Value2 = item.SignUpType;
                    col++;

                    //dual_fuel
                    exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                        11, false, false, false);
                    exRange.Value2 = item.DualSignUp;
                    col++;

                    //btn
                    exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                        11, false, false, false);
                    exRange.Value2 = item.ServicePhoneNumber;
                    col++;

                    //acct_num
                    exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                        11, false, false, false);
                    exRange.NumberFormat = "@";
                    exRange.Value2 = item.UDCAccountNumber;
                    col++;

                    //auth_fname
                    exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                        11, false, false, false);
                    exRange.Value2 = item.ServiceFirstName;
                    col++;

                    //auth_lname
                    exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                        11, false, false, false);
                    exRange.Value2 = item.ServiceLastName;
                    col++;

                    //bill_fname
                    exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                        11, false, false, false);
                    exRange.Value2 = item.BillingFirstName;
                    col++;

                    //bill_lname
                    exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                        11, false, false, false);
                    exRange.Value2 = item.BillingLastName;
                    col++;

                    //addr1
                    exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                        11, false, false, false);
                    exRange.Value2 = item.BillingAddress1;
                    col++;

                    //addr2
                    exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                        11, false, false, false);
                    exRange.Value2 = item.BillingAddress2;
                    col++;

                    //city
                    exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                        11, false, false, false);
                    exRange.Value2 = item.BillingCity;
                    col++;

                    //state
                    exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                        11, false, false, false);
                    exRange.Value2 = item.BillingState;
                    col++;

                    //zip
                    exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                        11, false, false, false);
                    exRange.Value2 = item.BillingZipCode;
                    col++;

                    //ldc_code
                    exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                        11, false, false, false);
                    exRange.Value2 = item.UDCCode;
                    col++;

                    //rate
                    exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                        11, false, false, false);
                    exRange.Value2 = item.Rate;
                    col++;

                    //term
                    exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                        11, false, false, false);
                    exRange.Value2 = item.Term;
                    col++;

                    //ver_code
                    exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                        11, false, false, false);
                    exRange.Value2 = item.MainId;
                    col++;

                    //response_id
                    exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                        11, false, false, false);
                    exRange.Value2 = item.ResponseId;
                    col++;

                    //sales_method
                    exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                        11, false, false, false);
                    exRange.Value2 = ""; //intentionally blank

                    col = dataColumnInitialize;//reset col to A
                    row++;
                }
            }

            #endregion Write Out Data
        }

        #endregion WriteReportForNoSalesMTDDetail_EXCEL (2 methods)

        #region WriteReportForMTDSalesRepByDisposition_EXCEL (2 methods)

        private static void WriteReportForMTDSalesRepByDisposition(ref Excel.Application exApp, ref Excel.Range exRange, DateTime startDate, DateTime endDate, Vendors vendor)
        {
            int rowInitialize = 1; //initial seed for the row data
            int row = 0;// where we start the row data

            int dataColumnInitialize = 65; //initial seed for column data - column  A
            int col = 0;

            row = rowInitialize;  //set the row for the data
            col = dataColumnInitialize;//set the column for the data

            List<Disposition> NonVerifiedDispositionList = GetNonVerifiedDispositionList(startDate, endDate);

            //foreach (var pair in Vendor)
            //foreach (Vendors vendor in vendorList)
            //{

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

            #endregion Header

            #region Sales Rep Data Totals

            //Get list of Sales Reps for a Vendor and DateRange
            List<SalesRep> SalesRepList = GetMTDSalesRepByDisposition(startDate, endDate, vendor.VendorId.ToString(), NonVerifiedDispositionList);

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

            #endregion Sales Rep Data Totals

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

            row++;

            col = dataColumnInitialize;//reset column back to A
            row++;

            #endregion Grand Totals

            //}
        }

        private static void WriteReportForMTDSalesRepByDisposition(ref Excel.Application exApp, ref Excel.Range exRange, DateTime startDate, DateTime endDate, List<Vendors> vendorList)
        {
            int rowInitialize = 1; //initial seed for the row data
            int row = 0;// where we start the row data

            int dataColumnInitialize = 65; //initial seed for column data - column  A
            int col = 0;

            row = rowInitialize;  //set the row for the data
            col = dataColumnInitialize;//set the column for the data

            List<Disposition> NonVerifiedDispositionList = GetNonVerifiedDispositionList(startDate, endDate);

            //foreach (var pair in Vendor)
            foreach (Vendors vendor in vendorList)
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

                #endregion Header

                #region Sales Rep Data Totals

                //Get list of Sales Reps for a Vendor and DateRange
                List<SalesRep> SalesRepList = GetMTDSalesRepByDisposition(startDate, endDate, vendor.VendorId.ToString(), NonVerifiedDispositionList);

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

                #endregion Sales Rep Data Totals

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

                row++;

                col = dataColumnInitialize;//reset column back to A
                row++;

                #endregion Grand Totals
            }
        }

        #endregion WriteReportForMTDSalesRepByDisposition_EXCEL (2 methods)

        #endregion Excel

        #region Get Data

        #region Get List of Vendors (1 method)

        /// <summary>
        /// Gets list of Vendors but excludes the Administrator
        /// </summary>
        /// <returns></returns>
        private static List<Vendors> GetVendorList()
        {
            List<Vendors> vendors = new List<Vendors>();
            using (ConstellationEntities entitites = new ConstellationEntities())
            {
                var query = from v in entitites.tblVendors
                            where v.VendorId != 0
                            select v;

                foreach (var v in query)
                {
                    Vendors vendor = new Vendors(v.VendorId, v.VendorName);
                    vendors.Add(vendor);
                }
            }

            return vendors;
        }

        #endregion Get List of Vendors (1 method)

        //#region Get Vendor List (1 method)
        ///// <summary>
        ///// Gets a list of Vendors
        ///// </summary>
        ///// <returns>All Vendors excluding the Administrator = 0</returns>
        //private static List<tblVendor> GetVendors()
        //{
        //    List<tblVendor> vendors = new List<tblVendor>();
        //    using (ConstellationEntities data = new ConstellationEntities())
        //    {
        //        //exclude the Administrator = 0 for VendorId
        //        vendors = data.tblVendors.Where(v => v.VendorId != 0).ToList();
        //    }
        //    return vendors;
        //}
        //#endregion

        #region Get State List (2 methods)

        #region Get List of States in a date range for a Specific Vendor

        private static List<string> GetStates(DateTime startDate, DateTime endDate, string vendorId)
        {
            List<string> states = new List<string>();
            using (ConstellationEntities entities = new ConstellationEntities())
            {
                var query = (from m in entities.tblMains
                             where m.CallDateTime > startDate
                                  && m.CallDateTime < endDate
                                  && m.ServiceState != null
                                  && m.VendorId == vendorId
                             select m.ServiceState).Distinct().OrderBy(servicestate => servicestate).ToList();

                foreach (var item in query)
                {
                    states.Add(item);
                }
            }
            return states;
        }

        #endregion Get List of States in a date range for a Specific Vendor

        #region Get List of States in a date range

        private static List<string> GetStates(DateTime startDate, DateTime endDate)
        {
            List<string> states = new List<string>();
            using (ConstellationEntities entities = new ConstellationEntities())
            {
                var query = (from m in entities.tblMains
                             where m.CallDateTime > startDate
                                  && m.CallDateTime < endDate
                                  && m.ServiceState != null
                             select m.ServiceState).Distinct().OrderBy(servicestate => servicestate).ToList();

                foreach (var item in query)
                {
                    states.Add(item);
                }
            }
            return states;
        }

        #endregion Get List of States in a date range

        #endregion Get State List (2 methods)

        #region Get Disposition List (2 methods)

        private static List<Disposition> GetDispositionList(DateTime startDate, DateTime endDate)
        {
            //SELECT COUNT(MainId), Concern
            //FROM [Constellation].[dbo].[tblMain]
            //WHERE CallDateTime > startDate and CallDateTime < endDate
            //AND Concern is not null
            //GROUP BY Concern
            //ORDER BY Concern

            List<Disposition> dispositionList = new List<Disposition>();

            try
            {
                using (ConstellationEntities entities = new ConstellationEntities())
                {
                    var query = (from m in entities.tblMains
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
            //FROM [Constellation].[dbo].[tblMain]
            //WHERE CallDateTime > startDate and CallDateTime < endDate
            //AND Concern is not null
            //AND Verified != '1'
            //GROUP BY Concern
            //ORDER BY Concern

            List<Disposition> nonVerifiedDispositionList = new List<Disposition>();

            try
            {
                using (ConstellationEntities entities = new ConstellationEntities())
                {
                    var query = (from m in entities.tblMains
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

        #endregion Get Disposition List (2 methods)

        #region Overall Data (4 methods)

        #region Gets Total Verifications performed for a date range and specific Vendor

        private static int GetTotalVerificationsPerformed_Overall(DateTime startDate, DateTime endDate, string vendorId)
        {
            //SELECT count(mainid) AS [Total Verifications Performed]
            //FROM [Constellation].[dbo].[tblMain]
            //WHERE calldatetime > startDate and calldatetime < endDate
            //and VendorId=vendorId
            //AND concern is not null
            int total = 0;
            try
            {
                using (ConstellationEntities entities = new ConstellationEntities())
                {
                    var query = (from m in entities.tblMains
                                 where m.CallDateTime > startDate
                                  && m.CallDateTime < endDate
                                  && m.VendorId == vendorId
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

        #endregion Gets Total Verifications performed for a date range and specific Vendor

        #region Gets Total Verifications performed for a date range

        private static int GetTotalVerificationsPerformed_Overall(DateTime startDate, DateTime endDate)
        {
            //SELECT count(mainid) AS [Total Verifications Performed]
            //FROM [Constellation].[dbo].[tblMain]
            //WHERE calldatetime > startDate and calldatetime < endDate
            //AND concern is not null
            int total = 0;
            try
            {
                using (ConstellationEntities entities = new ConstellationEntities())
                {
                    var query = (from m in entities.tblMains
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

        #endregion Gets Total Verifications performed for a date range

        #region Gets All Good Sales performed for a date range and specific Vendor

        private static int GetTotalGoodSales_Overall(DateTime startDate, DateTime endDate, string vendorId)
        {
            //SELECT count(mainid) AS [Good Sale]
            //FROM [Constellation].[dbo].[tblMain]
            //WHERE calldatetime > startDate and calldatetime < endDate
            //AND concern ='1'
            //AND verified ='1'
            int total = 0;
            try
            {
                using (ConstellationEntities entities = new ConstellationEntities())
                {
                    var query = (from m in entities.tblMains
                                 where m.CallDateTime > startDate
                                 && m.CallDateTime < endDate
                                 && m.Concern != null
                                 && m.Verified == "1"
                                 && m.VendorId == vendorId
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

        #endregion Gets All Good Sales performed for a date range and specific Vendor

        #region Gets All Good Sales performed for a date range

        private static int GetTotalGoodSales_Overall(DateTime startDate, DateTime endDate)
        {
            //SELECT count(mainid) AS [Good Sale]
            //FROM [Constellation].[dbo].[tblMain]
            //WHERE calldatetime > startDate and calldatetime < endDate
            //AND concern ='1'
            //AND verified ='1'
            int total = 0;
            try
            {
                using (ConstellationEntities entities = new ConstellationEntities())
                {
                    var query = (from m in entities.tblMains
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

        #endregion Gets All Good Sales performed for a date range

        #endregion Overall Data (4 methods)

        #region By Channel Data (4 methods)

        #region Gets Total Verifications performed for a date range by channel for a Specific Vendor

        private static int GetTotalVerificationsPerformed_ByChannel(DateTime startDate, DateTime endDate, string dnis, string vendorId)
        {
            //SELECT count(mainid) AS [Total Verifications Performed]
            //FROM [Constellation].[dbo].[tblMain]
            //WHERE calldatetime > startDate and calldatetime < endDate
            //AND dnis = dnis
            //AND concern is not null
            //AND vendorid = vendorid
            int total = 0;
            try
            {
                using (ConstellationEntities entities = new ConstellationEntities())
                {
                    var query = (from m in entities.tblMains
                                 where m.CallDateTime > startDate
                                  && m.CallDateTime < endDate
                                  && m.Dnis == dnis
                                  && m.Concern != null
                                  && m.VendorId == vendorId
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

        #endregion Gets Total Verifications performed for a date range by channel for a Specific Vendor

        #region Gets Total Verifications performed for a date range by channel

        private static int GetTotalVerificationsPerformed_ByChannel(DateTime startDate, DateTime endDate, string dnis)
        {
            //SELECT count(mainid) AS [Total Verifications Performed]
            //FROM [Constellation].[dbo].[tblMain]
            //WHERE calldatetime > startDate and calldatetime < endDate
            //AND dnis = dnis
            //AND concern is not null
            int total = 0;
            try
            {
                using (ConstellationEntities entities = new ConstellationEntities())
                {
                    var query = (from m in entities.tblMains
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

        #endregion Gets Total Verifications performed for a date range by channel

        #region Gets All Good Sales performed for a date range by channel for a Specific Vendor

        private static int GetTotalGoodSales_ByChannel(DateTime startDate, DateTime endDate, string dnis, string vendorId)
        {
            //SELECT count(mainid) AS [Good Sale]
            //FROM [Constellation].[dbo].[tblMain]
            //WHERE calldatetime > startDate and calldatetime < endDate
            //AND dnis = dnis
            //AND concern ='1'
            //AND verified ='1'
            //AND vendorid = vendorid
            int total = 0;
            try
            {
                using (ConstellationEntities entities = new ConstellationEntities())
                {
                    var query = (from m in entities.tblMains
                                 where m.CallDateTime > startDate
                                 && m.CallDateTime < endDate
                                 && m.Dnis == dnis
                                 && m.Concern != null
                                 && m.Verified == "1"
                                 && m.VendorId == vendorId
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

        #endregion Gets All Good Sales performed for a date range by channel for a Specific Vendor

        #region Gets All Good Sales performed for a date range by channel

        private static int GetTotalGoodSales_ByChannel(DateTime startDate, DateTime endDate, string dnis)
        {
            //SELECT count(mainid) AS [Good Sale]
            //FROM [Constellation].[dbo].[tblMain]
            //WHERE calldatetime > startDate and calldatetime < endDate
            //AND dnis = dnis
            //AND concern ='1'
            //AND verified ='1'
            int total = 0;
            try
            {
                using (ConstellationEntities entities = new ConstellationEntities())
                {
                    var query = (from m in entities.tblMains
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

        #endregion Gets All Good Sales performed for a date range by channel

        #endregion By Channel Data (4 methods)

        #region By State Data (4 methods)

        #region Gets Total Verifications performed for a date range by State for a Specific Vendor

        private static int GetTotalVerificationsPerformed_ByState(DateTime startDate, DateTime endDate, string state, string vendorId)
        {
            //SELECT count(mainid) AS [Total Verifications Performed]
            //FROM [Constellation].[dbo].[tblMain]
            //WHERE calldatetime > startDate and calldatetime < endDate
            //AND servicestate = state
            //AND concern is not null
            //AND vendorId = vendorId
            int total = 0;
            try
            {
                using (ConstellationEntities entities = new ConstellationEntities())
                {
                    var query = (from m in entities.tblMains
                                 where m.CallDateTime > startDate
                                  && m.CallDateTime < endDate
                                  && m.ServiceState == state
                                  && m.Concern != null
                                  && m.VendorId == vendorId
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

        #endregion Gets Total Verifications performed for a date range by State for a Specific Vendor

        #region Gets Total Verifications performed for a date range by State

        private static int GetTotalVerificationsPerformed_ByState(DateTime startDate, DateTime endDate, string state)
        {
            //SELECT count(mainid) AS [Total Verifications Performed]
            //FROM [Constellation].[dbo].[tblMain]
            //WHERE calldatetime > startDate and calldatetime < endDate
            //AND servicestate = state
            //AND concern is not null
            int total = 0;
            try
            {
                using (ConstellationEntities entities = new ConstellationEntities())
                {
                    var query = (from m in entities.tblMains
                                 where m.CallDateTime > startDate
                                  && m.CallDateTime < endDate
                                  && m.ServiceState == state
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

        #endregion Gets Total Verifications performed for a date range by State

        #region Gets All Good Sales performed for a date range by State for a Specific Vendor

        private static int GetTotalGoodSales_ByState(DateTime startDate, DateTime endDate, string state, string vendorId)
        {
            //SELECT count(mainid) AS [Good Sale]
            //FROM [Constellation].[dbo].[tblMain]
            //WHERE calldatetime > startDate and calldatetime < endDate
            //AND servicestate = state
            //AND concern ='1'
            //AND verified ='1'
            //AND vendorId = vendorId
            int total = 0;
            try
            {
                using (ConstellationEntities entities = new ConstellationEntities())
                {
                    var query = (from m in entities.tblMains
                                 where m.CallDateTime > startDate
                                 && m.CallDateTime < endDate
                                 && m.ServiceState == state
                                 && m.Concern != null
                                 && m.Verified == "1"
                                 && m.VendorId == vendorId
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

        #endregion Gets All Good Sales performed for a date range by State for a Specific Vendor

        #region Gets All Good Sales performed for a date range by State

        private static int GetTotalGoodSales_ByState(DateTime startDate, DateTime endDate, string state)
        {
            //SELECT count(mainid) AS [Good Sale]
            //FROM [Constellation].[dbo].[tblMain]
            //WHERE calldatetime > startDate and calldatetime < endDate
            //AND servicestate = state
            //AND concern ='1'
            //AND verified ='1'
            int total = 0;
            try
            {
                using (ConstellationEntities entities = new ConstellationEntities())
                {
                    var query = (from m in entities.tblMains
                                 where m.CallDateTime > startDate
                                 && m.CallDateTime < endDate
                                 && m.ServiceState == state
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

        #endregion Gets All Good Sales performed for a date range by State

        #endregion By State Data (4 methods)

        #region Disposition By State (2 methods)

        private static int GetDispositionCount_ByState(DateTime startDate, DateTime endDate, string state, string concern, string vendorId)
        {
            //Not a true SQL representation, just used
            //to quickly check all dispositions for a state and date range

            //SELECT concern, count(mainid) AS [Count]
            //FROM [Constellation].[dbo].[tblMain]
            //WHERE CallDateTime > startdate and CallDateTime < enddate
            //AND servicestate = state
            //AND Concern is not null
            //AND VendorId = vendorId
            //group by concern
            //order by concern desc

            int total = 0;
            try
            {
                using (ConstellationEntities entities = new ConstellationEntities())
                {
                    var query = (from m in entities.tblMains
                                 where m.CallDateTime > startDate
                                 && m.CallDateTime < endDate
                                 && m.ServiceState == state
                                 && m.Concern == concern
                                 && m.VendorId == vendorId
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

        private static int GetDispositionCount_ByState(DateTime startDate, DateTime endDate, string state, string concern)
        {
            //Not a true SQL representation, just used
            //to quickly check all dispositions for a state and date range

            //SELECT concern, count(mainid) AS [Count]
            //FROM [Constellation].[dbo].[tblMain]
            //WHERE CallDateTime > startdate and CallDateTime < enddate
            //AND servicestate = state
            //AND Concern is not null
            //group by concern
            //order by concern desc

            int total = 0;
            try
            {
                using (ConstellationEntities entities = new ConstellationEntities())
                {
                    var query = (from m in entities.tblMains
                                 where m.CallDateTime > startDate
                                 && m.CallDateTime < endDate
                                 && m.ServiceState == state
                                 && m.Concern == concern

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

        #endregion Disposition By State (2 methods)

        #region By Vendor Data (1 method)

        #region Gets Total Verifications performed for a date range by Vendor

        private static int GetTotalVerificationsPerformed_ByVendor(DateTime startDate, DateTime endDate, string vendorid)
        {
            //SELECT count(mainid) AS [Total Verifications Performed]
            //FROM [Constellation].[dbo].[tblMain]
            //WHERE calldatetime > startDate and calldatetime < endDate
            //AND vendorid = vendorid
            //AND concern is not null
            int total = 0;
            try
            {
                using (ConstellationEntities entities = new ConstellationEntities())
                {
                    var query = (from m in entities.tblMains
                                 where m.CallDateTime > startDate
                                  && m.CallDateTime < endDate
                                  && m.VendorId == vendorid
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

        #endregion Gets Total Verifications performed for a date range by Vendor

        #endregion By Vendor Data (1 method)

        #region Gets All Good Sales performed for a date range by Vendor (1 method)

        private static int GetTotalGoodSales_ByVendor(DateTime startDate, DateTime endDate, string vendorId)
        {
            //SELECT count(mainid) AS [Good Sale]
            //FROM [Constellation].[dbo].[tblMain]
            //WHERE calldatetime > startDate and calldatetime < endDate
            //AND vendorid = vendorid
            //AND concern ='1'
            //AND verified ='1'
            int total = 0;
            try
            {
                using (ConstellationEntities entities = new ConstellationEntities())
                {
                    var query = (from m in entities.tblMains
                                 where m.CallDateTime > startDate
                                 && m.CallDateTime < endDate
                                 && m.VendorId == vendorId
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

        #endregion Gets All Good Sales performed for a date range by Vendor (1 method)

        #region Disposition By Vendor (1 method)

        private static int GetDispositionCount_ByVendor(DateTime startDate, DateTime endDate, string vendorid, string concern)
        {
            //Not a true SQL representation, just used
            //to quickly check all dispositions for a state and date range

            //SELECT concern, count(mainid) AS [Count]
            //FROM [Constellation].[dbo].[tblMain]
            //WHERE CallDateTime > startdate and CallDateTime < enddate
            //AND vendorid = vendorid
            //AND Concern is not null
            //group by concern
            //order by concern desc

            int total = 0;
            try
            {
                using (ConstellationEntities entities = new ConstellationEntities())
                {
                    var query = (from m in entities.tblMains
                                 where m.CallDateTime > startDate
                                 && m.CallDateTime < endDate
                                 && m.VendorId == vendorid
                                 && m.Concern == concern

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

        #endregion Disposition By Vendor (1 method)

        #region No Sales MTD Detail (2 methods)

        /// <summary>
        /// Gets a list of concern details for a Specific Vendor in a date range
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="vendorId"></param>
        /// <returns></returns>
        private static IList<spMTDCallDetailByVendor_Result> GetNoSalesMTDDetail(DateTime startDate, DateTime endDate, int vendorId)
        {
            IList<spMTDCallDetailByVendor_Result> spResult = null;

            try
            {
                using (ConstellationEntities entities = new ConstellationEntities())
                {
                    spResult = entities.spMTDCallDetailByVendor(startDate: startDate, endDate: endDate, vendorId: vendorId).ToList();
                }
            }
            catch (Exception ex)
            {
                SendErrorMessage(ex);
                //throw ex;
            }

            return spResult;
        }

        // <summary>
        /// Gets a list of concern details for All Vendors in a date range
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        private static IList<spMTDCallDetailByVendor_Result> GetNoSalesMTDDetail(DateTime startDate, DateTime endDate)
        {
            IList<spMTDCallDetailByVendor_Result> spResult = null;

            try
            {
                using (ConstellationEntities entities = new ConstellationEntities())
                {
                    spResult = entities.spMTDCallDetailByVendor(startDate: startDate, endDate: endDate, vendorId: null).ToList();
                }
            }
            catch (Exception ex)
            {
                SendErrorMessage(ex);
                //throw ex;
            }

            return spResult;
        }

        //private static IList<spMTDCallDetail_Result> GetNoSalesMTDDetail(DateTime startDate, DateTime endDate)
        //{
        //    IList<spMTDCallDetail_Result> spResult = null;

        //    try
        //    {
        //        using (ConstellationEntities entities = new ConstellationEntities())
        //        {
        //            spResult = entities.spMTDCallDetail(startDate: startDate, endDate: endDate).ToList();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        SendErrorMessage(ex);
        //        //throw ex;
        //    }

        //    return spResult;

        //}

        #endregion No Sales MTD Detail (2 methods)

        #region MTD Sales Rep by Disposition (1 method)

        private static List<SalesRep> GetMTDSalesRepByDisposition(DateTime startDate, DateTime endDate, string vendorId, List<Disposition> nonVerifiedDispositionList)
        {
            //Select a.FirstName +' ' + a.LastName as Name, m.Concern, count(m.Concern) as Total
            // FROM [Constellation].[dbo].[tblMain] m
            // join [Constellation].[dbo].[tblAgent] a on m.tblAgentKeyId = a.KeyId
            // where m.CallDateTime > endDate and m.CallDateTime < endDate
            // and m.VendorId = 44
            // and m.concern in (nonVerifiedDispositionList)
            // group by m.Concern, a.FirstName +' ' + a.LastName
            List<SalesRep> salesRep = new List<SalesRep>();

            try
            {
                List<string> myfilter = new List<string>();
                foreach (Disposition disp in nonVerifiedDispositionList)
                {
                    myfilter.Add(disp.Concern);
                }

                using (ConstellationEntities entities = new ConstellationEntities())
                {
                    //Get all salesreps for a vendorId and date range where verified is not 1
                    var salesRepQuery = (from m in entities.tblMains
                                         join a in entities.tblAgents on m.tblAgentKeyId equals a.KeyId
                                         where m.CallDateTime > startDate
                                         && m.CallDateTime < endDate
                                         && m.VendorId == vendorId
                                         && m.Verified != "1"
                                         group a by new { a.KeyId, a.FirstName, a.LastName } into aGroup
                                         select new
                                         {
                                             aGroup.Key.FirstName,
                                             aGroup.Key.LastName,
                                             aGroup.Key.KeyId
                                         }).OrderBy(group => group.KeyId);

                    foreach (var salerepitem in salesRepQuery)
                    {
                        //Get all dispositions for a sales rep with concernlist for a date range and vendorId
                        var dispositionQuery = (from m in entities.tblMains
                                                where m.CallDateTime > startDate
                                                && m.CallDateTime < endDate
                                                && m.VendorId == vendorId
                                                && m.tblAgentKeyId == salerepitem.KeyId
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

        #endregion MTD Sales Rep by Disposition (1 method)

        #endregion Get Data

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

        private static void SendEmail(ref string xlsFilePath, DateTime reportDate, string vendor)
        {
            //string strMsgBody = string.Empty;
            try
            {
                string strToEmail = ConfigurationManager.AppSettings["mailRecipientTO_" + vendor].ToString();

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

                mail.Subject = "Constellation " + (vendor == "Total" ? "All Vendors" : "Vendor: " + vendor) + " Energy Daily Activity Report for " + reportDate.ToString("MMM") + " " + reportDate.ToString("dd") + " " + reportDate.ToString("yyyy") + ".";

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
        private static void SaveXlsDocument(ref string reportPath, ref string xlsFilename, ref string xlsFilePath, Excel.Workbook exBook, DateTime reportDate, string vendor)
        {
            string vendorType = string.Empty;
            //Build the file name

            vendorType = vendor == "Total" ? "All Vendors" : vendor;
            xlsFilename = "ConstellationEnergyDailyActivityReport - " + vendorType + " - " + String.Format("{0:yyyyMMdd}", reportDate) + ".xlsx";

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
                MonthStartDate = new DateTime(baseDate.Year, baseDate.Month, 1, 0, 0, 0).AddMonths(-1); //Begginning of the previous month
            }
            else
            {
                MonthStartDate = new DateTime(baseDate.Year, baseDate.Month, 1, 0, 0, 0); //Begginning of current month
            }
            YearStartDate = new DateTime(baseDate.Year, 1, 1, 0, 0, 0);   //Beginning of current year
        }

        private static void SendErrorMessage(Exception ex)
        {
            Calibrus.ErrorHandler.Alerting alert = new Calibrus.ErrorHandler.Alerting("ConstellationEnergyDailyActivityReport");
            alert.SendAlert(ex.Source, ex.Message, Environment.MachineName, Environment.UserName, Environment.Version.ToString());
        }

        #endregion Utilities
    }
}