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
  public abstract class ArchiveDataProviderBase : ProviderBase {

    /*==========================================================================================================================
    | METHOD: GET LONG LIVED TOKEN
    \-------------------------------------------------------------------------------------------------------------------------*/
    /// <summary>
    ///   When a user authenticated using the Facebook JavaScript SDK, a short-lived (session) access token is returned. This is 
    ///   not sufficient for continued use of the application. To mitigate this, client code may request that the access token
    ///   be replaced with a long-term access token.
    /// </summary>
    /// <param name="accessToken">The short-lived (session) access token assigned by the Facebook JavaScript SDK.</param>
    public string GetLongLivedToken(string accessToken) {
      long expiry;
      return GetLongLivedToken(accessToken, out expiry);
    }

    /*==========================================================================================================================
    | METHOD: GET LONG LIVED TOKEN (WITH EXPIRY)
    \-------------------------------------------------------------------------------------------------------------------------*/
    /// <summary>
    ///   
    /// </summary>
    public abstract string GetLongLivedToken(string accessToken, out long expiry);

    /*==========================================================================================================================
    | METHOD: LOAD GROUPS
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
    public async Task<Collection<dynamic>> GetGroupsAsync() {
      return await GetGroupsAsync(null, null);
    }

    /*==========================================================================================================================
    | METHOD: GET POSTS (ASYNCHRONOUS)
    \-------------------------------------------------------------------------------------------------------------------------*/
    /// <summary>
    ///   
    /// </summary>
    public async Task<Collection<dynamic>> GetPostsAsyc(string groupId, DateTime? since = null) {
      FacebookGroup group = ArchiveManager.Groups[groupId];
      return await GetPostsAsync(group.Id, since);
    }

    /*==========================================================================================================================
    | METHOD: GET POSTS (OVERLOAD)
    \-------------------------------------------------------------------------------------------------------------------------*/
    /// <summary>
    ///   
    /// </summary>
    public abstract Task<Collection<dynamic>> GetPostsAsync(long groupId, DateTime? since = null);

    /*==========================================================================================================================
    | METHOD: GET GROUPS (ASYNCHRONOUS)
    \-------------------------------------------------------------------------------------------------------------------------*/
    /// <summary>
    ///   
    /// </summary>
    public abstract Task<Collection<dynamic>> GetGroupsAsync(bool? publicOnly = null, string filter = "Skepti-Forum");

    /*==========================================================================================================================
    | METHOD: GET POST (ASYNCHRONOUS)
    \-------------------------------------------------------------------------------------------------------------------------*/
    /// <summary>
    ///   
    /// </summary>
    public async Task<dynamic> GetPostAsync(string groupId, long postId) {
      FacebookGroup group = ArchiveManager.Groups[groupId];
      return await GetPostAsync(group.Id, postId);
    }

    /*==========================================================================================================================
    | METHOD: GET POST (ASYNCHRONOUS)
    \-------------------------------------------------------------------------------------------------------------------------*/
    /// <summary>
    ///   
    /// </summary>
    public abstract Task<dynamic> GetPostAsync(long groupId, long postId);

  }

}