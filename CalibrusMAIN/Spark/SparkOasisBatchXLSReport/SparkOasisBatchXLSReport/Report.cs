using Calibrus.ExcelFunctions;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using Excel = Microsoft.Office.Interop.Excel;
using Renci.SshNet;

namespace SparkOasisBatchXLSReport
{
    public class Report
    {
        public static object na = System.Reflection.Missing.Value;


        #region Main
        public static void Main(string[] args)
        {
            string rootPath = string.Empty; //File we create this in this program and its location
            string hostName = string.Empty; //Server where we send the file
            string userName = string.Empty; //user account
            string password = string.Empty; //password
            string toDir = string.Empty; //Directory path to build for the file to put on the server


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
            toDir = ConfigurationManager.AppSettings["toDir"].ToString();


            //Get List of SalesChannel
            List<SalesChannelObject> salesChannelList = new List<SalesChannelObject>();
            SalesChannelObject saleschannelObj = new SalesChannelObject(1, "DTD");
            salesChannelList.Add(saleschannelObj);
            saleschannelObj = saleschannelObj = new SalesChannelObject(2, "TeleSales");
            salesChannelList.Add(saleschannelObj);

            //Get list of States from Program table for Oasis brand=2
            List<ProgramObject> programList = GetStateList();

            //Loop through SalesChannel then States (2 nested loops)
            #region SalesChannel Loop
            foreach (SalesChannelObject saleschannel in salesChannelList)
            {

                #region StateObject Loop
                foreach (ProgramObject program in programList)
                {
                    //Look for valid data based on a combination of Vendor, Utility, and Premise to see if we have data
                    //List<Record> recordList = GetListOfRecords(StartDate, EndDate, vendor.SalesChannelName, vendor.VendorName, program.State);
                    List<Record> recordList = GetListOfRecords(StartDate, EndDate, saleschannel.SalesChannelName, program.State);

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
                            //string vendorSalesChannelAbbrev = string.Empty;
                            //if (vendor.SalesChannelName.ToLower() == "telesales")
                            //{ vendorSalesChannelAbbrev = "tm"; }
                            //else
                            //{ vendorSalesChannelAbbrev = vendor.SalesChannelName.ToLower(); }
                            //string sheetName = String.Format("{0:yyyy_MM_dd}", StartDate) + "_cal_" + premise.PremiseTypeName.Substring(0, 3) + "_" + utility.LdcCode.ToLower() + utilitytype.UtilityTypeName.Substring(0, 1).ToLower() + "_" + vendorSalesChannelAbbrev + "_CIS";
                            //exSheet.Name = sheetName.Length > 30 ? sheetName.Substring(0, 30) : sheetName; //force length of sheet name due to excel constraints
                            exSheet.Select(na);

                            //write out Report
                            WriteReport(ref exApp, ref exRange, StartDate, EndDate, recordList);

                            //save report                                                        
                            SaveXlsDocument(ref rootPath, ref xlsFilename, ref xlsFilePath, exBook, StartDate, saleschannel.SalesChannelName, program.State);

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
                        FTPFile(ref rootPath, ref xlsFilename, ref xlsFilePath, StartDate, saleschannel.SalesChannelName, program.State, hostName, toDir, userName, password);
                    }
                }
                #endregion StateObject Loop
            }
            #endregion SalesChanel Loop
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

            //Revenue_Class_Desc
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "Revenue_Class_Desc";
            col++;

            //First_Name
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "First_Name";
            col++;

            //Last_Name
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "Last_Name";
            col++;

            //Customer_Name
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "Customer_Name";
            col++;

            //Home_Phone_Num
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "Home_Phone_Num";
            col++;

            //Work_Phone_Num
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "Work_Phone_Num";
            col++;

            //Social_Sec_Code
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "Social_Sec_Code";
            col++;

            //Fed_Tax_Id_Num
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "Fed_Tax_Id_Num";
            col++;

            //Cellular_Num
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "Cellular_Num";
            col++;

            //Email_Address
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "Email_Address";
            col++;

            //Language_Pref_Code
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "Language_Pref_Code";
            col++;

            //Credit_Score_Num
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "Credit_Score_Num";
            col++;

            //Contact_Name
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "Contact_Name";
            col++;

            //SLine1_Addr
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "SLine1_Addr";
            col++;

            //SLine2_Addr
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "SLine2_Addr";
            col++;

            //SCity_Name
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "SCity_Name";
            col++;

            //SCity_Name
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "SCity_Name";
            col++;

            //SPostal_Code
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "SPostal_Code";
            col++;

            //Marketer_Name
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "Marketer_Name";
            col++;

            //Distributor_Name
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "Distributor_Name";
            col++;

            //Service_Type_Desc
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "Service_Type_Desc";
            col++;

            //Bill_Method
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "Bill_Method";
            col++;

            //LDC_Account_Num
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "LDC_Account_Num";
            col++;

            //Enroll_Type_Desc
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "Enroll_Type_Desc";
            col++;

            //Requested_Start_Date
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "Requested_Start_Date";
            col++;

            //Special_Meter_Read_Date
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "Special_Meter_Read_Date";
            col++;

