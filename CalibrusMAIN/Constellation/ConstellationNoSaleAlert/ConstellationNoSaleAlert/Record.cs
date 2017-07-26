using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConstellationNoSaleAlert
{
    class Record : IDisposable
    {

        public int? AlertId { get; set; }
        public int? MainId { get; set; }
        public string Script { get; set; }
        public DateTime? CallDateTime { get; set; }
        public string VendorId { get; set; }
        public string VendorAgentId { get; set; }
        public int? ResponseId { get; set; }
        public string ConcernCode { get; set; }
        public string Concern { get; set; }
        public string MailToDistro { get; set; }

        public Record(int? alertId, int? mainId, string script, DateTime? callDateTime, string vendorId, string vendorAgentId, int? responseId, string concernCode, string concern, string mailToDistro)
        {
            AlertId = alertId;
            MainId = mainId;
            Script = script;
            CallDateTime = callDateTime;
            VendorId = vendorId;
            VendorAgentId = vendorAgentId;
            ResponseId = responseId;
            ConcernCode = concernCode;
            Concern = concern;
            MailToDistro = mailToDistro;
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
