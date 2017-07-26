using System.Linq;
using Calibrus.ClearviewPortal.Business;
using Calibrus.ClearviewPortal.DataAccess.Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Calibrus.ClearviewPortal.ViewModel
{
	public class UserViewModel : IValidatableObject
	{
		public UserViewModel(int id, string loggedInUser)
		{
		

			User user = Business.AppLogic.GetUser(id);

			Id = user.UserId;
			Username = user.AgentId;
			Password = user.Password;
			FirstName = user.FirstName;
			LastName = user.LastName;
			Language = user.Language;
			PhoneNumber = user.Phone;
            BirthDate = user.BirthDate;
            Ssn4 = user.Ssn4;
            BackgroundCheck = user.BackgroundCheck.GetValueOrDefault(false);
            CodeOfConduct = user.CodeOfConduct.GetValueOrDefault(false);
            EmailAddress = user.EmailAddress;
			IsActive = user.IsActive;
			UserTypeId = user.UserTypeId;

			OfficeId = user.OfficeId;
			ParentOfficeFormatted = user.OfficeId.HasValue ? String.Format("{0}", user.Office.OfficeName) : "";

			VendorId = user.VendorId;
			ParentVendorFormatted = user.VendorId.HasValue ? String.Format("{0}", user.Vendor.VendorName) : "";

			Offices = VendorId.HasValue ? Business.AppLogic.GetOffices(true, VendorId.Value) : Business.AppLogic.GetOffices(true);
			Vendors = Business.AppLogic.GetVendors(true);

			User loggedonUser = Business.AppLogic.GetUser(loggedInUser);
			UserTypes = Business.AppLogic.GetActiveUserTypes(loggedonUser.UserType.SecurityLevel);
		}

		public UserViewModel(int? vendorId, int? officeId, string loggedInUser)
		{
			User user = Business.AppLogic.GetUser(loggedInUser);

			ReferringVendorId = vendorId;

			if (vendorId.HasValue && vendorId > 0)
			{
				Vendor v = AppLogic.GetVendor(vendorId.Value);
				VendorId = v.VendorId;
				ParentVendorFormatted = v.VendorName;
			}
			else
			{
				Vendors = AppLogic.GetVendors(true);
			}

			if (officeId.HasValue && officeId > 0)
			{
				Office o = AppLogic.GetOffice(officeId.Value);
				OfficeId = o.OfficeId;
				ParentOfficeFormatted = o.OfficeName;
			}
			else
			{
				Offices = vendorId.HasValue ? Business.AppLogic.GetOffices(true, vendorId.Value) : Business.AppLogic.GetOffices(true);
			}

			UserTypes = Business.AppLogic.GetActiveUserTypes(user.UserType.SecurityLevel);
		}

		public UserViewModel(UserViewModel model)
		{
			Id = model.Id;
			Username = model.Username;
			Password = model.Password;
			FirstName = model.FirstName;
			LastName = model.LastName;
			Language = model.Language;
			PhoneNumber = model.PhoneNumber;
			EmailAddress = model.EmailAddress;
            BirthDate = model.BirthDate;
            Ssn4 = model.Ssn4;
            BackgroundCheck = model.BackgroundCheck;
            CodeOfConduct = model.CodeOfConduct;
			IsActive = model.IsActive;
			OfficeId = model.OfficeId;
			VendorId = model.VendorId;
			ParentVendorFormatted = model.ParentVendorFormatted;
			ParentOfficeFormatted = model.ParentOfficeFormatted;
			UserTypeId = model.UserTypeId;
			ReferringVendorId = model.ReferringVendorId;

			User user = Business.AppLogic.GetUser(model.LoggedOnUser);
			Vendors = !model.ReferringVendorId.HasValue ? Business.AppLogic.GetVendors(true) : null;
			Offices = model.ReferringVendorId.HasValue ? Business.AppLogic.GetOffices(true, model.ReferringVendorId.Value) : Business.AppLogic.GetOffices(true);
			UserTypes = Business.AppLogic.GetActiveUserTypes(user.UserType.SecurityLevel);
		}

		public UserViewModel()
		{
		}

		public int? ReferringVendorId { get; set; }

		[Display(Name = "Email Address")]
		[DataType(DataType.EmailAddress)]
		[EmailAddress]
		[MaxLength(50, ErrorMessage="Email cannot be more than 50 characters")]
		public string EmailAddress { get; set; }

		[Required]
		[MaxLength(50, ErrorMessage = "First Name cannot be more than 50 characters")]
		[Display(Name = "First Name")]
		public string FirstName { get; set; }

		public int? Id { get; set; }

		[Display(Name = "Active")]
		public bool IsActive { get; set; }

		public string Language { get; set; }

		public List<string> Languages
		{
			get
			{
				return new List<string> { "English", "Spanish", "Bilingual" };
			}
		}

		[Required]
		[MaxLength(50, ErrorMessage = "Last Name cannot be more than 50 characters")]
		[Display(Name = "Last Name")]
		public string LastName { get; set; }

		public string LoggedOnUser { get; set; }

		[Display(Name = "Office")]
		public int? OfficeId { get; set; }

		public List<Office> Offices { get; set; }

		[Display(Name = "Office")]
		public string ParentOfficeFormatted { get; set; }

		public string Password { get; set; }

		[Display(Name = "Phone Number")]
		[Phone]
		[DataType(DataType.PhoneNumber)]
		public string PhoneNumber { get; set; }

		[Display(Name = "Phone Number")]
		public string PhoneNumberFormatted
		{
			get { return Regex.Replace(PhoneNumber, @"(\d{3})(\d{3})(\d{4})", "($1) $2-$3"); }
		}

		[Required]
		[Display(Name = "Role")]
		public int RoleId { get; set; }

		[Required]
		[MaxLength(50, ErrorMessage = "Username/Agent ID cannot be more than 50 characters")]
		public string Username { get; set; }

		[Required]
		[Display(Name = "User Type")]
		public int UserTypeId { get; set; }
		
		public List<DataAccess.Infrastructure.UserType> UserTypes { get; private set; }

		[Display(Name = "Vendor")]
		public int? VendorId { get; set; }

		public List<Vendor> Vendors { get; private set; }

		[Display(Name = "Vendor")]
		public string ParentVendorFormatted { get; set; }

        [Display(Name = "Birth Date")]
        public DateTime? BirthDate { get; set; }

        [Display(Name = "SSN4")]
        [RegularExpression(@"^(\d){4}$", ErrorMessage = "SSN4 must be 4 digits")]
        public string Ssn4 { get; set; }

        [Display(Name = "Background Check")]
        public bool BackgroundCheck { get; set; }

        [Display(Name = "Code of Conduct")]
        public bool CodeOfConduct { get; set; }

        public void SaveViewModel()
		{
			//TODO: make sure loggedInUser is a member to the vendor the user id being created/edited in

			User oldUser = new User
			{
				AgentId = this.Username,
				Password = this.Password,
				FirstName = this.FirstName,
				LastName = this.LastName,
				Language = this.Language,
				Phone = this.PhoneNumber,
                BirthDate = this.BirthDate,
                Ssn4 = this.Ssn4,
                BackgroundCheck = this.BackgroundCheck,
                CodeOfConduct = this.CodeOfConduct,
				EmailAddress = this.EmailAddress,
				IsActive = this.IsActive,
				UserTypeId = this.UserTypeId,
				VendorId = this.VendorId == 0 ? null : this.VendorId,
				OfficeId = this.OfficeId == 0 ? null : this.OfficeId

			};

			if (Id.HasValue && Id > 0) //edit
			{
				oldUser.UserId = Id.Value;
				oldUser.ModifiedBy = LoggedOnUser;
				oldUser.ModifiedDateTime = DateTime.Now;
				
				Id = Business.AppLogic.UpdateUser(oldUser);
			}
			else //new
			{
				oldUser.CreatedBy = LoggedOnUser;
				oldUser.CreatedDateTime = DateTime.Now;

				Id = Business.AppLogic.CreateUser(oldUser);
			}
		}

		public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
		{
			DataAccess.Infrastructure.UserType userType = Business.AppLogic.GetUserType(this.UserTypeId);

			User oldUser = null;
			if (this.Id.HasValue)
			{
				oldUser = Business.AppLogic.GetUser(this.Id.Value);
			}

			switch (userType.UserTypeName)
			{
				case "Client Administrator":
					if (String.IsNullOrWhiteSpace(this.Password))
						yield return new ValidationResult("Password is required.", new[] { "Password" });
					if (String.IsNullOrWhiteSpace(this.EmailAddress))
						yield return new ValidationResult("Email Address is required.", new[] { "EmailAddress" });
					break;

                case "QA Administrator":
                    if (String.IsNullOrWhiteSpace(this.Password))
                        yield return new ValidationResult("Password is required.", new[] { "Password" });
                    if (String.IsNullOrWhiteSpace(this.EmailAddress))
                        yield return new ValidationResult("Email Address is required.", new[] { "EmailAddress" });
                    break;

                case "Vendor Administrator":
					if (String.IsNullOrWhiteSpace(this.Password))
						yield return new ValidationResult("Password is required.", new[] { "Password" });
					if (String.IsNullOrWhiteSpace(this.EmailAddress))
						yield return new ValidationResult("Email Address is required.", new[] { "EmailAddress" });
					if (VendorId == 0)
						yield return new ValidationResult("Vendor is required.", new[] { "VendorId" });
					break;

				case "Office Administrator":
					if (String.IsNullOrWhiteSpace(this.Password))
						yield return new ValidationResult("Password is required.", new[] { "Password" });
					if (String.IsNullOrWhiteSpace(this.EmailAddress))
						yield return new ValidationResult("Email Address is required.", new[] { "EmailAddress" });
					if (VendorId == 0)
						yield return new ValidationResult("Vendor is required.", new[] { "VendorId" });
					if (OfficeId == 0)
						yield return new ValidationResult("Office is required.", new[] { "OfficeId" });
					break;
				case "Door to Door":
					if (String.IsNullOrWhiteSpace(this.PhoneNumber))
						yield return new ValidationResult("Phone Number is required.", new[] { "PhoneNumber" });
					if (this.OfficeId ==0)
						yield return new ValidationResult("Office is required.", new[] { "OfficeId" });
					break;

				case "Telesales":
                    if (String.IsNullOrWhiteSpace(this.PhoneNumber))
                        yield return new ValidationResult("Phone Number is required.", new[] { "PhoneNumber" });
                    if (this.OfficeId == 0)
						yield return new ValidationResult("Office is required.", new[] { "OfficeId" });
					break;

				default:
					yield return new ValidationResult("Invalid User Type");
					break;
			}

			if ((oldUser == null && AppLogic.GetUser(Username, VendorId) != null) || (oldUser != null && oldUser.AgentId != this.Username && AppLogic.GetUser(Username, VendorId) != null))
			{
				yield return new ValidationResult("Username is already assigned.", new[] { "Username" });
			}
		}
	}
}