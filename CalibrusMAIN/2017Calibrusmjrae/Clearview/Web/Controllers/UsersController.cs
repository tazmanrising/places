using System;
using Calibrus.ClearviewPortal.DataAccess.Infrastructure;
using System.Collections.Generic;
using System.Web.Http;
using Calibrus.ClearviewPortal.Web.CustomAttributes;
using Microsoft.Ajax.Utilities;
using Calibrus.ClearviewPortal.DataAccess.CodeFirst;
using Calibrus.ClearviewPortal.DataAccess.CodeFirst.Models;

namespace Calibrus.ClearviewPortal.Web.Controllers
{
	[CustomApiAccessAuthorize]
    public class UsersController : ApiController
    {
        // GET api/<controller>
        [Route("api/users/")]
        public IEnumerable<User> Get()
        {
            List<User> users = Business.AppLogic.GetUsers(false);

	        foreach (User u in users)
	        {
                u.Password = "**********************";
                u.Vendor.IfNotNull(v => v.Users = null);
                u.Vendor.IfNotNull(v => v.Offices = null);
                u.Office.IfNotNull(o => o.Users = null);
                u.Office.IfNotNull(o => o.Vendor = null);
                u.UserType.IfNotNull(ut => ut.Users = null);
            }

	        return users;

        }

        [Route("api/users/{getAdmins:bool}")]
        public IEnumerable<User> Get(bool getAdmins)
        {
            List<User> users = Business.AppLogic.GetUsers(false, getAdmins);

            foreach (User u in users)
            {
                u.Password = "**********************";
                u.Vendor.IfNotNull(v => v.Users = null);
                u.Vendor.IfNotNull(v => v.Offices = null);
                u.Office.IfNotNull(o => o.Users = null);
                u.Office.IfNotNull(o => o.Vendor = null);
                u.UserType.IfNotNull(ut => ut.Users = null);
            }

            return users;

        }

        [Route("api/users/vendor/{id:int}")]
        public IEnumerable<User> GetVendorUsers(int id)
        {
			List<User> users = Business.AppLogic.GetVendorUsers(id, false);

			foreach (User u in users)
			{
				u.Vendor.IfNotNull(v => v.Users = null);
				u.Office.IfNotNull(o => o.Users = null);
				u.UserType.IfNotNull(ut => ut.Users = null);
			}

			return users;
        }

		[Route("api/users/office/{id:int}")]
		public IEnumerable<User> GetOfficeUsers(int id)
		{
			List<User> users = Business.AppLogic.GetOfficeUsers(id, false);

			foreach (User u in users)
			{
				u.Vendor.IfNotNull(v => v.Users = null);
				u.Office.IfNotNull(o => o.Users = null);
				u.UserType.IfNotNull(ut => ut.Users = null);
			}

			return users;
		}

		[Route("api/usertype/{id:int}")]
	    public UserType GetUserType(int id)
	    {
		    return Business.AppLogic.GetUserType(id);
	    }

        [HttpPost]
        [Route("api/user/status")]
        public void UpdateUserStatus(UserStatus us)
        {
            User user = Business.AppLogic.GetUser(us.UserId);
            user.IsActive = !user.IsActive;
            user.ModifiedBy = us.LoggedInUser;
            user.ModifiedDateTime = DateTime.Now;
            Business.AppLogic.UpdateUser(user);

            Business.AppLogic.CreateUserLog(new DataAccess.Infrastructure.UserLog
            {
                UserId = user.UserId,
                CreatedBy = us.LoggedInUser,
                CreatedDateTime = DateTime.Now,
                IsActive = user.IsActive,
                Note = us.Reason
            });
            
        
        }

        [Route("api/users/userlogs/{id:int}")]
        public IEnumerable<DataAccess.CodeFirst.Models.UserLog> GetUserLogs(int id)
        {
            //List<UserLog> logs = Business.AppLogic.GetUserLogs(id);
            var userLogs = new List<DataAccess.CodeFirst.Models.UserLog>();
            try
            {
                userLogs = UserService.GetUserLogs(id);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            return userLogs;
        }


        public class UserStatus
        {
            public int UserId { get; set; }
            public string Reason { get; set; }
            public string LoggedInUser { get; set; }        
        }

      
    }
}