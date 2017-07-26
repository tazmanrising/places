using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Configuration;
using Calibrus.Mail;
using Calibrus.ErrorHandler;
using Excel = Microsoft.Office.Interop.Excel;

namespace ConstellationDashboardDailyReport
{

    public class Report
    {

        public enum Commodity
        {
            Electric,
            Gas,
            Dual
        }
        #region Main
        public static void Main(string[] args)
        {
            string rootPath = string.Empty;
            string mailRecipientTO = string.Empty;

            //get report interval
            DateTime CurrentDate = new DateTime();
            DateTime MonthStartDate = new DateTime();
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
            mailRecipientTO = ConfigurationManager.AppSettings["mailRecipientTO"].ToString();

            //Object to pass to optional parameters
            object na = System.Reflection.Missing.Value;

            //start Excel
            Excel.Application exApp = new Excel.Application();
            Excel.Workbook exBook = null;
            Excel.Worksheet exSheet = null;
            Excel.Range exRange = null;

            int sheetsAdded = 0;

            exBook = exApp.Workbooks.Add(na);
            exApp.Visible = false;

            //Set global attributes
            exApp.StandardFont = "Calibri";
            exApp.StandardFontSize = 11;
            try
            {
                if (sheetsAdded < exBook.Sheets.Count)
                {
                    exSheet = (Excel.Worksheet)exBook.Sheets[sheetsAdded + 1];
                }
                else
                {
                    exSheet = (Excel.Worksheet)exBook.Sheets.Add(na, exBook.ActiveSheet, na, na);
                }

                //select the first tab in the workbook
                exSheet = (Excel.Worksheet)exApp.Worksheets[1];
                exSheet.Select(na);
                sheetsAdded++;

                string sheetName = String.Format("{0}", "Constellation Daily " + CurrentDate.ToString("MMM") + " " + CurrentDate.ToString("yyyy"));
                exSheet.Name = sheetName.Length > 30 ? sheetName.Substring(0, 30) : sheetName; //force length of sheet name due to excel constraints
                exSheet.Select(na);

                //Write Report
                WriteReport(ref exApp, ref exRange, CurrentDate, MonthStartDate, YearStartDate);


                //Save the xls Report
                SaveXlsDocument(ref rootPath, ref xlsFilename, ref xlsFilePath, exBook, CurrentDate);

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
            SendEmail(ref xlsFilePath, CurrentDate, mailRecipientTO);
        }
        #endregion

        #region Excel
        public static void WriteReport(ref Excel.Application exApp, ref Excel.Range exRange, DateTime currentDate, DateTime monthStartDate, DateTime yearStartDate)
        {
            #region Variables
            Excel.Font exFont = null;

            int FormulaTotalRowCountStart = 0;
            int FormulaTotalRowCountEnd = 0;

            //Hide Range Column place holders
            int weekHideRangeStart = 0;
            int monthHideRangeStart = 0;

            //Placeholders as I move through the Excel sheet
            int colCount = 0;
            int rowCount = 0;

            int headerDateRowStart = 1; //start of the headears for the Dates Week 1 then under it Date of that week ending on Friday

            int headerColumnStart = 65; //A ascii value for conversion
            int headerRowStart = 2; //start of the headears for Data

            int globalRowStart = 0; //start of the row for the Global
            int globalColumnStart = 65; //A ascii value for conversion
            int globalRowCount = 0; //placeholder for Global Row iterator
            int globalColCount = 0;

            //start for Data  after the headers	
            int columnDataStart = 65; //A
            int rowDataStart = 3;

            #endregion

            #region Vendor through UDC through Date Headers

            //Write out the   headers
            exRange = (Excel.Range)exApp.Cells[headerRowStart, ConvertColumn(headerColumnStart)];
            exRange.Value2 = "Vendor Name";
            exRange.Interior.ColorIndex = 37;
            exFont = exRange.Font;
            exFont.ColorIndex = 1;
            exFont.Bold = true;
            headerColumnStart++;

            exRange = (Excel.Range)exApp.Cells[headerRowStart, ConvertColumn(headerColumnStart)];
            exRange.Value2 = "Commodity";
            exRange.Interior.ColorIndex = 37;
            exFont = exRange.Font;
            exFont.ColorIndex = 1;
            exFont.Bold = true;
            headerColumnStart++;

            exRange = (Excel.Range)exApp.Cells[headerRowStart, ConvertColumn(headerColumnStart)];
            exRange.Value2 = "State";
            exRange.Interior.ColorIndex = 37;
            exFont = exRange.Font;
            exFont.ColorIndex = 1;
            exFont.Bold = true;
            headerColumnStart++;

            exRange = (Excel.Range)exApp.Cells[headerRowStart, ConvertColumn(headerColumnStart)];
            exRange.Value2 = "LDC Code";
            exRange.Interior.ColorIndex = 37;
            exFont = exRange.Font;
            exFont.ColorIndex = 1;
            exFont.Bold = true;
            headerColumnStart++;

            exRange = (Excel.Range)exApp.Cells[headerRowStart - 1, ConvertColumn(headerColumnStart)];
            exRange.Value2 = "Yesterday's Sales";
            exRange.Interior.ColorIndex = 37;
            exRange.HorizontalAlignment = Excel.Constants.xlCenter;
            exFont = exRange.Font;
            exFont.ColorIndex = 1;
            exFont.Bold = true;

            exRange = (Excel.Range)exApp.Cells[headerRowStart, ConvertColumn(headerColumnStart)];
            exRange.Value2 = String.Format("{0:MM/dd/yyyy}", currentDate.AddDays(-1));
            exRange.Interior.ColorIndex = 37;
            exRange.HorizontalAlignment = Excel.Constants.xlCenter;
            exFont = exRange.Font;
            exFont.ColorIndex = 1;
            exFont.Bold = true;
            headerColumnStart++;

            //Date Range Header Loop
            DateTime headerStartDate = yearStartDate;
            DateTime headerEndDate = yearStartDate.AddYears(1);
            DateTime headerDateCounter = headerStartDate;
            int headerWeekCounter = 1;

            //Weeks Header
            while (headerDateCounter < headerEndDate)
            {
                if (headerWeekCounter < 53)
                {
                    if (headerDateCounter > currentDate) // if the daterange goes beyond current date AND I haven't set the value
                    {
                        if (weekHideRangeStart == 0)
                        {
                            weekHideRangeStart = headerColumnStart; //set the column for the Week Hiding Range Start
                        }
                    }

                    exRange = (Excel.Range)exApp.Cells[headerDateRowStart, ConvertColumn(headerColumnStart)];
                    exRange.Value2 = "Week " + headerWeekCounter;
                    exRange.Interior.ColorIndex = 37;
                    exFont = exRange.Font;
                    exFont.ColorIndex = 1;
                    exFont.Bold = true;

                    exRange = (Excel.Range)exApp.Cells[headerRowStart, ConvertColumn(headerColumnStart)];
                    exRange.Value2 = String.Format("{0:MM/dd/yyyy}", GetFriday(headerDateCounter));
                    exRange.Interior.ColorIndex = 37;
                    exFont = exRange.Font;
                    exFont.ColorIndex = 1;
                    exFont.Bold = true;
                }
                else
                {
                    if (weekHideRangeStart == 0)
                    {
                        weekHideRangeStart = headerColumnStart; //set the column for the Week Hiding Range Start
                    }
                }

                headerColumnStart++;
                headerWeekCounter++;

                headerDateCounter = headerDateCounter.AddDays(7);
            }

            //reset values to step through columns
            headerColumnStart--;
            headerStartDate = yearStartDate;
            headerDateCounter = headerStartDate;

            //MTD Header
            while (headerDateCounter < headerEndDate)
            {

                if (headerDateCounter.Month == currentDate.Month) // if the daterange goes beyond current date AND I haven't set the value
                {
                    if (monthHideRangeStart == 0)
                    {
                        monthHideRangeStart = headerColumnStart; //set the column for the Month Hiding Range Start
                    }
                }
                exRange = (Excel.Range)exApp.Cells[headerRowStart, ConvertColumn(headerColumnStart)];
                exRange.Value2 = String.Format("{0:MMM}", headerDateCounter) + " MTD";
                exRange.Interior.ColorIndex = 37;
                exFont = exRange.Font;
                exFont.ColorIndex = 1;
                exFont.Bold = true;

                headerDateCounter = headerDateCounter.AddMonths(1);
                headerColumnStart++;
            }


            //YTD Header
            exRange = (Excel.Range)exApp.Cells[headerRowStart, ConvertColumn(headerColumnStart)];
            exRange.Value2 = "YTD";
            exRange.Interior.ColorIndex = 37;
            exFont = exRange.Font;
            exFont.ColorIndex = 1;
            exFont.Bold = true;

            #endregion

            #region Data output
            rowCount = rowDataStart; //3
            foreach (tblVendor vendor in GetVendors())
            {
                colCount = columnDataStart; //A              


                //Vendor Name
                exRange = (Excel.Range)exApp.Cells[rowCount, ConvertColumn(colCount)];
                exRange.Value2 = vendor.VendorName.ToString();
                exRange.Interior.ColorIndex = 15; //light grey
                exFont = exRange.Font;
                exFont.ColorIndex = 1;
                exFont.Bold = true;

                colCount++;

                FormulaTotalRowCountStart = rowCount;

                //Commodity Loop
                foreach (Commodity fueltype in Enum.GetValues(typeof(Commodity)))
                {
                    //Commodity
                    exRange = (Excel.Range)exApp.Cells[rowCount, ConvertColumn(colCount)];
                    exRange.Value2 = fueltype.ToString();
                    exRange.Interior.ColorIndex = 15; //light grey
                    if (fueltype.ToString() == "Dual")
                    {
                        exFont = exRange.Font;
                        exFont.ColorIndex = 53;//Maroon
                    }
                    else
                    {
                        exFont = exRange.Font;
                        exFont.ColorIndex = 1;
                    }
                    exFont.Bold = true;

                    colCount++;

                    //State - Empty by default                    
                    exRange = (Excel.Range)exApp.Cells[rowCount, ConvertColumn(colCount)];
                    exRange.Value2 = "";
                    exRange.Interior.ColorIndex = 15; //light grey
                    exFont = exRange.Font;
                    exFont.ColorIndex = 1;
                    exFont.Bold = true;

                    colCount++;

                    //LDC Code
                    List<string> LDCCodes = GetUDCCode(vendor.VendorId.ToString(), fueltype.ToString());
                    foreach (string ldccode in LDCCodes)
                    {
                        exRange = (Excel.Range)exApp.Cells[rowCount, ConvertColumn(colCount)];
                        exRange.Value2 = ldccode.ToString();
                        exRange.Interior.ColorIndex = 15; //light grey
                        if (fueltype.ToString() == "Dual")
                        {
                            exFont = exRange.Font;
                            exFont.ColorIndex = 53;//Maroon
                        }
                        else
                        {
                            exFont = exRange.Font;
                            exFont.ColorIndex = 1;
                        }
                        exFont.Bold = true;
                        colCount++;


                        //Yesterday's total
                        int yesterdayCount = 0;
                        yesterdayCount = GetUDCVerifiedCount(vendor.VendorId.ToString(), fueltype.ToString(), ldccode.ToString(), currentDate, "yesterday");
                        //yesterdayTotal += yesterdayCount;

                        exRange = (Excel.Range)exApp.Cells[rowCount, ConvertColumn(colCount)];
                        exRange.Value2 = yesterdayCount;

                        if (fueltype.ToString() == "Dual")
                        {
                            exRange.Interior.ColorIndex = 15; //light grey
                            exFont = exRange.Font;
                            exFont.ColorIndex = 53;//Maroon
                        }
                        else
                        {
                            exRange.Interior.ColorIndex = 34; //light turquoise
                            exFont = exRange.Font;
                            exFont.ColorIndex = 1;
                        }
                        exFont.Bold = false;
                        colCount++;

                        //reset values to step through columns

                        DateTime dataStartDate = yearStartDate;
                        DateTime dataEndDate = yearStartDate.AddYears(1);
                        DateTime dataDateCounter = dataStartDate;
                        int dataCounter = 1;

                        //Loop through and get Week values
                        while (dataDateCounter < dataEndDate)
                        {
                            if (dataCounter < 53)
                            {

                                //Get the end of the week for the current week, a Friday
                                //GetFriday(weekDataDateCounter);                                

                                exRange = (Excel.Range)exApp.Cells[rowCount, ConvertColumn(colCount)];
                                exRange.Value2 = GetUDCVerifiedCount(vendor.VendorId.ToString(), fueltype.ToString(), ldccode.ToString(), GetFriday(dataDateCounter), "weekly");

                                if (fueltype.ToString() == "Dual")
                                {
                                    exRange.Interior.ColorIndex = 15; //light grey
                                    exFont = exRange.Font;
                                    exFont.ColorIndex = 53;//Maroon
                                }
                                else
                                {
                                    exRange.Interior.ColorIndex = 34; //light turquoise
                                    exFont = exRange.Font;
                                    exFont.ColorIndex = 1;
                                }
                                exFont.Bold = false;

                            }

                            colCount++;
                            dataCounter++;

                            dataDateCounter = dataDateCounter.AddDays(7);
                        }



                        //reset values to step through columns
                        colCount--;
                        dataStartDate = yearStartDate;
                        dataDateCounter = dataStartDate;


                        //MTD values
                        while (dataDateCounter < dataEndDate)
                        {
                            exRange = (Excel.Range)exApp.Cells[rowCount, ConvertColumn(colCount)];
                            exRange.Value2 = GetUDCVerifiedCount(vendor.VendorId.ToString(), fueltype.ToString(), ldccode.ToString(), dataDateCounter, "mtd");

                            if (fueltype.ToString() == "Dual")
                            {
                                exRange.Interior.ColorIndex = 15; //light grey
                                exFont = exRange.Font;
                                exFont.ColorIndex = 53;//Maroon
                            }
                            else
                            {
                                exRange.Interior.ColorIndex = 34; //light turquoise
                                exFont = exRange.Font;
                                exFont.ColorIndex = 1;
                            }
                            exFont.Bold = false;


                            dataDateCounter = dataDateCounter.AddMonths(1);
                            colCount++;
                        }


                        //YTD values
                        exRange = (Excel.Range)exApp.Cells[rowCount, ConvertColumn(colCount)];
                        exRange.Value2 = GetUDCVerifiedCount(vendor.VendorId.ToString(), fueltype.ToString(), ldccode.ToString(), currentDate, "ytd");

                        if (fueltype.ToString() == "Dual")
                        {
                            exRange.Interior.ColorIndex = 15; //light grey
                            exFont = exRange.Font;
                            exFont.ColorIndex = 53;//Maroon
                        }
                        else
                        {
                            exRange.Interior.ColorIndex = 34; //light turquoise
                            exFont = exRange.Font;
                            exFont.ColorIndex = 1;
                        }
                        exFont.Bold = false;

                        //reset for next row
                        rowCount++;
                        colCount = 68;
                        headerColumnStart++;
                    }

                    if (fueltype.ToString() == "Gas")
                    {

                        FormulaTotalRowCountEnd = rowCount - 1; //get the row of where we will output the formula
                    }


                    //reset columncount to display next commodity
                    colCount = columnDataStart + 1;
                }

                #region Totals Formulas
                //totals here
                colCount = columnDataStart; //A 
                exRange = (Excel.Range)exApp.Cells[rowCount, ConvertColumn(colCount)];
                exRange.Value2 = vendor.VendorName.ToString() + " Total";
                exRange.Interior.ColorIndex = 6; //yellow
                exFont = exRange.Font;
                exFont.ColorIndex = 1;
                exFont.Bold = true;

                exRange = (Excel.Range)exApp.Cells[rowCount + 1, ConvertColumn(colCount)];
                exRange.Value2 = vendor.VendorName.ToString() + " Dual Fuel Total";
                exRange.Interior.ColorIndex = 15; //grey
                exFont = exRange.Font;
                exFont.ColorIndex = 1;
                exFont.Bold = true;
                colCount++;

                exRange = (Excel.Range)exApp.Cells[rowCount, ConvertColumn(colCount)];
                exRange.Value2 = "";
                exRange.Interior.ColorIndex = 6; //yellow
                exFont = exRange.Font;
                exFont.ColorIndex = 1;
                exFont.Bold = true;

                exRange = (Excel.Range)exApp.Cells[rowCount + 1, ConvertColumn(colCount)];
                exRange.Value2 = vendor.VendorName.ToString() + " Dual Fuel Total";
                exRange.Interior.ColorIndex = 15; //grey
                exFont = exRange.Font;
                exFont.ColorIndex = 1;
                exFont.Bold = true;
                colCount++;

                exRange = (Excel.Range)exApp.Cells[rowCount, ConvertColumn(colCount)];
                exRange.Value2 = "";
                exRange.Interior.ColorIndex = 6; //yellow
                exFont = exRange.Font;
                exFont.ColorIndex = 1;
                exFont.Bold = true;

                exRange = (Excel.Range)exApp.Cells[rowCount + 1, ConvertColumn(colCount)];
                exRange.Value2 = "";
                exRange.Interior.ColorIndex = 15; //grey
                exFont = exRange.Font;
                exFont.ColorIndex = 1;
                exFont.Bold = true;
                colCount++;

                exRange = (Excel.Range)exApp.Cells[rowCount, ConvertColumn(colCount)];
                exRange.Value2 = "";
                exRange.Interior.ColorIndex = 6; //yellow
                exFont = exRange.Font;
                exFont.ColorIndex = 1;
                exFont.Bold = true;

                exRange = (Excel.Range)exApp.Cells[rowCount + 1, ConvertColumn(colCount)];
                exRange.Value2 = "";
                exRange.Interior.ColorIndex = 15; //grey
                exFont = exRange.Font;
                exFont.ColorIndex = 1;
                exFont.Bold = true;
                colCount++;

                //Yesterdays formula for Gas and Electric
                exRange = (Excel.Range)exApp.Cells[rowCount, ConvertColumn(colCount)];
                exRange.Formula = string.Format("=SUM({0}{1}:{0}{2})", ConvertColumn(colCount), FormulaTotalRowCountStart, FormulaTotalRowCountEnd);
                exRange.Interior.ColorIndex = 6; //yellow
                exFont = exRange.Font;
                exFont.ColorIndex = 1;
                exFont.Bold = true;

                //Yesterdays formula for Dual
                exRange = (Excel.Range)exApp.Cells[rowCount + 1, ConvertColumn(colCount)];
                exRange.Formula = string.Format("=SUM({0}{1}:{0}{2})", ConvertColumn(colCount), FormulaTotalRowCountEnd + 1, rowCount - 1);
                exRange.Interior.ColorIndex = 15; //grey
                exFont = exRange.Font;
                exFont.ColorIndex = 1;
                exFont.Bold = true;
                colCount++;



                //Weekly loop formula
                DateTime formulaStartDate = yearStartDate;
                DateTime formulaEndDate = yearStartDate.AddYears(1);
                DateTime formulaDateCounter = formulaStartDate;
                int formulaCounter = 1;

                //Loop through and get Week values
                while (formulaDateCounter < formulaEndDate)
                {
                    if (formulaCounter < 53)
                    {
                        //Weekly formula for Gas and Electric
                        exRange = (Excel.Range)exApp.Cells[rowCount, ConvertColumn(colCount)];
                        exRange.Formula = string.Format("=SUM({0}{1}:{0}{2})", ConvertColumn(colCount), FormulaTotalRowCountStart, FormulaTotalRowCountEnd);
                        exRange.Interior.ColorIndex = 6; //yellow
                        exFont = exRange.Font;
                        exFont.ColorIndex = 1;
                        exFont.Bold = true;

                        //Weekly formula for Dual
                        exRange = (Excel.Range)exApp.Cells[rowCount + 1, ConvertColumn(colCount)];
                        exRange.Formula = string.Format("=SUM({0}{1}:{0}{2})", ConvertColumn(colCount), FormulaTotalRowCountEnd + 1, rowCount - 1);
                        exRange.Interior.ColorIndex = 15; //grey
                        exFont = exRange.Font;
                        exFont.ColorIndex = 1;
                        exFont.Bold = true;
                    }

                    colCount++;
                    formulaCounter++;

                    formulaDateCounter = formulaDateCounter.AddDays(7);
                }


                //reset values to step through columns
                colCount--;
                formulaStartDate = yearStartDate;
                formulaDateCounter = formulaStartDate;

                //MTD loop formula
                while (formulaDateCounter < formulaEndDate)
                {
                    //MTD formula for Gas and Electric
                    exRange = (Excel.Range)exApp.Cells[rowCount, ConvertColumn(colCount)];
                    exRange.Formula = string.Format("=SUM({0}{1}:{0}{2})", ConvertColumn(colCount), FormulaTotalRowCountStart, FormulaTotalRowCountEnd);
                    exRange.Interior.ColorIndex = 6; //yellow
                    exFont = exRange.Font;
                    exFont.ColorIndex = 1;
                    exFont.Bold = true;

                    //MTD formula for Dual
                    exRange = (Excel.Range)exApp.Cells[rowCount + 1, ConvertColumn(colCount)];
                    exRange.Formula = string.Format("=SUM({0}{1}:{0}{2})", ConvertColumn(colCount), FormulaTotalRowCountEnd + 1, rowCount - 1);
                    exRange.Interior.ColorIndex = 15; //grey
                    exFont = exRange.Font;
                    exFont.ColorIndex = 1;
                    exFont.Bold = true;


                    formulaDateCounter = formulaDateCounter.AddMonths(1);
                    colCount++;
                }


                //YTD Formula
                exRange = (Excel.Range)exApp.Cells[rowCount, ConvertColumn(colCount)];
                exRange.Formula = string.Format("=SUM({0}{1}:{0}{2})", ConvertColumn(colCount), FormulaTotalRowCountStart, FormulaTotalRowCountEnd);
                exRange.Interior.ColorIndex = 6; //yellow
                exFont = exRange.Font;
                exFont.ColorIndex = 1;
                exFont.Bold = true;

                //YTD Formula for Dual
                exRange = (Excel.Range)exApp.Cells[rowCount + 1, ConvertColumn(colCount)];
                exRange.Formula = string.Format("=SUM({0}{1}:{0}{2})", ConvertColumn(colCount), FormulaTotalRowCountEnd + 1, rowCount - 1);
                exRange.Interior.ColorIndex = 15; //grey
                exFont = exRange.Font;
                exFont.ColorIndex = 1;
                exFont.Bold = true;

                #endregion


                //next row to display the next vendor                
                rowCount++;
                rowCount++;

                globalRowStart = rowCount;
            }
            #endregion

            #region Grand Total
            globalRowCount = globalRowStart;

            globalColCount = globalColumnStart;
            //Grand Totals Headers
            exRange = (Excel.Range)exApp.Cells[globalRowCount, ConvertColumn(globalColCount)];
            exRange.Value2 = "Grand Total";
            exRange.Interior.ColorIndex = 37; //Turquoise
            exFont = exRange.Font;
            exFont.ColorIndex = 1;
            exFont.Bold = true;
            globalRowCount++;

            exRange = (Excel.Range)exApp.Cells[globalRowCount, ConvertColumn(globalColCount)];
            exRange.Value2 = "";
            exFont = exRange.Font;
            exFont.ColorIndex = 1;
            exFont.Bold = true;
            globalRowCount++;

            exRange = (Excel.Range)exApp.Cells[globalRowCount, ConvertColumn(globalColCount)];
            exRange.Value2 = "";
            exFont = exRange.Font;
            exFont.ColorIndex = 1;
            exFont.Bold = true;
            globalRowCount++;

            exRange = (Excel.Range)exApp.Cells[globalRowCount, ConvertColumn(globalColCount)];
            exRange.Value2 = "";
            exFont = exRange.Font;
            exFont.ColorIndex = 1;
            exFont.Bold = true;
            globalRowCount++;

            exRange = (Excel.Range)exApp.Cells[globalRowCount, ConvertColumn(globalColCount)];
            exRange.Value2 = "Grand Total";
            exRange.Interior.ColorIndex = 12; //Dark Yellow
            exFont = exRange.Font;
            exFont.ColorIndex = 1;
            exFont.Bold = true;


            globalRowCount = globalRowStart;//reset values
            globalColCount++;

            exRange = (Excel.Range)exApp.Cells[globalRowCount, ConvertColumn(globalColCount)];
            exRange.Value2 = "";
            exRange.Interior.ColorIndex = 37; //Turquoise
            exFont = exRange.Font;
            exFont.ColorIndex = 1;
            exFont.Bold = true;
            globalRowCount++;

            exRange = (Excel.Range)exApp.Cells[globalRowCount, ConvertColumn(globalColCount)];
            exRange.Value2 = "TOTAL ELECTRIC";
            exRange.Interior.ColorIndex = 15; //light grey
            exFont = exRange.Font;
            exFont.ColorIndex = 1;
            exFont.Bold = true;
            globalRowCount++;

            exRange = (Excel.Range)exApp.Cells[globalRowCount, ConvertColumn(globalColCount)];
            exRange.Value2 = "TOTAL GAS";
            exRange.Interior.ColorIndex = 15; //light grey
            exFont = exRange.Font;
            exFont.ColorIndex = 1;
            exFont.Bold = true;
            globalRowCount++;

            exRange = (Excel.Range)exApp.Cells[globalRowCount, ConvertColumn(globalColCount)];
            exRange.Value2 = "TOTAL DUAL FUEL";
            exRange.Interior.ColorIndex = 15; //light grey
            exFont = exRange.Font;
            exFont.ColorIndex = 1;
            exFont.Bold = true;
            globalRowCount++;

            exRange = (Excel.Range)exApp.Cells[globalRowCount, ConvertColumn(globalColCount)];
            exRange.Value2 = "";
            exRange.Interior.ColorIndex = 12; //Dark Yellow
            exFont = exRange.Font;
            exFont.ColorIndex = 1;
            exFont.Bold = true;

            globalRowCount = globalRowStart;//reset values
            globalColCount++;

            exRange = (Excel.Range)exApp.Cells[globalRowCount, ConvertColumn(globalColCount)];
            exRange.Value2 = "";
            exRange.Interior.ColorIndex = 37; //Turquoise
            exFont = exRange.Font;
            exFont.ColorIndex = 1;
            exFont.Bold = true;
            globalRowCount++;

            exRange = (Excel.Range)exApp.Cells[globalRowCount, ConvertColumn(globalColCount)];
            exRange.Value2 = "";
            exRange.Interior.ColorIndex = 15; //light grey
            exFont = exRange.Font;
            exFont.ColorIndex = 1;
            exFont.Bold = true;
            globalRowCount++;

            exRange = (Excel.Range)exApp.Cells[globalRowCount, ConvertColumn(globalColCount)];
            exRange.Value2 = "";
            exRange.Interior.ColorIndex = 15; //light grey
            exFont = exRange.Font;
            exFont.ColorIndex = 1;
            exFont.Bold = true;
            globalRowCount++;

            exRange = (Excel.Range)exApp.Cells[globalRowCount, ConvertColumn(globalColCount)];
            exRange.Value2 = "";
            exRange.Interior.ColorIndex = 15; //light grey
            exFont = exRange.Font;
            exFont.ColorIndex = 1;
            exFont.Bold = true;
            globalRowCount++;

            exRange = (Excel.Range)exApp.Cells[globalRowCount, ConvertColumn(globalColCount)];
            exRange.Value2 = "";
            exRange.Interior.ColorIndex = 12; //Dark Yellow
            exFont = exRange.Font;
            exFont.ColorIndex = 1;
            exFont.Bold = true;


            globalRowCount = globalRowStart;//reset values
            globalColCount++;

            exRange = (Excel.Range)exApp.Cells[globalRowCount, ConvertColumn(globalColCount)];
            exRange.Value2 = "";
            exRange.Interior.ColorIndex = 37; //Turquoise
            exFont = exRange.Font;
            exFont.ColorIndex = 1;
            exFont.Bold = true;
            globalRowCount++;

            exRange = (Excel.Range)exApp.Cells[globalRowCount, ConvertColumn(globalColCount)];
            exRange.Value2 = "";
            exRange.Interior.ColorIndex = 15; //light grey
            exFont = exRange.Font;
            exFont.ColorIndex = 1;
            exFont.Bold = true;
            globalRowCount++;

            exRange = (Excel.Range)exApp.Cells[globalRowCount, ConvertColumn(globalColCount)];
            exRange.Value2 = "";
            exRange.Interior.ColorIndex = 15; //light grey
            exFont = exRange.Font;
            exFont.ColorIndex = 1;
            exFont.Bold = true;
            globalRowCount++;

            exRange = (Excel.Range)exApp.Cells[globalRowCount, ConvertColumn(globalColCount)];
            exRange.Value2 = "";
            exRange.Interior.ColorIndex = 15; //light grey
            exFont = exRange.Font;
            exFont.ColorIndex = 1;
            exFont.Bold = true;
            globalRowCount++;

            exRange = (Excel.Range)exApp.Cells[globalRowCount, ConvertColumn(globalColCount)];
            exRange.Value2 = "";
            exRange.Interior.ColorIndex = 12; //Dark Yellow
            exFont = exRange.Font;
            exFont.ColorIndex = 1;
            exFont.Bold = true;

            #endregion

            #region Global Totals Data

            globalRowCount = globalRowStart;//reset values
            globalColCount++;

            //Global Totals Yesterday
            exRange = (Excel.Range)exApp.Cells[globalRowCount, ConvertColumn(globalColCount)];
            exRange.Formula = string.Format("=SUM({0}{1}:{0}{2})", ConvertColumn(globalColCount), globalRowCount + 1, globalRowCount + 2);
            exRange.Interior.ColorIndex = 37; //Turquoise
            exFont = exRange.Font;
            exFont.ColorIndex = 1;
            exFont.Bold = true;
            globalRowCount++;

            exRange = (Excel.Range)exApp.Cells[globalRowCount, ConvertColumn(globalColCount)];
            exRange.Value2 = GetUDCVerifiedCount("Electric", currentDate, "yesterday");
            exRange.Interior.ColorIndex = 15; //light grey
            exFont = exRange.Font;
            exFont.ColorIndex = 1;
            exFont.Bold = true;
            globalRowCount++;

            exRange = (Excel.Range)exApp.Cells[globalRowCount, ConvertColumn(globalColCount)];
            exRange.Value2 = GetUDCVerifiedCount("Gas", currentDate, "yesterday");
            exRange.Interior.ColorIndex = 15; //light grey
            exFont = exRange.Font;
            exFont.ColorIndex = 1;
            exFont.Bold = true;
            globalRowCount++;

            exRange = (Excel.Range)exApp.Cells[globalRowCount, ConvertColumn(globalColCount)];
            exRange.Value2 = GetUDCVerifiedCount("Dual", currentDate, "yesterday");
            exRange.Interior.ColorIndex = 15; //light grey
            exFont = exRange.Font;
            exFont.ColorIndex = 1;
            exFont.Bold = true;
            globalRowCount++;

            //Grand Total Example: =SUM(F136:F137)
            exRange = (Excel.Range)exApp.Cells[globalRowCount, ConvertColumn(globalColCount)];
            exRange.Formula = string.Format("=SUM({0}{1}:{0}{2})", ConvertColumn(globalColCount), globalRowCount - 3, globalRowCount - 2);
            exRange.Interior.ColorIndex = 12; //Dark Yellow
            exFont = exRange.Font;
            exFont.ColorIndex = 1;
            exFont.Bold = true;
            globalRowCount++;

            //Global Totals Weekly

            DateTime globalStartDate = yearStartDate;
            DateTime globalEndDate = yearStartDate.AddYears(1);
            DateTime globalDateCounter = globalStartDate;
            int globalCounter = 1;

            globalColCount++;
            //Loop through and get Global Week values           
            while (globalDateCounter < globalEndDate)
            {
                globalRowCount = globalRowStart;//reset values

                if (globalCounter < 53)
                {
                    exRange = (Excel.Range)exApp.Cells[globalRowCount, ConvertColumn(globalColCount)];
                    exRange.Formula = string.Format("=SUM({0}{1}:{0}{2})", ConvertColumn(globalColCount), globalRowCount + 1, globalRowCount + 2);
                    exRange.Interior.ColorIndex = 37; //Turquoise
                    exFont = exRange.Font;
                    exFont.ColorIndex = 1;
                    exFont.Bold = true;
                    globalRowCount++;

                    exRange = (Excel.Range)exApp.Cells[globalRowCount, ConvertColumn(globalColCount)];
                    exRange.Value2 = GetUDCVerifiedCount("Electric", GetFriday(globalDateCounter), "weekly");
                    exRange.Interior.ColorIndex = 15; //light grey
                    exFont = exRange.Font;
                    exFont.ColorIndex = 1;
                    exFont.Bold = true;
                    globalRowCount++;

                    exRange = (Excel.Range)exApp.Cells[globalRowCount, ConvertColumn(globalColCount)];
                    exRange.Value2 = GetUDCVerifiedCount("Gas", GetFriday(globalDateCounter), "weekly");
                    exRange.Interior.ColorIndex = 15; //light grey
                    exFont = exRange.Font;
                    exFont.ColorIndex = 1;
                    exFont.Bold = true;
                    globalRowCount++;

                    exRange = (Excel.Range)exApp.Cells[globalRowCount, ConvertColumn(globalColCount)];
                    exRange.Value2 = GetUDCVerifiedCount("Dual", GetFriday(globalDateCounter), "weekly");
                    exRange.Interior.ColorIndex = 15; //light grey
                    exFont = exRange.Font;
                    exFont.ColorIndex = 1;
                    exFont.Bold = true;
                    globalRowCount++;

                    //Grand Total Example: =SUM(F136:F137)
                    exRange = (Excel.Range)exApp.Cells[globalRowCount, ConvertColumn(globalColCount)];
                    exRange.Formula = string.Format("=SUM({0}{1}:{0}{2})", ConvertColumn(globalColCount), globalRowCount - 3, globalRowCount - 2);
                    exRange.Interior.ColorIndex = 12; //Dark Yellow
                    exFont = exRange.Font;
                    exFont.ColorIndex = 1;
                    exFont.Bold = true;
                    globalRowCount++;

                }

                globalColCount++;
                globalCounter++;

                globalDateCounter = globalDateCounter.AddDays(7);
            }


            //reset values to step through columns
            globalColCount--;
            globalStartDate = yearStartDate;
            globalDateCounter = globalStartDate;

            //Loop through and get Global Montlhy values          
            while (globalDateCounter < globalEndDate)
            {
                globalRowCount = globalRowStart;//reset values

                exRange = (Excel.Range)exApp.Cells[globalRowCount, ConvertColumn(globalColCount)];
                exRange.Formula = string.Format("=SUM({0}{1}:{0}{2})", ConvertColumn(globalColCount), globalRowCount + 1, globalRowCount + 2);
                exRange.Interior.ColorIndex = 37; //Turquoise
                exFont = exRange.Font;
                exFont.ColorIndex = 1;
                exFont.Bold = true;
                globalRowCount++;

                exRange = (Excel.Range)exApp.Cells[globalRowCount, ConvertColumn(globalColCount)];
                exRange.Value2 = GetUDCVerifiedCount("Electric", GetFriday(globalDateCounter), "mtd");
                exRange.Interior.ColorIndex = 15; //light grey
                exFont = exRange.Font;
                exFont.ColorIndex = 1;
                exFont.Bold = true;
                globalRowCount++;

                exRange = (Excel.Range)exApp.Cells[globalRowCount, ConvertColumn(globalColCount)];
                exRange.Value2 = GetUDCVerifiedCount("Gas", GetFriday(globalDateCounter), "mtd");
                exRange.Interior.ColorIndex = 15; //light grey
                exFont = exRange.Font;
                exFont.ColorIndex = 1;
                exFont.Bold = true;
                globalRowCount++;

                exRange = (Excel.Range)exApp.Cells[globalRowCount, ConvertColumn(globalColCount)];
                exRange.Value2 = GetUDCVerifiedCount("Dual", GetFriday(globalDateCounter), "mtd");
                exRange.Interior.ColorIndex = 15; //light grey
                exFont = exRange.Font;
                exFont.ColorIndex = 1;
                exFont.Bold = true;
                globalRowCount++;

                //Grand Total Example: =SUM(F136:F137)
                exRange = (Excel.Range)exApp.Cells[globalRowCount, ConvertColumn(globalColCount)];
                exRange.Formula = string.Format("=SUM({0}{1}:{0}{2})", ConvertColumn(globalColCount), globalRowCount - 3, globalRowCount - 2);
                exRange.Interior.ColorIndex = 12; //Dark Yellow
                exFont = exRange.Font;
                exFont.ColorIndex = 1;
                exFont.Bold = true;

                globalColCount++;

                globalDateCounter = globalDateCounter.AddMonths(1);
            }


            //Global Toals YTD
            globalRowCount = globalRowStart;//reset values


            //Global Totals YTD
            exRange = (Excel.Range)exApp.Cells[globalRowCount, ConvertColumn(globalColCount)];
            exRange.Formula = string.Format("=SUM({0}{1}:{0}{2})", ConvertColumn(globalColCount), globalRowCount + 1, globalRowCount + 2);
            exRange.Interior.ColorIndex = 37; //Turquoise
            exFont = exRange.Font;
            exFont.ColorIndex = 1;
            exFont.Bold = true;
            globalRowCount++;

            exRange = (Excel.Range)exApp.Cells[globalRowCount, ConvertColumn(globalColCount)];
            exRange.Value2 = GetUDCVerifiedCount("Electric", currentDate, "ytd");
            exRange.Interior.ColorIndex = 15; //light grey
            exFont = exRange.Font;
            exFont.ColorIndex = 1;
            exFont.Bold = true;
            globalRowCount++;

            exRange = (Excel.Range)exApp.Cells[globalRowCount, ConvertColumn(globalColCount)];
            exRange.Value2 = GetUDCVerifiedCount("Gas", currentDate, "ytd");
            exRange.Interior.ColorIndex = 15; //light grey
            exFont = exRange.Font;
            exFont.ColorIndex = 1;
            exFont.Bold = true;
            globalRowCount++;

            exRange = (Excel.Range)exApp.Cells[globalRowCount, ConvertColumn(globalColCount)];
            exRange.Value2 = GetUDCVerifiedCount("Dual", currentDate, "ytd");
            exRange.Interior.ColorIndex = 15; //light grey
            exFont = exRange.Font;
            exFont.ColorIndex = 1;
            exFont.Bold = true;
            globalRowCount++;

            //Grand Total Example: =SUM(F136:F137)
            exRange = (Excel.Range)exApp.Cells[globalRowCount, ConvertColumn(globalColCount)];
            exRange.Formula = string.Format("=SUM({0}{1}:{0}{2})", ConvertColumn(globalColCount), globalRowCount - 3, globalRowCount - 2);
            exRange.Interior.ColorIndex = 12; //Dark Yellow
            exFont = exRange.Font;
            exFont.ColorIndex = 1;
            exFont.Bold = true;
            globalRowCount++;


            #endregion

            //Freeze the top two rows
            //    Range("A3").Select
            //ActiveWindow.FreezePanes = True
            exRange = (Excel.Range)exApp.get_Range("A3", "A3");
            exRange.Activate();
            exRange.Select();
            exRange.Application.ActiveWindow.FreezePanes = true;

            exRange = (Excel.Range)exApp.get_Range("A1", "BR1");
            exRange.EntireColumn.AutoFit();

            #region Hide Columns

            //Variables to store for finding the ranges of columns to hide
            string beginningCol = string.Empty;
            string endCol = string.Empty;

            //need to do hiding for the week
            //Show up to the previous 4 weeks prior

            //Testing
            //weekHideRangeStart = 74; //Column F Week 1
            //monthHideRangeStart = 122; //Column BF Jan MTD

            //weekHideRangeStart = 117; //Column AZ Week 47
            //monthHideRangeStart = 132; // Column BP Nov MTD

            //weekHideRangeStart = 121; //Column BD Week 51
            //monthHideRangeStart = 133; // Column BQ Dec MTD
            switch (weekHideRangeStart)
            {
                // if not f, g, h, I, J, hide the columns from F -> beginningCol
                case 70: //F
                case 71: //G
                case 72: //H
                case 73: //I
                case 74: //J

                    //hide all weeks after current to current month
                    beginningCol = String.Format("{0}1", ConvertColumn(weekHideRangeStart));
                    endCol = String.Format("{0}1", ConvertColumn(monthHideRangeStart - 1));
                    exRange = exRange = (Excel.Range)exApp.get_Range(beginningCol, endCol);
                    exRange.EntireColumn.Hidden = true;

                    //hide all months after current to the YTD
                    beginningCol = String.Format("{0}1", ConvertColumn(monthHideRangeStart + 1));
                    endCol = String.Format("{0}1", ConvertColumn(133)); //134 = BR and is the YTD column, so hide the range prior which would be Dec MTD column BQ
                    exRange = exRange = (Excel.Range)exApp.get_Range(beginningCol, endCol);
                    exRange.EntireColumn.Hidden = true;

                    break;

                default:

                    //hide from Yesterdays Sales to previous 4 weeks
                    beginningCol = String.Format("{0}1", ConvertColumn(70));//Column F which is the first week of the weekly loop
                    endCol = String.Format("{0}1", ConvertColumn(weekHideRangeStart - 6));
                    exRange = exRange = (Excel.Range)exApp.get_Range(beginningCol, endCol);
                    exRange.EntireColumn.Hidden = true;



                    if (monthHideRangeStart != 133) //If this isn't the Last month, December Column BQ
                    {
                        if (monthHideRangeStart != 132) //(normal month hiding through the year) As long as this is not Column BP -  Nov MTD
                        {
                            //Hide next week to the current month
                            beginningCol = String.Format("{0}1", ConvertColumn(weekHideRangeStart));
                            endCol = String.Format("{0}1", ConvertColumn(monthHideRangeStart - 1));
                            exRange = exRange = (Excel.Range)exApp.get_Range(beginningCol, endCol);
                            exRange.EntireColumn.Hidden = true;

                            //hide current month to ytd
                            beginningCol = String.Format("{0}1", ConvertColumn(monthHideRangeStart + 1));
                            endCol = String.Format("{0}1", ConvertColumn(133)); //134 = BR and is the YTD column, so hide the range prior which would be Dec MTD column BQ
                            exRange = exRange = (Excel.Range)exApp.get_Range(beginningCol, endCol);
                            exRange.EntireColumn.Hidden = true;
                        }
                        else //we are in November
                        {
                            //Hide next week to the current month
                            beginningCol = String.Format("{0}1", ConvertColumn(weekHideRangeStart + 1));
                            endCol = String.Format("{0}1", ConvertColumn(monthHideRangeStart - 1));
                            exRange = exRange = (Excel.Range)exApp.get_Range(beginningCol, endCol);
                            exRange.EntireColumn.Hidden = true;



                            //hide December MTD
                            beginningCol = String.Format("{0}1", ConvertColumn(133)); // 133 = BQ Dec MTD
                            endCol = String.Format("{0}1", ConvertColumn(133)); //134 = BR and is the YTD column, so hide the range prior which would be Dec MTD column BQ
                            exRange = exRange = (Excel.Range)exApp.get_Range(beginningCol, endCol);
                            exRange.EntireColumn.Hidden = true;

                        }
                    }
                    else //we are in DEC
                    {
                        //Hide next week to the current month
                        beginningCol = String.Format("{0}1", ConvertColumn(weekHideRangeStart));
                        endCol = String.Format("{0}1", ConvertColumn(monthHideRangeStart - 1));
                        exRange = exRange = (Excel.Range)exApp.get_Range(beginningCol, endCol);
                        exRange.EntireColumn.Hidden = true;


                        //hide December MTD
                        //beginningCol = String.Format("{0}1", ConvertColumn(133)); // 133 = BQ Dec MTD
                        //endCol = String.Format("{0}1", ConvertColumn(133)); //134 = BR and is the YTD column, so hide the range prior which would be Dec MTD column BQ
                        //exRange = exRange = (Excel.Range)exApp.get_Range(beginningCol, endCol);
                        //exRange.EntireColumn.Hidden = true;
                    }


                    break;

            }
            #endregion




        }
        #endregion

        #region GetData
        /// <summary>
        /// Gets a list of Vendors
        /// </summary>
        /// <returns>All Vendors excluding the Administrator = 0</returns>
        private static List<tblVendor> GetVendors()
        {
            List<tblVendor> vendors = new List<tblVendor>();
            using (ConstellationEntities data = new ConstellationEntities())
            {
                //exclude the Administrator = 0 for VendorId
                vendors = data.tblVendors.Where(v => v.VendorId != 0).ToList();
            }
            return vendors;
        }

        /// <summary>
        /// Gets the UDC Codes for the commodities
        /// </summary>
        /// <param name="vendorId">Individual VendorId</param>
        /// <param name="signupType">Electric, Gas or Dual</param>
        /// <returns></returns>
        private static List<string> GetUDCCode(string vendorId, string signupType)
        {
            List<string> udccodes = new List<string>();
            IQueryable<tblMain> query = null;
            using (ConstellationEntities data = new ConstellationEntities())
            {

                switch (signupType)
                {
                    case "Electric":
                        query = from m in data.tblMains
                                where m.VendorId == vendorId
                                && (m.SignUpType == "Electric"
                                || m.SignUpType == "Dual")
                                select m;
                        udccodes = query.Select(d => d.UDCCode).Distinct().ToList();
                        break;

                    case "Gas":

                        //get the gas codes from the UDCCodes
                        query = from m in data.tblMains
                                where m.VendorId == vendorId
                                && m.SignUpType == "Gas"
                                select m;
                        udccodes = query.Select(d => d.UDCCode).Distinct().ToList();

                        //get the dual codes from the GasUDCCodes
                        query = from m in data.tblMains
                                where m.VendorId == vendorId
                                && m.SignUpType == "Dual"
                                select m;
                        List<string> gasudccodes = new List<string>();
                        gasudccodes = query.Select(d => d.GasUDCCode).Distinct().ToList();

                        //merge the code list via dictionary
                        var dict = gasudccodes.ToDictionary(p => p.ToString());
                        foreach (var code in udccodes)
                        {
                            dict[code.ToString()] = code;
                        }
                        var merged = dict.Values.ToList();

                        udccodes = merged;// overwrite the udccodes with the merged list to return
                        break;

                    case "Dual":
                        query = from m in data.tblMains
                                where m.VendorId == vendorId
                                && m.SignUpType == "Dual"
                                select m;
                        udccodes = query.Select(d => d.UDCCode).Distinct().ToList();
                        break;
                }



            }
            return udccodes;
        }
        /// <summary>
        /// Get the counts for the specific vendor, specific commodity, specific UDCCode, date range and the date type passed in
        /// </summary>
        /// <param name="vendorId"></param>
        /// <param name="signupType"></param>
        /// <param name="udccode"></param>
        /// <param name="baseDate"></param>
        /// <param name="dateType"></param>
        /// <returns></returns>
        private static int GetUDCVerifiedCount(string vendorId, string signupType, string udccode, DateTime baseDate, string dateType)
        {
            int count = 0;

            DateTime startDate = new DateTime();
            DateTime endDate = new DateTime();
            DateTime eDate = DateTime.Now;
            switch (dateType)
            {
                case "yesterday":
                    startDate = baseDate.AddDays(-1);
                    endDate = baseDate;
                    break;
                case "weekly":
                    startDate = baseDate.AddDays(-7);
                    endDate = baseDate;
                    break;
                case "mtd":
                    startDate = new DateTime(baseDate.Year, baseDate.Month, 1, 0, 0, 0); //Beginning of current month                    
                    endDate = startDate.AddMonths(1); //Beginning of next month
                    break;
                case "ytd":
                    //if (baseDate <= new DateTime(2014, 7, 1, 0, 0, 0))
                    //{
                    //    startDate = new DateTime(baseDate.Year, 7, 1, 0, 0, 0); //Beginning of July of 2014 current year to avoid test data which will mess up numbers
                    //    endDate = new DateTime(eDate.Year, eDate.Month, eDate.Day, 0, 0, 0); //End of the previous day
                    //}
                    //else
                    //{
                    startDate = new DateTime(baseDate.Year, 1, 1, 0, 0, 0); //Beginning of current year
                    endDate = startDate.AddYears(1); //Beginning of next year
                    //}
                    break;
            }


            IQueryable<tblMain> query = null;
            using (ConstellationEntities data = new ConstellationEntities())
            {
                switch (signupType)
                {
                    case "Electric":
                        query = from m in data.tblMains
                                where m.VendorId == vendorId
                                && (m.SignUpType == "Electric"
                                || m.SignUpType == "Dual")
                                && m.UDCCode == udccode
                                && m.Verified == "1"
                                && m.CallDateTime > startDate
                                && m.CallDateTime < endDate
                                select m;

                        count = query.Count();
                        break;

                    case "Gas":

                        //Get the Gas counts from UDCCode
                        query = from m in data.tblMains
                                where m.VendorId == vendorId
                                && m.SignUpType == "Gas"
                                && m.UDCCode == udccode
                                && m.Verified == "1"
                                && m.CallDateTime > startDate
                                && m.CallDateTime < endDate
                                select m;

                        count = query.Count();

                        //get the Dual Gas Counts from GasUDCCode
                        query = from m in data.tblMains
                                where m.VendorId == vendorId
                                && m.SignUpType == "Dual"
                                && m.GasUDCCode == udccode
                                && m.Verified == "1"
                                && m.CallDateTime > startDate
                                && m.CallDateTime < endDate
                                select m;

                        count += query.Count();
                        break;

                    case "Dual":
                        query = from m in data.tblMains
                                where m.VendorId == vendorId
                                && m.SignUpType == "Dual"
                                && m.UDCCode == udccode
                                && m.Verified == "1"
                                && m.CallDateTime > startDate
                                && m.CallDateTime < endDate
                                select m;
                        count = query.Count();
                        break;
                }

            }
            return count;
        }

        /// <summary>
        /// Get the Global counts for the  specific commodity,  date range and the date type passed in
        /// </summary>
        /// <param name="vendorId"></param>
        /// <param name="signupType"></param>
        /// <param name="udccode"></param>
        /// <param name="baseDate"></param>
        /// <param name="dateType"></param>
        /// <returns></returns>
        private static int GetUDCVerifiedCount(string signupType, DateTime baseDate, string dateType)
        {
            int count = 0;

            DateTime startDate = new DateTime();
            DateTime endDate = new DateTime();
            DateTime eDate = DateTime.Now;
            switch (dateType)
            {
                case "yesterday":
                    startDate = baseDate.AddDays(-1);
                    endDate = baseDate;
                    break;
                case "weekly":
                    startDate = baseDate.AddDays(-7);
                    endDate = baseDate;
                    break;
                case "mtd":
                    startDate = new DateTime(baseDate.Year, baseDate.Month, 1, 0, 0, 0); //Beginning of current month                    
                    endDate = startDate.AddMonths(1); //Beginning of next month
                    break;
                case "ytd":
                    //if (baseDate <= new DateTime(2014, 7, 1, 0, 0, 0))
                    //{
                    //    startDate = new DateTime(baseDate.Year, 7, 1, 0, 0, 0); //Beginning of July of 2014 current year to avoid test data which will mess up numbers
                    //    endDate = new DateTime(eDate.Year, eDate.Month, eDate.Day, 0, 0, 0); //End of the previous day
                    //}
                    //else
                    //{
                    startDate = new DateTime(baseDate.Year, 1, 1, 0, 0, 0); //Beginning of current year
                    endDate = startDate.AddYears(1); //Beginning of next year
                    //}
                    break;
            }


            IQueryable<tblMain> query = null;
            using (ConstellationEntities data = new ConstellationEntities())
            {
                switch (signupType)
                {
                    case "Electric":
                        query = from m in data.tblMains
                                where (m.SignUpType == "Electric" || m.SignUpType == "Dual")
                                && m.Verified == "1"
                                && m.CallDateTime > startDate
                                && m.CallDateTime < endDate
                                select m;

                        count = query.Count();
                        break;

                    case "Gas":

                        //Get the Gas counts from UDCCode
                        query = from m in data.tblMains
                                where m.SignUpType == "Gas"
                                && m.Verified == "1"
                                && m.CallDateTime > startDate
                                && m.CallDateTime < endDate
                                select m;

                        count = query.Count();

                        //get the Dual Gas Counts from GasUDCCode
                        query = from m in data.tblMains
                                where m.SignUpType == "Dual"
                                && m.Verified == "1"
                                && m.CallDateTime > startDate
                                && m.CallDateTime < endDate
                                select m;

                        count += query.Count();
                        break;

                    case "Dual":
                        query = from m in data.tblMains
                                where m.SignUpType == "Dual"
                                && m.Verified == "1"
                                && m.CallDateTime > startDate
                                && m.CallDateTime < endDate
                                select m;
                        count = query.Count();
                        break;
                }

            }
            return count;
        }

        #endregion

        #region Utilities


        /// <summary>
        /// Returns the week of Friday based on the date passed in.
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static DateTime GetFriday(DateTime date)
        {
            switch (date.DayOfWeek)
            {

                case DayOfWeek.Sunday:
                    return date.AddDays(+5);
                case DayOfWeek.Monday:
                    return date.AddDays(+4);
                case DayOfWeek.Tuesday:
                    return date.AddDays(+3);
                case DayOfWeek.Wednesday:
                    return date.AddDays(+2);
                case DayOfWeek.Thursday:
                    return date.AddDays(+1);
                default:// must be Saturday
                    return date.AddDays(+6);
            }
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
        private static void SaveXlsDocument(ref string reportPath, ref string xlsFilename, ref string xlsFilePath, Excel.Workbook exBook, DateTime cDate)
        {
            //Build the file name
            xlsFilename = "ConstellationDashboardDailyReport" + String.Format("{0:yyyyMMdd}", cDate) + ".xlsx";

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

        private static void SendEmail(ref string xlsFilePath, DateTime currentDate, String strToEmail)
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


                mail.From = "reports1@calibrus.com";

                mail.Subject = "Constellation Dashboard Daily Report for " + currentDate.ToString("MMM") + " " + currentDate.ToString("dd") + " " + currentDate.ToString("yyyy") + ".";


                //mail.Body = strMsgBody;
                mail.SendMessage();

            }
            catch (Exception ex)
            {
                SendErrorMessage(ex);
            }

        }

        private static void GetDates(out DateTime CurrentDate, out DateTime MonthStartDate, out DateTime YearStartDate)
        {

            DateTime baseDate;
            DateTimeWS.ReportingDateTimeService dts = null;
            try
            {
                dts = new DateTimeWS.ReportingDateTimeService();
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
            Calibrus.ErrorHandler.Alerting alert = new Calibrus.ErrorHandler.Alerting("ConstellationDashboardDailyReport");
            alert.SendAlert(ex.Source, ex.Message, Environment.MachineName, Environment.UserName, "1.0");
        }
        #endregion
    }
}
