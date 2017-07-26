using Calibrus.ClearviewPortal.DataAccess.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calibrus.ClearviewPortal.DataAccess.Repository
{
    public class LeadRepository : Repository<Lead>
    {
        public LeadRepository(CustomClearviewEntities ctx) : base(ctx) 
        {
        }
    }
}
