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
  public class ThreadsController : Controller {

    /*==========================================================================================================================
    | GET: /GROUPS/5/PERMALINK/5/
    \-------------------------------------------------------------------------------------------------------------------------*/
    /// <summary>
    ///   Provides access to a specific JSON file previously archived on the server via the Facebook Graph API. 
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
    public async Task<ActionResult> Details(long groupId, long postId = 0) {

      /*------------------------------------------------------------------------------------------------------------------------
      | Validate group Id
      \-----------------------------------------------------------------------------------------------------------------------*/
      if (groupId < 1 || !ArchiveManager.Groups.Contains(groupId)) {
        return HttpNotFound("A group with the ID '" + groupId + "' does not exist.");
      }

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