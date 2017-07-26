using System.Collections.Generic;
using Calibrus.ClearviewPortal.DataAccess.Infrastructure;

namespace Calibrus.ClearviewPortal.DataAccess.Repository
{
	public sealed class UserTypeRepository : Repository<UserType>
	{
		public UserTypeRepository(CustomClearviewEntities ctx)
			: base(ctx)
		{
		}

		public List<UserType> GetActiveItems()
		{
			return this.Filter(x => x.IsActive, o => o.DisplayOrder, System.Data.SqlClient.SortOrder.Ascending);
		}
	}
}