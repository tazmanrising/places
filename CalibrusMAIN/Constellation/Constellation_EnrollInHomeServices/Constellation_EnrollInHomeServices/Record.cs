using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Constellation_EnrollInHomeServices
{
    public class Record
    {
        //public string VendorName { get; set; }
        public Address BillingAddress { get; set; }
        public bool OnBillConsent { get; set; }
        public string CellPhoneNumber { get; set; }
        public string Channel { get; set; }
        public string Comment { get; set; }
        public string Commodity { get; set; }
        public string ConfirmationNumber { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string HeatingAndCoolingAge { get; set; }
        public string HeatingAndCoolingEquipment { get; set; }
        public string HeatingAndCoolingMake { get; set; }
        public string HeatingAndCoolingAge2 { get; set; }
        public string HeatingAndCoolingEquipment2 { get; set; }
        public string HeatingAndCoolingMake2 { get; set; }
        public string HeatingAndCoolingAge3 { get; set; }
        public string HeatingAndCoolingEquipment3 { get; set; }
        public string HeatingAndCoolingMake3 { get; set; }
        public string UDC { get; set; }
        public string PhoneNumber { get; set; }
        public string PlanId { get; set; }
        public string PromoCode { get; set; }
        public string SalesAgentId { get; set; }
        public string ServiceContractPromotion { get; set; }
        public string UtilityAccountNumber { get; set; }

        public Address ServiceAddress { get; set; }

        public class Address
        {
            public string AddressLine { get; set; }
            public string City { get; set; }
            public string State { get; set; }
            public string Zip { get; set; }

        }

    }
}
