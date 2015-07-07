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
  public abstract class ArchiveStorageProviderBase : ProviderBase {

    /*==========================================================================================================================
    | METHOD: GET GROUPS
    \-------------------------------------------------------------------------------------------------------------------------*/
    /// <summary>
    ///   
    /// </summary>
    public abstract Task<dynamic> GetGroupsAsync();

    /*==========================================================================================================================
    | METHOD: SET GROUPS
    \-------------------------------------------------------------------------------------------------------------------------*/
    /// <summary>
    ///   
    /// </summary>
    public abstract Task SetGroupsAsync();

    /*==========================================================================================================================
    | METHOD: POST EXISTS
    \-------------------------------------------------------------------------------------------------------------------------*/
    /// <summary>
    ///   
    /// </summary>
    public abstract Task<bool> PostExistsAsync(long groupId, long postId);

    /*==========================================================================================================================
    | METHOD: GET POSTS
    \-------------------------------------------------------------------------------------------------------------------------*/
    /// <summary>
    ///   
    /// </summary>
    public async Task<List<FileInfo>> GetPostsAsync(string groupId) {
      FacebookGroup group = ArchiveManager.Groups[groupId];
      return await GetPostsAsync(group.Id);
    }

    /*==========================================================================================================================
    | METHOD: GET POSTS (OVERLOAD)
    \-------------------------------------------------------------------------------------------------------------------------*/
    /// <summary>
    ///   
    /// </summary>
    public abstract Task<List<FileInfo>> GetPostsAsync(long groupId);

    /*==========================================================================================================================
    | METHOD: GET POST
    \-------------------------------------------------------------------------------------------------------------------------*/
    /// <summary>
    ///   
    /// </summary>
    public async Task<dynamic> GetPostAsync(string groupId, long postId) {
      FacebookGroup group = ArchiveManager.Groups[groupId];
      return await GetPostAsync(group.Id, postId);
    }

    /*==========================================================================================================================
    | METHOD: GET POST
    \-------------------------------------------------------------------------------------------------------------------------*/
    /// <summary>
    ///   
    /// </summary>
    public abstract Task<dynamic> GetPostAsync(long groupId, long postId);

    /*==========================================================================================================================
    | METHOD: SET POST
    \-------------------------------------------------------------------------------------------------------------------------*/
    /// <summary>
    ///   
    /// </summary>
    public abstract Task SetPostAsync(dynamic json);

  }

}