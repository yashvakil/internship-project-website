using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Filters;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Envision.Filters;

namespace Envision
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            Application["SessionsCount"] = 0;
        }

        protected void Session_Start()
        {
            Application["SessionsCount"] = ((int)Application["SessionsCount"]) + 1;
        }

        protected void Session_End()
        {
            Application["SessionsCount"] = ((int)Application["SessionsCount"]) - 1;
        }
    }
}
