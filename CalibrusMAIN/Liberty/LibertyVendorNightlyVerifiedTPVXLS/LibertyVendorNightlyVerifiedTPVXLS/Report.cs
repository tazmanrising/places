using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using Calibrus.ErrorHandler;
using Calibrus.ExcelFunctions;
using Calibrus.Mail;
using Excel = Microsoft.Office.Interop.Excel;

namespace LibertyVendorNightlyVerifiedTPVXLS
{
    public class Report
    {

        public static object na = System.Reflection.Missing.Value;

        #region Main

        public static void Main(string[] args)
        {
            string rootPath = string.Empty;

            //get report interval
            DateTime CurrentDate = new DateTime();
            DateTime StartDate = new DateTime();
            DateTime EndDate = new DateTime(); ;

            //start to  build the form pathing
            string xlsFilename = string.Empty;
            string xlsFilePath = string.Empty;

            if (args.Length > 0)
            {
                if (DateTime.TryParse(args[0], out CurrentDate))
                {
                    GetDates(out CurrentDate, out StartDate, out EndDate);
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
                GetDates(out CurrentDate, out StartDate, out EndDate);
            }

            //grab values from app.config
            rootPath = ConfigurationManager.AppSettings["rootPath"].ToString();

            //Get list of Offices
            List<Office> officeList = GetOfficeList();


            //Loop through Office 
            #region Office Loop

            foreach (Office office in officeList)
            {
                //Look for valid data based on a combination of Vendor, Utility, and Premise to see if we have data
                List<Record> recordList = GetListOfRecords(StartDate, EndDate, office.OfficeId);

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

                        string sheetName = "Completed TPVs Only";
                        exSheet.Name = sheetName.Length > 30 ? sheetName.Substring(0, 30) : sheetName; //force length of sheet name due to excel constraints
                        exSheet.Select(na);

                        //write out Report
                        WriteReport(ref exApp, ref exRange, StartDate, EndDate, recordList);

                        //save report
                        SaveXlsDocument(ref rootPath, ref xlsFilename, ref xlsFilePath, exBook, StartDate, office.OfficeName);

                        //Email report
                        SendEmail(ref xlsFilePath, StartDate, office);

                    }
                    catch (Exception ex)
                    {
                        SendErrorMessage(ex);
                    }
                    finally
                    {
                        exApp.DisplayAlerts = false;

                        //exBook.Close(); //moved this into the SaveXlsDocument method. 
                        exApp.Quit();
                    }


                }
            }

            #endregion VendorObject Loop

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

            //Error
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "Error";
            col++;

            //ContractNumber
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "ContractNumber";
            col++;

            //AccountNumber
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "AccountNumber";
            col++;

            //AccountName
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "AccountName";
            col++;

            //ServiceStreet
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "ServiceStreet";
            col++;

            //ServiceSuite
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "ServiceSuite";
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

            //ServiceZipPlus4
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "ServiceZipPlus4";
            col++;

            //BillingStreet
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "BillingStreet";
            col++;

            //BillingSuite
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "BillingSuite";
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

            //BillingZipPlus4
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "BillingZipPlus4";
            col++;

            //ContactFirstName
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "ContactFirstName";
            col++;

            //ContactLastName
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "ContactLastName";
            col++;

            //ContactPhoneNumber
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "ContactPhoneNumber";
            col++;

            //MeterNumber
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "MeterNumber";
            col++;

            //NameKey
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "NameKey";
            col++;

            //BillingAccount
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "BillingAccount";
            col++;

            //ServiceNumber
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "ServiceNumber";
            col++;

            //MDMA
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "MDMA";
            col++;

            //MSP
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "MSP";
            col++;

            //MeterInstaller
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "MeterInstaller";
            col++;

            //MeterReader
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "MeterReader";
            col++;

            //ScheduleCoordinator
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "ScheduleCoordinator";
            col++;

            //SalesChannel
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "SalesChannel";
            col++;

            //Market
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "Market";
            col++;

            //Utility
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "Utility";
            col++;

            //AccountType
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "AccountType";
            col++;

            //EffectiveStart
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "EffectiveStart";
            col++;

            //ContractDate
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "ContractDate";
            col++;

            //Term
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "Term";
            col++;

            //TransferRate
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "TransferRate";
            col++;

            //ContractRate
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "ContractRate";
            col++;

            //Rate2
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "Rate 2";
            col++;

            //Rate3
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "Rate 3";
            col++;

            //Rate4
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "Rate 4";
            col++;

