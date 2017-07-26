using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Calibrus.ErrorHandler;
using Calibrus.Ftp;
using System.Diagnostics;
using System.Security;

namespace SparkDataDumpCSV
{
    public class DataDumpReport
    {

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
            DateTime MonthStartDate = new DateTime();
            DateTime MonthEndDate = new DateTime();

            //start to  build the form pathing
            string csvFilename = string.Empty;
            string csvFilePath = string.Empty;

            if (args.Length > 0)
            {
                if (DateTime.TryParse(args[0], out StartDate))
                {
                    GetDates(out StartDate, out EndDate, out MonthStartDate, out MonthEndDate);
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
                GetDates(out StartDate, out EndDate, out MonthStartDate, out MonthEndDate);
            }

            //grab values from app.config
            rootPath = ConfigurationManager.AppSettings["rootPath"].ToString();

            hostName = ConfigurationManager.AppSettings["hostName"].ToString();
            userName = ConfigurationManager.AppSettings["userName"].ToString();
            password = ConfigurationManager.AppSettings["password"].ToString();

            #region DailyCSV
            try
            {
                string strBuffer = "";//holds data to put into report for each column


                //Build Record Object
                List<Record> Records = GetRecords(StartDate, EndDate);

                if (Records.Count > 0)
                {
                    csvFilePath = rootPath;
                    //MMddyyyy_CALIBRUS_DATAFILE.csv
                    csvFilename = StartDate.ToString("MM_dd_yyyy") + "_CALIBRUS_DATAFILE.csv";
                    csvFilePath += csvFilename; //build filepath

                    //Write Report
                    WriteCSV(Records, csvFilePath, ref strBuffer);


                    //FTP report                    
                    FTPFile(rootPath, csvFilename, csvFilePath, hostName, userName, password);

                }
            }
            catch (Exception ex)
            {
                SendErrorMessage(ex);
                //throw ex;
            }
            finally
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
            #endregion DailyCSV

            #region MonthlyCSV
            if (EndDate.Day == 1)//only run this on the first of the month EndDate represents the current day this runs which is for the previous day/month
            {
                try
                {
                    string strBuffer = "";//holds data to put into report for each column


                    //Build Record Object
                    List<Record> Records = GetRecords(MonthStartDate, MonthEndDate);

                    if (Records.Count > 0)
                    {
                        csvFilePath = rootPath;
                        //MMddyyyy_CALIBRUS_DATAFILE.csv
                        csvFilename = MonthStartDate.ToString("MM_yyyy") + "_CALIBRUS_DATAFILE.csv";
                        csvFilePath += csvFilename; //build filepath

                        //Write Report
                        WriteCSV(Records, csvFilePath, ref strBuffer);


                        //FTP report                    
                        FTPFile(rootPath, csvFilename, csvFilePath, hostName, userName, password);

                    }
                }
                catch (Exception ex)
                {
                    SendErrorMessage(ex);
                    //throw ex;
                }
                finally
                {
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                }
            }

            #endregion MonthlyCSV

        }
        #endregion Main

