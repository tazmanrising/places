using Calibrus.ClearviewPortal.DataAccess.Infrastructure;
using System.Collections.Generic;
using System.Web.Http;
using Calibrus.ClearviewPortal.Web.CustomAttributes;
using Microsoft.Ajax.Utilities;

namespace Calibrus.ClearviewPortal.Web.Controllers
{
	[CustomApiAccessAuthorize]
    public class RatesController : ApiController
    {
        // GET api/<controller>
        public IEnumerable<Program> Get()
        {
            List<Program> programs = Business.AppLogic.GetPrograms(false);

			foreach (Program p in programs)
			{
				p.UnitOfMeasure.IfNotNull(x => x.Programs = null);
				p.PremiseType.IfNotNull(x => x.Programs = null);
				p.Utility.IfNotNull(x => x.Programs = null);
				p.UtilityType.IfNotNull(x => x.Programs = null);
				p.AccountNumberType.IfNotNull(x => x.Programs = null);
                p.Brand.IfNotNull(x => x.Programs = null);
			}

	        return programs;
        }

		public IEnumerable<Program> Get(int id)
		{
            List<Program> programs = Business.AppLogic.GetPrograms(true, id);

            foreach (Program p in programs)
            {
                p.UnitOfMeasure.IfNotNull(x => x.Programs = null);
                p.PremiseType.IfNotNull(x => x.Programs = null);
                p.Utility.IfNotNull(x => x.Programs = null);
                p.UtilityType.IfNotNull(x => x.Programs = null);
                p.AccountNumberType.IfNotNull(x => x.Programs = null);
                p.Brand.IfNotNull(x => x.Programs = null);
            }

            return programs;
        }
    }
}