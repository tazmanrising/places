//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Constellation_EnrollInHomeServices
{
    using System;
    using System.Collections.Generic;
    
    public partial class tblHomeService
    {
        public int HomeServicesId { get; set; }
        public Nullable<int> VendorId { get; set; }
        public string VendorAgentId { get; set; }
        public Nullable<int> ResponseId { get; set; }
        public Nullable<System.DateTime> CreatedDateTime { get; set; }
        public string ServiceFirstName { get; set; }
        public string ServiceLastName { get; set; }
        public string ServiceAddress1 { get; set; }
        public string ServiceAddress2 { get; set; }
        public string ServiceCity { get; set; }
        public string ServiceCountry { get; set; }
        public string ServiceCounty { get; set; }
        public string ServiceState { get; set; }
        public string ServiceZipCode { get; set; }
        public string ServicePhoneNumber { get; set; }
        public string ServiceEmail { get; set; }
        public string BillingFirstName { get; set; }
        public string BillingLastName { get; set; }
        public string BillingAddress1 { get; set; }
        public string BillingAddress2 { get; set; }
        public string BillingCity { get; set; }
        public string BillingCountry { get; set; }
        public string BillingCounty { get; set; }
        public string BillingState { get; set; }
        public string BillingZipCode { get; set; }
        public string BillingEmail { get; set; }
        public string Language { get; set; }
        public Nullable<int> HomeServicesPlanId { get; set; }
        public Nullable<int> AddOns { get; set; }
        public Nullable<bool> IncludeOnBGEBill { get; set; }
        public string ElectricChoiceId { get; set; }
        public Nullable<int> MainId { get; set; }
    }
}
