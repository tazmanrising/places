using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalibrusTPV.Data.Models
{
    public class Question
    {
        public int Id { get; set; } // Id (Primary key)
        public string Name { get; set; } // Name (length: 50)
        public string Description { get; set; } // Description
        public string Verbiage { get; set; } // Verbiage
        public string VerbiageSpanish { get; set; } // VerbiageSpanish
        public bool? Active { get; set; } // Active

    }
}
