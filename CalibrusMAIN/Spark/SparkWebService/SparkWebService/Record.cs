using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace SparkWebService
{
    /// <summary>
    /// Represents Parent of the Record
    /// To Be inserted into Main table in the Spark db
    /// </summary>
    public class Record : IDisposable
    {
        //Values passed in to look up record information
        //in our db to use for the record insert transaction
        public string AgentId { get; set; }

        public string VendorNumber { get; set; }

        public string Email { get; set; }

        public string RecordLocator { get; set; }

        public string SalesState { get; set; }

        public string AuthorizationFirstName { get; set; }

        public string AuthorizationMiddle { get; set; }

        public string AuthorizationLastName { get; set; }

        public string Btn { get; set; }

        public string CompanyName { get; set; }

        public string CompanyContactFirstName { get; set; }

        public string CompanyContactLastName { get; set; }

        public string CompanyContactTitle { get; set; }

        public string Territory { get; set; }

        public string LeadType { get; set; }

        public string Relation { get; set; }

        public string NumberOfAccounts { get; set; }

        public string AccountFirstName { get; set; }

        public string AccountLastName { get; set; }

        public List<RecordDetail> RecordDetails { get; set; }

        private bool disposed = false;

        protected internal bool ValidateOrder(out string exception)
        {
            if (String.IsNullOrEmpty(AgentId.Trim())
                || String.IsNullOrEmpty(VendorNumber.Trim())
                //|| String.IsNullOrEmpty(Email.Trim())
                //|| String.IsNullOrEmpty(RecordLocator.Trim())
                || String.IsNullOrEmpty(AuthorizationFirstName.Trim())
                || String.IsNullOrEmpty(AuthorizationLastName.Trim())
                || String.IsNullOrEmpty(Btn.Trim())
                //|| String.IsNullOrEmpty(CompanyName.Trim())
                //|| String.IsNullOrEmpty(CompanyContactFirstName.Trim())
                //|| String.IsNullOrEmpty(CompanyContactLastName.Trim())
                //|| String.IsNullOrEmpty(Territory.Trim())
                //|| String.IsNullOrEmpty(LeadType.Trim())
                || String.IsNullOrEmpty(Relation.Trim())
                //|| String.IsNullOrEmpty(NumberOfAccounts.Trim())
                //|| String.IsNullOrEmpty(AccountFirstName.Trim())
                //|| String.IsNullOrEmpty(AccountLastName.Trim())
                )
            {
                exception = "Missing Record Parameter";
                return false;
            }

            //need to look up UserId
            if (!IsThereAUserId(AgentId.Trim(), VendorNumber.Trim()))
            {
                exception = String.Format("No User exists for AgentId: {0} and VendorNumber {1}", AgentId, VendorNumber);
                return false;
            }


            //Check to see that they intend to pass us at least one account
            if (RecordDetails == null || RecordDetails.Count() == 0)
            {
                exception = "No Order Detail Records Entered";
                return false;
            }

            string patternPhoneNumber = @"\d{10}";
            if (String.IsNullOrEmpty(StripAllNonNumerics(Btn)) || !Regex.IsMatch(StripAllNonNumerics(Btn), patternPhoneNumber))
            {
                exception = "Invalid Btn";
                return false;
            }

            foreach (RecordDetail rd in RecordDetails)
            {
                if (String.IsNullOrEmpty(rd.AccountNumber.Trim())
                    || String.IsNullOrEmpty(rd.ProgramCode.Trim())
                    //|| String.IsNullOrEmpty(rd.AccountType.Trim())
                    || String.IsNullOrEmpty(rd.BillingAddress.Trim())
                    || String.IsNullOrEmpty(rd.BillingCity.Trim())
                    || String.IsNullOrEmpty(rd.BillingState.Trim())
                    || String.IsNullOrEmpty(rd.BillingZip.Trim())
                    //|| String.IsNullOrEmpty(rd.CustomerNameKey.Trim())
                    //|| String.IsNullOrEmpty(rd.InCityLimits.Trim())
                    //|| String.IsNullOrEmpty(rd.MeterNumber.Trim())
                    //|| String.IsNullOrEmpty(rd.RateClass.Trim())
                    || String.IsNullOrEmpty(rd.ServiceAddress.Trim())
                    || String.IsNullOrEmpty(rd.ServiceCity.Trim())
                    || String.IsNullOrEmpty(rd.ServiceState.Trim())
                    || String.IsNullOrEmpty(rd.ServiceZip.Trim())
                    || String.IsNullOrEmpty(rd.BillingFirstName.Trim())
                    || String.IsNullOrEmpty(rd.BillingLastName.Trim())                   
                    //|| String.IsNullOrEmpty(rd.ServiceReferenceNumber.Trim())
                    //|| String.IsNullOrEmpty(rd.UtilityType.Trim())
                    )
                {
                    exception = String.Format("Missing Order Detail Record Parameter(s) for AccountNumber: {0}", rd.AccountNumber);
                    return false;
                }

                //Check to make sure there are no special characters or spaces in account number
                if (IsAlphaNumeric(rd.AccountNumber))
                {
                    exception = "AccountNumber parameter is incorrect. No special characters allowed.";
                    return false;
                }

                //need to look up ProgramId
                if (!IsThereAProgramId(rd.ProgramCode.Trim(), VendorNumber.Trim()))
                {
                    exception = String.Format("No Program exists for ProgramCode: {0}", rd.ProgramCode);
                    return false;
                }

                //Get UtilityType Based on ProgramCode and VendorNumber (This will override what they enter)
                rd.UtilityType = GetUtilityTypeName(rd.ProgramCode.Trim(), VendorNumber.Trim());


                //Phone Check for 180 days
                if (PhoneCheck(Btn.Trim(), rd.UtilityType))
                {
                    exception = String.Format("The Phone Number: {0} is already Verified.", Btn.Trim());
                    return false;
                }

                //Account Number Check for 180 days
                if (AccountNumberCheck(rd.AccountNumber.Trim(), rd.UtilityType))
                {
                    exception = String.Format("The {0} Account Number is already Verified.", rd.UtilityType);
                    return false;
                }

                //Existing Customer Check
                if (FuelTypeExistingCustomerAccountCheck(rd.AccountNumber.Trim(), rd.UtilityType))
                {
                    exception = String.Format("This account is an existing customer.");
                    return false;
                }

                //Account Number Check for Length and/or Prefix where applicable
                if (!AccountNumberCheckLength(rd.AccountNumber, rd.UtilityType, rd.ProgramCode))
                {
                    exception = String.Format("The {0} Account Number is invalid.", rd.AccountNumber);
                    return false;
                }

                //Check for PO Box address in Service Address
                if (IsPOBox(rd.ServiceAddress))
                {
                    exception = String.Format("The Service Address: {0} is invalid. P.O. Box addresses not allowed.", rd.ServiceAddress);
                    return false;
                }

                //Make Sure CustomerNameKey is required for Michigan, Massachussetts, and Connecticut
                if (rd.ServiceState.Trim().ToUpper().Contains("CT") || rd.ServiceState.Trim().ToUpper().Contains("MA") || rd.ServiceState.Trim().ToUpper().Contains("MI"))
                {
                    if(String.IsNullOrEmpty(rd.CustomerNameKey))
                    {
                        exception = String.Format("The CustomerNameKey Parameter is required for ServiceState: {0}.", rd.ServiceState);
                        return false;
                    }
                    else if (rd.CustomerNameKey.Length != 4)
                    {
                        exception = String.Format("The CustomerNameKey Parameter has to be four characters.");
                        return false;
                    }
                }

            }

            exception = "";
            return true;
        }

        #region Utilities

        private static string StripAllNonNumerics(string input)
        {
            if (!string.IsNullOrEmpty(input))
            {
                input = Regex.Replace(input, @"[^\d]", "");// strip all non-numeric chars
                return input;
            }
            return string.Empty;
        }

        private static bool IsAlphaNumeric(string input)
        {
            Regex rg = new Regex("^[a-zA-Z0-9]*$");

            //if has non AlpahNumeric char, return false, else return true.
            return rg.IsMatch(input) == true ? false : true;
        }

        private static bool IsPOBox(string input)
        {
            Regex rg = new Regex(@"^(?:Post(?:al)? (?:Office )?|P[. ]?O\.? )?Box\b", RegexOptions.IgnoreCase | RegexOptions.Multiline);

            //if is a PO Box address, return true, else return false.
            return rg.IsMatch(input) == true ? true : false;
        }

        #endregion Utilities

        #region EF Methods to check if passed in values exist in our db (7 methods)

        /// <summary>
        /// Check to see if there is a valid UserId with passed in AgentId and VendorNumber
        /// </summary>
        /// <param name="agentId"></param>
        /// <param name="vendorNumber"></param>
        /// <returns></returns>
        private bool IsThereAUserId(string agentId, string vendorNumber)
        {
            //SELECT u.UserId, v.VendorId, v.VendorNumber
            //FROM [Spark].[v1].[User] as u
            //join [Spark].[v1].[Vendor] as v on v.VendorId = u.VendorId
            //where u.AgentId = '23851TML'
            //and v.VendorNumber = '20'
            //and u.IsActive =1

            bool userexists = false;
            using (SparkEntities data = new SparkEntities())
            {
                var userid = (from u in data.Users
                              join v in data.Vendors on u.VendorId equals v.VendorId
                              where u.AgentId == agentId
                              && v.VendorNumber == vendorNumber
                              && u.IsActive == true
                              select u.UserId).Any();

                if (userid)
                {
                    userexists = true;
                }
            }
            return userexists;
        }

        /// <summary>
        /// Check to see if there is a valid ProgramId with passed in ProgramCode and VendorNumber
        /// </summary>
        /// <param name="programCode"></param>
        /// <param name="vendorNumber"></param>
        /// <returns></returns>
        private bool IsThereAProgramId(string programCode, string vendorNumber)
        {
            DateTime now = DateTime.Now;

            //SELECT p.programid, pv.VendorId, v.VendorId, v.VendorNumber
            //FROM [Spark].[v1].[Program] as p
            //join [Spark].[v1].[ProgramVendor] as pv on p.ProgramId = pv.ProgramId
            //join [Spark].[v1].[Vendor] as v on v.VendorId = pv.VendorId
            //where p.ProgramCode = 'A3'
            //and v.VendorNumber = '1001'
            //and (p.EffectiveStartDate < getdate() and p.EffectiveEndDate > getdate())

            bool programexists = false;
            using (SparkEntities data = new SparkEntities())
            {
                var userid = (from p in data.Programs
                              join pv in data.ProgramVendors on p.ProgramId equals pv.ProgramId
                              join v in data.Vendors on pv.VendorId equals v.VendorId
                              where p.ProgramCode == programCode
                              && v.VendorNumber == vendorNumber
                              && p.EffectiveStartDate < now
                              && p.EffectiveEndDate > now
                              select p.ProgramId).Any();

                if (userid)
                {
                    programexists = true;
                }
            }
            return programexists;
        }

        /// <summary>
        /// Gets utility type name based on ProgramCode and VendorNumber
        /// </summary>
        /// <param name="programCode"></param>
        /// <param name="vendorNumber"></param>
        /// <returns></returns>
        private string GetUtilityTypeName(string programCode, string vendorNumber)
        {
            //SELECT distinct ut.UtilityTypeName
            //FROM [Spark].[v1].[Program] p 
            //JOIN [Spark].[v1].[UtilityType] ut on p.UtilityTypeId = ut.UtilityTypeId
            //JOIN [Spark].[v1].[ProgramVendor] as pv on p.ProgramId = pv.ProgramId
            //JOIN [Spark].[v1].[Vendor] as v on v.VendorId = pv.VendorId
            //WHERE p.ProgramCode = '223'
            //AND v.VendorNumber = '14'  

            string name = string.Empty;
            using (SparkEntities data = new SparkEntities())
            {
                var utilityTypeName = (from p in data.Programs
                                       join ut in data.UtilityTypes on p.UtilityTypeId equals ut.UtilityTypeId
                                       join pv in data.ProgramVendors on p.ProgramId equals pv.ProgramId
                                       join v in data.Vendors on pv.VendorId equals v.VendorId
                                       where p.ProgramCode == programCode
                                       && v.VendorNumber == vendorNumber
                                       select ut.UtilityTypeName).ToList();

                foreach (string item in utilityTypeName.Distinct())
                {
                    name = item;
                }

                //name = utilityTypeName.Distinct().ToString();
            }
            return name;
        }

        /// <summary>
        /// Check to see if a BTN has been used for a verified record in the past 180 days
        /// </summary>
        /// <param name="btn"></param>
        /// <returns></returns>
        private bool PhoneCheck(string btn, string utiltyTypeName)
        {
            DateTime now = DateTime.Now;
            now = now.AddDays(-180);
            //    "SELECT *
            //       FROM v1.[Main]
            //       join vwOrderUtility o on v1.[Main].MainId = o.MainId
            //       where Btn=btn
            //       and CallDateTime>dateadd(d,-180,getdate())
            //       and Verified='1' and UtilityTypeName = utilityTypeName

            bool phonecheck = false;

            using (SparkEntities data = new SparkEntities())
            {
                var userid = (from m in data.Mains
                              join ou in data.vwOrderUtilities on m.MainId equals ou.MainId
                              where m.Btn == btn
                              && m.CallDateTime > now
                              && m.Verified == "1"
                              && ou.UtilityTypeName == utiltyTypeName
                              select m.Btn).Any();

                if (userid)
                {
                    phonecheck = true;
                }
            }
            return phonecheck;
        }

        /// <summary>
        /// Checks AccountNumber for UtilityTypeName in the past 180 days
        /// </summary>
        /// <param name="accountNumber"></param>
        /// <param name="utilityTypeName"></param>
        /// <returns></returns>
        private bool AccountNumberCheck(string accountNumber, string utilityTypeName)
        {

            DateTime now = DateTime.Now;
            now = now.AddDays(-180);

            //SELECT *
            //   FROM v1.[Main] a 
            //   join v1.OrderDetail b on a.MainId=b.MainId
            //   join v1.Program c on b.ProgramId=c.ProgramId
            //   join v1.Utility d on c.UtilityId=d.UtilityId
            //   where b.AccountNumber = accountNumber
            //   and a.CallDateTime > dateadd(d,-180,getdate())
            //   and Verified = '1'              
            //   and c.UtilityTypeId = 1 --1=gas 2=electric

            bool acctnumcheck = false;
            int utilityTypeId = 0;

            if (utilityTypeName == "Electric")
            { utilityTypeId = 2; }
            else
            { utilityTypeId = 1; }

            using (SparkEntities data = new SparkEntities())
            {
                var acctNum = (from m in data.Mains
                               join od in data.OrderDetails on m.MainId equals od.MainId
                               join p in data.Programs on od.ProgramId equals p.ProgramId
                               join u in data.Utilities on p.UtilityId equals u.UtilityId
                               where od.AccountNumber == accountNumber
                               && m.CallDateTime > now
                               && m.Verified == "1"
                               && p.UtilityTypeId == utilityTypeId
                               select m).Any();

                if (acctNum)
                {
                    acctnumcheck = true;
                }
            }
            return acctnumcheck;

        }

        /// <summary>
        /// Check to see if the Account Number is an Existing Customer for the UtilityTypeName
        /// </summary>
        /// <param name="accountNumber"></param>
        /// <param name="utiltyTypeName"></param>
        /// <returns></returns>
        private bool FuelTypeExistingCustomerAccountCheck(string accountNumber, string utiltyTypeName)
        {

            //SELECT *
            //FROM v1.[CurrentCustomer] a
            //where a.AccountNumber=accountnumber
            //and a.Commodity=utilitytypename

            bool currentcustomerexists = false;
            using (SparkEntities data = new SparkEntities())
            {
                var currentcustomer = (from cc in data.CurrentCustomers
                                       where cc.AccountNumber == accountNumber
                                       && cc.Commodity.Contains(utiltyTypeName)
                                       select cc).Any();

                if (currentcustomer)
                {
                    currentcustomerexists = true;
                }
            }
            return currentcustomerexists;
        }


        /// <summary>
        /// Checks AccountNumber for UtilityTypeName for Length and/or Prefix when applicable
        /// </summary>
        /// <param name="accountNumber"></param>
        /// <param name="utilityTypeName"></param>
        /// <returns></returns>
        private bool AccountNumberCheckLength(string accountNumber, string utilityTypeName, string programCode)
        {

            //SELECT DISTINCT  p.State
            //        ,u.LdcCode
            //        ,ut.UtilityTypeName
            //        ,p.UtilityId
            //        ,p.AccountNumberLength 
            //FROM v1.Program p
            //JOIN v1.Utility u on p.UtilityId = u.UtilityId
            //JOIN v1.UtilityType ut on ut.UtilityTypeId = p.UtilityTypeId
            //WHERE U.IsActive = 1
            //AND p.ProgramCode ='223' 
            //AND ut.UtilityTypeId = 1 --1=gas 2=electric

            bool acctnumcheck = false;
            int utilityTypeId = 0;

            if (utilityTypeName == "Electric")
            { utilityTypeId = 2; }
            else
            { utilityTypeId = 1; }

            using (SparkEntities data = new SparkEntities())
            {
                var record = (from p in data.Programs
                              join u in data.Utilities on p.UtilityId equals u.UtilityId
                              join ut in data.UtilityTypes on p.UtilityTypeId equals ut.UtilityTypeId
                              where u.IsActive == true
                              && p.ProgramCode == programCode
                              && p.UtilityTypeId == utilityTypeId
                              select new { p.State, u.LdcCode, ut.UtilityTypeName, p.UtilityId, p.AccountNumberLength }).FirstOrDefault();


                var regexLength = record.AccountNumberLength;
                var accountNumberPattern = string.Empty;
                var prefix = string.Empty;

                if (record.UtilityTypeName == "Gas")
                {
                    if (record.LdcCode == "NYSEG")
                    {
                        regexLength = regexLength - 3;
                        prefix = "N02";
                        accountNumberPattern = "^([Nn]02)(\\w{" + regexLength + "})$";
                        if (Regex.IsMatch(accountNumber, accountNumberPattern))
                        {
                            acctnumcheck = true;
                        }

                    }
                    else if (record.LdcCode == "RG&E")
                    {
                        regexLength = regexLength - 3;
                        prefix = "R02";
                        accountNumberPattern = "^([Rr]02)(\\w{" + regexLength + "})$";
                        if (Regex.IsMatch(accountNumber, accountNumberPattern))
                        {
                            acctnumcheck = true;
                        }
                    }
                    else if (record.LdcCode == "PSEG")
                    {
                        regexLength = regexLength - 2;
                        prefix = "PG";
                        accountNumberPattern = "^([pP][gG])(\\w{" + regexLength + "})$";
                        if (Regex.IsMatch(accountNumber, accountNumberPattern))
                        {
                            acctnumcheck = true;
                        }
                    }
                    else
                    {
                        if (!IsAlphaNumeric(accountNumber))
                        {
                            prefix = null;
                            accountNumberPattern = "^(\\w{" + regexLength + "})$";
                            if (Regex.IsMatch(accountNumber, accountNumberPattern))
                            {
                                acctnumcheck = true;
                            }

                        }
                    }

                }
                else //Electric
                {
                    if (record.LdcCode == "NYSEG")
                    {
                        regexLength = regexLength - 3;
                        prefix = "N01";
                        accountNumberPattern = "^([Nn]01)(\\w{" + regexLength + "})$";
                        if (Regex.IsMatch(accountNumber, accountNumberPattern))
                        {
                            acctnumcheck = true;
                        }
                    }
                    else if (record.LdcCode == "RG&E")
                    {
                        regexLength = regexLength - 3;
                        prefix = "R01";
                        accountNumberPattern = "^([Rr]01)(\\w{" + regexLength + "})$";
                        if (Regex.IsMatch(accountNumber, accountNumberPattern))
                        {
                            acctnumcheck = true;
                        }
                    }
                    else if (record.LdcCode == "PSEG")
                    {
                        regexLength = regexLength - 2;
                        prefix = "PE";
                        accountNumberPattern = "^([pP][eE])(\\w{" + regexLength + "})$";
                        if (Regex.IsMatch(accountNumber, accountNumberPattern))
                        {
                            acctnumcheck = true;
                        }
                    }
                    else
                    {
                        if (!IsAlphaNumeric(accountNumber))
                        {
                            prefix = null;
                            accountNumberPattern = "^(\\w{" + regexLength + "})$";
                            if (Regex.IsMatch(accountNumber, accountNumberPattern))
                            {
                                acctnumcheck = true;
                            }
                        }
                    }
                }
            }
            return acctnumcheck;

        }
        #endregion EF Methods to check if passed in values exist in our db (7 methods)

        //Implement IDisposable.
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    // Free other state (managed objects).
                }
                // Free your own state (unmanaged objects).
                // Set large fields to null.
                disposed = true;
            }
        }

        // Use C# destructor syntax for finalization code.
        ~Record()
        {
            // Simply call Dispose(false).
            Dispose(false);
        }
    }
}