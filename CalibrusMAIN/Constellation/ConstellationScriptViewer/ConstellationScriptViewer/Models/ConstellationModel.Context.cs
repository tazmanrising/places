﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ConstellationScriptViewer.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    
    public partial class ConstellationEntities : DbContext
    {
        public ConstellationEntities()
            : base("name=ConstellationEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<tblScriptLookup> tblScriptLookups { get; set; }
        public virtual DbSet<tblScriptLog> tblScriptLogs { get; set; }
    
        public virtual ObjectResult<spReturnScript_Result> spReturnScript(string param1)
        {
            var param1Parameter = param1 != null ?
                new ObjectParameter("param1", param1) :
                new ObjectParameter("param1", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<spReturnScript_Result>("spReturnScript", param1Parameter);
        }
    }
}
