using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Script2_Liberty.Data.Models
{
    public class MarketState
    {
        [Key]
        public int MarketStateId { get; set; } // MarketStateId (Primary key)
        public string State { get; set; } // State (length: 2)
        public bool? Active { get; set; } // Active
    }
}