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
    public async override Task SetGroupsAsync(FacebookGroupCollection groups) {
      var groupConfigPath = HttpContext.Current.Server.MapPath(ArchiveManager.Configuration.StorageDirectory + "/Groups.json");
      byte[] encodedText = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(groups));

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
    | METHOD: GET POST (ASYNCHRONOUS)
    \-------------------------------------------------------------------------------------------------------------------------*/
    /// <summary>
    ///   
    /// </summary>
    public async override Task<dynamic> GetPostAsync(long groupId, long postId) {
      var postPath = ArchiveManager.Configuration.StorageDirectory + "/" + groupId + "/" + groupId + "_" + postId;
      var postMappedPath = HttpContext.Current.Server.MapPath(postPath);
      var output = new StringBuilder();
      using (
        FileStream sourceStream = new FileStream(
          postMappedPath,
          FileMode.Open,
          FileAccess.Read,
          FileShare.Read,
          bufferSize: 4096,
          useAsync: true
        )
      ) {
        byte[] buffer = new byte[0x1000];
        int numRead;
        while ((numRead = await sourceStream.ReadAsync(buffer, 0, buffer.Length)) != 0) {
          string text = Encoding.Unicode.GetString(buffer, 0, numRead);
          output.Append(text);
        }
      }
      return output.ToString();
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
      var files = directory.GetFiles("*.json");
      return files.ToList<FileInfo>();
    }

    /*==========================================================================================================================
    | METHOD: SET POST
    \-------------------------------------------------------------------------------------------------------------------------*/
    /// <summary>
    ///   
    /// </summary>
    public async override Task<dynamic> SetPostAsync(dynamic json) {
      throw new NotImplementedException();
    }

  } //Class
} //Namespace
