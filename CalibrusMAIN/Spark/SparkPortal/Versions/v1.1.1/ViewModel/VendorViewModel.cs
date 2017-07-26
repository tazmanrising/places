using Calibrus.SparkPortal.Business;
using Calibrus.SparkPortal.DataAccess.Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Calibrus.SparkPortal.ViewModel
{
	public class VendorViewModel
	{
		public VendorViewModel()
		{
			OfficeList = new List<VendorOffice>();

		}

		public VendorViewModel(int id)
		{
			Vendor vendor = Business.AppLogic.GetVendor(id);
			Id = vendor.VendorId;
			VendorName = vendor.VendorName;
			VendorNumber = vendor.VendorNumber;
            CommissionNumber = vendor.CommissionNumber;
			OfficeList = new List<VendorOffice>();
			vendor.Offices.ToList().ForEach(office =>
				OfficeList.Add(new VendorOffice()
				{
					Id = office.OfficeId,
					OfficeName = office.OfficeName,
					OfficeEmail = office.OfficeEmail,
					IsActive = office.IsActive,
					Users = office.Users.Select(user => new VendorOffice.OfficeUser
					{
						Id = user.UserId,
						UserName = user.AgentId,
						IsActive = user.IsActive
					}
						).Where(x => x.IsActive).ToList()
				}
					)
				);
			IsActive = vendor.IsActive;
		}

		public int? Id { get; set; }

		[Required]
		[DisplayName("Vendor Name")]
		public string VendorName { get; set; }

		[Required]
		[DisplayName("Vendor Number")]
		public string VendorNumber { get; set; }		

        [DisplayName("Commission Number")]
        public string CommissionNumber { get; set; }

        [DisplayName("Active")]
		public bool IsActive { get; set; }

		public string LoggedOnUser { get; set; }

		public List<VendorOffice> OfficeList { get; set; }		

		public class VendorOffice
		{
			public int Id { get; set; }

			public string OfficeName { get; set; }

			public string OfficeEmail { get; set; }

			public bool IsActive { get; set; }

			public List<OfficeUser> Users { get; set; }

			public class OfficeUser
			{
				public int Id { get; set; }

				public string UserName { get; set; }

				public bool IsActive { get; set; }
			}
		}

		public void SaveViewModel()
		{
			if (Id.HasValue) //edit
			{
				Id = Business.AppLogic.UpdateVendor(new Vendor()
				{
					VendorId = this.Id.Value,
					VendorName = this.VendorName,
					VendorNumber = this.VendorNumber,
                    CommissionNumber = this.CommissionNumber,
					IsActive = this.IsActive,
					ModifiedBy = LoggedOnUser,
					ModifiedDateTime = DateTime.Now
				});
			}
			else //new
			{
				Id = Business.AppLogic.CreateVendor(new Vendor()
				{
					VendorName = this.VendorName,
					VendorNumber = this.VendorNumber,
                    CommissionNumber = this.CommissionNumber,
					IsActive = true,
					CreatedBy = LoggedOnUser,
					CreatedDateTime = DateTime.Now
				});
			}
		}
	}
}