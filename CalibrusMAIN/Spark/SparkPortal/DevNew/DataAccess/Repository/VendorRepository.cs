using Calibrus.SparkPortal.DataAccess.Infrastructure;

namespace Calibrus.SparkPortal.DataAccess.Repository
{
    public sealed class VendorRepository : Repository<Vendor>
    {
        public VendorRepository(SparkPortalDataEntities ctx)
            : base(ctx)
        {
        }
    }
}