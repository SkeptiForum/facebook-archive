/*==============================================================================================================================
| Author        Jeremy Caney (Jeremy@Caney.net)
| Client        Skepti-Forum Project
| Project       Forum Archive
\=============================================================================================================================*/
using System.Web.Mvc;

namespace SkeptiForum.Archive.Controllers {

  /*============================================================================================================================
  | CLASS: SITEMAP CONTROLLER
  \---------------------------------------------------------------------------------------------------------------------------*/
  /// <summary>
  ///   Used to provide access to a sitemap.org sitemap for use by search engines.
  /// </summary>
  public class SitemapController : Controller {

    /*==========================================================================================================================
    | GET: /SITEMAP
    \-------------------------------------------------------------------------------------------------------------------------*/
    /// <summary>
    ///   Generates an index of sitemaps available on this site, each corresponding to a Facebook group.
    /// </summary>
    /// <returns>An index of sitemaps.</returns>
    public ActionResult Index() {
      var directory = new System.IO.DirectoryInfo(Server.MapPath("/Archives/"));
      var directories = directory.GetDirectories();
      Response.ContentType = "application/xml";
      return View(directories);
    }

    /*==========================================================================================================================
    | GET: /SITEMAP/4/
    \-------------------------------------------------------------------------------------------------------------------------*/
    /// <summary>
    ///   Generates a sitemap of posts associated with an individual group based on JSON files discovered on disk.
    /// </summary>
    /// <param name="id">The identifier of the Facebook Group.</param>
    /// <returns>An index of posts associated with the specified group.</returns>
    public ActionResult Group(long id = -1) {

      /*------------------------------------------------------------------------------------------------------------------------
      | Validate group identifier
      \-----------------------------------------------------------------------------------------------------------------------*/
      if (id < 1 || !System.IO.Directory.Exists(Server.MapPath("/Archives/" + id))) {
        return HttpNotFound("A group with the ID '" + id + "' does not exist.");
      }

      /*------------------------------------------------------------------------------------------------------------------------
      | Set Content Type to XML
      \-----------------------------------------------------------------------------------------------------------------------*/
      Response.ContentType = "application/xml";

      /*------------------------------------------------------------------------------------------------------------------------
      | Return routing variables via ViewData
      \-----------------------------------------------------------------------------------------------------------------------*/
      ViewData["GroupId"] = id;

      /*------------------------------------------------------------------------------------------------------------------------
      | Retrieve JSON from disk
      \-----------------------------------------------------------------------------------------------------------------------*/
      var directory = new System.IO.DirectoryInfo(Server.MapPath("/Archives/" + id + "/"));
      var files = directory.GetFiles("*.json");

      /*------------------------------------------------------------------------------------------------------------------------
      | Return data to view
      \-----------------------------------------------------------------------------------------------------------------------*/
      return View(files);

    }

  } //Class
} //Namespace