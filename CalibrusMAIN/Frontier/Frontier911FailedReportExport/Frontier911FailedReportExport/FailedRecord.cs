using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Frontier911FailedReportExport
{
    public class FailedRecord
    {
        public string SubscriberId { get; set; }
        public string Name { get; set; }
        public string Signature { get; set; }
        public string BirthYear { get; set; }
        public string TN { get; set; }
        public string Email { get; set; }
        public string GeneralAction { get; set; }
        public DateTime? GeneralDate { get; set; }
        public string E911Action { get; set; }
        public DateTime? E911Date { get; set; }
        public string IsData { get; set; }
        public string IsVoip { get; set; }
        public string User { get; set; }
        public int? Attempts { get; set; }
        public DateTime? LastAttemptDate { get; set; }
        public string CustomerAccepted { get; set; }
        public string CustomerToBeDisconnected { get; set; }
        public string LastDispositionCode { get; set; }
    }
}