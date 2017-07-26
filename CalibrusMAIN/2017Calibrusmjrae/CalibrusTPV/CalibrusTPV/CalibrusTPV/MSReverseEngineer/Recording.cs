namespace CalibrusTPV.MSReverseEngineer
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Recording
    {
        public int Id { get; set; }

        public int TPVId { get; set; }

        public int CallType { get; set; }

        public DateTime Created { get; set; }

        public int CallTime { get; set; }

        [StringLength(50)]
        public string wavename { get; set; }

        public int? tpvAgentId { get; set; }

        public virtual TPV TPV { get; set; }
    }
}
