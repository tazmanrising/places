using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Script2_Liberty.Data.Models
{
   public  class ZipCodeLookup
    {
        [Key]
        public int ZipCodeLookupId { get; set; } // ZipCodeLookupId (Primary key)
        public string ZipCode { get; set; } // ZipCode (length: 5)
        public string City { get; set; } // City (length: 50)
        public string State { get; set; } // State (length: 2)
        public string County { get; set; } // County (length: 50)
        public string Country { get; set; } // Country (length: 2)
    }
}
