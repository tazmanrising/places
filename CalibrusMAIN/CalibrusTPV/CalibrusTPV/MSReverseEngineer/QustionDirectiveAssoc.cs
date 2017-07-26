namespace CalibrusTPV.MSReverseEngineer
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("QustionDirectiveAssoc")]
    public partial class QustionDirectiveAssoc
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        public int QuestionId { get; set; }

        public int DirectiveId { get; set; }

        public int SortOrder { get; set; }

        public virtual Directive Directive { get; set; }

        public virtual Question Question { get; set; }
    }
}
