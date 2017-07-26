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

#region Comments
/* 
 * This Report deals with the following
 * 
 * MerryMaids                           TMPSQL2 Client + KYRSQL1 PBX w/ dnis :2289,2290,2291,2292 also, must exclude wavnames to ignore the hang up calls
 * Texpo/YEP/Northstar 			        KYRSQL1 PBX w/ dnis: 6511,6512,6646,7185,6526,4508,6573
 * MiConnection 				        KYRSQL1 PBX w/ dnis: 8911
 * Companion 				            TMPSQL5 Client
 * Chubb 					            TMPSQL5 Client
 * Leslie’s 				            TMPSQL2 Client
 * Hagerty 				                TMPSQL5 Client 
 * Human Arc 				            TMPSQL2 Client
 * Risk Administration Services (RAS) 	TMPSQL5 Client
 * School Claims Services 			    TMPSQL5 Client
 * Society 				                TMPSQL5 Client
 * 
 * to get TotalCalls TotalSeconds
 * 
 * Every SQL Client will be:
 * SELECT count(*) as total, sum(calllength) as seconds
 * FROM tblMain
 * where calldatetime > 'startdate' and calldatetime < 'enddate'
 * 
 * Every PBX Client will be
 * SELECT count(*) as total, sum(calldurationseconds) as seconds
 * FROM CallDetail  
 * where initiateddate > 'startdate' and initiateddate < 'enddate'  
 * and dnis in ('dnis')
 * 
 * MerryMaids PBX requires to exclude the wavnames of the hang ups
 * SELECT count(*) as total, sum(calldurationseconds) as seconds
 * FROM CallDetail  
 * where initiateddate > 'startdate' and initiateddate < 'enddate'  
 * and dnis in ('dnis') //NOT IN THE WavName LIST
 */
#endregion

namespace CalibrusIVRDailyReport
{
    public class Report
    {
        public static object na = System.Reflection.Missing.Value;

        public enum PBXClients
        {
            MerryMaids,
            MiConnection,
            TexpoYEPNorthstar
        }

        public static void Main(string[] args)
        {
            string rootPath = string.Empty;
            string mailRecipientTO = string.Empty;


            //get report interval
            DateTime startDate = new DateTime();
            DateTime endDate = new DateTime();

            //start to  build the form pathing
            string xlsFilename = string.Empty;
            string xlsFilePath = string.Empty;

            if (args.Length > 0)
            {
                if (DateTime.TryParse(args[0], out startDate))
                {
                    endDate = startDate.AddDays(1);
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
                GetDailyRange(out startDate, out endDate);
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

                string sheetName = String.Format("{0}", "IVRDaily");
                exSheet.Name = sheetName.Length > 30 ? sheetName.Substring(0, 30) : sheetName; //force length of sheet name due to excel constraints
                exSheet.Select(na);

                //write excel worksheet
                SetReportHeaders(ref exApp, String.Format("{0}", startDate.ToString("MMMM dd yyyy")));
                WriteReport(ref exApp, startDate, endDate);

                //Save the xls Report
                SaveXlsDocument(ref rootPath, ref xlsFilename, ref xlsFilePath, exBook, startDate);

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
            SendEmail(ref xlsFilePath, startDate, endDate, mailRecipientTO);



        }

        #region Excel
        public static void SetReportHeaders(ref Excel.Application exApp, string header)
        {
            Excel.Range exRange = null;
            //headings
            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn("A", 1), new RangeColumn("C", 1), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter,
                14, true, false, false);
            exRange.Merge(na);
            exRange.Value2 = header;

            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn("A", 2), new RangeColumn("A", 2), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                14, true, false, false);
            exRange.Font.Underline = true;
            exRange.Interior.ColorIndex = 15;//grey
            exRange.Value2 = "Clients";

            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn("B", 2), new RangeColumn("B", 2), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
               14, true, false, false);
            exRange.Font.Underline = true;
            exRange.Interior.ColorIndex = 15;//grey
            exRange.Value2 = "Total Calls";

