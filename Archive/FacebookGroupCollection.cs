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
using Newtonsoft.Json;

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

    /*==========================================================================================================================
    | CONSTRUCTOR
    \-------------------------------------------------------------------------------------------------------------------------*/
    /// <summary>
    ///   Initializes a new instance of the `FacebookGroupCollection` class. 
    /// </summary>
    public FacebookGroupCollection() : base() {
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
    | FACTORY METHOD: CREATE FROM STORAGE PROVIDER (ASYNCHRONOUS)
    \-------------------------------------------------------------------------------------------------------------------------*/
    /// <summary>
    ///   
    /// </summary>
    /// <returns></returns>
    public static async Task<FacebookGroupCollection> CreateFromStorageProvider() {
      return await ArchiveManager.StorageProvider.GetGroupsAsync();
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
        taskRunner.Add(ArchiveManager.DataProvider.GetPostsAsync(group.Id));
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