            //Waive_Notification_Ind
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "Waive_Notification_Ind";
            col++;

            //Tax_Exemption_Ind
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "Tax_Exemption_Ind";
            col++;

            //Plan_Desc
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "Plan_Desc";
            col++;

            //Contract_Start_Date
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "Contract_Start_Date";
            col++;

            //Contract_End_Date
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "Contract_End_Date";
            col++;

            //Fixed_Commodity_Amt
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "Fixed_Commodity_Amt";
            col++;

            //Agent_Code
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "Agent_Code";
            col++;

            //Commission_Plan
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "Commission_Plan";
            col++;

            //Commission_Start_Date
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "Commission_Start_Date";
            col++;

            //Commission_End_Date
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "Commission_End_Date";
            col++;

            //Commission_Unit_Num
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "Commission_Unit_Num";
            col++;

            //Promotion_Code
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "Promotion_Code";
            col++;

            //Ad_Source_Desc
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "Ad_Source_Desc";
            col++;

            //MLine1_Addr
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "MLine1_Addr";
            col++;

            //MLine2_Addr
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "MLine2_Addr";
            col++;

            //MLine3_Addr
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "MLine3_Addr";
            col++;

            //MLine4_Addr
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "MLine4_Addr";
            col++;

            //MCity_Name
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "MCity_Name";
            col++;

            //MState
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "MState";
            col++;

            //Mcountry_Name
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "Mcountry_Name";
            col++;

            //MPostal_Code
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "MPostal_Code";
            col++;

            //Employee_Ind
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "Employee_Ind";
            col++;

            //Low_Income_Ind
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "Low_Income_Ind";
            col++;

            //Life_Support_Ind
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "Life_Support_Ind";
            col++;

            //Interruptible_Ind
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "Interruptible_Ind";
            col++;

            //Approx_Annual_Usage
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "Approx_Annual_Usage";
            col++;

            //Budget_Amt
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "Budget_Amt";
            col++;

            //Deposit_Installment_Amt
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "Deposit_Installment_Amt";
            col++;

            //Deposit_Installment_Qty
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "Deposit_Installment_Qty";
            col++;

            //Security_Question
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "Security_Question";
            col++;

            //Security_Answer
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "Security_Answer";
            col++;

            //Employer_Name
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "Employer_Name";
            col++;

            //Drivers_Lic_Num
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "Drivers_Lic_Num";
            col++;

            //Drivers_Lic_State_Code
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "Drivers_Lic_State_Code";
            col++;

            //Verification_Type_Desc
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "Verification_Type_Desc";
            col++;

            //Confirmation_Code
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "Confirmation_Code";
            col++;

            //Commission_Master_Unit_Num
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "Commission_Master_Unit_Num";
            col++;

            //Master_Code
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "Master_Code";
            col++;

            //Index_Adder_Num
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "Index_Adder_Num";
            col++;

            //Billing_Pkg_Name
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "Billing_Pkg_Name";
            col++;

            //Account_Name
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "Account_Name";
            col++;

            //ExportFileName
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "ExportFileName";
            col++;

            //ExportDate
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "ExportDate";
            col++;

            //Commission_Sub_Agent_Unit_Num
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "Commission_Sub_Agent_Unit_Num";
            col++;

            //Sub_Agent_Code
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "Sub_Agent_Code";
            col++;

            //Supply_Zone_Desc
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "Supply_Zone_Desc";
            col++;

            //Fax_Num
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "Fax_Num";
            col++;

            //Equipment_Id_Code
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "Equipment_Id_Code";
            col++;

            //Rto_Amt
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "Rto_Amt";
            col++;

            //Payment_Type_Desc
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "Payment_Type_Desc";
            col++;

            //Payment_Subscriber_Id_Code
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "Payment_Subscriber_Id_Code";
            col++;

            //Fixed_Charge_Amt
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "Fixed_Charge_Amt";
            col++;

            //Legacy_Account_Num
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "Legacy_Account_Num";
            col++;

            //Legacy_Id
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "Legacy_Id";
            col++;

            //Birth_Date
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "Birth_Date";
            col++;

            //Enrollment_Source_Code
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "Enrollment_Source_Code";
            col++;

            //Esignature_Code
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "Esignature_Code";
            col++;

            //Commission_2_Plan
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "Commission_2_Plan";
            col++;

            //Commission_2_Start_Date
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "Commission_2_Start_Date";
            col++;

            //Commission_2_End_Date
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "Commission_2_End_Date";
            col++;

            //Commission_2_Agent_Unit_Num
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "Commission_2_Agent_Unit_Num";
            col++;

            //Commission_2_Master_Unit_Num
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "Commission_2_Master_Unit_Num";
            col++;

            //Commission_2_Sub_Unit_Num
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "Commission_2_Sub_Unit_Num";
            col++;

            //Deposit_Suggested_Amt
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "Deposit_Suggested_Amt";
            col++;

            //Group_Desc
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "Group_Desc";
            col++;

            //Landlord_Agreement_Id_Desc
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "Landlord_Agreement_Id_Desc";
            col++;

