/*==============================================================================================================================
| Author        Jeremy Caney (Jeremy@Caney.net)
| Client        Skepti-Forum Project
| Project       Forum Archive
\=============================================================================================================================*/
using System;
using System.Collections.ObjectModel;
using Facebook;
using System.Threading.Tasks;

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
    private string      _name;
    private int         _limit            = 50;
    private string      _fields           = "id,from,to,message,message_tags,name,object_id,picture,properties,shares,source,"
      +                                      "caption,description,link,story,story_tags,status_type,type,created_time,is_expired,"
      +                                      "likes.limit(500),comments.limit(750){id,from,message,message_tags,created_time,"
      +                                      "like_count,attachment}";

    /*==========================================================================================================================
    | CONSTRUCTOR
    \-------------------------------------------------------------------------------------------------------------------------*/
    /// <summary>
    ///   Initializes a new instance of the <see cref="FacebookGroup"/> class based on the group's identifier(s). 
    /// </summary>
    /// <param name="id">
    ///   The unique numeric identifier associated with a Facebook group.
    /// </param>
    /// <param name="name">
    ///   The unique string identifier associated with a Facebook group. If set, this is used by Facebook for the group's URL.
    ///   For instance, the <see cref="Name"/> for the "GMO Skepti-Forum" group is "GMOSF".
    /// </param>
    public FacebookGroup(long id, string name = null) {
      _id = id;
      _name = name;
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
    | PROPERTY: NAME
    \-------------------------------------------------------------------------------------------------------------------------*/
    /// <summary>
    ///   Gets the unique string identifier associated with a Facebook group. If set, this is used by Facebook for the group's 
    ///   URL. For instance, the <see cref="Name"/> for the "GMO Skepti-Forum" group is "GMOSF". This value can only be set via 
    ///   the constructor.
    /// </summary>
    public string Name {
      get {
        return _name;
      }
    }

    /*==========================================================================================================================
    | METHOD: GET POSTS
    \-------------------------------------------------------------------------------------------------------------------------*/
    /// <summary>
    ///   Retrives all posts and related comments associated with the group from the Facebook Graph API. Will continue 
    ///   requesting posts from the API via paging until there are no more posts available.
    /// </summary>
    /// <returns>A collection of dynamic objects, each representing the JSON response from the Facebook Graph API.</returns>
    public Collection<dynamic> GetPosts() {

      /*------------------------------------------------------------------------------------------------------------------------
      | Establish variables
      \-----------------------------------------------------------------------------------------------------------------------*/
      var postsCollection       = new Collection<dynamic>();
      var client                = Facebook.Client;
      var groupPath             = Id + "/feed?limit=" + _limit + "&fields=" + _fields;

      /*------------------------------------------------------------------------------------------------------------------------
      | Query Facebook's Graph API for posts
      >-------------------------------------------------------------------------------------------------------------------------
      | ### NOTE JJC200615: Due to the amount of data being requested from Facebook, some groups may require multiple requests.
      | To facilitate this, this method continues to query the Facebook API while (it appears) there are more results. This is
      | determined by comparing the limit to the number of returned results; if they are the same, the method assumes there are 
      | more results, and signals this by setting the `groupPath` parameter to the next page URL returned by Facebook.
      \-----------------------------------------------------------------------------------------------------------------------*/
      while (!String.IsNullOrEmpty(groupPath)) {
        dynamic posts;
        try {
          posts = client.Get(groupPath);
        }
        catch {
          throw new Exception("Error from Facebook while loading data for " + Name + " (" + Id + ") with the following query: '" + groupPath + "'");
        }
        groupPath = "";
        if (posts.data.Count == _limit) {
          groupPath = posts.paging.next;
        }
        foreach (dynamic post in posts.data) {
          postsCollection.Add(post);
        }
      }

      /*------------------------------------------------------------------------------------------------------------------------
      | Return the results
      \-----------------------------------------------------------------------------------------------------------------------*/
      return postsCollection;

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

      /*------------------------------------------------------------------------------------------------------------------------
      | Establish variables
      \-----------------------------------------------------------------------------------------------------------------------*/
      var client                = Facebook.Client;
      var postsCollection       = new Collection<dynamic>();
      var groupPath             = Id + "/feed?limit=" + _limit + "&fields=" + _fields;

      /*------------------------------------------------------------------------------------------------------------------------
      | Asynchronously query Facebook's Graph API for posts
      >-------------------------------------------------------------------------------------------------------------------------
      | ### NOTE JJC200615: Due to the amount of data being requested from Facebook, some groups may require multiple requests.
      | To facilitate this, this method continues to query the Facebook API while (it appears) there are more results. This is
      | determined by comparing the limit to the number of returned results; if they are the same, the method assumes there are 
      | more results, and signals this by setting the `groupPath` parameter to the next page URL returned by Facebook.
      \-----------------------------------------------------------------------------------------------------------------------*/
      while (!String.IsNullOrEmpty(groupPath)) {
        dynamic posts;
        try {
          posts = await client.GetTaskAsync<dynamic>(groupPath);
        }
        catch (Exception ex) {
          throw new Exception("Error from Facebook while loading data for " + Name + " (" + Id + ") with the following query: '" + groupPath + "'. The error message was '" + ex.Message + "'.");
        }
        foreach (dynamic post in posts.data) {
          postsCollection.Add(post);
        }
        groupPath = "";
        if (posts.data.Count == _limit) {
          groupPath = posts.paging.next;
        }
      }

      /*------------------------------------------------------------------------------------------------------------------------
      | Return the results
      \-----------------------------------------------------------------------------------------------------------------------*/
      return postsCollection;

    }

  } //Class
} //Namespace
