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

namespace ConstellationDTDPOSIdReportXLS
{

    public class Report
    {
        public static object na = System.Reflection.Missing.Value;

        public enum ReportType
        {
            Spectrum,
            Internal
        }

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

            //start Excel
            Excel.Application exApp = new Excel.Application();
            Excel.Workbook exBook = null;
            Excel.Worksheet exSheet = null;
            Excel.Range exRange = null;

            int sheetsAdded = 0;

            #region ReportTypeForLoop

            foreach (ReportType reportType in Enum.GetValues(typeof(ReportType)))
            {

                List<spDTDPOSIdReport_Result> dtdPosIdReportResults = GetDTDPOSIdReport(StartDate, EndDate);

                sheetsAdded = 0;

                try
                {
                    exBook = exApp.Workbooks.Add(na);
                    exApp.Visible = false;

                    //Set global attributes
                    exApp.StandardFont = "Calibri";
                    exApp.StandardFontSize = 11;

                    if (sheetsAdded < exBook.Sheets.Count)
                    {
                        exSheet = (Excel.Worksheet)exBook.Sheets[sheetsAdded + 1];
                    }
                    else
                    {
                        exSheet = (Excel.Worksheet)exBook.Sheets.Add(na, exBook.ActiveSheet, na, na);
                    }

                    //dxc_pos_id_results_06_21_2016
                    string sheetName = string.Format("dxc_pos_id_results_{0:MM_dd_yyyy}", StartDate);
                    exSheet.Name = sheetName.Length > 30 ? sheetName.Substring(0, 30) : sheetName; //force length of sheet name due to excel constraints
                    exSheet.Select(na);

                    sheetsAdded++;

                    switch (reportType.ToString())
                    {
                        case "Internal":
                            mailRecipientTO = ConfigurationManager.AppSettings["mailRecipientINTERNALTO"].ToString();
                            mailRecipientBCC = ConfigurationManager.AppSettings["mailRecipientBCC"].ToString();
                            WriteReport(ref exApp, ref exRange, StartDate, EndDate, dtdPosIdReportResults, reportType.ToString());
                            break;

                        case "Spectrum":
                            mailRecipientTO = ConfigurationManager.AppSettings["mailRecipientSPECTRUMTO"].ToString();
                            mailRecipientBCC = ConfigurationManager.AppSettings["mailRecipientBCC"].ToString();
                            WriteReport(ref exApp, ref exRange, StartDate, EndDate, dtdPosIdReportResults, reportType.ToString());
                            break;

                    }

                    exRange = (Excel.Range)exApp.get_Range("A1", "CZ1");
                    exRange.EntireColumn.AutoFit();

                    //Freeze Pane
                    exRange = exSheet.Range["A2"];
                    exRange.Select();
                    exApp.ActiveWindow.FreezePanes = true;

                    //Save the xls Report
                    SaveXlsDocument(ref rootPath, ref xlsFilename, ref xlsFilePath, exBook, StartDate, reportType.ToString());
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
                SendEmail(ref xlsFilePath, StartDate, mailRecipientTO, mailRecipientBCC);
            }
            #endregion ReportTypeForLoop

        }

        #endregion Main

        #region Excel (1 method)

        public static void WriteReport(ref Excel.Application exApp, ref Excel.Range exRange, DateTime startDate, DateTime endDate, List<spDTDPOSIdReport_Result> dtdPosIdReportResults, string reportType)
        {
            int rowInitialize = 1; //initial seed for the row data
            int row = 0;// where we start the row data

            int dataColumnInitialize = 65; //initial seed for column data - column  A
            int col = 0;

            row = rowInitialize;  //set the row for the data
            col = dataColumnInitialize;//set the column for the data

            #region Header

            if (reportType == "Internal")
            {
                //infutor_pos_id_result
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                    11, false, false, false);
                exRange.Value2 = "infutor_pos_id_result";
                col++;

                //experian_pos_id_result	
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                    11, false, false, false);
                exRange.Value2 = "experian_pos_id_result";
                col++;

