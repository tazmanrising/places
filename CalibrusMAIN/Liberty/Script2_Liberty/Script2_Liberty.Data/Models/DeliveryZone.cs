using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Script2_Liberty.Data.Models
{
    public class DeliveryZone
    {
        [Key]
        public int DeliveryZoneId { get; set; } // DeliveryZoneId (Primary key)
        public string Name { get; set; } // Name (length: 50)
        public bool IsActive { get; set; } // IsActive
    }
}
