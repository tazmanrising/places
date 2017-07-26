using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using apiTest.Models;

namespace apiTest.Controllers
{
    public class ScriptLookupsController : ApiController
    {
        private SparkEntities db = new SparkEntities();

        // GET: api/ScriptLookups
        public IQueryable<ScriptNames> GetScriptLookups()
        {
            return db.ScriptLookups.Select(x => new ScriptNames { State = x.State, SalesChannel = x.SalesChannel, Script = x.Script }).Distinct();
        }

        [System.Web.Http.AcceptVerbs("GET", "POST")]
        [System.Web.Http.HttpGet]
        public IQueryable<ScriptLog> history(string script, int id)
        {

          return  db.ScriptLogs.Where(x => x.ScriptName == script && x.ScriptId == id);
        }

        // GET: api/ScriptLookups/somescript
        [ResponseType(typeof(spReturnScript_Result))]
        public IEnumerable<spReturnScript_Result> GetScriptLookup(string id)
        {
           
            return db.spReturnScript(id);
        }



        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ScriptLookupExists(int id)
        {
            return db.ScriptLookups.Count(e => e.ScriptLookupId == id) > 0;
        }
    }
}