                //experian_lookup_result
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                    11, false, false, false);
                exRange.Value2 = "experian_lookup_result";
                col++;
            }
            else
            {
                //pos_id_result
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                    11, false, false, false);
                exRange.Value2 = "pos_id_result";
                col++;
            }


            //p_date
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                   11, false, false, false);
            exRange.Value2 = "p_date";
            col++;

            //dt_insert
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                11, false, false, false);
            exRange.Value2 = "dt_insert";
            col++;

            //dt_date
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                   11, false, false, false);
            exRange.Value2 = "dt_date";
            col++;

            //dt_scan
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                   11, false, false, false);
            exRange.Value2 = "dt_scan";
            col++;

            //center_id
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                   11, false, false, false);
            exRange.Value2 = "center_id";
            col++;

            //vendor_name
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                   11, false, false, false);
            exRange.Value2 = "vendor_name";
            col++;

            //source
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                   11, false, false, false);
            exRange.Value2 = "source";
            col++;

            //language
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                   11, false, false, false);
            exRange.Value2 = "language";
            col++;

            //requested_language
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                   11, false, false, false);
            exRange.Value2 = "requested_language";
            col++;

            //sales_state
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                   11, false, false, false);
            exRange.Value2 = "sales_state";
            col++;

            //fuel_type
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                   11, false, false, false);
            exRange.Value2 = "fuel_type";
            col++;

            //dual_fuel
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                   11, false, false, false);
            exRange.Value2 = "dual_fuel";
            col++;

            //contract
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                   11, false, false, false);
            exRange.Value2 = "contract";
            col++;

            //recordlocator
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                   11, false, false, false);
            exRange.Value2 = "recordlocator";
            col++;

            //btn
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                   11, false, false, false);
            exRange.Value2 = "btn";
            col++;

            //callback_btn
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                   11, false, false, false);
            exRange.Value2 = "callback_btn";
            col++;

            //email_address
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                   11, false, false, false);
            exRange.Value2 = "email_address";
            col++;

            //program_code
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                   11, false, false, false);
            exRange.Value2 = "program_code";
            col++;

            //promo_code
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                   11, false, false, false);
            exRange.Value2 = "promo_code";
            col++;

            //acct_num
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                   11, false, false, false);
            exRange.Value2 = "acct_num";
            col++;

            //auth_fname
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                   11, false, false, false);
            exRange.Value2 = "auth_fname";
            col++;

            //auth_lname
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                   11, false, false, false);
            exRange.Value2 = "auth_lname";
            col++;

            //bill_fname
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                   11, false, false, false);
            exRange.Value2 = "bill_fname";
            col++;

            //bill_lname
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                   11, false, false, false);
            exRange.Value2 = "bill_lname";
            col++;

            //billing_address
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                   11, false, false, false);
            exRange.Value2 = "billing_address";
            col++;

            //billing_city
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                   11, false, false, false);
            exRange.Value2 = "billing_city";
            col++;

            //billing_state
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                   11, false, false, false);
            exRange.Value2 = "billing_state";
            col++;

            //billing_zip
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                   11, false, false, false);
            exRange.Value2 = "billing_zip";
            col++;

            //meter
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                   11, false, false, false);
            exRange.Value2 = "meter";
            col++;

            //addr1
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                   11, false, false, false);
            exRange.Value2 = "addr1";
            col++;

            //addr2
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                   11, false, false, false);
            exRange.Value2 = "addr2";
            col++;

            //city
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                   11, false, false, false);
            exRange.Value2 = "city";
            col++;

            //state
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                   11, false, false, false);
            exRange.Value2 = "state";
            col++;

            //zip
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                   11, false, false, false);
            exRange.Value2 = "zip";
            col++;

            //county
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                   11, false, false, false);
            exRange.Value2 = "county";
            col++;

            //mx_or_tge
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                   11, false, false, false);
            exRange.Value2 = "mx_or_tge";
            col++;

            //utility
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                   11, false, false, false);
            exRange.Value2 = "utility";
            col++;

            //variable_or_fixed
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                   11, false, false, false);
            exRange.Value2 = "variable_or_fixed";
            col++;

            //ldc_code
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                   11, false, false, false);
            exRange.Value2 = "ldc_code";
            col++;

            //pmt_code
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                   11, false, false, false);
            exRange.Value2 = "pmt_code";
            col++;

            //credit_score
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                   11, false, false, false);
            exRange.Value2 = "credit_score";
            col++;

            //name_key
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                   11, false, false, false);
            exRange.Value2 = "name_key";
            col++;

            //rate
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                   11, false, false, false);
            exRange.Value2 = "rate";
            col++;

            //unit_price
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                   11, false, false, false);
            exRange.Value2 = "unit_price";
            col++;

            //unit_measurement
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                   11, false, false, false);
            exRange.Value2 = "unit_measurement";
            col++;

            //discount_months
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                   11, false, false, false);
            exRange.Value2 = "discount_months";
            col++;

            //term
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                   11, false, false, false);
            exRange.Value2 = "term";
            col++;

            //cancel_fee
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                   11, false, false, false);
            exRange.Value2 = "cancel_fee";
            col++;

            //ver_code
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                   11, false, false, false);
            exRange.Value2 = "ver_code";
            col++;

            //tsr_id
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                   11, false, false, false);
            exRange.Value2 = "tsr_id";
            col++;

            //tsr_name
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                   11, false, false, false);
            exRange.Value2 = "tsr_name";
            col++;

            //tsr_dt_added
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                   11, false, false, false);
            exRange.Value2 = "tsr_dt_added";
            col++;

            //status_txt
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                   11, false, false, false);
            exRange.Value2 = "status_txt";
            col++;

            //status_id
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                   11, false, false, false);
            exRange.Value2 = "status_id";
            col++;

            //reason
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                   11, false, false, false);
            exRange.Value2 = "reason";
            col++;

            //scan_filename
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                   11, false, false, false);
            exRange.Value2 = "scan_filename";
            col++;

            //scan_status_txt
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                   11, false, false, false);
            exRange.Value2 = "scan_status_txt";
            col++;

            //scan_status_id
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                   11, false, false, false);
            exRange.Value2 = "scan_status_id";
            col++;

            //scan_reason
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                   11, false, false, false);
            exRange.Value2 = "scan_reason";
            col++;

            //scan_update_tpv_status_result
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                   11, false, false, false);
            exRange.Value2 = "scan_update_tpv_status_result";
            col++;

            //scan_import_contract_result
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                   11, false, false, false);
            exRange.Value2 = "scan_import_contract_result";
            col++;

            //dxc_rep_id
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                   11, false, false, false);
            exRange.Value2 = "dxc_rep_id";
            col++;

            //call_time
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                   11, false, false, false);
            exRange.Value2 = "call_time";
            col++;

            //activewav
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                   11, false, false, false);
            exRange.Value2 = "activewav";
            col++;

            //audited
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                   11, false, false, false);
            exRange.Value2 = "audited";
            col++;

            //station_id
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                   11, false, false, false);
            exRange.Value2 = "station_id";
            col++;

            //premise
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                   11, false, false, false);
            exRange.Value2 = "premise";
            col++;

            //cb_begin
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                   11, false, false, false);
            exRange.Value2 = "cb_begin";
            col++;

            //cb_end
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                   11, false, false, false);
            exRange.Value2 = "cb_end";
            col++;

            //call_back
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                   11, false, false, false);
            exRange.Value2 = "call_back";
            col++;

            //called_back
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                   11, false, false, false);
            exRange.Value2 = "called_back";
            col++;

            //completed
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                   11, false, false, false);
            exRange.Value2 = "completed";
            col++;

            //cb_station
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                   11, false, false, false);
            exRange.Value2 = "cb_station";
            col++;

            //cb_status
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                   11, false, false, false);
            exRange.Value2 = "cb_status";
            col++;

            //identity_matched
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                   11, false, false, false);
            exRange.Value2 = "identity_matched";
            col++;

            //value_matched
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                   11, false, false, false);
            exRange.Value2 = "value_matched";
            col++;

            //form_name
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                   11, false, false, false);
            exRange.Value2 = "form_name";
            col++;

            //udclist_result
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                   11, false, false, false);
            exRange.Value2 = "udclist_result";
            col++;

            //planlist_result
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                   11, false, false, false);
            exRange.Value2 = "planlist_result";
            col++;

            //save_customer_sign_up_result
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                   11, false, false, false);
            exRange.Value2 = "save_customer_sign_up_result";
            col++;

            //confirm_customer_sign_up_result
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                   11, false, false, false);
            exRange.Value2 = "confirm_customer_sign_up_result";
            col++;

            //save_customer_sign_up_by_type_update
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                   11, false, false, false);
            exRange.Value2 = "save_customer_sign_up_by_type_update";
            col++;

            //update_tpv_status_result
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                   11, false, false, false);
            exRange.Value2 = "update_tpv_status_result";
            col++;

            //update_tpv_verification_code_result
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                   11, false, false, false);
            exRange.Value2 = "update_tpv_verification_code_result";
            col++;

            //get_customer_data_by_personal_code_result
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                   11, false, false, false);
            exRange.Value2 = "get_customer_data_by_personal_code_result";
            col++;

            //zip_outside_approved_area
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                   11, false, false, false);
            exRange.Value2 = "zip_outside_approved_area";
            col++;

            //agent_used_own_phone
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                   11, false, false, false);
            exRange.Value2 = "agent_used_own_phone";
            col++;

            //call_back_num_prev_used
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                   11, false, false, false);
            exRange.Value2 = "call_back_num_prev_used";
            col++;

            //btn_prev_used
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                   11, false, false, false);
            exRange.Value2 = "btn_prev_used";
            col++;

            //existing_customer
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                   11, false, false, false);
            exRange.Value2 = "existing_customer";
            col++;

            //reponse_id
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                   11, false, false, false);
            exRange.Value2 = "reponse_id";
            col++;

            //existing_bge_home_customer
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                   11, false, false, false);
            exRange.Value2 = "existing_bge_home_customer";
            col++;

            //apt_num_exceed_limit
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                   11, false, false, false);
            exRange.Value2 = "apt_num_exceed_limit";
            col++;

            //pos_id_match
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                   11, false, false, false);
            exRange.Value2 = "pos_id_match";
            col++;

            //existing_address
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                   11, false, false, false);
            exRange.Value2 = "existing_address";
            col++;

            //lot_or_trailer
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                   11, false, false, false);
            exRange.Value2 = "lot_or_trailer";
            col++;

            //on_aggregation_list
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                   11, false, false, false);
            exRange.Value2 = "on_aggregation_list";
            col++;

            //zip_on_restricted_list
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                   11, false, false, false);
            exRange.Value2 = "zip_on_restricted_list";
            col++;

            //home_type
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                   11, false, false, false);
            exRange.Value2 = "home_type";
            col++;

            //carrier_name
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                   11, false, false, false);
            exRange.Value2 = "carrier_name";
            col++;

            //carrier_type
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                   11, false, false, false);
            exRange.Value2 = "carrier_type";
            col++;
            //caller_id_name
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                   11, false, false, false);
            exRange.Value2 = "caller_id_name";
            col++;
            //caller_id_type
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                   11, false, false, false);
            exRange.Value2 = "caller_id_type";
            col++;


            row++;
            col = dataColumnInitialize;//reset the column for the data
            #endregion Header


            #region Data
            foreach (var item in dtdPosIdReportResults)
            {

                if (reportType == "Internal")
                {
                    //infutor_pos_id_result
                    exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                        11, false, false, false);
                    exRange.Value2 = item.InfutorResult;
                    col++;

                    //experian_pos_id_result	
                    exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                        11, false, false, false);
                    exRange.Value2 = item.ExperianResult;
                    col++;

                    //experian_lookup_result
                    string str = item.Status;  //Success - Experian
                    string status = str.Substring(0, str.LastIndexOf("-"));  //Success
                    exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                        11, false, false, false);
                    exRange.Value2 = status;
                    col++;
                }
                else
                {
                    //pos_id_result
                    exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                        11, false, false, false);
                    exRange.Value2 = GetPos_id_result(item.InfutorResult, item.ExperianResult); //Find the pos_id_result
                    col++;
                }


                //p_date
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                       11, false, false, false);
                exRange.Value2 = string.Format("{0: MM/dd/yyyy}", item.ResponseDateTime);
                col++;

                //dt_insert
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                    11, false, false, false);
                exRange.Value2 = string.Format("{0: MM/dd/yyyy hh:mm}", item.ResponseDateTime);
                col++;

                //dt_date
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                       11, false, false, false);
                exRange.Value2 = string.Format("{0: MM/dd/yyyy hh:mm}", item.CallDateTime);
                col++;

                //dt_scan
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                       11, false, false, false);
                exRange.Value2 = "";
                col++;

                //center_id
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                       11, false, false, false);
                exRange.Value2 = item.VendorName;
                col++;

                //vendor_name
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                       11, false, false, false);
                exRange.Value2 = "";
                col++;

                //source
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                       11, false, false, false);
                exRange.Value2 = "";
                col++;

                //language
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                       11, false, false, false);
                exRange.Value2 = item.Language;
                col++;

                //requested_language
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                       11, false, false, false);
                exRange.Value2 = "";
                col++;

                //sales_state
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                       11, false, false, false);
                exRange.Value2 = item.ServiceState;
                col++;

                //fuel_type
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                       11, false, false, false);
                exRange.Value2 = item.SignUpType;
                col++;

                //dual_fuel
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                       11, false, false, false);
                exRange.Value2 = item.DualSignUp;
                col++;

                //contract
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                       11, false, false, false);
                exRange.Value2 = "";
                col++;

                //recordlocator
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                       11, false, false, false);
                exRange.Value2 = item.ResponseId;
                col++;

                //btn
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                       11, false, false, false);
                exRange.Value2 = item.ServicePhoneNumber;
                col++;

                //callback_btn
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                       11, false, false, false);
                exRange.Value2 = item.ServicePhoneNumber;
                col++;

                //email_address
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                       11, false, false, false);
                exRange.Value2 = item.ServiceEmail;
                col++;

                //program_code
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                       11, false, false, false);
                exRange.Value2 = "";
                col++;

                //promo_code
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                       11, false, false, false);
                exRange.Value2 = item.PromoCode;
                col++;

                //acct_num
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                       11, false, false, false);
                exRange.Value2 = item.UDCAccountNumber;
                col++;

                //auth_fname
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                       11, false, false, false);
                exRange.Value2 = item.ServiceFirstName;
                col++;

                //auth_lname
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                       11, false, false, false);
                exRange.Value2 = item.ServiceLastName;
                col++;

                //bill_fname
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                       11, false, false, false);
                exRange.Value2 = item.BillingFirstName;
                col++;

                //bill_lname
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                       11, false, false, false);
                exRange.Value2 = item.BillingLastName;
                col++;

                //billing_address
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                       11, false, false, false);
                exRange.Value2 = item.BillingAddress1 + " " + item.BillingAddress2;
                col++;

                //billing_city
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                       11, false, false, false);
                exRange.Value2 = item.BillingCity;
                col++;

                //billing_state
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                       11, false, false, false);
                exRange.Value2 = item.BillingState;
                col++;

                //billing_zip
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                       11, false, false, false);
                exRange.Value2 = item.BillingZipCode;
                col++;

                //meter
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                       11, false, false, false);
                exRange.Value2 = item.MeterNumber;
                col++;

                //addr1
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                       11, false, false, false);
                exRange.Value2 = item.ServiceAddress1;
                col++;

                //addr2
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                       11, false, false, false);
                exRange.Value2 = item.ServiceAddress2;
                col++;

                //city
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                       11, false, false, false);
                exRange.Value2 = item.ServiceCity;
                col++;

                //state
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                       11, false, false, false);
                exRange.Value2 = item.ServiceState;
                col++;

                //zip
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                       11, false, false, false);
                exRange.Value2 = item.ServiceZipCode;
                col++;

                //county
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                       11, false, false, false);
                exRange.Value2 = "";
                col++;

                //mx_or_tge
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                       11, false, false, false);
                exRange.Value2 = "";
                col++;

                //utility
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                       11, false, false, false);
                exRange.Value2 = "";
                col++;

                //variable_or_fixed
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                       11, false, false, false);
                exRange.Value2 = "";
                col++;

                //ldc_code
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                       11, false, false, false);
                exRange.Value2 = item.UDCCode;
                col++;

                //pmt_code
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                       11, false, false, false);
                exRange.Value2 = "";
                col++;

                //credit_score
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                       11, false, false, false);
                exRange.Value2 = "";
                col++;

                //name_key
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                       11, false, false, false);
                exRange.Value2 = item.CustomerNameKey;
                col++;

                //rate
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                       11, false, false, false);
                exRange.Value2 = item.Rate;
                col++;

                //unit_price
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                       11, false, false, false);
                exRange.Value2 = "";
                col++;

                //unit_measurement
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                       11, false, false, false);
                exRange.Value2 = "";
                col++;

                //discount_months
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                       11, false, false, false);
                exRange.Value2 = "";
                col++;

                //term
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                       11, false, false, false);
                exRange.Value2 = item.Term;
                col++;

                //cancel_fee
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                       11, false, false, false);
                exRange.Value2 = item.CancelFee;
                col++;

                //ver_code
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                       11, false, false, false);
                exRange.Value2 = item.MainId;
                col++;

                //tsr_id
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                       11, false, false, false);
                exRange.Value2 = item.AgentId;
                col++;

                //tsr_name
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                       11, false, false, false);
                exRange.Value2 = item.AgentName;
                col++;

                //tsr_dt_added
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                       11, false, false, false);
                exRange.Value2 = "";
                col++;

                //status_txt
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                       11, false, false, false);
                exRange.Value2 = (item.Verified == "1" ? "good sale" : "no sale");
                col++;

                //status_id
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                       11, false, false, false);
                exRange.Value2 = "";
                col++;

                //reason
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                       11, false, false, false);
                exRange.Value2 = "";
                col++;

                //scan_filename
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                       11, false, false, false);
                exRange.Value2 = "";
                col++;

                //scan_status_txt
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                       11, false, false, false);
                exRange.Value2 = "";
                col++;

                //scan_status_id
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                       11, false, false, false);
                exRange.Value2 = "";
                col++;

                //scan_reason
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                       11, false, false, false);
                exRange.Value2 = "";
                col++;

                //scan_update_tpv_status_result
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                       11, false, false, false);
                exRange.Value2 = "";
                col++;

                //scan_import_contract_result
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                       11, false, false, false);
                exRange.Value2 = "";
                col++;

                //dxc_rep_id
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                       11, false, false, false);
                exRange.Value2 = item.AgentId;
                col++;

                //call_time
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                       11, false, false, false);
                exRange.Value2 = "";
                col++;

                //activewav
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                       11, false, false, false);
                exRange.Value2 = item.WavName;
                col++;

                //audited
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                       11, false, false, false);
                exRange.Value2 = "False";
                col++;

                //station_id
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                       11, false, false, false);
                exRange.Value2 = "";
                col++;

                //premise
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                       11, false, false, false);
                exRange.Value2 = "";
                col++;

                //cb_begin
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                       11, false, false, false);
                exRange.Value2 = "";
                col++;

                //cb_end
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                       11, false, false, false);
                exRange.Value2 = "";
                col++;

                //call_back
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                       11, false, false, false);
                exRange.Value2 = "";
                col++;

                //called_back
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                       11, false, false, false);
                exRange.Value2 = "";
                col++;

                //completed
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                       11, false, false, false);
                exRange.Value2 = "";
                col++;

                //cb_station
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                       11, false, false, false);
                exRange.Value2 = "";
                col++;

                //cb_status
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                       11, false, false, false);
                exRange.Value2 = "";
                col++;

                //identity_matched
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                       11, false, false, false);
                exRange.Value2 = "";
                col++;

                //value_matched
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                       11, false, false, false);
                exRange.Value2 = "";
                col++;

                //form_name
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                       11, false, false, false);
                exRange.Value2 = "";
                col++;

                //udclist_result
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                       11, false, false, false);
                exRange.Value2 = "";
                col++;

                //planlist_result
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                       11, false, false, false);
                exRange.Value2 = "";
                col++;

                //save_customer_sign_up_result
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                       11, false, false, false);
                exRange.Value2 = "";
                col++;

                //confirm_customer_sign_up_result
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                       11, false, false, false);
                exRange.Value2 = "";
                col++;

                //save_customer_sign_up_by_type_update
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                       11, false, false, false);
                exRange.Value2 = "";
                col++;

                //update_tpv_status_result
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                       11, false, false, false);
                exRange.Value2 = "";
                col++;

                //update_tpv_verification_code_result
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                       11, false, false, false);
                exRange.Value2 = "";
                col++;

                //get_customer_data_by_personal_code_result
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                       11, false, false, false);
                exRange.Value2 = "";
                col++;

                //zip_outside_approved_area
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                       11, false, false, false);
                exRange.Value2 = "";
                col++;

                //agent_used_own_phone
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                       11, false, false, false);
                exRange.Value2 = "";
                col++;

                //call_back_num_prev_used
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                       11, false, false, false);
                exRange.Value2 = "";
                col++;

                //btn_prev_used
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                       11, false, false, false);
                exRange.Value2 = "";
                col++;

                //existing_customer
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                       11, false, false, false);
                exRange.Value2 = "";
                col++;

                //reponse_id
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                       11, false, false, false);
                exRange.Value2 = item.ResponseId;
                col++;

                //existing_bge_home_customer
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                       11, false, false, false);
                exRange.Value2 = "";
                col++;

                //apt_num_exceed_limit
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                       11, false, false, false);
                exRange.Value2 = "";
                col++;

                //pos_id_match
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                       11, false, false, false);
                exRange.Value2 = "";
                col++;

                //existing_address
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                       11, false, false, false);
                exRange.Value2 = "";
                col++;

                //lot_or_trailer
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                       11, false, false, false);
                exRange.Value2 = "";
                col++;

                //on_aggregation_list
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                       11, false, false, false);
                exRange.Value2 = "";
                col++;

                //zip_on_restricted_list
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                       11, false, false, false);
                exRange.Value2 = "";
                col++;

                //home_type
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                       11, false, false, false);
                exRange.Value2 = "";
                col++;

                //carrier_name
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                    11, false, false, false);
                exRange.Value2 = item.CarrierName;
                col++;

                //carrier_type
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                    11, false, false, false);
                exRange.Value2 = item.CarrierType;
                col++;

                //caller_id_name
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                    11, false, false, false);
                exRange.Value2 = item.CallerIdName;
                col++;

                //caller_id_type
                exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn(ConvertColumn(col), row), new RangeColumn(ConvertColumn(col), row), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                    11, false, false, false);
                exRange.Value2 = item.CallerIdType;
                col++;

                row++;
                col = dataColumnInitialize;//reset the column for the data
            }

            #endregion Data

        }

        #endregion Excel (1 method)

        #region Get Data (1 method)

        /// <summary>
        /// Calls Stored Procedure spDTDPOSIdReport as a Function Import on the EDMX
        /// </summary>
        /// <param name="sDate"></param>
        /// <param name="eDate"></param>
        /// <returns>List<spDTDPOSIdReport_Result> A list of values for the POS ID Report</returns>
        private static List<spDTDPOSIdReport_Result> GetDTDPOSIdReport(DateTime sDate, DateTime eDate)
        {
            List<spDTDPOSIdReport_Result> result = null;

            using (ConstellationEntities entities = new ConstellationEntities())
            {
                result = entities.spDTDPOSIdReport(startDate: sDate, endDate: eDate).ToList();
            }
            return result;
        }

        #endregion Get Data (1 method)

        #region Utilities
        /// <summary>
        /// Determines the pos_id_result based on the values passed in from infutor and experian
        /// </summary>
        /// <param name="infutor"></param>
        /// <param name="experian"></param>
        /// <returns></returns>
        private static string GetPos_id_result(string infutor, string experian)
        {
            string result = string.Empty;

            if (!IsValueNull(infutor))
            {
                if (infutor.ToLower() == "green")
                {
                    result = "GREEN";
                }
                else
                {
                    result = experian.ToUpper();//otherwise it is the experian value
                }
            }
            else
            {
                result = experian.ToUpper();//otherwise it is the experian value
            }


            return result;
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

                mail.Subject = "Constellation - Res - DTD - POS - ID - File Generation " + reportDate.ToString("MM-dd-yyyy") + ".";

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
        private static void SaveXlsDocument(ref string reportPath, ref string xlsFilename, ref string xlsFilePath, Excel.Workbook exBook, DateTime reportDate, string reportType)
        {

            //Build the file name

            if (reportType == "Internal")
            {
                //DER DTD and SC DTD PSA Results -June 2016_INTERNAL.xlsx
                xlsFilename = "DER DTD and SC DTD PSA Results -" + String.Format("{0:MMMM dd yyyy}", reportDate) + "_INTERNAL.xlsx";
            }
            else
            {
                //DER DTD and SC DTD PSA Results -June 2016.xlsx
                xlsFilename = "DER DTD and SC DTD PSA Results -" + String.Format("{0:MMMM dd yyyy}", reportDate) + ".xlsx";
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

            StartDate = new DateTime(baseDate.Year, baseDate.Month, baseDate.Day, 0, 0, 0).AddDays(-1);//Previous day
            EndDate = new DateTime(baseDate.Year, baseDate.Month, baseDate.Day, 0, 0, 0);//current date time as this runs for the previous day
        }
        #endregion Utilities

        #region Error Handling
        private static void SendErrorMessage(Exception ex)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendFormat("ex:{0}, innerEx:{1}", ex.Message, ex.InnerException == null ? "" : ex.InnerException.Message);

            Calibrus.ErrorHandler.Alerting alert = new Calibrus.ErrorHandler.Alerting("ConstellationDTDPOSIdReportXLS");
            alert.SendAlert(ex.Source, sb.ToString(), Environment.MachineName, Environment.UserName, Environment.Version.ToString());
        }
        #endregion Error Handling
    }
}
