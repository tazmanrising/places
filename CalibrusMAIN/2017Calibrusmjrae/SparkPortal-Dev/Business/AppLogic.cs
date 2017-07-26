using Calibrus.SparkPortal.DataAccess.Infrastructure;
using Calibrus.SparkPortal.DataAccess.Models;
using Calibrus.SparkPortal.DataAccess.Repository;
using System;
using System.Data;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Spatial;
using System.Data.SqlClient;
using System.Linq;


namespace Calibrus.SparkPortal.Business
{
    public static class AppLogic
    {
        public static int CopyProgram(int id, string loggedOnUser)
        {
            using (CustomSparkPortalDataEntities ctx = new CustomSparkPortalDataEntities())
            {
                RateRepository repo = new RateRepository(ctx);
                Program current = repo.Find(r => r.ProgramId == id, type => type.ProgramVendors);

                Program copy = new Program
                {
                    AccountNumberFixedLength = current.AccountNumberFixedLength,
                    AccountNumberLength = current.AccountNumberLength,
                    AccountNumberTypeId = current.AccountNumberTypeId,
                    BrandId = current.BrandId,
                    CancellationVerbiage = current.CancellationVerbiage,
                    CancellationVerbiageSpanish = current.CancellationVerbiageSpanish,
                    EffectiveEndDate = current.EffectiveEndDate,
                    EffectiveStartDate = current.EffectiveStartDate,
                    Etf = current.Etf,
                    Hefpa = current.Hefpa,
                    Market = current.Market,
                    MeterNumber = current.MeterNumber,
                    MeterNumberLength = current.MeterNumberLength,
                    Msf = current.Msf,
                    PremiseTypeId = current.PremiseTypeId,
                    ProgramCode = current.ProgramCode + " - COPY",
                    ProgramName = current.ProgramName + " - COPY",
                    ProgramDescription = current.ProgramDescription,
                    DefaultPricingPlanDescription = current.DefaultPricingPlanDescription,
                    PromotionalCode = current.PromotionalCode,
                    Rate = current.Rate,
                    RateVerbiage = current.RateVerbiage,
                    RateVerbiageSpanish = current.RateVerbiageSpanish,
                    RescindBy = current.RescindBy,
                    SalesChannel = current.SalesChannel,
                    State = current.State,
                    Term = current.Term,
                    UnitOfMeasureId = current.UnitOfMeasureId,
                    UtilityId = current.UtilityId,
                    UtilityTypeId = current.UtilityTypeId,
                    UpdatedBy = loggedOnUser,
                    UpdatedDateTime = DateTime.Now,
                    ServiceReference = current.ServiceReference,
                    CreditCheck = current.CreditCheck
                };

                foreach (ProgramVendor pv in current.ProgramVendors)
                {
                    copy.ProgramVendors.Add(new ProgramVendor
                    {
                        VendorId = pv.VendorId,
                        CreatedBy = "test",
                        CreatedDateTime = DateTime.Now
                    });
                }

                repo.Create(copy);
                ctx.SaveChanges();

                return copy.ProgramId;
            }
        }

        public static Main MainVerified(int MainId)
        {
            using (CustomSparkPortalDataEntities ctx = new CustomSparkPortalDataEntities())
            {
                MainRepository repo = new MainRepository(ctx);
                return repo.Find(MainId);
            }
        }

        public static DataAccess.Entities.Verification UpdateMainVerifiedToReverse(int MainId)
        {
            DataAccess.Entities.Verification ReverseMain = new DataAccess.Entities.Verification();

            if (string.IsNullOrEmpty(MainId.ToString()))
            {
                throw new System.ArgumentNullException() { Source = "Main" };
            }
            else
            {
                ReverseMain.MainId = MainId;
                ReverseMain.Verified = "8";
                ReverseMain.Concern = "Reversed";
                ReverseMain.ConcernCode = "00088";
            }

            using (CustomSparkPortalDataEntities ctx = new CustomSparkPortalDataEntities())
            {
                MainRepository repo = new MainRepository(ctx);

                Main m = MainVerified(ReverseMain.MainId);
                m.Verified = ReverseMain.Verified;
                m.Concern = ReverseMain.Concern;
                m.ConcernCode = ReverseMain.ConcernCode;

                repo.Update(m);
                ctx.SaveChanges();

            }
            return ReverseMain;
        }


        public static bool PhoneNumberExists(string phone)
        {
            DateTime d = DateTime.Now.AddDays(-180);

            using (CustomSparkPortalDataEntities ctx = new CustomSparkPortalDataEntities())
            {
                MainRepository repo = new MainRepository(ctx);
                List<Main> main = repo.Filter(x => x.Btn == phone && x.Verified == "1" && x.CallDateTime.Value > d);
                return main.Count > 0;
            }
        }

        public static bool LeadExists(int leadId)
        {
            if (leadId == 0) return false; //leadId == 0 means that this is a manual entry
            using (CustomSparkPortalDataEntities ctx = new CustomSparkPortalDataEntities())
            {
                MainRepository repo = new MainRepository(ctx);
                List<Main> main = repo.Filter(x => x.LeadsId > 0 && x.LeadsId == leadId && x.Verified == "1");
                return main.Count > 0;
            }
        }

        public static bool AccountNumberExists(string accountNumber)
        {
            using (CustomSparkPortalDataEntities ctx = new CustomSparkPortalDataEntities())
            {
                CurrentCustomerRepository repo = new CurrentCustomerRepository(ctx);
                List<CurrentCustomer> cust = repo.Filter(x => x.AccountNumber == accountNumber);
                return cust.Count > 0;
            }
        }

