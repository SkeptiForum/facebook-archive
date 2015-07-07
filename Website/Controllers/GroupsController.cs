using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace SkeptiForum.Archive.Controllers {

  [RoutePrefix("api/Groups")]
  public class GroupsController : ApiController {

    // GET api/Groups
    [Route()]
    public FacebookGroupCollection Get() {
      return ArchiveManager.Groups;
    }

    // GET api/Groups/5
    [Route("{groupId:long}")]
    public FacebookGroup Get(long groupId) {
      return ArchiveManager.Groups[groupId];
    }

    // GET api/Groups/5/5/
    [Route("{groupId:long}/{postId:long}")]
    public dynamic GetPost(long groupId, long postId) {
      return ArchiveManager.StorageProvider.GetPostAsync(groupId, postId).Result;
    }

    [Route("{groupId:long}/Update")]
    [HttpPost]
    public async Task<FacebookGroup> UpdateGroupAsync(long groupId) {

      /*------------------------------------------------------------------------------------------------------------------------
      | Validate the access token
      \-----------------------------------------------------------------------------------------------------------------------*/
      if (String.IsNullOrEmpty(ArchiveManager.Configuration.Api.Token)) {
        throw new ArgumentException(
          "This method assumes the presence of a cookie with the key 'fbak', representing the long-lived Facebook access token."
          + "This cookie is set by the /Api/Authorize/ endpoint, which accepts temporary access token returned by the Facebook"
          + "JavaScript SDK."
        );
      }

      /*------------------------------------------------------------------------------------------------------------------------
      | Retrieve all posts associated with the groups
      \-----------------------------------------------------------------------------------------------------------------------*/
      var group = ArchiveManager.Groups[groupId];
      var groupPosts = await group.GetPostsAsync(group.LastArchived);

      /*------------------------------------------------------------------------------------------------------------------------
      | Write each post to disk
      \-----------------------------------------------------------------------------------------------------------------------*/
      foreach (dynamic post in groupPosts) {
        var id = Int64.Parse(post.id.Substring(post.id.IndexOf("_")+1));
        await ArchiveManager.StorageProvider.SetPostAsync(post);
      }

      /*------------------------------------------------------------------------------------------------------------------------
      | Update group metadata 
      \-----------------------------------------------------------------------------------------------------------------------*/
      group.LastArchived = DateTime.Now;
      await ArchiveManager.StorageProvider.SetGroupsAsync();

      /*------------------------------------------------------------------------------------------------------------------------
      | Return latest state for group
      \-----------------------------------------------------------------------------------------------------------------------*/
      return group;

    }

    /*==========================================================================================================================
    | POST: /API/AUTHORIZE/
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
    [Route("~/Api/Authorize")]
    [HttpPost]
    public dynamic Authorize([FromBody] string accessToken) {

      /*------------------------------------------------------------------------------------------------------------------------
      | Set default expiration
      \-----------------------------------------------------------------------------------------------------------------------*/
      long expiry = 20 * 60 * 60;

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
      return new {
        status = true
      };

    }

  } //Class
} //Namespace