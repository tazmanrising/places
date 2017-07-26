using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Calibrus.ClearviewPortal.DataAccess.Infrastructure;

namespace Calibrus.ClearviewPortal.DataAccess.Repository
{
    public class DtDAgentTrackRepository: Repository<DtDAgentTrack>
    {
        public DtDAgentTrackRepository(CustomClearviewEntities ctx)
            : base(ctx)
        {
        }
    }
}
