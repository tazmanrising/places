using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for MainFormRecord
/// </summary>
public class MainFormRecord
{
    public int UserId { get; set; }
    public int SalesChannelId { get; set; }
    public string SalesAgentId { get; set; }
    public string Btn { get; set; }
    public int MarketStateId { get; set; }
    public int? MarketUtilityId { get; set; }
    public int? MarketProductId { get; set; }
    public int? DeliveryZoneId { get; set; }
    public string AuthorizationFirstName { get; set; }
    public string AuthorizationLastName { get; set; }
    public string Email { get; set; }
    public string BusinessTaxId { get; set; }
    public string BusinessName { get; set; }
    public string EnergyGRT { get; set; }
    public string SOHOAccount { get; set; }
    public string NumberOfAccounts { get; set; }
    public string SubTermMonth1Start { get; set; }
    public string SubTermMonth1End { get; set; }
    public string SubTermMonth2Start { get; set; }
    public string SubTermMonth2End { get; set; }
    public string SubTermMonth3Start { get; set; }
    public string SubTermMonth3End { get; set; }
    public string SubTermMonth4Start { get; set; }
    public string SubTermMonth4End { get; set; }
    public int? ContractTermId { get; set; }
    public string Rate { get; set; }
    public string ContractID { get; set; }
    public string EFLVersionCode { get; set; }
    public string SwitchOrMoveIn { get; set; }
    public string MaterialPreference { get; set; }
    public string EstDateExpiration { get; set; }
    public string RateEffectiveDate { get; set; }
    public string GasRate { get; set; }
    public int? GasContractTermId { get; set; }
    public string GasRateEffectiveDate { get; set; }
    public int? GasMarketUtilityId { get; set; }

    public List<OrderDetailFormRecord> OrderDetailFormRecords { get; set; }

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
    ~MainFormRecord()
    {
        // Simply call Dispose(false).
        Dispose(false);
    }
}