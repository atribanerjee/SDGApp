using System;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using System.Web.Mvc.Html;

namespace SDGApp.Helpers
{
    public static class HelperExtensions
    {
        public static MvcHtmlString RawActionLink(this AjaxHelper ajaxHelper, String rawHtml, String actionName, String controllerName, Object routeValues, AjaxOptions ajaxOptions, Object htmlAttributes)
        {
            String holder = Guid.NewGuid().ToString();
            String anchor = ajaxHelper.ActionLink(holder, actionName, controllerName, routeValues, ajaxOptions, htmlAttributes).ToString();
            return MvcHtmlString.Create(anchor.Replace(holder, rawHtml));
        }

        public static MvcHtmlString RawActionLink(this HtmlHelper htmlHelper, String rawHtml, String actionName, String controllerName, Object routeValues, Object htmlAttributes)
        {
            String holder = Guid.NewGuid().ToString();
            String anchor = htmlHelper.ActionLink(holder, actionName, controllerName, routeValues, htmlAttributes).ToString();
            return MvcHtmlString.Create(anchor.Replace(holder, rawHtml));
        }
    }
}