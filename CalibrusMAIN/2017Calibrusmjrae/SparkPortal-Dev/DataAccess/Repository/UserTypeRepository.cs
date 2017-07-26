using System.Collections.Generic;
using Calibrus.SparkPortal.DataAccess.Infrastructure;

namespace Calibrus.SparkPortal.DataAccess.Repository
{
	public sealed class UserTypeRepository : Repository<UserType>
	{
		public UserTypeRepository(SparkPortalDataEntities ctx)
			: base(ctx)
		{
		}

		public List<UserType> GetActiveItems()
		{
			return this.Filter(x => x.IsActive, o => o.DisplayOrder, System.Data.SqlClient.SortOrder.Ascending);
		}
	}
}