using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FrontierE911BrightPatternFileInsert
{
    class LoadFileCallDetailRecord
    {
        public int? E911BrightPatternLoadFileCallDetailId { get; set; }
        public DateTime? CallDetailDateTime { get; set; }
        public string Type { get; set; }
        public int? IVR { get; set; }
        public int? QueueTime { get; set; }
        public int? DialingRinging { get; set; }
        public int? Talk { get; set; }
        public int? Hold { get; set; }
        public int? WrapUpTime { get; set; }
        public int? Duration { get; set; }
        public string FromLocation { get; set; }
        public string OriginalDestination { get; set; }
        public string ConnectedTo { get; set; }
        public string ConnectedToNumber { get; set; }
        public string ServiceCampaign { get; set; }
        public string AgentDisposition { get; set; }
        public string Notes { get; set; }
        public string Disposition { get; set; }
        public string MediaType { get; set; }
        public string InSL { get; set; }
        public string GloablID { get; set; }
        public string InteractionStepID { get; set; }
        public string WavName { get; set; }
    }
}
