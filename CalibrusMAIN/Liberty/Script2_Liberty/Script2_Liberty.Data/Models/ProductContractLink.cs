using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Script2_Liberty.Data.Models
{
   public class ProductContractLink
    {
        [Key]
        public int ProductContractLinkId { get; set; } // ProductContractLinkId (Primary key)
        public int? MarketProductId { get; set; } // MarketProductId
        public int? ContractTermId { get; set; } // ContractTermId
    }
}
