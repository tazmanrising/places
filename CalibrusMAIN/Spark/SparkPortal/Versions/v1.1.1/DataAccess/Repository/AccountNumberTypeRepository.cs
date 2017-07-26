using System.Collections.Generic;
using Calibrus.SparkPortal.DataAccess.Infrastructure;

namespace Calibrus.SparkPortal.DataAccess.Repository
{
    public sealed class AccountNumberTypeRepository : Repository<AccountNumberType>
    {
        public AccountNumberTypeRepository(SparkPortalDataEntities ctx)
            : base(ctx)
        {
        }

        public List<AccountNumberType> GetActiveItems()
        {
            return this.Filter(x => x.IsActive, o => o.DisplayOrder, System.Data.SqlClient.SortOrder.Ascending);
        }
    }
}