        #region Write CSV
        public static void WriteCSV(List<Record> records, string csvFilePath, ref string strBuffer)
        {
            StreamWriter sw = File.CreateText(csvFilePath);

            //Used to hold content for report
            StringBuilder sb = new StringBuilder();

            #region Header

            //CallDateTime            
            strBuffer = "CallDateTime|";
            sb.AppendFormat(strBuffer);

            //TotalCallTime    
            strBuffer = "TotalCallTime|";
            sb.AppendFormat(strBuffer);

            //VendorName            
            strBuffer = "VendorName|";
            sb.AppendFormat(strBuffer);

            //VendorNumber           
            strBuffer = "VendorNumber|";
            sb.AppendFormat(strBuffer);

            //LdcCode            
            strBuffer = "LdcCode|";
            sb.AppendFormat(strBuffer);

            //UtilityTypeName            
            strBuffer = "UtilityTypeName|";
            sb.AppendFormat(strBuffer);

            //ProgramName            
            strBuffer = "ProgramName|";
            sb.AppendFormat(strBuffer);

            //Verified            
            strBuffer = "Verified|";
            sb.AppendFormat(strBuffer);

            //AccountNumber            
            strBuffer = "AccountNumber|";
            sb.AppendFormat(strBuffer);

            //PremiseTypeName            
            strBuffer = "PremiseTypeName|";
            sb.AppendFormat(strBuffer);

            //AuthorizationFirstName            
            strBuffer = "AuthorizationFirstName|";
            sb.AppendFormat(strBuffer);

            //AuthorizationLastName            
            strBuffer = "AuthorizationLastName|";
            sb.AppendFormat(strBuffer);

            //ServiceAddress            
            strBuffer = "ServiceAddress|";
            sb.AppendFormat(strBuffer);

            //ServiceCity            
            strBuffer = "ServiceCity|";
            sb.AppendFormat(strBuffer);

            //ServiceState            
            strBuffer = "ServiceState|";
            sb.AppendFormat(strBuffer);

            //ServiceZip            
            strBuffer = "ServiceZip|";
            sb.AppendFormat(strBuffer);

            //ServiceCounty            
            strBuffer = "ServiceCounty|";
            sb.AppendFormat(strBuffer);

            //Email            
            strBuffer = "Email|";
            sb.AppendFormat(strBuffer);

            //Btn            
            strBuffer = "Btn|";
            sb.AppendFormat(strBuffer);

            //AccountFirstName            
            strBuffer = "AccountFirstName|";
            sb.AppendFormat(strBuffer);

            //AccountLastName            
            strBuffer = "AccountLastName|";
            sb.AppendFormat(strBuffer);

            //BillingAddress            
            strBuffer = "BillingAddress|";
            sb.AppendFormat(strBuffer);

            //BillingCity            
            strBuffer = "BillingCity|";
            sb.AppendFormat(strBuffer);

            //BillingState            
            strBuffer = "BillingState|";
            sb.AppendFormat(strBuffer);

            //BillingZip            
            strBuffer = "BillingZip|";
            sb.AppendFormat(strBuffer);

            //BillingCounty            
            strBuffer = "BillingCounty|";
            sb.AppendFormat(strBuffer);

            //Language            
            strBuffer = "Language|";
            sb.AppendFormat(strBuffer);

            //ProgramCode            
            strBuffer = "ProgramCode|";
            sb.AppendFormat(strBuffer);

            //Rate            
            strBuffer = "Rate|";
            sb.AppendFormat(strBuffer);

            //Term            
            strBuffer = "Term|";
            sb.AppendFormat(strBuffer);

            //Msf            
            strBuffer = "Msf|";
            sb.AppendFormat(strBuffer);

            //Etf            
            strBuffer = "Etf|";
            sb.AppendFormat(strBuffer);

            //AgentId            
            strBuffer = "AgentId|";
            sb.AppendFormat(strBuffer);

            //Name            
            strBuffer = "Name|";
            sb.AppendFormat(strBuffer);

            //TpvAgentId            
            strBuffer = "TpvAgentId|";
            sb.AppendFormat(strBuffer);

            //TpvAgentName            
            strBuffer = "TpvAgentName|";
            sb.AppendFormat(strBuffer);

            //RateClass            
            strBuffer = "RateClass|";
            sb.AppendFormat(strBuffer);

            //MainId            
            strBuffer = "MainId|";
            sb.AppendFormat(strBuffer);

            //Concern            
            strBuffer = "Concern|";
            sb.AppendFormat(strBuffer);

            //ExternalSalesId   
            strBuffer = "ExternalSalesId|";
            sb.AppendFormat(strBuffer);

            //Brand   
            strBuffer = "Brand|";
            sb.AppendFormat(strBuffer);

            //ProductName   
            strBuffer = "ProductName|";
            sb.AppendFormat(strBuffer);

            //MarketerCode   
            strBuffer = "MarketerCode|";
            sb.AppendFormat(strBuffer);

            //OfficeName   
            strBuffer = "OfficeName|";
            sb.AppendFormat(strBuffer);

            //Source   
            strBuffer = "Source\r\n";
            sb.AppendFormat(strBuffer);

            #endregion Header

            #region Data
            //SPARK has an issue with carriage returns being injected on data inserts from people copy and pasting values from a document.
            //I have implemented a method StripCarriageReturns() which will address this using regex.
            //This is a band-aid using a shot gun approach on all values we return with a few exceptions

            int recordCount = records.Count();
            int recordCursor = 0;
            foreach (Record item in records)
            {
                recordCursor++;
                //CallDateTime            
                strBuffer = string.Format("{0:MM/dd/yyyy hh:mm:ss tt}", item.CallDateTime);
                sb.AppendFormat("{0}|", strBuffer);

                //TotalCallTime
                strBuffer = StripCarriageReturns(item.TotalCallTime);
                sb.AppendFormat("{0}|", strBuffer);

                //VendorName            
                strBuffer = StripCarriageReturns(item.VendorName);
                sb.AppendFormat("{0}|", strBuffer);

                //VendorNumber            
                strBuffer = StripCarriageReturns(item.VendorNumber);
                sb.AppendFormat("{0}|", strBuffer);

                //LdcCode            
                strBuffer = StripCarriageReturns(item.LdcCode);
                sb.AppendFormat("{0}|", strBuffer);

                //UtilityTypeName            
                strBuffer = StripCarriageReturns(item.UtilityTypeName);
                sb.AppendFormat("{0}|", strBuffer);

                //ProgramName            
                strBuffer = StripCarriageReturns(item.ProgramName);
                sb.AppendFormat("{0}|", strBuffer);

                //Verified            
                strBuffer = StripCarriageReturns(item.Verified);
                sb.AppendFormat("{0}|", strBuffer);

                //AccountNumber            
                strBuffer = StripCarriageReturns(item.AccountNumber);
                sb.AppendFormat("{0}|", strBuffer);

                //PremiseTypeName            
                strBuffer = StripCarriageReturns(item.PremiseTypeName);
                sb.AppendFormat("{0}|", strBuffer);

                //AuthorizationFirstName            
                strBuffer = StripCarriageReturns(item.AuthorizationFirstName);
                sb.AppendFormat("{0}|", strBuffer);

                //AuthorizationLastName            
                strBuffer = StripCarriageReturns(item.AuthorizationLastName);
                sb.AppendFormat("{0}|", strBuffer);

                //ServiceAddress            
                strBuffer = StripCarriageReturns(item.ServiceAddress);
                sb.AppendFormat("{0}|", strBuffer);

                //ServiceCity            
                strBuffer = StripCarriageReturns(item.ServiceCity);
                sb.AppendFormat("{0}|", strBuffer);

                //ServiceState            
                strBuffer = StripCarriageReturns(item.ServiceState);
                sb.AppendFormat("{0}|", strBuffer);

                //ServiceZip            
                strBuffer = StripCarriageReturns(item.ServiceZip);
                sb.AppendFormat("{0}|", strBuffer);

                //ServiceCounty            
                strBuffer = StripCarriageReturns(item.ServiceCounty);
                sb.AppendFormat("{0}|", strBuffer);

                //Email            
                strBuffer = StripCarriageReturns(item.Email);
                sb.AppendFormat("{0}|", strBuffer);

                //Btn            
                strBuffer = StripCarriageReturns(item.Btn);
                sb.AppendFormat("{0}|", strBuffer);

                //AccountFirstName            
                strBuffer = StripCarriageReturns(item.AccountFirstName);
                sb.AppendFormat("{0}|", strBuffer);

                //AccountLastName            
                strBuffer = StripCarriageReturns(item.AccountLastName);
                sb.AppendFormat("{0}|", strBuffer);

                //BillingAddress            
                strBuffer = StripCarriageReturns(item.BillingAddress);
                sb.AppendFormat("{0}|", strBuffer);

                //BillingCity            
                strBuffer = StripCarriageReturns(item.BillingCity);
                sb.AppendFormat("{0}|", strBuffer);

                //BillingState            
                strBuffer = StripCarriageReturns(item.BillingState);
                sb.AppendFormat("{0}|", strBuffer);

                //BillingZip            
                strBuffer = StripCarriageReturns(item.BillingZip);
                sb.AppendFormat("{0}|", strBuffer);

                //BillingCounty            
                strBuffer = StripCarriageReturns(item.BillingCounty);
                sb.AppendFormat("{0}|", strBuffer);

                //Language            
                strBuffer = StripCarriageReturns(item.Language);
                sb.AppendFormat("{0}|", strBuffer);

                //ProgramCode            
                strBuffer = StripCarriageReturns(item.ProgramCode);
                sb.AppendFormat("{0}|", strBuffer);

                //Rate  
                double dbRate = Convert.ToDouble(item.Rate);
                strBuffer = string.Format("{0:F4}", dbRate);
                sb.AppendFormat("{0}|", strBuffer);

                //Term 
                strBuffer = StripCarriageReturns(item.Term.ToString());
                sb.AppendFormat("{0}|", strBuffer);

                //Msf            
                double dbMsf = Convert.ToDouble(item.Msf);
                strBuffer = string.Format("{0:F4}", dbMsf);
                sb.AppendFormat("{0}|", strBuffer);

                //Etf            
                double dbEtf = Convert.ToDouble(item.Etf);
                strBuffer = string.Format("{0:F4}", dbEtf);
                sb.AppendFormat("{0}|", strBuffer);

                //AgentId            
                strBuffer = StripCarriageReturns(item.AgentId);
                sb.AppendFormat("{0}|", strBuffer);

                //SalesChannelName            
                strBuffer = StripCarriageReturns(item.SalesChannelName);
                sb.AppendFormat("{0}|", strBuffer);

                //TpvAgentId            
                strBuffer = StripCarriageReturns(item.TpvAgentId);
                sb.AppendFormat("{0}|", strBuffer);

                //TpvAgentName            
                strBuffer = StripCarriageReturns(item.TpvAgentName);
                sb.AppendFormat("{0}|", strBuffer);

                //RateClass            
                strBuffer = StripCarriageReturns(item.RateClass);
                sb.AppendFormat("{0}|", strBuffer);

                //MainId            
                strBuffer = StripCarriageReturns(item.MainId);
                sb.AppendFormat("{0}|", strBuffer);

                //Concern            
                strBuffer = StripCarriageReturns(item.Concern);
                sb.AppendFormat("{0}|", strBuffer);

                //ExternalSalesId            
                strBuffer = StripCarriageReturns(item.ExternalSalesId);
                sb.AppendFormat("{0}|", strBuffer);

                //Brand            
                strBuffer = StripCarriageReturns(item.Brand);
                sb.AppendFormat("{0}|", strBuffer);

                //ProductName
                strBuffer = StripCarriageReturns(item.ProductName);
                sb.AppendFormat("{0}|", strBuffer);

                //MarketerCode
                strBuffer = StripCarriageReturns(item.MarketerCode);
                sb.AppendFormat("{0}|", strBuffer);

                //OfficeName            
                strBuffer = StripCarriageReturns(item.OfficeName);
                sb.AppendFormat("{0}|", strBuffer);
                //Source            
                strBuffer = item.Source;
                if (recordCursor < recordCount)
                {
                    sb.AppendFormat("{0}{1}", strBuffer, Environment.NewLine);
                }
                else
                {
                    sb.AppendFormat("{0}", strBuffer); //otherwise end of records no newline to put
                }
            }

            #endregion Data

            //save file 
            sw.WriteLine(sb.ToString());
            sb.Remove(0, sb.Length);
            sw.Close();
            sw.Dispose();


        }

