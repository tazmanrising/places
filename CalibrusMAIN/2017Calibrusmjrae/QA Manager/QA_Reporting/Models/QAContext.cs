using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace QA_Reporting.Models
{
    public class QAContext : DbContext
    {
        public DbSet<QAList> QALists { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            try
            {

            }
            catch (Exception ex)
            {

                throw;
            }

            Database.SetInitializer<QAContext>(null);

            modelBuilder.Entity<QAList>().ToTable("QAList", schemaName: "dbo");





        }


    }
}