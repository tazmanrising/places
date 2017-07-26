using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Script2_Liberty.Data.Models
{
    public class Main
    {
        [Key]
        public int MainId { get; set; } // MainId (Primary key)
        public System.DateTime? CallDateTime { get; set; } // CallDateTime
        public System.DateTime? WebDateTime { get; set; } // WebDateTime
        public string WavName { get; set; } // WavName (length: 50)
        public string Dnis { get; set; } // Dnis (length: 4)
        public string Verified { get; set; } // Verified (length: 1)
        public string Concern { get; set; } // Concern (length: 50)
        public string ConcernCode { get; set; } // ConcernCode (length: 50)
        public string TpvAgentName { get; set; } // TpvAgentName (length: 50)
        public string TpvAgentId { get; set; } // TpvAgentId (length: 50)
        public int? TotalTime { get; set; } // TotalTime
        public int? UserId { get; set; } // UserId
        public string Email { get; set; } // Email (length: 100)
        public string RecordLocator { get; set; } // RecordLocator (length: 50)
        public string SalesState { get; set; } // SalesState (length: 2)
        public string AuthorizationFirstName { get; set; } // AuthorizationFirstName (length: 50)
        public string AuthorizationMiddle { get; set; } // AuthorizationMiddle (length: 1)
        public string AuthorizationLastName { get; set; } // AuthorizationLastName (length: 50)
        public string Btn { get; set; } // Btn (length: 10)
        public string CompanyName { get; set; } // CompanyName (length: 100)
        public string CompanyContactFirstName { get; set; } // CompanyContactFirstName (length: 50)
        public string CompanyContactLastName { get; set; } // CompanyContactLastName (length: 50)
        public string CompanyContactTitle { get; set; } // CompanyContactTitle (length: 50)
        public string Territory { get; set; } // Territory (length: 50)
        public string LeadType { get; set; } // LeadType (length: 50)
        public string Relation { get; set; } // Relation (length: 50)
        public string NumberOfAccounts { get; set; } // NumberOfAccounts (length: 5)
        public string AccountFirstName { get; set; } // AccountFirstName (length: 50)
        public string AccountLastName { get; set; } // AccountLastName (length: 50)
        public string OutboundWavName { get; set; } // OutboundWavName (length: 50)
        public System.DateTime? QaDateChange { get; set; } // QaDateChange
        public string QaReasonChange { get; set; } // QaReasonChange (length: 100)
        public string QaAgentId { get; set; } // QaAgentId (length: 10)
        public string QaOriginalConcern { get; set; } // QaOriginalConcern (length: 50)
        public bool? QaChanged { get; set; } // QaChanged
        public string Rate { get; set; } // Rate (length: 10)
        public string RateEffectiveDate { get; set; } // RateEffectiveDate (length: 10)
        public string SubTermMonth1Start { get; set; } // SubTermMonth1Start (length: 2)
        public string SubTermMonth1End { get; set; } // SubTermMonth1End (length: 2)
        public string SubTermMonth2Start { get; set; } // SubTermMonth2Start (length: 2)
        public string SubTermMonth2End { get; set; } // SubTermMonth2End (length: 2)
        public string SubTermMonth3Start { get; set; } // SubTermMonth3Start (length: 2)
        public string SubTermMonth3End { get; set; } // SubTermMonth3End (length: 2)
        public string SubTermMonth4Start { get; set; } // SubTermMonth4Start (length: 2)
        public string SubTermMonth4End { get; set; } // SubTermMonth4End (length: 2)
        public int? SalesChannelId { get; set; } // SalesChannelId
        public int? MarketStateId { get; set; } // MarketStateId
        public int? MarketUtilityId { get; set; } // MarketUtilityId
        public int? MarketProductId { get; set; } // MarketProductId
        public string BusinessName { get; set; } // BusinessName (length: 50)
        public string BusinessTaxId { get; set; } // BusinessTaxId (length: 50)
        public string SohoAccount { get; set; } // SOHOAccount (length: 3)
        public int? ContractTermId { get; set; } // ContractTermId
        public string SalesAgentId { get; set; } // SalesAgentId (length: 50)
        public string EnergyGrt { get; set; } // EnergyGRT (length: 10)
        public string ContractId { get; set; } // ContractID (length: 50)
        public string EflVersionCode { get; set; } // EFLVersionCode (length: 50)
        public string SwitchOrMoveIn { get; set; } // SwitchOrMoveIn (length: 10)
        public string MaterialPreference { get; set; } // MaterialPreference (length: 10)
        public string EstDateExpiration { get; set; } // EstDateExpiration (length: 10)
        public int? DeliveryZoneId { get; set; } // DeliveryZoneId
        public string GasRate { get; set; } // GasRate (length: 10)
        public int? GasContractTermId { get; set; } // GasContractTermId
        public string GasRateEffectiveDate { get; set; } // GasRateEffectiveDate (length: 10)
        public int? GasMarketUtilityId { get; set; } // GasMarketUtilityId
    }
}
