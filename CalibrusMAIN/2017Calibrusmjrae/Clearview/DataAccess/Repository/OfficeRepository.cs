using Calibrus.ClearviewPortal.DataAccess.Infrastructure;

namespace Calibrus.ClearviewPortal.DataAccess.Repository
{
	public sealed class OfficeRepository : Repository<Office>
	{
		public OfficeRepository(CustomClearviewEntities ctx) : base(ctx)
		{
		}
	}
}