using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConstellationDTDNoSaleXLSReport
{
    class Dispositions : IDisposable
    {
        public string Concern { get; set; }
        public int Count { get; set; }
        public Dispositions(string concern, int count)
        {
            Concern = concern;
            Count = count;
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
        ~Dispositions()
        {
            // Simply call Dispose(false).
            Dispose(false);
        }
    }
}
