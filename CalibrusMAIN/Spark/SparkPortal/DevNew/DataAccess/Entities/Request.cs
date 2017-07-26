using Calibrus.SparkPortal.DataAccess.Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calibrus.SparkPortal.DataAccess.Entities
{
    public class Request
    {

        public RequestUser User { get; set; }
        public RequestIpLocation IpLocation { get; set; }
        [MaxLength(10)]
        public string Phone { get; set; }
        [MaxLength(50)]
        public string FirstName { get; set; }
        [MaxLength(50)]
        public string LastName { get; set; }
        public string RecordLocator { get; set; }
        [MaxLength(50)]
        public string UtilityType { get; set; }
        public List<RequestOrderDetail> OrderDetails { get; set; }
        public Lead Lead { get; set; }

        public class RequestOrderDetail
        {
            [MaxLength(50)]
            public string UtilityType { get; set; }
            [MaxLength(50)]
            public string Address { get; set; }
            [MaxLength(50)]
            public string Address2 { get; set; }
            [MaxLength(50)]
            public string City { get; set; }
            [MaxLength(2)]
            public string State { get; set; }
            [MaxLength(5)]
            public string Zip { get; set; }
            [MaxLength(50)]
            public string BillingFirstName { get; set; }
            [MaxLength(50)]
            public string BillingLastName { get; set; }
            [MaxLength(50)]
            public string BillingAddress { get; set; }
            [MaxLength(50)]
            public string BillingAddress2 { get; set; }
            [MaxLength(50)]
            public string BillingCity { get; set; }
            [MaxLength(2)]
            public string BillingState { get; set; }
            [MaxLength(5)]
            public string BillingZip { get; set; }
            [MaxLength(100)]
            public string AccountNumber { get; set; }
            [MaxLength(100)]
            public string MeterNumber { get; set; }
            [MaxLength(50)]
            public string ServiceReference { get; set; }
            [MaxLength(50)]
            public string Relationship { get; set; }
            public Program Program { get; set; }
        }


        public class RequestUser
        {
            public int UserId { get; set; }
            public string AgentId { get; set; }
            public string SparkId { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public int VendorId { get; set; }
            public int OfficeId { get; set; }
        }

        public class RequestIpLocation
        {
            [MaxLength(50)]
            public string Ip { get; set; }
            [MaxLength(50)]
            public string City { get; set; }
            [MaxLength(50)]
            public string Region { get; set; }
            [MaxLength(50)]
            public string Country { get; set; }
            [MaxLength(50)]
            public string HostName { get; set; }
            [MaxLength(50)]
            public string Loc { get; set; }
            [MaxLength(50)]
            public string Org { get; set; }
            [MaxLength(50)]
            public string Postal { get; set; }
        }


    }
}
