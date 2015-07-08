/*==============================================================================================================================
| Author        Jeremy Caney (Jeremy@Caney.net)
| Client        Skepti-Forum Project
| Project       Forum Archive
\=============================================================================================================================*/
using System.Web.Mvc;
using SkeptiForum.Archive;
using System.Threading.Tasks;

namespace SkeptiForum.Archive.Controllers {

  /*============================================================================================================================
  | CLASS: SITEMAP CONTROLLER
  \---------------------------------------------------------------------------------------------------------------------------*/
  /// <summary>
  ///   Used to provide access to a sitemap.org sitemap for use by search engines.
  /// </summary>
  [RoutePrefix("Sitemap")]
  public class SitemapController : Controller {

    /*==========================================================================================================================
    | GET: /SITEMAP
    \-------------------------------------------------------------------------------------------------------------------------*/
    /// <summary>
    ///   Generates an index of sitemaps available on this site, each corresponding to a Facebook group.
    /// </summary>
    /// <returns>An index of sitemaps.</returns>
    [Route()]
    public ActionResult Index() {
      Response.ContentType = "application/xml";
      return View(ArchiveManager.Groups);
    }

    /*==========================================================================================================================
    | GET: /GROUPS/KEY/SITEMAP/
    \-------------------------------------------------------------------------------------------------------------------------*/
    /// <summary>
    ///   Generates a sitemap of posts associated with an individual group based on JSON files discovered on disk based on the 
    ///   friendly group key name.
    /// </summary>
    /// <param name="id">The identifier of the Facebook Group.</param>
    /// <returns>An index of posts associated with the specified group.</returns>
    [Route("~/Groups/{groupKey:alpha}/Sitemap")]
    public async Task<ActionResult> Group(string groupKey) {
      var group = ArchiveManager.Groups[groupKey];
      return await GetSitemapView(group.Id);
    }

    /*==========================================================================================================================
    | GET: /GROUPS/4/SITEMAP/
    \-------------------------------------------------------------------------------------------------------------------------*/
    /// <summary>
    ///   Generates a sitemap of posts associated with an individual group based on JSON files discovered on disk.
    /// </summary>
    /// <param name="id">The identifier of the Facebook Group.</param>
    /// <returns>An index of posts associated with the specified group.</returns>
    [Route("~/Groups/{groupId:long}/Sitemap")]
    public async Task<ActionResult> Group(long groupId = -1) {
      return await GetSitemapView(groupId);
    }

    /*==========================================================================================================================
    | METHOD: GET SITEMAP VIEW (HELPER)
    \-------------------------------------------------------------------------------------------------------------------------*/
    /// <summary>
    ///   Generates a sitemap of posts associated with an individual group based on JSON files discovered on disk.
    /// </summary>
    /// <param name="id">The identifier of the Facebook Group.</param>
    /// <returns>An index of posts associated with the specified group.</returns>
    private async Task<ActionResult> GetSitemapView(long groupId = -1) {

      /*------------------------------------------------------------------------------------------------------------------------
      | Validate group identifier
      \-----------------------------------------------------------------------------------------------------------------------*/
      if (groupId < 1 || !ArchiveManager.Groups.Contains(groupId)) {
        return HttpNotFound("A group with the ID '" + groupId + "' does not exist.");
      }

      /*------------------------------------------------------------------------------------------------------------------------
      | Set Content Type to XML
      \-----------------------------------------------------------------------------------------------------------------------*/
      Response.ContentType = "application/xml";

      /*------------------------------------------------------------------------------------------------------------------------
      | Return data to view
      \-----------------------------------------------------------------------------------------------------------------------*/
      return View(await ArchiveManager.StorageProvider.GetPostsAsync(groupId));

    }


  } //Class
} //Namespace