        public static Main CreateRequest(DataAccess.Entities.Request request)
        {

            Main main = new Main
            {
                UserId = request.User.UserId,
                Btn = request.Phone,
                AccountFirstName = request.FirstName,
                AccountLastName = request.LastName,
                AuthorizationFirstName = request.FirstName,
                AuthorizationLastName = request.LastName,
                CompanyName = request.BusinessName,
                CompanyContactFirstName = request.BusinessFirstName,
                CompanyContactLastName = request.BusinessLastName,
                CompanyContactTitle = request.BusinessTitle,
                RecordLocator = request.Lead.RecordLocator,
                LeadsId = request.Lead.LeadsId,
                NumberOfAccounts = request.OrderDetails.Count.ToString(),
                Verified = "9",
                WebDateTime = DateTime.Now,
                Concern = "No TPV Call",
                SourceId = 2,
                geolocation = (request.Geolocation == null ? null : DbGeography.FromText($"Point({request.Geolocation.lng} {request.Geolocation.lat})")),
                OrderDetails = new List<OrderDetail>(),
                IpLocations = new List<IpLocation>()
            };

            main.IpLocations.Add(new IpLocation
            {
                Ip = request.IpLocation?.Ip,
                City = request.IpLocation?.City,
                Country = request.IpLocation?.Country,
                HostName = request.IpLocation?.HostName,
                Loc = request.IpLocation?.Loc,
                Org = request.IpLocation?.Org,
                Postal = request.IpLocation?.Postal,
                Region = request.IpLocation?.Region
            });

            foreach (DataAccess.Entities.Request.RequestOrderDetail detail in request.OrderDetails)
            {
                main.Relation = detail.Relationship;

                OrderDetail o = new OrderDetail
                {
                    AccountNumber = detail.AccountNumber,
                    AccountType = detail.Program.AccountNumberType?.AccountNumberTypeName ?? "Unknown",
                    UtilityType = detail.UtilityType,
                    BillingAddress = $"{detail.BillingAddress} {detail.BillingAddress2}",
                    BillingCity = detail.BillingCity,
                    BillingState = detail.BillingState,
                    BillingZip = detail.BillingZip,
                    BillingFirstName = detail.BillingFirstName,
                    BillingLastName = detail.BillingLastName,
                    ServiceAddress = $"{detail.Address} {detail.Address2}",
                    ServiceCity = detail.City,
                    ServiceState = detail.State,
                    ServiceZip = detail.Zip,
                    MeterNumber = detail.MeterNumber,
                    ServiceReferenceNumber = detail.ServiceReference,
                    ProgramId = detail.Program.ProgramId
                };

                main.OrderDetails.Add(o);
            }

            using (CustomSparkPortalDataEntities ctx = new CustomSparkPortalDataEntities())
            {
                MainRepository repo = new MainRepository(ctx);
                repo.Create(main);
                ctx.SaveChanges();
            }

            return main;
        }

        public static List<Utility> GetUtilities(int vendorId, Enums.UtilityType utilityType, Enums.PremiseType accountType, string state)
        {
            List<Utility> utilities = new List<Utility>();
            DateTime dateOnly = DateTime.Now.Date;

            using (CustomSparkPortalDataEntities ctx = new CustomSparkPortalDataEntities())
            {
                RateRepository repo = new RateRepository(ctx);
                List<Program> programs = repo.Filter(r => r.ProgramVendors.Any(v => v.VendorId == vendorId)
                                                && r.PremiseTypeId == (int)accountType
                                                && r.UtilityTypeId == (int)utilityType
                                                && r.State == state
                                                && dateOnly >= r.EffectiveStartDate
                                                && dateOnly <= r.EffectiveEndDate
                                            , type => type.Utility);

                foreach (Program p in programs)
                {
                    utilities.Add(p.Utility);
                }
            }

            return utilities.Distinct().OrderBy(sort => sort.Name).ToList();
        }

        public static List<Utility> GetUtilities(int vendorId, Enums.UtilityType utilityType, Enums.PremiseType accountType)
        {
            List<Utility> utilities = new List<Utility>();
            DateTime dateOnly = DateTime.Now.Date;

            using (CustomSparkPortalDataEntities ctx = new CustomSparkPortalDataEntities())
            {
                RateRepository repo = new RateRepository(ctx);
                List<Program> programs = repo.Filter(r => r.ProgramVendors.Any(v => v.VendorId == vendorId)
                                                && r.PremiseTypeId == (int)accountType
                                                && r.UtilityTypeId == (int)utilityType
                                                && dateOnly >= r.EffectiveStartDate
                                                && dateOnly <= r.EffectiveEndDate
                                            , type => type.Utility);

                foreach (Program p in programs)
                {
                    utilities.Add(p.Utility);
                }
            }

            return utilities.Distinct().OrderBy(sort => sort.Name).ToList();
        }

        public static int? CreateOffice(Office office)
        {
            if (office == null)
            {
                throw new System.ArgumentNullException() { Source = "office" };
            }

            using (CustomSparkPortalDataEntities ctx = new CustomSparkPortalDataEntities())
            {
                OfficeRepository repo = new OfficeRepository(ctx);
                repo.Create(office);
                ctx.SaveChanges();
            }

            return office.OfficeId;
        }

        public static int CreateRate(Program program)
        {
            if (program == null)
            {
                throw new System.ArgumentNullException() { Source = "program" };
            }

            using (CustomSparkPortalDataEntities ctx = new CustomSparkPortalDataEntities())
            {
                RateRepository repo = new RateRepository(ctx);
                repo.Create(program);
                ctx.SaveChanges();
            }

            return program.ProgramId;
        }

        public static int CreateUser(User user)
        {
            if (user == null)
            {
                throw new System.ArgumentNullException() { Source = "user" };
            }

            using (CustomSparkPortalDataEntities ctx = new CustomSparkPortalDataEntities())
            {
                UserRepository userRepository = new UserRepository(ctx);
                userRepository.Create(user);
                ctx.SaveChanges();
            }

            return user.UserId;
        }

