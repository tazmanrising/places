using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Script2_Liberty.Data.Models
{
    public class ContractTerm
    {
        [Key]
        public int ContractTermId { get; set; } // ContractTermId (Primary key)
        public string MonthlyTerm { get; set; } // MonthlyTerm (length: 50)
    }
}
