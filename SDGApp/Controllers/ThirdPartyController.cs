using SDGApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SDGApp.Controllers
{
    [UserAuthorization]
    public class ThirdPartyController : Controller
    {
        UserModel UM;
        ThirdPartyModel TPM;

        public ThirdPartyController()
        {
            UM = new UserModel();
            TPM = new ThirdPartyModel();
        }

        [HttpGet]
        public ActionResult Index()
        {
            List<string> lstBreadcrumb = new List<string>();
            lstBreadcrumb.Add("Dashboard/Index");
            lstBreadcrumb.Add("Dashboard");
            lstBreadcrumb.Add("Third Party API Keys");
            ViewBag.lstbdcomb = lstBreadcrumb;
            return View();
        }


        [HttpPost]
        public JsonResult AddAPIKey(String inputAPIName)
        {
            var result = TPM.AddAPIKey(inputAPIName);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult DeleteAPIKey(Int64 id)
        {
            var result = TPM.DeleteAPIKey(id);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult LoadAPIKeys(Int64 UserID)
        {
            return PartialView("_APIKeysList", TPM.LoadAPIKeys(UserID));
        }
    }
}