using System.Web.Http;

namespace apiTest.App_Start
{
    public class WebApiConfig
    {
        public static void Configure(HttpConfiguration config)
        {
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
                
            );

            config.Routes.MapHttpRoute(
               name: "HistoryApi",
               routeTemplate:"api/{controller}/history/{script}/{id}",
               defaults: new {controller = "ScriptLookups", action = "history" }

           );
        }
    }
}