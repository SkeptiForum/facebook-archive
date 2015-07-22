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
using System.Linq;

namespace SkeptiForum.Archive {

  /*============================================================================================================================
  | CLASS: FACEBOOK GROUP
  \---------------------------------------------------------------------------------------------------------------------------*/
  /// <summary>
  ///   Represents a local reference to a Facebook group. Includes stored metadata representing the group's identifier and name,
  ///   as well as methods for retrieving posts from the group.
  /// </summary>
  /// <remarks>
  ///   Data and storage access is not actually performed by this class. Instead, methods are provided as convenient shortcuts 
  ///   to the configured <see cref="ArchiveManager.DataProvider"/> and <see cref="ArchiveManager.StorageProvider"/>.
  /// </remarks>
  public class FacebookGroup {

    /*==========================================================================================================================
    | PRIVATE VARIABLES
    \-------------------------------------------------------------------------------------------------------------------------*/
    private long        _id;
    private string      _key;
    private string      _name;
    private int         _postCount = -1;
    private bool        _isPublic = false;
    private DateTime    _lastArchived = DateTime.MinValue;
    private DateTime    _lastIndexed = DateTime.MinValue;

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
    ///   Gets the unique string identifier associated with a Facebook group. 
    /// </summary>
    /// <remarks>
    ///   If set, this is used by Facebook for the group's URL. For instance, the <see cref="Name"/> for the "GMO Skepti-Forum" 
    ///   group is "GMOSF". This value can only be set via the constructor.
    /// </remarks>
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
    | PROPERTY: IS PUBLIC?
    \-------------------------------------------------------------------------------------------------------------------------*/
    /// <summary>
    ///   Gets or sets whether or not the group is public; this should be set to false if the group is "Closed" or "Secret".
    /// </summary>
    public bool IsPublic {
      get {
        return _isPublic;
      }
      set {
        _isPublic = value;
      }
    }

    /*==========================================================================================================================
    | PROPERTY: POST COUNT
    \-------------------------------------------------------------------------------------------------------------------------*/
    /// <summary>
    ///   Gets or sets the number of posts associated with the current group. 
    /// </summary>
    /// <remarks>
    ///   The count is based on the number of JSON files located in the associated storage location for the group. The value may 
    ///   be reset manually, e.g., after downloading files.
    /// </remarks>
    public int PostCount {
      get {
        if (_postCount < 0) {
          _postCount = ArchiveManager.StorageProvider.GetPostsAsync(Id).Result.Count();
        }
        return _postCount;
      }
      set {
        _postCount = value;
      }
    }

    /*==========================================================================================================================
    | PROPERTY: LAST ARCHIVED
    \-------------------------------------------------------------------------------------------------------------------------*/
    /// <summary>
    ///   Gets or sets the time that the group was previously archived. 
    /// </summary>
    /// <remarks>
    ///   When setting the last archived date, the <see cref="PostCount"/> property is reset. This ensures that the value is 
    ///   dynamically updated the next time the property is called.
    /// </remarks>
    public DateTime LastArchived {
      get {
        return _lastArchived;
      }
      set {
        _lastArchived = value;
        _postCount = -1;
      }
    }

    /*==========================================================================================================================
    | PROPERTY: LAST INDEXED
    \-------------------------------------------------------------------------------------------------------------------------*/
    /// <summary>
    ///   Gets or sets the time that the group was previously indexed. 
    /// </summary>
    public DateTime LastIndexed {
      get {
        return _lastIndexed;
      }
      set {
        _lastIndexed = value;
      }
    }

    /*==========================================================================================================================
    | METHOD: GET POSTS (ASYNCHRONOUS)
    \-------------------------------------------------------------------------------------------------------------------------*/
    /// <summary>
    ///   Asynchronously retrieves all posts and related comments associated with the group from the Facebook Graph API. 
    /// </summary>
    /// <remarks>
    ///   This method will continue requesting posts from the API via paging until there are no more posts available.
    /// </remarks>
    /// <param name="since">The (optional) start date to pull records from.</param>
    /// <param name="until">The (optional) end date to pull records up to.</param>
    /// <returns>A collection of dynamic objects, each representing the JSON response from the Facebook Graph API.</returns>
    public async Task<Collection<dynamic>> GetPostsAsync(DateTime? since = null, DateTime? until = null) {
      return await ArchiveManager.DataProvider.GetPostsAsync(Id, since, until);
    }

    /*==========================================================================================================================
    | METHOD: INDEX GROUP (ASYNCHRONOUS)
    \-------------------------------------------------------------------------------------------------------------------------*/
    /// <summary>
    ///   Retrieves all posts from the configured <see cref="ArchiveManager.StorageProvider"/> and ensures that each post and 
    ///   its comments are added to the reporting provider.
    /// </summary>
    /// <param name="since">The (optional) start date to index records from.</param>
    /// <param name="until">The (optional) end date to index records up to.</param>
    public async Task IndexPostsAsync(DateTime? since = null, DateTime? until = null) {
      await ArchiveManager.ReportingProvider.IndexPostsAsync(Id, since, until);
    }


  } //Class
} //Namespace