        public static int CreateSalesChannelProgram(SalesChannelProgram scp)
        {
            if (scp == null)
            {
                throw new System.ArgumentNullException() { Source = "scp" };
            }

            using (CustomSparkPortalDataEntities ctx = new CustomSparkPortalDataEntities())
            {
                SalesChannelProgramRepository SalesChannelProgramRepository = new SalesChannelProgramRepository(ctx);
                SalesChannelProgramRepository.Create(scp);
                ctx.SaveChanges();
            }

            return scp.Id;
        }

        public static List<Title> GetTitles()
        {
            List<Title> t;
            using (CustomSparkPortalDataEntities ctx = new CustomSparkPortalDataEntities())
            {
                TitleRepository repo = new TitleRepository(ctx);
                t = repo.Filter(x => x.IsActive == true);
                ctx.SaveChanges();
            }

            return t;
        }

        public static List<Relationship> GetRelationships()
        {
            List<Relationship> r;
            using (CustomSparkPortalDataEntities ctx = new CustomSparkPortalDataEntities())
            {
                RelationshipRepository repo = new RelationshipRepository(ctx);
                r = repo.Filter(x => x.IsActive == true);
            }
            return r;
        }

        public static int CreateUserLog(UserLog userLog)
        {
            if (userLog == null)
            {
                throw new System.ArgumentNullException() { Source = "userLog" };
            }

            using (CustomSparkPortalDataEntities ctx = new CustomSparkPortalDataEntities())
            {
                UserLogRepository repo = new UserLogRepository(ctx);
                repo.Create(userLog);
                ctx.SaveChanges();
            }

            return userLog.UserLogId;
        }

        public static int CreateVendor(Vendor vendor)
        {
            if (vendor == null)
            {
                throw new System.ArgumentNullException() { Source = "vendor" };
            }

            using (CustomSparkPortalDataEntities ctx = new CustomSparkPortalDataEntities())
            {
                VendorRepository repo = new VendorRepository(ctx);
                repo.Create(vendor);
                ctx.SaveChanges();
            }

            return vendor.VendorId;
        }

        public static List<AccountNumberType> GetActiveAccountNumberTypes()
        {
            List<AccountNumberType> acct;
            using (CustomSparkPortalDataEntities ctx = new CustomSparkPortalDataEntities())
            {
                AccountNumberTypeRepository repo = new AccountNumberTypeRepository(ctx);
                acct = repo.GetActiveItems();
            }
            return acct;
        }

        public static bool ProgramCodeExists(int id, string programCode, DateTime startDate, DateTime endDate)
        {
            using (CustomSparkPortalDataEntities ctx = new CustomSparkPortalDataEntities())
            {
                RateRepository repo = new RateRepository(ctx);

                var test = (repo.Filter(x => x.ProgramId != id
                            && x.ProgramCode == programCode
                            && ((startDate >= x.EffectiveStartDate && startDate <= x.EffectiveEndDate) || (endDate >= x.EffectiveStartDate && endDate <= x.EffectiveEndDate))
                            ));


                return (repo.Count(x => x.ProgramId != id
                            && x.ProgramCode == programCode
                            && ((startDate >= x.EffectiveStartDate && startDate <= x.EffectiveEndDate) || (endDate >= x.EffectiveStartDate && endDate <= x.EffectiveEndDate))
                            ) > 0);
            }
        }

        public static List<PremiseType> GetActivePremiseTypes()
        {
            List<PremiseType> pt;
            using (CustomSparkPortalDataEntities ctx = new CustomSparkPortalDataEntities())
            {
                PremiseTypeRepository repo = new PremiseTypeRepository(ctx);
                pt = repo.GetActiveItems();
            }
            return pt;
        }

        public static List<SalesChannel> GetActiveSalesChannels()
        {
            List<SalesChannel> pt;
            using (CustomSparkPortalDataEntities ctx = new CustomSparkPortalDataEntities())
            {
                SalesChannelRepository repo = new SalesChannelRepository(ctx);
                pt = repo.GetActiveItems();
            }
            return pt;
        }

        public static List<UnitOfMeasure> GetActiveUnitOfMeasures()
        {
            List<UnitOfMeasure> uom;
            using (CustomSparkPortalDataEntities ctx = new CustomSparkPortalDataEntities())
            {
                UnitOfMeasureRepository repo = new UnitOfMeasureRepository(ctx);
                uom = repo.GetActiveItems();
            }
            return uom;
        }

        public static List<UserType> GetActiveUserTypes(int securityLevel)
        {
            List<UserType> ut;
            using (CustomSparkPortalDataEntities ctx = new CustomSparkPortalDataEntities())
            {
                UserTypeRepository repo = new UserTypeRepository(ctx);
                ut = repo.GetActiveItems().Where(x => x.SecurityLevel <= securityLevel).ToList();
            }
            return ut;
        }

        public static List<Utility> GetActiveUtilities()
        {
            List<Utility> ut;
            using (CustomSparkPortalDataEntities ctx = new CustomSparkPortalDataEntities())
            {
                UtilityRepository repo = new UtilityRepository(ctx);
                ut = repo.GetActiveItems();
            }
            return ut;
        }

        public static List<UtilityType> GetActiveUtilityTypes()
        {
            List<UtilityType> ut;
            using (CustomSparkPortalDataEntities ctx = new CustomSparkPortalDataEntities())
            {
                UtilityTypeRepository repo = new UtilityTypeRepository(ctx);
                ut = repo.GetActiveItems();
            }
            return ut;
        }
        public static Office GetOffice(int id)
        {
            Office office;
            using (CustomSparkPortalDataEntities ctx = new CustomSparkPortalDataEntities())
            {
                OfficeRepository repo = new OfficeRepository(ctx);
                office = repo.Find(x => x.OfficeId == id, type => type.Users, type => type.Vendor, type => type.SalesChannel);
            }
            return office;
        }