            exRange = RangeHelper.GetFormattedRange(ref exApp, new RangeColumn("C", 2), new RangeColumn("C", 2), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft,
                14, true, false, false);
            exRange.Font.Underline = true;
            exRange.Interior.ColorIndex = 15;//grey
            exRange.Value2 = "Total Seconds";
        }
        public static void WriteReport(ref Excel.Application exApp, DateTime startDate, DateTime endDate)
        {
            Excel.Range exRange = null;
            int rowInitialize = 3; //initial seed for the row data
            int row = 0;// where we start the row data

            row = rowInitialize;  //set the row for the data

            //Get MerryMaids TMPSQL2 information first
            var merrymaidscalls = GetMerryMaidsData(startDate, endDate);
            int mmCallTotal = 0;
            int? mmCallSeconds = 0;

            //Build a wavname list
            List<String> wavs = new List<string>();

            foreach (var item in merrymaidscalls)//TMPSQL2 values for MerryMaids
            {
                //Total Calls
                mmCallTotal = item.CallTotal == 0 ? 0 : item.CallTotal;

                //Total Seconds
                mmCallSeconds = item.CallSeconds == 0 ? 0 : item.CallSeconds;

                foreach (var wav in item.WavNames)
                {
                    wavs.Add(wav.WaveName);
                }
            }


            //Get  the PBX Clients Data
            Array pbxclient = Enum.GetValues(typeof(PBXClients));
            foreach (PBXClients client in pbxclient)
            {
                var pbxcalls = GetPBXData(startDate, endDate, client.ToString(), wavs);
                foreach (var item in pbxcalls)
                {
                    //Clients
                    exRange = RangeHelper.GetRange(ref exApp, new RangeColumn("A", row), new RangeColumn("A", row));
                    exRange.Value2 = client.ToString();

                    //Total Calls
                    exRange = RangeHelper.GetRange(ref exApp, new RangeColumn("B", row), new RangeColumn("B", row));
                    if (client.ToString() == "MerryMaids")
                    {
                        mmCallTotal += item.CallTotal == 0 ? 0 : item.CallTotal;
                        exRange.Value2 = mmCallTotal.ToString();
                    }
                    else
                    {
                        exRange.Value2 = item.CallTotal == 0 ? "0" : item.CallTotal.ToString();
                    }

                    //Total Seconds
                    exRange = RangeHelper.GetRange(ref exApp, new RangeColumn("C", row), new RangeColumn("C", row));
                    if (client.ToString() == "MerryMaids")
                    {
                        mmCallSeconds += item.CallSeconds == 0 ? 0 : item.CallSeconds;
                        exRange.Value2 = mmCallSeconds.ToString();
                    }
                    else
                    {
                        exRange.Value2 = item.CallSeconds == 0 ? "0" : item.CallSeconds.ToString();
                    }

                    row++;
                }
            }

            //get the SQL Client Data
            var chubbcalls = GetChubbData(startDate, endDate);
            foreach (var item in chubbcalls)
            {

                //Clients
                exRange = RangeHelper.GetRange(ref exApp, new RangeColumn("A", row), new RangeColumn("A", row));
                exRange.Value2 = "Chubb";

                //Total Calls
                exRange = RangeHelper.GetRange(ref exApp, new RangeColumn("B", row), new RangeColumn("B", row));
                exRange.Value2 = item.CallTotal == 0 ? "0" : item.CallTotal.ToString();

                //Total Seconds
                exRange = RangeHelper.GetRange(ref exApp, new RangeColumn("C", row), new RangeColumn("C", row));
                exRange.Value2 = item.CallSeconds == 0 ? "0" : item.CallSeconds.ToString();

                row++;
            }
            var companioncalls = GetCompanionData(startDate, endDate);
            foreach (var item in companioncalls)
            {

                //Clients
                exRange = RangeHelper.GetRange(ref exApp, new RangeColumn("A", row), new RangeColumn("A", row));
                exRange.Value2 = "Companion";

                //Total Calls
                exRange = RangeHelper.GetRange(ref exApp, new RangeColumn("B", row), new RangeColumn("B", row));
                exRange.Value2 = item.CallTotal == 0 ? "0" : item.CallTotal.ToString();

                //Total Seconds
                exRange = RangeHelper.GetRange(ref exApp, new RangeColumn("C", row), new RangeColumn("C", row));
                exRange.Value2 = item.CallSeconds == 0 ? "0" : item.CallSeconds.ToString();

                row++;
            }
            var hagertycalls = GetHagertyData(startDate, endDate);
            foreach (var item in hagertycalls)
            {

                //Clients
                exRange = RangeHelper.GetRange(ref exApp, new RangeColumn("A", row), new RangeColumn("A", row));
                exRange.Value2 = "Hagerty";

                //Total Calls
                exRange = RangeHelper.GetRange(ref exApp, new RangeColumn("B", row), new RangeColumn("B", row));
                exRange.Value2 = item.CallTotal == 0 ? "0" : item.CallTotal.ToString();

                //Total Seconds
                exRange = RangeHelper.GetRange(ref exApp, new RangeColumn("C", row), new RangeColumn("C", row));
                exRange.Value2 = item.CallSeconds == 0 ? "0" : item.CallSeconds.ToString();

                row++;
            }
            var humanarccalls = GetHumanArcData(startDate, endDate);
            foreach (var item in humanarccalls)
            {

                //Clients
                exRange = RangeHelper.GetRange(ref exApp, new RangeColumn("A", row), new RangeColumn("A", row));
                exRange.Value2 = "HumanArc";

                //Total Calls
                exRange = RangeHelper.GetRange(ref exApp, new RangeColumn("B", row), new RangeColumn("B", row));
                exRange.Value2 = item.CallTotal == 0 ? "0" : item.CallTotal.ToString();

                //Total Seconds
                exRange = RangeHelper.GetRange(ref exApp, new RangeColumn("C", row), new RangeColumn("C", row));
                exRange.Value2 = item.CallSeconds == 0 ? "0" : item.CallSeconds.ToString();

                row++;
            }
            var lesliepoolscalls = GetLesliePoolData(startDate, endDate);
            foreach (var item in lesliepoolscalls)
            {

                //Clients
                exRange = RangeHelper.GetRange(ref exApp, new RangeColumn("A", row), new RangeColumn("A", row));
                exRange.Value2 = "Leslie Pools";

                //Total Calls
                exRange = RangeHelper.GetRange(ref exApp, new RangeColumn("B", row), new RangeColumn("B", row));
                exRange.Value2 = item.CallTotal == 0 ? "0" : item.CallTotal.ToString();

                //Total Seconds
                exRange = RangeHelper.GetRange(ref exApp, new RangeColumn("C", row), new RangeColumn("C", row));
                exRange.Value2 = item.CallSeconds == 0 ? "0" : item.CallSeconds.ToString();

                row++;
            }

            //RAS db taken offline
            //var rascalls = GetRASData(startDate, endDate);
            //foreach (var item in rascalls)
            //{

            //    //Clients
            //    exRange = RangeHelper.GetRange(ref exApp, new RangeColumn("A", row), new RangeColumn("A", row));
            //    exRange.Value2 = "RAS";

            //    //Total Calls
            //    exRange = RangeHelper.GetRange(ref exApp, new RangeColumn("B", row), new RangeColumn("B", row));
            //    exRange.Value2 = item.CallTotal == 0 ? "0" : item.CallTotal.ToString();

            //    //Total Seconds
            //    exRange = RangeHelper.GetRange(ref exApp, new RangeColumn("C", row), new RangeColumn("C", row));
            //    exRange.Value2 = item.CallSeconds == 0 ? "0" : item.CallSeconds.ToString();

            //    row++;
            //}
            var schoolscalls = GetSchoolsData(startDate, endDate);
            foreach (var item in schoolscalls)
            {

                //Clients
                exRange = RangeHelper.GetRange(ref exApp, new RangeColumn("A", row), new RangeColumn("A", row));
                exRange.Value2 = "Schools Claim Service";

                //Total Calls
                exRange = RangeHelper.GetRange(ref exApp, new RangeColumn("B", row), new RangeColumn("B", row));
                exRange.Value2 = item.CallTotal == 0 ? "0" : item.CallTotal.ToString();

                //Total Seconds
                exRange = RangeHelper.GetRange(ref exApp, new RangeColumn("C", row), new RangeColumn("C", row));
                exRange.Value2 = item.CallSeconds == 0 ? "0" : item.CallSeconds.ToString();

                row++;
            }
            var societycalls = GetSocietyData(startDate, endDate);
            foreach (var item in societycalls)
            {

                //Clients
                exRange = RangeHelper.GetRange(ref exApp, new RangeColumn("A", row), new RangeColumn("A", row));
                exRange.Value2 = "Society";

                //Total Calls
                exRange = RangeHelper.GetRange(ref exApp, new RangeColumn("B", row), new RangeColumn("B", row));
                exRange.Value2 = item.CallTotal == 0 ? "0" : item.CallTotal.ToString();

                //Total Seconds
                exRange = RangeHelper.GetRange(ref exApp, new RangeColumn("C", row), new RangeColumn("C", row));
                exRange.Value2 = item.CallSeconds == 0 ? "0" : item.CallSeconds.ToString();

                row++;
            }

            RangeHelper.GetRange(ref exApp, new RangeColumn("A", 1), new RangeColumn("C", 1)).EntireColumn.AutoFit();



        }

