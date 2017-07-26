using Calibrus.ClearviewPortal.DataAccess.Infrastructure;

namespace Calibrus.ClearviewPortal.DataAccess.Repository
{
	public class DispositionRepository : Repository<Disposition>
	{
		public DispositionRepository(CustomClearviewEntities ctx) : base(ctx)
		{
		}
	}
}
