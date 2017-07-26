using System;

namespace SparkNaturalGasAndElectricXLSReport
{
    internal class Record : IDisposable
    {
        //Class which holds the data we are going to report on
        public string ConfirmationNumber;

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
        public string AccountFirstName;
        public string AccountLastName;
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
        public string UtilitySalesChannelName;
        public string ServiceReferenceNumber;
        public string SwitchDate;
        public string CreditCheck;
        public string BillMethod;
        public string CommissionNumber;
        public string ProgramDescription;
        public string DefaultPricingPlanDescription;
        public string LDCCode;
        public string SupplyZoneDesc;
        public string MLine1_Addr;
        public string MCity_Name;
        public string MState;
        public string MPostal_Code;


        public Record(string confirmationNumber,
         string utility,
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
         string accountFirstName,
         string accountLastName,
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
         string utilitySalesChannelName,
         string serviceReferenceNumber,
         string switchDate,
         bool creditCheck,
         string billMethod,
         string commissionNumber,
         string programDescription,
         string defaultPricingPlanDescription,
         string ldcCode,
         string supplyZoneDesc,
         string mLine1_Addr,
         string mCity_Name,
         string mState,
         string mPostal_Code)
        {
            ConfirmationNumber = confirmationNumber;
            Utility = utility;
            CommodityType = commodityType == "NaturalGas" ? "Gas" : commodityType;
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
            AccountFirstName = accountFirstName;
            AccountLastName = accountLastName;
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
            UtilitySalesChannelName = utilitySalesChannelName;
            ServiceReferenceNumber = serviceReferenceNumber;
            SwitchDate = switchDate;
            CreditCheck = creditCheck == true ? "Y" : "N";
            BillMethod = billMethod;
            CommissionNumber = commissionNumber;
            ProgramDescription = programDescription;
            DefaultPricingPlanDescription = defaultPricingPlanDescription;
            LDCCode = ldcCode;
            SupplyZoneDesc = supplyZoneDesc;
            MLine1_Addr = mLine1_Addr;
            MCity_Name = mCity_Name;
            MState = mState;
            MPostal_Code = mPostal_Code;
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