            //ContractType
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "ContractType";
            col++;

            //SalesAgent
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "SalesAgent";
            col++;

            //IDNumber
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "IDNumber";
            col++;

            //IDType
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "IDType";
            col++;

            //ProductType
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "ProductType";
            col++;

            //ServiceClass
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "ServiceClass";
            col++;

            //Zone
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "Zone";
            col++;

            //Title
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "Title";
            col++;

            //TaxExempt
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "TaxExempt";
            col++;

            //ContractVersion
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "ContractVersion";
            col++;

            //Usage
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "Usage";
            col++;

            //Email
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "Email";
            col++;

            //GasAccountNumber
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "GasAccountNumber";
            col++;

            //GasUtility
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "GasUtility";
            col++;

            //GasRate
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "GasRate";
            col++;

            //GasMonthlyTerm
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "GasMonthlyTerm";
            col++;

            col = colInitialize;
            row++;

            #endregion Header

            #region Data

            foreach (Record record in listOfRecords)
            {
                //Error
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = ""; //blank
                col++;

                //ContractNumber
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = record.MainId;
                col++;

                //AccountNumber
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = record.AccountNumber;
                col++;

                //AccountName
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = record.AuthorizationFirstName + " " + record.AuthorizationLastName;
                col++;

                //ServiceStreet
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = record.ServiceAddress + " " + record.ServiceAddress2;
                col++;

                //ServiceSuite
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

                //ServiceZipPlus4
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, false, false, false);
                exRange.Value2 = "";
                col++;

                //BillingStreet
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, false, false, false);
                exRange.Value2 = record.BillingAddress + "  " + record.BillingAddress2;
                col++;

                //BillingSuite
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

