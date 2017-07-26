using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FrontierE911LaodFileInsert
{
    class LoadFileRecord
    {
        public string SubscriberID { get; set; }
        public string Name { get; set; }
        public string Signature { get; set; }
        public string BirthYear { get; set; }// no longer supplied in csv file but we need it for placement when doing a bulk insert will be empty
        public string TN { get; set; }
        public string Email { get; set; }
        public string GeneralAction { get; set; }
        public DateTime? GeneralDate { get; set; }
        public string E911Action { get; set; }
        public DateTime? E911Date { get; set; }
        public string isData { get; set; }
        public string isVoip { get; set; }
        public string User { get; set; }
        public string State { get; set; }
        public string DPIRegion { get; set; }
    }
}
