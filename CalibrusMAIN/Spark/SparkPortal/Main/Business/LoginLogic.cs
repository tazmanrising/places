using Calibrus.SparkPortal.DataAccess.Infrastructure;
using Calibrus.SparkPortal.DataAccess.Repository;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Calibrus.SparkPortal.Business
{
	public static class LoginLogic
	{
		public static User ValidateUser(string username, string password)
		{
			User user;
			using (CustomSparkPortalDataEntities ctx = new CustomSparkPortalDataEntities())
			{
				UserRepository repo = new UserRepository(ctx);
				user = repo.Find(x => x.AgentId == username && x.Password == password, ut => ut.UserType, v => v.Vendor, o => o.Office);
			}
			return user;
		}
        
        public static bool IsClientAdmin(string username)
		{
			using (CustomSparkPortalDataEntities ctx = new CustomSparkPortalDataEntities())
			{
				UserRepository repo = new UserRepository(ctx);
				User user = repo.Find(x => x.AgentId == username, ut => ut.UserType);
				return user.UserType.SecurityLevel == 1000;
			}
		}

        public static User ValidateDataEntryUser(string sparkId, string password)
        {
            User user;
            using (CustomSparkPortalDataEntities ctx = new CustomSparkPortalDataEntities())
            {
                UserRepository repo = new UserRepository(ctx);
                var users = repo.Filter(x => x.Phone == password && (x.UserTypeId == 4 || x.UserTypeId == 5), ut => ut.UserType, v => v.Vendor, o => o.Office).ToList();
                if(users.Count==0)
                {
                    user = null;
                }
                else
                {
                    user = users.FirstOrDefault(x => x.SparkId == sparkId);
                }
            }
            return user;
        }
    }
}