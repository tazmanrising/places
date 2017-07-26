using System;
using Calibrus.SparkPortal.DataAccess.Infrastructure;
using Calibrus.SparkPortal.Web.CustomAttributes;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Calibrus.SparkPortal.ViewModel;

namespace Calibrus.SparkPortal.Web.Controllers
{
	[CustomApiAccessAuthorize]
	public class OfficesController : ApiController
	{
		[Route("api/offices/{active:bool}")]
		public IEnumerable<OfficeItem> GetOffices(bool active)
		{
			List<Office> offices = Business.AppLogic.GetOffices(active);
			List<OfficeItem> oi = new List<OfficeItem>();
			offices.ForEach(o =>
						oi.Add(new OfficeItem()
						{
							Id = o.OfficeId,
							OfficeName = o.OfficeName,
							OfficeEmail = o.OfficeEmail,
                            MarketerCode = o.MarketerCode,
                            SalesChannel = o.SalesChannel.Name,
							VendorName = o.Vendor.VendorName,
							VendorNumber = o.Vendor.VendorNumber,
							IsActive = o.IsActive,
							TotalUsers = o.Users.Count()
						}
					)
			);

			return oi;
		}

		[Route("api/offices/{vendorId:int}/{active:bool}")]
		public IEnumerable<OfficeItem> GetOffices(int vendorId, bool active)
		{
			List<Office> offices = Business.AppLogic.GetOffices(active, vendorId);
			List<OfficeItem> oi = new List<OfficeItem>();
			offices.ForEach(o =>
						oi.Add(new OfficeItem()
						{
							Id = o.OfficeId,
							OfficeName = o.OfficeName,
							OfficeEmail = o.OfficeEmail,
                            MarketerCode = o.MarketerCode,
                            SalesChannel = o.SalesChannel.Name,
                            VendorName = o.Vendor.VendorName,
							VendorNumber = o.Vendor.VendorNumber,
							IsActive = o.IsActive,
							TotalUsers = o.Users.Count()
						}
					)
			);

			return oi;
		}

		[HttpPost]
		public void Post(OfficeStatus os)
		{
			Office office = Business.AppLogic.GetOffice(os.OfficeId);
			office.IsActive = !office.IsActive;
			office.ModifiedBy = os.LoggedInUser;
			office.ModifiedDateTime = DateTime.Now;
			Business.AppLogic.UpdateOffice(office);
		}

		public class OfficeStatus
		{
			public int OfficeId { get; set; }
			public string LoggedInUser { get; set; }
		}

		public class OfficeItem
		{
			public int Id { get; set; }

			public string VendorName { get; set; }

			public string OfficeName { get; set; }

			public string OfficeEmail { get; set; }

            public string MarketerCode { get; set; }

            public string SalesChannel { get; set; }

            public string VendorNumber { get; set; }

			public bool IsActive { get; set; }

			public int TotalUsers { get; set; }
		}
	}
}