        #endregion

        #region GetData

        private static List<DataStore> GetPBXData(DateTime startDate, DateTime endDate, string PBXClient, List<String> WavNamesList)
        {
            List<DataStore> calls = new List<DataStore>();
            i3_eicEntities entities = new i3_eicEntities();

            string dnislist = string.Empty;//holds the Dnis list from App.config file
            IQueryable<CallDetail> query = null; //instantiate the query object of the CallDetail table

            switch (PBXClient)
            {

                /* we need to exlcude the Hang Up calls by excluding the wavanames
                * SELECT count(*) as total, sum(calldurationseconds) as seconds
                * FROM CallDetail
                * where initiateddate > 'startDate' and initiateddate < 'endDate'
                * and dnis in ('dnislist')
                * and CallId NOT in ('wavNamesList')
                */
                case "MerryMaids":
                    //get the specific dnis
                    dnislist = ConfigurationManager.AppSettings[PBXClient + "Dnis"].ToString();

                    //Get data for the specific client (MerryMaids excludes the wavnames from the TMPSQL2 table to ignore the hang up calls)
                    query = from m in entities.CallDetails
                            where m.InitiatedDate > startDate
                         && m.InitiatedDate < endDate
                         && dnislist.Contains(m.DNIS)
                         && !WavNamesList.Contains(m.CallId)
                            select m;
                    break;
                /*
                 * SELECT count(*) as total, sum(calldurationseconds) as seconds
                 * FROM CallDetail
                 * where initiateddate > 'startDate' and initiateddate < 'endDate'
                 * and dnis in ('dnislist')
                 */
                case "MiConnection":
                    //get the specific dnis
                    dnislist = ConfigurationManager.AppSettings[PBXClient + "Dnis"].ToString();

                    //Get data for the specific client
                    query = from m in entities.CallDetails
                            where m.InitiatedDate > startDate
                         && m.InitiatedDate < endDate
                         && dnislist.Contains(m.DNIS)
                            select m;
                    break;
                /*
                 * SELECT count(*) as total, sum(calldurationseconds) as seconds
                 * FROM CallDetail
                 * where initiateddate > 'startDate' and initiateddate < 'endDate'
                 * and dnis in ('dnislist')
                 */
                case "TexpoYEPNorthstar":
                    //get the specific dnis
                    dnislist = ConfigurationManager.AppSettings[PBXClient + "Dnis"].ToString();

                    //Get data for the specific client
                    query = from m in entities.CallDetails
                            where m.InitiatedDate > startDate
                         && m.InitiatedDate < endDate
                         && dnislist.Contains(m.DNIS)
                            select m;
                    break;
            }

            //get total calls
            var total = query.Count();

            //Test to see if we have no records            
            var seconds = (int?)0;

            if (total == 0)
            {
                seconds = 0; // set to 0
            }
            else
            {
                seconds = query.Sum(item => item.CallDurationSeconds); // summate the calllength
            }


            DataStore myData = new DataStore();
            myData.CallSeconds = seconds;
            myData.CallTotal = total;
            calls.Add(myData);

            return calls.ToList();
        }

