using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Calibrus.ClearviewPortal.DataAccess.Infrastructure;

namespace Calibrus.ClearviewPortal.DataAccess.Repository
{
    public sealed class UserRepository : Repository<User>
    {
        public UserRepository(CustomClearviewEntities ctx) : base(ctx) 
        {
        }


    }
}
