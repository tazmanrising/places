using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SparkMajorEnergyEnrollmentDaily
{
    class Record
    {
        public int? OrderDetailId { get; set; }
        public int? MainId { get; set; }
        public string AuthorizationFirstName { get; set; }
        public string AuthorizationLastName { get; set; }
        public string CompanyName { get; set; }
        public string ServiceAddress { get; set; }
        public string ServiceCity { get; set; }
        public string ServiceState { get; set; }
        public string ServiceZip { get; set; }
        public string Btn { get; set; }
        public string Email { get; set; }
        public string PreferredLanguage { get; set; }
        public string VendorName { get; set; }
        public string AgentId { get; set; }
        public string UtilityTypeName { get; set; }
        public string CompanyContactFirstName { get; set; }
        public string CompanyContactLastName { get; set; }
        public string Relation { get; set; }
        public string PremiseTypeName { get; set; }
        public string LdcCode { get; set; }
        public string AccountNumber { get; set; }
        public string ProgramCode { get; set; }
        public DateTime? CallDateTime { get; set; }
        public string Concern { get; set; }
        public int? Term { get; set; }
    }
}
