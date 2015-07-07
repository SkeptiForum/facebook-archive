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

  } //Class
} //Namespace 
