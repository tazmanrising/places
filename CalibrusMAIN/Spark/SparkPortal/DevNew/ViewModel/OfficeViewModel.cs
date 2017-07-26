using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using Calibrus.SparkPortal.DataAccess.Infrastructure;
using Calibrus.SparkPortal.Business;

namespace Calibrus.SparkPortal.ViewModel
{
	public class OfficeViewModel
	{
		public OfficeViewModel()
		{
			UserList = new List<OfficeUser>();
            SalesChannelList = AppLogic.GetActiveSalesChannels();
            SetDropdowns();
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
            SalesChannelList = AppLogic.GetActiveSalesChannels();
            SetDropdowns();
        }

		public OfficeViewModel(int id)
		{
			Office office = Business.AppLogic.GetOffice(id);
			Id = office.OfficeId;
			OfficeName = office.OfficeName;
			OfficeEmail = office.OfficeEmail;
            MarketerCode = office.MarketerCode;
            SalesChannelId = office.SalesChannelId;
            Address1 = office.Address1;
            Address2 = office.Address2;
            City = office.City;
            State = office.StateCode;
            ZipCode = office.ZipCode;
            ContactName = office.OfficeContact;
            ContactPhone = office.OfficePhone;
            SalesChannelList = AppLogic.GetActiveSalesChannels();
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
            SetDropdowns();
		}

        public void SetDropdowns()
        {
            States = GetStates();
        }
        public int? Id { get; set; }

		[Required]
		[DisplayName("Office Name")]
		public string OfficeName { get; set; }

        [DisplayName("Address1")]
        public string Address1 { get; set; }

        [DisplayName("Address2")]
        public string Address2 { get; set; }

        [DisplayName("City")]
        public string City { get; set; }

        [Display(Name = "State")]
        public string State { get; set; }

        [DisplayName("Zip Code")]
       
        [RegularExpression(@"\d{5}", ErrorMessage = "Zip Code 5Digits Only")]
        public string ZipCode { get; set; }


        public List<State> States { get; private set; }

        [Required]
		[DataType(DataType.EmailAddress)]
		[DisplayName("Office Email")]
		public string OfficeEmail { get; set; }

        [Required]
        [DisplayName("Marketer Code")]
        public string MarketerCode { get; set; }

        [Required]
        [DisplayName("Sales Channel")]
        public int SalesChannelId { get; set; }

        [DisplayName("Contact Name")]
        public string ContactName { get; set; }

        [DisplayName("Contact Phone")]
        [Phone]
        [DataType(DataType.PhoneNumber)]
        public string ContactPhone { get; set; }

        [Display(Name = "Contact Phone")]
        public string ContactPhoneFormatted
        {
            get { return Regex.Replace(ContactPhone, @"(\d{3})(\d{3})(\d{4})", "($1) $2-$3"); }
        }



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

        public List<SalesChannel> SalesChannelList { get; set; }

        private List<State> GetStates()
        {
            return Business.AppLogic.GetStates();
        }

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
                    SalesChannelId = this.SalesChannelId,
                    Address1 = this.Address1,
                    Address2 = this.Address2,
                    City = this.City,
                    StateCode = this.State,
                    ZipCode = this.ZipCode,
                    OfficeContact = this.ContactName,
                    OfficePhone = this.ContactPhone,
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
                    SalesChannelId = this.SalesChannelId,
                    Address1 = this.Address1,
                    Address2 = this.Address2,
                    City = this.City,
                    StateCode = this.State,
                    ZipCode = this.ZipCode,
                    OfficeContact = this.ContactName,
                    OfficePhone = this.ContactPhone,
                    VendorId = this.VendorId.Value,
					IsActive = true,
					CreatedBy = LoggedOnUser,
					CreatedDateTime = DateTime.Now
				});
			}
		}
	}
}