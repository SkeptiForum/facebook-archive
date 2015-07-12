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
using System.IO;
using SkeptiForum.Archive.Configuration;
using System.Threading.Tasks;

namespace SkeptiForum.Archive.Providers {

  /*============================================================================================================================
  | CLASS: ARCHIVE DATA PROVIDER BASE
  \---------------------------------------------------------------------------------------------------------------------------*/
  /// <summary>
  ///   Storage providers are an extensibility point allowing different persistance layers be supported for storing cached data
  ///   from the <see cref="ArchiveDataProviderBase"/> implementations. Concrete versions may store data, for instance, on the 
  ///   file system or in a database.
  /// </summary>
  /// <remarks>
  ///   The <see cref="ArchiveStorageProviderBase"/> provides both a concrete implemention of universal logic as well as an 
  ///   abstract definition of required memebers for each provider to implement. The former largerly focused on overloads and 
  ///   lookup methods that simplify access to the more complex members defined by the concrete provider. 
  /// </remarks>
  public abstract class ArchiveStorageProviderBase : ProviderBase {

    /*==========================================================================================================================
    | METHOD: GET GROUPS
    \-------------------------------------------------------------------------------------------------------------------------*/
    /// <summary>
    ///   Retrieves a list of groups that the current application is configured to use.
    /// </summary>
    public abstract Task<FacebookGroupCollection> GetGroupsAsync();

    /*==========================================================================================================================
    | METHOD: SET GROUPS
    \-------------------------------------------------------------------------------------------------------------------------*/
    /// <summary>
    ///   Saves a list of groups that the current application is configured to use. 
    /// </summary>
    /// <param name="groups">
    ///   An optional collection of groups to save. If left empty, the <see cref="ArchiveManager.Groups"/> collection is 
    ///   assumed. 
    /// </param>
    public abstract Task SetGroupsAsync(FacebookGroupCollection groups = null);

    /*==========================================================================================================================
    | METHOD: POST EXISTS
    \-------------------------------------------------------------------------------------------------------------------------*/
    /// <summary>
    ///   Determines if a post already exists in the storage medium.
    /// </summary>
    /// <param name="groupKey">The unique key identifier for the group to query.</param>
    /// <param name="postId">The unique identifier for the post.</param>
    public async Task<bool> PostExistsAsync(string groupKey, long postId) {
      FacebookGroup group = ArchiveManager.Groups[groupKey];
      return await PostExistsAsync(group.Id, postId);
    }

    /*==========================================================================================================================
    | METHOD: POST EXISTS
    \-------------------------------------------------------------------------------------------------------------------------*/
    /// <summary>
    ///   Determines if a post already exists in the storage medium.
    /// </summary>
    /// <param name="groupId">The unique identifier for the group to query.</param>
    /// <param name="postId">The unique identifier for the post.</param>
    public abstract Task<bool> PostExistsAsync(long groupId, long postId);

    /*==========================================================================================================================
    | METHOD: GET POSTS (ASYNCHRONOUS)
    \-------------------------------------------------------------------------------------------------------------------------*/
    /// <summary>
    ///   Asynchronously retrieves an enumerable list of <see cref="System.IO.FileInfo"/> references representing individual 
    ///   posts associated with a group.
    /// </summary>
    /// <param name="groupKey">The unique key identifier for the group to query.</param>
    public async Task<IEnumerable<FileInfo>> GetPostsAsync(string groupKey) {
      FacebookGroup group = ArchiveManager.Groups[groupKey];
      return await GetPostsAsync(group.Id);
    }

    /*==========================================================================================================================
    | METHOD: GET POSTS (OVERLOAD)
    >---------------------------------------------------------------------------------------------------------------------------
    | ### TODO JJC120715: This needs to be abstracted to use, for instance, a FacebookPost object (or new base class). As is, 
    | this is tied specifically to the FileSystemStorageProvider and does not provide a suitable abstraction for other future 
    | providers (e.g., a proposed MongoDB provider).
    \-------------------------------------------------------------------------------------------------------------------------*/
    /// <summary>
    ///   Asynchronously retrieves an enumerable list of <see cref="System.IO.FileInfo"/> references representing individual 
    ///   posts associated with a group.
    /// </summary>
    /// <param name="groupId">The unique identifier for the group to query.</param>
    public abstract Task<IEnumerable<FileInfo>> GetPostsAsync(long groupId);

    /*==========================================================================================================================
    | METHOD: GET POST
    \-------------------------------------------------------------------------------------------------------------------------*/
    /// <summary>
    ///   Retrieves an individual post from the storage medium.
    /// </summary>
    /// <param name="groupKey">The unique key identifier for the group to query.</param>
    /// <param name="postId">The unique identifier for the post.</param>
    public async Task<dynamic> GetPostAsync(string groupKey, long postId) {
      FacebookGroup group = ArchiveManager.Groups[groupKey];
      return await GetPostAsync(group.Id, postId);
    }

    /*==========================================================================================================================
    | METHOD: GET POST
    \-------------------------------------------------------------------------------------------------------------------------*/
    /// <summary>
    ///   Retrieves an individual post from the storage medium.
    /// </summary>
    /// <param name="groupId">The unique identifier for the group to query.</param>
    /// <param name="postId">The unique identifier for the post.</param>
    public abstract Task<dynamic> GetPostAsync(long groupId, long postId);

    /*==========================================================================================================================
    | METHOD: SET POST
    \-------------------------------------------------------------------------------------------------------------------------*/
    /// <summary>
    ///   Persists an individual post to the storage medium.
    /// </summary>
    /// <param name="json">A <see cref="Newtonsoft.Json.Linq.JObject"/> representation of the post.</param>
    public abstract Task SetPostAsync(dynamic json);

  }

}