            //Attention_Name
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "Attention_Name";
            col++;

            //Service_Priority_Desc
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "Service_Priority_Desc";
            col++;

            //Delivery_Method_Desc
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "Delivery_Method_Desc";
            col++;

            //Heat_Rate_Num
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "Heat_Rate_Num";
            col++;

            //Rep_Adder_Num
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "Rep_Adder_Num";
            col++;

            //Work_Ext_Num
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "Work_Ext_Num";
            col++;

            //Credit_Rating_Source_Desc
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "Credit_Rating_Source_Desc";
            col++;

            //Min_Index_Num
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "Min_Index_Num";
            col++;

            //Max_Index_Num
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "Max_Index_Num";
            col++;

            //Delinquent_Days_Cnt
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "Delinquent_Days_Cnt";
            col++;

            //Request_Hist_Usage_Ind
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "Request_Hist_Usage_Ind";
            col++;

            //Request_Hist_Interval_Ind
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "Request_Hist_Interval_Ind";
            col++;

            //Interval_Ind
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "Interval_Ind";
            col++;

            //Interval_Non_Edi_Ind
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, false, false, false);
            exRange.Value2 = "Interval_Non_Edi_Ind";
            col++;

            //Master_Ind
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "Master_Ind";
            col++;

            //Type_Of_Service_Desc
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "Type_Of_Service_Desc";
            col++;

            //Contact_2_Name
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "Contact_2_Name";
            col++;

            //Doing_Business_As_Name
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "Doing_Business_As_Name";
            col++;

            //Web_Site_Addr
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "Web_Site_Addr";
            col++;

            //Remit_Duns_Num
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "Remit_Duns_Num";
            col++;

            //Contact_Type_Desc
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "Contact_Type_Desc";
            col++;

            //Contact_Phone_Num
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "Contact_Phone_Num";
            col++;

            //Contact_2_Type_Desc
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "Contact_2_Type_Desc";
            col++;

            //Contact_2_Phone_Num
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "Contact_2_Phone_Num";
            col++;

            //Promotion_2_Code
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "Promotion_2_Code";
            col++;

            //Default_Pricing_Plan_Desc
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "Default_Pricing_Plan_Desc";
            col++;

            //Payment_Sub_Type_Desc
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "Payment_Sub_Type_Desc";
            col++;

            //Billing_UOM_Desc
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "Billing_UOM_Desc";
            col++;

            //Daily_Late_Fee_Pct
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "Daily_Late_Fee_Pct";
            col++;

            //Gas_Pool_Id
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "Gas_Pool_Id";
            col++;

            //Contact_Relationship
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "Contact_Relationship";
            col++;

            //Contact_2_Relationship
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "Contact_2_Relationship";
            col++;

            //Summary_Billing_Ind	
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "Summary_Billing_Ind";
            col++;

            //Grnty_Waived_Desc	
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "Grnty_Waived_Desc";
            col++;

            //Billing_Pkg_Acct_Num	
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "Billing_Pkg_Acct_Num";
            col++;

            //Referred_By_ID	
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "Referred_By_ID";
            col++;

            //Membership_Id_Code	
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "Membership_Id_Code";
            col++;

            //Partner_Name	
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "Partner_Name";
            col++;

            //Partner_ID	
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "Partner_ID";
            col++;

            //Contract_ID	
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "Contract_ID";
            col++;

            //Country_Name	
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "Country_Name";
            col++;

            //Service_State_Code	
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "Service_State_Code";
            col++;

            //Plan_Term_Num	
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "Plan_Term_Num";
            col++;

            //Expiration_Code	
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "Expiration_Code";
            col++;

            //Early_Termination_Amt	
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "Early_Termination_Amt";
            col++;

            //Residual_Value_Amt	
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "Residual_Value_Amt";
            col++;

            //ETF_Value_Amt	
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "ETF_Value_Amt";
            col++;

            //ETF_Type_Desc	
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "ETF_Type_Desc";
            col++;

            //Sales_Date
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "Sales_Date";
            col++;

            //Alt_email
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "Alt_email";
            col++;

            //Opt_in
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "Opt_in";
            col++;

            //NON NUMERIC TERM
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "NON NUMERIC TERM";
            col++;


            col = colInitialize;
            row++;

            #endregion Header

