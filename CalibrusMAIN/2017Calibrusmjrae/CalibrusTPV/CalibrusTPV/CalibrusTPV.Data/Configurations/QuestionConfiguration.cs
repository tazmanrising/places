using System.Data.Entity.ModelConfiguration;
using CalibrusTPV.Data.Models;

namespace CalibrusTPV.Data.Configurations
{
    public class QuestionConfiguration : EntityTypeConfiguration<Question>
    {

        public QuestionConfiguration()
        {
            ToTable("Question");
            HasKey(t => t.Id);

            Property(t => t.Description).HasColumnName("Description");
            Property(p => p.Name).IsRequired().HasMaxLength(50);

        }
        
       
    }
}
