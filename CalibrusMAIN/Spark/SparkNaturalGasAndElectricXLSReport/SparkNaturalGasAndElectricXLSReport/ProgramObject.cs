﻿using System;

namespace SparkNaturalGasAndElectricXLSReport
{
    internal class ProgramObject : IDisposable
    {
        public string State;

        public ProgramObject(string state)
        {
            State = state;
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
        ~ProgramObject()
        {
            // Simply call Dispose(false).
            Dispose(false);
        }
    }
}