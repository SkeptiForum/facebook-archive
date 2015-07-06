/*==============================================================================================================================
| Author        Jeremy Caney (Jeremy@Caney.net)
| Client        Skepti-Forum Project
| Project       Forum Archive
\=============================================================================================================================*/
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Facebook;
using SkeptiForum.Archive.Configuration;

namespace SkeptiForum.Archive.Providers {

  /*============================================================================================================================
  | CLASS: FACEBOOK DATA PROVIDER
  \---------------------------------------------------------------------------------------------------------------------------*/
  public class FacebookDataProvider : ArchiveDataProviderBase {

    /*==========================================================================================================================
    | PRIVATE VARIABLES
    \-------------------------------------------------------------------------------------------------------------------------*/

    /*==========================================================================================================================
    | CONSTRUCTOR
    \-------------------------------------------------------------------------------------------------------------------------*/
    /// <summary>
    ///
    /// </summary>
    public FacebookDataProvider() { }

    /*==========================================================================================================================
    | PROPERTY: CLIENT
    \-------------------------------------------------------------------------------------------------------------------------*/
    /// <summary>
    ///   Gets a reference to a <see cref="FacebookClient" /> object that will be shared by all users. It is expected that 
    ///   client code will check to ensure the client is instantiated with a <see cref="FacebookClient.AccessToken" /> and 
    ///   <see cref="FacebookClient.AppSecret" /> before referencing; if these are not set, then it is the responsibility of 
    ///   the client to look these up and provide them to the class.
    /// </summary>
    public FacebookClient GetClient(string accessToken = null) {
      var client = new FacebookClient();
      client.AppId = ArchiveManager.Configuration.Api.AppKey;
      client.AppSecret = ArchiveManager.Configuration.Api.AppSecret;
      client.AccessToken = accessToken?? ArchiveManager.Configuration.Api.Token;
      return client;
    }

    /*==========================================================================================================================
    | METHOD: GET LONG LIVED TOKEN
    \-------------------------------------------------------------------------------------------------------------------------*/
    /// <summary>
    ///   When a user authenticated using the Facebook JavaScript SDK, a short-lived (session) access token is returned. This is 
    ///   not sufficient for continued use of the application. To mitigate this, client code may request that the access token
    ///   be replaced with a long-term access token. Will additionally return the expiration time of the access token via an 
    ///   output parameter; this will <i>typically</i> be sixty days, but that is not guaranteed by the Facebook Graph API.
    /// </summary>
    /// <param name="accessToken">The short-lived (session) access token assigned by the Facebook JavaScript SDK.</param>
    /// <param name="expiry">The expiration time of the long-term access token returned.</param>
    public override string GetLongLivedToken(string accessToken, out long expiry) {
      dynamic result = new FacebookClient().Get(
        "oauth/access_token",
        new {
          client_id = ArchiveManager.Configuration.Api.AppKey,
          client_secret = ArchiveManager.Configuration.Api.AppSecret,
          grant_type = "fb_exchange_token",
          fb_exchange_token = accessToken
        }
      );
      expiry = result.expires;
      return result.access_token;
    }

