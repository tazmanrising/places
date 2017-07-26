using System.Collections.Generic;
using Calibrus.ClearviewPortal.DataAccess.Infrastructure;

namespace Calibrus.ClearviewPortal.DataAccess.Repository
{
	public sealed class SalesChannelRepository : Repository<SalesChannel>
	{
		public SalesChannelRepository(CustomClearviewEntities ctx) : base(ctx)
		{
		}

		public List<SalesChannel> GetActiveItems()
		{
			return this.Filter(x => x.IsActive, o => o.Name, System.Data.SqlClient.SortOrder.Ascending);
		}
	}
}
