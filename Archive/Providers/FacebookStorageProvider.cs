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
    \-------------------------------------------------------------------------------------------------------------------------*/
    /// <summary>
    ///   
    /// </summary>
    public async override Task<dynamic> GetPostAsync(long groupId, long postId) {

      var postPath = ArchiveManager.Configuration.StorageDirectory + "/" + groupId + "/" + groupId + "_" + postId + ".json";
      var postMappedPath = HttpContext.Current.Server.MapPath(postPath);

      string json; // = File.ReadAllText(postMappedPath);

      using (StreamReader reader = File.OpenText(postMappedPath)) {
        json = await reader.ReadToEndAsync();
      }

      return JObject.Parse(json);


    }

    /*==========================================================================================================================
    | METHOD: GET POSTS (ASYNCHRONOUS)
    \-------------------------------------------------------------------------------------------------------------------------*/
    /// <summary>
    ///   
    /// </summary>
    public async override Task<IEnumerable<FileInfo>> GetPostsAsync(long groupId) {
      var postPath = ArchiveManager.Configuration.StorageDirectory + "/" + groupId + "/";
      var postMappedPath = HttpContext.Current.Server.MapPath(postPath);
      var directory = new DirectoryInfo(postMappedPath);
      if (!directory.Exists) return new List<FileInfo>();
      return directory.EnumerateFiles("*.json");
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

      File.SetLastWriteTime(filePath, (DateTime)json.updated_time);

    }

  } //Class
} //Namespace
