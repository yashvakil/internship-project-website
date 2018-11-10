using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Envision.Models;

namespace Envision.Filters
{
    public class LogFilter : ActionFilterAttribute
    {
        private EnvisionEntities db = new EnvisionEntities();
        
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {

            if (filterContext.HttpContext.Session["user_type"] == null)
            {
                filterContext.HttpContext.Session["user_type"] = "GST";
            }
            string type = filterContext.HttpContext.Session["user_type"].ToString();
            string userip = filterContext.HttpContext.Request.UserHostAddress;
            string userhost = filterContext.HttpContext.Request.UserHostName;
            string uri = HttpContext.Current.Request.Url.AbsoluteUri;
            
            if (filterContext.HttpContext.Session["user_id"] == null)
            {
                if (db.Logs.Where(c => c.IP == userip && c.UserType == type).Select(c => c.UserId).First() != null)
                {
                    filterContext.HttpContext.Session["user_id"] = db.Logs.Where(c => c.IP == userip && c.UserType == type).Select(c => c.UserId).First();
                }
                else
                {
                    const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789abcdefghijklmnopqrstuvwxyz";
                    Boolean flag = true;
                    Random random = new Random();
                    while (flag)
                    {
                        filterContext.HttpContext.Session["user_id"] = new string(Enumerable.Repeat(chars, 20).Select(s => s[random.Next(s.Length)]).ToArray());
                        if (!db.Logs.Select(c => c.UserId).ToList().Contains(filterContext.HttpContext.Session["user_id"]))
                        {
                            flag = false;
                        }
                    }
                }
            }

            Log l = new Log();
            l.UserId = filterContext.HttpContext.Session["user_id"].ToString();
            l.UserType = filterContext.HttpContext.Session["user_type"].ToString();
            l.HostName = userhost;
            l.Time = System.DateTime.Now;
            l.Link = uri;
            l.IP = userip;

            db.Logs.Add(l);
           
            db.SaveChanges();
            
            
        }
        
    }
}