        private static List<DataStore> GetChubbData(DateTime startDate, DateTime endDate)
        {
            /*
             * SELECT count(*) as total, sum(calllength) as seconds
             * FROM [Chubb].[dbo].[tblMain]
             * where calldatetime > 'startdate' and calldatetime < 'enddate'
             */

            List<DataStore> calls = new List<DataStore>();
            ChubbEntities entities = new ChubbEntities();

            //Get data for the specific client            
            var query = from m in entities.tblMains
                        where m.CallDateTime > startDate
                     && m.CallDateTime < endDate
                        select m;
            var total = query.Count();
            //Test to see if we have no records            
            var seconds = (int?)0;
            if (total == 0)
            {
                seconds = 0; // set to 0
            }
            else
            {
                seconds = query.Sum(item => item.CallLength); // summate the calllength
            }

            DataStore myData = new DataStore();
            myData.CallSeconds = seconds;
            myData.CallTotal = total;
            calls.Add(myData);


            return calls.ToList();
        }
        private static List<DataStore> GetCompanionData(DateTime startDate, DateTime endDate)
        {

            /*
            * SELECT count(*) as total, sum(calllength) as seconds
            * FROM [Companion].[dbo].[tblMain]
            * where calldatetime > 'startdate' and calldatetime < 'enddate'
            */
            List<DataStore> calls = new List<DataStore>();
            CompanionEntities entities = new CompanionEntities();

            //Get data for the specific client  
            var query = from m in entities.tblMains
                        where m.CallDateTime > startDate
                     && m.CallDateTime < endDate
                        select m;
            var total = query.Count();
            //Test to see if we have no records            
            var seconds = (int?)0;
            if (total == 0)
            {
                seconds = 0; // set to 0
            }
            else
            {
                seconds = query.Sum(item => item.CallLength); // summate the calllength
            }

            DataStore myData = new DataStore();
            myData.CallSeconds = seconds;
            myData.CallTotal = total;
            calls.Add(myData);

            return calls.ToList();
        }
        private static List<DataStore> GetHagertyData(DateTime startDate, DateTime endDate)
        {

            /*
            * SELECT count(*) as total, sum(calllength) as seconds
            * FROM [Hagerty].[dbo].[tblMain]
            * where calldatetime > 'startdate' and calldatetime < 'enddate'
            */
            List<DataStore> calls = new List<DataStore>();
            HagertyEntities entities = new HagertyEntities();

            //Get data for the specific client
            var query = from m in entities.tblMains
                        where m.CallDateTime > startDate
                     && m.CallDateTime < endDate
                        select m;
            var total = query.Count();
            //Test to see if we have no records            
            var seconds = (int?)0;
            if (total == 0)
            {
                seconds = 0; // set to 0
            }
            else
            {
                seconds = query.Sum(item => item.CallLength); // summate the calllength
            }

            DataStore myData = new DataStore();
            myData.CallSeconds = seconds;
            myData.CallTotal = total;
            calls.Add(myData);

            return calls.ToList();
        }
        private static List<DataStore> GetHumanArcData(DateTime startDate, DateTime endDate)
        {
            /*
           * SELECT count(*) as total, sum(calllength) as seconds
           * FROM [HumanArc].[dbo].[tblMain]
           * where calldatetime > 'startdate' and calldatetime < 'enddate'
           */
            List<DataStore> calls = new List<DataStore>();
            HumanArcEntities entities = new HumanArcEntities();

            //Get data for the specific client
            var query = from m in entities.tblMains
                        where m.CallDateTime > startDate
                     && m.CallDateTime < endDate
                        select m;
            var total = query.Count();
            //Test to see if we have no records            
            var seconds = (int?)0;
            if (total == 0)
            {
                seconds = 0; // set to 0
            }
            else
            {
                seconds = query.Sum(item => item.CallLength); // summate the calllength
            }

            DataStore myData = new DataStore();
            myData.CallSeconds = seconds;
            myData.CallTotal = total;
            calls.Add(myData);

            return calls.ToList();
        }
        private static List<DataStore> GetLesliePoolData(DateTime startDate, DateTime endDate)
        {
            /*
            * SELECT count(*) as total, sum(calllength) as seconds
            * FROM [LesliePools].[dbo].[tblMain]
            * where calldatetime > 'startdate' and calldatetime < 'enddate'
            */
            List<DataStore> calls = new List<DataStore>();
            LesliesPoolEntities entities = new LesliesPoolEntities();

            //Get data for the specific client
            var query = from m in entities.tblMains
                        where m.CallDateTime > startDate
                     && m.CallDateTime < endDate
                        select m;
            var total = query.Count();

            //Test to see if we have no records            
            var seconds = (int?)0;
            if (total == 0)
            {
                seconds = 0; // set to 0
            }
            else
            {
                seconds = query.Sum(item => item.CallLength); // summate the calllength
            }

            DataStore myData = new DataStore();
            myData.CallSeconds = seconds;
            myData.CallTotal = total;
            calls.Add(myData);

            return calls.ToList();
        }
        private static List<DataStore> GetMerryMaidsData(DateTime startDate, DateTime endDate)
        {
            /*
            * SELECT count(*) as total, sum(IvrTimeSeconds) as seconds
            * FROM [MerryMaids].[dbo].[tblMain]
            * where calldatetime > 'startdate' and calldatetime < 'enddate'
            */
            List<DataStore> calls = new List<DataStore>();
            MerryMaidsEntities entities = new MerryMaidsEntities();

            //Get data for the specific client
            var query = from m in entities.tblMains
                        where m.CallDateTime > startDate
                     && m.CallDateTime < endDate
                        select m;
            var total = query.Count(); //Get total calls from the result set
            var allwavnames = query.Select(w => w.Wavname).ToList();//get a list of WavNames from the result set

            //Test to see if we have no records            
            var seconds = (int?)0;
            if (total == 0)
            {
                seconds = 0; // set to 0
            }
            else
            {
                seconds = query.Sum(item => item.IvrTimeSeconds); // summate the IvrTimeSeconds
            }

            DataStore myData = new DataStore();
            myData.CallSeconds = seconds;
            myData.CallTotal = total;

            //loop through allwavnames
            foreach (var wavs in allwavnames)
            {
                WavName wavname = new WavName(wavs.ToString());
                myData.WavNames.Add(wavname);//add them to the call object
            }

            calls.Add(myData);



            return calls.ToList();
        }

