/*==============================================================================================================================
| Author        Jeremy Caney (Jeremy@Caney.net)
| Client        Skepti-Forum Project
| Project       Forum Archive
\=============================================================================================================================*/
using System;
using System.Web.Mvc;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace SkeptiForum.Archive.Controllers {

  /*============================================================================================================================
  | CLASS: THREADS CONTROLLER
  \---------------------------------------------------------------------------------------------------------------------------*/
  /// <summary>
  ///   Provides access to individual threads via the routing rules established in the 
  ///   <see cref="SkeptiForum.Archive.Web.RouteConfig"/> class.
  /// </summary>
  [RoutePrefix("Groups")]
  public class ThreadsController : Controller {

    /*==========================================================================================================================
    | GET: /GROUPS/KEY/PERMALINK/5/
    \-------------------------------------------------------------------------------------------------------------------------*/
    /// <summary>
    ///   Provides access to a specific JSON file previously archived on the server via the Facebook Graph API using the 
    ///   friendly key name of the group (e.g., /VaccineSF/). 
    /// </summary>
    /// <remarks>
    ///   This action is simply intended to provide a lookup against the friendly key name; all further processing is handled by
    ///   the primary identifier-based route. 
    /// </remarks>
    /// <returns>A view containing all details of the requested current thread.</returns>
    [Route("{groupKey:alpha}/permalink/{postId}")]
    public async Task<ActionResult> Details(string groupKey, long postId = 0) {

      /*------------------------------------------------------------------------------------------------------------------------
      | Validate group Id
      \-----------------------------------------------------------------------------------------------------------------------*/
      var group = ArchiveManager.Groups[groupKey];

      if (!ArchiveManager.Groups.Contains(groupKey)) {
        return HttpNotFound("A group with the key '" + groupKey + "' does not exist.");
      }

      /*------------------------------------------------------------------------------------------------------------------------
      | Lookup and return appropriate model, view
      \-----------------------------------------------------------------------------------------------------------------------*/
      return await GetGroupView(group.Id, postId);

    }

    /*==========================================================================================================================
    | GET: /GROUPS/5/PERMALINK/5/
    \-------------------------------------------------------------------------------------------------------------------------*/
    /// <summary>
    ///   Provides access to a specific JSON file previously archived on the server via the Facebook Graph API. 
    /// </summary>
    /// <returns>A view containing all details of the requested current thread.</returns>
    [Route("{groupId:long}/permalink/{postId}")]
    public async Task<ActionResult> Details(long groupId, long postId = 0) {

      /*------------------------------------------------------------------------------------------------------------------------
      | Validate group Id
      \-----------------------------------------------------------------------------------------------------------------------*/
      if (groupId < 1 || !ArchiveManager.Groups.Contains(groupId)) {
        return HttpNotFound("A group with the ID '" + groupId + "' does not exist.");
      }

      /*------------------------------------------------------------------------------------------------------------------------
      | If group has key, use that path instead
      \-----------------------------------------------------------------------------------------------------------------------*/
      var group = ArchiveManager.Groups[groupId];
      if (group.Key != null) {
        return RedirectToAction("Details", "Threads", new { groupKey = group.Key, postId = postId });
      }

      /*------------------------------------------------------------------------------------------------------------------------
      | Lookup and return appropriate model, view
      \-----------------------------------------------------------------------------------------------------------------------*/
      return await GetGroupView(groupId, postId);

    }

    /*==========================================================================================================================
    | METHOD: GET GROUP VIEW (HELPER)
    \-------------------------------------------------------------------------------------------------------------------------*/
    /// <summary>
    ///   Centralized validation, model lookup, and view binding. 
    /// </summary>
    /// <remarks>
    ///   <para>
    ///     Provides a view model containing basic metadata necessary for the rendering of the server-side view (which, namely, 
    ///     includes metadata for search engines and Facebook's Open Graph schema.
    ///   </para>
    ///   <para>
    ///     Note: If the associated JSON file cannot be found, the view will redirect to the corresponding URL on Facebook. 
    ///     This may happen because the thread hasn't yet been indexed. It may also occur, however, if the IDs provided 
    ///     are invalid. In the latter case, Facebook will notify the user that this is the case by displaying an error.
    ///   </para>
    /// </remarks>
    /// <returns>A view containing all details of the requested current thread.</returns>
    private async Task<ActionResult> GetGroupView(long groupId, long postId = 0) {

      /*------------------------------------------------------------------------------------------------------------------------
      | If the post doesn't exist in the archive, redirect to Facebook
      \-----------------------------------------------------------------------------------------------------------------------*/
      if (!await ArchiveManager.StorageProvider.PostExistsAsync(groupId, postId)) {
        return Redirect("https://www.facebook.com/groups/" + groupId + "/permalink/" + postId);
      }

      /*------------------------------------------------------------------------------------------------------------------------
      | Retreive post
      \-----------------------------------------------------------------------------------------------------------------------*/
      dynamic json = await ArchiveManager.StorageProvider.GetPostAsync(groupId, postId);

      /*------------------------------------------------------------------------------------------------------------------------
      | Provide preprocessing of variables required by the view
      \-----------------------------------------------------------------------------------------------------------------------*/
      FacebookPost post = new FacebookPost(json);

      /*------------------------------------------------------------------------------------------------------------------------
      | Return data to view
      \-----------------------------------------------------------------------------------------------------------------------*/
      return View(post);

    }


  } //Class
} //Namespace