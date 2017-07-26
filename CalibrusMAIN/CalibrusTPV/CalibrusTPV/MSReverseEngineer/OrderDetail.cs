namespace CalibrusTPV.MSReverseEngineer
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("OrderDetail")]
    public partial class OrderDetail
    {
        [Key]
        [Column(Order = 0)]
        public int OrderDetailId { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int TPVId { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(50)]
        public string UtilityType { get; set; }

        [Key]
        [Column(Order = 3)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ProgramId { get; set; }

        [StringLength(50)]
        public string AccountType { get; set; }

        [StringLength(100)]
        public string AccountNumber { get; set; }

        [StringLength(100)]
        public string MeterNumber { get; set; }

        [StringLength(50)]
        public string RateClass { get; set; }

        [StringLength(50)]
        public string CustomerNameKey { get; set; }

        [StringLength(50)]
        public string ServiceReferenceNumber { get; set; }

        [StringLength(100)]
        public string ServiceAddress { get; set; }

        [StringLength(100)]
        public string ServiceCity { get; set; }

        [StringLength(2)]
        public string ServiceState { get; set; }

        [StringLength(50)]
        public string ServiceZip { get; set; }

        [StringLength(50)]
        public string ServiceCounty { get; set; }

        [StringLength(100)]
        public string BillingAddress { get; set; }

        [StringLength(100)]
        public string BillingCity { get; set; }

        [StringLength(2)]
        public string BillingState { get; set; }

        [StringLength(50)]
        public string BillingZip { get; set; }

        [StringLength(50)]
        public string BillingCounty { get; set; }

        [StringLength(50)]
        public string InCityLimits { get; set; }

        [StringLength(50)]
        public string BillingFirstName { get; set; }

        [StringLength(50)]
        public string BillingLastName { get; set; }

        public virtual TPV TPV { get; set; }
    }
}
