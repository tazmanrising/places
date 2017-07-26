using System;

namespace SparkProviderBatchXLSReport
{
    internal class UtilityTypeObject : IDisposable
    {
        public int UtilityTypeId;
        public string UtilityTypeName;

        public UtilityTypeObject(int id, string name)
        {
            UtilityTypeId = id;
            UtilityTypeName = name;
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
        ~UtilityTypeObject()
        {
            // Simply call Dispose(false).
            Dispose(false);
        }
    }
}