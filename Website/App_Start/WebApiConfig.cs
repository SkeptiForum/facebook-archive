using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Web.Http;
using System.Web.Http.OData.Builder;
using System.Web.Http.OData.Extensions;
using SkeptiForum.Archive.Reporting;

namespace SkeptiForum.Archive {

  public static class WebApiConfig {

    public static void Register(HttpConfiguration config) {

      // Web API configuration and services
      config.Formatters.JsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/html"));

      // Web API routes
      config.MapHttpAttributeRoutes();

      ODataConventionModelBuilder builder = new ODataConventionModelBuilder();
      builder.EntitySet<Activity>("ActivityLog");
      config.Routes.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());

    }

  } //Class
} //Namespace
