using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Calibrus.ClearviewPortal.DataAccess.CodeFirst;
using Calibrus.ClearviewPortal.DataAccess.CodeFirst.Models;

namespace Calibrus.ClearviewPortal.DataAccess.CodeFirst
{
    public class UserService
    {

        public static List<UserLog> GetUserLogs(int id)
        {

            try
            {
                using (var ctx = new ClearviewContext())
                {
                    var query = ctx.UserLogs
                       .Where(z => z.UserId == id)
                       .AsEnumerable()
                       .Select(z => new UserLog()
                       {
                           UserLogId = z.UserLogId,
                           UserId  = z.UserId,
                           IsActive = z.IsActive,
                           Note = z.Note,
                           CreatedDateTime = z.CreatedDateTime,
                           CreatedBy = z.CreatedBy
                     
                       }).ToList();

                    return query;
                }
            }
            catch (Exception e)
            {
                var x = e.Message;
                throw;
            }


        }

    }

}
