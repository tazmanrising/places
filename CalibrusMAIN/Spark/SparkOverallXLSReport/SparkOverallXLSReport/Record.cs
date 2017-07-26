using System;

namespace SparkOverallXLSReport
{
    internal class Record : IDisposable
    {
        //Class which holds the data we are going to report on
        public string Utility;

        public string CommodityType;
        public string UtilityAccountNumber;
        public string CustomerType;
        public string NameKey;
        public string ServiceFirstName;
        public string ServiceLastName;
        public string ServiceAddress1;
        public string ServiceCity;
        public string ServiceState;
        public string ServiceZip;
        public string ServiceCounty;
        public string ServiceEmail;
        public string ServicePhone;
        public string BillingFirstName;
        public string BillingLastName;
        public string BillingAddress1;
        public string BillingCity;
        public string BillingState;
        public string BillingZip;
        public string BillingCounty;
        public string BillingEmail;
        public string BillingPhone;
        public string Language;
        public string ProductOffering;
        public decimal CommodityPrice;
        public int? TermMonths;
        public decimal? MonthlyFee;
        public decimal? ETF;
        public string Marketer;
        public string ExternalSalesID;
        public string SalesChannel;
        public string SalesAgent;
        public DateTime? SoldDate;
        public string RateClass;
        public string MeterNumber;
        public int? TPVverificationid;
        public string LDCCode;
        public string UtilitySalesChannelName;

        public Record(string utility,
         string commodityType,
         string utilityAccountNumber,
         string customerType,
         string nameKey,
         string serviceFirstName,
         string serviceLastName,
         string serviceAddress1,
         string serviceCity,
         string serviceState,
         string serviceZip,
         string serviceCounty,
         string serviceEmail,
         string servicePhone,
         string billingFirstName,
         string billingLastName,
         string billingAddress1,
         string billingCity,
         string billingState,
         string billingZip,
         string billingCounty,
         string billingEmail,
         string billingPhone,
         string language,
         string productOffering,
         decimal commodityPrice,
         int? termMonths,
         decimal? monthlyFee,
         decimal? etf,
         string marketer,
         string externalSalesID,
         string salesChannel,
         string salesAgent,
         DateTime? soldDate,
         string rateClass,
         string meterNumber,
         int? tpvverificationid,
         string ldccode,
         string utilitySalesChannelName)
        {
            Utility = utility;
            CommodityType = commodityType.ToLower() == "gas" ? "NaturalGas" : commodityType;
            UtilityAccountNumber = utilityAccountNumber;
            CustomerType = customerType;
            NameKey = nameKey;
            ServiceFirstName = serviceFirstName;
            ServiceLastName = serviceLastName;
            ServiceAddress1 = serviceAddress1;
            ServiceCity = serviceCity;
            ServiceState = serviceState;
            ServiceZip = serviceZip;
            ServiceCounty = serviceCounty;
            ServiceEmail = serviceEmail;
            ServicePhone = servicePhone;
            BillingFirstName = billingFirstName;
            BillingLastName = billingLastName;
            BillingAddress1 = billingAddress1;
            BillingCity = billingCity;
            BillingState = billingState;
            BillingZip = billingZip;
            BillingCounty = billingCounty;
            BillingEmail = billingEmail;
            BillingPhone = billingPhone;
            Language = language;
            ProductOffering = productOffering;
            CommodityPrice = commodityPrice;
            TermMonths = termMonths;
            MonthlyFee = monthlyFee;
            ETF = etf;
            Marketer = marketer;
            ExternalSalesID = externalSalesID;
            SalesChannel = salesChannel.ToLower() == "dtd" ? "D2D" : salesChannel;
            SalesAgent = salesAgent;
            SoldDate = soldDate;
            RateClass = rateClass;
            MeterNumber = meterNumber;
            TPVverificationid = tpvverificationid;
            LDCCode = ldccode;
            UtilitySalesChannelName = utilitySalesChannelName;
        }

        private bool disposed = false;

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