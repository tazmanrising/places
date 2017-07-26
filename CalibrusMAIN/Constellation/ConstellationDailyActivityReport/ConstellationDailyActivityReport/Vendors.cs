using System;

namespace ConstellationDailyActivityReport
{
    internal class Vendors : IDisposable
    {
        public int VendorId;
        public string VendorName;

        public Vendors(int id, string name)
        {
            VendorId = id;
            VendorName = name;
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
        ~Vendors()
        {
            // Simply call Dispose(false).
            Dispose(false);
        }
    }
}