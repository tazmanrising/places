namespace CalibrusTPV.MSReverseEngineer
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class ScriptQuestion
    {
        [Key]
        public int ScriptId { get; set; }

        public int QtypeId { get; set; }

        [Required]
        [StringLength(2)]
        public string StateCode { get; set; }

        public int SalesChannelId { get; set; }

        public bool Active { get; set; }

        public int QuestionId { get; set; }

        public int ScriptOrder { get; set; }

        public virtual Question Question { get; set; }

        public virtual State State { get; set; }
    }
}
