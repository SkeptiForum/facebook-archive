/*==============================================================================================================================
| Author        Jeremy Caney (Jeremy@Caney.net)
| Client        Skepti-Forum Project
| Project       Forum Archive
\=============================================================================================================================*/
using System.Web.Mvc;
using System.Web.Routing;

namespace SkeptiForum.Archive.Web {

  /*============================================================================================================================
  | CLASS: MVC APPLICATION
  \---------------------------------------------------------------------------------------------------------------------------*/
  /// <summary>
  ///   Provides default configuration for the application, including any special processing that needs to happen relative to 
  ///   application events (such as <see cref="Application_Start"/> or <see cref="System.Web.HttpApplication.Error"/>.
  /// </summary>
  public class MvcApplication : System.Web.HttpApplication {

    /*==========================================================================================================================
    | METHOD: APPLICATION START (EVENT HANDLER)
    \-------------------------------------------------------------------------------------------------------------------------*/
    /// <summary>
    ///   Provides initial configuration for the application, including registration of MVC routes via the 
    ///   <see cref="RouteConfig"/> class.
    /// </summary>
    protected void Application_Start() {
      AreaRegistration.RegisterAllAreas();
      RouteConfig.RegisterRoutes(RouteTable.Routes);
    }

  } //Class
} //Method
