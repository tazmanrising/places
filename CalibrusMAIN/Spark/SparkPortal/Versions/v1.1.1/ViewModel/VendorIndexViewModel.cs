using Calibrus.SparkPortal.DataAccess.Infrastructure;
using System.Collections.Generic;
using System.Linq;

namespace Calibrus.SparkPortal.ViewModel
{
    public class VendorIndexViewModel
    {
        public VendorIndexViewModel()
        {
            List<Vendor> vendors = Business.AppLogic.GetVendors(false);
            Vendors = new List<VendorItem>();
            vendors.ForEach(v =>
                        Vendors.Add(new VendorItem()
                        {
                            Id = v.VendorId,
                            VendorName = v.VendorName,
                            VendorNumber = v.VendorNumber,
                            IsActive = v.IsActive,
                            TotalOffices = v.Offices.Count()
                        }
                    )
            );
        }

        public List<VendorItem> Vendors { get; set; }

        public class VendorItem
        {
            public int Id { get; set; }

            public string VendorName { get; set; }

            public string VendorNumber { get; set; }			

            public bool IsActive { get; set; }

            public int TotalOffices { get; set; }
        }
    }
}