using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using Calibrus.Mail;
using Calibrus.ErrorHandler;
using Calibrus.ExcelFunctions;
using Excel = Microsoft.Office.Interop.Excel;

namespace ConstellationDTDCneResDesDailyXLS
{
    public class Report
    {
        public static object na = System.Reflection.Missing.Value;

        #region Main
        public static void Main(string[] args)
        {
            string rootPath = string.Empty;

            string mailRecipientProtocallDTDTO = string.Empty;
            string mailRecipientWattsMarketingSolutionsTO = string.Empty;
            string mailRecipientUESTO = string.Empty;
            string mailRecipientNational1TO = string.Empty;
            string mailTo = string.Empty; //holder for the above in a loop
            string mailRecipientAllVendorsTO = string.Empty;
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
            mailRecipientProtocallDTDTO = ConfigurationManager.AppSettings["mailRecipientProtocallDTDTO"].ToString();
            mailRecipientWattsMarketingSolutionsTO = ConfigurationManager.AppSettings["mailRecipientWattsMarketingSolutionsTO"].ToString();
            mailRecipientUESTO = ConfigurationManager.AppSettings["mailRecipientUESTO"].ToString();
            mailRecipientNational1TO = ConfigurationManager.AppSettings["mailRecipientNational1TO"].ToString();
            mailRecipientAllVendorsTO = ConfigurationManager.AppSettings["mailRecipientAllVendorsTO"].ToString();
            mailRecipientBCC = ConfigurationManager.AppSettings["mailRecipientBCC"].ToString();

            //start Excel
            Excel.Application exApp = null;
            Excel.Workbook exBook = null;
            Excel.Worksheet exSheet = null;
            Excel.Range exRange = null;

            int sheetsAdded = 0;

            #region Individual Vendor Report

            //Get List of Vendors to send to
            List<Vendor> VendorList = GetVendors();

            //Build Distro List from config file
            foreach (Vendor Vendor in VendorList)
            {
                switch (Vendor.VendorId)
                {
                    case 4://Protocall-DTD
                        mailTo = mailRecipientProtocallDTDTO;
                        break;
                    case 9://Watts Marketing Solutions
                        mailTo = mailRecipientWattsMarketingSolutionsTO;
                        break;
                    case 12://UES
                        mailTo = mailRecipientUESTO;
                        break;
                    case 17: //National1
                        mailTo = mailRecipientNational1TO;
                        break;
                }

                //start Excel
                exApp = new Excel.Application();
                exBook = null;
                exSheet = null;
                exRange = null;

                sheetsAdded = 0;
                try
                {
                    exBook = exApp.Workbooks.Add(na);
                    exApp.Visible = false;

                    //Set global attributes
                    exApp.StandardFont = "Calibri";
                    exApp.StandardFontSize = 11;

                    #region Pos ID Dashboard Tab
                    if (sheetsAdded < exBook.Sheets.Count)
                    {
                        exSheet = (Excel.Worksheet)exBook.Sheets[sheetsAdded + 1];
                    }
                    else
                    {
                        exSheet = (Excel.Worksheet)exBook.Sheets.Add(na, exBook.ActiveSheet, na, na);
                    }

                    exSheet.Name = "Pos ID Dashboard";
                    exSheet.Select(na);

                    sheetsAdded++;

                    //WritePOSIDDashboardReport()
                    WritePOSIDDashboardReport(ref exApp, ref exRange, StartDate, EndDate, Vendor.VendorName);

                    //Autosize the columns
                    exRange = (Excel.Range)exApp.get_Range("A1", "V1");
                    exRange.EntireColumn.AutoFit();

                    #endregion Pos ID Dashboard Tab

                    #region Data - Sales By Agent Tab

                    if (sheetsAdded < exBook.Sheets.Count)
                    {
                        exSheet = (Excel.Worksheet)exBook.Sheets[sheetsAdded + 1];
                    }
                    else
                    {
                        exSheet = (Excel.Worksheet)exBook.Sheets.Add(na, exBook.ActiveSheet, na, na);
                    }

                    exSheet.Name = "Data - Sales By Agent";
                    exSheet.Select(na);

                    sheetsAdded++;

                    //WriteSalesByAgentReport()
                    WriteSalesByAgentReport(ref exApp, ref exRange, StartDate, EndDate, Vendor.VendorName);

                    //Autosize the columns
                    exRange = (Excel.Range)exApp.get_Range("A1", "H1");
                    exRange.EntireColumn.AutoFit();

                    #endregion Data - Sales By Agent Tab

                    #region Data Alerts By Agent Tab
                    if (sheetsAdded < exBook.Sheets.Count)
                    {
                        exSheet = (Excel.Worksheet)exBook.Sheets[sheetsAdded + 1];
                    }
                    else
                    {
                        exSheet = (Excel.Worksheet)exBook.Sheets.Add(na, exBook.ActiveSheet, na, na);
                    }

                    exSheet.Name = "Data - Alerts By Agent";
                    exSheet.Select(na);

                    sheetsAdded++;

                    //WriteAlertsByAgentReport() 
                    WriteAlertsByAgentReport(ref exApp, ref exRange, StartDate, EndDate, Vendor.VendorNumber);

                    //Autosize the columns
                    exRange = (Excel.Range)exApp.get_Range("A1", "J1");
                    exRange.EntireColumn.AutoFit();
                    #endregion Data Alerts By Agent Tab

                    #region Sales By Fuel Type Tab

                    if (sheetsAdded < exBook.Sheets.Count)
                    {
                        exSheet = (Excel.Worksheet)exBook.Sheets[sheetsAdded + 1];
                    }
                    else
                    {
                        exSheet = (Excel.Worksheet)exBook.Sheets.Add(na, exBook.ActiveSheet, na, na);
                    }

                    exSheet.Name = "Data - Sales By Fuel Type";
                    exSheet.Select(na);

                    sheetsAdded++;

                    //WriteSalesByFuelTypeReport() 
                    WriteSalesByFuelTypeReport(ref exApp, ref exRange, StartDate, EndDate, Vendor.VendorNumber);

                    //Autosize the columns
                    exRange = (Excel.Range)exApp.get_Range("A1", "H1");
                    exRange.EntireColumn.AutoFit();

                    #endregion Sales By Fuel Type Tab

                    #region Data No Sales By Agent Tab

                    if (sheetsAdded < exBook.Sheets.Count)
                    {
                        exSheet = (Excel.Worksheet)exBook.Sheets[sheetsAdded + 1];
                    }
                    else
                    {
                        exSheet = (Excel.Worksheet)exBook.Sheets.Add(na, exBook.ActiveSheet, na, na);
                    }

                    exSheet.Name = "Data - No Sales By Agent";
                    exSheet.Select(na);

                    sheetsAdded++;

                    //WriteNoSalesByAgentReport() 
                    WriteNoSalesByAgentReport(ref exApp, ref exRange, StartDate, EndDate, Vendor.VendorNumber);

                    //Autosize the columns
                    exRange = (Excel.Range)exApp.get_Range("A1", "Y1");
                    exRange.EntireColumn.AutoFit();

                    #endregion Data No Sales By Agent Tab

                    //select the first tab in the workbook
                    exSheet = (Excel.Worksheet)exApp.Worksheets[1];
                    exSheet.Select(na);

                    //Save the xls Report to represent the day of the reports data
                    SaveXlsDocument(ref rootPath, ref xlsFilename, ref xlsFilePath, exBook, StartDate, Vendor.VendorName);
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
                SendEmail(ref xlsFilePath, StartDate, mailTo, mailRecipientBCC, Vendor.VendorName);
            }
            #endregion Individual Vendor Report

            #region All Vendors Report
            //start Excel
            exApp = new Excel.Application();
            exBook = null;
            exSheet = null;
            exRange = null;

            sheetsAdded = 0;
            try
            {
                exBook = exApp.Workbooks.Add(na);
                exApp.Visible = false;

                //Set global attributes
                exApp.StandardFont = "Calibri";
                exApp.StandardFontSize = 11;

                #region Pos ID Dashboard Tab

                if (sheetsAdded < exBook.Sheets.Count)
                {
                    exSheet = (Excel.Worksheet)exBook.Sheets[sheetsAdded + 1];
                }
                else
                {
                    exSheet = (Excel.Worksheet)exBook.Sheets.Add(na, exBook.ActiveSheet, na, na);
                }

                exSheet.Name = "Pos ID Dashboard";
                exSheet.Select(na);

                sheetsAdded++;

                //WritePOSIDDashboardReport()//no VendorNumber since this is a total vendor report
                WritePOSIDDashboardReport(ref exApp, ref exRange, StartDate, EndDate, null);

                //Autosize the columns
                exRange = (Excel.Range)exApp.get_Range("A1", "V1");
                exRange.EntireColumn.AutoFit();

                #endregion Pos ID Dashboard Tab

                #region Data - Sales By Agent Tab

                if (sheetsAdded < exBook.Sheets.Count)
                {
                    exSheet = (Excel.Worksheet)exBook.Sheets[sheetsAdded + 1];
                }
                else
                {
                    exSheet = (Excel.Worksheet)exBook.Sheets.Add(na, exBook.ActiveSheet, na, na);
                }

                exSheet.Name = "Data - Sales By Agent";
                exSheet.Select(na);

                sheetsAdded++;

                //WriteSalesByAgentReport() //no VendorNumber since this is a total vendor report
                WriteSalesByAgentReport(ref exApp, ref exRange, StartDate, EndDate, null);

                //Autosize the columns
                exRange = (Excel.Range)exApp.get_Range("A1", "H1");
                exRange.EntireColumn.AutoFit();

                #endregion Data - Sales By Agent Tab

                #region Data Alerts By Agent Tab
                if (sheetsAdded < exBook.Sheets.Count)
                {
                    exSheet = (Excel.Worksheet)exBook.Sheets[sheetsAdded + 1];
                }
                else
                {
                    exSheet = (Excel.Worksheet)exBook.Sheets.Add(na, exBook.ActiveSheet, na, na);
                }

                exSheet.Name = "Data - Alerts By Agent";
                exSheet.Select(na);

                sheetsAdded++;

                //WriteAlertsByAgentReport() //no VendorNumber since this is a total vendor report
                WriteAlertsByAgentReport(ref exApp, ref exRange, StartDate, EndDate, null);

                //Autosize the columns
                exRange = (Excel.Range)exApp.get_Range("A1", "J1");
                exRange.EntireColumn.AutoFit();
                #endregion Data Alerts By Agent Tab

                #region Sales By Fuel Type Tab

                if (sheetsAdded < exBook.Sheets.Count)
                {
                    exSheet = (Excel.Worksheet)exBook.Sheets[sheetsAdded + 1];
                }
                else
                {
                    exSheet = (Excel.Worksheet)exBook.Sheets.Add(na, exBook.ActiveSheet, na, na);
                }

                exSheet.Name = "Data - Sales By Fuel Type";
                exSheet.Select(na);

                sheetsAdded++;

                //WriteSalesByFuelTypeReport() //no VendorNumber since this is a total vendor report
                WriteSalesByFuelTypeReport(ref exApp, ref exRange, StartDate, EndDate, null);

                //Autosize the columns
                exRange = (Excel.Range)exApp.get_Range("A1", "H1");
                exRange.EntireColumn.AutoFit();

                #endregion Sales By Fuel Type Tab

                #region Data No Sales By Agent Tab

                if (sheetsAdded < exBook.Sheets.Count)
                {
                    exSheet = (Excel.Worksheet)exBook.Sheets[sheetsAdded + 1];
                }
                else
                {
                    exSheet = (Excel.Worksheet)exBook.Sheets.Add(na, exBook.ActiveSheet, na, na);
                }

                exSheet.Name = "Data - No Sales By Agent";
                exSheet.Select(na);

                sheetsAdded++;

                //WriteNoSalesByAgentReport() //no VendorNumber since this is a total vendor report
                WriteNoSalesByAgentReport(ref exApp, ref exRange, StartDate, EndDate, null);

                //Autosize the columns
                exRange = (Excel.Range)exApp.get_Range("A1", "Y1");
                exRange.EntireColumn.AutoFit();

                #endregion Data No Sales By Agent Tab

                //select the first tab in the workbook
                exSheet = (Excel.Worksheet)exApp.Worksheets[1];
                exSheet.Select(na);

                //Save the xls Report to represent the day of the reports data
                SaveXlsDocument(ref rootPath, ref xlsFilename, ref xlsFilePath, exBook, StartDate, null);
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
            SendEmail(ref xlsFilePath, StartDate, mailRecipientAllVendorsTO, mailRecipientBCC, null);
            #endregion All Vendors Report
        }
        #endregion Main

        #region Excel (5 methods)

        #region WritePOSIDDashboardReport
        private static void WritePOSIDDashboardReport(ref Excel.Application exApp, ref Excel.Range exRange, DateTime startDate, DateTime endDate, string vendorNumber)
        {
            int rowInitialize = 1; //initial seed for the row data
            int row = 0;// where we start the row data


            int dataColumnInitialize = 65; //initial seed for column data - column  A
            int col = 0;

            row = rowInitialize;  //set the row for the data
            col = dataColumnInitialize;//set the column for the data

            #region Header

            col++;

            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col + 8), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                18, true, false, false);
            exRange.Merge(na);
            exRange.Value2 = "Infutor";
            col++;

            col++;

            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col + 8), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                18, true, false, false);
            exRange.Merge(na);
            exRange.Value2 = "Experian";
            col++;
            row++;

