using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClearviewCurrentCustomerImport
{
    public class CustomerRecord
    {
        public int Id { get; set; }
        public DateTime? InsertDateTime { get; set; }
        public string AccountNumber { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string Phone { get; set; }        
        public string Commodity { get; set; }
        public string Utility { get; set; }
        
        
    }
}
