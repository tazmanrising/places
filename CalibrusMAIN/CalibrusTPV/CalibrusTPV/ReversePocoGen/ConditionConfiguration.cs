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

    // Conditions
    [System.CodeDom.Compiler.GeneratedCode("EF.Reverse.POCO.Generator", "2.28.0.0")]
    public class ConditionConfiguration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<Condition>
    {
        public ConditionConfiguration()
            : this("dbo")
        {
        }

        public ConditionConfiguration(string schema)
        {
            ToTable("Conditions", schema);
            HasKey(x => x.Id);

            Property(x => x.Id).HasColumnName(@"Id").HasColumnType("int").IsRequired().HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.None);
            Property(x => x.Name).HasColumnName(@"Name").HasColumnType("nvarchar").IsOptional().HasMaxLength(50);
            Property(x => x.Condition_).HasColumnName(@"Condition").HasColumnType("nvarchar").IsOptional().HasMaxLength(150);
        }
    }

}
// </auto-generated>