            col = dataColumnInitialize; // reset back to Coloumn A

            col++;

            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                18, false, false, false);
            exRange.Value2 = "Total Checks";
            col++;

            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                18, false, false, false);
            exRange.Value2 = "Green";
            col++;

            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                18, false, false, false);
            exRange.Value2 = "% Green";
            col++;

            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                18, false, false, false);
            exRange.Value2 = "Blue";
            col++;

            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                18, false, false, false);
            exRange.Value2 = "% Blue";
            col++;

            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                18, false, false, false);
            exRange.Value2 = "Red";
            col++;

            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                18, false, false, false);
            exRange.Value2 = "% Red";
            col++;

            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                18, false, false, false);
            exRange.Value2 = "Blank";
            col++;

            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                18, false, false, false);
            exRange.Value2 = "% Blank";
            col++;

            col++;

            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                18, false, false, false);
            exRange.Value2 = "Total Checks";
            col++;

            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                18, false, false, false);
            exRange.Merge(na);
            exRange.Value2 = "Green";
            col++;

            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                18, false, false, false);
            exRange.Value2 = "% Green";
            col++;

            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                18, false, false, false);
            exRange.Value2 = "Blue";
            col++;

            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                18, false, false, false);
            exRange.Value2 = "% Blue";
            col++;

            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                18, false, false, false);
            exRange.Value2 = "Red";
            col++;

            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                18, false, false, false);
            exRange.Value2 = "% Red";
            col++;

            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                18, false, false, false);
            exRange.Value2 = "Blank";
            col++;

            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                18, false, false, false);
            exRange.Value2 = "% Blank";
            col++;

            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                18, false, false, false);
            exRange.Value2 = "Improved Over Infutor";
            col++;

            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                18, false, false, false);
            exRange.Value2 = "% Improved Over Infutor";
            col++;
            row++;

            row++;
            col = dataColumnInitialize; // reset back to Coloumn A
            #endregion Header

            #region Data

            #region Current Day Values
            spDTDPOSIdDashboardByDay_Result POSIDDashboardByDayResult = null;
            POSIDDashboardByDayResult = GetPOSIDDashboardByDay(startDate, endDate, vendorNumber);
            if (POSIDDashboardByDayResult != null)
            {

                //Date of values
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight,
                    18, false, false, false);
                exRange.Value2 = string.Format("{0:MM/dd/yyyy}", startDate);
                col++;

                //Total Checks
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight,
                    18, false, false, false);
                exRange.Value2 = POSIDDashboardByDayResult.InfutorTotalChecks;
                col++;

                //Green
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight,
                    18, false, false, false);
                exRange.Value2 = POSIDDashboardByDayResult.InfutorGreen;
                col++;

                //% Green
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight,
                    18, false, false, false);
                exRange.NumberFormat = "0.00%";
                string.Format("=IFERROR({0}/{1}, \"0.00%\" )", POSIDDashboardByDayResult.InfutorGreen, POSIDDashboardByDayResult.InfutorTotalChecks);
                col++;

                //Blue
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight,
                    18, false, false, false);
                exRange.Value2 = POSIDDashboardByDayResult.InfutorBlue;
                col++;

                //% Blue
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight,
                    18, false, false, false);
                exRange.NumberFormat = "0.00%";
                string.Format("=IFERROR({0}/{1}, \"0.00%\" )", POSIDDashboardByDayResult.InfutorBlue, POSIDDashboardByDayResult.InfutorTotalChecks);
                col++;

                //Red
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight,
                    18, false, false, false);
                exRange.Value2 = POSIDDashboardByDayResult.InfutorRed;
                col++;

                //"% Red
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight,
                    18, false, false, false);
                exRange.NumberFormat = "0.00%";
                string.Format("=IFERROR({0}/{1}, \"0.00%\" )", POSIDDashboardByDayResult.InfutorRed, POSIDDashboardByDayResult.InfutorTotalChecks);
                col++;

                //Blank
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight,
                    18, false, false, false);
                exRange.Value2 = POSIDDashboardByDayResult.InfutorBlank;
                col++;

                //% Blank
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight,
                    18, false, false, false);
                exRange.NumberFormat = "0.00%";
                string.Format("=IFERROR({0}/{1}, \"0.00%\" )", POSIDDashboardByDayResult.InfutorBlank, POSIDDashboardByDayResult.InfutorTotalChecks);
                col++;

                col++;

                //Total Checks
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight,
                    18, false, false, false);
                exRange.Value2 = POSIDDashboardByDayResult.ExperianTotalChecks;
                col++;

                //Green
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight,
                    18, false, false, false);
                exRange.Value2 = POSIDDashboardByDayResult.ExperianGreen;
                col++;

                //% Green            
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight,
                    18, false, false, false);
                exRange.NumberFormat = "0.00%";
                string.Format("=IFERROR({0}/{1}, \"0.00%\" )", POSIDDashboardByDayResult.ExperianGreen, POSIDDashboardByDayResult.ExperianTotalChecks);
                col++;

                //Blue
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight,
                    18, false, false, false);
                exRange.Value2 = POSIDDashboardByDayResult.ExperianBlue;
                col++;

                //% Blue
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight,
                    18, false, false, false);
                exRange.NumberFormat = "0.00%";
                string.Format("=IFERROR({0}/{1}, \"0.00%\" )", POSIDDashboardByDayResult.ExperianBlue, POSIDDashboardByDayResult.ExperianTotalChecks);
                col++;

                //Red
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight,
                    18, false, false, false);
                exRange.Value2 = POSIDDashboardByDayResult.ExperianRed;
                col++;

                //% Red
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    18, false, false, false);
                exRange.NumberFormat = "0.00%";
                string.Format("=IFERROR({0}/{1}, \"0.00%\" )", POSIDDashboardByDayResult.ExperianRed, POSIDDashboardByDayResult.ExperianTotalChecks);
                col++;

                //Blank
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight,
                    18, false, false, false);
                exRange.Value2 = POSIDDashboardByDayResult.ExperianBlank;
                col++;

                //% Blank
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    18, false, false, false);
                exRange.NumberFormat = "0.00%";
                string.Format("=IFERROR({0}/{1}, \"0.00%\" )", POSIDDashboardByDayResult.ExperianBlank, POSIDDashboardByDayResult.ExperianTotalChecks);
                col++;

                //Improved Over Infutor
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight,
                    18, false, false, false);
                exRange.Value2 = POSIDDashboardByDayResult.ExperianGreen + POSIDDashboardByDayResult.ExperianBlue;
                col++;

                //% Improved Over Infutor
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight,
                    18, false, false, false);
                exRange.NumberFormat = "0.00%";
                string.Format("=IFERROR({0}+{1}/{2}, \"0.00%\" )", POSIDDashboardByDayResult.ExperianGreen, POSIDDashboardByDayResult.ExperianBlue, POSIDDashboardByDayResult.ExperianTotalChecks);
                col++;

                row++;

                row++;
                col = dataColumnInitialize; // reset back to Coloumn A
            }
            #endregion Current Day Values

            #region Entire Year Values

            List<spDTDPOSIdDashboard_Result> POSIDDashboardResult = GetPOSIDDashboard(startDate, vendorNumber);
            foreach (var item in POSIDDashboardResult)
            {
                //Date of values
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight,
                    18, false, false, false);
                exRange.Value2 = item.MonthName;
                col++;

                //Total Checks
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight,
                    18, false, false, false);
                exRange.Value2 = item.InfutorTotalChecks;
                col++;

                //Green
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight,
                    18, false, false, false);
                exRange.Value2 = item.InfutorGreen;
                col++;

                //% Green
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight,
                    18, false, false, false);
                exRange.NumberFormat = "0.00%";
                string.Format("=IFERROR({0}/{1}, \"0.00%\" )", item.InfutorGreen, item.InfutorTotalChecks);
                col++;

                //Blue
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight,
                    18, false, false, false);
                exRange.Value2 = item.InfutorBlue;
                col++;

                //% Blue
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight,
                    18, false, false, false);
                exRange.NumberFormat = "0.00%";
                string.Format("=IFERROR({0}/{1}, \"0.00%\" )", item.InfutorBlue, item.InfutorTotalChecks);
                col++;

                //Red
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight,
                    18, false, false, false);
                exRange.Value2 = item.InfutorRed;
                col++;

                //"% Red
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight,
                    18, false, false, false);
                exRange.NumberFormat = "0.00%";
                string.Format("=IFERROR({0}/{1}, \"0.00%\" )", item.InfutorRed, item.InfutorTotalChecks);
                col++;

                //Blank
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight,
                    18, false, false, false);
                exRange.Value2 = item.InfutorBlank;
                col++;

                //% Blank
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight,
                    18, false, false, false);
                exRange.NumberFormat = "0.00%";
                string.Format("=IFERROR({0}/{1}, \"0.00%\" )", item.InfutorBlank, item.InfutorTotalChecks);
                col++;

                col++;

                //Total Checks
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight,
                    18, false, false, false);
                exRange.Value2 = item.ExperianTotalChecks;
                col++;

                //Green
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight,
                    18, false, false, false);
                exRange.Value2 = item.ExperianGreen;
                col++;

                //% Green            
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight,
                    18, false, false, false);
                exRange.NumberFormat = "0.00%";
                string.Format("=IFERROR({0}/{1}, \"0.00%\" )", item.ExperianGreen, item.ExperianTotalChecks);
                col++;

                //Blue
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight,
                    18, false, false, false);
                exRange.Value2 = item.ExperianBlue;
                col++;

                //% Blue
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight,
                    18, false, false, false);
                exRange.NumberFormat = "0.00%";
                string.Format("=IFERROR({0}/{1}, \"0.00%\" )", item.ExperianBlue, item.ExperianTotalChecks);
                col++;

                //Red
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight,
                    18, false, false, false);
                exRange.Value2 = item.ExperianRed;
                col++;

                //% Red
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    18, false, false, false);
                exRange.NumberFormat = "0.00%";
                string.Format("=IFERROR({0}/{1}, \"0.00%\" )", item.ExperianRed, item.ExperianTotalChecks);
                col++;

                //Blank
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight,
                    18, false, false, false);
                exRange.Value2 = item.ExperianBlank;
                col++;

                //% Blank
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    18, false, false, false);
                exRange.NumberFormat = "0.00%";
                string.Format("=IFERROR({0}/{1}, \"0.00%\" )", item.ExperianBlank, item.ExperianTotalChecks);
                col++;

                //Improved Over Infutor
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight,
                    18, false, false, false);
                exRange.Value2 = item.ExperianGreen + item.ExperianBlue;
                col++;

                //% Improved Over Infutor
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight,
                    18, false, false, false);
                exRange.NumberFormat = "0.00%";
                string.Format("=IFERROR({0}+{1}/{2}, \"0.00%\" )", item.ExperianGreen, item.ExperianBlue, item.ExperianTotalChecks);
                col++;

                row++;
            }


            row++;
            col = dataColumnInitialize; // reset back to Coloumn A
            #endregion Entire Year Values

            #region YTD Values

            //Date of values
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight,
                18, false, false, false);
            exRange.Value2 = "YTD";
            col++;

            //Total Checks
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight,
                18, false, false, false);
            exRange.Value2 = POSIDDashboardResult.Select(x => x.InfutorTotalChecks).Sum();
            col++;

            //Green
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight,
                18, false, false, false);
            exRange.Value2 = POSIDDashboardResult.Select(x => x.InfutorGreen).Sum();
            col++;

            //% Green
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight,
                18, false, false, false);
            exRange.NumberFormat = "0.00%";
            string.Format("=IFERROR({0}/{1}, \"0.00%\" )", POSIDDashboardResult.Select(x => x.InfutorGreen).Sum(), POSIDDashboardResult.Select(x => x.InfutorTotalChecks).Sum());
            col++;

            //Blue
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight,
                18, false, false, false);
            exRange.Value2 = POSIDDashboardResult.Select(x => x.InfutorBlue).Sum();
            col++;

            //% Blue
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight,
                18, false, false, false);
            exRange.NumberFormat = "0.00%";
            string.Format("=IFERROR({0}/{1}, \"0.00%\" )", POSIDDashboardResult.Select(x => x.InfutorBlue).Sum(), POSIDDashboardResult.Select(x => x.InfutorTotalChecks).Sum());
            col++;

            //Red
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight,
                18, false, false, false);
            exRange.Value2 = POSIDDashboardResult.Select(x => x.InfutorRed).Sum();
            col++;

            //"% Red
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight,
                18, false, false, false);
            exRange.NumberFormat = "0.00%";
            string.Format("=IFERROR({0}/{1}, \"0.00%\" )", POSIDDashboardResult.Select(x => x.InfutorRed).Sum(), POSIDDashboardResult.Select(x => x.InfutorTotalChecks).Sum());
            col++;

            //Blank
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight,
                18, false, false, false);
            exRange.Value2 = POSIDDashboardResult.Select(x => x.InfutorBlank).Sum();
            col++;

            //% Blank
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight,
                18, false, false, false);
            exRange.NumberFormat = "0.00%";
            string.Format("=IFERROR({0}/{1}, \"0.00%\" )", POSIDDashboardResult.Select(x => x.InfutorBlank).Sum(), POSIDDashboardResult.Select(x => x.InfutorTotalChecks).Sum());
            col++;

            col++;

            //Total Checks
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight,
                18, false, false, false);
            exRange.Value2 = POSIDDashboardResult.Select(x => x.ExperianBlank).Sum();
            col++;

            //Green
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight,
                18, false, false, false);
            exRange.Value2 = POSIDDashboardResult.Select(x => x.ExperianGreen).Sum();
            col++;

            //% Green            
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight,
                18, false, false, false);
            exRange.NumberFormat = "0.00%";
            string.Format("=IFERROR({0}/{1}, \"0.00%\" )", POSIDDashboardResult.Select(x => x.ExperianGreen).Sum(), POSIDDashboardResult.Select(x => x.ExperianTotalChecks).Sum());
            col++;

            //Blue
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight,
                18, false, false, false);
            exRange.Value2 = POSIDDashboardResult.Select(x => x.ExperianBlue).Sum();
            col++;

            //% Blue
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight,
                18, false, false, false);
            exRange.NumberFormat = "0.00%";
            string.Format("=IFERROR({0}/{1}, \"0.00%\" )", POSIDDashboardResult.Select(x => x.ExperianBlue).Sum(), POSIDDashboardResult.Select(x => x.ExperianTotalChecks).Sum());
            col++;

            //Red
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight,
                18, false, false, false);
            exRange.Value2 = POSIDDashboardResult.Select(x => x.ExperianRed).Sum();
            col++;

            //% Red
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                18, false, false, false);
            exRange.NumberFormat = "0.00%";
            string.Format("=IFERROR({0}/{1}, \"0.00%\" )", POSIDDashboardResult.Select(x => x.ExperianRed).Sum(), POSIDDashboardResult.Select(x => x.ExperianTotalChecks).Sum());
            col++;

            //Blank
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight,
                18, false, false, false);
            exRange.Value2 = POSIDDashboardResult.Select(x => x.ExperianBlank).Sum();
            col++;

            //% Blank
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                18, false, false, false);
            exRange.NumberFormat = "0.00%";
            string.Format("=IFERROR({0}/{1}, \"0.00%\" )", POSIDDashboardResult.Select(x => x.ExperianBlank).Sum(), POSIDDashboardResult.Select(x => x.ExperianTotalChecks).Sum());
            col++;

            //Improved Over Infutor
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight,
                18, false, false, false);
            exRange.Value2 = POSIDDashboardResult.Select(x => x.ExperianGreen).Sum() + POSIDDashboardResult.Select(x => x.ExperianBlue).Sum();
            col++;

            //% Improved Over Infutor
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight,
                18, false, false, false);
            exRange.NumberFormat = "0.00%";
            string.Format("=IFERROR({0}+{1}/{2}, \"0.00%\" )", POSIDDashboardResult.Select(x => x.ExperianGreen).Sum(), POSIDDashboardResult.Select(x => x.ExperianBlue).Sum(), POSIDDashboardResult.Select(x => x.ExperianTotalChecks).Sum());
            col++;

            row++;

            row++;
            col = dataColumnInitialize; // reset back to Coloumn A
            #endregion YTD Values

            #endregion Data
        }
        #endregion WritePOSIDDashboardReport

        #region WriteSalesByAgentReport

        private static void WriteSalesByAgentReport(ref Excel.Application exApp, ref Excel.Range exRange, DateTime startDate, DateTime endDate, string vendorNumber)
        {
            int rowInitialize = 1; //initial seed for the row data
            int row = 0;// where we start the row data


            int dataColumnInitialize = 65; //initial seed for column data - column  A
            int col = 0;

            row = rowInitialize;  //set the row for the data
            col = dataColumnInitialize;//set the column for the data

            #region Header

            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col + 7), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                24, true, true, false);
            exRange.Merge(na);
            exRange.Value2 = "Sales By Agent";
            row++;


            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                11, true, false, false);
            exRange.Value2 = "SALES_STATE";
            col++;

            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                11, true, false, false);
            exRange.Value2 = "CENTER_ID";
            col++;

            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                11, true, false, false);
            exRange.Value2 = "VENDOR_NAME";
            col++;

            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                11, true, false, false);
            exRange.Value2 = "TSR_ID";
            col++;

            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                11, true, false, false);
            exRange.Value2 = "TSR_NAME";
            col++;

            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                11, true, false, false);
            exRange.Value2 = "GOOD_SALES";
            col++;

            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                11, true, false, false);
            exRange.Value2 = "NO_SALES";
            col++;

            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                11, true, false, false);
            exRange.Value2 = "TOTAL_ATTEMPTS";
            col++;

            row++;
            col = dataColumnInitialize; // reset back to Coloumn A

            #endregion Header

            #region Data

            #region Loop for Aggregate State totals

            List<spDTDSalesByAgentAllStateAggregate_Result> salesByAgentAllStatesAggregate = GetSalesByAgentAllStatesAggregate(startDate, endDate, null);
            if (salesByAgentAllStatesAggregate.Count > 0)
            {
                foreach (var item in salesByAgentAllStatesAggregate)
                {
                    //SALES_STATE
                    exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                        11, false, false, false);
                    exRange.Value2 = "ALL";
                    col++;

                    //CENTER_ID
                    exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight,
                        11, false, false, false);
                    exRange.Value2 = item.CenterId;
                    col++;

                    //VENDOR_NAME
                    exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight,
                        11, false, false, false);
                    exRange.Value2 = item.VendorName;
                    col++;

                    //TSR_ID
                    exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                        11, false, false, false);
                    exRange.Value2 = item.TSRId;
                    col++;

                    //TSR_NAME
                    exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight,
                        11, false, false, false);
                    exRange.Value2 = item.TSRName;
                    col++;

                    //GOOD_SALES
                    exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight,
                        11, false, false, false);
                    exRange.Value2 = item.GoodSales;
                    col++;

                    //NO_SALES
                    exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight,
                        11, false, false, false);
                    exRange.Value2 = item.NoSales;
                    col++;

                    //TOTAL_ATTEMPTS
                    exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                        11, false, false, false);
                    exRange.Value2 = item.TotalAttempts;
                    col++;

                    row++;
                    col = dataColumnInitialize; // reset back to Coloumn A
                }
            }

            #endregion Loop for Aggregate State totals

            #region Loop for State break out

            List<spDTDSalesByAgent_Result> salesByAgentByState = GetSalesByAgentByState(startDate, endDate, null);
            if (salesByAgentByState.Count > 0)
            {
                foreach (var item in salesByAgentByState)
                {
                    //SALES_STATE
                    exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                        11, false, false, false);
                    exRange.Value2 = item.SalesState;
                    col++;

                    //CENTER_ID
                    exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight,
                        11, false, false, false);
                    exRange.Value2 = item.CenterId;
                    col++;

                    //VENDOR_NAME
                    exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight,
                        11, false, false, false);
                    exRange.Value2 = item.VendorName;
                    col++;

                    //TSR_ID
                    exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                        11, false, false, false);
                    exRange.Value2 = item.TSRId;
                    col++;

                    //TSR_NAME
                    exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight,
                        11, false, false, false);
                    exRange.Value2 = item.TSRName;
                    col++;

                    //GOOD_SALES
                    exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight,
                        11, false, false, false);
                    exRange.Value2 = item.GoodSales;
                    col++;

                    //NO_SALES
                    exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight,
                        11, false, false, false);
                    exRange.Value2 = item.NoSales;
                    col++;

                    //TOTAL_ATTEMPTS
                    exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                        11, false, false, false);
                    exRange.Value2 = item.TotalAttempts;
                    col++;

                    row++;
                    col = dataColumnInitialize; // reset back to Coloumn A
                }
            }

            #endregion Loop for State break out

            #endregion Data
        }

        #endregion WriteSalesByAgentReport

        #region WriteAlertsByAgentReport

        private static void WriteAlertsByAgentReport(ref Excel.Application exApp, ref Excel.Range exRange, DateTime startDate, DateTime endDate, string vendorNumber)
        {
            int rowInitialize = 1; //initial seed for the row data
            int row = 0;// where we start the row data


            int dataColumnInitialize = 65; //initial seed for column data - column  A
            int col = 0;

            row = rowInitialize;  //set the row for the data
            col = dataColumnInitialize;//set the column for the data

            #region Header

            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col + 7), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                24, true, true, false);
            exRange.Merge(na);
            exRange.Value2 = "Alerts By Agent";
            row++;


            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                11, true, false, false);
            exRange.Value2 = "SALES_STATE";
            col++;

            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                11, true, false, false);
            exRange.Value2 = "CENTER_ID";
            col++;

            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                11, true, false, false);
            exRange.Value2 = "VENDOR_NAME";
            col++;

            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                11, true, false, false);
            exRange.Value2 = "TSR_ID";
            col++;

            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                11, true, false, false);
            exRange.Value2 = "TSR_NAME";
            col++;

            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                11, true, false, false);
            exRange.Value2 = "6_OR_MORE_SALES";
            col++;

            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                11, true, false, false);
            exRange.Value2 = "BTN_USED_PREV";
            col++;

            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                11, true, false, false);
            exRange.Value2 = "CB_NUMBER_USED";
            col++;

            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                11, true, false, false);
            exRange.Value2 = "BTN_SAME_AS_AGENT";
            col++;

            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                11, true, false, false);
            exRange.Value2 = "APT_GREATER_THAN_2";
            col++;

            row++;
            col = dataColumnInitialize; // reset back to Coloumn A

            #endregion Header

            #region Data

            #region Loop for Aggregate State totals

            List<spDTDAlertsByAgentAllStateAggregate_Result> alertsByAgentAllStatesAggregate = GetAlertsByAgentAllStatesAggregate(startDate, endDate, vendorNumber);
            if (alertsByAgentAllStatesAggregate.Count > 0)
            {
                foreach (var item in alertsByAgentAllStatesAggregate)
                {
                    //SALES_STATE
                    exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                        11, false, false, false);
                    exRange.Value2 = "ALL";
                    col++;

                    //CENTER_ID
                    exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight,
                        11, false, false, false);
                    exRange.Value2 = item.CenterId;
                    col++;

                    //VENDOR_NAME
                    exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight,
                        11, false, false, false);
                    exRange.Value2 = item.VendorName;
                    col++;

                    //TSR_ID
                    exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                        11, false, false, false);
                    exRange.Value2 = item.TSRId;
                    col++;

                    //TSR_NAME
                    exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight,
                        11, false, false, false);
                    exRange.Value2 = item.TSRName;
                    col++;

                    //6_OR_MORE_SALES
                    exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight,
                        11, false, false, false);
                    exRange.Value2 = item.SixOrMoreSales;
                    col++;

                    //BTN_USED_PREV
                    exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight,
                        11, false, false, false);
                    exRange.Value2 = item.BTNUsedPrev;
                    col++;

                    //CB_NUMBER_USED
                    exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                        11, false, false, false);
                    exRange.Value2 = item.CallBackNumUsedPrev;
                    col++;

                    //BTN_SAME_AS_AGENT
                    exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                        11, false, false, false);
                    exRange.Value2 = item.BTNSameAsAgent;
                    col++;

                    //APT_GREATER_THAN_2
                    exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                        11, false, false, false);
                    exRange.Value2 = item.AptGreaterThanTwo;
                    col++;

                    row++;
                    col = dataColumnInitialize; // reset back to Coloumn A
                }
            }

            #endregion Loop for Aggregate State totals

            #region Loop for State break out

            List<spDTDAlertsByAgent_Result> alertsByAgent = GetAlertsByAgentByState(startDate, endDate, vendorNumber);
            if (alertsByAgent.Count > 0)
            {
                foreach (var item in alertsByAgent)
                {
                    //SALES_STATE
                    exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                        11, false, false, false);
                    exRange.Value2 = item.SalesState;
                    col++;

                    //CENTER_ID
                    exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight,
                        11, false, false, false);
                    exRange.Value2 = item.CenterId;
                    col++;

                    //VENDOR_NAME
                    exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight,
                        11, false, false, false);
                    exRange.Value2 = item.VendorName;
                    col++;

                    //TSR_ID
                    exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                        11, false, false, false);
                    exRange.Value2 = item.TSRId;
                    col++;

                    //TSR_NAME
                    exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight,
                        11, false, false, false);
                    exRange.Value2 = item.TSRName;
                    col++;

                    //6_OR_MORE_SALES
                    exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight,
                        11, false, false, false);
                    exRange.Value2 = item.SixOrMoreSales;
                    col++;

                    //BTN_USED_PREV
                    exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight,
                        11, false, false, false);
                    exRange.Value2 = item.BTNUsedPrev;
                    col++;

                    //CB_NUMBER_USED
                    exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                        11, false, false, false);
                    exRange.Value2 = item.CallBackNumUsedPrev;
                    col++;

                    //BTN_SAME_AS_AGENT
                    exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                        11, false, false, false);
                    exRange.Value2 = item.BTNSameAsAgent;
                    col++;

                    //APT_GREATER_THAN_2
                    exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                        11, false, false, false);
                    exRange.Value2 = item.AptGreaterThanTwo;
                    col++;

                    row++;
                    col = dataColumnInitialize; // reset back to Coloumn A
                }
            }

            #endregion Loop for State break out

            #endregion Data
        }

        #endregion WriteAlertsByAgentReport

        #region WriteSalesByFuelTypeReport

        private static void WriteSalesByFuelTypeReport(ref Excel.Application exApp, ref Excel.Range exRange, DateTime startDate, DateTime endDate, string vendorNumber)
        {
            int rowInitialize = 1; //initial seed for the row data
            int row = 0;// where we start the row data


            int dataColumnInitialize = 65; //initial seed for column data - column  A
            int col = 0;

            row = rowInitialize;  //set the row for the data
            col = dataColumnInitialize;//set the column for the data

            #region Header

            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col + 7), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                24, true, true, false);
            exRange.Merge(na);
            exRange.Value2 = "Sales By Fuel Type";
            row++;


            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                11, true, false, false);
            exRange.Value2 = "SALES_STATE";
            col++;

            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                11, true, false, false);
            exRange.Value2 = "CENTER_ID";
            col++;

            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                11, true, false, false);
            exRange.Value2 = "VENDOR_NAME";
            col++;

            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                11, true, false, false);
            exRange.Value2 = "TSR_ID";
            col++;

            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                11, true, false, false);
            exRange.Value2 = "TSR_NAME";
            col++;

            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                11, true, false, false);
            exRange.Value2 = "GAS";
            col++;

            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                11, true, false, false);
            exRange.Value2 = "ELECTRIC";
            col++;

            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                11, true, false, false);
            exRange.Value2 = "DUAL_FUEL";
            col++;

            row++;
            col = dataColumnInitialize; // reset back to Coloumn A

            #endregion Header

            #region Data

            #region Loop for Aggregate State totals

            List<spDTDSalesByFuelTypeAllStateAggregate_Result> salesByFuelTypeAllStatesAggregate = GetSalesByFuelTypeAllStatesAggregate(startDate, endDate, vendorNumber);
            if (salesByFuelTypeAllStatesAggregate.Count > 0)
            {
                foreach (var item in salesByFuelTypeAllStatesAggregate)
                {
                    //SALES_STATE
                    exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                        11, false, false, false);
                    exRange.Value2 = "ALL";
                    col++;

                    //CENTER_ID
                    exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight,
                        11, false, false, false);
                    exRange.Value2 = item.CenterId;
                    col++;

                    //VENDOR_NAME
                    exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight,
                        11, false, false, false);
                    exRange.Value2 = item.VendorName;
                    col++;

                    //TSR_ID
                    exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                        11, false, false, false);
                    exRange.Value2 = item.TSRId;
                    col++;

                    //TSR_NAME
                    exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight,
                        11, false, false, false);
                    exRange.Value2 = item.TSRName;
                    col++;

                    //GAS
                    exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight,
                        11, false, false, false);
                    exRange.Value2 = item.Gas;
                    col++;

                    //ELECTRIC
                    exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight,
                        11, false, false, false);
                    exRange.Value2 = item.Electric;
                    col++;

                    //DUAL FUEL
                    exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                        11, false, false, false);
                    exRange.Value2 = item.Dual;
                    col++;

                    row++;
                    col = dataColumnInitialize; // reset back to Coloumn A
                }
            }

            #endregion Loop for Aggregate State totals

            #region Loop for State break out

            List<spDTDSalesByFuelType_Result> salesByFuelTypeByState = GetSalesByFuelTypeByState(startDate, endDate, vendorNumber);
            if (salesByFuelTypeByState.Count > 0)
            {
                foreach (var item in salesByFuelTypeByState)
                {
                    //SALES_STATE
                    exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                        11, false, false, false);
                    exRange.Value2 = item.SalesState;
                    col++;

                    //CENTER_ID
                    exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight,
                        11, false, false, false);
                    exRange.Value2 = item.CenterId;
                    col++;

                    //VENDOR_NAME
                    exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight,
                        11, false, false, false);
                    exRange.Value2 = item.VendorName;
                    col++;

                    //TSR_ID
                    exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                        11, false, false, false);
                    exRange.Value2 = item.TSRId;
                    col++;

                    //TSR_NAME
                    exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight,
                        11, false, false, false);
                    exRange.Value2 = item.TSRName;
                    col++;

                    //GAS
                    exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight,
                        11, false, false, false);
                    exRange.Value2 = item.Gas;
                    col++;

                    //ELECTRIC
                    exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight,
                        11, false, false, false);
                    exRange.Value2 = item.Electric;
                    col++;

                    //DUAL FUEL
                    exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                        11, false, false, false);
                    exRange.Value2 = item.Dual;
                    col++;

                    row++;
                    col = dataColumnInitialize; // reset back to Coloumn A
                }
            }

            #endregion Loop for State break out

            #endregion Data
        }

        #endregion WriteSalesByFuelTypeReport

        #region WriteNoSalesByAgentReport

        private static void WriteNoSalesByAgentReport(ref Excel.Application exApp, ref Excel.Range exRange, DateTime startDate, DateTime endDate, string vendorNumber)
        {
            int rowInitialize = 1; //initial seed for the row data
            int row = 0;// where we start the row data


            int dataColumnInitialize = 65; //initial seed for column data - column  A
            int col = 0;

            row = rowInitialize;  //set the row for the data
            col = dataColumnInitialize;//set the column for the data

            #region Header

            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col + 7), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                24, true, true, false);
            exRange.Merge(na);
            exRange.Value2 = "No Sales By Agent";
            row++;


            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                11, true, false, false);
            exRange.Value2 = "SALES_STATE";
            col++;

            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                11, true, false, false);
            exRange.Value2 = "CENTER_ID";
            col++;

            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                11, true, false, false);
            exRange.Value2 = "VENDOR_NAME";
            col++;

            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                11, true, false, false);
            exRange.Value2 = "TSR_ID";
            col++;

            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                11, true, false, false);
            exRange.Value2 = "TSR_NAME";
            col++;

            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                11, true, false, false);
            exRange.Value2 = "DidNotAgreeToServiceAddress";
            col++;

            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                11, true, false, false);
            exRange.Value2 = "CustomerDidNotAgreeToAcctNumMeterNum";
            col++;

            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                11, true, false, false);
            exRange.Value2 = "CustomerDidNotAgreeToTermPrice";
            col++;


            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                11, true, false, false);
            exRange.Value2 = "CustomerDidNotUnderstandETFClause";
            col++;

            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                11, true, false, false);
            exRange.Value2 = "AgentInterruptedTPVProcess";
            col++;

            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                11, true, false, false);
            exRange.Value2 = "CustHungupDisconnectDuringVerification";
            col++;

            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                11, true, false, false);
            exRange.Value2 = "CustomerHadQuestionsDidNotAgree";
            col++;

            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                11, true, false, false);
            exRange.Value2 = "WasNotAuthorized";
            col++;

            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                11, true, false, false);
            exRange.Value2 = "LanguageBarrier";
            col++;

            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                11, true, false, false);
            exRange.Value2 = "AgentActedasCustomer";
            col++;

            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                11, true, false, false);
            exRange.Value2 = "CustomerChangedMind";
            col++;

            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                11, true, false, false);
            exRange.Value2 = "ConnectivityBadTransferConnection";
            col++;

            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                11, true, false, false);
            exRange.Value2 = "TestCall";
            col++;

            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                11, true, false, false);
            exRange.Value2 = "ExistingCustomer";
            col++;

            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                11, true, false, false);
            exRange.Value2 = "CustomerDidNotUnderstandRate";
            col++;

            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                11, true, false, false);
            exRange.Value2 = "CustomerDidNotUnderstandNoSavings";
            col++;

            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                11, true, false, false);
            exRange.Value2 = "CustomerDidNotUnderstandRenewal";
            col++;

            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                11, true, false, false);
            exRange.Value2 = "CustomerDidNotUnderstandRescission";
            col++;

            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                11, true, false, false);
            exRange.Value2 = "CustomerDidNotUnderstandSupplierRelation";
            col++;

            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                11, true, false, false);
            exRange.Value2 = "RefusedRecording";
            col++;

            row++;
            col = dataColumnInitialize; // reset back to Coloumn A

            #endregion Header

            #region Data

            #region Loop for Aggregate State totals

            List<spDTDNoSalesByAgentAllStateAggregate_Result> noSalesByAgentAllStatesAggregate = GetNoSalesByAgentAllStatesAggregate(startDate, endDate, vendorNumber);
            if (noSalesByAgentAllStatesAggregate.Count > 0)
            {
                foreach (var item in noSalesByAgentAllStatesAggregate)
                {
                    //SALES_STATE
                    exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                        11, false, false, false);
                    exRange.Value2 = "ALL";
                    col++;

                    //CENTER_ID
                    exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight,
                        11, false, false, false);
                    exRange.Value2 = item.CenterId;
                    col++;

                    //VENDOR_NAME
                    exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight,
                        11, false, false, false);
                    exRange.Value2 = item.VendorName;
                    col++;

                    //TSR_ID
                    exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                        11, false, false, false);
                    exRange.Value2 = item.TSRId;
                    col++;

                    //TSR_NAME
                    exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight,
                        11, false, false, false);
                    exRange.Value2 = item.TSRName;
                    col++;

                    //DidNotAgreeToServiceAddress
                    exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight,
                        11, false, false, false);
                    exRange.Value2 = item.DidNotAgreeToServiceAddress;
                    col++;

                    //CustomerDidNotAgreeToAcctNumMeterNum 
                    exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight,
                    11, false, false, false);
                    exRange.Value2 = item.CustomerDidNotAgreeToAcctNumMeterNum;
                    col++;

                    //CustomerDidNotAgreeToTermPrice
                    exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight,
                        11, false, false, false);
                    exRange.Value2 = item.CustomerDidNotAgreeToTermPrice;
                    col++;

                    //CustomerDidNotUnderstandETFClause
                    exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight,
                        11, false, false, false);
                    exRange.Value2 = item.CustomerDidNotUnderstandETFClause;
                    col++;

                    //AgentInterruptedTPVProcess
                    exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight,
                        11, false, false, false);
                    exRange.Value2 = item.AgentInterruptedTPVProcess;
                    col++;

                    //CustHungupDisconnectDuringVerification
                    exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight,
                        11, false, false, false);
                    exRange.Value2 = item.CustHungupDisconnectDuringVerification;
                    col++;

                    //CustomerHadQuestionsDidNotAgree
                    exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight,
                        11, false, false, false);
                    exRange.Value2 = item.CustomerHadQuestionsDidNotAgree;
                    col++;

                    //WasNotAuthorized
                    exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight,
                        11, false, false, false);
                    exRange.Value2 = item.WasNotAuthorized;
                    col++;

                    //LanguageBarrier
                    exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight,
                        11, false, false, false);
                    exRange.Value2 = item.LanguageBarrier;
                    col++;

                    //AgentActedasCustomer
                    exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight,
                        11, false, false, false);
                    exRange.Value2 = item.AgentActedasCustomer;
                    col++;

                    //CustomerChangedMind
                    exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight,
                        11, false, false, false);
                    exRange.Value2 = item.CustomerChangedMind;
                    col++;

                    //ConnectivityBadTransferConnection
                    exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight,
                        11, false, false, false);
                    exRange.Value2 = item.ConnectivityBadTransferConnection;
                    col++;

                    //TestCall
                    exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight,
                        11, false, false, false);
                    exRange.Value2 = item.TestCall;
                    col++;

                    //ExistingCustomer
                    exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight,
                        11, false, false, false);
                    exRange.Value2 = item.ExistingCustomer;
                    col++;

                    //CustomerDidNotUnderstandRate
                    exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight,
                        11, false, false, false);
                    exRange.Value2 = item.CustomerDidNotUnderstandRate;
                    col++;

                    //CustomerDidNotUnderstandNoSavings
                    exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight,
                        11, false, false, false);
                    exRange.Value2 = item.CustomerDidNotUnderstandNoSavings;
                    col++;

                    //CustomerDidNotUnderstandRenewal
                    exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight,
                        11, false, false, false);
                    exRange.Value2 = item.CustomerDidNotUnderstandRenewal;
                    col++;

                    //CustomerDidNotUnderstandRescission
                    exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight,
                        11, false, false, false);
                    exRange.Value2 = item.CustomerDidNotUnderstandRescission;
                    col++;

                    //CustomerDidNotUnderstandSupplierRelation
                    exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight,
                        11, false, false, false);
                    exRange.Value2 = item.CustomerDidNotUnderstandSupplierRelation;
                    col++;

                    //RefusedRecording
                    exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight,
                        11, false, false, false);
                    exRange.Value2 = item.RefusedRecording;
                    col++;

                    row++;
                    col = dataColumnInitialize; // reset back to Coloumn A
                }
            }

            #endregion Loop for Aggregate State totals

            #region Loop for State break out

            List<spDTDNoSalesByAgent_Result> noSalesByAgentByState = GetNoSalesByAgentByState(startDate, endDate, vendorNumber);
            if (noSalesByAgentByState.Count > 0)
            {
                foreach (var item in noSalesByAgentByState)
                {
                    //SALES_STATE
                    exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                        11, false, false, false);
                    exRange.Value2 = item.SalesState;
                    col++;

                    //CENTER_ID
                    exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight,
                        11, false, false, false);
                    exRange.Value2 = item.CenterId;
                    col++;

                    //VENDOR_NAME
                    exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight,
                        11, false, false, false);
                    exRange.Value2 = item.VendorName;
                    col++;

                    //TSR_ID
                    exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                        11, false, false, false);
                    exRange.Value2 = item.TSRId;
                    col++;

                    //TSR_NAME
                    exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight,
                        11, false, false, false);
                    exRange.Value2 = item.TSRName;
                    col++;

                    //DidNotAgreeToServiceAddress
                    exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight,
                        11, false, false, false);
                    exRange.Value2 = item.DidNotAgreeToServiceAddress;
                    col++;

                    //CustomerDidNotAgreeToAcctNumMeterNum 
                    exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight,
                    11, false, false, false);
                    exRange.Value2 = item.CustomerDidNotAgreeToAcctNumMeterNum;
                    col++;

                    //CustomerDidNotAgreeToTermPrice
                    exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight,
                        11, false, false, false);
                    exRange.Value2 = item.CustomerDidNotAgreeToTermPrice;
                    col++;

                    //CustomerDidNotUnderstandETFClause
                    exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight,
                        11, false, false, false);
                    exRange.Value2 = item.CustomerDidNotUnderstandETFClause;
                    col++;

                    //AgentInterruptedTPVProcess
                    exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight,
                        11, false, false, false);
                    exRange.Value2 = item.AgentInterruptedTPVProcess;
                    col++;

                    //CustHungupDisconnectDuringVerification
                    exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight,
                        11, false, false, false);
                    exRange.Value2 = item.CustHungupDisconnectDuringVerification;
                    col++;

                    //CustomerHadQuestionsDidNotAgree
                    exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight,
                        11, false, false, false);
                    exRange.Value2 = item.CustomerHadQuestionsDidNotAgree;
                    col++;

                    //WasNotAuthorized
                    exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight,
                        11, false, false, false);
                    exRange.Value2 = item.WasNotAuthorized;
                    col++;

                    //LanguageBarrier
                    exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight,
                        11, false, false, false);
                    exRange.Value2 = item.LanguageBarrier;
                    col++;

                    //AgentActedasCustomer
                    exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight,
                        11, false, false, false);
                    exRange.Value2 = item.AgentActedasCustomer;
                    col++;

                    //CustomerChangedMind
                    exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight,
                        11, false, false, false);
                    exRange.Value2 = item.CustomerChangedMind;
                    col++;

                    //ConnectivityBadTransferConnection
                    exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight,
                        11, false, false, false);
                    exRange.Value2 = item.ConnectivityBadTransferConnection;
                    col++;

                    //TestCall
                    exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight,
                        11, false, false, false);
                    exRange.Value2 = item.TestCall;
                    col++;

                    //ExistingCustomer
                    exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight,
                        11, false, false, false);
                    exRange.Value2 = item.ExistingCustomer;
                    col++;

                    //CustomerDidNotUnderstandRate
                    exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight,
                        11, false, false, false);
                    exRange.Value2 = item.CustomerDidNotUnderstandRate;
                    col++;

                    //CustomerDidNotUnderstandNoSavings
                    exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight,
                        11, false, false, false);
                    exRange.Value2 = item.CustomerDidNotUnderstandNoSavings;
                    col++;

                    //CustomerDidNotUnderstandRenewal
                    exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight,
                        11, false, false, false);
                    exRange.Value2 = item.CustomerDidNotUnderstandRenewal;
                    col++;

                    //CustomerDidNotUnderstandRescission
                    exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight,
                        11, false, false, false);
                    exRange.Value2 = item.CustomerDidNotUnderstandRescission;
                    col++;

                    //CustomerDidNotUnderstandSupplierRelation
                    exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight,
                        11, false, false, false);
                    exRange.Value2 = item.CustomerDidNotUnderstandSupplierRelation;
                    col++;

                    //RefusedRecording
                    exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight,
                        11, false, false, false);
                    exRange.Value2 = item.RefusedRecording;
                    col++;

                    row++;
                    col = dataColumnInitialize; // reset back to Coloumn A
                }
            }

            #endregion Loop for State break out

            #endregion Data
        }

        #endregion WriteNoSalesByAgentReport

        #endregion Excel (5 methods)

        #region GetData (11 methods)

        #region GetVendors (1 method)

        /// <summary>
        /// Gets a list of Vendors to report on the individual report
        /// </summary>
        public static List<Vendor> GetVendors()
        {
            //VendorId	VendorNumber	VendorName
            //4	        43	            Protocall-DTD
            //9	        10	            Watts Marketing Solutions
            //12	        45	            UES
            //17	        66	            National1

            //Filter for DTD Vendors to send
            var myInClause = new int[] { 4, 9, 12, 17 };

            List<Vendor> vendors = null;
            try
            {
                using (ConstellationEntities entities = new ConstellationEntities())
                {
                    vendors = (from v in entities.Vendors
                               where myInClause.Contains(v.VendorId)
                               select v).ToList();
                }
            }
            catch (Exception ex)
            {
                SendErrorMessage(ex);
            }
            return vendors;

        }
        #endregion GetVendors (1 method)

        #region GetPOSIDDashboard (2 methods)
        /// <summary>
        /// Gets the POS ID Dashboard values for a date range and optional Vendor
        /// </summary>
        /// <param name="sDate"></param>
        /// <param name="eDate"></param>
        /// <param name="vendorNumber"></param>
        /// <returns></returns>
        private static spDTDPOSIdDashboardByDay_Result GetPOSIDDashboardByDay(DateTime sDate, DateTime eDate, string vendorNumber)
        {
            spDTDPOSIdDashboardByDay_Result result = null;

            using (ConstellationEntities entities = new ConstellationEntities())
            {
                result = entities.spDTDPOSIdDashboardByDay(startDate: sDate, endDate: eDate, vendorNumber: vendorNumber).FirstOrDefault();
            }

            return result;
        }
        /// <summary>
        /// Gets the POS ID Dashboard values for an entire year and optional Vendor, the sproc will 
        /// convert the DateTime to necessary Date components for report ex: yearDate.Year, yearDate.Month
        /// </summary>
        /// <param name="yearDate"></param>
        /// /// <param name="vendorNumber"></param>
        /// <returns></returns>
        private static List<spDTDPOSIdDashboard_Result> GetPOSIDDashboard(DateTime yearDate, string vendorNumber)
        {
            List<spDTDPOSIdDashboard_Result> result = null;

            using (ConstellationEntities entities = new ConstellationEntities())
            {
                result = entities.spDTDPOSIdDashboard(yearDate: yearDate, vendorNumber: vendorNumber).ToList();
            }

            return result;
        }
        #endregion GetPOSIDDashboard (2 methods)

        #region GetSalesByAgent (2 methods)
        /// <summary>
        /// Get Sales By Agent for all states as an aggregate and optional Vendor
        /// </summary>
        /// <param name="sDate"></param>
        /// <param name="eDate"></param>
        /// <param name="vendorNumber"></param>
        /// <returns></returns>
        private static List<spDTDSalesByAgentAllStateAggregate_Result> GetSalesByAgentAllStatesAggregate(DateTime sDate, DateTime eDate, string vendorNumber)
        {
            List<spDTDSalesByAgentAllStateAggregate_Result> result = null;

            using (ConstellationEntities entities = new ConstellationEntities())
            {
                result = entities.spDTDSalesByAgentAllStateAggregate(startDate: sDate, endDate: eDate, vendorNumber: vendorNumber).ToList();
            }

            return result;
        }
        /// <summary>
        /// Gets Sales By Agent for indvidual states grouping and optional Vendor
        /// </summary>
        /// <param name="sDate"></param>
        /// <param name="eDate"></param>
        /// <param name="vendorNumber"></param>
        /// <returns></returns>
        private static List<spDTDSalesByAgent_Result> GetSalesByAgentByState(DateTime sDate, DateTime eDate, string vendorNumber)
        {
            List<spDTDSalesByAgent_Result> result = null;

            using (ConstellationEntities entities = new ConstellationEntities())
            {
                result = entities.spDTDSalesByAgent(startDate: sDate, endDate: eDate, serviceState: null, vendorNumber: vendorNumber).ToList();
            }

            return result;
        }
        #endregion GetSalesByAgent (2 methods)

        #region GetSalesByFuelType (2 methods)
        /// <summary>
        /// Get Sales By Fuel Type for all states as an aggregate and optional Vendor
        /// </summary>
        /// <param name="sDate"></param>
        /// <param name="eDate"></param>
        /// <param name="vendorNumber"></param>
        /// <returns></returns>
        private static List<spDTDSalesByFuelTypeAllStateAggregate_Result> GetSalesByFuelTypeAllStatesAggregate(DateTime sDate, DateTime eDate, string vendorNumber)
        {
            List<spDTDSalesByFuelTypeAllStateAggregate_Result> result = null;

            using (ConstellationEntities entities = new ConstellationEntities())
            {
                result = entities.spDTDSalesByFuelTypeAllStateAggregate(startDate: sDate, endDate: eDate, vendorNumber: vendorNumber).ToList();
            }

            return result;
        }
        /// <summary>
        /// Gets Sales By Fuel Type for indvidual states grouping and optional Vendor
        /// </summary>
        /// <param name="sDate"></param>
        /// <param name="eDate"></param>
        /// <param name="vendorNumber"></param>
        /// <returns></returns>
        private static List<spDTDSalesByFuelType_Result> GetSalesByFuelTypeByState(DateTime sDate, DateTime eDate, string vendorNumber)
        {
            List<spDTDSalesByFuelType_Result> result = null;

            using (ConstellationEntities entities = new ConstellationEntities())
            {
                result = entities.spDTDSalesByFuelType(startDate: sDate, endDate: eDate, serviceState: null, vendorNumber: vendorNumber).ToList();
            }

            return result;
        }
        #endregion GetSalesByFuelType (2 methods)

        #region GetNoSalesByAgent (2 methods)
        /// <summary>
        /// Get No Sales By Agent for all states as an aggregate and optional Vendor
        /// </summary>
        /// <param name="sDate"></param>
        /// <param name="eDate"></param>
        /// <param name="vendorNumber"></param>
        /// <returns></returns>
        private static List<spDTDNoSalesByAgentAllStateAggregate_Result> GetNoSalesByAgentAllStatesAggregate(DateTime sDate, DateTime eDate, string vendorNumber)
        {
            List<spDTDNoSalesByAgentAllStateAggregate_Result> result = null;

            using (ConstellationEntities entities = new ConstellationEntities())
            {
                result = entities.spDTDNoSalesByAgentAllStateAggregate(startDate: sDate, endDate: eDate, vendorNumber: vendorNumber).ToList();
            }

            return result;
        }
        /// <summary>
        /// Gets No Sales By Agent for indvidual states grouping and optional Vendor
        /// </summary>
        /// <param name="sDate"></param>
        /// <param name="eDate"></param>
        /// <param name="vendorNumber"></param>
        /// <returns></returns>
        private static List<spDTDNoSalesByAgent_Result> GetNoSalesByAgentByState(DateTime sDate, DateTime eDate, string vendorNumber)
        {
            List<spDTDNoSalesByAgent_Result> result = null;

            using (ConstellationEntities entities = new ConstellationEntities())
            {
                result = entities.spDTDNoSalesByAgent(startDate: sDate, endDate: eDate, serviceState: null, vendorNumber: vendorNumber).ToList();
            }

            return result;
        }
        #endregion GetNoSalesByAgent (2 methods)

        #region GetAlertsByAgent (2 methods)
        /// <summary>
        /// Get Sales By Agent for all states as an aggregate and optional Vendor
        /// </summary>
        /// <param name="sDate"></param>
        /// <param name="eDate"></param>
        /// <param name="vendorNumber"></param>
        /// <returns></returns>
        private static List<spDTDAlertsByAgentAllStateAggregate_Result> GetAlertsByAgentAllStatesAggregate(DateTime sDate, DateTime eDate, string vendorNumber)
        {
            List<spDTDAlertsByAgentAllStateAggregate_Result> result = null;

            using (ConstellationEntities entities = new ConstellationEntities())
            {
                result = entities.spDTDAlertsByAgentAllStateAggregate(startDate: sDate, endDate: eDate, vendorNumber: vendorNumber).ToList();
            }

            return result;
        }
        /// <summary>
        /// Gets Sales By Agent for indvidual states grouping and optional Vendor
        /// </summary>
        /// <param name="sDate"></param>
        /// <param name="eDate"></param>
        /// <param name="vendorNumber"></param>
        /// <returns></returns>
        private static List<spDTDAlertsByAgent_Result> GetAlertsByAgentByState(DateTime sDate, DateTime eDate, string vendorNumber)
        {
            List<spDTDAlertsByAgent_Result> result = null;

            using (ConstellationEntities entities = new ConstellationEntities())
            {
                result = entities.spDTDAlertsByAgent(startDate: sDate, endDate: eDate, serviceState: null, vendorNumber: vendorNumber).ToList();
            }

            return result;
        }
        #endregion GetAlertsByAgent (2 methods)

        #endregion GetData (10 methods)

        #region Utilities

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

        private static void SendEmail(ref string xlsFilePath, DateTime reportDate, string strToEmail, string strBccEmail, string vendorName)
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
                if (IsValueNull(vendorName))
                {
                    mail.Subject = "cne_res_dtd_des_report_daily_all_vendors_" + String.Format("{0:yyyyMMdd}", reportDate) + ".xlsx";
                }
                else
                {
                    mail.Subject = "cne_res_dtd_des_report_daily_" + vendorName.Replace(" ", string.Empty) + "_" + String.Format("{0:yyyyMMdd}", reportDate) + ".xlsx";
                }

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
        private static void SaveXlsDocument(ref string reportPath, ref string xlsFilename, ref string xlsFilePath, Excel.Workbook exBook, DateTime reportDate, string vendorName)
        {
            if (IsValueNull(vendorName))
            {
                //Build the file name
                //cne_res_dtd_des_report_daily_all_vendors_20151015.xlsx
                xlsFilename = "cne_res_dtd_des_report_daily_all_vendors_" + String.Format("{0:yyyyMMdd}", reportDate) + ".xlsx";
            }
            else
            {
                //Build the file name
                //cne_res_dtd_des_report_daily_[vendorName]_20151015.xlsx
                xlsFilename = "cne_res_dtd_des_report_daily_" + vendorName.Replace(" ", string.Empty) + "_" + String.Format("{0:yyyyMMdd}", reportDate) + ".xlsx";
            }

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

        private static void GetDates(out DateTime StartDate, out DateTime EndDate)
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
            StartDate = new DateTime(baseDate.Year, baseDate.Month, baseDate.Day, 0, 0, 0).AddDays(-1);//Previous Day
            EndDate = new DateTime(baseDate.Year, baseDate.Month, baseDate.Day, 0, 0, 0);//current date time

        }
        #endregion Utilities

        #region Error Handling
        private static void SendErrorMessage(Exception ex)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendFormat("ex:{0}, innerEx:{1}", ex.Message, ex.InnerException == null ? "" : ex.InnerException.Message);

            Calibrus.ErrorHandler.Alerting alert = new Calibrus.ErrorHandler.Alerting("ConstellationDTDCnesResDesDailyXLS");
            alert.SendAlert(ex.Source, sb.ToString(), Environment.MachineName, Environment.UserName, Environment.Version.ToString());
        }
        #endregion Error Handling
    }
}
