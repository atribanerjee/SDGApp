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
    public class WorkActivityController : Controller
    {
        // GET: Cardiac

        UserModel UM;
        BaseModel BM;
        WorkActivityModel WorkActivityModel;

        public WorkActivityController()
        {
            UM = new UserModel();
            BM = new BaseModel();
            WorkActivityModel = new WorkActivityModel();
        }

        
        public ActionResult Index()
        {
            List<string> lstBreadcrumb = new List<string>();
            lstBreadcrumb.Add("Dashboard/Index");
            lstBreadcrumb.Add("Dashboard");
            lstBreadcrumb.Add("Activity");
            ViewBag.lstbdcomb = lstBreadcrumb;

            return View();
        }

       
        [HttpPost]
        public JsonResult GetWorkoutChartHistory(string type, string currentdate)
        {
            List<WorkOutActivityViewModel> list = new List<WorkOutActivityViewModel>();

            int UserID = UM.GetLoggedInUserInfo().UserID;

            if(!String.IsNullOrEmpty(type) && !String.IsNullOrEmpty(currentdate) && UserID > 0)
            {
                DateTime currentdateee = DateTime.ParseExact(currentdate.ToString(), "MM-dd-yyyy", CultureInfo.InvariantCulture);

                list = WorkActivityModel.GetWorkActivity(currentdateee, type, UserID);

                if (list != null && list.Count > 0)
                {
                    return Json(new { WorkActivityList = list }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { WorkActivityList = string.Empty }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(new { WorkActivityList = string.Empty }, JsonRequestBehavior.AllowGet);
            }

        }



    }
}