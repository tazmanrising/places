using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Calibrus.ClearviewPortal.DataAccess.CodeFirst.Models;

namespace Calibrus.ClearviewPortal.DataAccess.CodeFirst
{
    public class ClearviewContext : DbContext
    {
        public DbSet<ServiceableZipCodes> ServiceableZipCodeses { get; set; }
        public DbSet<UserLog> UserLogs { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            try
            {

            }
            catch (Exception ex)
            {

                throw;
            }

            Database.SetInitializer<ClearviewContext>(null);

            modelBuilder.Entity<ServiceableZipCodes>().ToTable("ServiceableZipCodes", schemaName: "dbo");

            modelBuilder.Entity<UserLog>().ToTable("UserLog", schemaName: "v1");



        }

    }
}
