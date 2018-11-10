using System.Web;
using System.Web.Mvc;
using Envision.Filters;

namespace Envision
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new LogFilter());
            filters.Add(new DenyGSTFilter());
            filters.Add(new DenySTDFilter());
            filters.Add(new DenyCMPFilter());
        }
    }
}
