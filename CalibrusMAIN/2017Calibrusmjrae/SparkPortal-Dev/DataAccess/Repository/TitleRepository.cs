using Calibrus.SparkPortal.DataAccess.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calibrus.SparkPortal.DataAccess.Repository
{
    public class TitleRepository : Repository<Title>
    {
        public TitleRepository(SparkPortalDataEntities ctx) : base(ctx)
        {
        }
    }
}
