using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Calibrus.ClearviewPortal.DataAccess.Infrastructure;

namespace Calibrus.ClearviewPortal.DataAccess.Repository
{
    public class BrandRepository : Repository<Brand>
    {
        public BrandRepository(CustomClearviewEntities ctx) : base(ctx)
        {
        }
    }
}
