using SDGApp.Models;
using System.Web.Mvc;
using SDGApp.ViewModel;
using System;
using System.Collections.Generic;

namespace SDGApp.Controllers
{
    [UserAuthorization]
    public class ActivityController : Controller
    {
        UserModel UM;
        ActivityModel AM;
        BaseModel BM;

        public ActivityController()
        {
            UM = new UserModel();
            AM = new ActivityModel();
            BM = new BaseModel();
        }

        // GET: Activity
        public ActionResult Index()
        {
            List<string> lstBreadcrumb = new List<string>();
            lstBreadcrumb.Add("Dashboard/Index");
            lstBreadcrumb.Add("Dashboard");
            lstBreadcrumb.Add("Activity");
            ViewBag.lstbdcomb = lstBreadcrumb;

            if (!String.IsNullOrEmpty(Convert.ToString(TempData["SuccessMessage"])))
            {
                ViewBag.SuccessMessage = Convert.ToString(TempData["SuccessMessage"]);
            }
            return View();
        }
        public ActionResult Add()
        {
            List<string> lstBreadcrumb = new List<string>();
            lstBreadcrumb.Add("Activity/Index");
            lstBreadcrumb.Add("Activity");
            lstBreadcrumb.Add("Add");
            ViewBag.lstbdcomb = lstBreadcrumb;

            return View();
        }

        [HttpPost]
        public ActionResult Add(PlannedActivities PA)
        {
            if (ModelState.IsValid)
            {
                if (AM.AddNewActivity(PA))
                {

                    return RedirectToAction("Index", "Activity");
                }
            }
            return View();
        }
        public ActionResult ActivityList(int PageNumber, int PageSize)
        {
            PlannedActivities PA = new PlannedActivities();

            PA.PageNumber = PageNumber;
            PA.PageSize = PageSize;

            return PartialView("_ActivityList", AM.GetAllActivityList(PA));
        }

        public ActionResult Edit(int ID)
        {
            List<string> lstBreadcrumb = new List<string>();
            lstBreadcrumb.Add("Activity/Index");
            lstBreadcrumb.Add("Activity");
            lstBreadcrumb.Add("Edit");
            ViewBag.lstbdcomb = lstBreadcrumb;

            PlannedActivities PA = AM.GetActivityDetailByUserID(ID);
            return View(PA);
        }
        [HttpPost]
        public ActionResult Edit(PlannedActivities model)
        {
            if (ModelState.IsValid)
            {
                if (AM.ActivityDetailUpdate(model))
                {
                    BM.GetIntegerValue(BM.GetSessionValue("LoggedInUserRoleID"));
                    return RedirectToAction("Index", "Activity");
                }
                else
                {
                    ViewBag.ErrorMessage = "Profile update failed";
                    return View(model);
                }
            }

            return View(model);
        }
        [HttpPost]
        public JsonResult DeleteActivity(int ID)
        {
            if (AM.DeleteActivitybyActivityID(ID))
            {
                return Json(new { Result = true, Message = "Activity deleted successfully." }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Result = false, Message = "Activity deleted unsuccessful." }, JsonRequestBehavior.AllowGet);
            }

        }
    }
}