using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for OrderDetailFormRecord
/// </summary>
public class OrderDetailFormRecord
{
    public int OrderDetailFormRecordNumber { get; set; }
    public string Btn { get; set; }
    public string AccountNumber { get; set; }
    public string GasAccountNumber { get; set; }
    public string MeterNumber { get; set; }
    public string NameKey { get; set; }
    public string ServiceNumber { get; set; }
    public string SubTermRate1 { get; set; }
    public string SubTermRate2 { get; set; }
    public string SubTermRate3 { get; set; }
    public string SubTermRate4 { get; set; }
    public string ServiceAddress1 { get; set; }
    public string ServiceAddress2 { get; set; }
    public string ServiceCity { get; set; }
    public string ServiceState { get; set; }
    public string ServiceZip { get; set; }
    public string BillingAddress1 { get; set; }
    public string BillingAddress2 { get; set; }
    public string BillingCity { get; set; }
    public string BillingState { get; set; }
    public string BillingZip { get; set; }

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
    ~OrderDetailFormRecord()
    {
        // Simply call Dispose(false).
        Dispose(false);
    }
}