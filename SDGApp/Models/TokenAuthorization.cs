using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Filters;
using httpnet=System.Net;

namespace SDGApp.Models
{
    public class TokenAuthorization : AuthorizeAttribute, IAuthenticationFilter
    {
        public void OnAuthentication(AuthenticationContext filterContext)
        {
            
            string authparameter = string.Empty;
            
            var authenticationHeader = filterContext.HttpContext.Request.Headers;
            string token=authenticationHeader["authorization"];

            string tokenuserid=JwtTokenManager.ValidateToken(token);
            if(string.IsNullOrEmpty(tokenuserid))
            {
                filterContext.Result = new HttpUnauthorizedResult();
                return;
            }
            
            filterContext.Principal = JwtTokenManager.GetPrincipal(token);
        }

        public void OnAuthenticationChallenge(AuthenticationChallengeContext filterContext)
        {
            if (filterContext.Result == null || filterContext.Result is HttpUnauthorizedResult)
            {
                filterContext.Result = new JsonResult { Data = new { Result = false, Message = "User Login failed." } };
            }

        }


    }
}