// <auto-generated>
// ReSharper disable ConvertPropertyToExpressionBody
// ReSharper disable DoNotCallOverridableMethodsInConstructor
// ReSharper disable InconsistentNaming
// ReSharper disable PartialMethodWithSinglePart
// ReSharper disable PartialTypeWithSinglePart
// ReSharper disable RedundantNameQualifier
// ReSharper disable RedundantOverridenMember
// ReSharper disable UseNameofExpression
// TargetFrameworkVersion = 4.5
#pragma warning disable 1591    //  Ignore "Missing XML Comment" warning


namespace CalibrusTPV.ReversePocoGen
{

    // LookUps
    [System.CodeDom.Compiler.GeneratedCode("EF.Reverse.POCO.Generator", "2.28.0.0")]
    public class LookUpConfiguration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<LookUp>
    {
        public LookUpConfiguration()
            : this("dbo")
        {
        }

        public LookUpConfiguration(string schema)
        {
            ToTable("LookUps", schema);
            HasKey(x => x.Id);

            Property(x => x.Id).HasColumnName(@"id").HasColumnType("int").IsRequired().HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity);
            Property(x => x.LookupType).HasColumnName(@"LookupType").HasColumnType("int").IsRequired();
            Property(x => x.LookupId).HasColumnName(@"LookupId").HasColumnType("int").IsRequired();
            Property(x => x.Name).HasColumnName(@"Name").HasColumnType("nvarchar").IsRequired().HasMaxLength(50);
        }
    }

}
// </auto-generated>
