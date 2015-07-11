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
using System.Web.Http.ModelBinding;
using System.Web.Http.OData;
using System.Web.Http.OData.Routing;
using SkeptiForum.Archive.Reporting;

namespace SkeptiForum.Archive.Controllers {
  public class ActivityLogController : ODataController {
    private ReportingContext db = new ReportingContext();

    // GET: odata/ActivityLogController
    [EnableQuery]
    public IQueryable<Activity> GetActivity() {
      return db.ActivityLog;
    }

    // GET: odata/ActivityLogController(5)
    [EnableQuery]
    public SingleResult<Activity> GetActivity([FromODataUri] long key) {
      return SingleResult.Create(db.ActivityLog.Where(Activity => Activity.Id == key));
    }

    // PUT: odata/ActivityLogController(5)
    public async Task<IHttpActionResult> Put([FromODataUri] long key, Delta<Activity> patch) {
      Validate(patch.GetEntity());

      if (!ModelState.IsValid) {
        return BadRequest(ModelState);
      }

      Activity Activity = await db.ActivityLog.FindAsync(key);
      if (Activity == null) {
        return NotFound();
      }

      patch.Put(Activity);

      try {
        await db.SaveChangesAsync();
      }
      catch (DbUpdateConcurrencyException) {
        if (!ActivityExists(key)) {
          return NotFound();
        }
        else {
          throw;
        }
      }

      return Updated(Activity);
    }

    // POST: odata/ActivityLogController
    public async Task<IHttpActionResult> Post(Activity Activity) {
      if (!ModelState.IsValid) {
        return BadRequest(ModelState);
      }

      db.ActivityLog.Add(Activity);
      await db.SaveChangesAsync();

      return Created(Activity);
    }

    // PATCH: odata/ActivityLogController(5)
    [AcceptVerbs("PATCH", "MERGE")]
    public async Task<IHttpActionResult> Patch([FromODataUri] long key, Delta<Activity> patch) {
      Validate(patch.GetEntity());

      if (!ModelState.IsValid) {
        return BadRequest(ModelState);
      }

      Activity Activity = await db.ActivityLog.FindAsync(key);
      if (Activity == null) {
        return NotFound();
      }

      patch.Patch(Activity);

      try {
        await db.SaveChangesAsync();
      }
      catch (DbUpdateConcurrencyException) {
        if (!ActivityExists(key)) {
          return NotFound();
        }
        else {
          throw;
        }
      }

      return Updated(Activity);
    }

    // DELETE: odata/ActivityLogController(5)
    public async Task<IHttpActionResult> Delete([FromODataUri] long key) {
      Activity Activity = await db.ActivityLog.FindAsync(key);
      if (Activity == null) {
        return NotFound();
      }

      db.ActivityLog.Remove(Activity);
      await db.SaveChangesAsync();

      return StatusCode(HttpStatusCode.NoContent);
    }

    protected override void Dispose(bool disposing) {
      if (disposing) {
        db.Dispose();
      }
      base.Dispose(disposing);
    }

    private bool ActivityExists(long key) {
      return db.ActivityLog.Count(e => e.Id == key) > 0;
    }
  }
}
