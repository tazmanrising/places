using System.Collections.Generic;
using Calibrus.SparkPortal.DataAccess.Infrastructure;

namespace Calibrus.SparkPortal.DataAccess.Repository
{
	public sealed class SalesChannelRepository : Repository<SalesChannel>
	{
		public SalesChannelRepository(SparkPortalDataEntities ctx) : base(ctx)
		{
		}

		public List<SalesChannel> GetActiveItems()
		{
			return this.Filter(x => x.IsActive, o => o.Name, System.Data.SqlClient.SortOrder.Ascending);
		}
	}
}
