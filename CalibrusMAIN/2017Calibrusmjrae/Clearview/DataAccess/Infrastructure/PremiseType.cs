//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Calibrus.ClearviewPortal.DataAccess.Infrastructure
{
    using System;
    using System.Collections.Generic;
    
    public partial class PremiseType
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public PremiseType()
        {
            this.Programs = new HashSet<Program>();
        }
    
        public int PremiseTypeId { get; set; }
        public string PremiseTypeName { get; set; }
        public bool IsActive { get; set; }
        public int DisplayOrder { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Program> Programs { get; set; }
    }
}
