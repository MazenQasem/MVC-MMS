
using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace MMS2.Controllers
{

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public class MMCFilterAttribute : System.Web.Mvc.ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            HttpContext ctx = HttpContext.Current;

                          
            if (ctx.Session["User"] == null)
            {
                if (filterContext.HttpContext.Request.IsAjaxRequest())
                {
                                                              filterContext.Result = new JsonResult { Data = "_Logon_" };
                }
                else
                {
                                                                                   filterContext.Result = new RedirectToRouteResult(
                        new RouteValueDictionary {
                        { "Controller", "Login" },
                        { "Action", "Index" },
                        {"area",""}
                       
                });
                }
            }
            else
            {
                                                                                                                                                                                                                                                                                                             }
            base.OnActionExecuting(filterContext);
        }
    }


   
}