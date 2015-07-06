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

namespace SkeptiForum.Archive.Web.Controllers {

  /*============================================================================================================================
  | CLASS: ADMIN CONTROLLER
  \---------------------------------------------------------------------------------------------------------------------------*/
  /// <summary>
  ///   Provides external access to administrative requests.
  /// </summary>
  public class AdminController : Controller {

    /*==========================================================================================================================
    | GET: /ADMIN/
    \-------------------------------------------------------------------------------------------------------------------------*/
    /// <summary>
    ///   Provides access to a list of administrative options available.
    /// </summary>
    /// <returns>The index view for available administrative tasks.</returns>
    public ActionResult Index() {
      return View();
    }

    /*==========================================================================================================================
    | GET: /ADMIN/DOWNLOAD/
    \-------------------------------------------------------------------------------------------------------------------------*/
    /// <summary>
    ///   Checks the user's authentication status and, if they are authorized to use the application, gives them the option of 
    ///   downloading an archive of group posts from the Facebook Graph API via the <see cref="FacebookGroup.GetPostsAsync"/>
    ///   method. 
    /// </summary>
    /// <returns>The download view.</returns>
    public async Task<ActionResult> Download() {
      return View(ArchiveManager.Groups);
    }

    /*==========================================================================================================================
    | POST: /ADMIN/DOWNLOAD/
    \-------------------------------------------------------------------------------------------------------------------------*/
    /// <summary>
    ///   Processes the request to download an archive of group posts from the Facebook Graph API via the 
    ///   <see cref="FacebookGroup.GetPostsAsync"/> method. 
    /// </summary>
    /// <remarks>
    ///   This method assumes that the user is authenticated against the application. Note: This method may take several minutes 
    ///   to run.
    /// </remarks>
    /// <returns>A view model containing a list of forums whose posts were downloaded.</returns>
    [HttpPost]
    public async Task<ActionResult> Download(FormCollection collection) {

      /*------------------------------------------------------------------------------------------------------------------------
      | Validate the access token
      \-----------------------------------------------------------------------------------------------------------------------*/
      if (String.IsNullOrEmpty(ArchiveManager.Configuration.Api.Token)) {
        throw new ArgumentException(
          "This method assumes the presence of a cookie with the key 'fbak', representing the long-lived Facebook access token."
          + "This cookie is set by the /Admin/Authorize/ endpoint, which accepts temporary access token returned by the Facebook"
          + "JavaScript SDK."
        );
      }

      /*------------------------------------------------------------------------------------------------------------------------
      | Retrieve all posts associated with the groups
      \-----------------------------------------------------------------------------------------------------------------------*/
      var groupPosts = await ArchiveManager.Groups.GetPostsAsync();

      /*------------------------------------------------------------------------------------------------------------------------
      | Provide debug data regarding the number of posts
      \-----------------------------------------------------------------------------------------------------------------------*/
      foreach (Collection<dynamic> group in groupPosts) {
        foreach (dynamic post in group) {
          var directoryPath = Server.MapPath("~/Archives/" + post.to.data[0].id + "/");
          var filePath = directoryPath + "\\" + post.id + ".json";
          if (!ViewData.ContainsKey(post.to.data[0].name)) {
            ViewData.Add(post.to.data[0].name, "Id: " + post.to.data[0].id + " (" + group.Count + " posts)");
          }
          if (!IO.Directory.Exists(directoryPath)) {
            IO.Directory.CreateDirectory(directoryPath);
          }
          if (!IO.File.Exists(filePath)) {
            using (IO.StreamWriter sw = IO.File.CreateText(filePath)) {
              sw.Write(post.ToString());
            }
          }
        }
      }

      return View();
    }

    /*==========================================================================================================================
    | POST: /ADMIN/AUTHORIZE/
    \-------------------------------------------------------------------------------------------------------------------------*/
    /// <summary>
    ///   Given a temporary access token from the Facebook JavaScript SDK, returns a long-lived access token via the Facebook
    ///   Graph API.  
    /// </summary>
    /// <remarks>
    ///   Note: This method assumes that the user is authenticated against the application.
    /// </remarks>
    /// <param name="accessToken">The short-lived (session) access token </param>
    /// <returns>A JSON object representing whether or not the operation was successful.</returns>
    [HttpPost]
    public ActionResult Authorize(string accessToken) {

      /*------------------------------------------------------------------------------------------------------------------------
      | Set default expiration
      \-----------------------------------------------------------------------------------------------------------------------*/
      long expiry = 20*60*60;

      /*------------------------------------------------------------------------------------------------------------------------
      | Exchange temporary token for long-lived token
      \-----------------------------------------------------------------------------------------------------------------------*/
      accessToken = ArchiveManager.DataProvider.GetLongLivedToken(accessToken, out expiry);

      /*------------------------------------------------------------------------------------------------------------------------
      | Set token to cookie
      \-----------------------------------------------------------------------------------------------------------------------*/
      ArchiveManager.Configuration.Api.SetToken(accessToken, expiry);

      /*------------------------------------------------------------------------------------------------------------------------
      | Return results
      \-----------------------------------------------------------------------------------------------------------------------*/
      return Json(new {
        status = true
      });

    }

  } //Class
} //Namespace 
