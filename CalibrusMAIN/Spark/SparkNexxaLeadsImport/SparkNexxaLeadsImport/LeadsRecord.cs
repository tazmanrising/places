using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Spatial;
using Microsoft.SqlServer;

namespace SparkNexxaLeadsImport
{
    //NOTE: THIS RECORD HAS TO MATCH THE DATABASE IN ORDER AND CASE SENSITIVE AND SPELLING
    class LeadsRecord
    {
        public int? LeadsId { get; set; }       
        public string RecordLocator { get; set; }
        public string VendorNumber { get; set; }//VENDOR_CODE
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string Phone { get; set; }
        public string Utility { get; set; }//LDC_CODE
        public string CompanyName { get; set; }
        public DateTime? LoadDateTime { get; set; }
        public string MiddleName { get; set; }
        public string County { get; set; }  
        public string Zip4 { get; set; }
        public string ZipPlus4 { get; set; }
        public string ConnectDate { get; set; }
        public string UtilityZone { get; set; }
        public string HispanicFlag { get; set; }
        public string HispanicLangPref { get; set; }
        public string HomeSqFt { get; set; }
        public string DwellType { get; set; }
        public string ContactTitle { get; set; }
        public string HomeYrBuilt { get; set; }
        public string BuildingSqFt { get; set; }
        public string HispanicAculturation { get; set; }
        public string UsageThreshold { get; set; }
        public string IndividualCreditScore { get; set; }
        public string SicCode { get; set; }
        public string EmployeeSize { get; set; }
        public string CreditRating { get; set; }
        public string YrStartDate { get; set; }
        public string SicDesc { get; set; }
        public string CampaignCode { get; set; }
        public string RecordType { get; set; }
        public string Vendor { get; set; }
        public string ProcessDate { get; set; }
        public string ESIID { get; set; }
        public string CarrierRoute	{ get; set; }
        public string SequenceNumber	{ get; set; }
        public string Lat	{ get; set; }
        public string Long { get; set; }
        public Microsoft.SqlServer.Types.SqlGeometry Geolocation { get; set; }

    }
}
