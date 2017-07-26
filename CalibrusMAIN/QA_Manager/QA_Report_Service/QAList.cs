using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QA_Report_Service
{
    public class QAList
    {
        [Key]
        public int QAListId { get; set; }

        public string ClientName { get; set; }
        public string Agent { get; set; }
        public string Supervisor { get; set; }

        public string Location { get; set; }
        public string Number { get; set; }
        public string Disposition { get; set; }
        public DateTime CallDate { get; set; }
        public int CallLength { get; set; }
        public string InboundCall { get; set; }
        public string OutboundCall { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public bool Status { get; set; }
        public int IdentityColumnId { get; set; }
        public string Comment { get; set; }


    }
}