            #region Data
            foreach (Record record in listOfRecords)
            {
                //Revenue_Class_Desc
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = record.CustomerType == "Commercial" ? "Small Commercial" : record.CustomerType;
                col++;

                //First_Name
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = record.ServiceFirstName; //record.CustomerType == "Commercial" ? record.AccountFirstName : record.ServiceFirstName;
                col++;

                //Last_Name
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = record.ServiceLastName;//record.CustomerType == "Commercial" ? record.AccountFirstName : record.ServiceLastName;
                col++;

                //Customer_Name
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = record.CustomerType == "Commercial" ? record.CompanyName : string.Empty;
                col++;

                //Home_Phone_Num
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = record.ServicePhone;
                col++;

                //Work_Phone_Num
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = "";
                col++;

                //Social_Sec_Code
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = "";
                col++;

                //Fed_Tax_Id_Num
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = "";
                col++;

                //Cellular_Num
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = record.CustomerType == "Commercial" ? record.ServicePhone : string.Empty;
                col++;

                //Email_Address
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = record.ServiceEmail;
                col++;

                //Language_Pref_Code
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = record.Language;
                col++;

                //Credit_Score_Num
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = "";
                col++;

                //Contact_Name
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = record.CustomerType == "Commercial" ? record.ServiceFirstName + " " + record.ServiceLastName : record.BillingFirstName + " " + record.BillingLastName;
                col++;

                //SLine1_Addr
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = record.ServiceAddress1;
                col++;

                //SLine2_Addr
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = "";
                col++;

                //SCity_Name
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = record.ServiceCity;
                col++;

                //SCounty_Name
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = "";
                col++;

                //SPostal_Code
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = record.ServiceZip;
                col++;

                //Marketer_Name
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = "Oasis Energy"; //Hardcoded to Oasis Energy
                col++;

                //Distributor_Name
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = record.Utility;
                col++;

                //Service_Type_Desc
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = record.CommodityType;
                col++;

                //Bill_Method
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = record.BillMethod;
                col++;

                //LDC_Account_Num
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = record.UtilityAccountNumber;
                col++;

                //Enroll_Type_Desc
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = "Request";
                col++;

                //Requested_Start_Date
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = string.Format("{0:yyyy-MM-dd}", record.SoldDate);
                col++;

                //Special_Meter_Read_Date
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = "";
                col++;

                //Waive_Notification_Ind
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = "";
                col++;

                //Tax_Exemption_Ind
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = "N"; //This is always N, even if the customer is tax-exempt. Once Oasis receives their exempt form, we will update our system with Y.
                col++;

                //Plan_Desc
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = record.ProgramDescription;
                col++;

                //Contract_Start_Date
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = string.Format("{0:yyyy-MM-dd}", record.SoldDate);
                col++;

                DateTime ContractStartDate = DateTime.Parse(record.SoldDate.ToString());
                DateTime ContractEndDate = new DateTime(ContractStartDate.Year, ContractStartDate.Month, ContractStartDate.Day);
                int MonthlyTerm = int.Parse(record.TermMonths.ToString());
                ContractEndDate = ContractEndDate.AddMonths(MonthlyTerm);
                ContractEndDate = ContractEndDate.AddDays(-1);
                //Contract_End_Date
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = string.Format("{0:yyyy-MM-dd}", ContractEndDate); //ContractStartDate + MonthlyTerm - 1 day
                col++;

                //Fixed_Commodity_Amt
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                //exRange.NumberFormat = "0.00"; //removed as they wanted this formatted as text
                exRange.Value2 = record.CommodityPrice;
                col++;

                //Agent_Code
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = record.Marketer;
                col++;

                //Commission_Plan
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                //exRange.Value2 = ""; //"Flat Fee " + record.CommodityType;
                exRange.Value2 = "Flat Fee " + record.CommodityType;
                col++;

                //Commission_Start_Date
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = string.Format("{0:yyyy-MM-dd}", record.SoldDate);
                col++;

                //DateTime CommissionStartDate = DateTime.Parse(record.SoldDate.ToString());
                //DateTime CommissionEndDate = new DateTime(ContractStartDate.Year, ContractStartDate.Month, ContractStartDate.Day);
                //int DailyTerm = 30;
                //CommissionEndDate = CommissionStartDate.AddDays(DailyTerm);
                //Commission_End_Date
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = ""; //string.Format("{0:yyyy-MM-dd}", CommissionEndDate);//CommissionStartDate + 30
                col++;

                //Commission_Unit_Num
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = record.CommissionNumber;
                col++;

                //Promotion_Code
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = "";
                col++;

                //Ad_Source_Desc
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = record.LDCCode == "COLPAG" ? "T" : string.Empty;
                col++;

                //MLine1_Addr
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = "";
                col++;

                //MLine2_Addr
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = "";
                col++;

                //MLine3_Addr
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = "";
                col++;

                //MLine4_Addr
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = "";
                col++;

                //MCity_Name
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = "";
                col++;

                //MState
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = "";
                col++;

                //Mcountry_Name
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = "";
                col++;

                //MPostal_Code
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = "";
                col++;

                //Employee_Ind
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = "";
                col++;

                //Low_Income_Ind
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = "";
                col++;

                //Life_Support_Ind
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = "";
                col++;

                //Interruptible_Ind
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = "";
                col++;

                //Approx_Annual_Usage
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = "";
                col++;

                //Budget_Amt
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = "";
                col++;

                //Deposit_Installment_Amt
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = "";
                col++;

                //Deposit_Installment_Qty
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = "";
                col++;

                //Security_Question
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = "";
                col++;

                //Security_Answer
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = "";
                col++;

                //Employer_Name
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = "";
                col++;

                //Drivers_Lic_Num
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = "";
                col++;

                //Drivers_Lic_State_Code
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = "";
                col++;

                //Verification_Type_Desc
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = "3rd Party";//Hardcoded 3rd Party
                col++;

                //Confirmation_Code
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = record.ConfirmationNumber.ToString();
                col++;

                //Commission_Master_Unit_Num
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = "";
                col++;

                //Master_Code
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = record.SalesChannel;
                col++;

                //Index_Adder_Num
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = "";
                col++;

                //Billing_Pkg_Name
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = "";
                col++;

                //Account_Name
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = "";
                col++;

                //ExportFileName
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = "";
                col++;

                //ExportDate
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = "";
                col++;

                //Commission_Sub_Agent_Unit_Num
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = "";
                col++;

                //Sub_Agent_Code
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = record.SalesAgent;
                col++;

                //Supply_Zone_Desc
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = record.SupplyZoneDesc;
                col++;

                //Fax_Num
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = "";
                col++;

                //Equipment_Id_Code
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = "";
                col++;

                //Rto_Amt
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = "";
                col++;

                //Payment_Type_Desc
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = "";
                col++;

                //Payment_Subscriber_Id_Code
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = "";
                col++;

                //Fixed_Charge_Amt
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.NumberFormat = "0.00";
                exRange.Value2 = record.MonthlyFee;
                col++;

                //Legacy_Account_Num
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = string.Format("{0:yyyy-MM-dd}", record.SoldDate);
                col++;

                //Legacy_Id
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = "";
                col++;

                //Birth_Date
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = "";
                col++;

                //Enrollment_Source_Code
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = "";
                col++;

                //Esignature_Code
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = "";
                col++;

                //Commission_2_Plan
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = "";
                col++;

                //Commission_2_Start_Date
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = "";
                col++;

                //Commission_2_End_Date
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = "";
                col++;

                //Commission_2_Agent_Unit_Num
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = "";
                col++;

                //Commission_2_Master_Unit_Num
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = "";
                col++;

                //Commission_2_Sub_Unit_Num
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = "";
                col++;

                //Deposit_Suggested_Amt
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = "";
                col++;

                //Group_Desc
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = "";
                col++;

                //Landlord_Agreement_Id_Desc
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = "";
                col++;

                //Attention_Name
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = "";
                col++;

                //Service_Priority_Desc
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = "";
                col++;

                //Delivery_Method_Desc
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = "";
                col++;

                //Heat_Rate_Num
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = "";
                col++;

                //Rep_Adder_Num
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = "";
                col++;

                //Work_Ext_Num
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = "";
                col++;

                //Credit_Rating_Source_Desc
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = "";
                col++;

                //Min_Index_Num
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = "0";//Hardcoded 0
                col++;

                //Max_Index_Num
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = "0";//Hardcoded 0
                col++;

                //Delinquent_Days_Cnt
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = "";
                col++;

                //Request_Hist_Usage_Ind
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = "";
                col++;

                //Request_Hist_Interval_Ind
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = "";
                col++;

                //Interval_Non_Edi_Ind
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = "";
                col++;

                //Interval_Non_Edi_Ind
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = "";
                col++;

                //Master_Ind
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = "";
                col++;

                //Type_Of_Service_Desc
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = "";
                col++;

                //Contact_2_Name
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = "";
                col++;

                //Doing_Business_As_Name
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = "";
                col++;

                //Web_Site_Addr
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = "";
                col++;

                //Remit_Duns_Num
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = "";
                col++;

                //Contact_Type_Desc
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = "";
                col++;

                //Contact_Phone_Num
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = "";
                col++;

                //Contact_2_Type_Desc
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = "";
                col++;

                //Contact_2_Phone_Num
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = "";
                col++;

                //Promotion_2_Code
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = "";
                col++;

                //Default_Pricing_Plan_Desc
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = record.DefaultPricingPlanDescription;
                col++;

                //Payment_Sub_Type_Desc
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = "";
                col++;

                //Billing_UOM_Desc
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = "";
                col++;

                //Daily_Late_Fee_Pct
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = "";
                col++;

                //Gas_Pool_Id
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = record.LDCCode == "PNGPA" ? "9900004277" : string.Empty;
                col++;

                //Contact_Relationship
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = "";
                col++;

                //Contact_2_Relationship
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = "";
                col++;

                //Summary_Billing_Ind	
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = "";
                col++;

                //Grnty_Waived_Desc	
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = "";
                col++;

                //Billing_Pkg_Acct_Num	
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = "";
                col++;

                //Referred_By_ID	
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = "";
                col++;

                //Membership_Id_Code	
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = "";
                col++;

                //Partner_Name	
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = "";
                col++;

                //Partner_ID	
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = "";
                col++;

                //Contract_ID	
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = "";
                col++;

                //Country_Name	
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = "";
                col++;

                //Service_State_Code	
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = "";
                col++;

                //Plan_Term_Num	
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = "";
                col++;

                //Expiration_Code	
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = "";
                col++;

                //Early_Termination_Amt	
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = "";
                col++;

                //Residual_Value_Amt	
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = "";
                col++;

                //ETF_Value_Amt	
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = "";
                col++;

                //ETF_Type_Desc	
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = "";
                col++;

                //Sales_Date
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = string.Format("{0:yyyy-MM-dd}", record.SoldDate);
                col++;

                //Alt_email
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = "";
                col++;

                //Opt_in
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = "";
                col++;

                //NON NUMERIC TERM
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = "";
                col++;

                col = colInitialize;
                row++;
            }

