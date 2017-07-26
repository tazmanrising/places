using System;
using System.Collections.Generic;

namespace LibertyNightlyVerifiedTPVCSV
{
    public class Record : IDisposable
    {
        public int? MainId { get; set; }
        public string Language { get; set; }
        public string Btn { get; set; }
        public string SalesAgentId { get; set; }
        public string SalesChannel { get; set; }
        public string AuthorizationFirstName { get; set; }
        public string AuthorizationLastName { get; set; }
        public string AccountNumber { get; set; }
        public string OfferCode { get; set; }
        public string Rate { get; set; }
        public string MonthlyTerm { get; set; }
        public string RateEffectiveDate { get; set; }
        public string ServiceAddress { get; set; }
        public string ServiceAddress2 { get; set; }
        public string ServiceCity { get; set; }
        public string ServiceState { get; set; }
        public string ServiceZip { get; set; }
        public string BillingAddress { get; set; }
        public string BillingAddress2 { get; set; }
        public string BillingCity { get; set; }
        public string BillingState { get; set; }
        public string BillingZip { get; set; }
        public int? VerificationNumber { get; set; }
        public string Verified { get; set; }
        public bool? Commercial { get; set; }
        public string FEIN { get; set; }

        private bool disposed = false;

        public Record(int? mainId, string language, string accountNumber, string btn, string salesAgentId, string salesChannel, string authorizationFirstName, string authorizationLastName,
                        string offerCode, string rate, string monthlyTerm, string rateEffectiveDate, string serviceAddress, string serviceAddress2, string serviceCity,
                        string serviceState, string serviceZip, string billingAddress, string billingAddress2, string billingCity, string billingState, string billingZip,
                        int? verficationNumber, string verified, bool? commercial, string fein)
        {
            MainId = mainId;
            Language = language;
            AccountNumber = accountNumber;
            Btn = btn;
            SalesAgentId = salesAgentId;
            SalesChannel = SalesChannel;
            AuthorizationFirstName = authorizationFirstName;
            AuthorizationLastName = authorizationLastName;
            OfferCode = offerCode;
            Rate = rate;
            MonthlyTerm = monthlyTerm;
            RateEffectiveDate = rateEffectiveDate;
            ServiceAddress = serviceAddress;
            ServiceAddress2 = serviceAddress2;
            ServiceCity = serviceCity;
            ServiceState = serviceState;
            ServiceZip = serviceZip;
            BillingAddress = billingAddress;
            BillingAddress2 = billingAddress2;
            BillingCity = billingCity;
            BillingState = billingState;
            BillingZip = billingZip;
            VerificationNumber = verficationNumber;
            Verified = verified;
            Commercial = commercial;
            FEIN = fein;
        }

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