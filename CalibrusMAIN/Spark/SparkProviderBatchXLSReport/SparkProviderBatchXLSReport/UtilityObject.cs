using System;

namespace SparkProviderBatchXLSReport
{
    internal class UtilityObject : IDisposable
    {
        public int UtilityId;
        public string LdcCode;
        public string Name;

        public UtilityObject(int id, string ldccode, string name)
        {
            UtilityId = id;
            LdcCode = ldccode;
            Name = name;
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
        ~UtilityObject()
        {
            // Simply call Dispose(false).
            Dispose(false);
        }
    }
}
