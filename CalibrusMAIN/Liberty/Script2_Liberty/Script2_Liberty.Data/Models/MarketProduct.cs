using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Script2_Liberty.Data.Models
{
    public class MarketProduct
    {
        [Key]
        public int MarketProductId { get; set; } // MarketProductId (Primary key)
        public int? MarketStateId { get; set; } // MarketStateId
        public string Product { get; set; } // Product (length: 100)
        public string ProductWebForm { get; set; } // ProductWebForm (length: 200)
        public bool? Commercial { get; set; } // Commercial
        public bool? Active { get; set; } // Active
        public bool? SubTermRate { get; set; } // SubTermRate
    }
}
