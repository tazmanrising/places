using Calibrus.ClearviewPortal.DataAccess.Infrastructure;

namespace Calibrus.ClearviewPortal.DataAccess.Repository
{
    public sealed class VendorRepository : Repository<Vendor>
    {
        public VendorRepository(CustomClearviewEntities ctx)
            : base(ctx)
        {
        }
    }
}