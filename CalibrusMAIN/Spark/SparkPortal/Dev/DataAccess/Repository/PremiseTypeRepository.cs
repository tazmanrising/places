using Calibrus.SparkPortal.DataAccess.Infrastructure;
using System.Collections.Generic;

namespace Calibrus.SparkPortal.DataAccess.Repository
{
    public sealed class PremiseTypeRepository : Repository<PremiseType>
    {
        public PremiseTypeRepository(SparkPortalDataEntities ctx)
            : base(ctx)
        {
        }

        public List<PremiseType> GetActiveItems()
        {
            return this.Filter(x => x.IsActive, o => o.DisplayOrder, System.Data.SqlClient.SortOrder.Ascending);
        }

    }
}
