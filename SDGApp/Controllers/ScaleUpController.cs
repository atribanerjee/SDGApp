using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SDGApp.Models;
using SDGApp.ViewModel;

namespace SDGApp.Controllers
{
    [UserAuthorization]
    public class ScaleUpController : Controller
    {
        //[HttpGet]
        //// GET: ScaleUp
        //public ActionResult ECGEnlarge()
        //{

        //    MeasurementViewModel model = new MeasurementViewModel();
        //    UserModel UM = new UserModel();
        //    model = UM.GetLatestMeasurement(UM.GetLoggedInUserInfo().UserID);

        //    //var dateTime = BM.GetDateTimeValue("09-12-2019 19:25:00.000");
        //    if (model != null && model.UserID > 0)
        //    {
        //        var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        //        var unixDateTime = (model.CreatedDateTime - epoch).TotalMilliseconds;
        //        model.TotalMillisecond = unixDateTime;

        //        //TO ADD SECONDS
        //        string[] arrElapsetime = model.ECGElapsedTime.Split(',');
        //        double[] arrDtime = new double[arrElapsetime.Length];
        //        double mixedtime;

        //        for (int i = 0; i < arrElapsetime.Length; i++)
        //        {
        //            mixedtime = unixDateTime + Convert.ToDouble(arrElapsetime[i]);
        //            arrDtime[i] = mixedtime;
        //        }
        //        model.ECGElapsedTime = string.Join(",", arrDtime);

        //    }
        //    return View(model);
        //}

        //[HttpGet]
        //public ActionResult PPGEnlarge()
        //{

        //    MeasurementViewModel model = new MeasurementViewModel();
        //    UserModel UM = new UserModel();
        //    model = UM.GetLatestMeasurement(UM.GetLoggedInUserInfo().UserID);

        //    //var dateTime = BM.GetDateTimeValue("09-12-2019 19:25:00.000");
        //    if (model != null && model.UserID > 0)
        //    {
        //        var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        //        var unixDateTime = (model.CreatedDateTime - epoch).TotalMilliseconds;
        //        model.TotalMillisecond = unixDateTime;

        //        //TO ADD SECONDS
        //        string[] arrElapsetime = model.ECGElapsedTime.Split(',');
        //        double[] arrDtime = new double[arrElapsetime.Length];
        //        double mixedtime;

        //        for (int i = 0; i < arrElapsetime.Length; i++)
        //        {
        //            mixedtime = unixDateTime + Convert.ToDouble(arrElapsetime[i]);
        //            arrDtime[i] = mixedtime;
        //        }
        //        model.ECGElapsedTime = string.Join(",", arrDtime);

        //    }
        //    return View(model);
        //}
    }
}