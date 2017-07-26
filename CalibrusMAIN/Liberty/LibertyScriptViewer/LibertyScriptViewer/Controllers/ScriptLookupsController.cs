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
using LibertyScriptViewer.Models;

namespace LibertyScriptViewer.Controllers
{
    public class ScriptLookupsController : ApiController
    {
        private LibertyEntities db = new LibertyEntities();

        // GET: api/ScriptLookups
        public IQueryable<ScriptName> GetScriptLookups()
        {
            //Returns list of scripts
            return db.ScriptLookups.Where(sl => sl.Active == true).Select(sl => new ScriptName { State = sl.State, Commercial = sl.Commercial, Script = sl.Script }).Distinct().OrderBy(x => x.State);
        }
        // GET: api/ScriptLookup/somescript
        [ResponseType(typeof(spReturnScript_Result))]
        public IEnumerable<spReturnScript_Result> GetScriptLookups(string id)
        {
            //returns specific script
            return db.spReturnScript(id);
        }

        // GET: api/ScriptLookups/History/scriptname/id
        [System.Web.Http.AcceptVerbs("GET", "POST")]
        [System.Web.Http.HttpGet]
        public IQueryable<ScriptLog> GetHistory(string script, int id)
        {
            return db.ScriptLogs.Where(x => x.ScriptName == script && x.ScriptId == id);
        }

        // POST: api/ScriptLookups/EmailChanges/scriptChanges
        [System.Web.Http.AcceptVerbs("POST")]
        [System.Web.Http.HttpPost]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PostEmailChanges(HttpRequestMessage request)
        {

            try
            {
                //Test if you want to see the values pass in for debugging in watch window
                //var content = await request.Content.ReadAsStringAsync();
                //var emailItems = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(content);

                var emailItems = Newtonsoft.Json.JsonConvert.DeserializeObject<EmailObject>(await request.Content.ReadAsStringAsync());

                //send email             
                Utilities.AppLogic.EmailScriptChanges(emailItems);

            }
            catch (Exception ex)
            {
                return StatusCode(HttpStatusCode.BadRequest);
            }
            return StatusCode(HttpStatusCode.OK);
            //return StatusCode(HttpStatusCode.NoContent);            
        }

        //// PUT: api/ScriptLookups/5
        //[ResponseType(typeof(void))]
        //public async Task<IHttpActionResult> PutScriptLookup(int id, ScriptLookup scriptLookup)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    if (id != scriptLookup.ScriptLookupId)
        //    {
        //        return BadRequest();
        //    }

        //    db.Entry(scriptLookup).State = EntityState.Modified;

        //    try
        //    {
        //        await db.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!ScriptLookupExists(id))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return StatusCode(HttpStatusCode.NoContent);
        //}

        //// POST: api/ScriptLookups
        //[ResponseType(typeof(ScriptLookup))]
        //public async Task<IHttpActionResult> PostScriptLookup(ScriptLookup scriptLookup)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    db.ScriptLookups.Add(scriptLookup);
        //    await db.SaveChangesAsync();

        //    return CreatedAtRoute("DefaultApi", new { id = scriptLookup.ScriptLookupId }, scriptLookup);
        //}

        //// DELETE: api/ScriptLookups/5
        //[ResponseType(typeof(ScriptLookup))]
        //public async Task<IHttpActionResult> DeleteScriptLookup(int id)
        //{
        //    ScriptLookup scriptLookup = await db.ScriptLookups.FindAsync(id);
        //    if (scriptLookup == null)
        //    {
        //        return NotFound();
        //    }

        //    db.ScriptLookups.Remove(scriptLookup);
        //    await db.SaveChangesAsync();

        //    return Ok(scriptLookup);
        //}

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