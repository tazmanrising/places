using Calibrus.ExcelFunctions;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using Excel = Microsoft.Office.Interop.Excel;

namespace SparkTXESIIDXLS
{
    public class Report
    {
        public static object na = System.Reflection.Missing.Value;

        #region Main

        public static void Main(string[] args)
        {
            string rootPath = string.Empty;
            string hostName = string.Empty;
            string userName = string.Empty;
            string password = string.Empty;

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

            hostName = ConfigurationManager.AppSettings["hostName"].ToString();
            userName = ConfigurationManager.AppSettings["userName"].ToString();
            password = ConfigurationManager.AppSettings["password"].ToString();

            #region Build UpdatedESIID Report
            List<Record> recordList = GetListOfUpdatedESIIDRecords(StartDate, EndDate);

            //if we have records then we can build the report
            if (recordList.Count() > 0)
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

                    //newtab

                    string sheetName = String.Format("{0:yyyy_MM_dd}", StartDate) + "_cal_TX_UpdatedESIID";
                    exSheet.Name = sheetName.Length > 30 ? sheetName.Substring(0, 30) : sheetName; //force length of sheet name due to excel constraints
                    exSheet.Select(na);

                    //write out Report
                    WriteReport(ref exApp, ref exRange, StartDate, EndDate, recordList);

                    //save report
                    SaveXlsDocument(ref rootPath, ref xlsFilename, ref xlsFilePath, exBook, StartDate, "UpdatedESIID");
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
                //FTP report
                FTPFile(ref rootPath, ref xlsFilename, ref xlsFilePath, StartDate, "UpdatedESIID", hostName, userName, password);
            }
            #endregion Build UpdatedESIID Report

            #region BUild NewESIID Report
            recordList.Clear();
            recordList = GetListOfNewESIIDRecords(StartDate, EndDate);

            //if we have records then we can build the report
            if (recordList.Count() > 0)
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

                    //newtab

                    string sheetName = String.Format("{0:yyyy_MM_dd}", StartDate) + "_cal_TX_NewESIID";
                    exSheet.Name = sheetName.Length > 30 ? sheetName.Substring(0, 30) : sheetName; //force length of sheet name due to excel constraints
                    exSheet.Select(na);

                    //write out Report
                    WriteReport(ref exApp, ref exRange, StartDate, EndDate, recordList);

