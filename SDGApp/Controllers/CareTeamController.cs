using SDGApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SDGApp.Controllers
{
    [UserAuthorization]
    public class CareTeamController : Controller
    {
        CareTeamModel careTeamModel;
        UserModel UM;
        public CareTeamController()
        {
            careTeamModel = new CareTeamModel();
            UM = new UserModel();
        }
        // GET: CareTeam
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult SearchCarePeopleList(string prefix)
        {
            int LogedInUserID = UM.GetLoggedInUserInfo().UserID;

            var MessageToList = careTeamModel.GetCarePeopleList(LogedInUserID,prefix);
            return Json(MessageToList);
        }


        public JsonResult SendRequestToCarePerson(int UserID)
        {
            int LogedInUserID = UM.GetLoggedInUserInfo().UserID;
            Boolean Result = false;


            if (UserID > 0 && LogedInUserID > 0)
            {
                Result = careTeamModel.SendRequestToCarePerson(LogedInUserID, UserID);
            }

            return Json(Result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCareTeamList()
        {
            int LogedInUserID = UM.GetLoggedInUserInfo().UserID;

            if (LogedInUserID > 0)
            {
                var Result = careTeamModel.GetListCareTeam(LogedInUserID);

                return Json(Result, JsonRequestBehavior.AllowGet);
            }

            return Json(new { Result = "" }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteCarePerson(int CarePeopleID)
        {
            

            if (CarePeopleID > 0)
            {
                var Result = careTeamModel.DeleteCarePerson(CarePeopleID);

                return Json(Result, JsonRequestBehavior.AllowGet);
            }

            return Json(new { Result = "" }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CareTeamViewedPermission(int CarePeopleID, int chkBoxVal)
        {

            if (CarePeopleID > 0)
            {
                var Result = careTeamModel.ChangeViewingPermission(CarePeopleID, chkBoxVal);

                return Json(Result, JsonRequestBehavior.AllowGet);
            }

            return Json(new { Result = "" }, JsonRequestBehavior.AllowGet);
        }


    }
}