using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Calibrus.SparkPortal.DataAccess.Infrastructure;

namespace Calibrus.SparkPortal.DataAccess.Repository
{
    public sealed class UserRepository : Repository<User>
    {
        public UserRepository(SparkPortalDataEntities ctx) : base(ctx) 
        {
        }


    }
}
