using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QA_Report_Service
{
    public class QAFields
    {
        public int Id { get; set; }
        public string ServerName { get; set; }
        public string DatabaseSchemaTable { get; set; }
        public string ColumnIdName { get; set; }
        public string Description { get; set; }
        public bool Status { get; set; }
        public string DatabaseName { get; set; }
        public string ClientName { get; set; }
        public string Agent { get; set; }
        public string Supervisor { get; set; }
        public string Location { get; set; }
        public string Number { get; set; }
        public string Disposition { get; set; }
        public string CallDate { get; set; }
        public string CallLength { get; set; }
        public string InboundCall { get; set; }
        public string OutboundCall { get; set; }
        

    }
}
