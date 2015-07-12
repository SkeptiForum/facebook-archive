/*==============================================================================================================================
| Author        Jeremy Caney (Jeremy@Caney.net)
| Client        Skepti-Forum Project
| Project       Forum Archive
\=============================================================================================================================*/
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration.Provider;
using System.Data;
using System.Threading.Tasks;
using SkeptiForum.Archive.Configuration;

namespace SkeptiForum.Archive.Providers {

  /*============================================================================================================================
  | CLASS: ARCHIVE DATA PROVIDER BASE
  \---------------------------------------------------------------------------------------------------------------------------*/
  /// <summary>
  ///   Data providers are an extensibility point allowing different social networks to be supported. 
  /// </summary>
  /// <remarks>
  ///   The <see cref="ArchiveDataProviderBase"/> provides both a concrete implemention of universal logic as well as an 
  ///   abstract definition of required memebers for each provider to implement. The former largerly focused on overloads and 
  ///   lookup methods that simplify access to the more complex members defined by the concrete provider. 
  /// </remarks>
  public abstract class ArchiveDataProviderBase : ProviderBase {

    /*==========================================================================================================================
    | METHOD: GET LONG LIVED TOKEN
    \-------------------------------------------------------------------------------------------------------------------------*/
    /// <summary>
    ///   Exchanges a short-lived (session) access token with a long-lived (persistent) token.
    /// </summary>
    /// <remarks>
    ///   <para>
    ///     When a user authenticated using the Facebook JavaScript SDK, a short-lived (session) access token is returned. This 
    ///     is not sufficient for continued use of the application. To mitigate this, client code may request that the access 
    ///     token be replaced with a long-term access token. 
    ///   </para>
    /// </remarks>
    /// <param name="accessToken">The short-lived (session) access token assigned by the Facebook JavaScript SDK.</param>
    public string GetLongLivedToken(string accessToken) {
      long expiry;
      return GetLongLivedToken(accessToken, out expiry);
    }

    /*==========================================================================================================================
    | METHOD: GET LONG LIVED TOKEN (WITH EXPIRY)
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
    public abstract string GetLongLivedToken(string accessToken, out long expiry);

    /*==========================================================================================================================
    | METHOD: GET GROUPS
    \-------------------------------------------------------------------------------------------------------------------------*/
    /// <summary>
    ///   Populates the collection with a list of <see cref="SkeptiForum.Archive.FacebookGroup"/> objects based on a query to the 
    ///   Facebook API.
    /// </summary>
    /// <remarks>
    ///   Assumes that the default configured values for <see cref="Configuration.GroupQueryElement.PublicOnly"/> and <see 
    ///   cref="Configuration.GroupQueryElement.Filter"/> will be used.
    /// </remarks>
    public async Task<Collection<dynamic>> GetGroupsAsync() {
      return await GetGroupsAsync(null, null);
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
    public abstract Task<Collection<dynamic>> GetGroupsAsync(bool? publicOnly = null, string filter = "Skepti-Forum");

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
    public async Task<Collection<dynamic>> GetPostsAsyc(string groupKey, DateTime? since = null, DateTime? until = null) {
      FacebookGroup group = ArchiveManager.Groups[groupKey];
      return await GetPostsAsync(group.Id, since, until);
    }

    /*==========================================================================================================================
    | METHOD: GET POSTS (OVERLOAD)
    \-------------------------------------------------------------------------------------------------------------------------*/
    /// <summary>
    ///   Retrieves a collection of posts asynchronously from the backend service, optionally filtering based on a date range 
    ///   based on the unique numeric identifier.
    /// </summary>
    /// <param name="groupId">The unique identifier for the group to query.</param>
    /// <param name="since">The (optional) start date to pull records from.</param>
    /// <param name="until">The (optional) end date to pull records up to.</param>
    public abstract Task<Collection<dynamic>> GetPostsAsync(long groupId, DateTime? since = null, DateTime? until = null);

    /*==========================================================================================================================
    | METHOD: GET POST (ASYNCHRONOUS)
    \-------------------------------------------------------------------------------------------------------------------------*/
    /// <summary>
    ///   Retrieves a single post asynchronously from the backend service based on the unique key identifier.
    /// </summary>
    /// <param name="groupId">The unique key identifier for the group that the post exists in.</param>
    /// <param name="postId">The unique identifier for the post.</param>
    public async Task<dynamic> GetPostAsync(string groupKey, long postId) {
      FacebookGroup group = ArchiveManager.Groups[groupKey];
      return await GetPostAsync(group.Id, postId);
    }

    /*==========================================================================================================================
    | METHOD: GET POST (ASYNCHRONOUS)
    \-------------------------------------------------------------------------------------------------------------------------*/
    /// <summary>
    ///   Retrieves a single post asynchronously from the backend service based on the unique numeric identifier.
    /// </summary>
    /// <param name="groupId">The unique identifier for the group to query.</param>
    /// <param name="postId">The unique identifier for the post.</param>
    public abstract Task<dynamic> GetPostAsync(long groupId, long postId);

  }

}