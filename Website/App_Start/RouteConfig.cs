/*==============================================================================================================================
| Author        Jeremy Caney (Jeremy@Caney.net)
| Client        Skepti-Forum Project
| Project       Forum Archive
\=============================================================================================================================*/
using System.Web.Mvc;
using System.Web.Routing;

namespace SkeptiForum.Archive.Web {

  /*============================================================================================================================
  | CLASS: ROUTE CONFIGURATION
  \---------------------------------------------------------------------------------------------------------------------------*/
  /// <summary>
  ///   Provides default routing configuration for MVC and, if enabled, Web API.
  /// </summary>
  public class RouteConfig {

    /*==========================================================================================================================
    | METHOD: REGISTER ROUTES
    \-------------------------------------------------------------------------------------------------------------------------*/
    /// <summary>
    ///   Provided a <see cref="RouteCollection"/>, registers all routes associated with the application. In addition to the 
    ///   default routes provided by the Microsoft MVC Framework, this additionally includes a custom route for mimicking the 
    ///   Facebook "permalinks" for groups, to maintain consistency between the archive and the corresponding Facebook threads.
    /// </summary>
    /// <param name="routes">
    ///   The route collection for the server, typically passed from the <see cref="System.Web.HttpApplication"/> class.
    /// </param>
    public static void RegisterRoutes(RouteCollection routes) {

      /*------------------------------------------------------------------------------------------------------------------------
      | Ignore requests to AXDs (handled by HttpHandler)
      \-----------------------------------------------------------------------------------------------------------------------*/
      routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

      /*------------------------------------------------------------------------------------------------------------------------
      | Enable attribute-based routing
      \-----------------------------------------------------------------------------------------------------------------------*/
      routes.MapMvcAttributeRoutes();

      /*------------------------------------------------------------------------------------------------------------------------
      | Handle default route convention
      \-----------------------------------------------------------------------------------------------------------------------*/
      routes.MapRoute(
        name: "Default",
        url: "{controller}/{action}/{id}",
        defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
      );

    }
  }
}
