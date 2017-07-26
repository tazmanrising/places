namespace CalibrusTPV.MSReverseEngineer
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class LookUp
    {
        public int id { get; set; }

        public int LookupType { get; set; }

        public int LookupId { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }
    }
}
