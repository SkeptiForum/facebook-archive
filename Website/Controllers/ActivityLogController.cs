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
using SkeptiForum.Archive.Reporting.Providers;

namespace SkeptiForum.Archive.Controllers {

  public class ActivityLogController : ODataController {

    private ReportingContext db = new ReportingContext();

    // GET: odata/ActivityLog
    [EnableQuery]
    [Queryable(PageSize = 500)]
    public IQueryable<Activity> GetActivityLog() {
      return db.ActivityLog;
    }

    // GET: odata/ActivityLog(5)
    [EnableQuery]
    public SingleResult<Activity> GetActivity([FromODataUri] long key) {
      return SingleResult.Create(db.ActivityLog.Where(activity => activity.Id == key));
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

  } //Class
} //Namespace
