using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SparkDataDumpCSV
{
    public class Record
    {
        public DateTime? CallDateTime { get; set; }
        public string VendorName { get; set; }
        public string VendorNumber { get; set; }
        public string LdcCode { get; set; }
        public string UtilityTypeName { get; set; }
        public string ProgramName { get; set; }
        public string Verified { get; set; }
        public string AccountNumber { get; set; }
        public string PremiseTypeName { get; set; }
        public string AuthorizationFirstName { get; set; }
        public string AuthorizationLastName { get; set; }
        public string ServiceAddress { get; set; }
        public string ServiceCity { get; set; }
        public string ServiceState { get; set; }
        public string ServiceZip { get; set; }
        public string ServiceCounty { get; set; }
        public string Email { get; set; }
        public string Btn { get; set; }
        public string AccountFirstName { get; set; }
        public string AccountLastName { get; set; }
        public string BillingAddress { get; set; }
        public string BillingCity { get; set; }
        public string BillingState { get; set; }
        public string BillingZip { get; set; }
        public string BillingCounty { get; set; }
        public string Language { get; set; }
        public string ProgramCode { get; set; }
        public decimal? Rate { get; set; }
        public int? Term { get; set; }
        public decimal? Msf { get; set; }
        public decimal? Etf { get; set; }
        public string AgentId { get; set; }
        public string SalesChannelName { get; set; }
        public string TpvAgentId { get; set; }
        public string TpvAgentName { get; set; }
        public string RateClass { get; set; }
        public string MainId { get; set; }
        public string Concern { get; set; }
        public string OfficeName { get; set; }
        public string TotalCallTime { get; set; }
        public string ExternalSalesId { get; set; }
        public string Brand { get; set; }
        public string ProductName { get; set; }
        public string MarketerCode { get; set; }
        public string Source { get; set; }
    }
}
