using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Cors;

namespace ImagePreview
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            var domain = ConfigurationManager.AppSettings["SourceWebDomain"];
            var cors = new EnableCorsAttribute($"https://{domain}", "*", "*");
            config.EnableCors(cors);

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
