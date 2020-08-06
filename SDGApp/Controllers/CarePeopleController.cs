using SDGApp.Models;
using SDGApp.ViewModel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SDGApp.Controllers
{
    [UserAuthorization]
    public class CarePeopleController : Controller
    {
        UserModel UM;
        CarePeopleModel carePeopleModel;

        public CarePeopleController()
        {
            UM = new UserModel();
            carePeopleModel = new CarePeopleModel();
        }

        // GET: CarePeople
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult AddRequestForCare()
        {

            return View();
        }

        [HttpPost]
        public JsonResult RequestForCareList()
        {
            int UserId = UM.GetLoggedInUserInfo().UserID;

            var lst = carePeopleModel.RequestForCareList(UserId);

            return Json(lst, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ViewPeopleDtls(int UserID)
        {

            int LogedInUserID = UM.GetLoggedInUserInfo().UserID;

            if (UserID > 0 && LogedInUserID>0)
            {

                var checkpermission = carePeopleModel.CheckViewingPermission(UserID, LogedInUserID);

                if (checkpermission)
                {
                    ViewBag.ViewPeopleUserID = UserID;

                    var UserDtls = UM.GetUserDetailByUserID(UserID);

                    ViewBag.UserName = UserDtls.FirstName + " " + UserDtls.LastName;

                    return View();
                }
                else
                {
                    TempData["ErrorMessage"] = "No read permission";
                    return RedirectToAction("Index");
                }

            }
            else
            {
                TempData["ErrorMessage"] = "No read permission";
                return RedirectToAction("Index");
            }
            
        }

        public ActionResult GetMeasurmentDtls(int UserID)
        {
            MeasurementViewModel measurementViewModel = new MeasurementViewModel();
            if (UserID < 0)
            {
                measurementViewModel.UserID = 0;
            }
            else
            {
                measurementViewModel.UserID = UserID;
            }

            return PartialView("_MeasurmentDtls", measurementViewModel);

        }
        public ActionResult GetCardiacDtls(int UserID)
        {
            if (UserID < 0)
            {
                ViewData["UserID"] = 0;
            }
            else
            {
                ViewData["UserID"] = UserID;
            }

            return PartialView("_CardiacDtls", ViewData["UserID"]);

        }
        public ActionResult GetActivityDtls(int UserID)
        {
            if (UserID < 0)
            {
                ViewData["UserID"] = 0;
            }
            else
            {
                ViewData["UserID"] = UserID;
            }

            return PartialView("_ActivityDtls", ViewData["UserID"]);

        }
        public ActionResult GetSleepDtls(int UserID)
        {
            if (UserID < 0)
            {
                ViewData["UserID"] = 0;
            }
            else
            {
                ViewData["UserID"] = UserID;
            }

            return PartialView("_SleepDtls", ViewData["UserID"]);

        }

        [HttpPost]
        public JsonResult GetCardiacHistory(string type, string currentdate, int UserID)
        {
            List<CardiacViewModel> list = new List<CardiacViewModel>();
            CardiacModel cardiacModel = new CardiacModel();

            if (!String.IsNullOrEmpty(type) && !String.IsNullOrEmpty(currentdate) && UserID > 0)
            {
                DateTime currentdateee = DateTime.ParseExact(currentdate.ToString(), "MM-dd-yyyy", CultureInfo.InvariantCulture);

                list = cardiacModel.GetCardiacHistoryDtls(currentdateee, type, UserID);

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

        [HttpPost]
        public JsonResult GetWorkoutChartHistory(string type, string currentdate, int UserID)
        {
            List<WorkOutActivityViewModel> list = new List<WorkOutActivityViewModel>();
            WorkActivityModel workActivityModel = new WorkActivityModel();


            if (!String.IsNullOrEmpty(type) && !String.IsNullOrEmpty(currentdate) && UserID > 0)
            {
                DateTime currentdateee = DateTime.ParseExact(currentdate.ToString(), "MM-dd-yyyy", CultureInfo.InvariantCulture);

                list = workActivityModel.GetWorkActivity(currentdateee, type, UserID);

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

        [HttpPost]
        public JsonResult GetSleepChartHistory(string type, string currentdate, int UserID)
        {
            List<SleepActivityViewModel> list = new List<SleepActivityViewModel>();
            SleepModel sleepModel = new SleepModel();

            if (!String.IsNullOrEmpty(type) && !String.IsNullOrEmpty(currentdate) && UserID > 0)
            {
                DateTime currentdateee = DateTime.ParseExact(currentdate.ToString(), "MM-dd-yyyy", CultureInfo.InvariantCulture);

                list = sleepModel.GetSleepActivity(currentdateee, type, UserID);

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