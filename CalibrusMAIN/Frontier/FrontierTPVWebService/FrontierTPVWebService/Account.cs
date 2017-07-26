using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text.RegularExpressions;


namespace FrontierTPVWebService
{

    /// <summary>
    /// Summary description for Account
    /// </summary>
    public class Account
    {

        public string SalesAgentId { get; set; }
        public string State { get; set; }
        public string CustFirstName { get; set; }
        public string CustLastName { get; set; }
        public string DecisionMaker { get; set; }
        public string CompanyName { get; set; }
        public string Product { get; set; }
        public bool Business { get; set; }

        public PhoneNumber[] PhoneNumbers { get; set; }

        internal protected bool ValidateOrder(out string exception)
        {

            if (String.IsNullOrEmpty(SalesAgentId.Trim()) || String.IsNullOrEmpty(State.Trim()) || String.IsNullOrEmpty(CustFirstName.Trim()) || String.IsNullOrEmpty(CustLastName.Trim()) || String.IsNullOrEmpty(DecisionMaker.Trim()) || String.IsNullOrEmpty(Business.ToString()))
            {
                exception = "Missing Account Parameter";
                return false;
            }

            if (PhoneNumbers == null || PhoneNumbers.Count() == 0)
            {
                exception = "No Phone Numbers Entered";
                return false;
            }

            string patternPhoneNumber = @"\d{10}";
            foreach (PhoneNumber tn in PhoneNumbers)
            {
                if (String.IsNullOrEmpty(tn.Tn) || !Regex.IsMatch(tn.Tn, patternPhoneNumber))
                {
                    exception = "Invalid Phone Number";
                    return false;
                }

                if (tn.PLOCChange == false && tn.PLOCFreeze == false && tn.ILPIntra == false && tn.ILPIntraFreeze == false && tn.PICInter == false && tn.PICInterFreeze == false)
                {
                    exception = String.Format("No Services Selected for {0}", tn.Tn);
                    return false;
                }

            }

            exception = "";
            return true;
        }
    }
}