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

namespace SparkMajorEnergyEnrollmentDaily
{
    public class Enrollment
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
                //Build Record Object based on Vendor
                List<Record> recordList = GetListOfRecords(StartDate, EndDate, vendor.VendorId);

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

            //Business Name		
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "Business Name";
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

            //Zip		
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "Zip";
            col++;

            //Phone		
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "Phone";
            col++;

            //Work Phone		
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "Work Phone";
            col++;

            //Email		
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "Email";
            col++;

            //Language		
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "Language";
            col++;

            //Sales Agency		
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "Sales Agency";
            col++;

            //Sales Agent		
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "Sales Agent";
            col++;

            //Service Type		
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "Service Type";
            col++;

            //Contact Name		
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "Contact Name";
            col++;

            //Relationship To The Account Holder		
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "Relationship To The Account Holder";
            col++;

            //TPV Confirmation Code	
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "TPV Confirmation Code";
            col++;

            //Service Class		
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "Service Class";
            col++;

            //Electric Utility		
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "Electric Utility";
            col++;

            //Electric Account Number		
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "Electric Account Number";
            col++;

            //Electric Rate Plan		
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "Electric Rate Plan";
            col++;

            //Gas Utility		
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "Gas Utility";
            col++;

            //Gas Account Number		
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "Gas Account Number";
            col++;

            //Gas Rate Plan		
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "Gas Rate Plan";
            col++;

            //Application Number		
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "Application Number";
            col++;

            //Date Sold		
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "Date Sold";
            col++;

            //Disposition		
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "Disposition";
            col++;

            //Electric Duration		
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "Electric Duration";
            col++;