        #endregion

        #region Get Data
        private static List<Record> GetRecords(DateTime sDate, DateTime eDate)
        {
            List<Record> records = new List<Record>();
            try
            {
                using (SparkEntities entities = new SparkEntities())
                {
                    //SELECT uty.LdcCode, ut.UtilityTypeName, od.AccountNumber,pt.PremiseTypeName,
                    //        m.AuthorizationFirstName, m.AuthorizationLastName,od.ServiceAddress, od.ServiceCity, od.ServiceState,
                    //        od.ServiceZip, od.ServiceCounty, m.Email, m.Btn, m.AccountFirstName, m.AccountLastName, od.BillingAddress,
                    //        od.BillingCity, od.BillingState, od.BillingZip, od.BillingCounty, u.Language, p.ProgramCode, p.ProgramName,
                    //        p.Rate, p.Term, p.Msf, p.Etf, v.VendorName, v.VendorNumber, u.AgentId, sc.Name, m.TpvAgentId, u.FirstName, u.LastName,m.CallDateTime,
                    //        od.RateClass, 
                    //        case when m.Verified = '1' Then 'Good Sale' else 'No Sale' end as Verified,
                    //        m.Concern, o.OfficeName,m.MainId, m.TotalTime, od.OrderDetailId,  p.ProgramName, b.BrandName, o.MarketerCode
                    //FROM [Spark].[v1].[Main] m
                    //    join [Spark].[v1].[OrderDetail] od on m.mainid = od.MainId
                    //    join [Spark].[v1].[Program] p on od.ProgramId = p.ProgramId
                    //    join [Spark].[v1].[UnitOfMeasure] uom on p.UnitOfMeasureId = uom.UnitOfMeasureId
                    //    join [Spark].[v1].[UtilityType] ut on p.UtilityTypeId = ut.UtilityTypeId
                    //    join [Spark].[v1].[Utility] uty on p.UtilityId = uty.UtilityId
                    //    join [Spark].[v1].[AccountNumberType] ant on p.AccountNumberTypeId = ant.AccountNumberTypeId
                    //    join [Spark].[v1].[User] u on m.UserId = u.UserId
                    //    join [Spark].[v1].[Vendor] v on v.VendorId = u.VendorId
                    //    join [Spark].[v1].[PremiseType] pt on p.PremiseTypeId = pt.PremiseTypeId
                    //    join [Spark].[v1].[Office] o on o.OfficeId = u.OfficeId
                    //    join [Spark].[v1].[SalesChannel] sc on o.SalesChannelId = sc.SalesChannelId 
                    //    join [Spark].[v1].[Brand] b on b.BrandId = p.BrandId           
                    //WHERE m.CallDateTime > '02/01/2016'
                    //    and m.CallDateTime < '02/02/2016'
                    //ORDER BY  m.CallDateTime


                    var query = from m in entities.Mains
                                join od in entities.OrderDetails on m.MainId equals od.MainId
                                join p in entities.Programs on od.ProgramId equals p.ProgramId
                                join uom in entities.UnitOfMeasures on p.UnitOfMeasureId equals uom.UnitOfMeasureId
                                join uty in entities.UtilityTypes on p.UtilityTypeId equals uty.UtilityTypeId
                                join ut in entities.Utilities on p.UtilityId equals ut.UtilityId
                                join ant in entities.AccountNumberTypes on p.AccountNumberTypeId equals ant.AccountNumberTypeId
                                join u in entities.Users on m.UserId equals u.UserId
                                join v in entities.Vendors on u.VendorId equals v.VendorId
                                join pt in entities.PremiseTypes on p.PremiseTypeId equals pt.PremiseTypeId
                                join o in entities.Offices on u.OfficeId equals o.OfficeId
                                join sc in entities.SalesChannels on o.SalesChannelId equals sc.SalesChannelId
                                join b in entities.Brands on p.BrandId equals b.BrandId
                                
                                where m.CallDateTime > sDate
                                && m.CallDateTime < eDate

                                select new
                                {
                                    CallDateTime = m.CallDateTime,
                                    VendorName = v.VendorName,
                                    VendorNumber = v.VendorNumber,
                                    LdcCode = ut.LdcCode,
                                    UtilityTypeName = uty.UtilityTypeName,
                                    ProgramName = p.ProgramName,
                                    Verified = m.Verified,
                                    AccountNumber = od.AccountNumber,
                                    PremiseTypeName = pt.PremiseTypeName,
                                    AuthorizationFirstName = m.AuthorizationFirstName,
                                    AuthorizationLastName = m.AuthorizationLastName,
                                    ServiceAddress = od.ServiceAddress,
                                    ServiceCity = od.ServiceCity,
                                    ServiceState = od.ServiceState,
                                    ServiceZip = od.ServiceZip,
                                    ServiceCounty = od.ServiceCounty,
                                    Email = m.Email,
                                    Btn = m.Btn,
                                    AccountFirstName = m.AccountFirstName,
                                    AccountLastName = m.AccountLastName,
                                    BillingAddress = od.BillingAddress,
                                    BillingCity = od.BillingCity,
                                    BillingState = od.BillingState,
                                    BillingZip = od.BillingZip,
                                    BillingCounty = od.BillingCounty,
                                    //Language = u.Language,
                                    Dnis = m.Dnis,
                                    ProgramCode = p.ProgramCode,
                                    Rate = p.Rate,
                                    Term = p.Term,
                                    Msf = p.Msf,
                                    Etf = p.Etf,
                                    AgentId = u.AgentId,
                                    SalesChannelName = sc.Name,
                                    TpvAgentId = m.TpvAgentId,
                                    TpvAgentName = u.FirstName + " " + u.LastName,
                                    RateClass = od.RateClass,
                                    Concern = m.Concern,
                                    OfficeName = o.OfficeName,
                                    MainId = m.MainId,
                                    TotalCallTime = m.TotalTime,
                                    ExternalSalesId = od.OrderDetailId,
                                    ProductName = p.ProgramName,
                                    BrandName = b.BrandName,
                                    MarketerCode = o.MarketerCode,
                                    SourceId = m.SourceId
                                };

                    foreach (var item in query)
                    {
                        Record myrecord = new Record();

                        myrecord.CallDateTime = item.CallDateTime;
                        myrecord.VendorName = IsValueNull(item.VendorName) ? string.Empty : item.VendorName.ToUpper();
                        myrecord.VendorNumber = item.VendorNumber;
                        myrecord.LdcCode = IsValueNull(item.LdcCode) ? string.Empty : item.LdcCode.ToUpper();
                        myrecord.UtilityTypeName = IsValueNull(item.UtilityTypeName) ? string.Empty : item.UtilityTypeName.ToUpper();
                        myrecord.ProgramName = IsValueNull(item.ProductName) ? string.Empty : item.ProgramName.ToUpper();
                        myrecord.Verified = (item.Verified == "1" ? "Good Sale" : "No Sale").ToUpper();

                        //UtilityId LdcCode
                        //15 NSG
                        //16 PGL
                        string utilityAccountNumber = IsValueNull(item.AccountNumber) ? string.Empty : item.AccountNumber.ToUpper();
                        switch (item.LdcCode)
                        {
                            case "NSG":
                            case "PGL":
                                if (utilityAccountNumber.Length == 15)
                                    utilityAccountNumber = utilityAccountNumber.Insert(10, "-");

                                break;
                        }

                        myrecord.AccountNumber = utilityAccountNumber;
                        myrecord.PremiseTypeName = IsValueNull(item.PremiseTypeName) ? string.Empty : item.PremiseTypeName.ToUpper();
                        myrecord.AuthorizationFirstName = IsValueNull(item.AccountFirstName) ? string.Empty : item.AuthorizationFirstName.ToUpper();
                        myrecord.AuthorizationLastName = IsValueNull(item.AuthorizationLastName) ? string.Empty : item.AuthorizationLastName.ToUpper();
                        myrecord.ServiceAddress = IsValueNull(item.ServiceAddress) ? string.Empty : item.ServiceAddress.ToUpper();
                        myrecord.ServiceCity = IsValueNull(item.ServiceCity) ? string.Empty : item.ServiceCity.ToUpper();
                        myrecord.ServiceState = IsValueNull(item.ServiceState) ? string.Empty : item.ServiceState.ToUpper();
                        myrecord.ServiceZip = item.ServiceZip;
                        myrecord.ServiceCounty = IsValueNull(item.ServiceCounty) ? string.Empty : item.ServiceCounty.ToUpper();
                        myrecord.Email = IsValueNull(item.Email) ? string.Empty : item.Email.ToUpper();
                        myrecord.Btn = item.Btn;
                        myrecord.AccountFirstName = IsValueNull(item.AccountFirstName) ? string.Empty : item.AccountFirstName.ToUpper();
                        myrecord.AccountLastName = IsValueNull(item.AccountLastName) ? string.Empty : item.AccountLastName.ToUpper();
                        myrecord.BillingAddress = IsValueNull(item.BillingAddress) ? string.Empty : item.BillingAddress.ToUpper();
                        myrecord.BillingCity = IsValueNull(item.BillingCity) ? string.Empty : item.BillingCity.ToUpper();
                        myrecord.BillingState = IsValueNull(item.BillingState) ? string.Empty : item.BillingState.ToUpper();
                        myrecord.BillingZip = item.BillingZip;
                        myrecord.BillingCounty = IsValueNull(item.BillingCounty) ? string.Empty : item.BillingCounty.ToUpper();
                        //myrecord.Language = IsValueNull(item.Language) ? string.Empty : item.Language.ToUpper();
                        if (!IsValueNull(item.Dnis))
                        {
                            switch (item.Dnis)
                            {
                                //English: 1324, 1322                        
                                case "1324":
                                case "1322":
                                    myrecord.Language = "ENGLISH";
                                    break;
                                //Spanish: 1325, 1323
                                case "1325":
                                case "1323":
                                    myrecord.Language = "SPANISH";
                                    break;
                            }
                        }
                        else
                        {
                            myrecord.Language = string.Empty;
                        }

                        myrecord.ProgramCode = IsValueNull(item.ProgramCode) ? string.Empty : item.ProgramCode.ToUpper();
                        myrecord.Rate = item.Rate;
                        myrecord.Term = item.Term;
                        myrecord.Msf = item.Msf ?? 0M;
                        myrecord.Etf = item.Etf ?? 0M;
                        myrecord.AgentId = IsValueNull(item.AgentId) ? string.Empty : item.AgentId.ToUpper();
                        myrecord.SalesChannelName = IsValueNull(item.SalesChannelName) ? string.Empty : item.SalesChannelName.ToUpper();
                        myrecord.TpvAgentId = IsValueNull(item.TpvAgentId) ? string.Empty : item.TpvAgentId.ToUpper();
                        myrecord.TpvAgentName = IsValueNull(item.TpvAgentName) ? string.Empty : item.TpvAgentName.ToUpper();
                        myrecord.RateClass = IsValueNull(item.RateClass) ? string.Empty : item.RateClass.ToUpper();
                        myrecord.Concern = IsValueNull(item.Concern) ? string.Empty : item.Concern.ToUpper();
                        myrecord.OfficeName = IsValueNull(item.OfficeName) ? string.Empty : item.OfficeName.ToUpper();
                        myrecord.MarketerCode = IsValueNull(item.MarketerCode) ? string.Empty : item.MarketerCode.ToUpper();

                        myrecord.MainId = item.MainId.ToString();

                        //Converting Seconds to Decimal
                        ////Convert int? TotalCallTime to Double
                        //double dbTotalCallTime = Convert.ToDouble(item.TotalCallTime);

                        ////Convert TotalCallTime to timespan
                        //TimeSpan timespan = TimeSpan.FromSeconds(dbTotalCallTime);

                        ////Format timespan to decimal with  two places for totalminutes
                        //string formatted = timespan.TotalMinutes.ToString("#.00");
                        //myrecord.TotalCallTime = formatted;

                        myrecord.TotalCallTime = IsValueNull(item.TotalCallTime.ToString()) ? "0" : item.TotalCallTime.ToString();
                        myrecord.ExternalSalesId = "CAL" + item.ExternalSalesId.ToString();
                        myrecord.Brand = IsValueNull(item.BrandName) ? string.Empty : item.BrandName.ToUpper();
                        myrecord.ProductName = IsValueNull(item.ProductName) ? string.Empty : item.ProductName.ToUpper();

                        //Find Source if Applicable

                        var Source =  (from s in entities.Sources
                                      where s.Id == item.SourceId
                                           select s.Name).FirstOrDefault();
                        myrecord.Source = Source;

                        records.Add(myrecord);
                    }

                }
            }
            catch (Exception ex)
            {
                SendErrorMessage(ex);
                //throw ex;
            }

            return records;

        }
        #endregion

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