        //RAS db taken offline
        //private static List<DataStore> GetRASData(DateTime startDate, DateTime endDate)
        //{
        //    /*
        //    * SELECT count(*) as total, sum(calllength) as seconds
        //    * FROM [RAS].[dbo].[tblMain]
        //    * where calldatetime > 'startdate' and calldatetime < 'enddate'
        //    */
        //    List<DataStore> calls = new List<DataStore>();
        //    RASEntities entities = new RASEntities();

        //    //Get data for the specific client
        //    var query = from m in entities.tblMains
        //                where m.CallDateTime > startDate
        //             && m.CallDateTime < endDate
        //                select m;
        //    var total = query.Count();

        //    //Test to see if we have no records            
        //    var seconds = (int?)0;
        //    if (total == 0)
        //    {
        //        seconds = 0; // set to 0
        //    }
        //    else
        //    {
        //        seconds = query.Sum(item => item.CallLength); // summate the calllength
        //    }

        //    DataStore myData = new DataStore();
        //    myData.CallSeconds = seconds;
        //    myData.CallTotal = total;
        //    calls.Add(myData);

        //    return calls.ToList();
        //}

        private static List<DataStore> GetSchoolsData(DateTime startDate, DateTime endDate)
        {
            /*
            * SELECT count(*) as total, sum(calllength) as seconds
            * FROM [SchoolsClaimsService].[dbo].[tblMain]
            * where calldatetime > 'startdate' and calldatetime < 'enddate'
            */
            List<DataStore> calls = new List<DataStore>();
            SchoolsEntities entities = new SchoolsEntities();

            //Get data for the specific client
            var query = from m in entities.tblMains
                        where m.CallDateTime > startDate
                     && m.CallDateTime < endDate
                        select m;
            var total = query.Count();


            //Test to see if we have no records            
            var seconds = (int?)0;
            if (total == 0)
            {
                seconds = 0; // set to 0
            }
            else
            {
                seconds = query.Sum(item => item.CallLength); // summate the calllength
            }

            DataStore myData = new DataStore();
            myData.CallSeconds = seconds;
            myData.CallTotal = total;
            calls.Add(myData);

            return calls.ToList();
        }
        private static List<DataStore> GetSocietyData(DateTime startDate, DateTime endDate)
        {
            /*
            * SELECT count(*) as total, sum(calllength) as seconds
            * FROM [Society].[dbo].[tblMain]
            * where calldatetime > 'startdate' and calldatetime < 'enddate'
            */
            List<DataStore> calls = new List<DataStore>();
            SocietyEntities entities = new SocietyEntities();

            //Get data for the specific client
            var query = from m in entities.tblMains
                        where m.CallDateTime > startDate
                     && m.CallDateTime < endDate
                        select m;
            var total = query.Count();

            //Test to see if we have no records            
            var seconds = (int?)0;
            if (total == 0)
            {
                seconds = 0; // set to 0
            }
            else
            {
                seconds = query.Sum(item => item.CallLength); // summate the calllength
            }

            DataStore myData = new DataStore();
            myData.CallSeconds = seconds;
            myData.CallTotal = total;
            calls.Add(myData);

            return calls.ToList();
        }

