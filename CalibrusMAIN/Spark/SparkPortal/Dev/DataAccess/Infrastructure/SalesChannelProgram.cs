//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Calibrus.SparkPortal.DataAccess.Infrastructure
{
    using System;
    using System.Collections.Generic;
    
    public partial class SalesChannelProgram
    {
        public int Id { get; set; }
        public Nullable<int> SalesChannelId { get; set; }
        public Nullable<int> ProgramId { get; set; }
    
        public virtual Program Program { get; set; }
    }
}
