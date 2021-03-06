﻿/*==============================================================================================================================
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
  /// <summary>
  ///   Provides a concrete implementation of the <see cref="ArchiveDataProviderBase"/> configured to talk to Facebook via the 
  ///   <see cref="Facebook.FacebookClient"/> service. 
  /// </summary>
  /// <remarks>
  ///   This provider depends on API information being configured via the <see cref="Configuration.ApiElement"/> element. 
  ///   Without this data, the provider will throw exceptions.
  /// </remarks>
  public class FacebookDataProvider : ArchiveDataProviderBase {

    /*==========================================================================================================================
    | PRIVATE VARIABLES
    \-------------------------------------------------------------------------------------------------------------------------*/

    /*==========================================================================================================================
    | CONSTRUCTOR
    \-------------------------------------------------------------------------------------------------------------------------*/
    /// <summary>
    ///   Constructs a new instance of the <see cref="FacebookDataProvider"/> provider.
    /// </summary>
    public FacebookDataProvider() { }

    /*==========================================================================================================================
    | PROPERTY: CLIENT
    \-------------------------------------------------------------------------------------------------------------------------*/
    /// <summary>
    ///   Gets a reference to a <see cref="FacebookClient" /> object that will be shared by all users. 
    /// </summary>
    /// <remarks>
    ///   It is expected that client code will check to ensure the client is instantiated with a <see 
    ///   cref="FacebookClient.AccessToken" /> and <see cref="FacebookClient.AppSecret" /> before referencing; if these are not 
    ///   set, then it is the responsibility of the client to look these up and provide them to the class.
    /// </remarks>
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
    ///   Exchanges a short-lived (session) access token with a long-lived (persistent) token, along with that token's 
    ///   expiration date.
    /// </summary>
    /// <remarks>
    ///   <para>
    ///     When a user authenticated using the Facebook JavaScript SDK, a short-lived (session) access token is returned. This 
    ///     is not sufficient for continued use of the application. To mitigate this, client code may request that the access 
    ///     token be replaced with a long-term access token. 
    ///   </para>
    ///   <para>
    ///     This overload allows the caller to retrieve the precise expiration of the token. While this is typically two months,
    ///     this will vary not only by service, but also by other provider-specific conditions. For instance, a service may 
    ///     return an existing long-lived token which expires two months from the date it was issued, but which may have been 
    ///     issued at a previous date.
    ///   </para>
    /// </remarks>
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
    | METHOD: GET GROUPS (ASYNCHRONOUS)
    \-------------------------------------------------------------------------------------------------------------------------*/
    /// <summary>
    ///   Populates the collection with a list of <see cref="SkeptiForum.Archive.FacebookGroup"/> objects based on a query to the 
    ///   Facebook API.
    /// </summary>
    /// <param name="publicOnly">
    ///   Determines if only public groups should be listed. Set to false to load closed and private groups; be aware that this 
    ///   may introduce potential privacy concerns if also loading posts. Optionally overrides the value set for the collection.
    /// </param>
    /// <param name="filter">
    ///   Sets the filter to use in evaluating whether or not to include a group in the collection. This is intended to exclude 
    ///   groups not affiliated with the Skepti-Forum project. Optionally overrides the value set for the collection.
    /// </param>
    /// <returns>A collection of dynamic objects, each representing the JSON response from the Facebook Graph API.</returns>
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
    | METHOD: GET POSTS (ASYNCHRONOUS)
    \-------------------------------------------------------------------------------------------------------------------------*/
    /// <summary>
    ///   Retrieves a collection of posts asynchronously from the backend service, optionally filtering based on a date range
    ///   based on the key-based identifier.
    /// </summary>
    /// <param name="groupId">The unique key identifier for the group to query.</param>
    /// <param name="since">The (optional) start date to pull records from.</param>
    /// <param name="until">The (optional) end date to pull records up to.</param>
    public override async Task<Collection<dynamic>> GetPostsAsync(long groupId, DateTime? since = null, DateTime? until = null) {

      /*------------------------------------------------------------------------------------------------------------------------
      | Establish defaults
      \-----------------------------------------------------------------------------------------------------------------------*/
      var postLimit             = 50;
      var commentLimit          = 750;
      var updatePostLimit       = 5000;
      var postFields            = "id,from,to,message,message_tags,name,object_id,picture,properties,shares,source,"
                                + "caption,description,link,story,story_tags,status_type,type,created_time,is_expired,"
                                + "likes.limit(500)";
      var commentFields         = "id,from,message,message_tags,created_time,like_count,attachment";

      /*------------------------------------------------------------------------------------------------------------------------
      | Set values
      \-----------------------------------------------------------------------------------------------------------------------*/
      postLimit                 = ArchiveManager.Configuration?.Queries?.Posts?.Limit ?? postLimit;
      commentLimit              = ArchiveManager.Configuration?.Queries?.Comments?.Limit ?? commentLimit;
      postFields                = ArchiveManager.Configuration?.Queries?.Posts?.Fields ?? postFields;
      commentFields             = ArchiveManager.Configuration?.Queries?.Comments?.Fields ?? commentFields;

      /*------------------------------------------------------------------------------------------------------------------------
      | Establish variables
      \-----------------------------------------------------------------------------------------------------------------------*/
      var client                = GetClient();
      var postsCollection       = new Collection<dynamic>();
      var taskRunner            = new Collection<Task<dynamic>>();

      /*------------------------------------------------------------------------------------------------------------------------
      | Establish group path
      \-----------------------------------------------------------------------------------------------------------------------*/
      var groupQuerystring      = "fields=" + (postFields);
      if (groupQuerystring.IndexOf("comments") < 0) {
        groupQuerystring        += ",comments.limit(" + commentLimit + "){" + (commentFields) + "}";
      }
      var groupPath             = groupId + "/feed?limit=" + postLimit + "&" + groupQuerystring;
      if (since != null) {
        groupPath               += "&since=" + since.Value.ToString("o");
      }
      if (until != null) {
         groupPath               += "&until=" + until.Value.ToString("o");
      }
      var updatePostPath        = groupId + "/feed?limit=" + updatePostLimit + "&fields=created_time,updated_time";

      /*------------------------------------------------------------------------------------------------------------------------
      | Establish queue for legacy posts
      >-------------------------------------------------------------------------------------------------------------------------
      | Facebook's "since" parameter only applied to "created_time" not "updated_time". The following looks through an index of 
      | all posts to find those that were created before the "since" value, but updated afterwards. It then creates a queue of 
      | requests for those individual threads so they can be rearchived. Ignore if an explicit end date is specified, suggesting
      | interest in an exclusive range.
      \-----------------------------------------------------------------------------------------------------------------------*/
      if (since.HasValue && !until.HasValue) {
        dynamic postIndex = await client.GetTaskAsync<dynamic>(updatePostPath);
        foreach (dynamic post in postIndex.data) {

        //Determine if post is newer; if it is, it will be caught in the next set of queries
          if (DateTime.Parse(post.created_time) > since.Value) continue;

        //Get id
          string fullId = post.id;
          if (fullId.IndexOf("_") >= 0) {
            fullId = fullId.Substring(fullId.IndexOf("_") + 1);
          }
          long id = Int64.Parse(fullId);

        //Determine if post has been update recently
          if (DateTime.Parse(post.updated_time) < since.Value) continue;

        //Determine if post is already known
          if (ArchiveManager.StorageProvider.PostExistsAsync(groupId, id).Result) {
          // ### NOTE JJC090715: This chokes on legacy schema; need to determine which fields are inappropriate for older posts.
          }

        //Add the post to a queue to be downloaded
          taskRunner.Add(client.GetTaskAsync<dynamic>(post.id + "?" + groupQuerystring));

        }
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
        if (posts.data.Count > postLimit * 0.8) {
          groupPath = posts.paging.next;
        }
      }

      /*------------------------------------------------------------------------------------------------------------------------
      | Process queue of updated comments
      \-----------------------------------------------------------------------------------------------------------------------*/
      while (taskRunner.Count > 0) {
        Task<dynamic> getPostTask = await Task.WhenAny(taskRunner);
        taskRunner.Remove(getPostTask);
        postsCollection.Add(await getPostTask);
      }

      /*------------------------------------------------------------------------------------------------------------------------
      | Return the results
      \-----------------------------------------------------------------------------------------------------------------------*/
      return postsCollection;

    }

    /*==========================================================================================================================
    | METHOD: GET POST (ASYNCHRONOUS)
    \-------------------------------------------------------------------------------------------------------------------------*/
    /// <summary>
    ///   Retrieves a single post asynchronously from the backend service based on the unique key identifier.
    /// </summary>
    /// <param name="groupId">The unique key identifier for the group that the post exists in.</param>
    /// <param name="postId">The unique identifier for the post.</param>
    public override async Task<dynamic> GetPostAsync(long groupId, long postId) {
      return await GetClient().GetTaskAsync("me/groups/" + groupId + "/" + postId);
    }

  } //Class
} //Namespace
