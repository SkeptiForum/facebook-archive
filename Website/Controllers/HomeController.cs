/*==============================================================================================================================
| Author        Jeremy Caney (Jeremy@Caney.net)
| Client        Skepti-Forum Project
| Project       Forum Archive
\=============================================================================================================================*/
using System.Web.Mvc;

namespace SkeptiForum.Archive.Controllers {

  /*============================================================================================================================
  | CLASS: HOME CONTROLLER
  \---------------------------------------------------------------------------------------------------------------------------*/
  /// <summary>
  ///   Provides access to the default homepage for the site, as well as any ancillary pages (e.g., "About", "Contact", etc).
  /// </summary>
  public class HomeController : Controller {

    /*==========================================================================================================================
    | GET: /
    \-------------------------------------------------------------------------------------------------------------------------*/
    /// <summary>
    ///   Provides the landing page experience for the site. By default, this is a Google Custom Search Engine (CSE) used to 
    ///   search the indexed pages on the site.
    /// </summary>
    /// <returns>The archive site's homepage.</returns>
    public ActionResult Index() {
      return View();
    }

  }
}
