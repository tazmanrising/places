using Calibrus.ExcelFunctions;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using Excel = Microsoft.Office.Interop.Excel;

namespace SparkOverallXLSReport
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

           
            //Look for valid data based on a combination of Vendor, Utility, and Premise to see if we have data
            List<Record> recordList = GetListOfRecords(StartDate, EndDate);

            //COMMENTED OUT, THEY WANT THE REPORT EVEN IF THERE IS NO DATA.
            //if we have records then we can build the report
            //if (recordList.Count() > 0)
            //{
                //Calibrus_TPVDetail_08_17_2015.xlsx
                xlsFilename = "Calibrus_TPVDetail_" + String.Format("{0:MM_dd_yyyy}", StartDate) + ".xlsx";


                //start Excel
                Excel.Application exApp = new Excel.Application();
                Excel.Workbook exBook = null;
                Excel.Worksheet exSheet = null;
                Excel.Range exRange = null;

                int sheetsAdded = 0;
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

                    //new tab
                  
                    exSheet.Select(na);
                    //write out Report
                    WriteReport(ref exApp, ref exRange, StartDate, EndDate, recordList);

                 

                    //save report                    
                    SaveXlsDocument(ref rootPath, xlsFilename, ref xlsFilePath, exBook, StartDate);
                             
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
                FTPFile(ref rootPath,  xlsFilename, ref xlsFilePath, StartDate, hostName, userName, password);                            
            //}


        }

        #endregion

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


            //ActionType
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "ActionType";
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
            
            //RolloverProduct
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "RolloverProduct";
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

            //TPVverificationid
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "TPVverificationid";
            col++;
            

            col = colInitialize;
            row++;

            #endregion Header

            #region Data

            foreach (Record record in listOfRecords)
            {                

                //ActionType
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = "Enrollment";
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
                exRange.Value2 = "";
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
                exRange.Value2 = record.CustomerType;
                col++;

                //CompanyName
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = "";
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
                if (record.LDCCode.ToLower() == "bosted")
                {
                    taxExempt = "Y";
                }
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = taxExempt;
                col++;

                //TaxExempt%
                string taxExemptPercent = string.Empty;
                if (record.LDCCode.ToLower() == "bosted")
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

                //RolloverProduct
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

                //TPVverificationid

                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.NumberFormat = "@";
                exRange.Value2 = record.TPVverificationid;
                col++;

                col = colInitialize;
                row++;
            }

            #endregion Data

            exRange = (Excel.Range)exApp.get_Range("A1", "BJ1");
            exRange.EntireColumn.AutoFit();
        }

        #endregion Excel

        #region Get Data


        #region Method to Get RecordData (1 method)

        private static List<Record> GetListOfRecords(DateTime sDate, DateTime eDate)
        {
            //SELECT distinct uty.LdcCode,ut.UtilityTypeName,od.AccountNumber,pt.PremiseTypeName,od.CustomerNameKey,m.AuthorizationFirstName,m.AuthorizationLastName,od.ServiceAddress,
            //od.ServiceCity,od.ServiceState,od.ServiceZip,od.ServiceCounty,m.Email,m.Btn,m.AccountFirstName,m.AccountLastName,od.BillingAddress,od.BillingCity,
            //od.BillingState,od.BillingZip,od.BillingCounty,m.Email,m.Btn,u.Language,p.ProgramName,p.Rate,p.Term,p.Msf,p.Etf,v.MarketerCode,od.OrderDetailId,
            //sc.Name,u.AgentId,m.CallDateTime,od.RateClass,od.MeterNumber,m.MainId,uty.LdcCode,usc.UtilitySalesChannelName
            //FROM [Spark].[v1].[Main] m
            //join [Spark].[v1].[OrderDetail] od on m.mainid = od.MainId
            //join [Spark].[v1].[Program] p on od.ProgramId = p.ProgramId
            //join [Spark].[v1].[UnitOfMeasure] uom on p.UnitOfMeasureId = uom.UnitOfMeasureId
            //join [Spark].[v1].[UtilityType] ut on p.UtilityTypeId = ut.UtilityTypeId
            //join [Spark].[v1].[Utility] uty on p.UtilityId = uty.UtilityId
            //join [Spark].[v1].[AccountNumberType] ant on p.AccountNumberTypeId = ant.AccountNumberTypeId
            //join [Spark].[v1].[User] u on m.UserId = u.UserId
            //join [Spark].[v1].[Vendor] v on v.VendorId = u.VendorId
            //join [Spark].[v1].[PremiseType] pt on p.PremiseTypeId = pt.PremiseTypeId
            //JOIN [Spark].[v1].[Office] o on v.[VendorId] = o.[VendorId]
            //JOIN [Spark].[v1].[SalesChannel] sc on o.[SalesChannelId] = sc.[SalesChannelId]
            //--join [Spark].[v1].[SalesChannel] sc on v.SalesChannelId = sc.SalesChannelId
            //join [Spark].[v1].[UtilitySalesChannel] usc on uty.UtilityId = usc.UtilityId
            //where m.CallDateTime > '9/29/2015' and m.CallDateTime < '9/30/2015'
            //and m.Verified ='1'
            //and usc.SalesChannelId = o.SalesChannelId
            //and usc.UtilityId = uty.UtilityId


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
                                 join o in entitites.Offices on v.VendorId equals o.VendorId
                                 join sc in entitites.SalesChannels on o.SalesChannelId equals sc.SalesChannelId
                                 join usc in entitites.UtilitySalesChannels on uty.UtilityId equals usc.UtilityId
                                 where m.CallDateTime > sDate && m.CallDateTime < eDate
                                 && usc.SalesChannelId == v.SalesChannelId
                                 && usc.UtilityId == uty.UtilityId
                                 && m.Verified == "1"
                                 select new
                                 {
                                     Utility = uty.LdcCode,
                                     CommodityType = ut.UtilityTypeName,
                                     UtilityAccountNumber = od.AccountNumber,
                                     CustomerType = pt.PremiseTypeName,
                                     NameKey = od.CustomerNameKey,
                                     ServiceFirstName = m.AuthorizationFirstName,
                                     ServiceLastName = m.AuthorizationLastName,
                                     ServiceAddress1 = od.ServiceAddress,
                                     ServiceCity = od.ServiceCity,
                                     ServiceState = od.ServiceState,
                                     ServiceZip = od.ServiceZip,
                                     ServiceCounty = od.ServiceCounty,
                                     ServiceEmail = m.Email,
                                     ServicePhone = m.Btn,
                                     BillingFirstName = m.AccountFirstName,
                                     BillingLastName = m.AccountLastName,
                                     BillingAddress1 = od.BillingAddress,
                                     BillingCity = od.BillingCity,
                                     BillingState = od.BillingState,
                                     BillingZip = od.BillingZip,
                                     BillingCounty = od.BillingCounty,
                                     BillingEmail = m.Email,
                                     BillingPhone = m.Btn,
                                     Language = u.Language,
                                     ProductOffering = p.ProgramName,
                                     CommodityPrice = p.Rate,
                                     TermMonths = p.Term,
                                     MonthlyFee = p.Msf,
                                     ETF = p.Etf,
                                     Marketer = v.MarketerCode,
                                     ExternalSalesID = od.OrderDetailId,
                                     SalesChannel = sc.Name,
                                     SalesAgent = u.AgentId,
                                     SoldDate = m.CallDateTime,
                                     RateClass = od.RateClass,
                                     MeterNumber = od.MeterNumber,
                                     TPVverificationid = m.MainId,
                                     LDCCode = uty.LdcCode,
                                     UtilitySalesChannelName = usc.UtilitySalesChannelName
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

                        Record record = new Record(item.Utility, item.CommodityType, item.UtilityAccountNumber, item.CustomerType,
                                                    item.NameKey, item.ServiceFirstName, item.ServiceLastName, item.ServiceAddress1, item.ServiceCity,
                                                    item.ServiceState, item.ServiceZip, item.ServiceCounty, item.ServiceEmail, item.ServicePhone, item.BillingFirstName,
                                                    item.BillingLastName, item.BillingAddress1, item.BillingCity, item.BillingState, item.BillingZip, item.BillingCounty,
                                                    item.BillingEmail, item.BillingPhone, item.Language, item.ProductOffering, item.CommodityPrice,
                                                    item.TermMonths, item.MonthlyFee, item.ETF, item.Marketer, item.ExternalSalesID.ToString(), item.SalesChannel, item.SalesAgent,
                                                    item.SoldDate, item.RateClass, item.MeterNumber, item.TPVverificationid, item.LDCCode, item.UtilitySalesChannelName);
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
        ///  Saves XLS workbook document to a folder in the reportPath
        /// </summary>
        /// <param name="reportPath"></param>
        /// <param name="xlsFilename"></param>
        /// <param name="xlsFilePath"></param>
        /// <param name="exBook"></param>
        /// <param name="currentDate"></param>
        /// <param name="customerType"></param>
        /// <param name="marketCommodity"></param>
        /// <param name="salesChannel"></param>
        /// <param name="vendorName"></param>
        private static void SaveXlsDocument(ref string reportPath, string xlsFilename, ref string xlsFilePath, Excel.Workbook exBook, DateTime currentDate)
        {
            
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

        //private static void SendEmail(ref string xlsFilePath, DateTime currentDate, string vendor)
        //{
        //    //string strMsgBody = string.Empty;
        //    try
        //    {
        //        string strToEmail = ConfigurationManager.AppSettings["mailRecipientTO_" + vendor].ToString();

        //        //StringBuilder sb = new StringBuilder();

        //        //sb.AppendLine("");
        //        //strMsgBody = sb.ToString();

        //        SmtpMail mail = new SmtpMail("TMPWEB1", false);

        //        mail.AddAttachment(xlsFilePath);//Attach XLS report
        //        mail.AddRecipient(strToEmail, RecipientType.To);

        //        mail.From = "reports1@calibrus.com";

        //        mail.Subject = "Spark Batch Report for " + vendor + " " + currentDate.ToString("dddd, dd MMMM yyyy") + ".";

        //        //mail.Body = strMsgBody;
        //        mail.SendMessage();

        //    }
        //    catch (Exception ex)
        //    {
        //        SendErrorMessage(ex);
        //    }

        //}
        private static void FTPFile(ref string reportPath, string xlsFilename, ref string xlsFilePath, DateTime currentDate, string HostName, string UserName, string Password)
        {              

            xlsFilePath = string.Format(reportPath + xlsFilename);
            try
            {
                Calibrus.Ftp.Upload ftp = new Calibrus.Ftp.Upload();
                ftp.Host = new Uri(string.Format("ftp://{0}/", HostName));
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
