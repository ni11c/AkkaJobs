using System.Globalization;
using System.Threading;
using Owin;
using Microsoft.Owin;
[assembly: OwinStartup(typeof(Agridea.Prototypes.Akka.Web.Startup))]

namespace Agridea.Prototypes.Akka.Web
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            //app.MapSignalR();

            CultureInfo ci = Thread.CurrentThread.CurrentCulture;
            var lcid = CultureInfo.CurrentCulture.Parent.LCID;
            if (lcid != 9)
            {
                Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo(1033);
            }
            app.MapSignalR();
            if (lcid != 9)
            {
                Thread.CurrentThread.CurrentCulture = ci;
            }
        }
    }
}
