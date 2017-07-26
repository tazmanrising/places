namespace CalibrusTPV.MSReverseEngineer
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class DbFirstModel : DbContext
    {
        public DbFirstModel()
            : base("name=DbFirstModel")
        {
        }

        public virtual DbSet<Condition> Conditions { get; set; }
        public virtual DbSet<Directive> Directives { get; set; }
        public virtual DbSet<LookUp> LookUps { get; set; }
        public virtual DbSet<Question> Questions { get; set; }
        public virtual DbSet<QustionDirectiveAssoc> QustionDirectiveAssocs { get; set; }
        public virtual DbSet<Recording> Recordings { get; set; }
        public virtual DbSet<ScriptQuestion> ScriptQuestions { get; set; }
        public virtual DbSet<State> States { get; set; }
        public virtual DbSet<sysdiagram> sysdiagrams { get; set; }
        public virtual DbSet<TPV> TPVs { get; set; }
        public virtual DbSet<OrderDetail> OrderDetails { get; set; }
        public virtual DbSet<vGender> vGenders { get; set; }
        public virtual DbSet<vLookup> vLookups { get; set; }
        public virtual DbSet<vQtype> vQtypes { get; set; }
        public virtual DbSet<vSalesChannel> vSalesChannels { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Directive>()
                .HasMany(e => e.QustionDirectiveAssocs)
                .WithRequired(e => e.Directive)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Question>()
                .HasMany(e => e.QustionDirectiveAssocs)
                .WithRequired(e => e.Question)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Question>()
                .HasMany(e => e.ScriptQuestions)
                .WithRequired(e => e.Question)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Recording>()
                .Property(e => e.wavename)
                .IsFixedLength();

            modelBuilder.Entity<ScriptQuestion>()
                .Property(e => e.StateCode)
                .IsFixedLength();

            modelBuilder.Entity<State>()
                .Property(e => e.StateCode)
                .IsFixedLength();

            modelBuilder.Entity<State>()
                .HasMany(e => e.ScriptQuestions)
                .WithRequired(e => e.State)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<TPV>()
                .Property(e => e.Dnis)
                .IsFixedLength();

            modelBuilder.Entity<TPV>()
                .Property(e => e.Verified)
                .IsFixedLength();

            modelBuilder.Entity<TPV>()
                .HasMany(e => e.Recordings)
                .WithRequired(e => e.TPV)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<OrderDetail>()
                .Property(e => e.UtilityType)
                .IsUnicode(false);

            modelBuilder.Entity<OrderDetail>()
                .Property(e => e.AccountType)
                .IsUnicode(false);

            modelBuilder.Entity<OrderDetail>()
                .Property(e => e.AccountNumber)
                .IsUnicode(false);

            modelBuilder.Entity<OrderDetail>()
                .Property(e => e.MeterNumber)
                .IsUnicode(false);

            modelBuilder.Entity<OrderDetail>()
                .Property(e => e.RateClass)
                .IsUnicode(false);

            modelBuilder.Entity<OrderDetail>()
                .Property(e => e.CustomerNameKey)
                .IsUnicode(false);

            modelBuilder.Entity<OrderDetail>()
                .Property(e => e.ServiceReferenceNumber)
                .IsUnicode(false);

            modelBuilder.Entity<OrderDetail>()
                .Property(e => e.ServiceAddress)
                .IsUnicode(false);

            modelBuilder.Entity<OrderDetail>()
                .Property(e => e.ServiceCity)
                .IsUnicode(false);

            modelBuilder.Entity<OrderDetail>()
                .Property(e => e.ServiceState)
                .IsUnicode(false);

            modelBuilder.Entity<OrderDetail>()
                .Property(e => e.ServiceZip)
                .IsUnicode(false);

            modelBuilder.Entity<OrderDetail>()
                .Property(e => e.ServiceCounty)
                .IsUnicode(false);

            modelBuilder.Entity<OrderDetail>()
                .Property(e => e.BillingAddress)
                .IsUnicode(false);

            modelBuilder.Entity<OrderDetail>()
                .Property(e => e.BillingCity)
                .IsUnicode(false);

            modelBuilder.Entity<OrderDetail>()
                .Property(e => e.BillingState)
                .IsUnicode(false);

            modelBuilder.Entity<OrderDetail>()
                .Property(e => e.BillingZip)
                .IsUnicode(false);

            modelBuilder.Entity<OrderDetail>()
                .Property(e => e.BillingCounty)
                .IsUnicode(false);

            modelBuilder.Entity<OrderDetail>()
                .Property(e => e.InCityLimits)
                .IsUnicode(false);

            modelBuilder.Entity<OrderDetail>()
                .Property(e => e.BillingFirstName)
                .IsUnicode(false);

            modelBuilder.Entity<OrderDetail>()
                .Property(e => e.BillingLastName)
                .IsUnicode(false);
        }
    }
}
