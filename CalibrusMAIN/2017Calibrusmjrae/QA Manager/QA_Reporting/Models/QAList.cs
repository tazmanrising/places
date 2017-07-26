using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QA_Reporting.Models
{
    public class QAList
    {
        [JsonIgnore]
        public int QAListId { get; set; }
        public string ClientName { get; set; }
        public string Agent { get; set; }
        public string Supervisor { get; set; }
        public string Location { get; set; }
        public string Number { get; set; }
        public string Disposition { get; set; }
        public DateTime? Calldate { get; set; }
        public int CallLength { get; set; }
        public string InboundCall { get; set; }
        public string OutboundCall { get; set; }
        [JsonIgnore]
        public DateTime? CreatedDateTime { get; set; }
        [JsonIgnore]
        public bool? Status { get; set; }
        [JsonIgnore]
        public int IdentityColumnId { get; set; }
        [JsonIgnore]
        public string Comment { get; set; }
}
}