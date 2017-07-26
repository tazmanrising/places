using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Script2_Liberty.Data.Models
{
    public class Office
    {
        [Key]
        public int OfficeId { get; set; } // OfficeId (Primary key)
        public int VendorId { get; set; } // VendorId
        public string OfficeName { get; set; } // OfficeName (length: 50)
        public string OfficeEmail { get; set; } // OfficeEmail (length: 50)
        public bool IsActive { get; set; } // IsActive
        public System.DateTime CreatedDateTime { get; set; } // CreatedDateTime
        public string CreatedBy { get; set; } // CreatedBy (length: 50)
        public System.DateTime? ModifiedDateTime { get; set; } // ModifiedDateTime
        public string ModifiedBy { get; set; } // ModifiedBy (length: 50)
    }
}
