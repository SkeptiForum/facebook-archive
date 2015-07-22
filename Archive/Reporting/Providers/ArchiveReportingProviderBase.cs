/*==============================================================================================================================
| Author        Jeremy Caney (Jeremy@Caney.net)
| Client        Skepti-Forum Project
| Project       Forum Archive
\=============================================================================================================================*/using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Configuration.Provider;
using System.Data;

namespace SkeptiForum.Archive.Reporting.Providers {

  /*============================================================================================================================
  | CLASS: ARCHIVE REPORTING PROVIDER BASE
  \---------------------------------------------------------------------------------------------------------------------------*/
  /// <summary>
  ///   Reporting providers are an extensibility point allowing different storage locations for reporting data to be supported. 
  /// </summary>
  /// <remarks>
  ///   <para>
  ///     The <see cref="ArchiveReportingProviderBase"/> provides both a concrete implemention of universal logic as well as an 
  ///     abstract definition of required members for each provider to implement. The former largerly focused on overloads and 
  ///     lookup methods that simplify access to the more complex members defined by the concrete provider. 
  ///   </para>
  ///   <para>
  ///     In practice, concrete instances of the reporting provider may use the same storage medium as the <see 
  ///     cref="Archive.Providers.ArchiveStorageProviderBase"/> for the application. And, indeed, the reporting provider will 
  ///     certainly depend on the storage provider. That said, keeping these distinct allows for them to be maintained in 
  ///     different systems, such as the file system for a storage provider, and a SQL database for a reporting provider.
  ///   </para>
  /// </remarks>
  public abstract class ArchiveReportingProviderBase : ProviderBase {

    /*==========================================================================================================================
    | METHOD: INDEX GROUP (ASYNCHRONOUS)
    \-------------------------------------------------------------------------------------------------------------------------*/
    /// <summary>
    ///   For each group configured for the application in <see cref="ArchiveManager.Groups"/>, retrieves all posts from the 
    ///   configured <see cref="ArchiveManager.StorageProvider"/> and ensures that both the post and its comments are added to 
    ///   the reporting provider.
    /// </summary>
    /// <param name="groupKey">The unique key identifier for the group to query.</param>
    public async Task IndexGroupsAsync() {
      var taskRunner = new List<Task>();
      foreach (FacebookGroup group in ArchiveManager.Groups) {
        //taskRunner.Add(IndexGroup(group.Id));
        await IndexPostsAsync(group.Id);
      }
      Task.WaitAll(taskRunner.ToArray());
    }

    /*==========================================================================================================================
    | METHOD: INDEX GROUP (ASYNCHRONOUS)
    \-------------------------------------------------------------------------------------------------------------------------*/
    /// <summary>
    ///   Retrieves all posts from the configured <see cref="ArchiveManager.StorageProvider"/> and ensures that each post and 
    ///   its comments are added to the reporting provider.
    /// </summary>
    /// <param name="groupKey">The unique key identifier for the group to query.</param>
    public async Task IndexPostsAsync(string groupKey, DateTime? since = null, DateTime? until = null) {
      FacebookGroup group = ArchiveManager.Groups[groupKey];
      await IndexPostsAsync(group.Id);
    }

    /*==========================================================================================================================
    | METHOD: GET POSTS (OVERLOAD)
    \-------------------------------------------------------------------------------------------------------------------------*/
    /// <summary>
    ///   Retrieves all posts from the configured <see cref="ArchiveManager.StorageProvider"/> and ensures that each post and 
    ///   its comments are added to the reporting provider.
    /// </summary>
    /// <param name="groupId">The unique identifier for the group to query.</param>
    /// <param name="since">The (optional) start date to index records from.</param>
    /// <param name="until">The (optional) end date to index records up to.</param>
    public abstract Task IndexPostsAsync(long groupId, DateTime? since = null, DateTime? until = null);

  } //Class
} //Namespace
