using Calibrus.ClearviewPortal.DataAccess.Infrastructure;

namespace Calibrus.ClearviewPortal.DataAccess.Repository
{
    public class RelationshipRepository : Repository<Relationship>
    {
        public RelationshipRepository(CustomClearviewEntities ctx) : base(ctx)
        {

        }
    }
}