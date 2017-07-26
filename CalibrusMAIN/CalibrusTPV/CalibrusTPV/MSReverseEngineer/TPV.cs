namespace CalibrusTPV.MSReverseEngineer
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("TPV")]
    public partial class TPV
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TPV()
        {
            Recordings = new HashSet<Recording>();
            OrderDetails = new HashSet<OrderDetail>();
        }

        public int Id { get; set; }

        public int tpvAgentId { get; set; }

        public int UserId { get; set; }

        [Required]
        [StringLength(10)]
        public string Dnis { get; set; }

        [Required]
        [StringLength(1)]
        public string Verified { get; set; }

        [StringLength(10)]
        public string Btn { get; set; }

        [StringLength(50)]
        public string ConcernCode { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Recording> Recordings { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
