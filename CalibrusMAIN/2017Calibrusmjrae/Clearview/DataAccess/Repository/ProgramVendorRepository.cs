using Calibrus.ClearviewPortal.DataAccess.Infrastructure;

namespace Calibrus.ClearviewPortal.DataAccess.Repository
{
	public sealed class ProgramVendorRepository : Repository<ProgramVendor>
	{
		public ProgramVendorRepository(CustomClearviewEntities ctx)
			: base(ctx)
		{
		}
	}
}