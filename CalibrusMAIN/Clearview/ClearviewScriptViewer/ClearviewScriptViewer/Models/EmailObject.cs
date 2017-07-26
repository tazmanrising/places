using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ClearviewScriptViewer.Models
{
    public class EmailObject
    {
        public string Client { get; set; }
        public string Script { get; set; }
        public string ScriptId { get; set; }
        public string ScriptOrder { get; set; }
        public string Active { get; set; }
        public string YesNo { get; set; }
        public string Verbiage { get; set; }
        public string VerbiageSpanish { get; set; }
        public string NoVerbiage { get; set; }
        public string NoVerbiageSpanish { get; set; }
        public string Condition { get; set; }
        public string NoConcernCode { get; set; }
        public string Notes { get; set; }
        public string CCDistro { get; set; }
    }    
}