using System;

namespace ConstellationHomeServicesDailyXLS
{
    internal class Record : IDisposable
    {
        //Class which holds the data we are going to report on

        public int? EnrollmentId;
        public string SalesChannel;
        public int? SalesVendorId;
        public string SalesVendor;
        public string SalesRep;
        public DateTime? DateOfSale;
        public string Utility;
        public string Commodity;
        public string UtilityAccountNumber;
        public string Product;
        public int? AddOns;
        public string Jurisdiction;
        public string FirstName;
        public string LastName;
        public string Email;
        public string ServiceAddress1;
        public string ServiceAddress2;
        public string ServiceCity;
        public string ServiceState;
        public string ServiceZip;
        public string ServicePhone;
        public string BillingAddress1;
        public string BillingAddress2;
        public string BillingCity;
        public string BillingState;
        public string BillingZip;
        public string BillingPhone;
        public string IncludeOnBGEBIll;
        public string ElectricChoiceId;
        public string Dnis;


        public Record(int? enrollmentId,
          string salesChannel,
          int? salesVendorId,
          string salesVendor,
          string salesRep,
          DateTime? dateOfSale,
          string utility,
          string commodity,
          string utilityAccountNumber,
          string product,
          int? addOns,
          string jurisdiction,
          string firstName,
          string lastName,
          string email,
          string serviceAddress1,
          string serviceAddress2,
          string serviceCity,
          string serviceState,
          string serviceZip,
          string servicePhone,
          string billingAddress1,
          string billingAddress2,
          string billingCity,
          string billingState,
          string billingZip,
          string billingPhone,
          string includeOnBGEBill,
          string electricChoiceId,
            string dnis
            )
        {
            EnrollmentId = enrollmentId;
            SalesChannel = salesChannel;
            SalesVendorId = salesVendorId;
            SalesVendor = salesVendor;
            SalesRep = salesRep;
            DateOfSale = dateOfSale;
            Utility = utility;
            Commodity = commodity;
            UtilityAccountNumber = utilityAccountNumber;
            Product = product;
            AddOns = addOns;
            Jurisdiction = jurisdiction;
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            ServiceAddress1 = serviceAddress1;
            ServiceAddress2 = serviceAddress2;
            ServiceCity = serviceCity;
            ServiceState = serviceState;
            ServiceZip = serviceZip;
            ServicePhone = servicePhone;
            BillingAddress1 = billingAddress1;
            BillingAddress2 = billingAddress2;
            BillingCity = billingCity;
            BillingState = billingState;
            BillingZip = billingZip;
            BillingPhone = billingPhone;
            IncludeOnBGEBIll = includeOnBGEBill;
            ElectricChoiceId = electricChoiceId;
            Dnis = dnis;

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