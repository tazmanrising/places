using System;

namespace SparkProviderBatchXLSReport
{
    internal class PremiseTypeObject : IDisposable
    {
        public int PremiseTypeId;
        public string PremiseTypeName;

        public PremiseTypeObject(int id, string name)
        {
            PremiseTypeId = id;
            PremiseTypeName = name;
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
        ~PremiseTypeObject()
        {
            // Simply call Dispose(false).
            Dispose(false);
        }
    }
}