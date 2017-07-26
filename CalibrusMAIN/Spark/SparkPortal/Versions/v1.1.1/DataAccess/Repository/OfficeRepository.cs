using Calibrus.SparkPortal.DataAccess.Infrastructure;

namespace Calibrus.SparkPortal.DataAccess.Repository
{
	public sealed class OfficeRepository : Repository<Office>
	{
		public OfficeRepository(SparkPortalDataEntities ctx) : base(ctx)
		{
		}
	}
}