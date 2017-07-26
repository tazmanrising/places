using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Configuration;
using Calibrus.Mail;
using Calibrus.ErrorHandler;
using Calibrus.ExcelFunctions;
using Excel = Microsoft.Office.Interop.Excel;


namespace ConstellationNoSaleXLSReport
{
    public class NoSaleReport
    {
        public static object na = System.Reflection.Missing.Value;

        #region Main
        public static void Main(string[] args)
        {
            string rootPath = string.Empty;
            //get report interval
            DateTime StartDate = new DateTime();
            DateTime EndDate = new DateTime();
            DateTime YTDStartDate = new DateTime();

            //start to  build the form pathing
            string xlsFilename = string.Empty;
            string xlsFilePath = string.Empty;

            if (args.Length > 0)
            {
                if (DateTime.TryParse(args[0], out StartDate))
                {
                    EndDate = new DateTime(StartDate.Year, StartDate.Month, StartDate.Day, 0, 0, 0);//current date time should be first of the current month
                    YTDStartDate = new DateTime(StartDate.Year, 1, 1, 0, 0, 0); //Start of the current Year
                    StartDate = new DateTime(StartDate.Year, StartDate.Month, StartDate.Day, 0, 0, 0).AddMonths(-1);//Previous Month date time
                    //GetDates(out StartDate, out EndDate, out YTDStartDate);
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
                GetDates(out StartDate, out EndDate, out YTDStartDate);
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
                    //Set global attributes
                    exApp.StandardFont = "Calibri";
                    exApp.StandardFontSize = 11;

                    exBook = exApp.Workbooks.Add(na);
                    exApp.Visible = false;

                    #region All No Sales - 30 Days Tab
                    if (sheetsAdded < exBook.Sheets.Count)
                    {
                        exSheet = (Excel.Worksheet)exBook.Sheets[sheetsAdded + 1];
                    }
                    else
                    {
                        exSheet = (Excel.Worksheet)exBook.Sheets.Add(na, exBook.ActiveSheet, na, na);
                    }

                    sheetsAdded++;

                    //newtab
                    string sheetName = "All No Sales - 30 Days";
                    exSheet.Name = sheetName.Length > 30 ? sheetName.Substring(0, 30) : sheetName; //force length of sheet name due to excel constraints
                    exSheet.Select(na);

                    //write out All No Sales - 30 Days
                    WriteReportAllNoSales(ref exApp, ref exRange, StartDate, EndDate, vendor);
                    #endregion


                    #region All No Sales - 30 Day Detail Tab
                    if (sheetsAdded < exBook.Sheets.Count)
                    {
                        exSheet = (Excel.Worksheet)exBook.Sheets[sheetsAdded + 1];
                    }
                    else
                    {
                        exSheet = (Excel.Worksheet)exBook.Sheets.Add(na, exBook.ActiveSheet, na, na);
                    }

                    sheetsAdded++;


                    //newtab
                    sheetName = "All No Sales - 30 Day Detail";
                    exSheet.Name = sheetName.Length > 30 ? sheetName.Substring(0, 30) : sheetName; //force length of sheet name due to excel constraints
                    exSheet.Select(na);

                    //write out All No Sales - 30 Day Detail
                    WriteReportAllNoSales30DayDetail(ref exApp, ref exRange, StartDate, EndDate, vendor);
                    #endregion


                    #region All No Sales - YTD Tab
                    if (sheetsAdded < exBook.Sheets.Count)
                    {
                        exSheet = (Excel.Worksheet)exBook.Sheets[sheetsAdded + 1];
                    }
                    else
                    {
                        exSheet = (Excel.Worksheet)exBook.Sheets.Add(na, exBook.ActiveSheet, na, na);
                    }

                    sheetsAdded++;

                    //newtab
                    sheetName = "All No Sales - YTD";
                    exSheet.Name = sheetName.Length > 30 ? sheetName.Substring(0, 30) : sheetName; //force length of sheet name due to excel constraints
                    exSheet.Select(na);

                    //write out ALl No Sales - YTD
                    WriteReportAllNoSales(ref exApp, ref exRange, YTDStartDate, EndDate, vendor);
                    #endregion

                    //select first sheet in workbook
                    exSheet = (Excel.Worksheet)exApp.Worksheets[1];
                    exSheet.Select(na);

                    //save report
                    //SaveXlsDocument(ref rootPath, ref xlsFilename, ref xlsFilePath, exBook, EndDate, vendor.VendorId.ToString());
                    SaveXlsDocument(ref rootPath, ref xlsFilename, ref xlsFilePath, exBook, EndDate, vendor.VendorName);

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

                //email report
                //SendEmail(ref xlsFilePath, EndDate, vendor.VendorId.ToString());
                SendEmail(ref xlsFilePath, EndDate, vendor.VendorName);
            }
            #endregion

            #region All Vendors
            //Then write out for All Vendors
            sheetsAdded = 0;

            try
            {
                //Set global attributes
                exApp.StandardFont = "Calibri";
                exApp.StandardFontSize = 11;

                exBook = exApp.Workbooks.Add(na);
                exApp.Visible = false;

                #region All No Sales - 30 Days Tab
                if (sheetsAdded < exBook.Sheets.Count)
                {
                    exSheet = (Excel.Worksheet)exBook.Sheets[sheetsAdded + 1];
                }
                else
                {
                    exSheet = (Excel.Worksheet)exBook.Sheets.Add(na, exBook.ActiveSheet, na, na);
                }

                sheetsAdded++;

                //newtab
                string sheetName = "All No Sales - 30 Days";
                exSheet.Name = sheetName.Length > 30 ? sheetName.Substring(0, 30) : sheetName; //force length of sheet name due to excel constraints
                exSheet.Select(na);

                //write out All No Sales - 30 Days
                WriteReportAllNoSales(ref exApp, ref exRange, StartDate, EndDate, vendorsList);
                #endregion


                #region All No Sales - 30 Day Detail Tab
                if (sheetsAdded < exBook.Sheets.Count)
                {
                    exSheet = (Excel.Worksheet)exBook.Sheets[sheetsAdded + 1];
                }
                else
                {
                    exSheet = (Excel.Worksheet)exBook.Sheets.Add(na, exBook.ActiveSheet, na, na);
                }

                sheetsAdded++;


                //newtab
                sheetName = "All No Sales - 30 Day Detail";
                exSheet.Name = sheetName.Length > 30 ? sheetName.Substring(0, 30) : sheetName; //force length of sheet name due to excel constraints
                exSheet.Select(na);

                //write out All No Sales - 30 Day Detail
                WriteReportAllNoSales30DayDetail(ref exApp, ref exRange, StartDate, EndDate);
                #endregion


                #region All No Sales - YTD Tab
                if (sheetsAdded < exBook.Sheets.Count)
                {
                    exSheet = (Excel.Worksheet)exBook.Sheets[sheetsAdded + 1];
                }
                else
                {
                    exSheet = (Excel.Worksheet)exBook.Sheets.Add(na, exBook.ActiveSheet, na, na);
                }

                sheetsAdded++;

                //newtab
                sheetName = "All No Sales - YTD";
                exSheet.Name = sheetName.Length > 30 ? sheetName.Substring(0, 30) : sheetName; //force length of sheet name due to excel constraints
                exSheet.Select(na);

                //write out ALl No Sales - YTD
                WriteReportAllNoSales(ref exApp, ref exRange, YTDStartDate, EndDate, vendorsList);
                #endregion

                //select first sheet in workbook
                exSheet = (Excel.Worksheet)exApp.Worksheets[1];
                exSheet.Select(na);

                //save report
                SaveXlsDocument(ref rootPath, ref xlsFilename, ref xlsFilePath, exBook, EndDate, "Total");

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

            //email report
            SendEmail(ref xlsFilePath, EndDate, "Total");

            #endregion

        }
        #endregion

        #region Excel

        #region Write Report All No Sales 30 Days or YTD - (2 methods)
        //Specific Vendor
        /// <summary>
        /// writes out All No Sales for a specific vendor through a date range
        /// </summary>
        /// <param name="exApp"></param>
        /// <param name="exRange"></param>
        /// <param name="sDate"></param>
        /// <param name="eDate"></param>
        /// <param name="vendorid"></param>
        private static void WriteReportAllNoSales(ref Excel.Application exApp, ref Excel.Range exRange, DateTime sDate, DateTime eDate, Vendors vendor)
        {
            #region Variables
            Excel.Font exFont = null;
            //Placeholders as I move through the Excel sheet
            int rowInitialize = 1; //initial seed for the row data
            int colInitialize = 65; // column A
            int row = 0;// where we start the row data
            int col = 0;

            row = rowInitialize;  //set the row for the data   
            //col = colInitialize;//set the column for the data

            #endregion

            #region Header
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn("A", row), new RangeColumn("A", row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
              18, true, false, false);
            exRange.Value2 = string.Format("{0} through {1}", sDate.ToString("d"), eDate.AddDays(-1).ToString("d"));

            row++;

            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn("A", row), new RangeColumn("A", row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                18, true, false, false);
            exRange.Value2 = vendor.VendorName;

            row++;

            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn("A", row), new RangeColumn("A", row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                11, false, true, false);
            exRange.Value2 = "Reason:";

            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn("B", row), new RangeColumn("B", row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                11, false, true, false);
            exRange.Value2 = "Occurrences:";

            row++;

            #endregion

            #region Data
            List<Dispositions> NonVerifiedDispositionList = GetNonVerifiedDispositionList(sDate, eDate, vendor.VendorNumber);

            foreach (var noSales in NonVerifiedDispositionList)
            {
                //Reason
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn("A", row), new RangeColumn("A", row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                11, false, false, false);
                exRange.NumberFormat = "@";
                exRange.Value2 = string.Format("No Sale - {0}", noSales.Concern);

                //Occurences
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn("B", row), new RangeColumn("B", row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight,
                    11, false, false, false);
                exRange.Value2 = noSales.Count;
                row++;
            }

            #endregion

            exRange = (Excel.Range)exApp.get_Range("A1", "B1");
            exRange.EntireColumn.AutoFit();

        }

        //All Vendors
        /// <summary>
        /// writes out All No Sales for All Vendors
        /// </summary>
        /// <param name="exApp"></param>
        /// <param name="exRange"></param>
        /// <param name="sDate"></param>
        /// <param name="eDate"></param>
        /// <param name="vendorList"></param>
        private static void WriteReportAllNoSales(ref Excel.Application exApp, ref Excel.Range exRange, DateTime sDate, DateTime eDate, List<Vendors> vendorList)
        {
            #region Variables
            Excel.Font exFont = null;
            //Placeholders as I move through the Excel sheet
            int rowInitialize = 1; //initial seed for the row data
            int colInitialize = 65; // column A
            int row = 0;// where we start the row data
            int col = 0;

            row = rowInitialize;  //set the row for the data   
            //col = colInitialize;//set the column for the data

            #endregion

            #region All Vendors
            #region Header
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn("A", row), new RangeColumn("A", row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
              18, true, false, false);
            exRange.Value2 = string.Format("{0} through {1}", sDate.ToString("d"), eDate.AddDays(-1).ToString("d"));

            row++;

            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn("A", row), new RangeColumn("A", row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                18, true, false, false);
            exRange.Value2 = "All Vendors";

            row++;

            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn("A", row), new RangeColumn("A", row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                11, false, true, false);
            exRange.Value2 = "Reason:";

            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn("B", row), new RangeColumn("B", row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                11, false, true, false);
            exRange.Value2 = "Occurrences:";

            row++;

            #endregion
            #region Data
            List<Dispositions> NonVerifiedDispositionList_AllVendors = GetNonVerifiedDispositionList(sDate, eDate);

            foreach (var noSales in NonVerifiedDispositionList_AllVendors)
            {
                //Reason
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn("A", row), new RangeColumn("A", row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                11, false, false, false);
                exRange.NumberFormat = "@";
                exRange.Value2 = string.Format("No Sale - {0}", noSales.Concern);

                //Occurences
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn("B", row), new RangeColumn("B", row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight,
                    11, false, false, false);
                exRange.Value2 = noSales.Count;
                row++;
            }

            #endregion
            #endregion

            row++;

            #region Specific Vendors
            foreach (Vendors vendor in vendorList)
            {
                #region Header
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn("A", row), new RangeColumn("A", row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                    18, true, false, false);
                exRange.Value2 = vendor.VendorName;

                row++;

                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn("A", row), new RangeColumn("A", row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                    11, false, true, false);
                exRange.Value2 = "Reason:";

                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn("B", row), new RangeColumn("B", row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                    11, false, true, false);
                exRange.Value2 = "Occurrences:";

                row++;

                #endregion
                #region Data
                List<Dispositions> NonVerifiedDispositionList = GetNonVerifiedDispositionList(sDate, eDate, vendor.VendorNumber);

                foreach (var noSales in NonVerifiedDispositionList)
                {
                    //Reason
                    exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn("A", row), new RangeColumn("A", row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                    11, false, false, false);
                    exRange.NumberFormat = "@";
                    exRange.Value2 = string.Format("No Sale - {0}", noSales.Concern);

                    //Occurences
                    exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn("B", row), new RangeColumn("B", row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight,
                        11, false, false, false);
                    exRange.Value2 = noSales.Count;
                    row++;
                }

                #endregion

                row++;
            }
            #endregion

            exRange = (Excel.Range)exApp.get_Range("A1", "B1");
            exRange.EntireColumn.AutoFit();

        }

        #endregion

        #region Write Report All No Sales 30 Day Detail - (2 methods)
        //Specific Vendor
        private static void WriteReportAllNoSales30DayDetail(ref Excel.Application exApp, ref Excel.Range exRange, DateTime sDate, DateTime eDate, Vendors vendor)
        {
            int rowInitialize = 1; //initial seed for the row data
            int row = 0;// where we start the row data  

            int dataColumnInitialize = 65; //initial seed for column data - column  A
            int col = 0;

            row = rowInitialize;  //set the row for the data   
            col = dataColumnInitialize;//set the column for the data

            #region Header

            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn("A", row), new RangeColumn("D", row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                18, true, false, false);
            exRange.Merge(na);
            exRange.Value2 = string.Format("{0} through {1}", sDate.ToString("d"), eDate.AddDays(-1).ToString("d"));

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
            exRange.Value2 = "tsr_name";
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

            ////dual_fuel	
            //exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
            //    11, true, false, false);
            //exRange.Value2 = "dual_fuel";
            //col++;

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

            //electric_rate	
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "electic_rate";
            col++;

            //electric_term	
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "electric_term";
            col++;

            //gas_rate	
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "gas_rate";
            col++;

            //gas_term	
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "gas_term";
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


            #endregion

            #region Data

            IList<spNoSaleTMDetailByVendor_Result> mtdCallDeatilResult = GetNoSalesMTDDetail(sDate, eDate, vendor.VendorNumber);

            #endregion

            #region Write Out Data

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

                    ////dual_fuel	
                    //exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    //    11, false, false, false);
                    //exRange.Value2 = item.DualSignUp;
                    //col++;

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

                    //electric rate	
                    exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                        11, false, false, false);
                    exRange.Value2 = item.ElectricPrice;
                    col++;

                    //electreic term	
                    exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                        11, false, false, false);
                    exRange.Value2 = item.ElectricTerm;
                    col++;

                    //gas rate	
                    exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                        11, false, false, false);
                    exRange.Value2 = item.GasPrice;
                    col++;

