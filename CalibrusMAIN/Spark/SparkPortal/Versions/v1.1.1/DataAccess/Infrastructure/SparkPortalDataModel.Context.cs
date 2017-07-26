﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Calibrus.SparkPortal.DataAccess.Infrastructure
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    
    public partial class SparkPortalDataEntities : DbContext
    {
        public SparkPortalDataEntities()
            : base("name=SparkPortalDataEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<AccountNumberType> AccountNumberTypes { get; set; }
        public virtual DbSet<PremiseType> PremiseTypes { get; set; }
        public virtual DbSet<ProgramVendor> ProgramVendors { get; set; }
        public virtual DbSet<State> States { get; set; }
        public virtual DbSet<UnitOfMeasure> UnitOfMeasures { get; set; }
        public virtual DbSet<UserLog> UserLogs { get; set; }
        public virtual DbSet<UserType> UserTypes { get; set; }
        public virtual DbSet<Utility> Utilities { get; set; }
        public virtual DbSet<UtilityType> UtilityTypes { get; set; }
        public virtual DbSet<Disposition> Dispositions { get; set; }
        public virtual DbSet<Report> Reports { get; set; }
        public virtual DbSet<Brand> Brands { get; set; }
        public virtual DbSet<OrderDetail> OrderDetails { get; set; }
        public virtual DbSet<Lead> Leads { get; set; }
        public virtual DbSet<Relationship> Relationships { get; set; }
        public virtual DbSet<IpLocation> IpLocations { get; set; }
        public virtual DbSet<Main> Mains { get; set; }
        public virtual DbSet<SalesChannel> SalesChannels { get; set; }
        public virtual DbSet<Program> Programs { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Office> Offices { get; set; }
        public virtual DbSet<SalesChannelProgram> SalesChannelPrograms { get; set; }
        public virtual DbSet<CurrentCustomer> CurrentCustomers { get; set; }
        public virtual DbSet<Vendor> Vendors { get; set; }
    
        public virtual ObjectResult<GetVerifiedChartDetail_Result> GetVerifiedChartDetail(Nullable<System.DateTime> startDate, Nullable<int> vendorId, Nullable<int> officeId)
        {
            var startDateParameter = startDate.HasValue ?
                new ObjectParameter("StartDate", startDate) :
                new ObjectParameter("StartDate", typeof(System.DateTime));
    
            var vendorIdParameter = vendorId.HasValue ?
                new ObjectParameter("VendorId", vendorId) :
                new ObjectParameter("VendorId", typeof(int));
    
            var officeIdParameter = officeId.HasValue ?
                new ObjectParameter("OfficeId", officeId) :
                new ObjectParameter("OfficeId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<GetVerifiedChartDetail_Result>("GetVerifiedChartDetail", startDateParameter, vendorIdParameter, officeIdParameter);
        }
    
        public virtual ObjectResult<GetVerifiedChartSummary_Result> GetVerifiedChartSummary(Nullable<System.DateTime> startDate, Nullable<int> vendorId, Nullable<int> officeId)
        {
            var startDateParameter = startDate.HasValue ?
                new ObjectParameter("StartDate", startDate) :
                new ObjectParameter("StartDate", typeof(System.DateTime));
    
            var vendorIdParameter = vendorId.HasValue ?
                new ObjectParameter("VendorId", vendorId) :
                new ObjectParameter("VendorId", typeof(int));
    
            var officeIdParameter = officeId.HasValue ?
                new ObjectParameter("OfficeId", officeId) :
                new ObjectParameter("OfficeId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<GetVerifiedChartSummary_Result>("GetVerifiedChartSummary", startDateParameter, vendorIdParameter, officeIdParameter);
        }
    
        public virtual ObjectResult<GetVerifiedAccountsChartDetail_Result> GetVerifiedAccountsChartDetail(Nullable<System.DateTime> startDate, Nullable<int> vendorId, Nullable<int> officeId)
        {
            var startDateParameter = startDate.HasValue ?
                new ObjectParameter("StartDate", startDate) :
                new ObjectParameter("StartDate", typeof(System.DateTime));
    
            var vendorIdParameter = vendorId.HasValue ?
                new ObjectParameter("VendorId", vendorId) :
                new ObjectParameter("VendorId", typeof(int));
    
            var officeIdParameter = officeId.HasValue ?
                new ObjectParameter("OfficeId", officeId) :
                new ObjectParameter("OfficeId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<GetVerifiedAccountsChartDetail_Result>("GetVerifiedAccountsChartDetail", startDateParameter, vendorIdParameter, officeIdParameter);
        }
    
        public virtual ObjectResult<GetVerifiedAccountsChartSummary_Result> GetVerifiedAccountsChartSummary(Nullable<System.DateTime> startDate, Nullable<int> vendorId, Nullable<int> officeId)
        {
            var startDateParameter = startDate.HasValue ?
                new ObjectParameter("StartDate", startDate) :
                new ObjectParameter("StartDate", typeof(System.DateTime));
    
            var vendorIdParameter = vendorId.HasValue ?
                new ObjectParameter("VendorId", vendorId) :
                new ObjectParameter("VendorId", typeof(int));
    
            var officeIdParameter = officeId.HasValue ?
                new ObjectParameter("OfficeId", officeId) :
                new ObjectParameter("OfficeId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<GetVerifiedAccountsChartSummary_Result>("GetVerifiedAccountsChartSummary", startDateParameter, vendorIdParameter, officeIdParameter);
        }
    
        public virtual ObjectResult<GetTopVendors_Result> GetTopVendors(Nullable<System.DateTime> startDate)
        {
            var startDateParameter = startDate.HasValue ?
                new ObjectParameter("StartDate", startDate) :
                new ObjectParameter("StartDate", typeof(System.DateTime));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<GetTopVendors_Result>("GetTopVendors", startDateParameter);
        }
    
        public virtual ObjectResult<GetTopOfficeStats_Result> GetTopOfficeStats(Nullable<System.DateTime> startDate, Nullable<int> vendorId, Nullable<int> officeId)
        {
            var startDateParameter = startDate.HasValue ?
                new ObjectParameter("StartDate", startDate) :
                new ObjectParameter("StartDate", typeof(System.DateTime));
    
            var vendorIdParameter = vendorId.HasValue ?
                new ObjectParameter("VendorId", vendorId) :
                new ObjectParameter("VendorId", typeof(int));
    
            var officeIdParameter = officeId.HasValue ?
                new ObjectParameter("OfficeId", officeId) :
                new ObjectParameter("OfficeId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<GetTopOfficeStats_Result>("GetTopOfficeStats", startDateParameter, vendorIdParameter, officeIdParameter);
        }
    
        public virtual ObjectResult<GetTopVendorStats_Result> GetTopVendorStats(Nullable<System.DateTime> startDate, Nullable<int> vendorId)
        {
            var startDateParameter = startDate.HasValue ?
                new ObjectParameter("StartDate", startDate) :
                new ObjectParameter("StartDate", typeof(System.DateTime));
    
            var vendorIdParameter = vendorId.HasValue ?
                new ObjectParameter("VendorId", vendorId) :
                new ObjectParameter("VendorId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<GetTopVendorStats_Result>("GetTopVendorStats", startDateParameter, vendorIdParameter);
        }
    
        public virtual ObjectResult<GetTopUserStats_Result> GetTopUserStats(Nullable<System.DateTime> startDate, Nullable<int> vendorId, Nullable<int> officeId)
        {
            var startDateParameter = startDate.HasValue ?
                new ObjectParameter("StartDate", startDate) :
                new ObjectParameter("StartDate", typeof(System.DateTime));
    
            var vendorIdParameter = vendorId.HasValue ?
                new ObjectParameter("VendorId", vendorId) :
                new ObjectParameter("VendorId", typeof(int));
    
            var officeIdParameter = officeId.HasValue ?
                new ObjectParameter("OfficeId", officeId) :
                new ObjectParameter("OfficeId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<GetTopUserStats_Result>("GetTopUserStats", startDateParameter, vendorIdParameter, officeIdParameter);
        }
    
        public virtual int spInActivateUser()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("spInActivateUser");
        }
    
        public virtual ObjectResult<spGetUsers_Result> spGetUsers(string queryType, Nullable<int> id, Nullable<bool> isActive)
        {
            var queryTypeParameter = queryType != null ?
                new ObjectParameter("queryType", queryType) :
                new ObjectParameter("queryType", typeof(string));
    
            var idParameter = id.HasValue ?
                new ObjectParameter("Id", id) :
                new ObjectParameter("Id", typeof(int));
    
            var isActiveParameter = isActive.HasValue ?
                new ObjectParameter("IsActive", isActive) :
                new ObjectParameter("IsActive", typeof(bool));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<spGetUsers_Result>("spGetUsers", queryTypeParameter, idParameter, isActiveParameter);
        }
    }
}
