﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SparkOasisBatchXLSReport
{
   
    internal class SalesChannelObject : IDisposable
    {
        public int SalesChannelId;
        public string SalesChannelName;
      

        public SalesChannelObject(int salesChannelId, string salesChannelName)
        {
            SalesChannelId = salesChannelId;
            SalesChannelName = salesChannelName;                     
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
        ~SalesChannelObject()
        {
            // Simply call Dispose(false).
            Dispose(false);
        }
    }
}
