using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Calibrus.SparkPortal.DataAccess.Infrastructure;

namespace Calibrus.SparkPortal.DataAccess.Repository
{
    public class StateRepository : Repository<State>
    {
        public StateRepository(SparkPortalDataEntities ctx)
            : base(ctx) 
        {
        }
    }
}
