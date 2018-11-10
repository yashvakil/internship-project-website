using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Envision.Models;

namespace Envision.Filters
{
    public class DenyCMPFilter : ActionFilterAttribute
    {
        private EnvisionEntities db = new EnvisionEntities();

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.HttpContext.Session["user_type"] == null || filterContext.HttpContext.Session["user_type"].ToString().Equals("CMP"))
            {
                filterContext.RouteData.Values["Allowed"] = false;
            }
        }

    }
}