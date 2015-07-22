/*==============================================================================================================================
| Author        Jeremy Caney (Jeremy@Caney.net)
| Client        Skepti-Forum Project
| Project       Forum Archive
\=============================================================================================================================*/
using System;
using System.Web;
using System.Web.Mvc;
using IO = System.IO;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using SkeptiForum.Archive;
using SkeptiForum.Archive.Reporting;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace SkeptiForum.Archive.Web.Controllers {

  /*============================================================================================================================
  | CLASS: ADMIN CONTROLLER
  \---------------------------------------------------------------------------------------------------------------------------*/
  /// <summary>
  ///   Provides external access to administrative requests.
  /// </summary>
  [RoutePrefix("Admin")]
  public class AdminController : Controller {

    /*==========================================================================================================================
    | GET: /ADMIN/
    \-------------------------------------------------------------------------------------------------------------------------*/
    /// <summary>
    ///   Provides access to a list of administrative options available. 
    /// </summary>
    /// <remarks>
    ///   The controller provides the view with a list of all groups configured for the application. This includes the last 
    ///   archive date and the last index date, which the view may use to determine which groups are in need of rearchiving
    ///   or reindexing, and provide access to the appropriate methods via the WebAPI.
    /// </remarks>
    /// <returns>The index view for available administrative tasks.</returns>
    [Route()]
    public ActionResult Index() {
      return View(ArchiveManager.Groups);
    }

  } //Class
} //Namespace 
