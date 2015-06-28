/*==============================================================================================================================
| Author        Jeremy Caney (Jeremy@Caney.net)
| Client        Skepti-Forum Project
| Project       Forum Archive
\=============================================================================================================================*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Facebook;

namespace SkeptiForum.Archive {

  /*============================================================================================================================
  | CLASS: FACEBOOK GROUP COLLECTION
  \---------------------------------------------------------------------------------------------------------------------------*/
  /// <summary>
  ///   Provides a collection of <see cref="SkeptiForum.Archive.FacebookGroup"/> objects. Also includes methods for loading 
  ///   both groups and group posts from the Facebook Graph API.
  /// </summary>
  public class FacebookGroupCollection : KeyedCollection<long, FacebookGroup> {

    /*==========================================================================================================================
    | PRIVATE VARIABLES
    \-------------------------------------------------------------------------------------------------------------------------*/
    private     string          _filter          = "Skepti-Forum";
    private     bool            _publicOnly      = true;
    private     string          _fields          = "id,name,privacy";
    private     int             _limit           = 750;

    /*==========================================================================================================================
    | CONSTRUCTOR
    \-------------------------------------------------------------------------------------------------------------------------*/
    /// <summary>
    ///   Initializes a new instance of the `FacebookGroupCollection` class. Will automatically load groups from the Facebook
    ///   Graph API using the <see cref="LoadGroups(bool?, string)" /> method. 
    /// </summary>
    /// <param name="publicOnly">
    ///   Determines if only public groups should be listed. Set to false to load closed and private groups; be aware that this 
    ///   may introduce potential privacy concerns if also loading posts. Defaults to true.
    /// </param>
    /// <param name="filter">
    ///   Sets the filter to use in evaluating whether or not to include a group in the collection. Defaults to "Skepti-Forum",
    ///   meaning all groups containing the words "Skepti-Forum" will be included. This is intended to exclude groups not 
    ///   affiliated with the Skepti-Forum project.
    /// </param>
    public FacebookGroupCollection(bool publicOnly = true, string filter = "Skepti-Forum") : base() {
      _publicOnly = publicOnly;
      _filter = filter;
      LoadGroups();
    }

    /*==========================================================================================================================
    | PROPERTY: PUBLIC ONLY
    \-------------------------------------------------------------------------------------------------------------------------*/
    /// <summary>
    ///   Determines if only public groups should be listed. The property is read only, and can only be set via the constructor.
    /// </summary>
    public bool PublicOnly {
      get {
        return _publicOnly;
      }
    }
    public string Filter {
      get {
        return _filter;
      }
    }

    /*==========================================================================================================================
    | METHOD: CONTAINS
    \-------------------------------------------------------------------------------------------------------------------------*/
    /// <summary>
    ///   Provides an overload to the out-of-the-box <see cref="Collection{T}.Contains(T)"/> method to support searching by a
    ///   group's name, as opposed to the group's numeric identifier (which is the default index).
    /// </summary>
    /// <param name="name">The key name of the group, used for the Facebook URL (e.g., "GMOSF").</param>
    /// <returns>True of a group with the provided <paramref name="name"/> exists, otherwise false.</returns>
    public bool Contains(string name) {
      foreach (FacebookGroup group in Items) {
        if (group.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase)) return true;
      }
      return false;
    }

    /*==========================================================================================================================
    | INDEXER: STRING
    \-------------------------------------------------------------------------------------------------------------------------*/
    /// <summary>
    ///   Provides an overload to the out-of-the-box <see cref="Collection{T}.Items"/> indexer to support lookup up a group by 
    ///   its name, as opposed to the group's numeric identifier (which is the default indexer).
    /// </summary>
    /// <param name="name">The key name of the group, used for the Facebook URL (e.g., "GMOSF").</param>
    public FacebookGroup this[string name] {
      get {
        foreach (FacebookGroup group in Items) {
          if (group.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase)) return group;
        }
        throw new KeyNotFoundException("A group with the key '" + name + " could not be found.");
      }
    }

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
    public void LoadGroups(bool? publicOnly = null, string filter = "Skepti-Forum") {

      /*------------------------------------------------------------------------------------------------------------------------
      | Establish defaults
      \-----------------------------------------------------------------------------------------------------------------------*/
      publicOnly                = publicOnly ?? _publicOnly;
      filter                    = filter ?? _filter;

      /*------------------------------------------------------------------------------------------------------------------------
      | Establish variables
      \-----------------------------------------------------------------------------------------------------------------------*/
      var client                = Facebook.Client;
      dynamic groups            = client.Get("me/groups?limit=" + _limit + "&fields=" + _fields);

      /*------------------------------------------------------------------------------------------------------------------------
      | Loop through each group and add it to the collection
      \-----------------------------------------------------------------------------------------------------------------------*/
      foreach (dynamic group in groups.data) {
        if (!String.IsNullOrEmpty(filter) && !group.name.Contains(Filter)) continue;
        if (publicOnly == true && !group.privacy.Equals("open", StringComparison.InvariantCultureIgnoreCase)) continue;
        FacebookGroup facebookGroup = new FacebookGroup(group);
        this.Add(facebookGroup);
      }
    }

    /*==========================================================================================================================
    | METHOD: LOAD POSTS (ASYNCHRONOUS)
    \-------------------------------------------------------------------------------------------------------------------------*/
    /// <summary>
    ///   For each <see cref="SkeptiForum.Archive.FacebookGroup" /> in the collection, asynchronously returns a list of posts.
    /// </summary>
    /// <returns>A collection of collections representing, respectively, groups and posts.</returns>
    public async Task<Collection<Collection<dynamic>>> GetPostsAsync() {

      /*------------------------------------------------------------------------------------------------------------------------
      | Establish variables
      \-----------------------------------------------------------------------------------------------------------------------*/
      var taskRunner            = new Collection<Task<Collection<dynamic>>>();
      var postsCollection       = new Collection<Collection<dynamic>>();

      /*------------------------------------------------------------------------------------------------------------------------
      | For each group, begin querying Facebook
      \-----------------------------------------------------------------------------------------------------------------------*/
      foreach (FacebookGroup group in this) {
        taskRunner.Add(group.GetPostsAsync());
      //Collection<dynamic> postCollection = await group.GetPostsAsync();
      //postsCollection.Add(postCollection);
      }

      /*------------------------------------------------------------------------------------------------------------------------
      | When each request finishes, process and then wait for the next request
      \-----------------------------------------------------------------------------------------------------------------------*/
      while (taskRunner.Count > 0) {
        Task<Collection<dynamic>> getPostsTask = await Task.WhenAny(taskRunner);
        taskRunner.Remove(getPostsTask);
        var postCollection = await getPostsTask;
        postsCollection.Add(postCollection);
      }

      /*------------------------------------------------------------------------------------------------------------------------
      | Return results
      \-----------------------------------------------------------------------------------------------------------------------*/
      return postsCollection;

    }

    /*==========================================================================================================================
    | METHOD: GET KEY FOR ITEM
    \-------------------------------------------------------------------------------------------------------------------------*/
    /// <summary>
    ///   Sets the <see cref="FacebookGroup.Id"/> property as the key for the <see cref="FacebookGroupCollection" /> class.
    /// </summary>
    /// <param name="item">A reference to the item in the collection that the key will be retrieved from.</param>
    /// <returns>The key to use for the collection.</returns>
    protected override long GetKeyForItem(FacebookGroup item) {
      return item.Id;
    }

  } //Class
} //Namespace
