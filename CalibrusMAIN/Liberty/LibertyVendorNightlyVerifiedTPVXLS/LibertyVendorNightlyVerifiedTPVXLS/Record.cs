using System;


namespace LibertyVendorNightlyVerifiedTPVXLS
{
    public class Record : IDisposable
    {

        public int? MainId { get; set; }
        public string AccountNumber { get; set; }
        public string AuthorizationFirstName { get; set; }
        public string AuthorizationLastName { get; set; }
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
        public string Btn { get; set; }
        public string MeterNumber { get; set; }
        public string NameKey { get; set; }
        public string SalesChannelName { get; set; }
        public string MarketState { get; set; }
        public string MarketUtility { get; set; }
        public bool? Commercial { get; set; }
        public string EffectiveStartDate { get; set; }
        public string MonthlyTerm { get; set; }
        public string Rate { get; set; }
        public string SalesAgentId { get; set; }
        public string Email { get; set; }
        public string ServiceNumber { get; set; }
        public string Rate1 { get; set; }
        public string Rate2 { get; set; }
        public string Rate3 { get; set; }
        public string Rate4 { get; set; }
        public string GasAccountNumber { get; set; }
        public string GasMarketUtility { get; set; }
        public string GasRate { get; set; }
        public string GasMonthlyTerm { get; set; }


        private bool disposed = false;

        public Record(int? mainId, string accountNumber, string authorziationFirstName, string authorizationLastName, string serviceAddress, string serviceAddress2, string serviceCity,
                    string serviceState, string serviceZip, string billingAddress, string billingAddress2, string billingCity, string billingState,
                    string billingZip, string btn, string meterNumber, string nameKey, string salesChannelName, string marketState, string marketUtility,
                    bool? commercial, string effectiveStartDate, string monthlyTerm, string rate, string salesAgentId, string email, string serviceNumber,
                    string rate1, string rate2, string rate3, string rate4, string gasAccountNumber, string gasMarketUtility, string gasRate, string gasMonthlyTerm)
        {
            MainId = mainId;
            AccountNumber = accountNumber;
            AuthorizationFirstName = authorziationFirstName;
            AuthorizationLastName = authorizationLastName;
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
            Btn = btn;
            MeterNumber = meterNumber;
            NameKey = nameKey;
            SalesChannelName = salesChannelName;
            MarketState = marketState;
            MarketUtility = marketUtility;
            Commercial = commercial;
            EffectiveStartDate = effectiveStartDate;
            MonthlyTerm = monthlyTerm;
            Rate = rate;
            SalesAgentId = salesAgentId;
            Email = email;
            ServiceNumber = serviceNumber;
            Rate1 = rate1;
            Rate2 = rate2;
            Rate3 = rate3;
            Rate4 = Rate4;
            GasAccountNumber = gasAccountNumber;
            GasMarketUtility = gasMarketUtility;
            GasRate = gasRate;
            GasMonthlyTerm = gasMonthlyTerm;
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