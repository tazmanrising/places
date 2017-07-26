using System;
using Calibrus.ClearviewPortal.DataAccess.Infrastructure;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Calibrus.ClearviewPortal.Web.CustomAttributes;

namespace Calibrus.ClearviewPortal.Web.Controllers
{
	[CustomApiAccessAuthorize]
    public class VendorsController : ApiController
    {
        [Route("api/vendors/{active:bool}")]
        public IEnumerable<VendorItem> GetVendors(bool active)
        {
            List<Vendor> vendors = Business.AppLogic.GetVendors(active);
            List<VendorItem> vi = new List<VendorItem>();
            vendors.ForEach(v =>
                        vi.Add(new VendorItem()
                        {
                            Id = v.VendorId,
                            VendorName = v.VendorName,
                            SalesChannel = v.SalesChannel.Name,
                            VendorNumber = v.VendorNumber,
                            IsActive = v.IsActive,
                            TotalOffices = v.Offices.Count()
                        }
                    )
            );

            return vi;
        }

		[HttpPost]
		public void Post(VendorStatus v)
		{
			Vendor vendor = Business.AppLogic.GetVendor(v.VendorId);
			vendor.IsActive = !vendor.IsActive;
			vendor.ModifiedBy = v.LoggedInUser;
			vendor.ModifiedDateTime = DateTime.Now;
			Business.AppLogic.UpdateVendor(vendor);
		}

		public class VendorStatus
		{
			public int VendorId { get; set; }
			public string LoggedInUser { get; set; }
		}

        public class VendorItem
        {
            public int Id { get; set; }

            public string VendorName { get; set; }

            public string VendorNumber { get; set; }

			public string MarketerCode { get; set; }

			public string SalesChannel { get; set; }

            public bool IsActive { get; set; }

            public int TotalOffices { get; set; }
        }
    }
}