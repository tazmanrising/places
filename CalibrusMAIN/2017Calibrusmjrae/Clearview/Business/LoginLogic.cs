using Calibrus.ClearviewPortal.DataAccess.Infrastructure;
using Calibrus.ClearviewPortal.DataAccess.Repository;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Calibrus.ClearviewPortal.Business
{
	public static class LoginLogic
	{
		public static User ValidateUser(string username, string password)
		{
			User user;
			using (CustomClearviewEntities ctx = new CustomClearviewEntities())
			{
				UserRepository repo = new UserRepository(ctx);
				user = repo.Find(x => x.AgentId == username && x.Password == password, ut => ut.UserType, v => v.Vendor, o => o.Office);
			}
			return user;
		}
        
        public static bool IsClientAdmin(string username)
		{
			using (CustomClearviewEntities ctx = new CustomClearviewEntities())
			{
				UserRepository repo = new UserRepository(ctx);
				User user = repo.Find(x => x.AgentId == username, ut => ut.UserType);
				return user.UserType.SecurityLevel == 1000;
			}
		}

        public static User ValidateDataEntryUser(string ClearviewId, string password)
        {
            User user;
            using (CustomClearviewEntities ctx = new CustomClearviewEntities())
            {
                UserRepository repo = new UserRepository(ctx);
                var users = repo.Filter(x => x.Phone == password, ut => ut.UserType, v => v.Vendor, o => o.Office).ToList();
                if(users.Count==0)
                {
                    user = null;
                }
                else
                {
                    user = users.FirstOrDefault(x => x.ClearviewId == ClearviewId);
                }
            }
            return user;
        }
    }
}