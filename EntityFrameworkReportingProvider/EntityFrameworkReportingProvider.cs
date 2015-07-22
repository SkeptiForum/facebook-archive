using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkeptiForum.Archive;
using SkeptiForum.Archive.Providers;
using SkeptiForum.Archive.Reporting;
using SkeptiForum.Archive.Reporting.Providers;
using System.Web;
using System.IO;

namespace SkeptiForum.Archive.Reporting.Providers {

  class EntityFrameworkReportingProvider : ArchiveReportingProviderBase {

    /*==========================================================================================================================
    | METHOD: INDEX GROUP (ASYNCHRONOUS)
    \-------------------------------------------------------------------------------------------------------------------------*/
    /// <summary>
    ///   Retrieves all posts from the configured <see cref="ArchiveManager.StorageProvider"/> and ensures that each post and 
    ///   its comments are added to the reporting provider.
    /// </summary>
    /// <param name="groupId">The unique identifier for the group to query.</param>
    /// <param name="since">The (optional) start date to index records from.</param>
    /// <param name="until">The (optional) end date to index records up to.</param>
    public override async Task IndexPostsAsync(long groupId, DateTime? since = null, DateTime? until = null) {

      var taskRunner = new List<Task<dynamic>>();

      foreach (FileInfo file in await ArchiveManager.StorageProvider.GetPostsAsync(groupId, since, until)) {
        var posUnderscore = file.Name.IndexOf("_") + 1;
        var posPeriod = file.Name.IndexOf(".", posUnderscore);
        var postId = Int64.Parse(file.Name.Substring(posUnderscore, posPeriod - posUnderscore));
        taskRunner.Add(ArchiveManager.StorageProvider.GetPostAsync(groupId, postId));
      }

      using (ReportingContext database = new ReportingContext()) {

        while (taskRunner.Count > 0) {
          Task<dynamic> task = await Task.WhenAny(taskRunner);
          taskRunner.Remove(task);
          var post = await task;
          string postPath = HttpContext.Current.Server.MapPath(ArchiveManager.Configuration.StorageDirectory + "/" + post.to.data[0].id + "/" + post.id + ".json");
          System.IO.File.SetLastWriteTime(postPath, (DateTime)(post.updated_time ?? DateTime.Now));
          IndexObject(database, post, ObjectType.Post, groupId);
        }

        await database.SaveChangesAsync();

      }

    }

    /*==========================================================================================================================
    | METHOD: INDEX OBJECT
    \-------------------------------------------------------------------------------------------------------------------------*/
    /// <summary>
    ///   Given an object, ensures the item is represented in the index. In the case of posts, will automatically crawl children
    ///   (i.e., comments).
    /// </summary>
    private void IndexObject(ReportingContext database, dynamic json, ObjectType type, long groupId, long postId = -1) {

      string fullId = json.id;
      if (fullId.IndexOf("_") >= 0) {
        fullId = fullId.Substring(fullId.IndexOf("_") + 1);
      }
      long id = Int64.Parse(fullId);

      if (postId < 0) postId = id;

      if (database.Set<Activity>().Where<Activity>(i => i.Id == id).Count() == 0) {

        var activity = new Activity();

        activity.Id = id;
        activity.DateCreated = json.created_time;
        activity.UserId = json.from?.id ?? -1;
        activity.Type = type;
        activity.GroupId = groupId;
        activity.PostId = postId;

        if (type == ObjectType.Post) {
          activity.LikeCount = json.likes?.data?.Count ?? 0;
        }
        else if (type == ObjectType.Comment) {
          activity.LikeCount = json.like_count ?? 0;
        }

        database.ActivityLog.Add(activity);

      }


      if (type == ObjectType.Post) {
        foreach (dynamic comment in json.comments?.data ?? new dynamic[0]) {
          IndexObject(database, comment, ObjectType.Comment, groupId, postId);
        }
      }

    }

  }
}
