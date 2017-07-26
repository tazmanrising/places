using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FrontierE911BrightPatternFileInsert
{
    class LoadFileRecord
    {
        public int? E911BrightPatternLoadFileId { get; set; }
        public int? E911LoadFileId { get; set; }
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
        public string State { get; set; }
        public string DPIRegion { get; set; }
        public string ThisPhonenumber { get; set; }
        public string IsCallAttempt { get; set; }
        public string Completed { get; set; }
        public string RecordDisposition { get; set; }
        public string RecordDispositionCode { get; set; }
        public string Outofquota { get; set; }
        public string Quotagroup { get; set; }
        public string CallDisposition { get; set; }
        public string CallDispositionCode { get; set; }
        public string CallNote { get; set; }
        public DateTime? CallTime { get; set; }
        public string DialingDuration { get; set; }
        public string CPADuration { get; set; }
        public string AnsweredDuration { get; set; }
        public string Agent { get; set; }
        public string Connected { get; set; }
        public string CPAresult { get; set; }
        public string CPArecordingfile { get; set; }
        public string CPARTPserverid { get; set; }
        public string Recordingfile { get; set; }
        public string RTPserverid { get; set; }
        public string GlobalInteractionID { get; set; }
        public string RecordID { get; set; }
        public string Listname { get; set; }
    }
}