        /// <summary>
        /// Removes Carriage Returns, paragraph returns, etc in a string.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private static string StripCarriageReturns(string input)
        {
            if (!string.IsNullOrEmpty(input))
            {
                input = Regex.Replace(input, @"[\u000A\u000B\u000C\u000D\u2028\u2029\u0085]+", String.Empty);
                return input;
            }
            return string.Empty;
        }

        public static string StripAllNonNumerics(string input)
        {
            if (!string.IsNullOrEmpty(input))
            {
                input = Regex.Replace(input, @"[^\d]", "");// strip all non-numeric chars
                return input;
            }
            return string.Empty;
        }

        private static void FTPFile(string reportPath, string filename, string filePath, string HostName, string UserName, string Password)
        {

            filePath = string.Format(reportPath + filename);
            try
            {
                Calibrus.Ftp.Upload ftp = new Calibrus.Ftp.Upload();
                ftp.Host = new Uri(string.Format("ftp://{0}/", HostName));
                ftp.UserName = UserName;
                ftp.Password = Password;
                ftp.UploadFile(filePath, filename);
            }
            catch (Exception ex)
            {
                SendErrorMessage(ex);
            }
        }

        private static void GetDates(out DateTime StartDate, out DateTime EndDate, out DateTime MonthStartDate, out DateTime MonthEndDate)
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
            EndDate = new DateTime(baseDate.Year, baseDate.Month, baseDate.Day, 0, 0, 0);//current day
            MonthStartDate = new DateTime(baseDate.Year, baseDate.Month, 1, 0, 0, 0).AddMonths(-1);//Previous Month
            MonthEndDate = new DateTime(baseDate.Year, baseDate.Month, 1, 0, 0, 0);//current Month
        }

        private static void SendErrorMessage(Exception ex)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendFormat("ex:{0}, innerEx:{1}", ex.Message, ex.InnerException == null ? "" : ex.InnerException.Message);

            Calibrus.ErrorHandler.Alerting alert = new Calibrus.ErrorHandler.Alerting("SparkDataDumpCSV");
            alert.SendAlert(ex.Source, sb.ToString(), Environment.MachineName, Environment.UserName, Environment.Version.ToString());
        }

        #endregion Utilities
    }
}
