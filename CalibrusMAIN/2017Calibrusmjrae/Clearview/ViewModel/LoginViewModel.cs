using System;
using System.Collections.Generic;
using System.Linq;
using Calibrus.ClearviewPortal.Business;
using Calibrus.ClearviewPortal.DataAccess.Infrastructure;
using System.ComponentModel.DataAnnotations;

namespace Calibrus.ClearviewPortal.ViewModel
{
    public class LoginViewModel
    {
        [Required]
        [Display(Name = "UserName")]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }

        public CurrentUser LoggedInUser { get; set; }

        public bool ValidateUser()
        {
            User user = LoginLogic.ValidateUser(Username, Password);
            if (user != null)
            {
                LoggedInUser = new CurrentUser()
                {
                    AgentId = user.AgentId,
                    Password = user.Password,
					VendorNumber = user.Vendor != null ?  user.Vendor.VendorNumber : null,
					VendorId = user.Vendor != null ? user.Vendor.VendorId : 0,
					OfficeId = user.Office != null ? user.Office.OfficeId : 0,
					OfficeName = user.Office != null ? user.Office.OfficeName : String.Empty,
                    UserType = user.UserType
                };
                return true;
            }
            return false;
        }

        public class CurrentUser
        {
            public string AgentId { get; set; }

            public string Password { get; set; }

            public string VendorNumber { get; set; }

            public int VendorId { get; set; }

			public int OfficeId { get; set; }

	        public string OfficeName { get; set; }

            public DataAccess.Infrastructure.UserType UserType { get; set; }
        
        }
    }
}