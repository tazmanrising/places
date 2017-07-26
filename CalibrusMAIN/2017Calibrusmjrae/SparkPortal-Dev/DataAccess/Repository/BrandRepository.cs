using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Calibrus.SparkPortal.DataAccess.Infrastructure;

namespace Calibrus.SparkPortal.DataAccess.Repository
{
    public class BrandRepository : Repository<Brand>
    {
        public BrandRepository(SparkPortalDataEntities ctx) : base(ctx)
        {
        }
    }
}
