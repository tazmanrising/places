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

    // QustionDirectiveAssoc
    [System.CodeDom.Compiler.GeneratedCode("EF.Reverse.POCO.Generator", "2.28.0.0")]
    public class QustionDirectiveAssocConfiguration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<QustionDirectiveAssoc>
    {
        public QustionDirectiveAssocConfiguration()
            : this("dbo")
        {
        }

        public QustionDirectiveAssocConfiguration(string schema)
        {
            ToTable("QustionDirectiveAssoc", schema);
            HasKey(x => x.Id);

            Property(x => x.Id).HasColumnName(@"Id").HasColumnType("int").IsRequired().HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.None);
            Property(x => x.QuestionId).HasColumnName(@"QuestionId").HasColumnType("int").IsRequired();
            Property(x => x.DirectiveId).HasColumnName(@"DirectiveId").HasColumnType("int").IsRequired();
            Property(x => x.SortOrder).HasColumnName(@"SortOrder").HasColumnType("int").IsRequired();

            // Foreign keys
            HasRequired(a => a.Directive).WithMany(b => b.QustionDirectiveAssocs).HasForeignKey(c => c.DirectiveId).WillCascadeOnDelete(false); // FK_QustionDirectiveAssoc_Directive
            HasRequired(a => a.Question).WithMany(b => b.QustionDirectiveAssocs).HasForeignKey(c => c.QuestionId).WillCascadeOnDelete(false); // FK_QustionDirectiveAssoc_Question
        }
    }

}
// </auto-generated>
