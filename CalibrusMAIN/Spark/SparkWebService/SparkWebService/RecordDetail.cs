using System;

namespace SparkWebService
{
    /// <summary>
    /// Represents children of the Record
    /// To Be inserted into OrderDetail table in the Spark db
    /// </summary>
    public class RecordDetail : IDisposable
    {
        //Values passed in to look up record information
        //in our db to use for the record insert transaction
        public string ProgramCode { get; set; }

        public string UtilityType { get; set; }

        public string AccountType { get; set; }

        public string AccountNumber { get; set; }

        public string MeterNumber { get; set; }

        public string RateClass { get; set; }

        public string CustomerNameKey { get; set; }

        public string ServiceReferenceNumber { get; set; }

        public string ServiceAddress { get; set; }

        public string ServiceCity { get; set; }

        public string ServiceState { get; set; }

        public string ServiceZip { get; set; }

        public string BillingAddress { get; set; }

        public string BillingCity { get; set; }

        public string BillingState { get; set; }

        public string BillingZip { get; set; }

        public string InCityLimits { get; set; }

        public string BillingFirstName { get; set; }

        public string BillingLastName { get; set; }

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
        ~RecordDetail()
        {
            // Simply call Dispose(false).
            Dispose(false);
        }
    }
}