        public static List<Office> GetOffices(bool activeOnly)
        {
            List<Office> offices;
            using (CustomSparkPortalDataEntities ctx = new CustomSparkPortalDataEntities())
            {
                OfficeRepository repo = new OfficeRepository(ctx);
                offices = activeOnly ?
                    repo.Filter(v => v.IsActive, sort => sort.OfficeName, SortOrder.Ascending, type => type.Vendor, type => type.Users, type => type.SalesChannel)
                    : repo.All(t => t.Users, type => type.Vendor, type => type.Users, type => type.SalesChannel);
            }
            return offices;
        }

        public static List<Brand> GetBrands(bool v)
        {
            List<Brand> b;
            using (CustomSparkPortalDataEntities ctx = new CustomSparkPortalDataEntities())
            {
                BrandRepository repo = new BrandRepository(ctx);
                b = repo.All();
            }
            return b;
        }

        public static List<Office> GetOffices(bool activeOnly, int vendorId)
        {
            List<Office> offices;
            using (CustomSparkPortalDataEntities ctx = new CustomSparkPortalDataEntities())
            {
                OfficeRepository repo = new OfficeRepository(ctx);
                offices = activeOnly
                    ? repo.Filter(v => v.VendorId == vendorId && v.IsActive, sort => sort.OfficeName, SortOrder.Ascending, type => type.Vendor, type => type.Users, type => type.SalesChannel)
                    : repo.Filter(v => v.VendorId == vendorId, type => type.Vendor, type => type.Users, type => type.SalesChannel);
            }
            return offices;
        }

        public static List<User> GetOfficeUsers(int officeId, bool activeOnly)
        {
            List<User> users;
            using (CustomSparkPortalDataEntities ctx = new CustomSparkPortalDataEntities())
            {
                UserRepository repo = new UserRepository(ctx);
                users = activeOnly
                    ? repo.Filter(u => u.OfficeId == officeId && u.IsActive, sort => sort.LastName, SortOrder.Ascending, type => type.UserType, type => type.Vendor, type => type.Office)
                    : repo.Filter(u => u.OfficeId == officeId, sort => sort.LastName, SortOrder.Ascending, type => type.UserType, type => type.Vendor, type => type.Office);
                //ctx.SaveChanges();
            }
            return users;
        }

        public static Program GetProgram(int id)
        {
            Program program;
            using (CustomSparkPortalDataEntities ctx = new CustomSparkPortalDataEntities())
            {
                RateRepository repo = new RateRepository(ctx);
                program = repo.Find(x => x.ProgramId == id, t => t.ProgramVendors, s => s.SalesChannelPrograms);
            }
            return program;
        }

        public static Lead GetLead(string recordLocator, string vendorNumber)
        {
            Lead lead;
            using (CustomSparkPortalDataEntities ctx = new CustomSparkPortalDataEntities())
            {
                LeadRepository repo = new LeadRepository(ctx);
                lead = repo.Find(x => x.RecordLocator == recordLocator && x.VendorNumber == vendorNumber);
            }
            return lead;
        }
        public static Lead GetEsiid(string esiid, string vendorNumber)
        {
            Lead lead;
            using (CustomSparkPortalDataEntities ctx = new CustomSparkPortalDataEntities())
            {
                LeadRepository repo = new LeadRepository(ctx);
                lead = repo.Find(x => x.ESIID == esiid && x.VendorNumber == vendorNumber);
            }
            return lead;
        }

        public static List<spGetMainClone_Result> GetMainClone(int mainId)
        {
            List<spGetMainClone_Result> result = null;
            using (CustomSparkPortalDataEntities ctx = new CustomSparkPortalDataEntities())
            {
                result = ctx.spGetMainClone(mainId: mainId).ToList();
            }
            return result;
        }

        public static List<Program> GetPrograms(bool activeOnly)
        {
            List<Program> programs;
            DateTime dateOnly = DateTime.Now.Date;

            using (CustomSparkPortalDataEntities ctx = new CustomSparkPortalDataEntities())
            {
                RateRepository repo = new RateRepository(ctx);
                programs = activeOnly
                    ? repo.Filter(a => dateOnly >= a.EffectiveStartDate && dateOnly <= a.EffectiveEndDate, sort => sort.ProgramName, SortOrder.Ascending, u => u.UnitOfMeasure, p => p.PremiseType,
                        ut => ut.UtilityType, u => u.Utility, b => b.Brand, s => s.SalesChannelPrograms)
                    : repo.All(sort => sort.ProgramName, SortOrder.Ascending, t => t.UnitOfMeasure, u => u.UtilityType, p => p.PremiseType,
                        ut => ut.UtilityType, u => u.Utility, b => b.Brand, s => s.SalesChannelPrograms); //EffecticeEndDate >= today
            }
            return programs;
        }