            #endregion Data

            exRange = (Excel.Range)exApp.get_Range("A1", "EO1");
            exRange.EntireColumn.AutoFit();
        }

        #endregion Excel

        #region Get Data


        #region Methods to build the loops for running the report (2 methods)

        //VENDOR OBJECT is no longer used. They do not want us to break out by Vendors
        ///// <summary>
        ///// Gets a list of Vendors joined with the office joined with the SalesChannel data for the primary loop
        ///// </summary>
        ///// <returns></returns>
        //private static List<VendorObject> GetVendorList()
        //{
        //    //SELECT  distinct v.[VendorId]
        //    //       ,v.[VendorNumber]
        //    //       ,v.[VendorName]
        //    //       ,o.[MarketerCode]
        //    //       ,v.[SalesChannelId]
        //    //       ,sc.[Name]
        //    //       ,v.[IsActive]	   
        //    //FROM [Spark].[v1].[Vendor] v
        //    //JOIN [Spark].[v1].[Office] o on v.[VendorId] = o.[VendorId]
        //    //JOIN [Spark].[v1].[SalesChannel] sc on o.[SalesChannelId] = sc.[SalesChannelId]
        //    //where v.[IsActive] = 1
        //    //and sc.[IsActive] = 1
        //    List<VendorObject> vendors = new List<VendorObject>();
        //    using (SparkEntities entitites = new SparkEntities())
        //    {
        //        var query = (from v in entitites.Vendors
        //                     join o in entitites.Offices on v.VendorId equals o.VendorId
        //                     join sc in entitites.SalesChannels on o.SalesChannelId equals sc.SalesChannelId
        //                     where v.IsActive == true
        //                     && sc.IsActive == true
        //                     select new
        //                     {
        //                         VendorId = v.VendorId,
        //                         VendorNumber = v.VendorNumber,
        //                         VendorName = v.VendorName,
        //                         MarketerCode = o.MarketerCode,
        //                         SalesChannelId = v.SalesChannelId,
        //                         SalesChannelName = sc.Name
        //                     }).Distinct();

        //        foreach (var item in query.OrderBy(sc => sc.SalesChannelName))
        //        {
        //            VendorObject vendor = new VendorObject(item.VendorId, item.VendorNumber, item.VendorName, item.MarketerCode, item.SalesChannelId, item.SalesChannelName);
        //            vendors.Add(vendor);
        //        }
        //    }

        //    return vendors;
        //}

        /// <summary>
        /// Gets a list of Oasis branded states from the Program table
        /// </summary>
        /// <returns></returns>
        private static List<ProgramObject> GetStateList()
        {
            //SELECT 	distinct [State]     
            //FROM [Spark].[v1].[Program]
            //where BrandId = 2
            List<ProgramObject> states = new List<ProgramObject>();
            using (SparkEntities entitites = new SparkEntities())
            {
                var query = (from p in entitites.Programs
                             where p.BrandId == 2
                             select new
                             {
                                 State = p.State
                             }).Distinct();

                foreach (var u in query)
                {
                    ProgramObject state = new ProgramObject(u.State);
                    states.Add(state);
                }
            }

            return states;
        }

        #endregion Methods to build the loops for running the report (2 methods)
        //VENDOR OBJECT is no longer used. They do not want us to break out by Vendors


        #region Method to Get RecordData (1 method)
        //private static List<Record> GetListOfRecords(DateTime sDate, DateTime eDate, string salesChannelName, string vendorName, string stateName)
        private static List<Record> GetListOfRecords(DateTime sDate, DateTime eDate, string salesChannelName, string stateName)
        {
            //SELECT distinct m.mainid, ut.UtilityTypeName, od.AccountNumber, pt.PremiseTypeName, od.CustomerNameKey, m.AuthorizationFirstName, m.AuthorizationLastName, od.ServiceAddress,
            //    od.ServiceCity,od.ServiceState, od.ServiceZip, od.ServiceCounty, m.Email, m.Btn, m.AccountFirstName,m.AccountLastName,od.BillingFirstName,od.BillingLastName,
            //    od.BillingAddress,od.BillingCity, od.BillingState, od.BillingZip,od.BillingCounty, m.Email, m.Btn,u.Language, p.ProgramName, p.Rate,p.Term, p.Msf, p.Etf,
            //    o.MarketerCode,od.OrderDetailId, sc.Name, usc.UtilitySalesChannelName,u.AgentId,m.CallDateTime, od.RateClass, od.MeterNumber,  od.ServiceReferenceNumber,
            //    m.SwitchDate, p.CreditCheck, p.ProgramDescription, p.DefaultPricingPlanDescription, p.DefaultPricingPlanDescription, oun.BatchName, oun.BillMethod, uty.LdcCode
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
            //join [Spark].[v1].[Office] o on u.[OfficeId] = o.[OfficeId]
            //join [Spark].[v1].[UtilitySalesChannel] usc on uty.UtilityId = usc.UtilityId
            //join [Spark].[v1].[SalesChannel] sc on usc.SalesChannelId = sc.SalesChannelId
            //join [Spark].[v1].[OasisUtilityName] oun on uty.UtilityId = oun.UtilityId
            //where m.CallDateTime > '5/23/2016' and m.CallDateTime < '5/24/2016'
            //and m.Verified ='1'
            //and sc.Name ='DTD'
            //--and v.VendorName='Watts Marketing Services, LLC'
            //and p.State = 'PA'
            //and usc.SalesChannelId = o.SalesChannelId
            //and usc.UtilityId = uty.UtilityId	
            //and oun.UtilityId = uty.UtilityId
            //and oun.UtilityTypeId = ut.UtilityTypeId
            //and p.BrandId = 2
            //order by m.MainId

            List<Record> records = new List<Record>();
            try
            {
                using (SparkEntities entitites = new SparkEntities())
                {
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
                                 join usc in entitites.UtilitySalesChannels on uty.UtilityId equals usc.UtilityId
                                 join sc in entitites.SalesChannels on usc.SalesChannelId equals sc.SalesChannelId
                                 join oun in entitites.OasisUtilityNames on uty.UtilityId equals oun.UtilityId
                                 where m.CallDateTime > sDate && m.CallDateTime < eDate
                                 && m.Verified == "1"
                                 && sc.Name == salesChannelName
                                     // && v.VendorName == vendorName
                                 && p.State == stateName
                                 && usc.SalesChannelId == o.SalesChannelId
                                 && usc.UtilityId == uty.UtilityId
                                 && oun.UtilityId == uty.UtilityId
                                 && oun.UtilityTypeId == ut.UtilityTypeId
                                 && p.BrandId == 2 //We only want Oasis data
                                 select new
                                 {
                                     ConfirmationNumber = m.MainId,
                                     Utility = string.IsNullOrEmpty(oun.BatchName) ? string.Empty : oun.BatchName.ToUpper(),
                                     CommodityType = string.IsNullOrEmpty(ut.UtilityTypeName) ? string.Empty : ut.UtilityTypeName.ToUpper(),
                                     UtilityAccountNumber = od.AccountNumber.ToUpper(),
                                     CustomerType = string.IsNullOrEmpty(pt.PremiseTypeName) ? string.Empty : pt.PremiseTypeName.ToUpper(),
                                     NameKey = string.IsNullOrEmpty(od.CustomerNameKey) ? string.Empty : od.CustomerNameKey.ToUpper(),
                                     ServiceFirstName = string.IsNullOrEmpty(m.AuthorizationFirstName) ? string.Empty : m.AuthorizationFirstName.ToUpper(),
                                     ServiceLastName = string.IsNullOrEmpty(m.AuthorizationLastName) ? string.Empty : m.AuthorizationLastName.ToUpper(),
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
                                     //Language = string.IsNullOrEmpty(m.PreferredLanguage) ? string.Empty : m.PreferredLanguage.ToUpper(),
                                     ProductOffering = string.IsNullOrEmpty(p.ProgramName) ? string.Empty : p.ProgramName.ToUpper(),
                                     CommodityPrice = p.Rate,
                                     TermMonths = p.Term,
                                     MonthlyFee = p.Msf,
                                     ETF = p.Etf,
                                     Marketer = o.MarketerCode,
                                     ExternalSalesID = od.OrderDetailId,
                                     SalesChannel = (sc.Name == "TeleSales") ? "OTM" : sc.Name.ToUpper(),
                                     SalesAgent = string.IsNullOrEmpty(u.AgentId) ? string.Empty : u.AgentId.ToUpper(),
                                     SoldDate = m.CallDateTime,
                                     RateClass = od.RateClass,
                                     MeterNumber = od.MeterNumber,
                                     UtilitySalesChannelName = string.IsNullOrEmpty(usc.UtilitySalesChannelName) ? string.Empty : usc.UtilitySalesChannelName.ToUpper(),
                                     ServiceReferenceNumber = string.IsNullOrEmpty(od.ServiceReferenceNumber) ? string.Empty : od.ServiceReferenceNumber,
                                     SwitchDate = string.IsNullOrEmpty(m.SwitchDate) ? string.Empty : m.SwitchDate,
                                     CreditCheck = p.CreditCheck,
                                     BillMethod = string.IsNullOrEmpty(oun.BillMethod) ? string.Empty : oun.BillMethod.ToUpper(),
                                     CommissionNumber = v.CommissionNumber,
                                     ProgramDescription = string.IsNullOrEmpty(p.ProgramDescription) ? string.Empty : p.ProgramDescription.ToUpper(),
                                     DefaultPricingPlanDescription = string.IsNullOrEmpty(p.DefaultPricingPlanDescription) ? string.Empty : p.DefaultPricingPlanDescription.ToUpper(),
                                     LDCCode = string.IsNullOrEmpty(uty.LdcCode) ? string.Empty : uty.LdcCode.ToUpper(),
                                     SupplyZoneDesc = string.IsNullOrEmpty(oun.SupplyZoneDesc) ? string.Empty : oun.SupplyZoneDesc.ToUpper(),
                                     CompanyName = string.IsNullOrEmpty(m.CompanyName) ? string.Empty : m.CompanyName.ToUpper(),
                                     Dnis = m.Dnis
                                 }).Distinct().ToList();


                    foreach (var item in query)
                    {

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

                        Record record = new Record(item.ConfirmationNumber.ToString(), item.Utility, item.CommodityType, item.UtilityAccountNumber.ToUpper(), item.CustomerType,
                                                    item.NameKey, item.ServiceFirstName, item.ServiceLastName, item.ServiceAddress1, item.ServiceCity,
                                                    item.ServiceState, item.ServiceZip, item.ServiceCounty, item.ServiceEmail, item.ServicePhone, item.AccountFirstName,
                                                    item.AccountLastName, item.BillingFirstName, item.BillingLastName, item.BillingAddress1, item.BillingCity,
                                                    item.BillingState, item.BillingZip, item.BillingCounty, item.BillingEmail, item.BillingPhone, language,
                                                    item.ProductOffering, item.CommodityPrice, item.TermMonths, item.MonthlyFee, item.ETF, item.Marketer,
                                                    "CAL" + item.ExternalSalesID.ToString(), item.SalesChannel, item.SalesAgent, item.SoldDate, item.RateClass,
                                                    item.MeterNumber, item.UtilitySalesChannelName, item.ServiceReferenceNumber, item.SwitchDate, item.CreditCheck,
                                                    item.BillMethod, item.CommissionNumber, item.ProgramDescription, item.DefaultPricingPlanDescription, item.LDCCode,
                                                    item.SupplyZoneDesc, item.CompanyName);
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
        /// <param name="salesChannel"></param>
        /// <param name="vendorName"></param>
        /// <param name="programState"></param>        
        private static void SaveXlsDocument(ref string reportPath, ref string xlsFilename, ref string xlsFilePath, Excel.Workbook exBook, DateTime currentDate, string salesChannel, string programState)
        {
            //Oasis_Prospects_SalesChannel_ProgramState_yyyyMMdd.xlsx            
            xlsFilename = "Oasis_Prospects_" + salesChannel.ToUpper() + "_" + programState + "_" + String.Format("{0:yyyyMMdd}", currentDate) + ".xlsx";

            xlsFilePath = string.Format(reportPath + xlsFilename);
            bool fileExists = File.Exists(xlsFilePath);
            if (fileExists)
            {
                //delete it
                File.Delete(xlsFilePath);
            }
            //save workbook
            exBook.SaveAs(Filename: xlsFilePath, FileFormat: Excel.XlFileFormat.xlOpenXMLWorkbook);
        }

        private static void FTPFile(ref string reportPath, ref string filename, ref string putFilePath, DateTime currentDate, string salesChannel, string programState, string HostName, string ToDir, string UserName, string Password)
        {
            //Oasis_Prospects_VendorName_SalesChannel_ProgramState_yyyyMMdd.xlsx
            // xlsFilename = "Oasis_Prospects_" +  salesChannel.ToUpper() + "_" + programState + "_" + String.Format("{0:yyyyMMdd}", currentDate) + ".xlsx";

            //xlsFilePath = string.Format(reportPath + xlsFilename);

            putFilePath = string.Format(reportPath + filename);
            try
            {
                //Calibrus.Ftp.Upload ftp = new Calibrus.Ftp.Upload();
                //ftp.Host = new Uri(string.Format("ftp://{0}/", HostName));
                //ftp.UserName = UserName;
                //ftp.Password = Password;
                //ftp.UploadFile(xlsFilePath, xlsFilename);

                //Renci.sshNet
                UploadFileRenciSshNet(HostName, UserName, Password, ToDir, putFilePath);
            }
            catch (Exception ex)
            {
                SendErrorMessage(ex);
            }
        }

        /// <summary>
        /// This sample will upload a file on your local machine to the remote system. 
        /// Using the Renci.SshNet dll found on \\Tmpdev2\Production\CalibrusFramework\2012\RenciSshNet which is written in .net 4.0 and is a rewrite of the Tamir.SharpSSH
        /// http://sshnet.codeplex.com/wikipage?title=Draft%20for%20Documentation%20page
        /// </summary>
        private static void UploadFileRenciSshNet(string host, string username, string password, string toDir, string localFileName)
        {
            //string host = "";
            //string username = "";
            //string password = "";
            //string localFileName = "";
            string remoteFileName = System.IO.Path.GetFileName(localFileName);

            using (var sftp = new SftpClient(host, username, password))
            {
                sftp.Connect();

                using (Stream file = File.OpenRead(localFileName))
                {
                    sftp.ChangeDirectory(toDir);
                    sftp.UploadFile(file, remoteFileName);
                }

                sftp.Disconnect();
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
            Calibrus.ErrorHandler.Alerting alert = new Calibrus.ErrorHandler.Alerting("SparkOasisBatchXLSReport");
            alert.SendAlert(ex.Source, ex.Message, Environment.MachineName, Environment.UserName, Environment.Version.ToString());
        }
        #endregion Utilities
    }
}
