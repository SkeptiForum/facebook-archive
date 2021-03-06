﻿/*==============================================================================================================================
| Author        Jeremy Caney (Jeremy@Caney.net)
| Client        Skepti-Forum Project
| Project       Forum Archive
\=============================================================================================================================*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkeptiForum.Archive.Configuration;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SkeptiForum.Archive.Providers {

  /*============================================================================================================================
  | CLASS: FILE SYSTEM STORAGE PROVIDER
  \---------------------------------------------------------------------------------------------------------------------------*/
  /// <summary>
  ///   Provides a concrete implementation of the <see cref="ArchiveStorageProviderBase"/> configured to persist data to the 
  ///   file system using .NET's out-of-the-box <see cref="File.IO"/> classes. 
  /// </summary>
  public class FileSystemStorageProvider : ArchiveStorageProviderBase {

    /*==========================================================================================================================
    | CONSTRUCTOR
    \-------------------------------------------------------------------------------------------------------------------------*/
    /// <summary>
    ///   Constructs a new instance of the <see cref="FileSystemStorageProvider"/>.
    /// </summary>
    public FileSystemStorageProvider() { }

    /*==========================================================================================================================
    | METHOD: GET GROUPS (ASYNCHRONOUS)
    >---------------------------------------------------------------------------------------------------------------------------
    | ### HACK JJC120715: The ArchiveManager.Groups property cannot call this method asynchronously, and thus calls it with    
    | GetPostAsync(...).Result. This causes the process to hang. This only seems to happen when using ReadToEndAsync(); other
    | asynchronous methods do not exhibit this behavior. To mitigate this, this is currently using the synchronous method, until
    | a long-term fix can be identified.
    \-------------------------------------------------------------------------------------------------------------------------*/
    /// <summary>
    ///   Retrieves a list of groups that the current application is configured to use.
    /// </summary>
    /// <remarks>
    ///   Assumes that the groups will be stored in the configured <see cref="Configuration.ArchiveSection.StorageDirectory"/>
    ///   in a file named "Groups.json".
    /// </remarks>
    public async override Task<FacebookGroupCollection> GetGroupsAsync() {
      var groupConfigPath = HttpContext.Current.Server.MapPath(ArchiveManager.Configuration.StorageDirectory + "/Groups.json");
      if (!System.IO.File.Exists(groupConfigPath)) {
        return new FacebookGroupCollection();
      }
      using (StreamReader reader = File.OpenText(groupConfigPath)) {
        return JsonConvert.DeserializeObject<FacebookGroupCollection>(reader.ReadToEnd());
      }
    }

    /*==========================================================================================================================
    | METHOD: SET GROUPS (ASYNCHRONOUS)
    \-------------------------------------------------------------------------------------------------------------------------*/
    /// <summary>
    ///   Saves a list of groups that the current application is configured to use. 
    /// </summary>
    /// <param name="groups">
    ///   An optional collection of groups to save. If left empty, the <see cref="ArchiveManager.Groups"/> collection is 
    ///   assumed. 
    /// </param>
    public async override Task SetGroupsAsync(FacebookGroupCollection groups = null) {
      var groupConfigPath = HttpContext.Current.Server.MapPath(ArchiveManager.Configuration.StorageDirectory + "/Groups.json");
      byte[] encodedText = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(ArchiveManager.Groups));

      using (FileStream sourceStream = new FileStream(
        groupConfigPath,
        FileMode.Create,
        FileAccess.Write,
        FileShare.None,
        bufferSize: 4096,
        useAsync: true
        )) {
        await sourceStream.WriteAsync(encodedText, 0, encodedText.Length);
      }
    }

    /*==========================================================================================================================
    | METHOD: POST EXISTS
    \-------------------------------------------------------------------------------------------------------------------------*/
    /// <summary>
    ///   Determines if a post already exists in the storage medium.
    /// </summary>
    /// <param name="groupId">The unique identifier for the group to query.</param>
    /// <param name="postId">The unique identifier for the post.</param>
    public async override Task<bool> PostExistsAsync(long groupId, long postId) {
      var postPath = ArchiveManager.Configuration.StorageDirectory + "/" + groupId + "/" + groupId + "_" + postId + ".json";
      var postMappedPath = HttpContext.Current.Server.MapPath(postPath);
      return File.Exists(postMappedPath);
    }

    /*==========================================================================================================================
    | METHOD: GET POSTS (ASYNCHRONOUS)
    \-------------------------------------------------------------------------------------------------------------------------*/
    /// <summary>
    ///   Asynchronously retrieves an enumerable list of <see cref="System.IO.FileInfo"/> references representing individual 
    ///   posts associated with a group.
    /// </summary>
    /// <param name="groupId">The unique identifier for the group to query.</param>
    /// <param name="since">The (optional) start date to pull records from.</param>
    /// <param name="until">The (optional) end date to pull records up to.</param>
    public async override Task<IEnumerable<FileInfo>> GetPostsAsync(long groupId, DateTime? since = null, DateTime? until = null) {

      var postPath = ArchiveManager.Configuration.StorageDirectory + "/" + groupId + "/";
      var postMappedPath = HttpContext.Current.Server.MapPath(postPath);
      var directory = new DirectoryInfo(postMappedPath);
      var files = new List<FileInfo>();

      if (!directory.Exists) return files;

      foreach (FileInfo file in directory.GetFiles("*.json")) {
        if (since != null && file.LastAccessTimeUtc < since) continue;
        if (until != null && file.LastWriteTimeUtc > until) continue;
        files.Add(file);
      };

      return files.AsEnumerable<FileInfo>();

    }

    /*==========================================================================================================================
    | METHOD: GET POST (ASYNCHRONOUS)
    \-------------------------------------------------------------------------------------------------------------------------*/
    /// <summary>
    ///   Retrieves an individual post from the storage medium.
    /// </summary>
    /// <param name="groupId">The unique identifier for the group to query.</param>
    /// <param name="postId">The unique identifier for the post.</param>
    public async override Task<dynamic> GetPostAsync(long groupId, long postId) {

      var postPath = ArchiveManager.Configuration.StorageDirectory + "/" + groupId + "/" + groupId + "_" + postId + ".json";
      var postMappedPath = HttpContext.Current.Server.MapPath(postPath);

      string json; // = File.ReadAllText(postMappedPath);

      using (StreamReader reader = File.OpenText(postMappedPath)) {
        //json = await reader.ReadToEndAsync();
        json = reader.ReadToEnd();
      }

      return JObject.Parse(json);

    }

    /*==========================================================================================================================
    | METHOD: SET POST
    \-------------------------------------------------------------------------------------------------------------------------*/
    /// <summary>
    ///   Persists an individual post to the storage medium.
    /// </summary>
    /// <param name="json">A <see cref="Newtonsoft.Json.Linq.JObject"/> representation of the post.</param>
    public async override Task SetPostAsync(dynamic json) {

      var directoryPath = HttpContext.Current.Server.MapPath(ArchiveManager.Configuration.StorageDirectory + "/" + json.to.data[0].id + "/");
      var filePath = directoryPath + "\\" + json.id + ".json";

      byte[] encodedText = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(json));

      if (!Directory.Exists(directoryPath)) {
        Directory.CreateDirectory(directoryPath);
      }

      using (FileStream sourceStream = new FileStream(
        filePath,
        FileMode.Create,
        FileAccess.Write,
        FileShare.None,
        bufferSize: 4096,
        useAsync: true
        )) {
        await sourceStream.WriteAsync(encodedText, 0, encodedText.Length);
      }

      File.SetLastWriteTime(filePath, DateTime.Parse(json.updated_time?? json.created_time?? DateTime.Now.ToString()));

    }

  } //Class
} //Namespace
