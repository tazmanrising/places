using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Script2_Liberty.Data.Models
{
    public class OrderDetail
    {
        [Key]
        public int OrderDetailId { get; set; } // OrderDetailId (Primary key)
        public int? MainId { get; set; } // MainId
        public string UtilityType { get; set; } // UtilityType (length: 50)
        public int? ProgramId { get; set; } // ProgramId
        public string AccountType { get; set; } // AccountType (length: 50)
        public string AccountNumber { get; set; } // AccountNumber (length: 100)
        public string MeterNumber { get; set; } // MeterNumber (length: 100)
        public string RateClass { get; set; } // RateClass (length: 50)
        public string CustomerNameKey { get; set; } // CustomerNameKey (length: 50)
        public string ServiceReferenceNumber { get; set; } // ServiceReferenceNumber (length: 50)
        public string ServiceAddress { get; set; } // ServiceAddress (length: 100)
        public string ServiceAddress2 { get; set; } // ServiceAddress2 (length: 100)
        public string ServiceCity { get; set; } // ServiceCity (length: 100)
        public string ServiceState { get; set; } // ServiceState (length: 2)
        public string ServiceZip { get; set; } // ServiceZip (length: 50)
        public string ServiceCounty { get; set; } // ServiceCounty (length: 50)
        public string BillingAddress { get; set; } // BillingAddress (length: 100)
        public string BillingAddress2 { get; set; } // BillingAddress2 (length: 100)
        public string BillingCity { get; set; } // BillingCity (length: 100)
        public string BillingState { get; set; } // BillingState (length: 2)
        public string BillingZip { get; set; } // BillingZip (length: 50)
        public string BillingCounty { get; set; } // BillingCounty (length: 50)
        public string InCityLimits { get; set; } // InCityLimits (length: 50)
        public string SubTermRate1 { get; set; } // SubTermRate1 (length: 10)
        public string SubTermRate2 { get; set; } // SubTermRate2 (length: 10)
        public string SubTermRate3 { get; set; } // SubTermRate3 (length: 10)
        public string SubTermRate4 { get; set; } // SubTermRate4 (length: 10)
        public string NameKey { get; set; } // NameKey (length: 50)
        public string ServiceNumber { get; set; } // ServiceNumber (length: 50)
        public string GasAccountNumber { get; set; } // GasAccountNumber (length: 100)
    }
}
