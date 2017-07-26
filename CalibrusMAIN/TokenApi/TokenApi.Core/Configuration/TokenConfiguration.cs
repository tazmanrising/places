using System.Data;
using System.Data.Entity.ModelConfiguration;
using TokenApi.Core.Models;


namespace TokenApi.Core.Configuration
{
    public class TokenConfiguration : EntityTypeConfiguration<TokenStore>
    {
        public TokenConfiguration()
        {
            ToTable("Tokens");
            HasKey(t => t.TokenId);
            

            //Property(t => t.SkillDesc).HasColumnName("SKILLDESC");
            //Property(p => p.SkillName).IsRequired().HasMaxLength(100);        
        }

    }

}
