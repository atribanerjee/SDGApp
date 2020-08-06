using SDGApp.Models;
using SDGApp.ViewModel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web.Mvc;

namespace SDGApp.Controllers
{
    [UserAuthorization]
    public class CardiacController : Controller
    {
        // GET: Cardiac

        UserModel UM;
        BaseModel BM;
        CardiacModel CardiacModel;

        public CardiacController()
        {
            UM = new UserModel();
            BM = new BaseModel();
            CardiacModel = new CardiacModel();
        }

        public ActionResult Index()
        {
            CardiacViewModel model = new CardiacViewModel();
            List<string> lstBreadcrumb = new List<string>();
            lstBreadcrumb.Add("Dashboard/Index");
            lstBreadcrumb.Add("Dashboard");
            lstBreadcrumb.Add("Cardiac");
            ViewBag.lstbdcomb = lstBreadcrumb;

            return View(model);
        }

        [HttpPost]
        public JsonResult GetCardiacHistory(string type, string currentdate)
        {
            List<CardiacViewModel> list = new List<CardiacViewModel>();
            int UserID = UM.GetLoggedInUserInfo().UserID;

            if (!String.IsNullOrEmpty(type) && !String.IsNullOrEmpty(currentdate) && UserID > 0)
            {
                DateTime currentdateee = DateTime.ParseExact(currentdate.ToString(), "MM-dd-yyyy", CultureInfo.InvariantCulture);

                list = CardiacModel.GetCardiacHistoryDtls(currentdateee, type, UserID);

                if (list != null && list.Count > 0)
                {

                    return Json(new { CardiacList = list }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { CardiacList = string.Empty }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(new { CardiacList = string.Empty }, JsonRequestBehavior.AllowGet);
            }
        }


    }
}