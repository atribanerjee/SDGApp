using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace SDGApp.Models
{
    public class UserAuthorization : AuthorizeAttribute
    {
        BaseModel BM;
        public UserAuthorization()
        {
            BM = new BaseModel();
        }
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (BM.GetSessionValue("LoggedInUserID") != null)
            { return true; }
            else return false;
        }
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Result = new RedirectToRouteResult(
                        new RouteValueDictionary(
                            new
                            {
                                controller = "Account",
                                action = "Login",
                                ReturnUrl = filterContext.HttpContext.Request.Url?.GetComponents(UriComponents.PathAndQuery, UriFormat.SafeUnescaped)


                                //ReturnUrl = filterContext.RouteData.Values["controller"] + "_" + filterContext.RouteData.Values["action"] + "_" + filterContext.RouteData.Values["id"]
                            })
                        );

        }
    }
}