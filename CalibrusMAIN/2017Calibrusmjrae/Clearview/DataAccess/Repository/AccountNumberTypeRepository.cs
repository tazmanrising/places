using System.Collections.Generic;
using Calibrus.ClearviewPortal.DataAccess.Infrastructure;

namespace Calibrus.ClearviewPortal.DataAccess.Repository
{
    public sealed class AccountNumberTypeRepository : Repository<AccountNumberType>
    {
        public AccountNumberTypeRepository(CustomClearviewEntities ctx)
            : base(ctx)
        {
        }

        public List<AccountNumberType> GetActiveItems()
        {
            return this.Filter(x => x.IsActive, o => o.DisplayOrder, System.Data.SqlClient.SortOrder.Ascending);
        }
    }
}
