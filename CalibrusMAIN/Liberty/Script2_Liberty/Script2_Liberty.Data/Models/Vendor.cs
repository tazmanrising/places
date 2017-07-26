using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Script2_Liberty.Data.Models
{
    public class Vendor
    {
        [Key]
        public int VendorId { get; set; } // VendorId (Primary key)
        public string VendorNumber { get; set; } // VendorNumber (length: 10)
        public string VendorName { get; set; } // VendorName (length: 100)
        public string MarketerCode { get; set; } // MarketerCode (length: 50)
        public int SalesChannelId { get; set; } // SalesChannelId
        public bool IsActive { get; set; } // IsActive
        public System.DateTime CreatedDateTime { get; set; } // CreatedDateTime
        public string CreatedBy { get; set; } // CreatedBy (length: 50)
        public System.DateTime? ModifiedDateTime { get; set; } // ModifiedDateTime
        public string ModifiedBy { get; set; } // ModifiedBy (length: 50)
    }
}
