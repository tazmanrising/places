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
using ConstellationScriptViewer.Models;

namespace ConstellationScriptViewer.Controllers
{
    public class ScriptLookupsController : ApiController
    {
        private ConstellationEntities db = new ConstellationEntities();

        // GET: api/tblScriptLookups
        public IQueryable<ScriptName> GetScriptLookups()
        {
            //Returns list of scripts
            return db.tblScriptLookups.Select(sl => new ScriptName { Script = sl.Script }).Distinct().OrderBy(x => x.Script);
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
        public IQueryable<tblScriptLog> GetHistory(string script, int id)
        {
            return db.tblScriptLogs.Where(x => x.ScriptName == script && x.ScriptId == id);
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

        //// GET: api/tblScriptLookups/5
        //[ResponseType(typeof(tblScriptLookup))]
        //public IHttpActionResult GettblScriptLookup(int id)
        //{
        //    tblScriptLookup tblScriptLookup = db.tblScriptLookups.Find(id);
        //    if (tblScriptLookup == null)
        //    {
        //        return NotFound();
        //    }

        //    return Ok(tblScriptLookup);
        //}

        //// PUT: api/tblScriptLookups/5
        //[ResponseType(typeof(void))]
        //public IHttpActionResult PuttblScriptLookup(int id, tblScriptLookup tblScriptLookup)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    if (id != tblScriptLookup.ScriptLookupId)
        //    {
        //        return BadRequest();
        //    }

        //    db.Entry(tblScriptLookup).State = EntityState.Modified;

        //    try
        //    {
        //        db.SaveChanges();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!tblScriptLookupExists(id))
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

        //// POST: api/tblScriptLookups
        //[ResponseType(typeof(tblScriptLookup))]
        //public IHttpActionResult PosttblScriptLookup(tblScriptLookup tblScriptLookup)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    db.tblScriptLookups.Add(tblScriptLookup);
        //    db.SaveChanges();

        //    return CreatedAtRoute("DefaultApi", new { id = tblScriptLookup.ScriptLookupId }, tblScriptLookup);
        //}

        //// DELETE: api/tblScriptLookups/5
        //[ResponseType(typeof(tblScriptLookup))]
        //public IHttpActionResult DeletetblScriptLookup(int id)
        //{
        //    tblScriptLookup tblScriptLookup = db.tblScriptLookups.Find(id);
        //    if (tblScriptLookup == null)
        //    {
        //        return NotFound();
        //    }

        //    db.tblScriptLookups.Remove(tblScriptLookup);
        //    db.SaveChanges();

        //    return Ok(tblScriptLookup);
        //}

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool tblScriptLookupExists(int id)
        {
            return db.tblScriptLookups.Count(e => e.ScriptLookupId == id) > 0;
        }
    }
}