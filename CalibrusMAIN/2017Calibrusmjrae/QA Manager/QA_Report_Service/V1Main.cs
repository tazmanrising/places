using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QA_Report_Service
{
    public class V1Main
    {

        /*
        Clearview
        
         
        */
        public int MainId { get; set; } // id     IdentityColumn
        public string ClientName { get; set; } // new field   ClientName 
        public string TpvAgentId { get; set; }  // Agent
        public int Btn { get; set; } // Number     ( phone number)    int should be fine unless ever  () or - are included
        public string Concern { get; set; } // Disposition
        public DateTime WebDateTime { get; set; } // CallDate
        public int TotalTime { get; set; } // Call Length
        public string WavName { get; set; } // Inbound Call 
        public string OutboundWavName { get; set; } // Inbound Call 
        public bool Active { get; set; } // Inbound Call 


    }
}
