using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using Calibrus.ClearviewPortal.DataAccess.Infrastructure;
using Calibrus.ClearviewPortal.Business;

namespace Calibrus.ClearviewPortal.ViewModel
{
	public class OfficeViewModel
	{
		public OfficeViewModel()
		{
			UserList = new List<OfficeUser>();
            
        }

		public OfficeViewModel(int? vendorId, string loggedInUser)
		{
			if (vendorId.HasValue)
			{
				Vendor vendor = Business.AppLogic.GetVendor(vendorId.Value);
				ParentVendorFormatted = String.Format("{0} ({1})", vendor.VendorName, vendor.VendorNumber);
			}
			
			VendorId = vendorId;
			LoggedOnUser = loggedInUser;
			Vendors = Business.AppLogic.GetVendors(true);
        }

		public OfficeViewModel(int id)
		{
			Office office = Business.AppLogic.GetOffice(id);
			Id = office.OfficeId;
			OfficeName = office.OfficeName;
			OfficeEmail = office.OfficeEmail;
            MarketerCode = office.MarketerCode;
            ParentVendorFormatted = String.Format("{0}", office.Vendor.VendorName);
			UserList = new List<OfficeUser>();
			office.Users.ToList().ForEach(user =>
				UserList.Add(new OfficeUser()
				{
					Id = user.UserId,
					Username = user.AgentId,
					FirstName = user.FirstName,
					LastName = user.LastName,
					Language = user.Language,
					Phone = user.Phone,
					IsActive = user.IsActive
				}
					)
			);
			IsActive = office.IsActive;
			VendorId = office.VendorId;
		}

		public int? Id { get; set; }

		[Required]
		[DisplayName("Office Name")]
		public string OfficeName { get; set; }

		[Required]
		[DataType(DataType.EmailAddress)]
		[DisplayName("Office Email")]
		public string OfficeEmail { get; set; }

        [Required]
        [DisplayName("Marketer Code")]
        public string MarketerCode { get; set; }

        

        [DisplayName("Active")]
		public bool IsActive { get; set; }

		[Display(Name = "Vendor")]
		[Required]
		public int? VendorId { get; set; }
	

		[Display(Name = "Vendor")]
		public string ParentVendorFormatted { get; private set; }

		public List<Vendor> Vendors { get; private set; }

		public string LoggedOnUser { get; set; }

		public List<OfficeUser> UserList { get; set; }

        

        public class OfficeUser
		{
			public int Id { get; set; }

			public string Username { get; set; }

			public string FirstName { get; set; }

			public string LastName { get; set; }

			public bool IsOfficeAdmin { get; set; }

			public bool IsActive { get; set; }

			public string Language { get; set; }

			public string Phone { get; set; }
		}

		public void SaveViewModel()
		{
			if (Id.HasValue) //edit
			{
				Id = Business.AppLogic.UpdateOffice(new Office()
				{
					OfficeId = this.Id.Value,
					OfficeName = this.OfficeName,
					OfficeEmail = this.OfficeEmail,
                    MarketerCode = this.MarketerCode,
					VendorId = this.VendorId.Value,
					IsActive = this.IsActive,
					ModifiedBy = LoggedOnUser,
					ModifiedDateTime = DateTime.Now
				});
			}
			else //new
			{
				Id = Business.AppLogic.CreateOffice(new Office()
				{
					OfficeName = this.OfficeName,
					OfficeEmail = this.OfficeEmail,
                    MarketerCode = this.MarketerCode,                    
                    VendorId = this.VendorId.Value,
					IsActive = true,
					CreatedBy = LoggedOnUser,
					CreatedDateTime = DateTime.Now
				});
			}
		}
	}
}