                    //gas term	
                    exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                        11, false, false, false);
                    exRange.Value2 = item.GasTerm;
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

            #endregion

            exRange = (Excel.Range)exApp.get_Range("A1", "Z1");
            exRange.EntireColumn.AutoFit();

        }

        //All Vendors
        private static void WriteReportAllNoSales30DayDetail(ref Excel.Application exApp, ref Excel.Range exRange, DateTime sDate, DateTime eDate)
        {
            int rowInitialize = 1; //initial seed for the row data
            int row = 0;// where we start the row data  

            int dataColumnInitialize = 65; //initial seed for column data - column  A
            int col = 0;

            row = rowInitialize;  //set the row for the data   
            col = dataColumnInitialize;//set the column for the data

            #region Header

            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn("A", row), new RangeColumn("D", row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                18, true, false, false);
            exRange.Merge(na);
            exRange.Value2 = string.Format("{0} through {1}", sDate.ToString("d"), eDate.AddDays(-1).ToString("d"));

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
            exRange.Value2 = "tsr_name";
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

            ////dual_fuel	
            //exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
            //    11, true, false, false);
            //exRange.Value2 = "dual_fuel";
            //col++;

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

            //electric_rate	
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "electric_rate";
            col++;

            //electric_term	
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "electric_term";
            col++;

            //gas_rate	
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "gas_rate";
            col++;

            //gas_term	
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "gas_term";
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


            #endregion

            #region Data

            IList<spNoSaleTMDetailByVendor_Result> mtdCallDeatilResult = GetNoSalesMTDDetail(sDate, eDate);

            #endregion

            #region Write Out Data

            foreach (var item in mtdCallDeatilResult.OrderBy(s => s.VendorName).ThenBy(s => s.CallDateTime))
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

                    ////dual_fuel	
                    //exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    //    11, false, false, false);
                    //exRange.Value2 = item.DualSignUp;
                    //col++;

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

                    //electric rate	
                    exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                        11, false, false, false);
                    exRange.Value2 = item.ElectricPrice;
                    col++;

                    //electric term	
                    exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                        11, false, false, false);
                    exRange.Value2 = item.ElectricTerm;
                    col++;

                    //gas rate	
                    exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                        11, false, false, false);
                    exRange.Value2 = item.GasPrice;
                    col++;

                    //gas term	
                    exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                        11, false, false, false);
                    exRange.Value2 = item.GasTerm;
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

            #endregion



            exRange = (Excel.Range)exApp.get_Range("A1", "Z1");
            exRange.EntireColumn.AutoFit();

        }
        #endregion

        #endregion

        #region GetData

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
                var myInClause = new string[] { "44", "86" };//Forcing Protocall-TM and Global-TM since we have no distro list in the db

                var query = from v in entitites.Vendors
                            where myInClause.Contains(v.VendorNumber)
                            //&& v.SalesChannelId == 2
                            //&& v.IsActive == true                            
                            select v;

                foreach (var v in query)
                {
                    Vendors vendor = new Vendors(v.VendorId, v.VendorNumber, v.VendorName);
                    vendors.Add(vendor);
                }

            }

            return vendors;
        }
        #endregion

        #region All No Sales - 30 Day or YTD  (2 methods)
        /// <summary>
        /// Gets a list of concerns and their counts descending for a Specific Vendor in a date range, can be used by both Monthly and YTD tabs
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="vendorNumber"></param>
        /// <returns></returns>
        private static List<Dispositions> GetNonVerifiedDispositionList(DateTime startDate, DateTime endDate, string vendorNumber)
        {

            //SELECT Concern, COUNT(MainId) as Total
            //FROM [Constellation].[dbo].[tblMain]
            //WHERE CallDateTime > startDate and CallDateTime < endDate
            //and VendorId = vendorNumber
            //AND Concern is not null
            //AND Verified != '1'
            //GROUP BY  Concern
            //ORDER BY COUNT(MainId) desc

            List<Dispositions> nonVerifiedDispositionList = new List<Dispositions>();

            try
            {
                using (ConstellationEntities entities = new ConstellationEntities())
                {
                    var query = (from m in entities.tblMains
                                 where m.CallDateTime > startDate
                                  && m.CallDateTime < endDate
                                  && m.VendorId == vendorNumber
                                  && m.Concern != null
                                  && m.Verified != "1"
                                 group m by m.Concern into concernList
                                 select new
                                 {
                                     Concern = concernList.Key,
                                     Count = concernList.Count()
                                 }).OrderByDescending(group => group.Count);

                    foreach (var concernItem in query)
                    {
                        Dispositions myDisposition = new Dispositions(concernItem.Concern, concernItem.Count);

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

        /// <summary>
        /// Gets a list of concerns and their counts descending for All Vendors in a date range, can be used by both Monthly and YTD tabs
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        private static List<Dispositions> GetNonVerifiedDispositionList(DateTime startDate, DateTime endDate)
        {

            //SELECT Concern, COUNT(MainId) as Total
            //FROM [Constellation].[dbo].[tblMain]
            //WHERE CallDateTime > startDate and CallDateTime < endDate
            //AND Concern is not null
            //AND Verified != '1'
            //GROUP BY  Concern
            //ORDER BY COUNT(MainId) desc

            List<Dispositions> nonVerifiedDispositionList = new List<Dispositions>();

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
                                 }).OrderByDescending(group => group.Count);

                    foreach (var concernItem in query)
                    {
                        Dispositions myDisposition = new Dispositions(concernItem.Concern, concernItem.Count);

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

        #region All No Sales - 30 Day Detail (2 methods)

        /// <summary>
        /// Gets a list of concern details for a Specific Vendor in a date range
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="vendorNumber"></param>
        /// <returns></returns>
        private static IList<spNoSaleTMDetailByVendor_Result> GetNoSalesMTDDetail(DateTime startDate, DateTime endDate, string vendorNumber)
        {
            IList<spNoSaleTMDetailByVendor_Result> spResult = null;

            try
            {
                using (ConstellationEntities entities = new ConstellationEntities())
                {
                    spResult = entities.spNoSaleTMDetailByVendor(startDate: startDate, endDate: endDate, vendorNumber: vendorNumber).ToList();
                }
            }
            catch (Exception ex)
            {
                SendErrorMessage(ex);
                //throw ex;
            }

            return spResult;

        }

        /// <summary>
        /// Gets a list of concern details for All Vendors in a date range
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        private static IList<spNoSaleTMDetailByVendor_Result> GetNoSalesMTDDetail(DateTime startDate, DateTime endDate)
        {
            IList<spNoSaleTMDetailByVendor_Result> spResult = null;

            try
            {
                using (ConstellationEntities entities = new ConstellationEntities())
                {
                    spResult = entities.spNoSaleTMDetailByVendor(startDate: startDate, endDate: endDate, vendorNumber: null).ToList();
                }
            }
            catch (Exception ex)
            {
                SendErrorMessage(ex);
                //throw ex;
            }

            return spResult;

        }
        #endregion

        #endregion

        #region Utilities

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
        /// <summary>
        /// Saves XLS workbook document to a folder in the reportPath
        /// </summary>
        /// <param name="mainRecord"></param>
        /// <param name="reportPath"></param>
        /// <param name="xlsFilename"></param>
        /// <param name="xlsFilePath"></param>
        /// <param name="exBook"></param>
        private static void SaveXlsDocument(ref string reportPath, ref string xlsFilename, ref string xlsFilePath, Excel.Workbook exBook, DateTime currentDate, string vendor)
        {
            string vendorType = string.Empty;
            //Build the file name

            //vendorType = vendor == "Total" ? "Total" : "Vendor" + vendor;
            //xlsFilename = "Constellation_" + vendorType + "_NoSale" + String.Format("{0:yyyyMMdd}", currentDate) + ".xlsx";
            vendorType = vendor == "Total" ? "All Vendors" : vendor;
            xlsFilename = "CNE Res TM - No Sale Trending Report - " + vendorType + " - " + String.Format("{0:yyyyMMdd}", currentDate) + ".xlsx";

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
        private static void SendEmail(ref string xlsFilePath, DateTime currentDate, string vendor)
        {
            //string strMsgBody = string.Empty;
            try
            {

                string strToEmail = ConfigurationManager.AppSettings["mailRecipientTO_" + vendor].ToString();
                string strBccEmail = ConfigurationManager.AppSettings["mailRecipientBCC"].ToString();
                //StringBuilder sb = new StringBuilder();

                //sb.AppendLine("");
                //strMsgBody = sb.ToString();

                SmtpMail mail = new SmtpMail("TMPWEB1", false);

                mail.AddAttachment(xlsFilePath);//Attach XLS report
                mail.AddRecipient(strToEmail, RecipientType.To);
                mail.AddRecipient(strBccEmail, RecipientType.Bcc);

                mail.From = "reports1@calibrus.com";

                mail.Subject = "Constellation " + (vendor == "Total" ? "All Vendors" : "Vendor: " + vendor) + " No Sale Report for " + currentDate.ToString("dddd, dd MMMM yyyy") + ".";


                //mail.Body = strMsgBody;
                mail.SendMessage();

            }
            catch (Exception ex)
            {
                SendErrorMessage(ex);
            }


        }
        private static void GetDates(out DateTime StartDate, out DateTime EndDate, out DateTime YTDStartDate)
        {

            DateTime baseDate;
            DateTimeService.ReportingDateTimeService dts = null;
            try
            {
                dts = new DateTimeService.ReportingDateTimeService();
                baseDate = DateTime.Parse(dts.GetDateTime());

                //baseDate = new DateTime(2014, 8, 20); //ad hoc

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

            StartDate = new DateTime(baseDate.Year, baseDate.Month, baseDate.Day, 0, 0, 0).AddMonths(-1);//Previous Month date time
            EndDate = new DateTime(baseDate.Year, baseDate.Month, baseDate.Day, 0, 0, 0);//current date time should be first of the current month
            YTDStartDate = new DateTime(baseDate.Year, 1, 1, 0, 0, 0); //Start of the current Year

        }   
        #endregion

        #region ErrorHandling
        private static void SendErrorMessage(Exception ex)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendFormat("ex:{0}, innerEx:{1}", ex.Message, ex.InnerException == null ? "" : ex.InnerException.Message);

            Calibrus.ErrorHandler.Alerting alert = new Calibrus.ErrorHandler.Alerting("ConstellationNoSaleXLSReport");
            alert.SendAlert(ex.Source, sb.ToString(), Environment.MachineName, Environment.UserName, Environment.Version.ToString());
        }
        #endregion ErrorHandling
    }
}
