/*==============================================================================================================================
| Author        Jeremy Caney (Jeremy@Caney.net)
| Client        Skepti-Forum Project
| Project       Forum Archive
\=============================================================================================================================*/
using System;
using System.Web;
using System.Web.Mvc;
using IO = System.IO;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using SkeptiForum.Archive;
using SkeptiForum.Archive.Reporting;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace SkeptiForum.Archive.Web.Controllers {

  /*============================================================================================================================
  | CLASS: ADMIN CONTROLLER
  \---------------------------------------------------------------------------------------------------------------------------*/
  /// <summary>
  ///   Provides external access to administrative requests.
  /// </summary>
  public class AdminController : Controller {

    /*==========================================================================================================================
    | GET: /ADMIN/
    \-------------------------------------------------------------------------------------------------------------------------*/
    /// <summary>
    ///   Provides access to a list of administrative options available.
    /// </summary>
    /// <returns>The index view for available administrative tasks.</returns>
    public ActionResult Index() {
      return View();
    }

    /*==========================================================================================================================
    | GET: /ADMIN/DOWNLOAD/
    \-------------------------------------------------------------------------------------------------------------------------*/
    /// <summary>
    ///   Checks the user's authentication status and, if they are authorized to use the application, gives them the option of 
    ///   downloading an archive of group posts from the Facebook Graph API via the <see cref="FacebookGroup.GetPostsAsync"/>
    ///   method. 
    /// </summary>
    /// <returns>The download view.</returns>
    public async Task<ActionResult> Download() {
      return View(ArchiveManager.Groups);
    }

    /*==========================================================================================================================
    | GET: /ADMIN/REPORTING/
    \-------------------------------------------------------------------------------------------------------------------------*/
    /// <summary>
    ///   Provides tools for displaying and generating reporting data for the archive content. 
    /// </summary>
    /// <returns>The download view.</returns>
    public async Task<ActionResult> Reporting() {
      ViewData["GroupId"] = (long)-1;
      return View(ArchiveManager.Groups);
    }

    private Stopwatch _stopwatch = new Stopwatch();

    /*==========================================================================================================================
    | POST: /ADMIN/REPORTING/
    \-------------------------------------------------------------------------------------------------------------------------*/
    /// <summary>
    ///   Generates content for reporting. 
    /// </summary>
    /// <returns>The download view.</returns>
    [HttpPost]
    public async Task<ActionResult> Reporting(long groupId = -1) {
      ViewData["GroupId"] = groupId;
      _stopwatch.Start();
      var taskRunner = new List<Task>();
      ViewData["Bootstrapping"] = "Looping through groups, establishing tasks: " + _stopwatch.Elapsed.TotalSeconds;
      foreach (FacebookGroup group in ArchiveManager.Groups) {
        if (groupId > 0 && group.Id != groupId) continue;
      //taskRunner.Add(IndexGroup(group.Id));
        await IndexGroup(group.Id);
      }
      ViewData["RunningTasks"] = "Tasks are triggered; waiting for completion: " + _stopwatch.Elapsed.TotalSeconds;
      Task.WaitAll(taskRunner.ToArray());
      ViewData["TasksFinished"] = "Tasks have all completed: " + _stopwatch.Elapsed.TotalSeconds;
      _stopwatch.Stop();
      return View(ArchiveManager.Groups);
    }

    /*==========================================================================================================================
    | METHOD: INDEX GROUP
    \-------------------------------------------------------------------------------------------------------------------------*/
    /// <summary>
    ///   Given a group, ensures all child items (posts, comments) are indexed.
    /// </summary>
    private async Task IndexGroup(long groupId) {
      var taskRunner = new List<Task<dynamic>>();
      int count = 0;
      foreach (IO.FileInfo file in await ArchiveManager.StorageProvider.GetPostsAsync(groupId)) {
        var posUnderscore = file.Name.IndexOf("_") + 1;
        var posPeriod = file.Name.IndexOf(".", posUnderscore);
        var postId = Int64.Parse(file.Name.Substring(posUnderscore, posPeriod - posUnderscore));
        taskRunner.Add(ArchiveManager.StorageProvider.GetPostAsync(groupId, postId));
        ViewData["PostQueued" + postId] = "The post " + postId + " has been queued: " + _stopwatch.Elapsed.TotalSeconds;
      }
      ViewData["GroupQueued" + groupId] = "Processing group " + groupId + ": " + _stopwatch.Elapsed.TotalSeconds;
      ViewData["GroupStatusA" + groupId] = "The task runner for the " + groupId + " is " + taskRunner.Count + " items: " + _stopwatch.Elapsed.TotalSeconds;
      using (ReportingContext database = new ReportingContext()) {
        while (taskRunner.Count > 0) {
          Task<dynamic> task = await Task.WhenAny(taskRunner);
          taskRunner.Remove(task);
          var post = await task;
          ViewData["PostLoaded" + post.id] = "The post by " + (post.from?.name ?? "[Unknown]") + " has finished loading: " + _stopwatch.Elapsed.TotalSeconds;
          string postPath = HttpContext.Server.MapPath(ArchiveManager.Configuration.StorageDirectory + "/" + post.to.data[0].id + "/" + post.id + ".json");
          System.IO.File.SetLastWriteTime(postPath, (DateTime)post.updated_time);
          IndexObject(database, post, ObjectType.Post, groupId);
        }
        ViewData["Saving" + groupId] = "Saving " + groupId + " changes to database: " + _stopwatch.Elapsed.TotalSeconds;
        await database.SaveChangesAsync();
      }
      ViewData["GroupStatusB" + groupId] = "The task runner for the " + groupId + " is " + taskRunner.Count + " items: " + _stopwatch.Elapsed.TotalSeconds;
      ViewData["GroupFinished" + groupId] = "Finished with " + groupId + ": " + _stopwatch.Elapsed.TotalSeconds;
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
        ViewData[type + "Processing" + id] = "Not found. Writing to the database." + _stopwatch.Elapsed.TotalSeconds;
        var activity = new Activity();
        activity.Id = id;
        activity.DateCreated = json.created_time;
        activity.UserId = json.from?.id ?? -1;
        activity.Type = type;
        activity.GroupId = groupId;
        activity.PostId = postId;
        if (type == ObjectType.Post) {
          activity.LikeCount = json.likes?.data?.Count?? 0;
        }
        else if (type == ObjectType.Comment) {
          activity.LikeCount = json.like_count?? 0;
        }

        database.ActivityLog.Add(activity);
      }
      else {
        ViewData[type + "Processing" + id] = "Found! Skipping..." + _stopwatch.Elapsed.TotalSeconds;
      }

      if (type == ObjectType.Post) {
        foreach (dynamic comment in json.comments?.data ?? new dynamic[0]) {
          IndexObject(database, comment, ObjectType.Comment, groupId, postId);
        }
        ViewData[type + "Comments" + id] = "Finished processing comments: " + _stopwatch.Elapsed.TotalSeconds;
      }

    }

  } //Class
} //Namespace 
