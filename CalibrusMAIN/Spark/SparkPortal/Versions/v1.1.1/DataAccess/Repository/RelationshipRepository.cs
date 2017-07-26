using Calibrus.SparkPortal.DataAccess.Infrastructure;

namespace Calibrus.SparkPortal.DataAccess.Repository
{
    public class RelationshipRepository : Repository<Relationship>
    {
        public RelationshipRepository(SparkPortalDataEntities ctx) : base(ctx)
        {

        }
    }
}