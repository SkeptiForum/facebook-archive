/*==============================================================================================================================
| Author        Jeremy Caney (Jeremy@Caney.net)
| Client        Skepti-Forum Project
| Project       Forum Archive
\=============================================================================================================================*/
using System;
using System.Web.Mvc;
using Newtonsoft.Json.Linq;

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
    public ActionResult Details(long groupId, long postId = 0) {

      /*------------------------------------------------------------------------------------------------------------------------
      | Establish variables
      \-----------------------------------------------------------------------------------------------------------------------*/
      var jsonPath = Server.MapPath("/Archives/" + groupId + "/" + groupId + "_" + postId + ".json");

      /*------------------------------------------------------------------------------------------------------------------------
      | Validate group Id
      \-----------------------------------------------------------------------------------------------------------------------*/
      if (groupId < 1 || !System.IO.Directory.Exists(Server.MapPath("/Archives/" + groupId))) {
        return HttpNotFound("A group with the ID '" + groupId + "' does not exist.");
      }

      /*------------------------------------------------------------------------------------------------------------------------
      | If the post doesn't exist in the archive, redirect to Facebook
      \-----------------------------------------------------------------------------------------------------------------------*/
      if (postId > 0 && !System.IO.File.Exists(jsonPath)) {
      //return HttpNotFound("A post with the name '" + postId + "' does not exist. If it is a newer post, it may not have been archived yet.");
        return Redirect("https://www.facebook.com/groups/" + groupId + "/permalink/" + postId);
      }
      dynamic json = JObject.Parse(System.IO.File.ReadAllText(jsonPath));

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