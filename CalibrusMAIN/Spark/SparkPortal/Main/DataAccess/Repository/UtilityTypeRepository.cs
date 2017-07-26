using Calibrus.SparkPortal.DataAccess.Infrastructure;
using System.Collections.Generic;

namespace Calibrus.SparkPortal.DataAccess.Repository
{
    public sealed class UtilityTypeRepository : Repository<UtilityType>
    {
        public UtilityTypeRepository(SparkPortalDataEntities ctx)
            : base(ctx)
        {
        }

        public List<UtilityType> GetActiveItems()
        {
            return this.Filter(x => x.IsActive, o => o.DisplayOrder, System.Data.SqlClient.SortOrder.Ascending);
        }

    }
}
