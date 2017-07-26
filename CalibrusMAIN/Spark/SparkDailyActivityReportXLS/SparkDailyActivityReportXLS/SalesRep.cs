using System;
using System.Collections.Generic;

namespace SparkDailyActivityReportXLS
{
    internal class SalesRep : IDisposable
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public int TotalConcernCount { get; set; }

        public List<Disposition> Dispositions = new List<Disposition>();

        //Constructor
        public SalesRep(string firstName, string lastName, int totalConcernCount, List<Disposition> dispositions)
        {
            FirstName = firstName;
            LastName = lastName;
            TotalConcernCount = totalConcernCount;
            Dispositions = dispositions;
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
        ~SalesRep()
        {
            // Simply call Dispose(false).
            Dispose(false);
        }
    }
}