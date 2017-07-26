using Calibrus.ClearviewPortal.DataAccess.Infrastructure;
using System.Collections.Generic;

namespace Calibrus.ClearviewPortal.DataAccess.Repository
{
    public sealed class UnitOfMeasureRepository : Repository<UnitOfMeasure>
    {
        public UnitOfMeasureRepository(CustomClearviewEntities ctx)
            : base(ctx)
        {
        }

        public List<UnitOfMeasure> GetActiveItems()
        {
            return this.Filter(x => x.IsActive, o => o.DisplayOrder, System.Data.SqlClient.SortOrder.Ascending);
        }


    }
}
