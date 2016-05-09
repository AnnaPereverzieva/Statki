using System.Web.Mvc;
using System.Web.Routing;

namespace Statki
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{name1}/{name2}",
                defaults: new
                {
                    controller = "Authentication",
                    action = "Login",
                    name1 = UrlParameter.Optional,
                    name2 = UrlParameter.Optional


                }
                );

            routes.MapRoute(
               name: "Default2",
               url: "{controller}/{action}/{id}"
               
               );
        }
    }
}

