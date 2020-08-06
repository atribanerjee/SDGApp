using Newtonsoft.Json;
using SDGApp.Models;
using SDGApp.ViewModel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace SDGApp.Controllers
{
    [UserAuthorization]
    public class SleepController : Controller
    {
        UserModel UM;
        BaseModel BM;
        SleepModel SleepModel;

        public SleepController()
        {
            UM = new UserModel();
            BM = new BaseModel();
            SleepModel = new SleepModel();
        }

        public ActionResult Index()
        {
            SleepActivityViewModel model = new SleepActivityViewModel();
            List<string> lstBreadcrumb = new List<string>();
            lstBreadcrumb.Add("Dashboard/Index");
            lstBreadcrumb.Add("Dashboard");
            lstBreadcrumb.Add("Sleep");
            ViewBag.lstbdcomb = lstBreadcrumb;

            return View(model);
        }


        [HttpPost]
        public JsonResult GetSleepChartHistory(string type, string currentdate)
        {
            List<SleepActivityViewModel> list = new List<SleepActivityViewModel>();
            int UserID = UM.GetLoggedInUserInfo().UserID;

            if (!String.IsNullOrEmpty(type) && !String.IsNullOrEmpty(currentdate) && UserID > 0)
            {
                DateTime currentdateee = DateTime.ParseExact(currentdate.ToString(), "MM-dd-yyyy", CultureInfo.InvariantCulture);

                list = SleepModel.GetSleepActivity(currentdateee, type, UserID);

                var sbpmonth = currentdateee.ToString("MMMM");

                if (list != null && list.Count > 0)
                {
                    return Json(new { SleepList = list }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { SleepList = string.Empty }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(new { SleepList = string.Empty }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}