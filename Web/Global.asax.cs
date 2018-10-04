using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Akka.Actor;
using Web;

namespace Agridea.Prototypes.Akka.Web
{
    public class MvcApplication : HttpApplication
    {
        protected static ActorSystem system_;

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AkkaConfig.ConfigureActors(system_);
        }
    }
}
