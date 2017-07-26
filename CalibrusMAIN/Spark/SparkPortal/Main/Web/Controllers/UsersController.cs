using System;
using Calibrus.SparkPortal.DataAccess.Infrastructure;
using System.Collections.Generic;
using System.Web.Http;
using Calibrus.SparkPortal.Web.CustomAttributes;
using Microsoft.Ajax.Utilities;

namespace Calibrus.SparkPortal.Web.Controllers
{
	[CustomApiAccessAuthorize]
    public class UsersController : ApiController
    {
        // GET api/<controller>
        public IEnumerable<User> Get()   
        {
            var users = Business.AppLogic.GetUsers(true);
            foreach (User u in users)
            {
                u.Password = "**********************";
                u.Vendor.IfNotNull(v => v.Users = null);
                u.Office.IfNotNull(o => o.Users = null);
                u.UserType.IfNotNull(ut => ut.Users = null);
            }


            return users;
        }

        [Route("api/users/userlist/{querytype}/{id:int?}")]
        public IEnumerable<spGetUsers_Result> GetOfficeUsers(string querytype, int id) { var users = Business.AppLogic.GetUsersOnly(null, querytype, id); return users; }

        [Route("api/users/userlist/{querytype}/{id:int?}/{isactive:bool}")]
        public  IEnumerable<spGetUsers_Result>  GetUsersOnly(string querytype, int id, bool isactive)
        {
           
            var users = Business.AppLogic.GetUsersOnly(isactive,querytype,id);
            return users;
        }

        [Route("api/users/userlogs/{id:int}")]
        public IEnumerable<UserLog> GetUserLogs(int id)
        {
            List<UserLog> logs = Business.AppLogic.GetUserLogs(id);
            return logs;
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
        public void Post(UserStatus us)
        {
            User user = Business.AppLogic.GetUser(us.UserId);
            user.IsActive = !user.IsActive;
            user.ModifiedBy = us.LoggedInUser;
            user.ModifiedDateTime = DateTime.Now;
            Business.AppLogic.UpdateUser(user);

            Business.AppLogic.CreateUserLog(new UserLog
            {
                UserId = user.UserId,
                CreatedBy = us.LoggedInUser,
                CreatedDateTime = DateTime.Now,
                IsActive = user.IsActive,
                Note = us.Reason
            });
            
        
        }

        public class UserStatus
        {
            public int UserId { get; set; }
            public string Reason { get; set; }
            public string LoggedInUser { get; set; }        
        }

      
    }
}