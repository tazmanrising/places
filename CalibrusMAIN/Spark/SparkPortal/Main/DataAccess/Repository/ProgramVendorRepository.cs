using Calibrus.SparkPortal.DataAccess.Infrastructure;

namespace Calibrus.SparkPortal.DataAccess.Repository
{
	public sealed class ProgramVendorRepository : Repository<ProgramVendor>
	{
		public ProgramVendorRepository(SparkPortalDataEntities ctx)
			: base(ctx)
		{
		}
	}
}