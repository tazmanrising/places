using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Configuration;
using Calibrus.ExcelFunctions;
using Calibrus.Mail;
using Calibrus.ErrorHandler;
using Excel = Microsoft.Office.Interop.Excel;


namespace ConstellationExcelReports_POC
{
    class Reports
    {
        public static object na = System.Reflection.Missing.Value;

        #region Main
        public static void Main(string[] args)
        {
            string rootPath = string.Empty;
            string mailRecipientTO = string.Empty;

            //get report interval
            DateTime CurrentDate = new DateTime();



            //start to  build the form pathing
            string xlsFilename = string.Empty;
            string xlsFilePath = string.Empty;

            if (args.Length > 0)
            {
                if (DateTime.TryParse(args[0], out CurrentDate))
                {
                    GetDates(out CurrentDate);
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
                GetDates(out CurrentDate);
            }

            //grab values from app.config
            rootPath = ConfigurationManager.AppSettings["rootPath"].ToString();
            //mailRecipientTO = ConfigurationManager.AppSettings["mailRecipientTO"].ToString();

            List<Vendors> vendorsList = GetVendorList();

            //start Excel
            Excel.Application exApp = new Excel.Application();
            Excel.Workbook exBook = null;
            Excel.Worksheet exSheet = null;
            Excel.Range exRange = null;

            int sheetsAdded = 0;
            #region VendorForLoop
            foreach (Vendors vendor in vendorsList)
            {
                sheetsAdded = 0;

                try
                {
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

                    //Set global attributes
                    exApp.StandardFont = "Calibri";
                    exApp.StandardFontSize = 11;

                    //newtab
                    string sheetName = String.Format("{0}", "Agent Activity " + vendor.VendorId.ToString());
                    exSheet.Name = sheetName.Length > 30 ? sheetName.Substring(0, 30) : sheetName; //force length of sheet name due to excel constraints
                    exSheet.Select(na);

                    //write out Agent Activity Report
                    WriteReportAgentActivity(ref exApp, ref exRange, CurrentDate, vendor.VendorId.ToString());

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
                    sheetName = String.Format("{0}", "Daily Calls " + vendor.VendorId.ToString());
                    exSheet.Name = sheetName.Length > 30 ? sheetName.Substring(0, 30) : sheetName; //force length of sheet name due to excel constraints
                    exSheet.Select(na);

                    //write out Daily Calls Total
                    WriteReportDailyCalls(ref exApp, ref exRange, CurrentDate, vendor.VendorId.ToString());


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
                    sheetName = String.Format("{0}", "No Sales " + vendor.VendorId.ToString());
                    exSheet.Name = sheetName.Length > 30 ? sheetName.Substring(0, 30) : sheetName; //force length of sheet name due to excel constraints
                    exSheet.Select(na);

                    //write out No Sales Dispositions
                    WriteReportsDailyNoSales(ref exApp, ref exRange, CurrentDate, vendor.VendorId.ToString());

                    //select first sheet in workbook
                    exSheet = (Excel.Worksheet)exApp.Worksheets[String.Format("{0}", "Agent Activity " + vendor.VendorId.ToString())];
                    exSheet.Select(na);

                    //save report
                    SaveXlsDocument(ref rootPath, ref xlsFilename, ref xlsFilePath, exBook, CurrentDate, vendor.VendorId.ToString());

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
                SendEmail(ref xlsFilePath, CurrentDate, vendor.VendorId.ToString());

            }
            #endregion

            //Then write out for all the Vendors
            sheetsAdded = 0;

            try
            {
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


                //Set global attributes
                exApp.StandardFont = "Calibri";
                exApp.StandardFontSize = 11;


                //newtab
                string sheetName = String.Format("{0}", "Agent Activity Total");
                exSheet.Name = sheetName.Length > 30 ? sheetName.Substring(0, 30) : sheetName; //force length of sheet name due to excel constraints
                exSheet.Select(na);


                //write out Agent Activity Report
                WriteReportAgentActivity(ref exApp, ref exRange, CurrentDate);

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
                sheetName = String.Format("{0}", "Daily Calls Total");
                exSheet.Name = sheetName.Length > 30 ? sheetName.Substring(0, 30) : sheetName; //force length of sheet name due to excel constraints
                exSheet.Select(na);


                //write out Daily Calls Total
                WriteReportDailyCalls(ref exApp, ref exRange, CurrentDate);


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
                sheetName = String.Format("{0}", "No Sales Total");
                exSheet.Name = sheetName.Length > 30 ? sheetName.Substring(0, 30) : sheetName; //force length of sheet name due to excel constraints
                exSheet.Select(na);

                //write out No Sales Dispositions
                WriteReportsDailyNoSales(ref exApp, ref exRange, CurrentDate);

                exSheet = (Excel.Worksheet)exApp.Worksheets[String.Format("{0}", "Agent Activity Total")];
                exSheet.Select(na);

                //save report
                SaveXlsDocument(ref rootPath, ref xlsFilename, ref xlsFilePath, exBook, CurrentDate, "Total");

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
            SendEmail(ref xlsFilePath, CurrentDate, "Total");
        }




        #endregion

        #region Excel
        /// <summary>
        /// writes out AgentActivity for a specific vendor
        /// </summary>
        /// <param name="exApp"></param>
        /// <param name="exRange"></param>
        /// <param name="currentDate"></param>
        /// <param name="vendorid"></param>
        public static void WriteReportAgentActivity(ref Excel.Application exApp, ref Excel.Range exRange, DateTime currentDate, string vendorid)
        {

            #region Variables
            Excel.Font exFont = null;
            //Placeholders as I move through the Excel sheet
            int colCount = 0;
            int rowCount = 0;


            int headerColumnStart = 65; //A ascii value for conversion
            int headerRowStart = 1; //start of the headears for Data


            //start for Data  after the headers	
            int columnDataStart = 65; //A
            int rowDataStart = 2;


            #endregion

            #region Data Headers
            colCount = headerColumnStart;
            rowCount = headerRowStart;
            //Write out the Data  headers
            exRange = (Excel.Range)exApp.Cells[rowCount, ConvertColumn(colCount)];
            exRange.Value2 = "Employee ID";
            exRange.Interior.ColorIndex = 37;
            exFont = exRange.Font;
            exFont.ColorIndex = 1;
            exFont.Bold = true;
            colCount++;

            exRange = (Excel.Range)exApp.Cells[rowCount, ConvertColumn(colCount)];
            exRange.Value2 = "Name";
            exRange.Interior.ColorIndex = 37;
            exFont = exRange.Font;
            exFont.ColorIndex = 1;
            exFont.Bold = true;
            colCount++;

            exRange = (Excel.Range)exApp.Cells[rowCount, ConvertColumn(colCount)];
            exRange.Value2 = "Good Sales";
            exRange.Interior.ColorIndex = 37;
            exFont = exRange.Font;
            exFont.ColorIndex = 1;
            exFont.Bold = true;
            colCount++;

            exRange = (Excel.Range)exApp.Cells[rowCount, ConvertColumn(colCount)];
            exRange.Value2 = "No Sales";
            exRange.Interior.ColorIndex = 37;
            exFont = exRange.Font;
            exFont.ColorIndex = 1;
            exFont.Bold = true;
            colCount++;

            exRange = (Excel.Range)exApp.Cells[rowCount, ConvertColumn(colCount)];
            exRange.Value2 = "% No Sales";
            exRange.Interior.ColorIndex = 37;
            exFont = exRange.Font;
            exFont.ColorIndex = 1;
            exFont.Bold = true;
            colCount++;

            exRange = (Excel.Range)exApp.Cells[rowCount, ConvertColumn(colCount)];
            exRange.Value2 = "Total Sales";
            exRange.Interior.ColorIndex = 37;
            exFont = exRange.Font;
            exFont.ColorIndex = 1;
            exFont.Bold = true;

            #endregion

            #region DistinctAgents
            double allVerifieds = 0.0;
            double allFailed = 0.0;

            rowCount = rowDataStart;
            colCount = columnDataStart;
            foreach (var agent in GetAgentsList(vendorid, currentDate))
            {
                double verifieds = 0.0;
                double fails = 0.0;
                //verified
                verifieds = GetGoodSales(agent.AgentKeyId, currentDate);
                //failed
                fails = GetBadSales(agent.AgentKeyId, currentDate);


                //set the totals
                allVerifieds += verifieds;
                allFailed += fails;

                //Agent Id
                exRange = (Excel.Range)exApp.Cells[rowCount, ConvertColumn(colCount)];
                exRange.Value2 = agent.AgentId;
                exFont = exRange.Font;
                exFont.Bold = false;
                colCount++;

                //Agent Name
                exRange = (Excel.Range)exApp.Cells[rowCount, ConvertColumn(colCount)];
                exRange.Value2 = agent.AgentName;
                exFont = exRange.Font;
                exFont.Bold = false;
                colCount++;

                //Good Sales count
                exRange = (Excel.Range)exApp.Cells[rowCount, ConvertColumn(colCount)];
                exRange.Value2 = verifieds;
                exFont = exRange.Font;
                exFont.Bold = false;
                colCount++;

                //No Sales count
                exRange = (Excel.Range)exApp.Cells[rowCount, ConvertColumn(colCount)];
                exRange.Value2 = fails;
                exFont = exRange.Font;
                exFont.Bold = false;
                colCount++;

                //% No Sales
                exRange = (Excel.Range)exApp.Cells[rowCount, ConvertColumn(colCount)];

                if (fails == 0)
                { exRange.Value2 = 0; }
                else
                {
                    //double per = (fails / (fails + verifieds));
                    exRange.Value2 = (fails / (fails + verifieds));
                }


                exRange.NumberFormat = "0.00%";
                exFont = exRange.Font;
                exFont.Bold = false;
                colCount++;

                //Total Sales
                exRange = (Excel.Range)exApp.Cells[rowCount, ConvertColumn(colCount)];
                exRange.Value2 = fails + verifieds;
                exFont = exRange.Font;
                exFont.Bold = false;
                colCount = columnDataStart;

                rowCount++;

            }
            #endregion

            #region Total
            colCount = columnDataStart;

            colCount++;
            //Grand Totals
            exRange = (Excel.Range)exApp.Cells[rowCount, ConvertColumn(colCount)];
            exRange.Value2 = "Grand Totals";
            exFont = exRange.Font;
            exFont.Bold = true;
            colCount++;

            //Good Sales count
            exRange = (Excel.Range)exApp.Cells[rowCount, ConvertColumn(colCount)];
            exRange.Value2 = allVerifieds;
            exFont = exRange.Font;
            exFont.Bold = false;
            colCount++;

            //No Sales count
            exRange = (Excel.Range)exApp.Cells[rowCount, ConvertColumn(colCount)];
            exRange.Value2 = allFailed;
            exFont = exRange.Font;
            exFont.Bold = false;
            colCount++;

            //% No Sales
            exRange = (Excel.Range)exApp.Cells[rowCount, ConvertColumn(colCount)];

            if (allFailed == 0)
            { exRange.Value2 = 0; }
            else
            { exRange.Value2 = (allFailed / (allFailed + allVerifieds)); }

            exRange.NumberFormat = "0.00%";
            exFont = exRange.Font;
            exFont.Bold = false;
            colCount++;

            //Total Sales
            exRange = (Excel.Range)exApp.Cells[rowCount, ConvertColumn(colCount)];
            exRange.Value2 = allFailed + allVerifieds;
            exFont = exRange.Font;
            exFont.Bold = false;
            colCount++;

            #endregion
            exRange = (Excel.Range)exApp.get_Range("A1", "F1");
            exRange.EntireColumn.AutoFit();
 
        }
        /// <summary>
        /// writes out AgentActivity for ALL vendors
        /// </summary>
        /// <param name="exApp"></param>
        /// <param name="exRange"></param>
        /// <param name="currentDate"></param>
        /// <param name="vendorid"></param>
        public static void WriteReportAgentActivity(ref Excel.Application exApp, ref Excel.Range exRange, DateTime currentDate)
        {

            #region Variables
            Excel.Font exFont = null;
            //Placeholders as I move through the Excel sheet
            int colCount = 0;
            int rowCount = 0;


            int headerColumnStart = 65; //A ascii value for conversion
            int headerRowStart = 1; //start of the headears for Data


            //start for Data  after the headers	
            int columnDataStart = 65; //A
            int rowDataStart = 2;


            #endregion

            #region Data Headers
            colCount = headerColumnStart;
            rowCount = headerRowStart;
            //Write out the Data  headers
            exRange = (Excel.Range)exApp.Cells[rowCount, ConvertColumn(colCount)];
            exRange.Value2 = "Employee ID";
            exRange.Interior.ColorIndex = 37;
            exFont = exRange.Font;
            exFont.ColorIndex = 1;
            exFont.Bold = true;
            colCount++;

            exRange = (Excel.Range)exApp.Cells[rowCount, ConvertColumn(colCount)];
            exRange.Value2 = "Name";
            exRange.Interior.ColorIndex = 37;
            exFont = exRange.Font;
            exFont.ColorIndex = 1;
            exFont.Bold = true;
            colCount++;

            exRange = (Excel.Range)exApp.Cells[rowCount, ConvertColumn(colCount)];
            exRange.Value2 = "Good Sales";
            exRange.Interior.ColorIndex = 37;
            exFont = exRange.Font;
            exFont.ColorIndex = 1;
            exFont.Bold = true;
            colCount++;

            exRange = (Excel.Range)exApp.Cells[rowCount, ConvertColumn(colCount)];
            exRange.Value2 = "No Sales";
            exRange.Interior.ColorIndex = 37;
            exFont = exRange.Font;
            exFont.ColorIndex = 1;
            exFont.Bold = true;
            colCount++;

            exRange = (Excel.Range)exApp.Cells[rowCount, ConvertColumn(colCount)];
            exRange.Value2 = "% No Sales";
            exRange.Interior.ColorIndex = 37;
            exFont = exRange.Font;
            exFont.ColorIndex = 1;
            exFont.Bold = true;
            colCount++;

            exRange = (Excel.Range)exApp.Cells[rowCount, ConvertColumn(colCount)];
            exRange.Value2 = "Total Sales";
            exRange.Interior.ColorIndex = 37;
            exFont = exRange.Font;
            exFont.ColorIndex = 1;
            exFont.Bold = true;

            #endregion

            #region DistinctAgents
            double allVerifieds = 0.0;
            double allFailed = 0.0;

            rowCount = rowDataStart;
            colCount = columnDataStart;
            foreach (var agent in GetAgentsList(currentDate))
            {
                double verifieds = 0.0;
                double fails = 0.0;
                //verified
                verifieds = GetGoodSales(agent.AgentKeyId, currentDate);
                //failed
                fails = GetBadSales(agent.AgentKeyId, currentDate);


                //set the totals
                allVerifieds += verifieds;
                allFailed += fails;

                //Agent Id
                exRange = (Excel.Range)exApp.Cells[rowCount, ConvertColumn(colCount)];
                exRange.Value2 = agent.AgentId;
                exFont = exRange.Font;
                exFont.Bold = false;
                colCount++;

                //Agent Name
                exRange = (Excel.Range)exApp.Cells[rowCount, ConvertColumn(colCount)];
                exRange.Value2 = agent.AgentName;
                exFont = exRange.Font;
                exFont.Bold = false;
                colCount++;

                //Good Sales count
                exRange = (Excel.Range)exApp.Cells[rowCount, ConvertColumn(colCount)];
                exRange.Value2 = verifieds;
                exFont = exRange.Font;
                exFont.Bold = false;
                colCount++;

                //No Sales count
                exRange = (Excel.Range)exApp.Cells[rowCount, ConvertColumn(colCount)];
                exRange.Value2 = fails;
                exFont = exRange.Font;
                exFont.Bold = false;
                colCount++;

                //% No Sales
                exRange = (Excel.Range)exApp.Cells[rowCount, ConvertColumn(colCount)];
                if (fails == 0)
                { exRange.Value2 = 0; }
                else
                { exRange.Value2 = (fails / (fails + verifieds)); }
                exRange.NumberFormat = "0.00%";
                exFont = exRange.Font;
                exFont.Bold = false;
                colCount++;

                //Total Sales
                exRange = (Excel.Range)exApp.Cells[rowCount, ConvertColumn(colCount)];
                exRange.Value2 = fails + verifieds;
                exFont = exRange.Font;
                exFont.Bold = false;
                colCount = columnDataStart;

                rowCount++;

            }
            #endregion

            #region Total
            colCount = columnDataStart;

            colCount++;
            //Grand Totals
            exRange = (Excel.Range)exApp.Cells[rowCount, ConvertColumn(colCount)];
            exRange.Value2 = "Grand Totals";
            exFont = exRange.Font;
            exFont.Bold = true;
            colCount++;

            //Good Sales count
            exRange = (Excel.Range)exApp.Cells[rowCount, ConvertColumn(colCount)];
            exRange.Value2 = allVerifieds;
            exFont = exRange.Font;
            exFont.Bold = false;
            colCount++;

            //No Sales count
            exRange = (Excel.Range)exApp.Cells[rowCount, ConvertColumn(colCount)];
            exRange.Value2 = allFailed;
            exFont = exRange.Font;
            exFont.Bold = false;
            colCount++;

            //% No Sales
            exRange = (Excel.Range)exApp.Cells[rowCount, ConvertColumn(colCount)];
            if (allFailed == 0)
            { exRange.Value2 = 0; }
            else
            { exRange.Value2 = (allFailed / (allFailed + allVerifieds)); }
            exRange.NumberFormat = "0.00%";
            exFont = exRange.Font;
            exFont.Bold = false;
            colCount++;

            //Total Sales
            exRange = (Excel.Range)exApp.Cells[rowCount, ConvertColumn(colCount)];
            exRange.Value2 = allFailed + allVerifieds;
            exFont = exRange.Font;
            exFont.Bold = false;
            colCount++;

            #endregion
            exRange = (Excel.Range)exApp.get_Range("A1", "F1");
            exRange.EntireColumn.AutoFit();
        }


        /// <summary>
        /// writes out AgentActivity for a specific vendor
        /// </summary>
        /// <param name="exApp"></param>
        /// <param name="exRange"></param>
        /// <param name="currentDate"></param>
        /// <param name="vendorid"></param>
        public static void WriteReportDailyCalls(ref Excel.Application exApp, ref Excel.Range exRange, DateTime currentDate, string vendorid)
        {

            #region Variables
            Excel.Font exFont = null;
            //Placeholders as I move through the Excel sheet
            int colCount = 0;
            int rowCount = 0;


            int headerColumnStart = 65; //A ascii value for conversion
            int headerRowStart = 1; //start of the headears for Data


            //start for Data  after the headers	
            int columnDataStart = 65; //A
            int rowDataStart = 2;


            #endregion

            #region Data Headers
            colCount = headerColumnStart;
            rowCount = headerRowStart;
            //Write out the Data  headers
            exRange = (Excel.Range)exApp.Cells[rowCount, ConvertColumn(colCount)];
            exRange.Value2 = "Date";
            exRange.Interior.ColorIndex = 37;
            exFont = exRange.Font;
            exFont.ColorIndex = 1;
            exFont.Bold = true;
            colCount++;

            exRange = (Excel.Range)exApp.Cells[rowCount, ConvertColumn(colCount)];
            exRange.Value2 = "Good Sales";
            exRange.Interior.ColorIndex = 37;
            exFont = exRange.Font;
            exFont.ColorIndex = 1;
            exFont.Bold = true;
            colCount++;

            exRange = (Excel.Range)exApp.Cells[rowCount, ConvertColumn(colCount)];
            exRange.Value2 = "No Sales";
            exRange.Interior.ColorIndex = 37;
            exFont = exRange.Font;
            exFont.ColorIndex = 1;
            exFont.Bold = true;
            colCount++;

            exRange = (Excel.Range)exApp.Cells[rowCount, ConvertColumn(colCount)];
            exRange.Value2 = "% No Sales";
            exRange.Interior.ColorIndex = 37;
            exFont = exRange.Font;
            exFont.ColorIndex = 1;
            exFont.Bold = true;
            colCount++;

            exRange = (Excel.Range)exApp.Cells[rowCount, ConvertColumn(colCount)];
            exRange.Value2 = "Total Sales";
            exRange.Interior.ColorIndex = 37;
            exFont = exRange.Font;
            exFont.ColorIndex = 1;
            exFont.Bold = true;

            #endregion

            #region DistinctAgents
            double allVerifieds = 0.0;
            double allFailed = 0.0;


            foreach (var agent in GetAgentsList(vendorid, currentDate))
            {
                double verifieds = 0.0;
                double fails = 0.0;
                //verified
                verifieds = GetGoodSales(agent.AgentKeyId, currentDate);
                //failed
                fails = GetBadSales(agent.AgentKeyId, currentDate);


                //set the totals
                allVerifieds += verifieds;
                allFailed += fails;


            }
            #endregion


            rowCount = rowDataStart;
            colCount = columnDataStart;


            //Date 
            exRange = (Excel.Range)exApp.Cells[rowCount, ConvertColumn(colCount)];
            exRange.Value2 = currentDate.AddDays(-1).ToString("dddd, dd MMMM yyyy"); //subtract day for yesterday
            exFont = exRange.Font;
            exFont.Bold = true;
            colCount++;


            //Good Sales count
            exRange = (Excel.Range)exApp.Cells[rowCount, ConvertColumn(colCount)];
            exRange.Value2 = allVerifieds;
            exFont = exRange.Font;
            exFont.Bold = false;
            colCount++;

            //No Sales count
            exRange = (Excel.Range)exApp.Cells[rowCount, ConvertColumn(colCount)];
            exRange.Value2 = allFailed;
            exFont = exRange.Font;
            exFont.Bold = false;
            colCount++;

            //% No Sales
            exRange = (Excel.Range)exApp.Cells[rowCount, ConvertColumn(colCount)];

            if (allFailed == 0)
            { exRange.Value2 = 0; }
            else
            { exRange.Value2 = (allFailed / (allFailed + allVerifieds)); }

            exRange.NumberFormat = "0.00%";
            exFont = exRange.Font;
            exFont.Bold = false;
            colCount++;

            //Total Sales
            exRange = (Excel.Range)exApp.Cells[rowCount, ConvertColumn(colCount)];
            exRange.Value2 = allFailed + allVerifieds;
            exFont = exRange.Font;
            exFont.Bold = false;
            rowCount++;


            #region Total
            colCount = columnDataStart;


            //Grand Totals
            exRange = (Excel.Range)exApp.Cells[rowCount, ConvertColumn(colCount)];
            exRange.Value2 = "Grand Totals";
            exFont = exRange.Font;
            exFont.Bold = true;
            colCount++;

            //Good Sales count
            exRange = (Excel.Range)exApp.Cells[rowCount, ConvertColumn(colCount)];
            exRange.Value2 = allVerifieds;
            exFont = exRange.Font;
            exFont.Bold = false;
            colCount++;

            //No Sales count
            exRange = (Excel.Range)exApp.Cells[rowCount, ConvertColumn(colCount)];
            exRange.Value2 = allFailed;
            exFont = exRange.Font;
            exFont.Bold = false;
            colCount++;

            colCount++;

            //Total Sales
            exRange = (Excel.Range)exApp.Cells[rowCount, ConvertColumn(colCount)];
            exRange.Value2 = allFailed + allVerifieds;
            exFont = exRange.Font;
            exFont.Bold = false;
            rowCount++;

            colCount = columnDataStart;


            //Grand Totals
            exRange = (Excel.Range)exApp.Cells[rowCount, ConvertColumn(colCount)];
            exRange.Value2 = "Daily Averagess";
            exFont = exRange.Font;
            exFont.Bold = true;
            colCount++;

            //Good Sales count
            exRange = (Excel.Range)exApp.Cells[rowCount, ConvertColumn(colCount)];
            exRange.Value2 = allVerifieds;
            exRange.NumberFormat = "0.00";
            exFont = exRange.Font;
            exFont.Bold = false;
            colCount++;

            //No Sales count
            exRange = (Excel.Range)exApp.Cells[rowCount, ConvertColumn(colCount)];
            exRange.Value2 = allFailed;
            exRange.NumberFormat = "0.00";
            exFont = exRange.Font;
            exFont.Bold = false;
            colCount++;

            //% No Sales
            exRange = (Excel.Range)exApp.Cells[rowCount, ConvertColumn(colCount)];

            if (allFailed == 0)
            { exRange.Value2 = 0; }
            else
            { exRange.Value2 = (allFailed / (allFailed + allVerifieds)); }

            exRange.NumberFormat = "0.00%";
            exFont = exRange.Font;
            exFont.Bold = false;
            colCount++;

            //Total Sales
            exRange = (Excel.Range)exApp.Cells[rowCount, ConvertColumn(colCount)];
            exRange.Value2 = allFailed + allVerifieds;
            exRange.NumberFormat = "0.00";
            exFont = exRange.Font;
            exFont.Bold = false;
            colCount++;


            #endregion
            exRange = (Excel.Range)exApp.get_Range("A1", "F1");
            exRange.EntireColumn.AutoFit();
        }
        /// <summary>
        /// writes out AgentActivity for ALL vendors
        /// </summary>
        /// <param name="exApp"></param>
        /// <param name="exRange"></param>
        /// <param name="currentDate"></param>
        /// <param name="vendorid"></param>
        public static void WriteReportDailyCalls(ref Excel.Application exApp, ref Excel.Range exRange, DateTime currentDate)
        {

            #region Variables
            Excel.Font exFont = null;
            //Placeholders as I move through the Excel sheet
            int colCount = 0;
            int rowCount = 0;


            int headerColumnStart = 65; //A ascii value for conversion
            int headerRowStart = 1; //start of the headears for Data


            //start for Data  after the headers	
            int columnDataStart = 65; //A
            int rowDataStart = 2;


            #endregion

            #region Data Headers
            colCount = headerColumnStart;
            rowCount = headerRowStart;
            //Write out the Data  headers
            exRange = (Excel.Range)exApp.Cells[rowCount, ConvertColumn(colCount)];
            exRange.Value2 = "Date";
            exRange.Interior.ColorIndex = 37;
            exFont = exRange.Font;
            exFont.ColorIndex = 1;
            exFont.Bold = true;
            colCount++;

            exRange = (Excel.Range)exApp.Cells[rowCount, ConvertColumn(colCount)];
            exRange.Value2 = "Good Sales";
            exRange.Interior.ColorIndex = 37;
            exFont = exRange.Font;
            exFont.ColorIndex = 1;
            exFont.Bold = true;
            colCount++;

            exRange = (Excel.Range)exApp.Cells[rowCount, ConvertColumn(colCount)];
            exRange.Value2 = "No Sales";
            exRange.Interior.ColorIndex = 37;
            exFont = exRange.Font;
            exFont.ColorIndex = 1;
            exFont.Bold = true;
            colCount++;

            exRange = (Excel.Range)exApp.Cells[rowCount, ConvertColumn(colCount)];
            exRange.Value2 = "% No Sales";
            exRange.Interior.ColorIndex = 37;
            exFont = exRange.Font;
            exFont.ColorIndex = 1;
            exFont.Bold = true;
            colCount++;

            exRange = (Excel.Range)exApp.Cells[rowCount, ConvertColumn(colCount)];
            exRange.Value2 = "Total Sales";
            exRange.Interior.ColorIndex = 37;
            exFont = exRange.Font;
            exFont.ColorIndex = 1;
            exFont.Bold = true;

            #endregion

            #region DistinctAgents
            double allVerifieds = 0.0;
            double allFailed = 0.0;


            foreach (var agent in GetAgentsList(currentDate))
            {
                double verifieds = 0.0;
                double fails = 0.0;
                //verified
                verifieds = GetGoodSales(agent.AgentKeyId, currentDate);
                //failed
                fails = GetBadSales(agent.AgentKeyId, currentDate);


                //set the totals
                allVerifieds += verifieds;
                allFailed += fails;


            }
            #endregion


            rowCount = rowDataStart;
            colCount = columnDataStart;


            //Date 
            exRange = (Excel.Range)exApp.Cells[rowCount, ConvertColumn(colCount)];
            exRange.Value2 = currentDate.AddDays(-1).ToString("dddd, dd MMMM yyyy"); //subtract day for the previous day
            exFont = exRange.Font;
            exFont.Bold = true;
            colCount++;


            //Good Sales count
            exRange = (Excel.Range)exApp.Cells[rowCount, ConvertColumn(colCount)];
            exRange.Value2 = allVerifieds;
            exFont = exRange.Font;
            exFont.Bold = false;
            colCount++;

            //No Sales count
            exRange = (Excel.Range)exApp.Cells[rowCount, ConvertColumn(colCount)];
            exRange.Value2 = allFailed;
            exFont = exRange.Font;
            exFont.Bold = false;
            colCount++;

            //% No Sales
            exRange = (Excel.Range)exApp.Cells[rowCount, ConvertColumn(colCount)];

            if (allFailed == 0)
            { exRange.Value2 = 0; }
            else
            { exRange.Value2 = (allFailed / (allFailed + allVerifieds)); }

            exRange.NumberFormat = "0.00%";
            exFont = exRange.Font;
            exFont.Bold = false;
            colCount++;

            //Total Sales
            exRange = (Excel.Range)exApp.Cells[rowCount, ConvertColumn(colCount)];
            exRange.Value2 = allFailed + allVerifieds;
            exFont = exRange.Font;
            exFont.Bold = false;
            rowCount++;


            #region Total
            colCount = columnDataStart;


            //Grand Totals
            exRange = (Excel.Range)exApp.Cells[rowCount, ConvertColumn(colCount)];
            exRange.Value2 = "Grand Totals";
            exFont = exRange.Font;
            exFont.Bold = true;
            colCount++;

            //Good Sales count
            exRange = (Excel.Range)exApp.Cells[rowCount, ConvertColumn(colCount)];
            exRange.Value2 = allVerifieds;
            exFont = exRange.Font;
            exFont.Bold = false;
            colCount++;

            //No Sales count
            exRange = (Excel.Range)exApp.Cells[rowCount, ConvertColumn(colCount)];
            exRange.Value2 = allFailed;
            exFont = exRange.Font;
            exFont.Bold = false;
            colCount++;

            colCount++;

            //Total Sales
            exRange = (Excel.Range)exApp.Cells[rowCount, ConvertColumn(colCount)];
            exRange.Value2 = allFailed + allVerifieds;
            exFont = exRange.Font;
            exFont.Bold = false;
            rowCount++;

            colCount = columnDataStart;


            //Grand Totals
            exRange = (Excel.Range)exApp.Cells[rowCount, ConvertColumn(colCount)];
            exRange.Value2 = "Daily Averagess";
            exFont = exRange.Font;
            exFont.Bold = true;
            colCount++;

            //Good Sales count
            exRange = (Excel.Range)exApp.Cells[rowCount, ConvertColumn(colCount)];
            exRange.Value2 = allVerifieds;
            exRange.NumberFormat = "0.00";
            exFont = exRange.Font;
            exFont.Bold = false;
            colCount++;

            //No Sales count
            exRange = (Excel.Range)exApp.Cells[rowCount, ConvertColumn(colCount)];
            exRange.Value2 = allFailed;
            exRange.NumberFormat = "0.00";
            exFont = exRange.Font;
            exFont.Bold = false;
            colCount++;

            //% No Sales
            exRange = (Excel.Range)exApp.Cells[rowCount, ConvertColumn(colCount)];

            if (allFailed == 0)
            { exRange.Value2 = 0; }
            else
            { exRange.Value2 = (allFailed / (allFailed + allVerifieds)); }

            exRange.NumberFormat = "0.00%";
            exFont = exRange.Font;
            exFont.Bold = false;
            colCount++;

            //Total Sales
            exRange = (Excel.Range)exApp.Cells[rowCount, ConvertColumn(colCount)];
            exRange.Value2 = allFailed + allVerifieds;
            exRange.NumberFormat = "0.00";
            exFont = exRange.Font;
            exFont.Bold = false;
            colCount++;


            #endregion
            exRange = (Excel.Range)exApp.get_Range("A1", "F1");
            exRange.EntireColumn.AutoFit();
        }


        /// <summary>
        /// writes out all No Sales (faile dispositions) for a specific vendor
        /// </summary>
        /// <param name="exApp"></param>
        /// <param name="exRange"></param>
        /// <param name="currentDate"></param>
        /// <param name="vendorid"></param>
        public static void WriteReportsDailyNoSales(ref Excel.Application exApp, ref Excel.Range exRange, DateTime currentDate, string vendorid)
        {

            #region Variables
            Excel.Font exFont = null;
            //Placeholders as I move through the Excel sheet
            int colCount = 0;
            int rowCount = 0;


            int headerColumnStart = 65; //A ascii value for conversion
            int headerRowStart = 1; //start of the headears for Data


            //start for Data  after the headers	
            int columnDataStart = 65; //A
            int rowDataStart = 2;


            #endregion

            #region Data Headers
            colCount = headerColumnStart;
            rowCount = headerRowStart;

            //Write out the Data  headers
            exRange = (Excel.Range)exApp.Cells[rowCount, ConvertColumn(colCount)];
            exRange.Value2 = "Total Contacts";
            exRange.Interior.ColorIndex = 37;
            exFont = exRange.Font;
            exFont.ColorIndex = 1;
            exFont.Bold = true;
            colCount++;

            exRange = (Excel.Range)exApp.Cells[rowCount, ConvertColumn(colCount)];
            exRange.Value2 = "Good Sales";
            exRange.Interior.ColorIndex = 37;
            exFont = exRange.Font;
            exFont.ColorIndex = 1;
            exFont.Bold = true;
            colCount++;

            exRange = (Excel.Range)exApp.Cells[rowCount, ConvertColumn(colCount)];
            exRange.Value2 = "No Sales";
            exRange.Interior.ColorIndex = 37;
            exFont = exRange.Font;
            exFont.ColorIndex = 1;
            exFont.Bold = true;
            colCount++;

            exRange = (Excel.Range)exApp.Cells[rowCount, ConvertColumn(colCount)];
            exRange.Value2 = "% Good Sales";
            exRange.Interior.ColorIndex = 37;
            exFont = exRange.Font;
            exFont.ColorIndex = 1;
            exFont.Bold = true;
            colCount++;

            exRange = (Excel.Range)exApp.Cells[rowCount, ConvertColumn(colCount)];
            exRange.Value2 = "% No Sales";
            exRange.Interior.ColorIndex = 37;
            exFont = exRange.Font;
            exFont.ColorIndex = 1;
            exFont.Bold = true;

            #endregion

            #region DistinctAgents
            double allVerifieds = 0.0;
            double allFailed = 0.0;

            rowCount = rowDataStart;
            colCount = columnDataStart;
            foreach (var agent in GetAgentsList(vendorid, currentDate))
            {
                double verifieds = 0.0;
                double fails = 0.0;
                //verified
                verifieds = GetGoodSales(agent.AgentKeyId, currentDate);
                //failed
                fails = GetBadSales(agent.AgentKeyId, currentDate);


                //set the totals
                allVerifieds += verifieds;
                allFailed += fails;

            }
            #endregion

            #region Total

            //Grand Totals
            exRange = (Excel.Range)exApp.Cells[rowCount, ConvertColumn(colCount)];
            exRange.Value2 = allVerifieds + allFailed;
            exFont = exRange.Font;
            exFont.Bold = true;
            colCount++;

            //Good Sales count
            exRange = (Excel.Range)exApp.Cells[rowCount, ConvertColumn(colCount)];
            exRange.Value2 = allVerifieds;
            exFont = exRange.Font;
            exFont.Bold = false;
            colCount++;

            //No Sales count
            exRange = (Excel.Range)exApp.Cells[rowCount, ConvertColumn(colCount)];
            exRange.Value2 = allFailed;
            exFont = exRange.Font;
            exFont.Bold = false;
            colCount++;

            //% Good Sales
            exRange = (Excel.Range)exApp.Cells[rowCount, ConvertColumn(colCount)];

            if (allVerifieds == 0)
            { exRange.Value2 = 0; }
            else
            { exRange.Value2 = (allVerifieds / (allFailed + allVerifieds)); }

            exRange.NumberFormat = "0.00%";
            exFont = exRange.Font;
            exFont.Bold = false;
            colCount++;

            //% No Sales
            exRange = (Excel.Range)exApp.Cells[rowCount, ConvertColumn(colCount)];

            if (allFailed == 0)
            { exRange.Value2 = 0; }
            else
            { exRange.Value2 = (allFailed / (allFailed + allVerifieds)); }

            exRange.NumberFormat = "0.00%";
            exFont = exRange.Font;
            exFont.Bold = false;
            colCount++;

            rowCount++;
            #endregion


            #region dispositionlist
            rowCount++;
            colCount = columnDataStart;

            exRange = (Excel.Range)exApp.Cells[rowCount, ConvertColumn(colCount)];
            exRange.Value2 = "Reason for No Sale";
            exFont = exRange.Font;
            exFont.Bold = true;
            colCount++;

            exRange = (Excel.Range)exApp.Cells[rowCount, ConvertColumn(colCount)];
            exRange.Value2 = "Count";
            exFont = exRange.Font;
            exFont.Bold = true;
            colCount++;

            exRange = (Excel.Range)exApp.Cells[rowCount, ConvertColumn(colCount)];
            exRange.Value2 = "Percent of No Sales";
            exFont = exRange.Font;
            exFont.Bold = true;
            colCount++;

            exRange = (Excel.Range)exApp.Cells[rowCount, ConvertColumn(colCount)];
            exRange.Value2 = "Percent of Total Contacts";
            exFont = exRange.Font;
            exFont.Bold = true;

            rowCount++;
            colCount = columnDataStart;

            //get concernlist with count
            foreach (Dispositions disp in GetDispositions(vendorid, currentDate))
            {
                //Disposition
                exRange = (Excel.Range)exApp.Cells[rowCount, ConvertColumn(colCount)];
                exRange.Value2 = "No Sale - " + disp.Disposition;
                exFont = exRange.Font;
                exFont.Bold = true;
                colCount++;

                //Count
                exRange = (Excel.Range)exApp.Cells[rowCount, ConvertColumn(colCount)];
                exRange.Value2 = disp.Count;
                exFont = exRange.Font;
                exFont.Bold = true;
                colCount++;

                //% of No Sales
                exRange = (Excel.Range)exApp.Cells[rowCount, ConvertColumn(colCount)];
                if (disp.Count == 0)
                { exRange.Value2 = 0; }
                else
                { exRange.Value2 = (disp.Count / allFailed); }

                exRange.NumberFormat = "0.00%";
                exFont = exRange.Font;
                exFont.Bold = true;
                colCount++;

                //% of Total Contacts
                exRange = (Excel.Range)exApp.Cells[rowCount, ConvertColumn(colCount)];
                if (disp.Count == 0)
                { exRange.Value2 = 0; }
                else
                { exRange.Value2 = (disp.Count / (allFailed + allVerifieds)); }

                exRange.NumberFormat = "0.00%";
                exFont = exRange.Font;
                exFont.Bold = true;
                colCount = columnDataStart;
                rowCount++;
            }

            #endregion
            exRange = (Excel.Range)exApp.get_Range("A1", "F1");
            exRange.EntireColumn.AutoFit();
        }


        /// <summary>
        /// writes out all No Sales (faile dispositions) for ALL vendor
        /// </summary>
        /// <param name="exApp"></param>
        /// <param name="exRange"></param>
        /// <param name="currentDate"></param>
        /// <param name="vendorid"></param>
        public static void WriteReportsDailyNoSales(ref Excel.Application exApp, ref Excel.Range exRange, DateTime currentDate)
        {

            #region Variables
            Excel.Font exFont = null;
            //Placeholders as I move through the Excel sheet
            int colCount = 0;
            int rowCount = 0;


            int headerColumnStart = 65; //A ascii value for conversion
            int headerRowStart = 1; //start of the headears for Data


            //start for Data  after the headers	
            int columnDataStart = 65; //A
            int rowDataStart = 2;


            #endregion

            #region Data Headers
            colCount = headerColumnStart;
            rowCount = headerRowStart;

            //Write out the Data  headers
            exRange = (Excel.Range)exApp.Cells[rowCount, ConvertColumn(colCount)];
            exRange.Value2 = "Total Contacts";
            exRange.Interior.ColorIndex = 37;
            exFont = exRange.Font;
            exFont.ColorIndex = 1;
            exFont.Bold = true;
            colCount++;

            exRange = (Excel.Range)exApp.Cells[rowCount, ConvertColumn(colCount)];
            exRange.Value2 = "Good Sales";
            exRange.Interior.ColorIndex = 37;
            exFont = exRange.Font;
            exFont.ColorIndex = 1;
            exFont.Bold = true;
            colCount++;

            exRange = (Excel.Range)exApp.Cells[rowCount, ConvertColumn(colCount)];
            exRange.Value2 = "No Sales";
            exRange.Interior.ColorIndex = 37;
            exFont = exRange.Font;
            exFont.ColorIndex = 1;
            exFont.Bold = true;
            colCount++;

            exRange = (Excel.Range)exApp.Cells[rowCount, ConvertColumn(colCount)];
            exRange.Value2 = "% Good Sales";
            exRange.Interior.ColorIndex = 37;
            exFont = exRange.Font;
            exFont.ColorIndex = 1;
            exFont.Bold = true;
            colCount++;

            exRange = (Excel.Range)exApp.Cells[rowCount, ConvertColumn(colCount)];
            exRange.Value2 = "% No Sales";
            exRange.Interior.ColorIndex = 37;
            exFont = exRange.Font;
            exFont.ColorIndex = 1;
            exFont.Bold = true;

            #endregion

            #region DistinctAgents
            double allVerifieds = 0.0;
            double allFailed = 0.0;

            rowCount = rowDataStart;
            colCount = columnDataStart;
            foreach (var agent in GetAgentsList(currentDate))
            {
                double verifieds = 0.0;
                double fails = 0.0;
                //verified
                verifieds = GetGoodSales(agent.AgentKeyId, currentDate);
                //failed
                fails = GetBadSales(agent.AgentKeyId, currentDate);


                //set the totals
                allVerifieds += verifieds;
                allFailed += fails;

            }
            #endregion

            #region Total

            //Grand Totals
            exRange = (Excel.Range)exApp.Cells[rowCount, ConvertColumn(colCount)];
            exRange.Value2 = allVerifieds + allFailed;
            exFont = exRange.Font;
            exFont.Bold = true;
            colCount++;

            //Good Sales count
            exRange = (Excel.Range)exApp.Cells[rowCount, ConvertColumn(colCount)];
            exRange.Value2 = allVerifieds;
            exFont = exRange.Font;
            exFont.Bold = false;
            colCount++;

            //No Sales count
            exRange = (Excel.Range)exApp.Cells[rowCount, ConvertColumn(colCount)];
            exRange.Value2 = allFailed;
            exFont = exRange.Font;
            exFont.Bold = false;
            colCount++;

            //% Good Sales
            exRange = (Excel.Range)exApp.Cells[rowCount, ConvertColumn(colCount)];

            if (allVerifieds == 0)
            { exRange.Value2 = 0; }
            else
            { exRange.Value2 = (allVerifieds / (allFailed + allVerifieds)); }

            exRange.NumberFormat = "0.00%";
            exFont = exRange.Font;
            exFont.Bold = false;
            colCount++;

            //% No Sales
            exRange = (Excel.Range)exApp.Cells[rowCount, ConvertColumn(colCount)];

            if (allFailed == 0)
            { exRange.Value2 = 0; }
            else
            { exRange.Value2 = (allFailed / (allFailed + allVerifieds)); }

            exRange.NumberFormat = "0.00%";
            exFont = exRange.Font;
            exFont.Bold = false;
            colCount++;

            rowCount++;
            #endregion


            #region dispositionlist
            rowCount++;
            colCount = columnDataStart;

            exRange = (Excel.Range)exApp.Cells[rowCount, ConvertColumn(colCount)];
            exRange.Value2 = "Reason for No Sale";
            exFont = exRange.Font;
            exFont.Bold = true;
            colCount++;

            exRange = (Excel.Range)exApp.Cells[rowCount, ConvertColumn(colCount)];
            exRange.Value2 = "Count";
            exFont = exRange.Font;
            exFont.Bold = true;
            colCount++;

            exRange = (Excel.Range)exApp.Cells[rowCount, ConvertColumn(colCount)];
            exRange.Value2 = "Percent of No Sales";
            exFont = exRange.Font;
            exFont.Bold = true;
            colCount++;

            exRange = (Excel.Range)exApp.Cells[rowCount, ConvertColumn(colCount)];
            exRange.Value2 = "Percent of Total Contacts";
            exFont = exRange.Font;
            exFont.Bold = true;

            rowCount++;
            colCount = columnDataStart;

            //get concernlist with count
            foreach (Dispositions disp in GetDispositions(currentDate))
            {
                //Disposition
                exRange = (Excel.Range)exApp.Cells[rowCount, ConvertColumn(colCount)];
                exRange.Value2 = "No Sale - " + disp.Disposition;
                exFont = exRange.Font;
                exFont.Bold = true;
                colCount++;

                //Count
                exRange = (Excel.Range)exApp.Cells[rowCount, ConvertColumn(colCount)];
                exRange.Value2 = disp.Count;
                exFont = exRange.Font;
                exFont.Bold = true;
                colCount++;

                //% of No Sales
                exRange = (Excel.Range)exApp.Cells[rowCount, ConvertColumn(colCount)];
                if (disp.Count == 0)
                { exRange.Value2 = 0; }
                else
                { exRange.Value2 = (disp.Count / allFailed); }

                exRange.NumberFormat = "0.00%";
                exFont = exRange.Font;
                exFont.Bold = true;
                colCount++;

                //% of Total Contacts
                exRange = (Excel.Range)exApp.Cells[rowCount, ConvertColumn(colCount)];
                if (disp.Count == 0)
                { exRange.Value2 = 0; }
                else
                { exRange.Value2 = (disp.Count / (allFailed + allVerifieds)); }

                exRange.NumberFormat = "0.00%";
                exFont = exRange.Font;
                exFont.Bold = true;
                colCount = columnDataStart;
                rowCount++;
            }

            #endregion
            exRange = (Excel.Range)exApp.get_Range("A1", "F1");
            exRange.EntireColumn.AutoFit();
        }


        #endregion

        #region GetData
        /// <summary>
        /// Gets list of Vendors
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

        /// <summary>
        /// Gets a distinct list of agents based on vendorId
        /// </summary>
        /// <param name="vendorId"></param>
        /// <returns></returns>
        private static List<Agents> GetAgentsList(string vendorId, DateTime cDate)
        {
            DateTime sDate = cDate.AddDays(-1);
            DateTime eDate = cDate;

            List<Agents> agents = new List<Agents>();
            using (ConstellationEntities entitites = new ConstellationEntities())
            {
                var query = (from m in entitites.tblMains
                             join a in entitites.tblAgents 
                             on m.tblAgentKeyId equals a.KeyId
                             where m.Concern != null
                             && m.CallDateTime > sDate
                             && m.CallDateTime < eDate
                             && m.VendorId == vendorId
                             select new { a.KeyId, a.AgentId, a.FirstName, a.LastName }).Distinct();


                foreach (var a in query)
                {
                    Agents agent = new Agents(a.KeyId,a.AgentId,  a.FirstName + " " + a.LastName);
                    agents.Add(agent);
                }
            }


            return agents;

        }

        /// <summary>
        /// Gets a distinct list of agents for all vendors
        /// </summary>
        /// <returns></returns>
        private static List<Agents> GetAgentsList(DateTime cDate)
        {
            DateTime sDate = cDate.AddDays(-1);
            DateTime eDate = cDate;

            List<Agents> agents = new List<Agents>();
            using (ConstellationEntities entitites = new ConstellationEntities())
            {
                var query = (from m in entitites.tblMains
                             join a in entitites.tblAgents
                             on m.tblAgentKeyId equals a.KeyId
                             where m.Concern != null
                             && m.CallDateTime > sDate
                             && m.CallDateTime < eDate
                             select new { a.KeyId, a.AgentId, a.FirstName, a.LastName }).Distinct();


                foreach (var a in query)
                {
                    Agents agent = new Agents(a.KeyId, a.AgentId, a.FirstName + " " + a.LastName);
                    agents.Add(agent);
                }
            }


            return agents;

        }


        /// <summary>
        /// Get all sales for an agentid (where Verified =1)
        /// </summary>
        /// <param name="agentid"></param>
        /// <returns></returns>
        private static int GetGoodSales(int agentKeyId, DateTime cDate)
        {
            DateTime sDate = cDate.AddDays(-1);
            DateTime eDate = cDate;

            int goodsales = 0;

            using (ConstellationEntities entitites = new ConstellationEntities())
            {
                var query = (from a in entitites.tblMains
                             where a.tblAgentKeyId == agentKeyId
                             && a.CallDateTime > sDate
                             && a.CallDateTime < eDate
                             && a.Verified == "1"
                             select a).Count();

                goodsales = query;
            }

            return goodsales;


        }

        /// <summary>
        /// Gets all bad sales for an agentid (where Verified !=1)
        /// </summary>
        /// <param name="agentid"></param>
        /// <returns></returns>
        private static int GetBadSales(int agentKeyId, DateTime cDate)
        {
            DateTime sDate = cDate.AddDays(-1);
            DateTime eDate = cDate;
            int badsales = 0;

            using (ConstellationEntities entitites = new ConstellationEntities())
            {
                var query = (from a in entitites.tblMains
                             where a.tblAgentKeyId == agentKeyId
                             && a.CallDateTime > sDate
                             && a.CallDateTime < eDate
                             && a.Verified != "1"
                             select a).Count();

                badsales = query;
            }

            return badsales;


        }


        /// <summary>
        /// Gets distinct list of dispositions for vendorid where Verified !=1
        /// </summary>
        /// <param name="vendorid"></param>
        /// <returns></returns>
        private static List<Dispositions> GetDispositions(string vendorid, DateTime cDate)
        {
            DateTime sDate = cDate.AddDays(-1);
            DateTime eDate = cDate;
            List<Dispositions> dispositions = new List<Dispositions>();
            using (ConstellationEntities entitites = new ConstellationEntities())
            {
                var query = (from d in entitites.tblMains
                             where d.VendorId == vendorid
                             && d.CallDateTime > sDate
                             && d.CallDateTime < eDate
                              && d.Verified != "1"
                             group d by d.Concern into g
                             select new
                             {
                                 Concern = g.Key,
                                 Count = g.Count()
                             });

                foreach (var disp in query)
                {
                    Dispositions disposition = new Dispositions(disp.Count, disp.Concern);
                    dispositions.Add(disposition);
                }
            }


            return dispositions;
        }

        /// <summary>
        /// Gets distinct list of dispositions for ALL vendors where Verified !=1
        /// </summary>
        /// <param name="vendorid"></param>
        /// <returns></returns>
        private static List<Dispositions> GetDispositions(DateTime cDate)
        {
            DateTime sDate = cDate.AddDays(-1);
            DateTime eDate = cDate;
            List<Dispositions> dispositions = new List<Dispositions>();
            using (ConstellationEntities entitites = new ConstellationEntities())
            {
                var query = (from d in entitites.tblMains
                             where d.Verified != "1"
                             && d.CallDateTime > sDate
                             && d.CallDateTime < eDate
                             group d by d.Concern into g
                             select new
                             {
                                 Concern = g.Key,
                                 Count = g.Count()
                             });

                foreach (var disp in query)
                {
                    Dispositions disposition = new Dispositions(disp.Count, disp.Concern);
                    dispositions.Add(disposition);
                }
            }


            return dispositions;
        }
        #endregion

        #region Utilities
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
        private static void SaveXlsDocument(ref string reportPath, ref string xlsFilename, ref string xlsFilePath, Excel.Workbook exBook, DateTime cDate, string vendor)
        {
            string vendorType = string.Empty;
            //Build the file name

            vendorType = vendor == "Total" ? "Total" : "Vendor" + vendor;
            xlsFilename = "Constellation_" + vendorType + "_DailyStats" + String.Format("{0:yyyyMMdd}", cDate.AddDays(-1)) + ".xlsx";

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

                //StringBuilder sb = new StringBuilder();

                //sb.AppendLine("");
                //strMsgBody = sb.ToString();

                SmtpMail mail = new SmtpMail("TMPWEB1", false);

                mail.AddAttachment(xlsFilePath);//Attach XLS report
                mail.AddRecipient(strToEmail, RecipientType.To);


                mail.From = "reports1@calibrus.com";

                mail.Subject = "Constellation " + (vendor == "" ? "Total " : "Vendor: " + vendor) + " Daily Report for " + currentDate.AddDays(-1).ToString("dddd, dd MMMM yyyy") + ".";


                //mail.Body = strMsgBody;
                mail.SendMessage();

            }
            catch (Exception ex)
            {
                SendErrorMessage(ex);
            }


        }
        private static void GetDates(out DateTime CurrentDate)
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

            CurrentDate = new DateTime(baseDate.Year, baseDate.Month, baseDate.Day, 0, 0, 0);//current date time


        }
        private static void SendErrorMessage(Exception ex)
        {
            Calibrus.ErrorHandler.Alerting alert = new Calibrus.ErrorHandler.Alerting("ConstellationDailyStats");
            alert.SendAlert(ex.Source, ex.Message, Environment.MachineName, Environment.UserName, "1.0");
        }
        #endregion


    }
}