        public static List<Program> GetPrograms(bool activeOnly, int id)
        {
            List<Program> programs;
            DateTime dateOnly = DateTime.Now.Date;

            using (CustomSparkPortalDataEntities ctx = new CustomSparkPortalDataEntities())
            {
                RateRepository repo = new RateRepository(ctx);
                programs = activeOnly
                    ? repo.Filter(a => a.ProgramVendors.Any(v => v.VendorId == id) && dateOnly >= a.EffectiveStartDate && dateOnly <= a.EffectiveEndDate,
                        sort => sort.ProgramName, SortOrder.Ascending, u => u.UnitOfMeasure, p => p.PremiseType,
                        ut => ut.UtilityType, u => u.Utility, b => b.Brand, s => s.SalesChannelPrograms)
                    : repo.Filter(a => a.ProgramVendors.Any(v => v.VendorId == id), sort => sort.ProgramName, SortOrder.Ascending, t => t.UnitOfMeasure, u => u.UtilityType, p => p.PremiseType,
                        ut => ut.UtilityType, u => u.Utility, b => b.Brand, s => s.SalesChannelPrograms); //EffecticeEndDate >= today
            }
            return programs;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="utilityId"></param>
        /// <param name="vendorId"></param>
        /// <param name="utilityType">Gas, Electric, Duel Fuel</param>
        /// <param name="accountType">B = Business, R = Residential</param>
        /// <returns></returns>
        public static List<Program> GetPrograms(int utilityId, int vendorId, Enums.UtilityType utilityType, Enums.PremiseType premiseType)
        {
            List<Program> programs;
            DateTime dateOnly = DateTime.Now.Date;

            using (CustomSparkPortalDataEntities ctx = new CustomSparkPortalDataEntities())
            {
                RateRepository repo = new RateRepository(ctx);
                programs = repo.Filter(p => p.UtilityId == utilityId
                                        && p.PremiseTypeId == (int)premiseType
                                        && p.ProgramVendors.Any(pv => pv.VendorId == vendorId)
                                        && p.UtilityTypeId == (int)utilityType
                                        && dateOnly >= p.EffectiveStartDate
                                        && dateOnly <= p.EffectiveEndDate,
                        sort => sort.ProgramName, SortOrder.Ascending, u => u.UnitOfMeasure, p => p.PremiseType,
                        ut => ut.UtilityType, u => u.Utility, b => b.Brand, a => a.AccountNumberType, v => v.ProgramVendors);
            }
            return programs;
        }

        public static List<Vendor> GetProgramVendors(int programId)
        {
            List<Vendor> vendors;
            using (CustomSparkPortalDataEntities ctx = new CustomSparkPortalDataEntities())
            {
                VendorRepository repo = new VendorRepository(ctx);
                vendors = repo.Filter(v => v.ProgramVendors.Any(p => p.ProgramId == programId));
            }
            return vendors;
        }

        public static List<State> GetStates()
        {
            List<State> state;
            using (CustomSparkPortalDataEntities ctx = new CustomSparkPortalDataEntities())
            {
                StateRepository repo = new StateRepository(ctx);
                state = repo.All(s => s.StateCode, System.Data.SqlClient.SortOrder.Ascending);
            }
            return state;
        }

        public static User GetUser(int id)
        {
            User user;
            using (CustomSparkPortalDataEntities ctx = new CustomSparkPortalDataEntities())
            {
                UserRepository repo = new UserRepository(ctx);
                user = repo.Find(x => x.UserId == id, type => type.UserLogs, type => type.UserType, type => type.Vendor, type => type.Office, Log => Log.UserLogs);
            }
            return user;
        }

        public static List<UserLog> GetUserLogs(int id)
        {
            List<UserLog> userlog;
            using (CustomSparkPortalDataEntities ctx = new CustomSparkPortalDataEntities())
            {
                UserLogRepository repo = new UserLogRepository(ctx);
                userlog = repo.Filter(x => x.UserId == id);
            }
            return userlog;
        }


        public static User GetUser(string userName)
        {
            User user;
            using (CustomSparkPortalDataEntities ctx = new CustomSparkPortalDataEntities())
            {
                UserRepository repo = new UserRepository(ctx);
                user = repo.Find(x => x.AgentId == userName, type => type.UserLogs, type => type.UserType);
            }
            return user;
        }

        public static User GetVendorUser(string userName, int vendorId)
        {
            User user;
            using (CustomSparkPortalDataEntities ctx = new CustomSparkPortalDataEntities())
            {
                UserRepository repo = new UserRepository(ctx);
                user = repo.Find(x => (x.AgentId == userName && x.VendorId == vendorId && x.UserType.SecurityLevel == 0) || (x.AgentId == userName && x.UserType.SecurityLevel > 0), type => type.UserLogs, type => type.UserType);
            }
            return user;
        }

        public static User GetUserForUpdate(int id)
        {
            User user;
            using (CustomSparkPortalDataEntities ctx = new CustomSparkPortalDataEntities())
            {
                UserRepository repo = new UserRepository(ctx);
                user = repo.Find(x => x.UserId == id);
            }
            return user;
        }

        public static List<User> GetUsers(bool activeOnly)
        {
            List<User> users;
            using (CustomSparkPortalDataEntities ctx = new CustomSparkPortalDataEntities())
            {
                UserRepository repo = new UserRepository(ctx);
                users = activeOnly ? repo.Filter(u => u.IsActive, sort => sort.LastName, SortOrder.Ascending, type => type.UserType, type => type.Vendor, type => type.Office) :
                    repo.All(sort => sort.LastName, SortOrder.Ascending, type => type.UserType, type => type.Vendor, type => type.Office);
                //ctx.SaveChanges();
            }
            return users;
        }
        // stephen return users only

        public static List<spGetUsers_Result> GetUsersOnly(bool? IsActive, string queryType, int Id)
        {
            List<spGetUsers_Result> result = null;
            using (CustomSparkPortalDataEntities ctx = new CustomSparkPortalDataEntities())
            {
                result = ctx.spGetUsers(queryType: queryType, id: Id, isActive: IsActive).ToList();
            }
            return result;
        }

        // stephen
        public static List<GetUtilityPrograms_Result> GetUtilityPrograms(int vendor, int office, string state, string zip, bool? creditcheck, int? premisetype)

        {
            List<GetUtilityPrograms_Result> result = null;
            using (CustomSparkPortalDataEntities ctx = new CustomSparkPortalDataEntities())
            {
                result = ctx.GetUtilityPrograms(userVendorId: vendor, userOfficeId: office, state: state, zip: zip,creditcheck: creditcheck, premisetype: premisetype).ToList();
               
            }
            return result;
        }

        public static List<Lead> GetLeadsByZip(string vendorNumber, string zipcode)
        {
            List<Lead> leads;
            using (CustomSparkPortalDataEntities ctx = new CustomSparkPortalDataEntities())
            {
                LeadRepository repo = new LeadRepository(ctx);
                leads = repo.Filter(x => x.VendorNumber == vendorNumber && x.Zip == zipcode);
            }
            return leads;
        }

        public static UserType GetUserType(int id)
        {
            UserType ut;
            using (CustomSparkPortalDataEntities ctx = new CustomSparkPortalDataEntities())
            {
                UserTypeRepository repo = new UserTypeRepository(ctx);
                ut = repo.Find(id);
            }
            return ut;
        }
        public static Vendor GetVendor(int id)
        {
            Vendor vendor;
            using (CustomSparkPortalDataEntities ctx = new CustomSparkPortalDataEntities())
            {
                VendorRepository repo = new VendorRepository(ctx);
                vendor = repo.Find(x => x.VendorId == id, type => type.Offices);
            }
            return vendor;
        }

        public static List<Vendor> GetVendors(bool activeOnly)
        {
            List<Vendor> vendors;
            using (CustomSparkPortalDataEntities ctx = new CustomSparkPortalDataEntities())
            {
                VendorRepository repo = new VendorRepository(ctx);
                vendors = activeOnly ? repo.Filter(v => v.IsActive, sort => sort.VendorName, SortOrder.Ascending, o => o.Offices.Select(x => x.SalesChannel)) : repo.All(o => o.Offices.Select(x => x.SalesChannel));
            }
            return vendors;
        }

        public static List<User> GetVendorUsers(int vendorId, bool activeOnly)
        {
            List<User> users;
            using (CustomSparkPortalDataEntities ctx = new CustomSparkPortalDataEntities())
            {
                UserRepository repo = new UserRepository(ctx);
                users = activeOnly
                    ? repo.Filter(u => u.VendorId == vendorId && u.IsActive, sort => sort.LastName, SortOrder.Ascending, type => type.UserType, type => type.Vendor, type => type.Office)
                    : repo.Filter(u => u.VendorId == vendorId, sort => sort.LastName, SortOrder.Ascending, type => type.UserType, type => type.Vendor, type => type.Office);
                //ctx.SaveChanges();
            }
            return users;
        }

        public static List<Report> GetReports(int securityLevel)
        {
            List<Report> reports;
            using (CustomSparkPortalDataEntities ctx = new CustomSparkPortalDataEntities())
            {
                ReportRepository repo = new ReportRepository(ctx);
                reports = repo.Filter(sec => sec.SecurityLevel <= securityLevel);
            }
            return reports;
        }

        public static int? UpdateOffice(Office office)
        {
            if (office == null)
            {
                throw new System.ArgumentNullException() { Source = "office" };
            }

            using (CustomSparkPortalDataEntities ctx = new CustomSparkPortalDataEntities())
            {
                OfficeRepository repo = new OfficeRepository(ctx);

                Office o = GetOffice(office.OfficeId);
                o.VendorId = office.VendorId;
                o.OfficeName = office.OfficeName;
                o.OfficeEmail = office.OfficeEmail;
                o.MarketerCode = office.MarketerCode;
                o.SalesChannelId = office.SalesChannelId;
                o.Address1 = office.Address1;
                o.Address2 = office.Address2;
                o.City = office.City;
                o.StateCode = office.StateCode;
                o.ZipCode = office.ZipCode;
                o.OfficeContact = office.OfficeContact;
                o.OfficePhone = office.OfficePhone;
                o.IsActive = office.IsActive;
                o.ModifiedBy = office.ModifiedBy;
                o.ModifiedDateTime = office.ModifiedDateTime;
                o.SalesChannel = null;

                repo.Update(o);
                ctx.SaveChanges();
            }

            return office.OfficeId;
        }

        public static int UpdateRate(Program program)
        {
            if (program == null)
            {
                throw new System.ArgumentNullException() { Source = "program" };
            }

            using (CustomSparkPortalDataEntities ctx = new CustomSparkPortalDataEntities())
            {
                using (DbContextTransaction trans = ctx.Database.BeginTransaction())
                {
                    RateRepository repo = new RateRepository(ctx);

                    Program oldRate = repo.Find(p => p.ProgramId == program.ProgramId, t => t.ProgramVendors, s => s.SalesChannelPrograms);

                    oldRate.ProgramId = program.ProgramId;
                    oldRate.ProgramCode = program.ProgramCode;
                    oldRate.ProgramName = program.ProgramName;
                    oldRate.ProgramDescription = program.ProgramDescription;
                    oldRate.DefaultPricingPlanDescription = program.DefaultPricingPlanDescription;
                    oldRate.EffectiveStartDate = program.EffectiveStartDate;
                    oldRate.EffectiveEndDate = program.EffectiveEndDate;
                    oldRate.Msf = program.Msf;
                    oldRate.Etf = program.Etf;
                    oldRate.Rate = program.Rate;
                    oldRate.PromotionalCode = program.PromotionalCode;
                    oldRate.UnitOfMeasureId = program.UnitOfMeasureId;
                    oldRate.Term = program.Term;
                    oldRate.UtilityTypeId = program.UtilityTypeId;
                    oldRate.PremiseTypeId = program.PremiseTypeId;
                    oldRate.State = program.State;
                    oldRate.UtilityId = program.UtilityId;
                    oldRate.AccountNumberTypeId = program.AccountNumberTypeId;
                    oldRate.AccountNumberLength = program.AccountNumberLength;
                    oldRate.AccountNumberFixedLength = program.AccountNumberFixedLength;
                    oldRate.MeterNumber = program.MeterNumber;
                    oldRate.MeterNumberLength = program.MeterNumberLength;
                    oldRate.ServiceReference = program.ServiceReference;
                    oldRate.RescindBy = program.RescindBy;
                    oldRate.Hefpa = program.Hefpa;
                    oldRate.Vendor = program.Vendor;
                    oldRate.Market = program.Market;
                    oldRate.SalesChannel = program.SalesChannel;
                    oldRate.RateVerbiage = program.RateVerbiage;
                    oldRate.CancellationVerbiage = program.CancellationVerbiage;
                    oldRate.RateVerbiageSpanish = program.RateVerbiageSpanish;
                    oldRate.CancellationVerbiageSpanish = program.CancellationVerbiageSpanish;
                    oldRate.BrandId = program.BrandId;
                    oldRate.UpdatedBy = program.UpdatedBy;
                    oldRate.UpdatedDateTime = program.UpdatedDateTime;
                    oldRate.CreditCheck = program.CreditCheck;

                    List<ProgramVendor> deleteList = new List<ProgramVendor>();
                    List<ProgramVendor> addList = new List<ProgramVendor>();

                    foreach (ProgramVendor cpv in oldRate.ProgramVendors)
                    {
                        if (!program.ProgramVendors.Any(x => x.ProgramId == cpv.ProgramId && x.VendorId == cpv.VendorId))
                        {
                            deleteList.Add(cpv);
                        }
                    }

                    foreach (ProgramVendor pv in program.ProgramVendors)
                    {
                        if (!oldRate.ProgramVendors.Any(x => x.ProgramId == pv.ProgramId && x.VendorId == pv.VendorId))
                        {
                            pv.ProgramId = program.ProgramId;
                            addList.Add(pv);
                        }
                    }

                    foreach (ProgramVendor d in deleteList)
                    {
                        oldRate.ProgramVendors.Remove(d);
                    }

                    foreach (ProgramVendor a in addList)
                    {
                        oldRate.ProgramVendors.Add(a);
                    }

                    // sales channels

                    List<SalesChannelProgram> scpDeleteList = new List<SalesChannelProgram>();
                    List<SalesChannelProgram> scpAddList = new List<SalesChannelProgram>();
                    foreach (SalesChannelProgram scp in oldRate.SalesChannelPrograms)
                    {
                        if (!program.SalesChannelPrograms.Any(x => x.ProgramId == scp.ProgramId && x.SalesChannelId == scp.SalesChannelId))
                        {
                            scpDeleteList.Add(scp);
                        }
                    }

                    foreach (SalesChannelProgram scp in program.SalesChannelPrograms)
                    {
                        if (!oldRate.SalesChannelPrograms.Any(x => x.ProgramId == scp.ProgramId && x.SalesChannelId == scp.SalesChannelId))
                        {
                            scp.ProgramId = program.ProgramId;
                            scpAddList.Add(scp);
                        }
                    }

                    foreach (SalesChannelProgram d in scpDeleteList)
                    {
                        oldRate.SalesChannelPrograms.Remove(d);
                    }

                    foreach (SalesChannelProgram a in scpAddList)
                    {
                        oldRate.SalesChannelPrograms.Add(a);
                    }

                    repo.Update(oldRate);

                    ctx.SaveChanges();
                    trans.Commit();
                }
            }

            return program.ProgramId;
        }

        public static int UpdateUser(User oldUser)
        {
            if (oldUser == null)
            {
                throw new System.ArgumentNullException() { Source = "user" };
            }

            using (CustomSparkPortalDataEntities ctx = new CustomSparkPortalDataEntities())
            {
                User newUser = GetUserForUpdate(oldUser.UserId);

                newUser.AgentId = oldUser.AgentId;
                newUser.Password = oldUser.Password;
                newUser.FirstName = oldUser.FirstName;
                newUser.LastName = oldUser.LastName;
                newUser.Language = oldUser.Language;
                newUser.Phone = oldUser.Phone;
                newUser.EmailAddress = oldUser.EmailAddress;
                newUser.VendorId = oldUser.VendorId;
                newUser.OfficeId = oldUser.OfficeId;
                newUser.Gender = oldUser.Gender;
                newUser.ShirtSize = oldUser.ShirtSize;
                newUser.City = oldUser.City;
                newUser.StateCode = oldUser.StateCode;
                newUser.BirthDate = oldUser.BirthDate;
                newUser.SSN = oldUser.SSN;               
                newUser.DrugTest = oldUser.DrugTest;
                newUser.BackgroundCheck = oldUser.BackgroundCheck;
                newUser.CodeOfConduct = oldUser.CodeOfConduct;
                newUser.IsActive = oldUser.IsActive;
                newUser.UserTypeId = oldUser.UserTypeId;
                newUser.ModifiedBy = oldUser.ModifiedBy;
                newUser.ModifiedDateTime = oldUser.ModifiedDateTime;

                UserRepository repo = new UserRepository(ctx);
                repo.Update(newUser);
                ctx.SaveChanges();
            }

            return oldUser.UserId;
        }

        public static int UpdateVendor(Vendor vendor)
        {
            if (vendor == null)
            {
                throw new System.ArgumentNullException() { Source = "vendor" };
            }

            using (CustomSparkPortalDataEntities ctx = new CustomSparkPortalDataEntities())
            {
                VendorRepository repo = new VendorRepository(ctx);

                Vendor v = GetVendor(vendor.VendorId);
                v.VendorId = vendor.VendorId;
                v.VendorName = vendor.VendorName;
                v.VendorNumber = vendor.VendorNumber;
                v.CommissionNumber = vendor.CommissionNumber;
                v.IsActive = vendor.IsActive;
                v.ModifiedBy = vendor.ModifiedBy;
                v.ModifiedDateTime = vendor.ModifiedDateTime;

                repo.Update(v);
                ctx.SaveChanges();
            }

            return vendor.VendorId;
        }

        public static IEnumerable<Disposition> GetDispositions(bool activeOnly)
        {
            List<Disposition> dispositions;
            using (CustomSparkPortalDataEntities ctx = new CustomSparkPortalDataEntities())
            {
                DispositionRepository repo = new DispositionRepository(ctx);
                dispositions = repo.All(sort => sort.DisplayOrder, SortOrder.Ascending);
            }
            return dispositions;
        }

        public static IEnumerable<Main> GetCalls(SearchContext searchContext)
        {

            List<Main> calls;
            using (CustomSparkPortalDataEntities ctx = new CustomSparkPortalDataEntities())
            {
                MainRepository repo = new MainRepository(ctx);
                calls = repo.GetCalls(searchContext);
            }
            return calls;

        }

        public static List<GetVerifiedChartSummary_Result> GetVerifiedChartData(DateTime sDate, int vendorId, int officeId)
        {
            List<GetVerifiedChartSummary_Result> verifiedChart;
            int? v = (vendorId == 0 ? new int?() : vendorId);
            int? o = (officeId == 0 ? new int?() : officeId);
            using (CustomSparkPortalDataEntities ctx = new CustomSparkPortalDataEntities())
            {
                verifiedChart = ctx.GetVerifiedChartSummary(sDate, v, o).ToList();
            }
            return verifiedChart;
        }

        public static List<GetVerifiedChartDetail_Result> GetVerifiedChartDetailData(DateTime sDate, int vendorId, int officeId)
        {
            List<GetVerifiedChartDetail_Result> verifiedChart;
            int? v = (vendorId == 0 ? new int?() : vendorId);
            int? o = (officeId == 0 ? new int?() : officeId);
            using (CustomSparkPortalDataEntities ctx = new CustomSparkPortalDataEntities())
            {
                verifiedChart = ctx.GetVerifiedChartDetail(sDate, v, o).ToList();
            }
            return verifiedChart;
        }

        public static List<GetVerifiedAccountsChartSummary_Result> GetVerifiedAccountsChartData(DateTime sDate, int vendorId, int officeId)
        {
            List<GetVerifiedAccountsChartSummary_Result> verifiedChart;
            int? v = (vendorId == 0 ? new int?() : vendorId);
            int? o = (officeId == 0 ? new int?() : officeId);
            using (CustomSparkPortalDataEntities ctx = new CustomSparkPortalDataEntities())
            {
                verifiedChart = ctx.GetVerifiedAccountsChartSummary(sDate, v, o).ToList();
            }
            return verifiedChart;
        }

        public static List<GetVerifiedAccountsChartDetail_Result> GetVerifiedAccountsChartDetailData(DateTime sDate, int vendorId, int officeId)
        {
            List<GetVerifiedAccountsChartDetail_Result> verifiedChart;
            int? v = (vendorId == 0 ? new int?() : vendorId);
            int? o = (officeId == 0 ? new int?() : officeId);
            using (CustomSparkPortalDataEntities ctx = new CustomSparkPortalDataEntities())
            {
                verifiedChart = ctx.GetVerifiedAccountsChartDetail(sDate, v, o).ToList();
            }
            return verifiedChart;
        }

        public static List<GetTopVendors_Result> GetTopVendorsData(DateTime sDate)
        {
            List<GetTopVendors_Result> verifiedChart;
            using (CustomSparkPortalDataEntities ctx = new CustomSparkPortalDataEntities())
            {
                verifiedChart = ctx.GetTopVendors(sDate).ToList();
            }
            return verifiedChart;
        }

        public static List<GetTopVendorStats_Result> GetTopVendorsStats(DateTime sDate, int vendorId)
        {
            List<GetTopVendorStats_Result> data;
            int? v = (vendorId == 0 ? new int?() : vendorId);
            using (CustomSparkPortalDataEntities ctx = new CustomSparkPortalDataEntities())
            {
                data = ctx.GetTopVendorStats(sDate, v).OrderByDescending(o => o.VerifiedAccounts).ThenBy(t => t.VendorName).Take(5).ToList();
            }
            return data;
        }

        public static List<GetTopOfficeStats_Result> GetTopOfficeStats(DateTime sDate, int vendorId, int officeId)
        {
            List<GetTopOfficeStats_Result> data;
            int? v = (vendorId == 0 ? new int?() : vendorId);
            int? o = (officeId == 0 ? new int?() : officeId);
            using (CustomSparkPortalDataEntities ctx = new CustomSparkPortalDataEntities())
            {
                data = ctx.GetTopOfficeStats(sDate, v, o).OrderByDescending(y => y.VerifiedAccounts).ThenBy(t => t.OfficeName).Take(10).ToList();
            }
            return data;
        }

        public static List<GetTopUserStats_Result> GetTopUsersStats(DateTime sDate, int vendorId, int officeId)
        {
            List<GetTopUserStats_Result> data;
            int? v = (vendorId == 0 ? new int?() : vendorId);
            int? o = (officeId == 0 ? new int?() : officeId);
            using (CustomSparkPortalDataEntities ctx = new CustomSparkPortalDataEntities())
            {
                data = ctx.GetTopUserStats(sDate, v, o)
                    .Where(x => x.VerifiedAccounts > 0)
                    .OrderByDescending(y => y.VerifiedAccounts)
                    .ThenBy(y => y.SparkId)
                    .Take(25)
                    .ToList();
            }
            return data;
        }

        public static bool AddAgentTrack(DataAccess.Entities.TrackAgent track)
        {
            DtDAgentTrack newTrack = new DtDAgentTrack();
            bool savetrack = true;
            newTrack.AgentId = track.AgentId;
            newTrack.Geolocation = DbGeography.FromText($"Point({track.Geolocation.lng} {track.Geolocation.lat})");
            try
            {
                using (CustomSparkPortalDataEntities ctx = new CustomSparkPortalDataEntities())
                {
                    DtsAgentTrackRepository repo = new DtsAgentTrackRepository(ctx);
                    repo.Create(newTrack);
                    ctx.SaveChanges();

                }

            }
            catch (Exception ex)
            {
                savetrack = false;
            }


            return savetrack;


        }



    }
}