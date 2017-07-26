using System.Linq;
using Calibrus.SparkPortal.Business;
using Calibrus.SparkPortal.DataAccess.Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Calibrus.SparkPortal.ViewModel
{
	public class UserViewModel : IValidatableObject
	{
        public UserViewModel()
        {
            SetDropdowns();
        }
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
			EmailAddress = user.EmailAddress;
			IsActive = user.IsActive;
			UserTypeId = user.UserTypeId;
            // dtd 
            Gender = user.Gender;
            ShirtSize = user.ShirtSize;
            City = user.City;
            State = user.StateCode;
            // telesales
            SSN = user.SSN;
            BirthDate = user.BirthDate.GetValueOrDefault();
            Country = user.Country;
            UserLogs = user.UserLogs.ToList();

            SetDropdowns();
			OfficeId = user.OfficeId;
			ParentOfficeFormatted = user.OfficeId.HasValue ? String.Format("{0}", user.Office.OfficeName) : "";

			VendorId = user.VendorId;
			ParentVendorFormatted = user.VendorId.HasValue ? String.Format("{0}", user.Vendor.VendorName) : "";

            ParentUserTypeFormatted = UserTypeId > 0 ? String.Format("{0}", user.UserType.UserTypeName) : "";
            
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

            SetDropdowns();

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
			IsActive = model.IsActive;
			OfficeId = model.OfficeId;
			VendorId = model.VendorId;
			ParentVendorFormatted = model.ParentVendorFormatted;
			ParentOfficeFormatted = model.ParentOfficeFormatted;
            ParentUserTypeFormatted = model.ParentUserTypeFormatted;
			UserTypeId = model.UserTypeId;
			ReferringVendorId = model.ReferringVendorId;
            Gender = model.Gender;
            ShirtSize = model.ShirtSize;
            City = model.City;
            State = model.State;
            // telesales
            SSN = model.SSN;
            BirthDate = model.BirthDate;
            Country = model.Country;
            SetDropdowns();

			User user = Business.AppLogic.GetUser(model.LoggedOnUser);
			Vendors = !model.ReferringVendorId.HasValue ? Business.AppLogic.GetVendors(true) : null;
			Offices = model.ReferringVendorId.HasValue ? Business.AppLogic.GetOffices(true, model.ReferringVendorId.Value) : Business.AppLogic.GetOffices(true);
			UserTypes = Business.AppLogic.GetActiveUserTypes(user.UserType.SecurityLevel);
		}

        public void SetDropdowns()
        {
            States = GetStates();
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

        public List<State> States { get; private set; }
        
        [Display(Name = "Gender")]
        public string Gender { get; set; } 

       public List<UserLog> UserLogs { get; private set; }

        public List<string> Genders
        {
            get
            {
                return new List<string> { "Male", "Female" };
            }
        }

        [Display(Name = "Shirt Size")]       
        public string ShirtSize { get; set; }  

        public List<string> ShirtSizesMale
        {
            get
            {
                return new List<string> { "S", "M", "L", "XL", "2XL", "3XL", "4XL", "5XL" };
            }
        }

        public List<string> ShirtSizesFemale
        {
            get
            {
                return new List<string> {"XS", "S", "M", "L", "XL", "2XL", "3XL" };
            }
        }

        [Display(Name = "Mobile Phone Number")]
        [Phone]
        [DataType(DataType.PhoneNumber)]
        public string MobilePhoneNumber { get; set; }

        [Display(Name = "Mobile Phone Number")]
        public string MobilePhoneNumberFormatted
        {
            get { return Regex.Replace(MobilePhoneNumber, @"(\d{3})(\d{3})(\d{4})", "($1) $2-$3"); }
        }

        [Display(Name = "Last 4 SSN")]
        [RegularExpression(@"\d{4}",ErrorMessage ="4 digits required")]
        public string SSN { get; set; }

        [Display(Name = "BirthDate")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}",
               ApplyFormatInEditMode = false)]
        public DateTime BirthDate { get; set; }


        [Display(Name = "City")]
        public string City { get; set; }

        [Display(Name = "State")]
        public string State { get; set; }

        [Display(Name = "Country")]
        public string Country { get; set; }

        [Display(Name = "Date Shipped")]
        [DataType(DataType.DateTime)]
        public DateTime DateShipped { get; set; } 

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

        [Display(Name = "User Type")]
        public string ParentUserTypeFormatted { get; set; }

        private List<State> GetStates()
        {
            return Business.AppLogic.GetStates();
        }

       



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
                EmailAddress = this.EmailAddress,
                IsActive = this.IsActive,
                UserTypeId = this.UserTypeId,
                VendorId = this.VendorId == 0 ? null : this.VendorId,
                OfficeId = this.OfficeId == 0 ? null : this.OfficeId,
                Gender = this.Gender,
                ShirtSize = this.ShirtSize,
                City = this.City,
                StateCode = this.State,
                BirthDate = this.BirthDate,
                SSN = this.SSN   

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
                    if (this.VendorId == 0)
                        yield return new ValidationResult("Vendor is required.", new[] { "VendorId" });
                    if (this.Gender == "?")
                        yield return new ValidationResult("Gender is required.", new[] { "Gender" });
                    if (this.ShirtSize == "?")
                        yield return new ValidationResult("Shirt Size is required.", new[] { "ShirtSize" });
                    if (String.IsNullOrWhiteSpace(this.City))
                        yield return new ValidationResult("City is required.", new[] { "City" });
                    if (String.IsNullOrWhiteSpace(this.State))
                        yield return new ValidationResult("State is required.", new[] { "State" });

                    break;

				case "Telesales":
                    if (String.IsNullOrWhiteSpace(this.PhoneNumber))
                        yield return new ValidationResult("Phone Number is required.", new[] { "PhoneNumber" });
                    if (this.OfficeId == 0)
						yield return new ValidationResult("Office is required.", new[] { "OfficeId" });
                    if (this.VendorId == 0)
                        yield return new ValidationResult("Vendor is required.", new[] { "VendorId" });
                    //if (String.IsNullOrWhiteSpace(this.SSN))
                    //    yield return new ValidationResult("Last 4 of SSN is required.", new[] { "SSN" });
                    //if (String.IsNullOrWhiteSpace(this.City))
                    //    yield return new ValidationResult("City is required.", new[] { "City" });
                    //if (String.IsNullOrWhiteSpace(this.State))
                    //    yield return new ValidationResult("State is required.", new[] { "State" });
                    //if (DateTime.Compare(this.BirthDate , new DateTime(0001, 1, 1, 0, 0, 0)) == 0)
                    //    yield return new ValidationResult("Birthdate is required.", new[] { "BirthDate" });
                    
                    break;

                case "Sales Administrator":
                    break;



				default:
					yield return new ValidationResult("Invalid User Type");
					break;
			}

			if (
                (oldUser == null && VendorId.GetValueOrDefault(0) == 0 && AppLogic.GetUser(Username) != null) || //client admin or vendor admin agent id already exists
                (oldUser == null && VendorId.GetValueOrDefault(0) > 0 && AppLogic.GetVendorUser(Username, VendorId.Value) != null) || //std user and agent id already exists for vendor
                (oldUser != null && VendorId.GetValueOrDefault(0) == 0 && oldUser.AgentId != this.Username && AppLogic.GetUser(Username) != null) || //client admin or vendor admin agent id that changed already exists
                (oldUser != null && VendorId.GetValueOrDefault(0) > 0 && oldUser.AgentId != this.Username && AppLogic.GetVendorUser(Username, VendorId.Value) != null))  //std user that agent id changed and agent id already exists for vendor
            {
				yield return new ValidationResult("Username is already assigned.", new[] { "Username" });
			}
		}
	}
}