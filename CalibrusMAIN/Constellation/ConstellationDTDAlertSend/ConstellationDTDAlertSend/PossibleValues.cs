using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConstellationDTDAlertSend
{
    class PossibleValues
    {
        public DateTime? CallDateTime { get; set; }
        public int? ResponseId { get; set; }
        public string Concern { get; set; }
        public int? MainId { get; set; }
        public string VendorNumber { get; set; }
        public string AgentId { get; set; }
        public string ServicePhoneNumber { get; set; }
        public string VendorId { get; set; }
        public string CallBackNumber { get; set; }
        public string TlpAgent { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string BillingAddress1 { get; set; }
        public string BillingAddress2 { get; set; }
        public string BillingCity { get; set; }
        public string BillingState { get; set; }
        public string BillingZip { get; set; }
        public string Status { get; set; }
        public DateTime? PreviousCallDateTime { get; set; }
        public string BTNUsed { get; set; }
        public string SalesAgentName { get; set; }        
        public string PreviousCustomerName { get; set; }
        public string PreviousAddress { get; set; }
        public int? PreviousVerificationCode { get; set; }
        public string PreviousAgentId { get; set; }
        public string PreviousUDCAccountNumber { get; set; }
        public int? SaleId { get; set; }
        public string Market { get; set; }
    }
}
