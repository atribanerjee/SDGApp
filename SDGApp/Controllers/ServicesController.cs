using SDGApp.Models;
using System;
using System.Web.Mvc;

namespace SDGApp.Controllers
{
    public class ServicesController : Controller
    {
        UserModel UM;
        DeviceModel DM;
        ActivityModel AM;

        public ServicesController()
        {
            UM = new UserModel();
            DM = new DeviceModel();
            AM = new ActivityModel();
        }

        /// <summary>
        /// Used to register a new user
        /// </summary>
        /// <param name="FirstName"></param>
        /// <param name="LastName"></param>
        /// <param name="CountryID"></param>
        /// <param name="StateID"></param>
        /// <param name="CityID"></param>
        /// <param name="ZipCode"></param>
        /// <param name="Address"></param>
        /// <param name="Email"></param>
        /// <param name="Password"></param>
        /// <param name="Gender">M/F/O</param>
        /// <param name="Height"></param>
        /// <param name="Weight"></param>
        /// <param name="Race"></param>
        /// <param name="IsAdmin">Not mandatory(Set true if admin)</param>
        /// <param name="SocialSecurityNo">Not mandatory</param>
        /// <param name="PhoneNumber">Not mandatory</param>
        /// <param name="MobileNumber">Not mandatory</param>
        /// <param name="CompanyName">Not mandatory</param>
        /// <param name="FacebookUrl">Not mandatory</param>
        /// <param name="TwitterUrl">Not mandatory</param>
        /// <param name="GooglePlusUrl">Not mandatory</param>
        /// <param name="FlickrUrl">Not mandatory</param>
        /// <param name="YoutubeUrl">Not mandatory</param>
        /// <param name="SkypeID">Not mandatory</param>
        /// <returns>Success / Failed (With Error message)</returns>
        [HttpPost]
        public JsonResult RegisterNewUser
            (
                String FirstName,
                String LastName,
                Int32 CountryID,
                Int32 StateID,
                Int32 CityID,
                String ZipCode,
                String Address,
                String Email,
                String Password,
                String Gender,
                Decimal Height,
                Decimal Weight,
                String Race,
                Boolean IsAdmin = false,
                String SocialSecurityNo = "",
                String PhoneNumber = "",
                String MobileNumber = "",
                String CompanyName = "",
                String FacebookUrl = "",
                String TwitterUrl = "",
                String GooglePlusUrl = "",
                String FlickrUrl = "",
                String YoutubeUrl = "",
                String SkypeID = ""
            )
        {
            return Json(UM.RegisterNewUser(
                FirstName,
                LastName,
                CountryID,
                StateID,
                CityID,
                 ZipCode,
                 Address,
                 Email,
                 Password,
                 Gender,
                 Height,
                 Weight,
                 Race,
                 IsAdmin,
                 SocialSecurityNo,
                 PhoneNumber,
                 MobileNumber,
                 CompanyName,
                 FacebookUrl,
                 TwitterUrl,
                 GooglePlusUrl,
                 FlickrUrl,
                 YoutubeUrl,
                 SkypeID), JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// Used to register a new device
        /// </summary>
        /// <param name="SerialNumber"></param>
        /// <param name="DeviceIMEI"></param>
        /// <returns>Success / Failed (With Error message)</returns>
        [HttpPost]
        public JsonResult RegisterNewDevice(String SerialNumber, String DeviceIMEI)
        {
            return Json(DM.RegisterNewDevice(SerialNumber, DeviceIMEI), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Used to register a new planned activity
        /// </summary>
        /// <param name="PlannedActivitiesName"></param>
        /// <returns>Success / Failed (With Error message)</returns>
        [HttpPost]
        public JsonResult RegisterNewPlannedActivity(String PlannedActivitiesName,DateTime DefaultDateTime)
        {
            return Json(AM.RegisterNewPlannedActivity(PlannedActivitiesName, DefaultDateTime), JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// Used to register a new recognised activity
        /// </summary>
        /// <param name="RecognisedActivitiesName"></param>
        /// <returns>Success / Failed (With Error message)</returns>
        [HttpPost]
        public JsonResult RegisterNewRecognisedActivity(String RecognisedActivitiesName)
        {
            return Json(AM.RegisterNewRecognisedActivity(RecognisedActivitiesName), JsonRequestBehavior.AllowGet);
        }

        ///// <summary>
        /////  Used to save user activity
        ///// </summary>
        ///// <param name="UserID"></param>
        ///// <param name="PlannedActivitiesID">Nullable</param>
        ///// <param name="RecognisedActivitiesID">Nullable</param>
        ///// <param name="DeviceID"></param>
        ///// <param name="Tag"></param>
        ///// <param name="Measurement"></param>
        ///// <param name="StartDateTime"></param>
        ///// <param name="EndDatetime"></param>
        ///// <returns>Success / Failed (With Error message)</returns>
        [HttpPost]
        public JsonResult SaveUserActivity
        (
            Int32 UserID,
            Int32 DeviceID,
            DateTime StartDateTime,
            DateTime EndDatetime,
            Int32 TagID
        )
        {
            return Json(AM.SaveUserActivity(UserID, DeviceID, StartDateTime, EndDatetime, TagID), JsonRequestBehavior.AllowGet);
        }
    }
}