    /*==========================================================================================================================
    | METHOD: GET POSTS (ASYNCHRONOUS)
    \-------------------------------------------------------------------------------------------------------------------------*/
    /// <summary>
    ///   Asynchronously retrieves all posts and related comments associated with the group from the Facebook Graph API. Will 
    ///   continue requesting posts from the API via paging until there are no more posts available.
    /// </summary>
    /// <returns>A collection of dynamic objects, each representing the JSON response from the Facebook Graph API.</returns>
    public override async Task<Collection<dynamic>> GetPostsAsync(long groupId) {

      /*------------------------------------------------------------------------------------------------------------------------
      | Establish variables
      \-----------------------------------------------------------------------------------------------------------------------*/
      var client = GetClient();
      var postLimit = ArchiveManager.Configuration?.Queries?.Posts?.Limit ?? 50;
      var commentLimit = ArchiveManager.Configuration?.Queries?.Comments?.Limit ?? 750;
      var postsCollection = new Collection<dynamic>();
      var postFields = "id,from,to,message,message_tags,name,object_id,picture,properties,shares,source,"
        + "caption,description,link,story,story_tags,status_type,type,created_time,is_expired,"
        + "likes.limit(500)";
      var commentFields = "id,from,message,message_tags,created_time,like_count,attachment";
      var groupPath = groupId + "/feed"
        + "?limit=" + postLimit
        + "&fields=" + (ArchiveManager.Configuration?.Queries?.Posts?.Fields ?? postFields);
      if (groupPath.Length > 0 && groupPath.IndexOf("comments") < 0) {
        groupPath += ",comments"
          + ".limit(" + commentLimit + ")"
          + "{" + (ArchiveManager.Configuration?.Queries?.Comments?.Fields ?? commentFields) + "}";
      }

      /*------------------------------------------------------------------------------------------------------------------------
      | Asynchronously query Facebook's Graph API for posts
      >-------------------------------------------------------------------------------------------------------------------------
      | ### NOTE JJC200615: Due to the amount of data being requested from Facebook, some groups may require multiple requests.
      | To facilitate this, this method continues to query the Facebook API while (it appears) there are more results. This is
      | determined by comparing the limit to the number of returned results; if they are the same, the method assumes there are 
      | more results, and signals this by setting the `groupPath` parameter to the next page URL returned by Facebook.
      \-----------------------------------------------------------------------------------------------------------------------*/
      while (!String.IsNullOrEmpty(groupPath)) {
        dynamic posts;
        try {
          posts = await client.GetTaskAsync<dynamic>(groupPath);
        }
        catch (Exception ex) {
          throw new Exception(
            "Error from Facebook while loading data for " + Name + " (" + groupId + ") with the following query: " 
            + "'" + groupPath + "'. The error message was '" + ex.Message + "'."
          );
        }
        foreach (dynamic post in posts.data) {
          postsCollection.Add(post);
        }
        groupPath = "";
        if (posts.data.Count == postLimit) {
          groupPath = posts.paging.next;
        }
      }

      /*------------------------------------------------------------------------------------------------------------------------
      | Return the results
      \-----------------------------------------------------------------------------------------------------------------------*/
      return postsCollection;

    }

    /*==========================================================================================================================
    | METHOD: GET GROUPS (ASYNCHRONOUS)
    \-------------------------------------------------------------------------------------------------------------------------*/
    /// <summary>
    ///   
    /// </summary>
    /// <returns></returns>
    public override async Task<Collection<dynamic>> GetGroupsAsync(bool? publicOnly = null, string filter = null) {

      /*------------------------------------------------------------------------------------------------------------------------
      | Establish defaults
      \-----------------------------------------------------------------------------------------------------------------------*/
        string fields = ArchiveManager.Configuration?.Queries?.Groups?.Fields ?? "id,name,privacy";
        int limit = ArchiveManager.Configuration?.Queries?.Groups?.Limit ?? 750;

        publicOnly = publicOnly ?? ArchiveManager.Configuration?.Queries?.Groups?.PublicOnly ?? true;
        filter = filter ?? ArchiveManager.Configuration?.Queries?.Groups?.Filter ?? "Skepti-Forum";

      /*------------------------------------------------------------------------------------------------------------------------
      | Establish variables
      \-----------------------------------------------------------------------------------------------------------------------*/
      dynamic groupCollection = new Collection<dynamic>(); 
      dynamic groupResults = await GetClient().GetTaskAsync("me/groups?limit=" + limit + "&fields=" + fields);

      /*------------------------------------------------------------------------------------------------------------------------
      | Loop through each group and add it to the collection
      \-----------------------------------------------------------------------------------------------------------------------*/
      foreach (dynamic group in groupResults.data) {
        if (!String.IsNullOrEmpty(filter) && !group.name.Contains(filter)) continue;
        if (publicOnly == true && !group.privacy.Equals("open", StringComparison.InvariantCultureIgnoreCase)) continue;
        FacebookGroup facebookGroup = new FacebookGroup(group);
        groupCollection.Add(facebookGroup);
      }

      return groupCollection;
    }

    /*==========================================================================================================================
    | METHOD: GET POST (ASYNCHRONOUS)
    \-------------------------------------------------------------------------------------------------------------------------*/
    /// <summary>
    ///   
    /// </summary>
    /// <returns></returns>
    public override async Task<dynamic> GetPostAsync(long groupId, long postId) {
      return await GetClient().GetTaskAsync("me/groups/" + groupId + "/" + postId);
    }

  } //Class
} //Namespace
