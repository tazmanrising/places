using Calibrus.SparkPortal.DataAccess.Infrastructure;

namespace Calibrus.SparkPortal.DataAccess.Repository
{
	public class DispositionRepository : Repository<Disposition>
	{
		public DispositionRepository(SparkPortalDataEntities ctx) : base(ctx)
		{
		}
	}
}
