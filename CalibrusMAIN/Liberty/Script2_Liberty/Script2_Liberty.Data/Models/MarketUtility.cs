using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Script2_Liberty.Data.Models
{
    public class MarketUtility
    {
        [Key]
        public int MarketUtilityId { get; set; } // MarketUtilityId (Primary key)
        public int? MarketStateId { get; set; } // MarketStateId
        public string Utility { get; set; } // Utility (length: 50)
        public string UtilityName { get; set; } // UtilityName (length: 100)
        public string AccountType { get; set; } // AccountType (length: 50)
        public int? AccountDigits { get; set; } // AccountDigits
        public string AccountMask { get; set; } // AccountMask (length: 50)
        public bool? NameKey { get; set; } // NameKey
        public int? NameKeyDigits { get; set; } // NameKeyDigits
        public string NameKeyMask { get; set; } // NameKeyMask (length: 50)
        public bool? ServiceReference { get; set; } // ServiceReference
        public int? ServiceReferenceDigits { get; set; } // ServiceReferenceDigits
        public string ServiceReferenceMask { get; set; } // ServiceReferenceMask (length: 50)
        public bool? MeterNumber { get; set; } // MeterNumber
        public int? MeterNumberDigits { get; set; } // MeterNumberDigits
        public string MeterNumberMask { get; set; } // MeterNumberMask (length: 50)
        public bool? Active { get; set; } // Active
        public bool? IsElectric { get; set; } // IsElectric
        public bool? IsGas { get; set; } // IsGas
    }
}
