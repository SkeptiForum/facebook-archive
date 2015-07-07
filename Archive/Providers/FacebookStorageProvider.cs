/*==============================================================================================================================
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
  | CLASS: FACEBOOK STORAGE PROVIDER
  \---------------------------------------------------------------------------------------------------------------------------*/
  public class FacebookStorageProvider : ArchiveStorageProviderBase {

    /*==========================================================================================================================
    | CONSTRUCTOR
    \-------------------------------------------------------------------------------------------------------------------------*/
    /// <summary>
    ///
    /// </summary>
    public FacebookStorageProvider() { }

    /*==========================================================================================================================
    | METHOD: GET GROUPS (ASYNCHRONOUS)
    \-------------------------------------------------------------------------------------------------------------------------*/
    /// <summary>
    ///   
    /// </summary>
    public async override Task<dynamic> GetGroupsAsync() {
      var groupConfigPath = HttpContext.Current.Server.MapPath(ArchiveManager.Configuration.StorageDirectory + "/Groups.json");
      if (!System.IO.File.Exists(groupConfigPath)) {
        return new FacebookGroupCollection();
      }
      else {
        return JsonConvert.DeserializeObject<FacebookGroupCollection>(System.IO.File.ReadAllText(groupConfigPath));
      }
    }

    /*==========================================================================================================================
    | METHOD: SET GROUPS (ASYNCHRONOUS)
    \-------------------------------------------------------------------------------------------------------------------------*/
    /// <summary>
    ///   
    /// </summary>
    public async override Task SetGroupsAsync() {
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
    ///   
    /// </summary>
    public async override Task<bool> PostExistsAsync(long groupId, long postId) {
      var postPath = ArchiveManager.Configuration.StorageDirectory + "/" + groupId + "/" + groupId + "_" + postId + ".json";
      var postMappedPath = HttpContext.Current.Server.MapPath(postPath);
      return File.Exists(postMappedPath);
    }

    /*==========================================================================================================================
    | METHOD: GET POST (ASYNCHRONOUS)
    >---------------------------------------------------------------------------------------------------------------------------
    | ### TODO JJC060715: Having unexpected issues with asynchronous reading, likely due to how the file is encoded. Will need
    | to revisit this later. For now, using a basic synchronous call (but keeping the async signature to maintain 
    | API compatibility).
    \-------------------------------------------------------------------------------------------------------------------------*/
    /// <summary>
    ///   
    /// </summary>
    public async override Task<dynamic> GetPostAsync(long groupId, long postId) {

      var postPath = ArchiveManager.Configuration.StorageDirectory + "/" + groupId + "/" + groupId + "_" + postId + ".json";
      var postMappedPath = HttpContext.Current.Server.MapPath(postPath);

      return JObject.Parse(File.ReadAllText(postMappedPath));

      var output = new StringBuilder();
      var buffer = new char[0x1000];
      int numRead;

      using (StreamReader reader = new StreamReader(postMappedPath, Encoding.UTF8)) {
        while ((numRead = await reader.ReadAsync(buffer, 0, buffer.Length)) != 0) {
          output.Append(buffer);
        }
      }

      return JObject.Parse(output.ToString());

    }

    /*==========================================================================================================================
    | METHOD: GET POSTS (ASYNCHRONOUS)
    \-------------------------------------------------------------------------------------------------------------------------*/
    /// <summary>
    ///   
    /// </summary>
    public async override Task<List<FileInfo>> GetPostsAsync(long groupId) {
      var postPath = ArchiveManager.Configuration.StorageDirectory + "/" + groupId + "/";
      var postMappedPath = HttpContext.Current.Server.MapPath(postPath);
      var directory = new DirectoryInfo(postMappedPath);
      if (!directory.Exists) return new List<FileInfo>();
      var files = directory.GetFiles("*.json");
      return files.ToList<FileInfo>();
    }

    /*==========================================================================================================================
    | METHOD: SET POST
    \-------------------------------------------------------------------------------------------------------------------------*/
    /// <summary>
    ///   
    /// </summary>
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
    }

  } //Class
} //Namespace
