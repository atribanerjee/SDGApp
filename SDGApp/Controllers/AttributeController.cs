using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SDGApp.Views
{
    public class AttributeController : Controller
    {
        // GET: Attribute
        public ActionResult Index()
        {
            List<string> lstBreadcrumb = new List<string>();
            lstBreadcrumb.Add("Dashboard/Index");
            lstBreadcrumb.Add("Dashboard");
            lstBreadcrumb.Add("Attribute");
            ViewBag.lstbdcomb = lstBreadcrumb;

            
            return View();
            }
    }
}