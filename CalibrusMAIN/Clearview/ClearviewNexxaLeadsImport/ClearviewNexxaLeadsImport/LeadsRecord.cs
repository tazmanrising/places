using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClearviewNexxaLeadsImport
{
    //NOTE: THIS RECORD HAS TO MATCH THE DATABASE IN ORDER AND CASE SENSITIVE AND SPELLING
    class LeadsRecord
    {
        public int? LeadsId { get; set; }
        public string RecordLocator { get; set; }
        public string VendorNumber { get; set; }//VENDOR_CODE
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string Phone { get; set; }
        public string Utility { get; set; }//LDC_CODE
        public DateTime? LoadDateTime { get; set; }
        public string MiddleName { get; set; }
        public string County { get; set; }
        public string Zip4 { get; set; }       
        public string ConnectDate { get; set; }       
        public string DwellType { get; set; }       
        public string CampaignCode { get; set; }
        public string ProcessDate { get; set; }
    }
}
