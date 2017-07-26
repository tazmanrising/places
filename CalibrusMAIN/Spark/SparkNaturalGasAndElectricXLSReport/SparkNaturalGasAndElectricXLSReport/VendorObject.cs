using System;

namespace SparkNaturalGasAndElectricXLSReport
{
    internal class VendorObject : IDisposable
    {
        public int VendorId;
        public string VendorNumber;
        public string VendorName;
        public string MarketerCode;
        public int SalesChannelId;
        public string SalesChannelName;

        public VendorObject(int vendorid, string vendornumber, string vendorname, string marketercode, int saleschannelid, string saleschannelname)
        {
            VendorId = vendorid;
            VendorNumber = vendornumber;
            VendorName = vendorname;
            MarketerCode = marketercode;
            SalesChannelId = saleschannelid;
            SalesChannelName = saleschannelname;
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
        ~VendorObject()
        {
            // Simply call Dispose(false).
            Dispose(false);
        }
    }
}