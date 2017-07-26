using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CalibrusTPV.Data.Configurations;
using CalibrusTPV.Data.Models;

namespace CalibrusTPV.Data.ScriptsDb
{
    public class ScriptsContext : DbContext
    {

        //public ScriptsContext() : base("ScriptsContext")
        //{
        //    Database.SetInitializer(new NullDatabaseInitializer<ScriptsContext>());

        //}


        public DbSet<Question> Questions { get; set; }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {

        
            Database.SetInitializer<ScriptsContext>(null);   //: base("Name=ScriptsContext")

            //modelBuilder.Entity<Question>().ToTable("Question", schemaName: "dbo");

            
            modelBuilder.Configurations.Add(new QuestionConfiguration());





        }



    }

}
