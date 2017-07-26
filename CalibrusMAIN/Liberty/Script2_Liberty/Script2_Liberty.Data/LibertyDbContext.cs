using System;
using System.Data.Entity;
using Script2_Liberty.Data.Models;

namespace Script2_Liberty.Data
{
    public class LibertyDbContext : DbContext
    {
        //static LibertyDbContext()
        //{
        //    System.Data.Entity.Database.SetInitializer<LibertyDbContext>(null);
        //}

        public LibertyDbContext()
          : base("Name=LibertyConnString")
        {
        }

        public LibertyDbContext(string connectionString)
            : base(connectionString)
        {
        }

        public DbSet<ContractTerm> ContractTerms { get; set; }
        public DbSet<DeliveryZone> DeliveryZones { get; set; }
        public DbSet<Main> Mains { get; set; }
        public DbSet<MarketProduct> MarketProducts { get; set; }
        public DbSet<MarketState> MarketStates { get; set; }
        public DbSet<MarketUtility> MarketUtilitys { get; set; }
        public DbSet<Office> Offices { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<ProductContractLink> ProductContractLinks { get; set; }
        public DbSet<SalesChannel> SalesChannels { get; set; }
        public DbSet<Vendor> Vendors { get; set; }
        public DbSet<ZipCodeLookup> ZipCodeLookups { get; set; }

      

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //try
            //{

            //}
            //catch (Exception ex)
            //{

            //    throw;
            //}

            //Database.SetInitializer<LibertyDbContext>(null);

            modelBuilder.Entity<ContractTerm>().ToTable("ContractTerm", schemaName: "v1");
            modelBuilder.Entity<DeliveryZone>().ToTable("DeliveryZone", schemaName: "v1");
            modelBuilder.Entity<Main>().ToTable("Main", schemaName: "v1");
            modelBuilder.Entity<MarketProduct>().ToTable("MarketProduct", schemaName: "v1");
            modelBuilder.Entity<MarketState>().ToTable("MarketState", schemaName: "v1");
            modelBuilder.Entity<MarketUtility>().ToTable("MarketUtility", schemaName: "v1");
            modelBuilder.Entity<Office>().ToTable("Office", schemaName: "v1");
            modelBuilder.Entity<OrderDetail>().ToTable("OrderDetail", schemaName: "v1");
            modelBuilder.Entity<ProductContractLink>().ToTable("ProductContractLink", schemaName: "v1");
            modelBuilder.Entity<SalesChannel>().ToTable("SalesChannel", schemaName: "v1");
            modelBuilder.Entity<Vendor>().ToTable("Vendor", schemaName: "v1");
            modelBuilder.Entity<ZipCodeLookup>().ToTable("ZipCodeLookup", schemaName: "v1");

        }
    }
}