            //Gas Duration	
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                11, true, false, false);
            exRange.Value2 = "Gas Duration";
            col++;

            col = colInitialize;
            row++;

            #endregion Header

            #region Data

            foreach (Record record in listOfRecords)
            {
                //First Name	
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = record.AuthorizationFirstName;
                col++;

                //Last Name		
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = record.AuthorizationLastName;
                col++;

                //Business Name		
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = record.CompanyName;
                col++;

                //Address		
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = record.ServiceAddress;
                col++;

                //City		
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = record.ServiceCity;
                col++;

                //State		
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = record.ServiceState;
                col++;

                //Zip		
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = record.ServiceZip;
                col++;

                //Phone		
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = record.Btn;
                col++;

                //Work Phone	
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = "";
                col++;

                //Email		
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = record.Email;
                col++;

                //Language		
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = record.PreferredLanguage;
                col++;

                //Sales Agency		
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = "PLT"; //record.VendorName; //Removed
                col++;

                //Sales Agent		
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = record.AgentId;
                col++;

                //Service Type		
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = record.UtilityTypeName;
                col++;

                //Contact Name		
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = record.CompanyContactFirstName + " " + record.CompanyContactLastName;
                col++;

                //Relationship To The Account Holder	
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = record.Relation;
                col++;

                //TPV Confirmation Code		
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = record.MainId;
                col++;

                //Service Class		
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = record.PremiseTypeName;
                col++;

                //Electric Utility		
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = record.UtilityTypeName.ToLower() == "electric" ? record.LdcCode : string.Empty;
                col++;

                //Electric Account Number		
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = record.UtilityTypeName.ToLower() == "electric" ? record.AccountNumber : string.Empty;
                col++;

                //Electric Rate Plan	
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = record.UtilityTypeName.ToLower() == "electric" ? record.ProgramCode : string.Empty;
                col++;

                //Gas Utility		
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = record.UtilityTypeName.ToLower() == "gas" ? record.LdcCode : string.Empty;
                col++;

                //Gas Account Number	
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = record.UtilityTypeName.ToLower() == "gas" ? record.AccountNumber : string.Empty;
                col++;

                //Gas Rate Plan		
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = record.UtilityTypeName.ToLower() == "gas" ? record.ProgramCode : string.Empty;
                col++;

                //Application Number		
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = "";
                col++;

                //Date Sold		
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = String.Format("{0:MM/dd/yyyy}", record.CallDateTime);
                col++;

                //Disposition		
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = "Fixed"; //record.Concern;  //removed
                col++;

                //Electric Duration		
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = record.UtilityTypeName.ToLower() == "electric" ? record.Term : null;
                col++;

                //Gas Duration	
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                    11, false, false, false);
                exRange.Value2 = record.UtilityTypeName.ToLower() == "gas" ? record.Term : null;
                col++;


                //Reset column
                col = colInitialize;
                row++;
            }
            #endregion Data

            exRange = (Excel.Range)exApp.get_Range("A1", "AC1");
            exRange.EntireColumn.AutoFit();
        }
        #endregion Excel

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

        private static List<Record> GetListOfRecords(DateTime sDate, DateTime eDate, int vendorId)
        {
            //SELECT  Distinct od.OrderDetailId
            //    ,m.MainId
            //    ,od.ServiceCity
            //    ,od.ServiceState
            //    ,od.ServiceZip
            //    ,m.Btn
            //    ,m.Email
            //    ,m.PreferredLanguage	
            //    ,v.VendorName
            //    ,u.AgentId
            //    ,ut.UtilityTypeName
            //    ,m.CompanyContactFirstName
            //    ,m.CompanyContactLastName
            //    ,m.Relation	
            //    ,pt.PremiseTypeName	
            //    ,uty.LDCCode
            //    ,od.AccountNumber
            //    ,p.ProgramCode	
            //    ,m.CallDateTime	
            //    ,m.Concern	
            //    ,p.Term
            //FROM [Spark].[v1].[Main] m
            //join [Spark].[v1].[OrderDetail] od on m.mainid = od.MainId
            //join [Spark].[v1].[Program] p on od.ProgramId = p.ProgramId
            //join [Spark].[v1].[UtilityType] ut on p.UtilityTypeId = ut.UtilityTypeId
            //join [Spark].[v1].[Utility] uty on p.UtilityId = uty.UtilityId
            //join [Spark].[v1].[User] u on m.UserId = u.UserId
            //join [Spark].[v1].[Vendor] v on v.VendorId = u.VendorId
            //join [Spark].[v1].[PremiseType] pt on p.PremiseTypeId = pt.PremiseTypeId
            //join [Spark].[v1].[Brand] b on b.BrandId = p.BrandId 
            //where m.CallDateTime > '1/1/2017' and m.CallDateTime < '2/1/2017'
            //and m.Verified <> '9'            
            //and v.VendorId = 16
            //and p.BrandId = 6
            //order by OrderDetailId

            List<Record> records = new List<Record>();
            try
            {
                using (SparkEntities entities = new SparkEntities())
                {
                    //Filter for Loadfile Processed
                    var myInClause = new int[] { 6 }; //1 Spark data, 6 Major Energy data

                    var query = (from m in entities.Mains
                                 join od in entities.OrderDetails on m.MainId equals od.MainId
                                 join p in entities.Programs on od.ProgramId equals p.ProgramId
                                 join ut in entities.UtilityTypes on p.UtilityTypeId equals ut.UtilityTypeId
                                 join uty in entities.Utilities on p.UtilityId equals uty.UtilityId
                                 join u in entities.Users on m.UserId equals u.UserId
                                 join v in entities.Vendors on u.VendorId equals v.VendorId
                                 join pt in entities.PremiseTypes on p.PremiseTypeId equals pt.PremiseTypeId
                                 join b in entities.Brands on p.BrandId equals b.BrandId
                                 where m.CallDateTime > sDate
                                 && m.CallDateTime < eDate
                                 && m.Verified != "9"
                                 && v.VendorId == vendorId
                                     //&& p.BrandId == 2 //We only want Oasis data - 5 //We only want Provider data
                                 && myInClause.Contains(p.BrandId)
                                 select new
                                 {
                                     OrderDetailId = od.OrderDetailId,
                                     MainId = m.MainId,
                                     AuthorizationFirstName = m.AuthorizationFirstName,
                                     AuthorizationLastName = m.AuthorizationLastName,
                                     CompanyName = m.CompanyName,
                                     ServiceAddress = od.ServiceAddress,
                                     ServiceCity = od.ServiceCity,
                                     ServiceState = od.ServiceState,
                                     ServiceZip = od.ServiceZip,
                                     Btn = m.Btn,
                                     Email = m.Email,
                                     PreferredLanguage = m.PreferredLanguage,
                                     VendorName = v.VendorName,
                                     AgentId = u.AgentId,
                                     UtilityTypeName = ut.UtilityTypeName,
                                     CompanyContactFirstName = m.CompanyContactFirstName,
                                     CompanyContactLastName = m.CompanyContactLastName,
                                     Relation = m.Relation,
                                     PremiseTypeName = pt.PremiseTypeName,
                                     LdcCode = uty.LdcCode,
                                     AccountNumber = od.AccountNumber,
                                     ProgramCode = p.DefaultPricingPlanDescription,
                                     CallDateTime = m.CallDateTime,
                                     Concern = m.Concern,
                                     Term = p.Term
                                 }).Distinct().ToList();

                    foreach (var item in query)
                    {
                        Record record = new Record();
                        record.OrderDetailId = item.OrderDetailId;
                        record.MainId = item.MainId;
                        record.AuthorizationFirstName = item.AuthorizationFirstName;
                        record.AuthorizationLastName = item.AuthorizationLastName;
                        record.CompanyName = item.CompanyName;
                        record.ServiceAddress = item.ServiceAddress;
                        record.ServiceCity = item.ServiceCity;
                        record.ServiceState = item.ServiceState;
                        record.ServiceZip = item.ServiceZip;
                        record.Btn = item.Btn;
                        record.Email = item.Email;
                        record.PreferredLanguage = item.PreferredLanguage;
                        record.VendorName = item.VendorName;
                        record.AgentId = item.AgentId;
                        record.UtilityTypeName = item.UtilityTypeName;
                        record.CompanyContactFirstName = item.CompanyContactFirstName;
                        record.CompanyContactLastName = item.CompanyContactLastName;
                        record.Relation = item.Relation;
                        record.PremiseTypeName = item.PremiseTypeName;
                        record.LdcCode = IsValueNull(item.LdcCode) ? string.Empty : ConvertLDCCode(item.LdcCode);
                        record.AccountNumber = item.AccountNumber;
                        record.ProgramCode = item.ProgramCode;
                        record.CallDateTime = item.CallDateTime;
                        record.Concern = item.Concern;
                        record.Term = item.Term;
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
            xlsFilename = "Major_Energy_" + vendorName + "_Enrollment_" + String.Format("{0:MMddyyyy}", currentDate) + ".xls";

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

                mail.Subject = "Spark Major Energy Enrollment for " + vendorName + " " + currentDate.ToString("dddd, dd MMMM yyyy") + ".";

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

            Calibrus.ErrorHandler.Alerting alert = new Calibrus.ErrorHandler.Alerting("SparkMajorEnergyEnrollmentDaily");
            alert.SendAlert(ex.Source, sb.ToString(), Environment.MachineName, Environment.UserName, Environment.Version.ToString());
        }
        #endregion Error Handling
    }
}
