using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SDGApp.Models;
using SDGApp.ViewModel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web.Mvc;

namespace SDGApp.Controllers
{
    [UserAuthorization]
    public class DashboardController : Controller
    {
        UserModel UM;
        DashboardModel DashboardModel;

        public DashboardController()
        {
            UM = new UserModel();
            DashboardModel = new DashboardModel();
        }

        [HttpGet]
        public ActionResult Index()
        {
            int UserID = UM.GetLoggedInUserInfo().UserID;
            List<string> lstTabNames = DashboardModel.TabListByUserId(UserID);
            List<SelectListItem> lstWidgetNames = DashboardModel.WidgetNameList(UserID);
            ViewBag.ListWidgetNames = lstWidgetNames;
            ViewBag.ListTabNames = lstTabNames;
            return View();
        }
       
        [HttpPost]
        public JsonResult StoreWidgetStyle(String WidgetDtls)
        {
            
            int UserId = UM.GetLoggedInUserInfo().UserID;

            var result = DashboardModel.SaveWidgetStyle(WidgetDtls, UserId);

            return Json(new { result = result }, JsonRequestBehavior.AllowGet);

        }

        [HttpGet]
        public JsonResult GetUserWidgetStyle()
        {
            int UserId = UM.GetLoggedInUserInfo().UserID;

            var result = DashboardModel.GetUserWidgetStyle(UserId);


            return Json( result , JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public JsonResult AddTab(String inputTabNamevalue)
        {

            var result = DashboardModel.AddNewTab(inputTabNamevalue);


            return Json(result, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public JsonResult DeleteTab(String tabName)
        {
            var userid = UM.GetLoggedInUserInfo().UserID;
            var result = DashboardModel.RemoveTab(tabName, userid);

            return Json(result, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public JsonResult DeleteWidget(String widgetName)
        {
            var userid = UM.GetLoggedInUserInfo().UserID;
            var result = DashboardModel.RemoveWidget(widgetName, userid);

            return Json(result, JsonRequestBehavior.AllowGet);

        }
        

    }
}