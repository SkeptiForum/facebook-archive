/*==============================================================================================================================
| Author        Jeremy Caney (Jeremy@Caney.net)
| Client        Skepti-Forum Project
| Project       Forum Archive
\=============================================================================================================================*/
using System;
using System.Collections.ObjectModel;
using Facebook;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SkeptiForum.Archive {

  /*============================================================================================================================
  | CLASS: FACEBOOK GROUP
  \---------------------------------------------------------------------------------------------------------------------------*/
  /// <summary>
  ///   Represents a local reference to a Facebook group. Includes stored metadata representing the group's identifier and name,
  ///   as well as methods for retrieving posts from the group.
  /// </summary>
  public class FacebookGroup {

    /*==========================================================================================================================
    | PRIVATE VARIABLES
    \-------------------------------------------------------------------------------------------------------------------------*/
    private long        _id;
    private string      _key;
    private string      _name;

    /*==========================================================================================================================
    | CONSTRUCTOR
    \-------------------------------------------------------------------------------------------------------------------------*/
    /// <summary>
    ///   Initializes a new instance of the <see cref="FacebookGroup"/> class based on the group's identifier(s). 
    /// </summary>
    /// <param name="id">
    ///   The unique numeric identifier associated with a Facebook group.
    /// </param>
    /// <param name="key">
    ///   The unique string identifier associated with a Facebook group. If set, this is used by Facebook for the group's URL.
    ///   For instance, the <see cref="Key"/> for the "GMO Skepti-Forum" group is "GMOSF".
    /// </param>
    [JsonConstructor]
    public FacebookGroup(long id, string key = null) {
      _id = id;
      _key = key;
    }

    /*==========================================================================================================================
    | CONSTRUCTOR
    \-------------------------------------------------------------------------------------------------------------------------*/
    /// <summary>
    ///   Initializes a new instance of the <see cref="FacebookGroup"/> class based on the response from the <see 
    ///   cref="FacebookClient.Get(string)"/> operation (such as that utilized by the <see 
    ///   cref="FacebookGroupCollection.LoadGroups(bool?, string)"/> method). 
    /// </summary>
    /// <param name="group">
    ///   The dynamic object representing the JSON response from the Facebook Graph API.
    /// </param>
    public FacebookGroup(dynamic group) {
      _id = Convert.ToInt64(group.id);
      _name = group.name;
    }

    /*==========================================================================================================================
    | PROPERTY: IDENTIFIER
    \-------------------------------------------------------------------------------------------------------------------------*/
    /// <summary>
    ///   Gets the unique numeric identifier associated with a Facebook group. This value can only be set via the constructor.
    /// </summary>
    public long Id {
      get {
        return _id;
      }
    }

    /*==========================================================================================================================
    | PROPERTY: KEY
    \-------------------------------------------------------------------------------------------------------------------------*/
    /// <summary>
    ///   Gets the unique string identifier associated with a Facebook group. If set, this is used by Facebook for the group's 
    ///   URL. For instance, the <see cref="Name"/> for the "GMO Skepti-Forum" group is "GMOSF". This value can only be set via 
    ///   the constructor.
    /// </summary>
    public string Key {
      get {
        return _key;
      }
    }

    /*==========================================================================================================================
    | PROPERTY: NAME
    \-------------------------------------------------------------------------------------------------------------------------*/
    /// <summary>
    ///   Gets or sets the friendly name for the group.
    /// </summary>
    public string Name {
      get {
        return _name;
      }
      set {
        _name = value;
      }
    }

    /*==========================================================================================================================
    | METHOD: GET POSTS (ASYNCHRONOUS)
    \-------------------------------------------------------------------------------------------------------------------------*/
    /// <summary>
    ///   Asynchronously retrieves all posts and related comments associated with the group from the Facebook Graph API. Will 
    ///   continue requesting posts from the API via paging until there are no more posts available.
    /// </summary>
    /// <returns>A collection of dynamic objects, each representing the JSON response from the Facebook Graph API.</returns>
    public async Task<Collection<dynamic>> GetPostsAsync() {
      return await ArchiveManager.DataProvider.GetPostsAsync(Id);
    }

  } //Class
} //Namespace
