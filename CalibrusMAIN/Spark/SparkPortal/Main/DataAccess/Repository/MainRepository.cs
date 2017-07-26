using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Calibrus.SparkPortal.DataAccess.Infrastructure;
using Calibrus.SparkPortal.DataAccess.Models;

namespace Calibrus.SparkPortal.DataAccess.Repository
{
	public class MainRepository : Repository<Main>
	{
		public MainRepository(SparkPortalDataEntities ctx) : base(ctx)
		{
		}

		public List<Main> GetCalls(SearchContext search)
		{
			IQueryable<Main> result = DbSet.AsQueryable();
			

			if (search.RecordId.GetValueOrDefault(0) > 0)
			{
				result = result.Where(x => x.MainId == search.RecordId)
					.Include(type => type.OrderDetails)
					.Include(type => type.OrderDetails.Select(p => p.Program))
					.Include(type => type.User)
					.Include(type => type.User.Vendor);

				foreach (Main call in result)
				{
					foreach (OrderDetail detail in call.OrderDetails)
					{
						detail.Main = null;
						if (detail.Program != null)
						{
							detail.Program.OrderDetails = null;
							detail.Program.PremiseType = null;
							detail.Program.ProgramVendors = null;
							detail.Program.UnitOfMeasure = null;
							detail.Program.Utility = null;
							detail.Program.UtilityType = null;
							detail.Program.Vendor = null;
						}
					}

					if (call.User != null)
					{
						call.User.Office = null;
						call.User.UserType = null;
						call.User.Mains = null;
						call.User.UserLogs = null;

						if (call.User.Vendor != null)
						{
							call.User.Vendor.Offices = null;
							call.User.Vendor.ProgramVendors = null;
							call.User.Vendor.Users = null;
						}

						if (call.User.Office != null)
						{
							call.User.Office.SalesChannel = null;
						}
					}

				}

				return result.ToList();
			}

			if (search.StartDate.HasValue && search.EndDate.HasValue)
			{
				result = result.Where(x => x.CallDateTime >= search.StartDate.Value && x.CallDateTime < search.EndDate.Value);
			}

			if (search.Disposition != null)
			{
				result = result.Where(x => x.Concern == search.Disposition.Concern);
			}

			if (!String.IsNullOrEmpty(search.TpvAgentId))
			{
				result = result.Where(x => x.TpvAgentId == search.TpvAgentId);
			}

			if (!String.IsNullOrEmpty(search.VendorAgentId))
			{
				result = result.Where(x => x.User.AgentId == search.VendorAgentId);
			}

			if (!String.IsNullOrEmpty(search.PhoneNumber))
			{
				result = result.Where(x => x.Btn == search.PhoneNumber);
			}

            if (search.AccountNumber != "")
            {
                    result = result.Where(x => x.OrderDetails.Any(d => d.AccountNumber == search.AccountNumber));
			}

			if (search.VendorId.HasValue)
			{
				result = result.Where(x => x.User.VendorId == search.VendorId.Value);
			}

			if (search.OfficeId.HasValue)
			{
				result = result.Where(x => x.User.OfficeId == search.OfficeId.Value);
			}

			result = result.Include(type => type.OrderDetails);
			result = result.Include(type => type.OrderDetails.Select(p => p.Program));
			result = result.Include(type => type.User);
			result = result.Include(type => type.User.Vendor);

			//TODO: Return custom object instead of model.
			foreach (Main call in result)
			{
				foreach (OrderDetail detail in call.OrderDetails)
				{
					detail.Main = null;
					if (detail.Program != null)
					{
						detail.Program.OrderDetails = null;
						detail.Program.PremiseType = null;
						detail.Program.ProgramVendors = null;
						detail.Program.UnitOfMeasure = null;
						detail.Program.Utility = null;
						detail.Program.UtilityType = null;
						detail.Program.Vendor = null;
					}
				}

				if (call.User != null)
				{
					call.User.Office = null;
					call.User.UserType = null;
					call.User.Mains = null;
					call.User.UserLogs = null;

					if (call.User.Vendor != null)
					{
						call.User.Vendor.Offices = null;
						call.User.Vendor.ProgramVendors = null;
						call.User.Vendor.Users = null;
					}

					if (call.User.Office != null)
					{
						call.User.Office.SalesChannel = null;
					}
				}

			}

			return result.Take(500).ToList();
		}

		
	}
}
