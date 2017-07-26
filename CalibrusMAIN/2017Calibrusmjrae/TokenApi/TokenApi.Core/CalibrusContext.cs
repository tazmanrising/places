using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TokenApi.Core.Configuration;
using TokenApi.Core.Models;

namespace TokenApi.Core
{

    

    public class CalibrusContext : DbContext
    {




        public DbSet<ApiLog> ApiLogs { get; set; }
        public DbSet<TokenStore> TokenStores { get; set; }
        public DbSet<ApiAccess> ApiAccesses { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {

            try
            {
                //todo  
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }


            Database.SetInitializer<CalibrusContext>(null);
            modelBuilder.Entity<ApiLog>().ToTable("ApiLog", schemaName: "dbo");
            modelBuilder.Entity<TokenStore>().ToTable("Tokens", schemaName: "dbo");
            modelBuilder.Entity<ApiAccess>().ToTable("ApiAccess", schemaName: "dbo");

            //todo  possible use configurations instead
            //modelBuilder.Configurations.Add(new TokenConfiguration());





        }
        

    }

}