                    //save report                   
                    SaveXlsDocument(ref rootPath, ref xlsFilename, ref xlsFilePath, exBook, StartDate, "NewESIID");
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
                //FTP report
                FTPFile(ref rootPath, ref xlsFilename, ref xlsFilePath, StartDate, "NewESIID", hostName, userName, password);
            }
            #endregion BUild NewESIID Report

        }
        #endregion Main

        #region Excel

        private static void WriteReport(ref Excel.Application exApp, ref Excel.Range exRange, DateTime sDate, DateTime eDate, List<Record> listOfRecords)
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

            //UCID
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "UCID";
            col++;

            //ActionType
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "ActionType";
            col++;

            //CustomerGrouping
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "CustomerGrouping";
            col++;

            //Utility
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "Utility";
            col++;

            //CommodityType
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "CommodityType";
            col++;

            //BillingType
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "BillingType";
            col++;

            //ContractPath
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "ContractPath";
            col++;

            //UtilityAccountNumber
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "UtilityAccountNumber";
            col++;

            //AlternateAccountNumber
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "AlternateAccountNumber";
            col++;

            //UtilityMeterNumber
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "UtilityMeterNumber";
            col++;

            //MeterType
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "MeterType";
            col++;

            //CustomerType
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "CustomerType";
            col++;

            //CompanyName
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "CompanyName";
            col++;

            //DBAName
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "DBAName";
            col++;

            //NameKey
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "NameKey";
            col++;

            //ServiceFirstName
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "ServiceFirstName";
            col++;

            //ServiceLastName
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "ServiceLastName";
            col++;

            //ServiceAddress1
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "ServiceAddress1";
            col++;

            //ServiceAddress2
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "ServiceAddress2";
            col++;

            //ServiceCity
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "ServiceCity";
            col++;

            //ServiceState
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "ServiceState";
            col++;

            //ServiceZip
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "ServiceZip";
            col++;

            //ServiceCounty
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "ServiceCounty";
            col++;

            //ServiceEmail
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "ServiceEmail";
            col++;

            //ServicePhone
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            col++;
            exRange.Value2 = "ServicePhone";

            //ServiceFax
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "ServiceFax";
            col++;

            //BillingFirstName
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "BillingFirstName";
            col++;

            //BillingLastName
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "BillingLastName";
            col++;

            //BillingAddress1
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "BillingAddress1";
            col++;

            //BillingAddress2
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "BillingAddress2";
            col++;

            //BillingCity
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "BillingCity";
            col++;

            //BillingState
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "BillingState";
            col++;

            //BillingZip
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "BillingZip";
            col++;

            //BillingCounty
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "BillingCounty";
            col++;

            //BillingEmail
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, true, false, false);
            exRange.Value2 = "BillingEmail";
            col++;

            //BillingPhone
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "BillingPhone";
            col++;

            //BillingFax
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "BillingFax";
            col++;

            //DateOfBirth
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "DateOfBirth";
            col++;

            //SSN
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "SSN";
            col++;

            //Language
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "Language";
            col++;

            //DeliveryType
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "DeliveryType";
            col++;

            //LifeSupport
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "LifeSupport";
            col++;

            //TaxID
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "TaxID";
            col++;

            //TaxExempt
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "TaxExempt";
            col++;

            //TaxExempt%
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "TaxExempt%";
            col++;

            //PromoCode
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "PromoCode";
            col++;

            //ReferFriendID
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "ReferFriendID";
            col++;

            //ProductType
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "ProductType";
            col++;

            //ProductOffering
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "ProductOffering";
            col++;

            //CommodityPrice
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "CommodityPrice";
            col++;

            //TermMonths
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "TermMonths";
            col++;

            //MonthlyFee
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "MonthlyFee";
            col++;

            //DailyCharge
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "DailyCharge";
            col++;

            //ETF
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "ETF";
            col++;

            //RolloverProduct
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "RolloverProduct";
            col++;

            //isPriorityMovein
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "isPriorityMovein";
            col++;

            //MoveInDate
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "MoveInDate";
            col++;

            //SwitchDate
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "SwitchDate";
            col++;

            //StartMonthYear
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "StartMonthYear";
            col++;

            //ReleaseDate
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "ReleaseDate";
            col++;

            //ReadCycle
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "ReadCycle";
            col++;

            //Marketer
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "Marketer";
            col++;

            //Marketer2
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "Marketer2";
            col++;

            //ExternalSalesID
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "ExternalSalesID";
            col++;

            //SalesChannel
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "SalesChannel";
            col++;

            //SalesAgent
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "SalesAgent";
            col++;

            //SoldDate
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "SoldDate";
            col++;

            //TelemarketingCall
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "TelemarketingCall";
            col++;

            //TPVCall
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "TPVCall";
            col++;

            //AcknowledgeLetterOfAgency
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "AcknowledgeLetterOfAgency";
            col++;

            //Notes
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "Notes";
            col++;

            //ServicePlanOptionID
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "ServicePlanOptionID";
            col++;

            //GRT
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "GRT";
            col++;

            //TOUMeter
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "TOUMeter";
            col++;

            //GasPool
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "GasPool";
            col++;

            //Zone
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "Zone";
            col++;

            //Pipeline
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "Pipeline";
            col++;

            //AggregatorFee
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "AggregatorFee";
            col++;

            //Adder
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "Adder";
            col++;

            //RateClass
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "RateClass";
            col++;

            //Usage
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "Usage";
            col++;

            //JanContractedUsage
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "JanCOntractedUsage";
            col++;

            //FebContractedUsage
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "FebContractedUsage";
            col++;

            //MarContractedUsage
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "MarContractedUsage";
            col++;

            //AprContractedUsage
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "AprContractedUsage";
            col++;

            //MayContractedUsage
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "MayContractedUsage";
            col++;

            //JuneContractedUsage
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "JuneContractedUsage";
            col++;

            //JulyContractedUsage
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "JulyContractedUsage";
            col++;

            //AugContractedUsage
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "AugContractedUsage";
            col++;

            //SepContractedUsage
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "SepContractedUsage";
            col++;

            //OctContractedUsage
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "OctContractedUsage";
            col++;

            //NovContractedUsage
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "NovContractedUsage";
            col++;

            //DecContractedUsage
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "DecContractedUsage";
            col++;

            //UpperBand
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "UpperBand";
            col++;

            //LowerBand
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "LowerBand";
            col++;

            //FeeAbove
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "FeeAbove";
            col++;

            //OverIndex
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "OverIndex";
            col++;

            //FeeBelow
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "FeeBelow";
            col++;

            //UnderIndex
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "UnderIndex";
            col++;

            //ChargeFuel
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "ChargeFuel";
            col++;

            //DemandCharge
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "DemandCharge";
            col++;

            //IsNodal
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "IsNodal";
            col++;

            //RateIndexId
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "RateIndexId";
            col++;

            //NetTerms
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "NetTerms";
            col++;

            //EffectiveStartDate
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "EffectiveStartDate";
            col++;

            //EffectiveEndDate
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "EffectiveEndDate";
            col++;

            //CreditCheck
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "CreditCheck";
            col++;

            col = colInitialize;
            row++;

            #endregion Header

            #region Data

            foreach (Record record in listOfRecords)
            {
                //UCID
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = "";
                col++;

                //ActionType
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = "Enrollment";
                col++;

                //CustomerGrouping
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = "";
                col++;

                //Utility
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = record.Utility;
                col++;

                //CommodityType
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = record.CommodityType;
                col++;

                //BillingType
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = "";
                col++;

                //ContractPath
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = "MassMarket";
                col++;

                //UtilityAccountNumber
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = record.UtilityAccountNumber;
                col++;

                //AlternateAccountNumber
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = record.ServiceReferenceNumber;
                col++;

                //UtilityMeterNumber
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = record.MeterNumber;
                col++;

                //MeterType
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = "";
                col++;

                //CustomerType
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = record.CommercialCustomerType;
                col++;

                //CompanyName
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = record.CompanyName;
                col++;

                //DBAName
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = record.CompanyName;
                col++;

                //NameKey
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = record.NameKey;
                col++;

                //ServiceFirstName
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = record.ServiceFirstName;
                col++;

                //ServiceLastName
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = record.ServiceLastName;
                col++;

                //ServiceAddress1
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = record.ServiceAddress1;
                col++;

                //ServiceAddress2
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = "";
                col++;

                //ServiceCity
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = record.ServiceCity;
                col++;

                //ServiceState
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = record.ServiceState;
                col++;

                //ServiceZip
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = record.ServiceZip;
                col++;

                //ServiceCounty
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = record.ServiceCounty;
                col++;

                //ServiceEmail
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = record.ServiceEmail;
                col++;

                //ServicePhone
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = record.ServicePhone;
                col++;

                //ServiceFax
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = "";
                col++;

                //BillingFirstName
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = record.BillingFirstName;
                col++;

                //BillingLastName
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = record.BillingLastName;
                col++;

                //BillingAddress1
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = record.BillingAddress1;
                col++;

                //BillingAddress2
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = "";
                col++;

                //BillingCity
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = record.BillingCity;
                col++;

                //BillingState
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = record.BillingState;
                col++;

                //BillingZip
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = record.BillingZip;
                col++;

                //BillingCounty
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = record.BillingCounty;
                col++;

                //BillingEmail
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = record.BillingEmail;
                col++;

                //BillingPhone
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = record.BillingPhone;
                col++;

                //BillingFax
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = "";
                col++;

                //DateOfBirth
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = "";
                col++;

                //SSN
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = "";
                col++;

                //Language
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = record.Language;
                col++;

                //DeliveryType
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = "";
                col++;

                //LifeSupport
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = "N";
                col++;

                //TaxID
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = "";
                col++;

                //TaxExempt
                string taxExempt = string.Empty;
                if (record.Utility.ToLower() == "bosted" || (record.Utility.ToLower() == "nstar" && record.PremiseTypeName.ToLower() == "residential"))
                {
                    taxExempt = "Y";
                }
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = taxExempt;
                col++;

                //TaxExempt%
                string taxExemptPercent = string.Empty;
                if (record.Utility.ToLower() == "bosted" || (record.Utility.ToLower() == "nstar" && record.PremiseTypeName.ToLower() == "residential"))
                {
                    taxExemptPercent = "100";
                }
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = taxExemptPercent;
                col++;

                //PromoCode
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = "";
                col++;

                //ReferFriendID
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = "";
                col++;

                //ProductType
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = "";
                col++;

                //ProductOffering
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = record.ProductOffering;
                col++;

                //CommodityPrice
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.NumberFormat = "0.000";
                exRange.Value2 = record.CommodityPrice;
                col++;

                //TermMonths
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = record.TermMonths;
                col++;

                //MonthlyFee
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.NumberFormat = "0.00";
                exRange.Value2 = record.MonthlyFee;
                col++;

                //DailyCharge
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = "";
                col++;

                //ETF
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.NumberFormat = "0.00";
                exRange.Value2 = record.ETF;
                col++;

                //RolloverProduct
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = "";
                col++;

                //isPriorityMovein
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = "";
                col++;

                //MoveInDate
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = "";
                col++;

                //SwitchDate
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = record.SwitchDate;
                col++;

                //StartMonthYear
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = "";
                col++;

                //ReleaseDate
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = "";
                col++;

                //ReadCycle
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = "";
                col++;

                //Marketer
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = record.Marketer;
                col++;

                //Marketer2
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = "";
                col++;

                //ExternalSalesID
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = record.ExternalSalesID;
                col++;

                //SalesChannel
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = record.UtilitySalesChannelName;
                col++;

                //SalesAgent
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = record.SalesAgent;
                col++;

                //SoldDate
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = string.Format("{0:MM/dd/yyyy}", record.SoldDate);
                col++;

                //TelemarketingCall
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = "";
                col++;

                //TPVCall
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = "";
                col++;

                //AcknowledgeLetterOfAgency
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = "";
                col++;

                //Notes
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = "";
                col++;

                //ServicePlanOptionID
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = "";
                col++;

                //GRT
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = "";
                col++;

                //TOUMeter
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = "";
                col++;

                //GasPool
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = "";
                col++;

                //Zone
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = "";
                col++;

                //Pipeline
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = "";
                col++;

                //AggregatorFee
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = "";
                col++;

                //Adder
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = "";
                col++;

                //RateClass
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = record.RateClass;
                col++;

                //Usage
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = "";
                col++;

                //JanContractedUsage
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = "";
                col++;

                //FebContractedUsage
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = "";
                col++;

                //MarContractedUsage
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = "";
                col++;

                //AprContractedUsage
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = "";
                col++;

                //MayContractedUsage
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = "";
                col++;

                //JuneContractedUsage
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = "";
                col++;

                //JulyContractedUsage
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = "";
                col++;

                //AugContractedUsage
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = "";
                col++;

                //SepContractedUsage
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = "";
                col++;

                //OctContractedUsage
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = "";
                col++;

                //NovContractedUsage
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = "";
                col++;

                //DecContractedUsage
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = "";
                col++;

                //UpperBand
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = "";
                col++;

                //LowerBand
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = "";
                col++;

                //FeeAbove
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = "";
                col++;

                //OverIndex
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = "";
                col++;

                //FeeBelow
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = "";
                col++;

                //UnderIndex
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = "";
                col++;

                //ChargeFuel
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = "";
                col++;

                //DemandCharge
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = "";
                col++;

                //IsNodal
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = "";
                col++;

                //RateIndexId
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = "";
                col++;

                //NetTerms
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = "";
                col++;

                //EffectiveStartDate
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = "";
                col++;

                //EffectiveEndDate
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = "";
                col++;

                //CreditCheck
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = record.CreditCheck;

                col = colInitialize;
                row++;
            }

            #endregion Data

            exRange = (Excel.Range)exApp.get_Range("A1", "DC1");
            exRange.EntireColumn.AutoFit();
        }

        #endregion Excel

        #region Get Data

        #region Methods to get RecordData (2 methods)

        
        private static List<Record> GetListOfUpdatedESIIDRecords(DateTime sDate, DateTime eDate)
        {
            //SELECT distinct uty.LdcCode, ut.UtilityTypeName, od.AccountNumber, pt.PremiseTypeName, od.CustomerNameKey, m.AuthorizationFirstName, m.AuthorizationLastName, od.ServiceAddress,
            //     od.ServiceCity,od.ServiceState, od.ServiceZip, od.ServiceCounty, m.Email, m.Btn, m.AccountFirstName,m.AccountLastName,od.BillingFirstName,od.BillingLastName,
            //     od.BillingAddress,od.BillingCity, od.BillingState, od.BillingZip,od.BillingCounty, m.Email, m.Btn,u.Language, p.ProgramName, p.Rate,p.Term, p.Msf, p.Etf,
            //     o.MarketerCode,od.OrderDetailId, sc.Name,u.AgentId,m.CallDateTime, od.RateClass, od.MeterNumber, usc.UtilitySalesChannelName, od.ServiceReferenceNumber,
            //     m.SwitchDate, p.CreditCheck, m.CompanyName
            //         FROM [Spark].[v1].[Main] m
            //         join [Spark].[v1].[OrderDetail] od on m.mainid = od.MainId
            //         join [Spark].[v1].[Program] p on od.ProgramId = p.ProgramId
            //         join [Spark].[v1].[UnitOfMeasure] uom on p.UnitOfMeasureId = uom.UnitOfMeasureId
            //         join [Spark].[v1].[UtilityType] ut on p.UtilityTypeId = ut.UtilityTypeId
            //         join [Spark].[v1].[Utility] uty on p.UtilityId = uty.UtilityId
            //         join [Spark].[v1].[AccountNumberType] ant on p.AccountNumberTypeId = ant.AccountNumberTypeId
            //         join [Spark].[v1].[User] u on m.UserId = u.UserId
            //         join [Spark].[v1].[Vendor] v on v.VendorId = u.VendorId
            //         join [Spark].[v1].[PremiseType] pt on p.PremiseTypeId = pt.PremiseTypeId
            //         join [Spark].[v1].[Office] o on u.[OfficeId] = o.[OfficeId]
            //         join [Spark].[v1].[SalesChannel] sc on o.[SalesChannelId] = sc.[SalesChannelId]
            //         join [Spark].[v1].[UtilitySalesChannel] usc on uty.UtilityId = usc.UtilityId
            //         JOIN [Spark].[v1].[Leads] l on l.LeadsId = m.LeadsId
            //         where m.CallDateTime > '6/1/2017' and m.CallDateTime < '7/1/2017'
            //         and m.Verified ='1'
            //         and m.SalesState ='TX'           	
            //         and p.BrandId = 1
            //         and usc.SalesChannelId = o.SalesChannelId
            //         and l.ESIID <> od.AccountNumber
            //         and (l.ESIID is null or l.ESIID = '')
            //         and (od.AccountNumber is not null and od.AccountNumber <>'')

            List<Record> records = new List<Record>();
            try
            {
                using (SparkEntities entitites = new SparkEntities())
                {
                    //string rateClass = string.Empty;
                    //string nameKey = string.Empty;

                    var query = (from m in entitites.Mains
                                 join od in entitites.OrderDetails on m.MainId equals od.MainId
                                 join p in entitites.Programs on od.ProgramId equals p.ProgramId
                                 join uom in entitites.UnitOfMeasures on p.UnitOfMeasureId equals uom.UnitOfMeasureId
                                 join ut in entitites.UtilityTypes on p.UtilityTypeId equals ut.UtilityTypeId
                                 join uty in entitites.Utilities on p.UtilityId equals uty.UtilityId
                                 join ant in entitites.AccountNumberTypes on p.AccountNumberTypeId equals ant.AccountNumberTypeId
                                 join u in entitites.Users on m.UserId equals u.UserId
                                 join v in entitites.Vendors on u.VendorId equals v.VendorId
                                 join pt in entitites.PremiseTypes on p.PremiseTypeId equals pt.PremiseTypeId
                                 join o in entitites.Offices on u.OfficeId equals o.OfficeId
                                 join sc in entitites.SalesChannels on o.SalesChannelId equals sc.SalesChannelId
                                 join usc in entitites.UtilitySalesChannels on uty.UtilityId equals usc.UtilityId
                                 join l in entitites.Leads on m.LeadsId equals l.LeadsId
                                 where m.CallDateTime > sDate && m.CallDateTime < eDate
                                 && m.Verified == "1"
                                 && m.SalesState == "TX"
                                 && p.BrandId == 1 //We only want Spark data Not Oasis or Censtar
                                 && usc.SalesChannelId == o.SalesChannelId
                                 && l.ESIID != od.AccountNumber
                                 && (l.ESIID == null || l.ESIID == "")
                                 && (od.AccountNumber != null && od.AccountNumber != "")
                                 select new
                                 {
                                     Utility = string.IsNullOrEmpty(uty.LdcCode) ? string.Empty : uty.LdcCode.ToUpper(),
                                     CommodityType = string.IsNullOrEmpty(ut.UtilityTypeName) ? string.Empty : ut.UtilityTypeName.ToUpper(),
                                     UtilityAccountNumber = od.AccountNumber.ToUpper(),
                                     PremiseTypeName = string.IsNullOrEmpty(pt.PremiseTypeName) ? string.Empty : pt.PremiseTypeName.ToUpper(),
                                     NameKey = string.IsNullOrEmpty(od.CustomerNameKey) ? string.Empty : od.CustomerNameKey.ToUpper(),
                                     AuthorizationFirstName = string.IsNullOrEmpty(m.AuthorizationFirstName) ? string.Empty : m.AuthorizationFirstName.ToUpper(),
                                     AuthorizationLastName = string.IsNullOrEmpty(m.AuthorizationLastName) ? string.Empty : m.AuthorizationLastName.ToUpper(),
                                     ServiceAddress1 = string.IsNullOrEmpty(od.ServiceAddress) ? string.Empty : od.ServiceAddress.ToUpper(),
                                     ServiceCity = string.IsNullOrEmpty(od.ServiceCity) ? string.Empty : od.ServiceCity.ToUpper(),
                                     ServiceState = string.IsNullOrEmpty(od.ServiceState) ? string.Empty : od.ServiceState.ToUpper(),
                                     ServiceZip = od.ServiceZip,
                                     ServiceCounty = string.IsNullOrEmpty(od.ServiceCounty) ? string.Empty : od.ServiceCounty.ToUpper(),
                                     ServiceEmail = string.IsNullOrEmpty(m.Email) ? string.Empty : m.Email.ToUpper(),
                                     ServicePhone = m.Btn,
                                     AccountFirstName = string.IsNullOrEmpty(m.AccountFirstName) ? string.Empty : m.AccountFirstName.ToUpper(),
                                     AccountLastName = string.IsNullOrEmpty(m.AccountLastName) ? string.Empty : m.AccountLastName.ToUpper(),
                                     BillingFirstName = string.IsNullOrEmpty(od.BillingFirstName) ? string.Empty : od.BillingFirstName.ToUpper(),
                                     BillingLastName = string.IsNullOrEmpty(od.BillingLastName) ? string.Empty : od.BillingLastName.ToUpper(),
                                     BillingAddress1 = string.IsNullOrEmpty(od.BillingAddress) ? string.Empty : od.BillingAddress.ToUpper(),
                                     BillingCity = string.IsNullOrEmpty(od.BillingCity) ? string.Empty : od.BillingCity.ToUpper(),
                                     BillingState = string.IsNullOrEmpty(od.BillingState) ? string.Empty : od.BillingState.ToUpper(),
                                     BillingZip = od.BillingZip,
                                     BillingCounty = string.IsNullOrEmpty(od.BillingCounty) ? string.Empty : od.BillingCounty.ToUpper(),
                                     BillingEmail = string.IsNullOrEmpty(m.Email) ? string.Empty : m.Email.ToUpper(),
                                     BillingPhone = m.Btn,
                                     //Language = string.IsNullOrEmpty(u.Language) ? string.Empty : u.Language.ToUpper(),
                                     ProductOffering = string.IsNullOrEmpty(p.ProgramName) ? string.Empty : p.ProgramName.ToUpper(),
                                     CommodityPrice = p.Rate,
                                     TermMonths = p.Term,
                                     MonthlyFee = p.Msf,
                                     ETF = p.Etf,
                                     Marketer = string.IsNullOrEmpty(o.MarketerCode) ? string.Empty : o.MarketerCode.ToUpper(),
                                     ExternalSalesID = od.OrderDetailId,
                                     SalesChannel = string.IsNullOrEmpty(sc.Name) ? string.Empty : sc.Name.ToUpper(),
                                     SalesAgent = string.IsNullOrEmpty(u.AgentId) ? string.Empty : u.AgentId.ToUpper(),
                                     SoldDate = m.CallDateTime,
                                     RateClass = od.RateClass,
                                     MeterNumber = od.MeterNumber,
                                     UtilitySalesChannelName = string.IsNullOrEmpty(usc.UtilitySalesChannelName) ? string.Empty : usc.UtilitySalesChannelName.ToUpper(),
                                     ServiceReferenceNumber = string.IsNullOrEmpty(od.ServiceReferenceNumber) ? string.Empty : od.ServiceReferenceNumber.ToUpper(),
                                     SwitchDate = string.IsNullOrEmpty(m.SwitchDate) ? string.Empty : m.SwitchDate.ToUpper(),
                                     CreditCheck = p.CreditCheck,
                                     CommercialCustomerType = string.IsNullOrEmpty(uty.CommercialCustomerType) ? string.Empty : uty.CommercialCustomerType.ToUpper(),
                                     CompanyName = string.IsNullOrEmpty(m.CompanyName) ? string.Empty : m.CompanyName.ToUpper(),
                                     Dnis = m.Dnis,
                                     LeadsId = m.LeadsId
                                 }).Distinct().ToList();

                    ////Get rateClass
                    //if (ldcCode == "BOSTED")
                    //{
                    //    rateClass = "R1";
                    //}
                    //else if (ldcCode == "BGE")
                    //{
                    //    if (utilityTypeName == "Electric")
                    //    {
                    //        rateClass = "R";
                    //    }
                    //    else
                    //    {
                    //        rateClass = "D";
                    //    }
                    //}
                    //else if (ldcCode == "CONED")
                    //{
                    //    rateClass = "";//empty on purpose
                    //}
                    //else if (ldcCode == "NIMO")
                    //{
                    //    rateClass = "Sc1";
                    //}
                    //else if (ldcCode == "PSEG")
                    //{
                    //    rateClass = "R";
                    //}

                    foreach (var item in query)
                    {


                        //Get nameKey
                        //nameKey = item.BillingLastName.Substring(0, 4);
                        string serviceFirstName = string.Empty;
                        string serviceLastName = string.Empty;
                        string billingFirstName = string.Empty;
                        string billingLastName = string.Empty;

                        string commercialCustomerType = item.CommercialCustomerType;

                        if (item.PremiseTypeName.ToLower() == "commercial")
                        {
                            if (item.Utility == "CLP"
                                || item.Utility == "UIC"
                                || item.Utility == "BGE"
                                || item.Utility == "PSEG"
                                || item.Utility == "COH"
                                || item.Utility == "DEO"
                                || item.Utility == "PECO"
                                || item.Utility == "PECO"
                                || item.Utility == "PPL"
                                || item.Utility == "CMS"
                                || item.Utility == "NIPSCO")
                            {
                                commercialCustomerType = item.CommercialCustomerType;// "RESIDENTIAL";
                            }
                            else
                            {
                                if (string.IsNullOrEmpty(commercialCustomerType))
                                {
                                    commercialCustomerType = "SMALL COMMERCIAL";
                                }
                            }

                            serviceFirstName = item.AuthorizationFirstName;
                            serviceLastName = item.AuthorizationLastName;
                            billingFirstName = item.AuthorizationFirstName;
                            billingLastName = item.AuthorizationLastName;
                        }
                        else
                        {
                            commercialCustomerType = item.PremiseTypeName;//Residential Reports get the PremiseTypeName from the db

                            serviceFirstName = item.BillingFirstName;
                            serviceLastName = item.BillingLastName;
                            billingFirstName = item.BillingFirstName;
                            billingLastName = item.BillingLastName;
                        }

                        string language = string.Empty;
                        if (!IsValueNull(item.Dnis))
                        {
                            switch (item.Dnis)
                            {
                                //English: 1324, 1322                        
                                case "1324":
                                case "1322":
                                    language = "ENGLISH";
                                    break;
                                //Spanish: 1325, 1323
                                case "1325":
                                case "1323":
                                    language = "SPANISH";
                                    break;
                            }
                        }
                        else
                        {
                            language = string.Empty;
                        }

                        //UtilityId LdcCode
                        //15 NSG
                        //16 PGL
                        string utilityAccountNumber = item.UtilityAccountNumber;
                        switch (item.Utility)
                        {
                            case "NSG":
                            case "PGL":
                                if (utilityAccountNumber.Length == 15)
                                    utilityAccountNumber = utilityAccountNumber.Insert(10, "-");

                                break;
                        }


                        Record record = new Record(item.Utility, item.CommodityType, utilityAccountNumber, item.PremiseTypeName,
                                                    item.NameKey, serviceFirstName, serviceLastName, item.ServiceAddress1, item.ServiceCity,
                                                    item.ServiceState, item.ServiceZip, item.ServiceCounty, item.ServiceEmail, item.ServicePhone, item.AccountFirstName,
                                                    item.AccountLastName, billingFirstName, billingLastName, item.BillingAddress1, item.BillingCity,
                                                    item.BillingState, item.BillingZip, item.BillingCounty, item.BillingEmail, item.BillingPhone, language,
                                                    item.ProductOffering, item.CommodityPrice, item.TermMonths, item.MonthlyFee, item.ETF, item.Marketer,
                                                    "CAL" + item.ExternalSalesID.ToString(), item.SalesChannel, item.SalesAgent, item.SoldDate, item.RateClass,
                                                    item.MeterNumber, item.UtilitySalesChannelName, item.ServiceReferenceNumber, item.SwitchDate, item.CreditCheck,
                                                    commercialCustomerType, item.CompanyName);
                        records.Add(record);
                    }

                }
            }
            catch (Exception ex)
            {
                SendErrorMessage(ex);
            }
            return records;
        }
        private static List<Record> GetListOfNewESIIDRecords(DateTime sDate, DateTime eDate)
        {
            //SELECT distinct uty.LdcCode, ut.UtilityTypeName, od.AccountNumber, pt.PremiseTypeName, od.CustomerNameKey, m.AuthorizationFirstName, m.AuthorizationLastName, od.ServiceAddress,
            //     od.ServiceCity,od.ServiceState, od.ServiceZip, od.ServiceCounty, m.Email, m.Btn, m.AccountFirstName,m.AccountLastName,od.BillingFirstName,od.BillingLastName,
            //     od.BillingAddress,od.BillingCity, od.BillingState, od.BillingZip,od.BillingCounty, m.Email, m.Btn,u.Language, p.ProgramName, p.Rate,p.Term, p.Msf, p.Etf,
            //     o.MarketerCode,od.OrderDetailId, sc.Name,u.AgentId,m.CallDateTime, od.RateClass, od.MeterNumber, usc.UtilitySalesChannelName, od.ServiceReferenceNumber,
            //     m.SwitchDate, p.CreditCheck, m.CompanyName
            //         FROM [Spark].[v1].[Main] m
            //         join [Spark].[v1].[OrderDetail] od on m.mainid = od.MainId
            //         join [Spark].[v1].[Program] p on od.ProgramId = p.ProgramId
            //         join [Spark].[v1].[UnitOfMeasure] uom on p.UnitOfMeasureId = uom.UnitOfMeasureId
            //         join [Spark].[v1].[UtilityType] ut on p.UtilityTypeId = ut.UtilityTypeId
            //         join [Spark].[v1].[Utility] uty on p.UtilityId = uty.UtilityId
            //         join [Spark].[v1].[AccountNumberType] ant on p.AccountNumberTypeId = ant.AccountNumberTypeId
            //         join [Spark].[v1].[User] u on m.UserId = u.UserId
            //         join [Spark].[v1].[Vendor] v on v.VendorId = u.VendorId
            //         join [Spark].[v1].[PremiseType] pt on p.PremiseTypeId = pt.PremiseTypeId
            //         join [Spark].[v1].[Office] o on u.[OfficeId] = o.[OfficeId]
            //         join [Spark].[v1].[SalesChannel] sc on o.[SalesChannelId] = sc.[SalesChannelId]
            //         join [Spark].[v1].[UtilitySalesChannel] usc on uty.UtilityId = usc.UtilityId
            //         JOIN [Spark].[v1].[Leads] l on l.LeadsId = m.LeadsId
            //         where m.CallDateTime > '6/1/2017' and m.CallDateTime < '7/1/2017'
            //         and m.Verified ='1'
            //         and m.SalesState ='TX'           	
            //         and p.BrandId = 1
            //         and usc.SalesChannelId = o.SalesChannelId
            //         and l.ESIID <> od.AccountNumber
            //         and (l.ESIID is not null or l.ESIID <>'')
            //         and (od.AccountNumber is not null and od.AccountNumber <>'')

            List<Record> records = new List<Record>();
            try
            {
                using (SparkEntities entitites = new SparkEntities())
                {
                    //string rateClass = string.Empty;
                    //string nameKey = string.Empty;

                    var query = (from m in entitites.Mains
                                 join od in entitites.OrderDetails on m.MainId equals od.MainId
                                 join p in entitites.Programs on od.ProgramId equals p.ProgramId
                                 join uom in entitites.UnitOfMeasures on p.UnitOfMeasureId equals uom.UnitOfMeasureId
                                 join ut in entitites.UtilityTypes on p.UtilityTypeId equals ut.UtilityTypeId
                                 join uty in entitites.Utilities on p.UtilityId equals uty.UtilityId
                                 join ant in entitites.AccountNumberTypes on p.AccountNumberTypeId equals ant.AccountNumberTypeId
                                 join u in entitites.Users on m.UserId equals u.UserId
                                 join v in entitites.Vendors on u.VendorId equals v.VendorId
                                 join pt in entitites.PremiseTypes on p.PremiseTypeId equals pt.PremiseTypeId
                                 join o in entitites.Offices on u.OfficeId equals o.OfficeId
                                 join sc in entitites.SalesChannels on o.SalesChannelId equals sc.SalesChannelId
                                 join usc in entitites.UtilitySalesChannels on uty.UtilityId equals usc.UtilityId
                                 join l in entitites.Leads on m.LeadsId equals l.LeadsId
                                 where m.CallDateTime > sDate && m.CallDateTime < eDate
                                 && m.Verified == "1"
                                 && m.SalesState == "TX"
                                 && p.BrandId == 1 //We only want Spark data Not Oasis or Censtar
                                 && usc.SalesChannelId == o.SalesChannelId
                                 && l.ESIID != od.AccountNumber
                                 && (l.ESIID != null && l.ESIID != "")
                                 && (od.AccountNumber != null && od.AccountNumber != "")
                                 select new
                                 {
                                     Utility = string.IsNullOrEmpty(uty.LdcCode) ? string.Empty : uty.LdcCode.ToUpper(),
                                     CommodityType = string.IsNullOrEmpty(ut.UtilityTypeName) ? string.Empty : ut.UtilityTypeName.ToUpper(),
                                     UtilityAccountNumber = od.AccountNumber.ToUpper(),
                                     PremiseTypeName = string.IsNullOrEmpty(pt.PremiseTypeName) ? string.Empty : pt.PremiseTypeName.ToUpper(),
                                     NameKey = string.IsNullOrEmpty(od.CustomerNameKey) ? string.Empty : od.CustomerNameKey.ToUpper(),
                                     AuthorizationFirstName = string.IsNullOrEmpty(m.AuthorizationFirstName) ? string.Empty : m.AuthorizationFirstName.ToUpper(),
                                     AuthorizationLastName = string.IsNullOrEmpty(m.AuthorizationLastName) ? string.Empty : m.AuthorizationLastName.ToUpper(),
                                     ServiceAddress1 = string.IsNullOrEmpty(od.ServiceAddress) ? string.Empty : od.ServiceAddress.ToUpper(),
                                     ServiceCity = string.IsNullOrEmpty(od.ServiceCity) ? string.Empty : od.ServiceCity.ToUpper(),
                                     ServiceState = string.IsNullOrEmpty(od.ServiceState) ? string.Empty : od.ServiceState.ToUpper(),
                                     ServiceZip = od.ServiceZip,
                                     ServiceCounty = string.IsNullOrEmpty(od.ServiceCounty) ? string.Empty : od.ServiceCounty.ToUpper(),
                                     ServiceEmail = string.IsNullOrEmpty(m.Email) ? string.Empty : m.Email.ToUpper(),
                                     ServicePhone = m.Btn,
                                     AccountFirstName = string.IsNullOrEmpty(m.AccountFirstName) ? string.Empty : m.AccountFirstName.ToUpper(),
                                     AccountLastName = string.IsNullOrEmpty(m.AccountLastName) ? string.Empty : m.AccountLastName.ToUpper(),
                                     BillingFirstName = string.IsNullOrEmpty(od.BillingFirstName) ? string.Empty : od.BillingFirstName.ToUpper(),
                                     BillingLastName = string.IsNullOrEmpty(od.BillingLastName) ? string.Empty : od.BillingLastName.ToUpper(),
                                     BillingAddress1 = string.IsNullOrEmpty(od.BillingAddress) ? string.Empty : od.BillingAddress.ToUpper(),
                                     BillingCity = string.IsNullOrEmpty(od.BillingCity) ? string.Empty : od.BillingCity.ToUpper(),
                                     BillingState = string.IsNullOrEmpty(od.BillingState) ? string.Empty : od.BillingState.ToUpper(),
                                     BillingZip = od.BillingZip,
                                     BillingCounty = string.IsNullOrEmpty(od.BillingCounty) ? string.Empty : od.BillingCounty.ToUpper(),
                                     BillingEmail = string.IsNullOrEmpty(m.Email) ? string.Empty : m.Email.ToUpper(),
                                     BillingPhone = m.Btn,
                                     //Language = string.IsNullOrEmpty(u.Language) ? string.Empty : u.Language.ToUpper(),
                                     ProductOffering = string.IsNullOrEmpty(p.ProgramName) ? string.Empty : p.ProgramName.ToUpper(),
                                     CommodityPrice = p.Rate,
                                     TermMonths = p.Term,
                                     MonthlyFee = p.Msf,
                                     ETF = p.Etf,
                                     Marketer = string.IsNullOrEmpty(o.MarketerCode) ? string.Empty : o.MarketerCode.ToUpper(),
                                     ExternalSalesID = od.OrderDetailId,
                                     SalesChannel = string.IsNullOrEmpty(sc.Name) ? string.Empty : sc.Name.ToUpper(),
                                     SalesAgent = string.IsNullOrEmpty(u.AgentId) ? string.Empty : u.AgentId.ToUpper(),
                                     SoldDate = m.CallDateTime,
                                     RateClass = od.RateClass,
                                     MeterNumber = od.MeterNumber,
                                     UtilitySalesChannelName = string.IsNullOrEmpty(usc.UtilitySalesChannelName) ? string.Empty : usc.UtilitySalesChannelName.ToUpper(),
                                     ServiceReferenceNumber = string.IsNullOrEmpty(od.ServiceReferenceNumber) ? string.Empty : od.ServiceReferenceNumber.ToUpper(),
                                     SwitchDate = string.IsNullOrEmpty(m.SwitchDate) ? string.Empty : m.SwitchDate.ToUpper(),
                                     CreditCheck = p.CreditCheck,
                                     CommercialCustomerType = string.IsNullOrEmpty(uty.CommercialCustomerType) ? string.Empty : uty.CommercialCustomerType.ToUpper(),
                                     CompanyName = string.IsNullOrEmpty(m.CompanyName) ? string.Empty : m.CompanyName.ToUpper(),
                                     Dnis = m.Dnis,
                                     LeadsId = m.LeadsId
                                 }).Distinct().ToList();

                    ////Get rateClass
                    //if (ldcCode == "BOSTED")
                    //{
                    //    rateClass = "R1";
                    //}
                    //else if (ldcCode == "BGE")
                    //{
                    //    if (utilityTypeName == "Electric")
                    //    {
                    //        rateClass = "R";
                    //    }
                    //    else
                    //    {
                    //        rateClass = "D";
                    //    }
                    //}
                    //else if (ldcCode == "CONED")
                    //{
                    //    rateClass = "";//empty on purpose
                    //}
                    //else if (ldcCode == "NIMO")
                    //{
                    //    rateClass = "Sc1";
                    //}
                    //else if (ldcCode == "PSEG")
                    //{
                    //    rateClass = "R";
                    //}

                    foreach (var item in query)
                    {


                        //Get nameKey
                        //nameKey = item.BillingLastName.Substring(0, 4);
                        string serviceFirstName = string.Empty;
                        string serviceLastName = string.Empty;
                        string billingFirstName = string.Empty;
                        string billingLastName = string.Empty;

                        string commercialCustomerType = item.CommercialCustomerType;

                        if (item.PremiseTypeName.ToLower() == "commercial")
                        {
                            if (item.Utility == "CLP"
                                || item.Utility == "UIC"
                                || item.Utility == "BGE"
                                || item.Utility == "PSEG"
                                || item.Utility == "COH"
                                || item.Utility == "DEO"
                                || item.Utility == "PECO"
                                || item.Utility == "PECO"
                                || item.Utility == "PPL"
                                || item.Utility == "CMS"
                                || item.Utility == "NIPSCO")
                            {
                                commercialCustomerType = item.CommercialCustomerType;// "RESIDENTIAL";
                            }
                            else
                            {
                                if (string.IsNullOrEmpty(commercialCustomerType))
                                {
                                    commercialCustomerType = "SMALL COMMERCIAL";
                                }
                            }

                            serviceFirstName = item.AuthorizationFirstName;
                            serviceLastName = item.AuthorizationLastName;
                            billingFirstName = item.AuthorizationFirstName;
                            billingLastName = item.AuthorizationLastName;
                        }
                        else
                        {
                            commercialCustomerType = item.PremiseTypeName;//Residential Reports get the PremiseTypeName from the db

                            serviceFirstName = item.BillingFirstName;
                            serviceLastName = item.BillingLastName;
                            billingFirstName = item.BillingFirstName;
                            billingLastName = item.BillingLastName;
                        }

                        string language = string.Empty;
                        if (!IsValueNull(item.Dnis))
                        {
                            switch (item.Dnis)
                            {
                                //English: 1324, 1322                        
                                case "1324":
                                case "1322":
                                    language = "ENGLISH";
                                    break;
                                //Spanish: 1325, 1323
                                case "1325":
                                case "1323":
                                    language = "SPANISH";
                                    break;
                            }
                        }
                        else
                        {
                            language = string.Empty;
                        }

                        //UtilityId LdcCode
                        //15 NSG
                        //16 PGL
                        string utilityAccountNumber = item.UtilityAccountNumber;
                        switch (item.Utility)
                        {
                            case "NSG":
                            case "PGL":
                                if (utilityAccountNumber.Length == 15)
                                    utilityAccountNumber = utilityAccountNumber.Insert(10, "-");

                                break;
                        }


                        Record record = new Record(item.Utility, item.CommodityType, utilityAccountNumber, item.PremiseTypeName,
                                                    item.NameKey, serviceFirstName, serviceLastName, item.ServiceAddress1, item.ServiceCity,
                                                    item.ServiceState, item.ServiceZip, item.ServiceCounty, item.ServiceEmail, item.ServicePhone, item.AccountFirstName,
                                                    item.AccountLastName, billingFirstName, billingLastName, item.BillingAddress1, item.BillingCity,
                                                    item.BillingState, item.BillingZip, item.BillingCounty, item.BillingEmail, item.BillingPhone, language,
                                                    item.ProductOffering, item.CommodityPrice, item.TermMonths, item.MonthlyFee, item.ETF, item.Marketer,
                                                    "CAL" + item.ExternalSalesID.ToString(), item.SalesChannel, item.SalesAgent, item.SoldDate, item.RateClass,
                                                    item.MeterNumber, item.UtilitySalesChannelName, item.ServiceReferenceNumber, item.SwitchDate, item.CreditCheck,
                                                    commercialCustomerType, item.CompanyName);
                        records.Add(record);
                    }

                }
            }
            catch (Exception ex)
            {
                SendErrorMessage(ex);
            }
            return records;
        }

        #endregion Methods to get RecordData (2 methods)

        #endregion Get Data

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
        ///  Saves XLS workbook document to a folder in the reportPath
        /// </summary>
        /// <param name="reportPath"></param>
        /// <param name="xlsFilename"></param>
        /// <param name="xlsFilePath"></param>
        /// <param name="exBook"></param>
        /// <param name="currentDate"></param>

        private static void SaveXlsDocument(ref string reportPath, ref string xlsFilename, ref string xlsFilePath, Excel.Workbook exBook, DateTime currentDate, string reportType)
        {
            //Two Report Types
            //YYYY_MM_DD_Spark_Calibrus_TX_UpdatedESIID – for enrollments where the ESIID does not match what was provided in the leads table.
            //YYYY_MM_DD_Spark_Calibrus_TX_NewESIID – for enrollments where there was no ESIID included in the leads table.
            xlsFilename = String.Format("{0:yyyy_MM_dd}", currentDate) + "_Spark_Calibrus_TX_" + reportType + ".xls";

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


        private static void FTPFile(ref string reportPath, ref string xlsFilename, ref string xlsFilePath, DateTime currentDate, string reportType, string HostName, string UserName, string Password)
        {

            //Two Report Types
            //YYYY_MM_DD_Spark_Calibrus_TX_UpdatedESIID – for enrollments where the ESIID does not match what was provided in the leads table.
            //YYYY_MM_DD_Spark_Calibrus_TX_NewESIID – for enrollments where there was no ESIID included in the leads table.
            xlsFilename = String.Format("{0:yyyy_MM_dd}", currentDate) + "_Spark_Calibrus_TX_" + reportType + ".xls";

            xlsFilePath = string.Format(reportPath + xlsFilename);
            try
            {
                Calibrus.Ftp.Upload ftp = new Calibrus.Ftp.Upload();
                ftp.Host = new Uri(string.Format("ftp://{0}/TXEnrollments/", HostName));
                ftp.UserName = UserName;
                ftp.Password = Password;
                ftp.UploadFile(xlsFilePath, xlsFilename);
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

        private static void SendErrorMessage(Exception ex)
        {
            Calibrus.ErrorHandler.Alerting alert = new Calibrus.ErrorHandler.Alerting("SparkBatchXLSReport");
            alert.SendAlert(ex.Source, ex.Message, Environment.MachineName, Environment.UserName, Environment.Version.ToString());
        }
        #endregion Utilities

    }
}
