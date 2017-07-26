using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Calibrus.ClearviewPortal.DataAccess.Infrastructure;

namespace Calibrus.ClearviewPortal.DataAccess.Repository
{
	public sealed class UtilityRepository : Repository<Utility>
	{
		public UtilityRepository(CustomClearviewEntities ctx)
			: base(ctx)
		{
		}

		public List<Utility> GetActiveItems()
		{
			return this.Filter(x => x.IsActive, o => o.DisplayOrder, System.Data.SqlClient.SortOrder.Ascending);
		}
	}
}