        #endregion

        #region Utilities
        /// <summary>
        /// Saves XLS workbook document to a folder named Processed in the directory where the .exe is run
        /// </summary>
        /// <param name="mainRecord"></param>
        /// <param name="reportPath"></param>
        /// <param name="xlsFilename"></param>
        /// <param name="xlsFilePath"></param>
        /// <param name="exBook"></param>
        private static void SaveXlsDocument(ref string reportPath, ref string xlsFilename, ref string xlsFilePath, Excel.Workbook exBook, DateTime sDate)
        {
            //Build the file name
            xlsFilename = "CalibrusIVRCalls" + String.Format("{0:yyyyMMdd}", sDate) + ".xlsx";

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

        private static void SendEmail(ref string xlsFilePath, DateTime sDate, DateTime eDate, String strToEmail)
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

                mail.Subject = "Calibrus IVR Daily Report for the range of " + String.Format("{0:yyyyMMdd}", sDate) + "-" + String.Format("{0:yyyyMMdd}", eDate);


                //mail.Body = strMsgBody;
                mail.SendMessage();

            }
            catch (Exception ex)
            {
                SendErrorMessage(ex);
            }


        }
        private static void GetDailyRange(out DateTime startDate, out DateTime endDate)
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

            startDate = new DateTime(baseDate.Year, baseDate.Month, baseDate.Day, 0, 0, 0).AddDays(-1); //previous day
            endDate = new DateTime(baseDate.Year, baseDate.Month, baseDate.Day, 0, 0, 0);   //current day
        }
        private static void SendErrorMessage(Exception ex)
        {
            Calibrus.ErrorHandler.Alerting alert = new Calibrus.ErrorHandler.Alerting("CalibrusIVRDailyReport");
            alert.SendAlert(ex.Source, ex.Message, Environment.MachineName, Environment.UserName, "1.0");
        }
        #endregion
    }


}
