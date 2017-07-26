using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Calibrus.ClearviewPortal.DataAccess.Infrastructure;

namespace Calibrus.ClearviewPortal.DataAccess.Repository
{
    public sealed class UserLogRepository : Repository<UserLog>
    {
        public UserLogRepository(CustomClearviewEntities ctx) : base(ctx)
        {
        }
    }
}