                //BillingZipPlus4
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, false, false, false);
                exRange.Value2 = "";
                col++;

                //ContactFirstName
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, false, false, false);
                exRange.Value2 = record.AuthorizationFirstName;
                col++;

                //ContactLastName
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, false, false, false);
                exRange.Value2 = record.AuthorizationLastName;
                col++;

                //ContactPhoneNumber
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, false, false, false);
                exRange.Value2 = record.Btn;
                col++;

                //MeterNumber
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, false, false, false);
                exRange.Value2 = record.MeterNumber;
                col++;

                //NameKey
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, false, false, false);
                exRange.Value2 = record.NameKey;
                col++;

                //BillingAccount
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, false, false, false);
                exRange.Value2 = "";
                col++;

                //ServiceNumber
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, false, false, false);
                exRange.Value2 = record.ServiceNumber;
                col++;

                //MDMA
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, false, false, false);
                exRange.Value2 = ""; //blank
                col++;

                //MSP
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, false, false, false);
                exRange.Value2 = ""; //blank
                col++;

                //MeterInstaller
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, false, false, false);
                exRange.Value2 = ""; //blank
                col++;

                //MeterReader
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, false, false, false);
                exRange.Value2 = ""; //blank
                col++;

                //ScheduleCoordinator
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, false, false, false);
                exRange.Value2 = ""; //blank
                col++;

                //SalesChannel
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, false, false, false);
                exRange.Value2 = record.SalesChannelName;
                col++;

                //Market
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, false, false, false);
                exRange.Value2 = record.MarketState;
                col++;

                //Utility
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, false, false, false);
                exRange.Value2 = record.MarketUtility;
                col++;

                //AccountType
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, false, false, false);
                exRange.Value2 = record.Commercial == true ? "COMMERCIAL" : "RESIDENTIAL";
                col++;

                //EffectiveStart
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, false, false, false);
                exRange.Value2 = string.Format("{0:MM/dd/yyyy}", record.EffectiveStartDate);
                col++;

                //ContractDate
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, false, false, false);
                exRange.Value2 = "";
                col++;

                //Term
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, false, false, false);
                exRange.Value2 = record.MonthlyTerm;
                col++;

                //TransferRate
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, false, false, false);
                exRange.Value2 = "";
                col++;

                //ContractRate
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, false, false, false);
                exRange.Value2 = IsValueNull(record.Rate) ? record.Rate1 : record.Rate; //Need to use SubTermRate1 if Rate is null in main
                col++;

                //Rate2
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, false, false, false);
                exRange.Value2 = record.Rate2;
                col++;

                //Rate3
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, false, false, false);
                exRange.Value2 = record.Rate3;
                col++;

                //Rate4
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, false, false, false);
                exRange.Value2 = record.Rate4;
                col++;

                //ContractType
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, false, false, false);
                exRange.Value2 = "";
                col++;

                //SalesAgent
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, false, false, false);
                exRange.Value2 = record.SalesAgentId;
                col++;

                //IDNumber
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, false, false, false);
                exRange.Value2 = ""; //blank
                col++;

                //IDType
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, false, false, false);
                exRange.Value2 = ""; //blank
                col++;

                //ProductType
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, false, false, false);
                exRange.Value2 = ""; //blank
                col++;

                //ServiceClass
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, false, false, false);
                exRange.Value2 = ""; //blank
                col++;

                //Zone
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, false, false, false);
                exRange.Value2 = ""; //blank
                col++;

                //Title
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, false, false, false);
                exRange.Value2 = ""; //blank
                col++;

                //TaxExempt
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, false, false, false);
                exRange.Value2 = ""; //blank
                col++;

                //ContractVersion
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, false, false, false);
                exRange.Value2 = ""; //blank
                col++;

                //Usage
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, false, false, false);
                exRange.Value2 = ""; //blank
                col++;

                //Email
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, false, false, false);
                exRange.Value2 = record.Email;
                col++;

                //GasAccountNumber
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, false, false, false);
                exRange.Value2 = record.GasAccountNumber;
                col++;

                //GasUtility
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, false, false, false);
                exRange.Value2 = record.GasMarketUtility;
                col++;

                //GasRate
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, false, false, false);
                exRange.Value2 = record.GasRate;
                col++;

                //GasMonthlyTerm
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, false, false, false);
                exRange.Value2 = record.GasMonthlyTerm;
                col++;

                col = colInitialize;
                row++;
            }

            #endregion Data

            exRange = (Excel.Range)exApp.get_Range("A1", "DB1");
            exRange.EntireColumn.AutoFit();
        }

        #endregion Excel

        #region Get Data

        #region Method to build the loop for running the report (1 method)
        /// <summary>
        /// Gets a list of Offices  data for the primary loop
        /// </summary>
        /// <returns></returns>
        private static List<Office> GetOfficeList()
        {

            List<Office> office = new List<Office>();
            using (LibertyEntities entitites = new LibertyEntities())
            {
                var query = (from o in entitites.Offices
                             where o.IsActive == true
                             select o).ToList();

                office = query;
            }

            return office;
        }
        #endregion Method to build the loop for running the report (1 method)


        #region Method to Get RecordData (1 method)
        private static List<Record> GetListOfRecords(DateTime sDate, DateTime eDate, int officeId)
        {
            //Select *
            //FROM [Liberty].[v1].[Main] as m
            //JOIN [Liberty].[v1].[OrderDetail] as od on m.MainId = od.MainId 
            //JOIN [Liberty].[v1].[SalesChannel] as sc on sc.SalesChannelId = m.SalesChannelId
            //left outer join [Liberty].[v1].[ContractTerm] ct on ct.ContractTermId=m.ContractTermId
            //left outer join [Liberty].[v1].[ContractTerm] ctg on ctg.ContractTermId=m.GasContractTermId
            //JOIN [Liberty].[v1].[MarketProduct] as mp on mp.MarketProductId = m.MarketProductId
            //JOIN [Liberty].[v1].[MarketState] as ms on ms.MarketStateId = m.MarketStateId
            //left outer join [Liberty].[v1].[MarketUtility] mu on mu.MarketUtilityId = m.MarketUtilityId
            //left outer join [Liberty].[v1].[MarketUtility] mg on mg.MarketUtilityId = m.GasMarketUtilityId
            //JOIN [Liberty].[v1].[User] as u on u.UserId = m.UserId
            //JOIN [Liberty].[v1].[Office] as o on u.OfficeId = o.OfficeId

            //Where m.CallDateTime > '01/01/2015' 
            //AND m.CallDateTime < '01/02/2015'
            //AND m.Verified ='1'
            //AND o.OfficeId='44'


            List<Record> records = new List<Record>();
            try
            {
                using (LibertyEntities entitites = new LibertyEntities())
                {
                    //string rateClass = string.Empty;
                    //string nameKey = string.Empty;

                    var query = (from m in entitites.Mains
                                 join od in entitites.OrderDetails on m.MainId equals od.MainId
                                 join sc in entitites.SalesChannels on m.SalesChannelId equals sc.SalesChannelId
                                 //join ct in entitites.ContractTerms on m.ContractTermId equals ct.ContractTermId
                                 join mp in entitites.MarketProducts on m.MarketProductId equals mp.MarketProductId
                                 join ms in entitites.MarketStates on m.MarketStateId equals ms.MarketStateId
                                 //join mu in entitites.MarketUtilities on m.MarketUtilityId equals mu.MarketUtilityId                                 
                                 join u in entitites.Users on m.UserId equals u.UserId
                                 join o in entitites.Offices on u.OfficeId equals o.OfficeId
                                 where m.CallDateTime > sDate && m.CallDateTime < eDate
                                 && m.Verified == "1"
                                 && o.OfficeId == officeId

                                 select new
                                 {
                                     MainId = m.MainId,
                                     AccountNumber = od.AccountNumber,
                                     AuthorizationFirstName = m.AuthorizationFirstName,
                                     AuthorizationLastName = m.AuthorizationLastName,
                                     ServiceAddress = od.ServiceAddress,
                                     ServiceAddress2 = od.ServiceAddress2,
                                     ServiceCity = od.ServiceCity,
                                     ServiceState = od.ServiceState,
                                     ServiceZip = od.ServiceZip,
                                     BillingAddress = od.BillingAddress,
                                     BillingAddress2 = od.BillingAddress2,
                                     BillingCity = od.BillingCity,
                                     BillingState = od.BillingState,
                                     BillingZip = od.BillingZip,
                                     Btn = m.Btn,
                                     MeterNumber = od.MeterNumber,
                                     NameKey = od.NameKey,
                                     SalesChannelName = o.OfficeName,
                                     MarketState = ms.State,
                                     MarketUtilityId = m.MarketUtilityId,
                                     Commercial = mp.Commercial,
                                     EffectiveStartDate = m.RateEffectiveDate,
                                     ContractTermId = m.ContractTermId,
                                     Rate = m.Rate,
                                     SalesAgentId = m.SalesAgentId,
                                     Email = m.Email,
                                     ServiceNumber = od.ServiceNumber,
                                     Rate1 = od.SubTermRate1,
                                     Rate2 = od.SubTermRate2,
                                     Rate3 = od.SubTermRate3,
                                     Rate4 = od.SubTermRate4,
                                     GasAccountNumber = od.GasAccountNumber,
                                     GasMarketUtilityId = m.GasMarketUtilityId,
                                     GasRate = m.GasRate,
                                     GasContractTermId = m.GasContractTermId
                                 }).ToList();



                    foreach (var item in query)
                    {
                        string MarketUtility = string.Empty;
                        string MonthlyTerm = string.Empty;
                        string GasMarketUtility = string.Empty;
                        string GasMonthlyTerm = string.Empty;

                        if (item.MarketUtilityId.HasValue)
                        {
                            MarketUtility = (from mu in entitites.MarketUtilities
                                             where mu.MarketUtilityId == item.MarketUtilityId
                                             select mu.Utility).FirstOrDefault();
                        }
                        if (item.ContractTermId.HasValue)
                        {
                            MonthlyTerm = (from ct in entitites.ContractTerms
                                           where ct.ContractTermId == item.ContractTermId
                                           select ct.MonthlyTerm).FirstOrDefault();
                        }
                        if (item.GasMarketUtilityId.HasValue)
                        {
                            GasMarketUtility = (from mu in entitites.MarketUtilities
                                                where mu.MarketUtilityId == item.GasMarketUtilityId
                                                select mu.Utility).FirstOrDefault();
                        }
                        if (item.GasContractTermId.HasValue)
                        {
                            GasMonthlyTerm = (from ct in entitites.ContractTerms
                                              where ct.ContractTermId == item.GasContractTermId
                                              select ct.MonthlyTerm).FirstOrDefault();
                        }
                        Record record = new Record(item.MainId, item.AccountNumber, item.AuthorizationFirstName, item.AuthorizationLastName,
                                                    item.ServiceAddress, item.ServiceAddress2, item.ServiceCity, item.ServiceState, item.ServiceZip,
                                                    item.BillingAddress, item.BillingAddress2, item.BillingCity, item.BillingState, item.BillingZip, item.Btn,
                                                    item.MeterNumber, item.NameKey, item.SalesChannelName, item.MarketState, MarketUtility, item.Commercial,
                                                    item.EffectiveStartDate, MonthlyTerm, item.Rate, item.SalesAgentId, item.Email, item.ServiceNumber,
                                                    item.Rate1, item.Rate2, item.Rate3, item.Rate4, item.GasAccountNumber, GasMarketUtility, item.GasRate, GasMonthlyTerm);
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

        #endregion Method to Get RecordData (1 method)

        #region Method to Get Distrobution for a specific Office
        private static string getDistro(int officeId)
        {
            string distro = string.Empty;
            using (LibertyEntities entitites = new LibertyEntities())
            {
                var query = (from nrd in entitites.NightlyReportDistroes
                             where nrd.OfficeId == officeId
                             select nrd.DistroList).FirstOrDefault();

                distro = query;
            }

            return distro;
        }
        #endregion Method to Get Distrobution for a specific Office

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
        /// <param name="vendorName"></param>
        private static void SaveXlsDocument(ref string reportPath, ref string xlsFilename, ref string xlsFilePath, Excel.Workbook exBook, DateTime currentDate, string vendorName)
        {


            xlsFilename = String.Format("{0:yyyy_MM_dd}", currentDate) + "Vendor Nightly Report-Verified TPVS Only" + "_" + vendorName + "_CIS" + ".xlsx";

            xlsFilePath = string.Format(reportPath + xlsFilename);
            bool fileExists = File.Exists(xlsFilePath);
            if (fileExists)
            {
                //delete it
                File.Delete(xlsFilePath);
            }
            //save workbook
            exBook.SaveAs(Filename: xlsFilePath);
            exBook.Close();
        }

        private static void SendEmail(ref string xlsFilePath, DateTime reportDate, Office office)
        {
            string strToEmail = string.Empty;
            //string strMsgBody = string.Empty;
            try
            {

                strToEmail = getDistro(office.OfficeId);
                if (!string.IsNullOrEmpty(strToEmail))
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

                    mail.Subject = "Liberty Nightly Verified Report for Office: " + office.OfficeName + "  " + reportDate.ToString("MMM") + " " + reportDate.ToString("dd") + " " + reportDate.ToString("yyyy") + ".";


                    //mail.Body = strMsgBody;
                    mail.SendMessage();
                }

            }
            catch (Exception ex)
            {
                SendErrorMessage(ex, office.OfficeId.ToString(), office.OfficeName);
            }
        }

        //private static void CopyFileAndMove(string getfile, string putfile, ref string xlsFilename)
        //{
        //    getfile += xlsFilename;
        //    putfile += xlsFilename;
        //    try
        //    {
        //        bool fileExists = File.Exists(putfile);
        //        if (fileExists)
        //        {
        //            //delete it
        //            File.Delete(putfile);
        //        }
        //        //move the file to the processed directory
        //        File.Copy(String.Format(@"{0}", getfile), String.Format(@"{0}", putfile));
        //    }
        //    catch (Exception ex)
        //    { throw ex; }
        //}


        private static void GetDates(out DateTime CurrentDate, out DateTime StartDate, out DateTime EndDate)
        {
            DateTime baseDate;
            DateTimeService.ReportingDateTimeService dts = null;
            try
            {
                dts = new DateTimeService.ReportingDateTimeService();
                baseDate = DateTime.Parse(dts.GetDateTime());
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

            baseDate = new DateTime(baseDate.Year, baseDate.Month, baseDate.Day, baseDate.Hour, baseDate.Minute, baseDate.Second);//current date time

            CurrentDate = new DateTime(baseDate.Year, baseDate.Month, baseDate.Day, baseDate.Hour, baseDate.Minute, baseDate.Second); //Date for when File runs
            StartDate = new DateTime(baseDate.Year, baseDate.Month, baseDate.Day, 0, 0, 0).AddDays(-1); //previous day  This will be the Start date for the day it runs
            EndDate = new DateTime(baseDate.Year, baseDate.Month, baseDate.Day, 0, 0, 0); //current day this will be the End date
        }

        private static void SendErrorMessage(Exception ex)
        {
            Calibrus.ErrorHandler.Alerting alert = new Calibrus.ErrorHandler.Alerting("LibertyVendorNightlyVerifiedTPVXLS");
            alert.SendAlert(ex.Source, ex.Message, Environment.MachineName, Environment.UserName, Environment.Version.ToString());
        }
        private static void SendErrorMessage(Exception ex, string officeId, string officeName)
        {
            Calibrus.ErrorHandler.Alerting alert = new Calibrus.ErrorHandler.Alerting("LibertyVendorNightlyVerifiedTPVXLS");
            alert.SendAlert(ex.Source, "No Email for OfficeId:" + officeId + " OfficeName:" + officeName + "- " + ex.Message, Environment.MachineName, Environment.UserName, Environment.Version.ToString());
        }
        #endregion Utilities
    }
}
