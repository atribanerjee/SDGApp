using Newtonsoft.Json;
using SDGApp.Models;
using SDGApp.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SDGApp.Controllers
{
    [UserAuthorization]
    public class MeasurementController : Controller
    {
        UserModel UM;
        BaseModel BM;
        DeviceServicesModel DSM;
        MeasurementModel measurementModel;

        public MeasurementController()
        {
            UM = new UserModel();
            BM = new BaseModel();
            DSM = new DeviceServicesModel();
            measurementModel = new MeasurementModel();
        }

        // GET: Measurement
        public ActionResult Index()
        {
            int UserID = UM.GetLoggedInUserInfo().UserID;
            List<string> lstBreadcrumb = new List<string>();
            lstBreadcrumb.Add("Dashboard/Index");
            lstBreadcrumb.Add("Dashboard");
            lstBreadcrumb.Add("Measurement");
            ViewBag.lstbdcomb = lstBreadcrumb;
            // ViewBag.UserDdl = UM.GetUserDDL();

            MeasurementViewModel model = new MeasurementViewModel();
            model.UserID = UserID;
            return View(model);
        }



        #region[*** Not in Use**]
        //public ActionResult GetEcg(int UserID)
        //{
        //    //ViewBag.EcgValues = UM.GetLatestMeasurement(UM.GetLoggedInUserInfo().UserID).EcgValues;

        //    //return PartialView("_GetEcg");

        //    MeasurementViewModel model = new MeasurementViewModel();
        //    model = UM.GetLatestMeasurement(UserID);

        //    //var dateTime = BM.GetDateTimeValue("09-12-2019 19:25:00.000");
        //    if (model != null && model.UserID > 0)
        //    {
        //        var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        //        var unixDateTime = (model.CreatedDateTime - epoch).TotalMilliseconds;
        //        model.TotalMillisecond = unixDateTime;

        //        //TO ADD SECONDS
        //        if (!String.IsNullOrEmpty(model.ECGElapsedTime))
        //        {
        //            //string[] arrElapsetime = model.ECGElapsedTime.Split(',');
        //            //double[] arrDtime = new double[arrElapsetime.Length];
        //            //double mixedtime;

        //            //for (int i = 0; i < arrElapsetime.Length; i++)
        //            //{
        //            //    if (!string.IsNullOrEmpty(arrElapsetime[i]))
        //            //    {
        //            //        mixedtime = unixDateTime + Convert.ToDouble(arrElapsetime[i]);
        //            //        arrDtime[i] = mixedtime;
        //            //    }
        //            //}
        //            //model.ECGElapsedTime = string.Join(",", arrDtime);

        //            string[] arrElapsetime = model.ECGElapsedTime.Split(',');
        //            double[] arrDtime = new double[model.EcgValues.Split(',').Length];


        //            if (arrElapsetime != null && arrElapsetime.Length >= 2)
        //            {
        //                var difference = Convert.ToDouble(arrElapsetime[1]) - Convert.ToDouble(arrElapsetime[0]);
        //                model.Difference = Math.Round(difference, 3) / model.EcgValues.Split(',').Length;
        //            }

        //            //for (int i = 0; i < model.EcgValues.Split(',').Length; i++)
        //            //{
        //            //    if (!string.IsNullOrEmpty(arrElapsetime[0]))
        //            //    {
        //            //        mixedtime = unixDateTime + Convert.ToDouble(arrElapsetime[0])+ model.Difference;
        //            //        arrDtime[i] = mixedtime;
        //            //    }
        //            //}
        //            //model.ECGElapsedTime = string.Join(",", arrDtime);

        //        }

        //    }
        //    else
        //    {
        //        model = new MeasurementViewModel();
        //    }


        //    /*
        //    ViewBag.EcgValues = model.EcgValues;
        //    ViewBag.Ellapsetime = model.ECGElapsedTime;
        //    return View(model);*/
        //    return PartialView("_GetECGApex", model);
        //}

        //public ActionResult GetPpg(int UserID)
        //{
        //    /*ViewBag.PpgValues = UM.GetLatestMeasurement(UM.GetLoggedInUserInfo().UserID).PpgValues;

        //    return PartialView("_GetPpg");*/


        //    MeasurementViewModel model = new MeasurementViewModel();
        //    model = UM.GetLatestMeasurement(UserID);
        //    if (model != null && model.UserID > 0)
        //    {
        //        //var dateTime = BM.GetDateTimeValue("09-12-2019 19:25:00.000");

        //        var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        //        var unixDateTime = (model.CreatedDateTime - epoch).TotalMilliseconds;
        //        model.TotalMillisecond = unixDateTime;

        //        if (!String.IsNullOrEmpty(model.PPGElapsedTime))
        //        {
        //            //TO ADD SECONDS
        //            //string[] arrElapsetime = model.PPGElapsedTime.Split(',');
        //            //double[] arrDtime = new double[arrElapsetime.Length];
        //            //double mixedtime;

        //            //for (int i = 0; i < arrElapsetime.Length; i++)
        //            //{
        //            //    if (!string.IsNullOrEmpty(arrElapsetime[i]))
        //            //    {
        //            //        mixedtime = unixDateTime + Convert.ToDouble(arrElapsetime[i]);
        //            //        arrDtime[i] = mixedtime;
        //            //    }
        //            //}
        //            //model.PPGElapsedTime = string.Join(",", arrDtime);
        //            //var difference = Convert.ToDouble(arrElapsetime[1]) - Convert.ToDouble(arrElapsetime[0]);
        //            //model.Difference = Math.Round(difference, 3);

        //            string[] arrElapsetime = model.PPGElapsedTime.Split(',');
        //            double[] arrDtime = new double[model.PpgValues.Split(',').Length];


        //            if (arrElapsetime != null && arrElapsetime.Length >= 2)
        //            {
        //                var difference = Convert.ToDouble(arrElapsetime[1]) - Convert.ToDouble(arrElapsetime[0]);
        //                model.Difference = Math.Round(difference, 3) / model.PpgValues.Split(',').Length;
        //            }
        //        }
        //    }
        //    else
        //    {
        //        model = new MeasurementViewModel();
        //    }

        //    return PartialView("_GetPPGApex", model);
        //}

        #endregion

        //using

        public ActionResult GetHRHrvBP(int UserID)
        {
            MeasurementViewModel model = measurementModel.GetHRBPHrvByUserID(UserID);
            if (model != null)
            {
                ViewBag.HR = BM.GetStringValue(model.HR);
                ViewBag.HRV = BM.GetStringValue(model.HRV);
                ViewBag.BP = BM.GetStringValue(model.BP);
                ViewBag.Calories = BM.GetStringValue(model.Calories);
            }
            else
            {
                ViewBag.HR = "";
                ViewBag.HRV = "";
                ViewBag.BP = "";
                ViewBag.Calories = "";
            }
            return PartialView("_GetHRHrvBP");
        }

        public ActionResult GetHRHrvBPByFileID(int FileID)
        {
            MeasurementViewModel model = measurementModel.GetHRBPHrvByFileID(FileID);
            if (model != null)
            {
                ViewBag.HR = BM.GetStringValue(model.HR);
                ViewBag.HRV = BM.GetStringValue(model.HRV);
                ViewBag.BP = BM.GetStringValue(model.BP);
                ViewBag.Calories = BM.GetStringValue(model.Calories);
            }
            else
            {
                ViewBag.HR = "";
                ViewBag.HRV = "";
                ViewBag.BP = "";
                ViewBag.Calories = "";
            }
            return PartialView("_GetHRHrvBP");
        }

        public ActionResult GetEcgGraph(int UserID)
        {
            MeasurementViewModel model = new MeasurementViewModel();
            model = measurementModel.GetLatestECGPPGMeasurementFromFile(UserID);

            if (model != null && model.UserID > 0)
            {

            }
            else
            {
                model = new MeasurementViewModel();
            }

            return PartialView("_GetECGHighChart", model);
        }

        public ActionResult GetPpgGraph(int UserID)
        {
            MeasurementViewModel model = new MeasurementViewModel();
            model = measurementModel.GetLatestECGPPGMeasurementFromFile(UserID);

            if (model != null && model.UserID > 0)
            {

            }
            else
            {
                model = new MeasurementViewModel();
            }

            return PartialView("_GetPPGHighChart", model);
        }

        public ActionResult GetEcgGraphByID(int FileID)
        {
            MeasurementViewModel model = new MeasurementViewModel();

            model = measurementModel.GetMeasurmentByFileID(FileID, "ecg");

            return PartialView("_GetECGHighChart", model);
        }

        public ActionResult GetPpgGraphByID(int FileID)
        {
            MeasurementViewModel model = new MeasurementViewModel();
            model = measurementModel.GetMeasurmentByFileID(FileID, "ppg");

            return PartialView("_GetPPGHighChart", model);


        }

        public ActionResult GetHistory(int UserID, string type, DateTime currentdate)
        {
            List<MeasurementViewModel> list = new List<MeasurementViewModel>();

            DateTime currentdateee = Convert.ToDateTime(currentdate);

            if (type != "" && type == "day")
            {

                list = measurementModel.GetHistoryList(UserID, currentdate, type);

                return PartialView("_GetHistoryDayWise", list);

            }
            else if (type != "" && type == "month")
            {
                list = measurementModel.GetHistoryList(UserID, currentdate, type);
                return PartialView("_GetHistoryMonthWise", list);

            }
            else if (type != "" && type == "week")
            {
                list = measurementModel.GetHistoryList(UserID, currentdate, type);
                return PartialView("_GetHistoryWeekWise", list);
            }

            return PartialView("_GetHistoryDayWise", list);
        }


        #region :: DownloadJson
              
        public ActionResult DownloadFile(int FileID)
        {
           
            String jsonFileFullPath = String.Empty;

            String FileName = String.Empty;

            jsonFileFullPath = measurementModel.GetFilePathByFileID(FileID, ref FileName);

            //get the temp folder and file path in server
            string fullPath =Path.Combine(Server.MapPath(jsonFileFullPath), FileName);

            
            return File(fullPath, "application/json", FileName);

        }

        #endregion
    }
}