
using Newtonsoft.Json;
using SDGApp.Helpers;
using SDGApp.Models;
using SDGApp.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Services;
using System.Web.Services.Description;

namespace SDGApp.Controllers
{

    public class DeviceServicesController : Controller
    {
        DeviceServicesModel DSM;
        UserModel UM;
        MailHelper MH;
        BaseModel BM;
        TagLabesModel TLM;
        UserMessageModel UMM;
        UserContactsModel UCM;
        TagHistoryModel THM;
        WorkActivityModel WAM;
        SleepModel SM;

        public DeviceServicesController()
        {
            DSM = new DeviceServicesModel();
            UM = new UserModel();
            MH = new MailHelper();
            BM = new BaseModel();
            TLM = new TagLabesModel();
            UMM = new UserMessageModel();
            UCM = new UserContactsModel();
            THM = new TagHistoryModel();
            WAM = new WorkActivityModel();
            SM = new SleepModel();
        }

        public ActionResult Index()
        {
            return View();
        }

        #region [ :: User Login ]

        /// <summary>
        /// User Login Service API
        ///  http://localhost:64259/DeviceServices/UserLogin?Password=12345&UserName=atribanerjee
        /// </summary>       
        /// <param name="LoginID"></param>
        /// <param name="Password"></param>
        /// <returns>results if success User Details else fail Message</returns>

        public JsonResult UserLogin(string UserName, string Password)
        {
            UserViewModel model = new UserViewModel();

            model.UserName = UserName;
            model.Password = Password;

            UserViewModel Data = DSM.CheckLogInService(model);

            if (Data != null)
            {
                string salt = JwtTokenManager.GenerateToken(BM.GetStringValue(Data.UserID));
                return Json(new { Result = true, Data = Data, Salt = salt }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Result = false, Message = "User Login failed." }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion


        #region [ :: User Registration Service Call ]

        /// <summary>
        /// User Registration Service API
        ///  http://localhost:64259/DeviceServices/UserRegistation/?FirstName=Json&LastName=Makhhi&Address=FLORIDA&Email=makhhijson@gmail.com&Password=123456&CofirmPassword=123456&UserName=Johnmakkisand&Zip=145258&UserTypeId=3&CompanyID=0&UserRoleID=0
        /// </summary>
        /// <param name="FirstName"></param>
        /// <param name="LastName"></param>
        /// <param name="Email"></param>
        /// <param name="Password"></param>
        /// <param name="CofirmPassword"></param>
        /// <param name="UserName"></param>
        /// 
        /// <returns>results Success Message / Fail message</returns>

        [HttpPost]
        public JsonResult UserRegistation(AppUserRegisterViewModel model)
        {

            if (ModelState.IsValid)
            {
                if (!DSM.CheckEmailID(model.Email) && !DSM.CheckUserName(model.UserName))
                {
                    Int64 ID = 0;

                    if (DSM.AddNewUser(model, ref ID))
                    {
                        model.Userid = BM.GetIntegerValue(ID);

                        AppUserRegisterViewModel newmodel = new AppUserRegisterViewModel();

                        newmodel.Userid = model.Userid;
                        newmodel.FirstName = model.FirstName;
                        newmodel.LastName = model.LastName;
                        newmodel.UserName = model.UserName;
                        newmodel.Email = model.Email;
                        newmodel.Phone = model.Phone;

                        string salt = JwtTokenManager.GenerateToken(BM.GetStringValue(newmodel.Userid));
                        return Json(new { Result = true, Message = "User Registration done Successfully.", Data = newmodel, Salt = salt }, JsonRequestBehavior.AllowGet);

                    }
                    else
                    {
                        return Json(new { Result = false, Message = "User Registration failed." }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json(new { Result = false, Message = "Email or User Name already exists. Please try another." }, JsonRequestBehavior.AllowGet);
                }

            }
            else
            {
                var errors = new List<string>();
                foreach (var state in ModelState)
                {
                    foreach (var error in state.Value.Errors)
                    {
                        errors.Add(error.ErrorMessage);
                    }
                }

                return Json(new { Result = false, Errors = errors }, JsonRequestBehavior.AllowGet);
            }

        }

        #endregion


        #region [ :: User Forget Password ]

        /// <summary>
        /// User Login Service API
        ///  http://localhost:64259/DeviceServices/ForgetPassword/?Email=atri.tih@gmail.com
        /// </summary>       
        /// <param name="Email"></param>
        /// <returns>results if success return reset password link url else invalid email</returns>

        public JsonResult ForgetPassword(string Email)
        {
            if (!String.IsNullOrEmpty(Email))
            {
                UserViewModel UVM = new UserViewModel();
                Guid guid = Guid.NewGuid();
                UVM.GuID = guid.ToString();
                UVM.Email = Email;
                UserViewModel model = DSM.GetUserDetailsByEmailAndSetNewGuID(UVM.Email, UVM.GuID);

                if (model.UserID > 0)
                {
                    Dictionary<string, string> objDict = new Dictionary<string, string>();
                    objDict.Add("Pseudo", model.FirstName);

                    objDict.Add("Year", DateTime.Now.Year.ToString());

                    objDict.Add("ActivationUrl", GlobalConstants.BaseUrl + "/Account/ResetPassword?ID=" + UVM.GuID);

                    MH.SendEmail("Reset Password Requested", UVM.Email, "ForgotPassword.html", objDict);

                    return Json(new { Result = true,ID= model.UserID, Message = "A reset password link has been sent to your registered email. Please check your email." }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { Result = false, Message = "Invalid email." }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(new { Result = false, Message = "Input email address." }, JsonRequestBehavior.AllowGet);
            }

        }

        #endregion


        #region [ :: User Reset Password ]

        /// <summary>
        /// Get User Details By GUID Service API
        /// [GET] http://localhost:64259/DeviceServices/ResetPassword/?GUID=05ca788f-afd9-4d95-8bb7-d8dcb5df0204
        /// <param name="GUID"></param>
        ///<returns>results if success return User Deatails by id else invalid url</returns>

        [HttpGet]
        public JsonResult ResetPassword(string GUID)
        {
            if (!String.IsNullOrEmpty(GUID))
            {
                UserViewModel uvm = DSM.GetUserDetailByGUID(GUID);

                if (uvm != null && uvm.UserID > 0)
                {
                    return Json(new { Result = true, Data = uvm }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { Result = false, Message = "Invalid Url." }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(new { Result = false, Message = "Input GUID as parameter." }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Reset Password Service API
        ///  http://localhost:53415/DeviceServices/ResetPassword/?UserID=2&NewPassword=j123456

        /// <param name="UserID"></param>
        /// <param name="NewPassword"></param>
        ///<returns>results if success return User Deatails by id else invalid url</returns>
        [HttpPost]
        public JsonResult ResetPassword(int UserID, string NewPassword)
        {
            if (!String.IsNullOrEmpty(NewPassword) || UserID > 0)
            {
                UserViewModel model = DSM.GetUserDetailByServiceUserId(UserID);

                model.UserID = BM.GetIntegerValue(UserID);
                model.NewPassword = NewPassword;

                if (UM.UpdatepasswordforUser(model.UserID, model.NewPassword))
                {
                    Dictionary<string, string> objDict = new Dictionary<string, string>();
                    objDict.Add("Pseudo", model.FirstName);

                    MH.SendEmail("Password Changed Successfully", model.Email, "ChangePasswordSuccessfully.html", objDict);
                    return Json(new { Result = true,ID= model.UserID, Message = "Your Password has successfully changed." }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { Result = false, Message = "Reset Password failed." }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(new { Result = false, Message = "UserId or New Password not found." }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion


        #region [  :: Get User Details  ]

        /// <summary>
        /// Get User Details By User ID Service API
        /// [GET] http://localhost:64259/DeviceServices/GetUSerDetailsByUserID?UserID=2
        /// <param name="UserID"></param>
        ///<returns>results if success return User Deatails by id else invalid url</returns>
        [TokenAuthorization]
        public JsonResult GetUSerDetailsByUserID(int UserID)
        {
            if (UserID > 0)
            {
                UserViewModel Data = DSM.GetUserDetailId(UserID);
                if (Data != null)
                {
                    return Json(new { Result = true, Data = Data }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { Result = false, Message = "User details not found." }, JsonRequestBehavior.AllowGet);
                }

            }
            else
            {
                return Json(new { Result = false, Message = "Please input UserId." }, JsonRequestBehavior.AllowGet);
            }


        }

        /// <summary>
        /// Get User Details By User ID Service API
        /// [POST] http://localhost:64259/DeviceServices/EditUser?model
        /// <param name="model"></param>
        ///<returns>results if success return User Deatails by id else invalid url</returns>

        [TokenAuthorization]
        [HttpPost]
        public JsonResult EditUser(AppUserProfileViewModel model)
        {
            string fileImageName = string.Empty;
            bool isImageUpdate = false;


            if (model.IsUpdate == true)
            {
                isImageUpdate = true;
            }
            else
            {
                isImageUpdate = false;
            }


            if (model != null && model.UserID > 0)
            {

                //model.CountryID = 231; // 231-United State
                //model.State = 3890;   // 3890- Alabama

                //model.Picture = fileImageName;

                //RemoveModelStateItem("Password,NewPassword");

                if (ModelState.IsValid)
                {

                    if (isImageUpdate)
                    {
                        if (!string.IsNullOrEmpty(model.UserImage))
                        {
                            //byte[] bytes = Convert.FromBase64String(model.UserImage.Split(',')[1]);

                            ////Convert the Byte Array to Base64 Encoded string.
                            //string base64String = Convert.ToBase64String(bytes, 0, bytes.Length);

                            //***Save Base64 Encoded string as Image File***//

                            string base64String = model.UserImage;

                            //Convert Base64 Encoded string to Byte Array.
                            byte[] imageBytes = Convert.FromBase64String(base64String);

                            //Save the Byte Array as Image File.
                            string ImageName = Guid.NewGuid().ToString() + ".Jpeg";
                            string filephysicalPath = HttpContext.Server.MapPath("~/Content/images/" + ImageName);

                            System.IO.MemoryStream ms = new System.IO.MemoryStream(imageBytes, 0, imageBytes.Length);

                            // Convert byte[] to Image
                            ms.Write(imageBytes, 0, imageBytes.Length);
                            System.Drawing.Image image = System.Drawing.Image.FromStream(ms, true);
                            image.Save(filephysicalPath, System.Drawing.Imaging.ImageFormat.Jpeg);

                            model.UserImage = ImageName;
                        }
                        else
                        {
                            model.UserImage = "";
                        }
                    }



                    if (DSM.UserProfileUpdate(model))
                    {
                        return Json(new { Result = true,ID=model.UserID, Message = "Update User details Successfully." }, JsonRequestBehavior.AllowGet);
                    }

                    else
                    {
                        return Json(new { Result = false, Message = "Update User details failed." }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    var errors = new List<string>();
                    foreach (var state in ModelState)
                    {
                        foreach (var error in state.Value.Errors)
                        {
                            errors.Add(error.ErrorMessage);
                        }
                    }

                    return Json(new { Result = false, Errors = errors }, JsonRequestBehavior.AllowGet);
                }

            }
            else
            {
                return Json(new { Result = false, Message = "Update User details failed." }, JsonRequestBehavior.AllowGet);
            }

        }


        #region
        //    {

        //   "dateOfBirth": "1-5-1985",
        //    "firstName": "Amir",
        //    "gender": "M",
        //    "height": "6.00",
        //    "lastName": "Pournajib",
        //    "skin": "#e0ac69",
        //    "UnitNameID": 1,
        //    "UserID": 250,
        //    "UserName": "amir",
        //    "Weight": "90.00",
        //    "Email": "amir.pou@gmail.com",
        //    "CountryID": 101,
        //    "State": 41,
        //    "CityName": "kolkata",
        //    "Zip": "1234",
        //    "Institution": "",
        //    "UserRoleID": 2,
        //    "UserTypeID": 3,
        //    "Address": "Edmonton",
        //    "Phone": "545454545454",
        //    "MobileNo": "80131826147",
        //    "Picture": "",
        //    "GeoLocation": ""       

        //}
        #endregion
        [HttpPost]
        [TokenAuthorization]
        public JsonResult ProfileUpdate()
        {
            ProfileUpdateViewModel pvm = new ProfileUpdateViewModel();

            string FileID = string.Empty;


            if (Request.Files == null || Request.Files.Count <= 0)
            {
                return Json(new { Result = false, Message = "No image uploaded, data saving failed!" }, JsonRequestBehavior.AllowGet);
            }

            foreach (string file in Request.Files)
            {
                try
                {
                    var postedFile = Request.Files[file];

                    if (postedFile != null && postedFile.ContentLength > 0)
                    {
                        if (!postedFile.FileName.Trim().ToLower().Contains(".json"))
                        {
                            var path = Path.Combine(Server.MapPath("~/Content/images"), postedFile.FileName.Trim());
                            postedFile.SaveAs(path);
                            FileID = postedFile.FileName; //.Trim().Split('.')[0];
                        }
                        else
                        {
                            // read the file 1st

                            using (StreamReader r = new StreamReader(postedFile.InputStream))
                            {
                                string json = r.ReadToEnd();
                                pvm = JsonConvert.DeserializeObject<ProfileUpdateViewModel>(json);
                            }
                        }
                    }
                }
                catch (Exception Ex)
                {
                    return Json(new { Result = false, Message = "Profile Update failed." }, JsonRequestBehavior.AllowGet);
                }
            }


            if (pvm != null && pvm.userId > 0)
            {

                if (ModelState.IsValid)
                {
                    pvm.Picture = FileID;
                    if (DSM.ProfileUpdate(pvm))
                    {
                        return Json(new { Result = true, Message = "Profile Updated Successfully." }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { Result = false, Message = "Profile Update failed." }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    var errors = new List<string>();
                    foreach (var state in ModelState)
                    {
                        foreach (var error in state.Value.Errors)
                        {
                            errors.Add(error.ErrorMessage);
                        }
                    }

                    return Json(new { Result = false, Errors = errors }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(new { Result = false, Message = "Profile Update failed." }, JsonRequestBehavior.AllowGet);
            }

        }

        #endregion


        #region [****** Measurment ******]

        [HttpPost]
        [TokenAuthorization]
        public JsonResult estimate(RootObject model)
        {
            List<Int64> MeaseurmentID = new List<Int64>();

            var result = DSM.StoreUserMeasurementDtls(model, ref MeaseurmentID);

            if (result)
            {
                return Json(new { Result = result, Response = HttpStatusCode.OK, ID = MeaseurmentID, Message = "Data saved successfully" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Result = result, Response = HttpStatusCode.InternalServerError, Message = "Data saving failed" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [TokenAuthorization]
        public ActionResult GetMeasurmentDtlsList(int UserID, int PageIndex = 1, int PageSize = 10, DateTime? FromDate = null, DateTime? ToDate = null, int[] IDs = null)
        {
            var UserDtls = UMM.ServGetUserDetailByUserID(UserID);

            if (UserDtls != null && UserDtls.UserID > 0)
            {
                var lstMeasurmentDtls = DSM.GetMeasurmentDtlsList(UserID, PageIndex, PageSize, FromDate, ToDate,IDs);

                if (lstMeasurmentDtls.Count > 0)
                {
                    return Json(new { Result = true, Response = HttpStatusCode.OK, Data = lstMeasurmentDtls }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { Result = true, Response = HttpStatusCode.NoContent, Data = "" }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(new { Result = false, Response = HttpStatusCode.NotFound, Message = "Please provide valid userid." }, JsonRequestBehavior.AllowGet);
            }


        }

        #endregion


        #region[***** Activity ******]

        [HttpPost]
        [TokenAuthorization]
        public JsonResult StoreMotionRecordDtls(MotionRecordsViewModel model)
        {

            if (ModelState.IsValid)
            {
                int ActivityID = 0;

                if (DSM.InsertActivityDtls(model, ref ActivityID))
                {
                    return Json(new { Result = true, ID= ActivityID, Message = "Data stored Successfully." }, JsonRequestBehavior.AllowGet);
                }

                else
                {
                    return Json(new { Result = false, Message = "Data storing failed." }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                var errors = new List<string>();
                foreach (var state in ModelState)
                {
                    foreach (var error in state.Value.Errors)
                    {
                        errors.Add(error.ErrorMessage);
                    }
                }

                return Json(new { Result = false, Errors = errors }, JsonRequestBehavior.AllowGet);
            }

        }


        [HttpPost]
        [TokenAuthorization]
        public ActionResult GetMotionRecordList(int UserID, int PageIndex = 1, int PageSize = 10, DateTime? FromDate = null, DateTime? ToDate = null, int[] IDs = null)
        {
            var UserDtls = UMM.ServGetUserDetailByUserID(UserID);

            if (UserDtls != null && UserDtls.UserID > 0)
            {
                var lstActivity = WAM.GetWorkActivityList(UserID, PageIndex, PageSize, FromDate, ToDate,IDs);

                if (lstActivity.Count > 0)
                {
                    return Json(new { Result = true, Response = HttpStatusCode.OK, Data = lstActivity }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { Result = true, Response = HttpStatusCode.NoContent, Data = "" }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(new { Result = false, Response = HttpStatusCode.NotFound, Message = "Please provide valid userid." }, JsonRequestBehavior.AllowGet);
            }


        }

        #endregion


        #region[***** Sleep ******]

        [HttpPost]
        [TokenAuthorization]
        public JsonResult StoreSleepRecordDtls(SleepActivityViewModel model)
        {
            int SleepentityID = 0;

            if (DSM.InsertSleepRecord(model, ref SleepentityID))
            {

                return Json(new { Result = true,ID= SleepentityID, Message = "Data stored Successfully." }, JsonRequestBehavior.AllowGet);
            }

            else
            {
                return Json(new { Result = false, Message = "Data storing failed." }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [TokenAuthorization]
        public ActionResult GetSleepRecordDtlsList(int UserID, int PageIndex = 1, int PageSize = 10, DateTime? FromDate = null, DateTime? ToDate = null, int[] IDs = null)
        {
            var UserDtls = UMM.ServGetUserDetailByUserID(UserID);

            if (UserDtls != null && UserDtls.UserID > 0)
            {
                var lstActivity = SM.GetSleepActivityList(UserID, PageIndex, PageSize, FromDate, ToDate, IDs);

                if (lstActivity.Count > 0)
                {
                    return Json(new { Result = true, Response = HttpStatusCode.OK, Data = lstActivity }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { Result = true, Response = HttpStatusCode.NoContent, Data = "" }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(new { Result = false, Response = HttpStatusCode.NotFound, Message = "Please provide valid userid." }, JsonRequestBehavior.AllowGet);
            }


        }

        #endregion


        #region [******** Tag ******]

        //json - for api AddTagLabel
        //{
        //	"LabelName" : "Good",
        //	"MinRange" : 2 ,
        //	"MaxRange" : 30 ,
        //	"DefaultValue" : 10 ,
        //	"PrecisionDigit" : 5,
        // "ImageName":"fgfdgfdsgfdgdfgfdg",
        // "UnitName":" ",
        // "UserID": 2
        //}

        [HttpPost]
        [TokenAuthorization]
        public ActionResult AddTagLabel(TagLabelViewModel model)
        {
            if (ModelState.IsValid)     // validates the model with input parameters.
            {
                int TagLabelID = 0;
                if (!DSM.DuplicateTagLabel(model.LabelName, model.UserID,ref TagLabelID))
                {
                    if (!String.IsNullOrEmpty(model.ImageName))
                    {
                        try
                        {
                            string base64String = model.ImageName;
                            byte[] imageBytes = Convert.FromBase64String(base64String);

                            string ImageName = Guid.NewGuid().ToString() + ".Jpeg";
                            string filephysicalPath = HttpContext.Server.MapPath("~/Content/images/TagLabel/" + ImageName);

                            System.IO.MemoryStream ms = new System.IO.MemoryStream(imageBytes, 0, imageBytes.Length);

                            // Convert byte[] to Image
                            ms.Write(imageBytes, 0, imageBytes.Length);
                            System.Drawing.Image image = System.Drawing.Image.FromStream(ms, true);
                            image.Save(filephysicalPath, System.Drawing.Imaging.ImageFormat.Jpeg);

                            model.ImageName = ImageName;


                        }
                        catch (Exception Ex)
                        {
                            model.ImageName = "";
                        }

                    }
                    else
                    {
                        model.ImageName = "";
                    }

                    if (TLM.AddNewLabel(model))
                    {
                        return Json(new { Result = true, Response = HttpStatusCode.OK, ID =model.ID, Message = "Label created successfully." }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { Result = false, Message = "Label creation failed." }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json(new { Result = false,ID= TagLabelID, Message = "Duplicate Tag Label." }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                var errors = new List<string>();
                foreach (var state in ModelState)
                {
                    foreach (var error in state.Value.Errors)
                    {
                        errors.Add(error.ErrorMessage);
                    }
                }

                return Json(new { Result = false, Errors = errors }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [TokenAuthorization]
        public JsonResult EditTagLabel(TagLabelViewModel model)
        {
            int TagLabelID = model.ID;
            if (ModelState.IsValid)     // validates the model with input parameters.
            {
                if (!DSM.DuplicateTagLabel(model.LabelName, model.UserID,ref TagLabelID))
                {
                    if (!String.IsNullOrEmpty(model.ImageName))
                    {
                        try
                        {
                            string base64String = model.ImageName;
                            byte[] imageBytes = Convert.FromBase64String(base64String);

                            string ImageName = Guid.NewGuid().ToString() + ".Jpeg";
                            string filephysicalPath = HttpContext.Server.MapPath("~/Content/images/TagLabel/" + ImageName);

                            System.IO.MemoryStream ms = new System.IO.MemoryStream(imageBytes, 0, imageBytes.Length);

                            // Convert byte[] to Image
                            ms.Write(imageBytes, 0, imageBytes.Length);
                            System.Drawing.Image image = System.Drawing.Image.FromStream(ms, true);
                            image.Save(filephysicalPath, System.Drawing.Imaging.ImageFormat.Jpeg);

                            model.ImageName = ImageName;

                        }
                        catch (Exception Ex)
                        {
                            model.ImageName = "";
                        }

                    }
                    else
                    {
                        model.ImageName = "";
                    }

                    if (TLM.UpdateTagLabelByID(model))
                    {
                        return Json(new { Result = true, Response = HttpStatusCode.OK, ID = TagLabelID, Message = "Label edited successfully." }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { Result = false, Message = "Label edition failed." }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json(new { Result = false,ID= TagLabelID, Message = "Duplicate Tag Label." }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                var errors = new List<string>();
                foreach (var state in ModelState)
                {
                    foreach (var error in state.Value.Errors)
                    {
                        errors.Add(error.ErrorMessage);
                    }
                }

                return Json(new { Result = false, Errors = errors, ID = TagLabelID }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [TokenAuthorization]
        public ActionResult GetTagLabelList(int UserID, int PageIndex = 1, int PageSize = 10, DateTime? FromDate = null, DateTime? ToDate = null, int[] IDs = null)
        {
            var UserDtls = UMM.ServGetUserDetailByUserID(UserID);

            if (UserDtls != null && UserDtls.UserID > 0)
            {
                var lstTagLabel = TLM.GetAllTagLabelList(UserID, PageIndex, PageSize, FromDate, ToDate, IDs,"api");

                if (lstTagLabel.Count > 0)
                {
                    return Json(new { Result = true, Response = HttpStatusCode.OK, Data = lstTagLabel }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { Result = true, Response = HttpStatusCode.NoContent, Data = "" }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(new { Result = false, Response = HttpStatusCode.NotFound, Message = "Please provide valid userid." }, JsonRequestBehavior.AllowGet);
            }


        }

        [HttpPost]
        [TokenAuthorization]
        public ActionResult DeleteTagLabelByID(int TagLabelID)
        {

            if (TagLabelID > 0)
            {
                var result = TLM.DeleteLabelByID(TagLabelID);

                if (result)
                {
                    return Json(new { Result = true, Response = HttpStatusCode.OK, Message = "Tag label delete successfully." }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { Result = false, Response = HttpStatusCode.NoContent, Message = "Tag label delete failled." }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(new { Result = false, Response = HttpStatusCode.NotFound, Message = "Please provide valid userid." }, JsonRequestBehavior.AllowGet);
            }


        }


        //json - for api StoreTagRecordDtls
        //{
        //	"date" : "2020/02/06",
        //	"id" : 1 ,
        //	"note" : "u" ,
        //	"time" : "15:26:13" ,
        //	"type" : "Well-being",
        //	"userId" : 4 ,
        //	"value": 10
        //}
        [HttpPost]
        [TokenAuthorization]
        public ActionResult StoreTagRecordDtls(TagsHistServiceViewModel model)
        {
            int TagHistoryID = 0;

            if (ModelState.IsValid)
            {
                if (DSM.AddNewTag(model, ref TagHistoryID))
                {
                    return Json(new { Result = true, Message = "Tag History created successfully.", ID = TagHistoryID }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { Result = false, Message = "Tag History creation failed.", ID = TagHistoryID }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                var errors = new List<string>();
                foreach (var state in ModelState)
                {
                    foreach (var error in state.Value.Errors)
                    {
                        errors.Add(error.ErrorMessage);
                    }
                }

                return Json(new { Result = false, Errors = errors, ID = TagHistoryID }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [TokenAuthorization]
        public ActionResult EditTagRecordDtls(TagsHistServiceViewModel model)
        {
            int TagHistoryID = model.ID;

            if (ModelState.IsValid)
            {
                if (DSM.EditTag(model))
                {
                    return Json(new { Result = true, Message = "Tag History edited successfully.", ID = TagHistoryID }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { Result = false, Message = "Tag History edition failed.", ID = TagHistoryID }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                var errors = new List<string>();
                foreach (var state in ModelState)
                {
                    foreach (var error in state.Value.Errors)
                    {
                        errors.Add(error.ErrorMessage);
                    }
                }

                return Json(new { Result = false, Errors = errors, ID = TagHistoryID }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [TokenAuthorization]
        public ActionResult GetTagRecordList(int UserID, int PageIndex = 1, int PageSize = 10, DateTime? FromDate = null, DateTime? ToDate = null, int[] IDs=null)
        {
            var UserDtls = UMM.ServGetUserDetailByUserID(UserID);

            if (UserDtls != null && UserDtls.UserID > 0)
            {
                var lstTagLabel = THM.GetAllTagRecordList(UserID, PageIndex, PageSize, FromDate, ToDate, IDs);

                if (lstTagLabel.Count > 0)
                {
                    return Json(new { Result = true, Response = HttpStatusCode.OK, Data = lstTagLabel }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { Result = true, Response = HttpStatusCode.NoContent, Data = "" }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(new { Result = false, Response = HttpStatusCode.NotFound, Message = "Please provide valid userid." }, JsonRequestBehavior.AllowGet);
            }


        }

        #endregion


        #region [ :: USER MESSAGE ]

        // NEW USER MESSAGE API ON 17042020

        //json - for api ComposeMail
        //{
        //  "MessageFrom": "sandip.bhattacharjee@gmail.com",
        //	"MessageTo" :  "amitava.mukherjee@baseclass.co.in" ,
        //	"MessageCc" :  "mana.14111980@gmail.com"  ,
        //	"MessageSubject" : "test mail api" ,
        //	"MessageBody" : "test mail api body" 
        //}

        [HttpPost]
        [TokenAuthorization]
        public JsonResult Sendmessage(UserMessageViewModel model)
        {

            if (ModelState.IsValid)
            {

                if (!string.IsNullOrEmpty(model.RequestType) && !string.IsNullOrEmpty(model.UserFile) && !string.IsNullOrEmpty(model.FileExtension) && !model.FileExtension.Contains("exe"))
                {
                    try
                    {
                        //***Save Base64 Encoded string as Image File***//

                        string base64String = model.UserFile;

                        //Convert Base64 Encoded string to Byte Array.
                        byte[] FileBytes = Convert.FromBase64String(base64String);

                        //Save the Byte Array as Image File.
                        string filextension = model.FileExtension;
                        string SaveFileName = Guid.NewGuid().ToString() + "." + filextension;
                        string filephysicalPath = HttpContext.Server.MapPath("~/Content/EmailAttachments/" + SaveFileName);

                        System.IO.MemoryStream ms = new System.IO.MemoryStream(FileBytes, 0, FileBytes.Length);
                        ms.Write(FileBytes, 0, FileBytes.Length);

                        FileStream f = new FileStream(@filephysicalPath, FileMode.Create);

                        try
                        {
                            ms.WriteTo(f);
                            f.Flush();
                        }
                        finally
                        {
                            f.Close();
                        }


                        model.UserFile = SaveFileName;
                    }
                    catch (Exception Ex)
                    {


                    }

                }
                else
                {
                    model.UserFile = "";
                }

                if (!string.IsNullOrEmpty(model.MessageFrom))
                {

                    List<string> lstemilto = new List<string>();
                    List<string> lstemilCc = new List<string>();

                    if (!string.IsNullOrEmpty(model.MessageTo))
                    {
                        var msgto = model.MessageTo.ToString().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                        foreach (var itemto in msgto)
                        {
                            string item_to = itemto.ToString();
                            lstemilto.Add(item_to);
                        }
                        lstemilto = lstemilto.Distinct().ToList();
                    }

                    if (!string.IsNullOrEmpty(model.MessageCc))
                    {
                        var msgcc = model.MessageCc.ToString().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                        foreach (var itemcc in msgcc)
                        {
                            string item_cc = itemcc.ToString();
                            lstemilCc.Add(item_cc);
                        }
                        lstemilCc = lstemilCc.Distinct().ToList();
                    }

                    //differences will be the data
                    var lstFilteredCc = lstemilCc.RemoveAll(a => lstemilto.Contains(a));

                    var uvm = UMM.GetUserDetaislByEmailID(model.MessageFrom.Trim());

                    if (uvm != null && uvm.UserID > 0)
                    {
                        model.LoginUserID = uvm.UserID;
                        model.LoginUserEmail = uvm.Email;

                        if (UMM.ComposeNewMessage(model))
                        {
                            return Json(new { Result = true, ID = model.MessageReturnIDs, Response = HttpStatusCode.OK, Message = "Message successfully sent." }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            return Json(new { Result = false, Response = HttpStatusCode.ServiceUnavailable, Message = "Message sending failed." }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        return Json(new { Result = false, Response = HttpStatusCode.InternalServerError, Message = "MessageFrom should not blank, Message sending failed." }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json(new { Result = false, Response = HttpStatusCode.InternalServerError, Message = "MessageFrom should not blank, Message sending failed." }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                var errors = new List<string>();
                foreach (var state in ModelState)
                {
                    foreach (var error in state.Value.Errors)
                    {
                        errors.Add(error.ErrorMessage);
                    }
                }

                return Json(new { Result = false, Response = HttpStatusCode.ServiceUnavailable, Errors = errors }, JsonRequestBehavior.AllowGet);
            }
        }

        //json - for api MessageList
        //{
        //  "messagetype" : "inbox"
        //  "SenderUserID": 57 ,
        //	"pageSize" :  10 ,
        //	"pageNumber" :  1  ,
        //	
        //}


        [HttpPost]
        [TokenAuthorization]
        public JsonResult GetMessagelistByMessageTypeid(int UserID, int pageSize, int pageNumber, int MessageTypeid = 1, int[] IDs = null)
        {
            var unreadmessagecount = 0;


            var uvm = UMM.ServGetUserDetailByUserID(UserID);

            if (uvm != null && uvm.UserID > 0)
            {
                if (MessageTypeid > 0)
                {
                    List<UserMessageViewModel> lstinbox = UMM.FetchMessageList(UserID, pageNumber, pageSize, MessageTypeid,IDs);

                    if (lstinbox != null && lstinbox.Count > 0)
                    {
                        foreach (var item in lstinbox)
                        {
                            if (!item.IsViewed)
                            {
                                unreadmessagecount++;
                            }
                        }

                        return Json(new { Result = true, Response = HttpStatusCode.OK, TotalMessageCount = lstinbox[0].TotalRecords, UnreadMessageCount = unreadmessagecount, Data = lstinbox }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { Result = false, Response = HttpStatusCode.NoContent, Message = "No Record Found" }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json(new { Result = false, Response = HttpStatusCode.InternalServerError, Message = "Please provide valid messagetype i.e. 1 - inbox / 2 - sent / 3 - bin. " }, JsonRequestBehavior.AllowGet);
                }

            }
            else
            {
                return Json(new { Result = false, Response = HttpStatusCode.NotFound, Message = "Please provide valid UserID. " }, JsonRequestBehavior.AllowGet);
            }

        }

        // FETCH MESSAGE DETAILS BY MESSAGEID

        //json - for api ViewMessageByMessageID
        //{
        //  "MessageID" : "91"
        //	
        //}

        [HttpGet]
        [TokenAuthorization]
        public JsonResult GetMessagedetlsByMessageid(int Messageid)
        {

            var model = UMM.GetMessageDetailById(Messageid);

            if (model != null && model.MessageID > 0)
            {
                return Json(new { Result = true,ID= model.MessageID, Response = HttpStatusCode.OK, Data = model }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Result = false, Response = HttpStatusCode.NotFound, Message = "Invalid Message ID" }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        [TokenAuthorization]
        public JsonResult DeleteMessagebyId(int MessageID)
        {
            if (UMM.DeleteMessageByMessageID(MessageID))
            {
                return Json(new { Result = true,ID=MessageID, Response = HttpStatusCode.OK, Message = "Message deleted successfully." }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Result = false, Response = HttpStatusCode.ServiceUnavailable, Message = "Message deleted failed." }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [TokenAuthorization]
        public JsonResult MultipleDeleteMessages(int[] MessagesIds)
        {
            if (UMM.MessagesMarkAsDelete(MessagesIds))
            {
                return Json(new { Result = true,ID= MessagesIds, Response = HttpStatusCode.OK, Message = "Messages deleted successfully." }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Result = false, Response = HttpStatusCode.ServiceUnavailable, Message = "Messages deleted failed." }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [TokenAuthorization]
        public JsonResult EmptyMessagesFromTrash(int UserID)
        {
            var uvm = UMM.ServGetUserDetailByUserID(UserID);
            if (uvm != null && uvm.UserID > 0)
            {
                if (UMM.EmptyFromTrashByUserID(UserID))
                {
                    return Json(new { Result = true, Response = HttpStatusCode.OK, Message = "Trash messages deleted successfully." }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { Result = false, Response = HttpStatusCode.ServiceUnavailable, Message = "Trash messages deleted failed." }, JsonRequestBehavior.AllowGet);
                }

            }
            else
            {
                return Json(new { Result = false, Response = HttpStatusCode.NotFound, Message = "Please provide valid UserID. " }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [TokenAuthorization]
        public JsonResult MultipleMessageDeleteFromTrash(int[] MessagesIds)
        {
            if (UMM.MultipleMessageDeleteFromTrash(MessagesIds))
            {
                return Json(new { Result = true, ID= MessagesIds, Response = HttpStatusCode.OK, Message = "Messages deleted successfully." }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Result = false, Response = HttpStatusCode.ServiceUnavailable, Message = "Messages deleted failed." }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [TokenAuthorization]
        public JsonResult MarkAsReadByMessageID(int MessageId)
        {
            if (MessageId > 0)
            {
                if (UMM.MarkAsReadByMessageId(MessageId))
                {
                    return Json(new { Result = true,ID= MessageId, Response = HttpStatusCode.OK, Message = "Mark as read message successfully." }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { Result = false, Response = HttpStatusCode.ServiceUnavailable, Message = "Mark as read message failed." }, JsonRequestBehavior.AllowGet);
                }

            }
            else
            {
                return Json(new { Result = false, Response = HttpStatusCode.NotFound, Message = "Please provide valid MessageId. " }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [TokenAuthorization]
        public JsonResult MarkAsReadByMessageTypeID(int UserID, int MessageTypeId)
        {
            var uvm = UMM.ServGetUserDetailByUserID(UserID);
            if (uvm != null && uvm.UserID > 0)
            {
                if (MessageTypeId > 0)
                {
                    if (UMM.MarkAsReadByMessageType(UserID, MessageTypeId))
                    {
                        return Json(new { Result = true, Response = HttpStatusCode.OK, Message = "Mark as read messages successfully." }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { Result = false, Response = HttpStatusCode.ServiceUnavailable, Message = "Mark as read messages failed." }, JsonRequestBehavior.AllowGet);
                    }

                }
                else
                {
                    return Json(new { Result = false, Response = HttpStatusCode.InternalServerError, Message = "Please provide valid messagetype i.e. 1 - inbox / 2 - sent / 3 - bin. " }, JsonRequestBehavior.AllowGet);
                }


            }
            else
            {
                return Json(new { Result = false, Response = HttpStatusCode.NotFound, Message = "Please provide valid UserID. " }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [TokenAuthorization]
        public JsonResult SearchMessagesByTypeID(String SearchValue, int UserID, int MessageTypeId)

        {
            var uvm = UMM.ServGetUserDetailByUserID(UserID);

            if (uvm != null && uvm.UserID > 0)
            {
                if (MessageTypeId > 0)
                {
                    List<UserMessageViewModel> lstmessage = UMM.SearchMailByValue(SearchValue, UserID, MessageTypeId);

                    if (lstmessage != null && lstmessage.Count > 0)
                    {
                        return Json(new { Result = true, Response = HttpStatusCode.OK, Data = lstmessage }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { Result = false, Response = HttpStatusCode.NoContent, Message = "No Record Found" }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json(new { Result = false, Response = HttpStatusCode.InternalServerError, Message = "Please provide valid messagetype i.e. 1 - inbox / 2 - sent / 3 - bin. " }, JsonRequestBehavior.AllowGet);
                }

            }
            else
            {
                return Json(new { Result = false, Response = HttpStatusCode.NotFound, Message = "Please provide valid UserID. " }, JsonRequestBehavior.AllowGet);
            }

        }

        [HttpPost]
        [TokenAuthorization]
        public JsonResult RestoreMessageByID(int MessageID, int UserId)
        {
            var uvm = UMM.ServGetUserDetailByUserID(UserId);

            if (uvm != null && uvm.UserID > 0)
            {
                if (UMM.RestoreMessageByID(UserId, MessageID))
                {
                    return Json(new { Result = true, ID= MessageID,  Response = HttpStatusCode.OK, Message = "Message restored successfully." }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { Result = false, Response = HttpStatusCode.ServiceUnavailable, Message = "Message restored failed." }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(new { Result = false, Response = HttpStatusCode.NotFound, Message = "Please provide valid UserID." }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [TokenAuthorization]
        public JsonResult SendResponseByMessageIDAndTypeID(UserMessageViewModel model)
        {

            if (!string.IsNullOrEmpty(model.RequestType) && !string.IsNullOrEmpty(model.UserFile) && !string.IsNullOrEmpty(model.FileExtension) && !model.FileExtension.Contains("exe"))
            {
                try
                {
                    //***Save Base64 Encoded string as Image File***//

                    string base64String = model.UserFile;

                    //Convert Base64 Encoded string to Byte Array.
                    byte[] FileBytes = Convert.FromBase64String(base64String);

                    //Save the Byte Array as Image File.
                    string filextension = model.FileExtension;
                    string SaveFileName = Guid.NewGuid().ToString() + "." + filextension;
                    string filephysicalPath = HttpContext.Server.MapPath("~/Content/EmailAttachments/" + SaveFileName);

                    //System.IO.File.WriteAllBytes(@filephysicalPath, Convert.FromBase64String(base64String));


                    System.IO.MemoryStream ms = new System.IO.MemoryStream(FileBytes, 0, FileBytes.Length);

                    ////// Convert byte[] to Image
                    ms.Write(FileBytes, 0, FileBytes.Length);

                    FileStream f = new FileStream(@filephysicalPath, FileMode.Create);

                    try
                    {
                        ms.WriteTo(f);
                        f.Flush();
                    }
                    finally
                    {
                        f.Close();
                    }


                    model.UserFile = SaveFileName;
                }
                catch (Exception Ex)
                {

                }

            }
            else
            {
                model.UserFile = "";
            }

            if (ModelState.IsValid)
            {
                if (model.MessageID > 0)
                {
                    if (model.MsgResponseTypeID > 0)
                    {
                        if (!string.IsNullOrEmpty(model.MessageFrom))
                        {

                            List<string> lstemilto = new List<string>();
                            List<string> lstemilCc = new List<string>();

                            if (!string.IsNullOrEmpty(model.MessageTo))
                            {
                                var msgto = model.MessageTo.ToString().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                                foreach (var itemto in msgto)
                                {
                                    string item_to = itemto.ToString();
                                    lstemilto.Add(item_to);
                                }
                                lstemilto = lstemilto.Distinct().ToList();
                            }

                            if (!string.IsNullOrEmpty(model.MessageCc))
                            {
                                var msgcc = model.MessageCc.ToString().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                                foreach (var itemcc in msgcc)
                                {
                                    string item_cc = itemcc.ToString();
                                    lstemilCc.Add(item_cc);
                                }
                                lstemilCc = lstemilCc.Distinct().ToList();
                            }

                            //differences will be the data
                            var lstFilteredCc = lstemilCc.RemoveAll(a => lstemilto.Contains(a));

                            var uvm = UMM.GetUserDetaislByEmailID(model.MessageFrom.Trim());

                            if (uvm != null && uvm.UserID > 0)
                            {
                                model.LoginUserID = uvm.UserID;
                                model.LoginUserEmail = uvm.Email;

                                if (UMM.ComposeNewMessage(model))
                                {
                                    return Json(new { Result = true, ID = model.MessageReturnIDs, Response = HttpStatusCode.OK, Message = "Message successfully sent." }, JsonRequestBehavior.AllowGet);
                                }
                                else
                                {
                                    return Json(new { Result = false, Response = HttpStatusCode.ServiceUnavailable, Message = "Message sending failed." }, JsonRequestBehavior.AllowGet);
                                }
                            }
                            else
                            {
                                return Json(new { Result = false, Response = HttpStatusCode.InternalServerError, Message = "MessageFrom should not blank, Message sending failed." }, JsonRequestBehavior.AllowGet);
                            }
                        }
                        else
                        {
                            return Json(new { Result = false, Response = HttpStatusCode.InternalServerError, Message = "MessageFrom should not blank, Message sending failed." }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        return Json(new { Result = false, Response = HttpStatusCode.InternalServerError, Message = "Please provide valid MsgResponseTypeID i.e. 1 - Reply / 2 - ReplyToAll / 3 - Forward. , Message sending failed." }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json(new { Result = false, Response = HttpStatusCode.InternalServerError, Message = "Invalid MessageID , Message sending failed." }, JsonRequestBehavior.AllowGet);
                }

            }
            else
            {
                var errors = new List<string>();
                foreach (var state in ModelState)
                {
                    foreach (var error in state.Value.Errors)
                    {
                        errors.Add(error.ErrorMessage);
                    }
                }

                return Json(new { Result = false, Response = HttpStatusCode.ServiceUnavailable, Errors = errors }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion


        #region [ USER CONTACTS ]

        [HttpPost]
        [TokenAuthorization]
        public JsonResult SearchLeads(int LoggedinUserID, string SearchText)
        {
            if (string.IsNullOrEmpty(SearchText))
            {
                return Json(new { Result = false, Response = HttpStatusCode.InternalServerError, Message = "Please provide searchtext for searching." }, JsonRequestBehavior.AllowGet);
            }

            var uvm = UMM.ServGetUserDetailByUserID(LoggedinUserID);


            if (uvm != null && uvm.UserID > 0)
            {
                var lstcontacts = UCM.SearchLeads(LoggedinUserID, Server.MapPath("~/Content/images"), SearchText);

                if (lstcontacts != null && lstcontacts.Count > 0)
                {
                    return Json(new { Result = true, Response = HttpStatusCode.OK, Data = lstcontacts }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { Result = false, Response = HttpStatusCode.NoContent, Message = "No contacts found" }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(new { Result = false, Response = HttpStatusCode.NotFound, Message = "Please provide valid logged in userid." }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [TokenAuthorization]
        public JsonResult SendInvitation(int LoggedinUserID, int InviteeUserID)
        {
            var uvm = UMM.ServGetUserDetailByUserID(LoggedinUserID);

            bool IsValidInvitee = true;

            if (uvm != null && uvm.UserID > 0)
            {
                var inviteeuvm = UMM.ServGetUserDetailByUserID(InviteeUserID);

                if (inviteeuvm != null && inviteeuvm.UserID > 0)
                {
                    string ReturnMessage = string.Empty;

                    bool IsSent = UCM.SendInvitation(LoggedinUserID, InviteeUserID, ref IsValidInvitee, ref ReturnMessage);

                    if (IsSent)
                    {
                        return Json(new { Result = true, Response = HttpStatusCode.OK, Message = ReturnMessage }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        if (IsValidInvitee)
                        {
                            return Json(new { Result = false, Response = HttpStatusCode.InternalServerError, Message = "Invitation sending is failed." }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            return Json(new { Result = false, Response = HttpStatusCode.NotAcceptable, Message = "Invitation cannot be sent for this user, either this is already in contact or deleted contact or invitation pending or rejected." }, JsonRequestBehavior.AllowGet);
                        }
                    }
                }
                else
                {
                    return Json(new { Result = false, Response = HttpStatusCode.NotFound, Message = "Please provide valid invitee userid." }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(new { Result = false, Response = HttpStatusCode.NotFound, Message = "Please provide valid logged in userid." }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [TokenAuthorization]
        public JsonResult GetPendingInvitationList(int LoggedinUserID, string SearchKey = "", int[] IDs = null)
        {
            List<PendingInvitationViewModel> lst = new List<PendingInvitationViewModel>();

            var uvm = UMM.ServGetUserDetailByUserID(LoggedinUserID);


            if (uvm != null && uvm.UserID > 0)
            {
                List<UserContactsViewModel> lstinv = new List<UserContactsViewModel>();

                lstinv = UCM.GetPendingInvitationList(LoggedinUserID, Server.MapPath("~/Content/images"), SearchKey,IDs);

                if (lstinv != null && lstinv.Count > 0)
                {
                    foreach (var item in lstinv)
                    {
                        lst.Add(new PendingInvitationViewModel
                        {
                            ContactID = item.ContactID,
                            SenderUserID = item.FKSenderUserID,
                            SenderFirstName = item.FirstName,
                            SenderLastName = item.LastName,
                            SenderEmail = item.Email,
                            SenderPhone = item.Phone,
                            SenderPicture = item.Picture
                        });
                    }

                    return Json(new { Result = true, Response = HttpStatusCode.OK, Data = lst }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { Result = false, Response = HttpStatusCode.NoContent, Message = "No pending invitation found" }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(new { Result = false, Response = HttpStatusCode.NotFound, Message = "Please provide valid logged in userid." }, JsonRequestBehavior.AllowGet);
            }

        }

        [HttpPost]
        [TokenAuthorization]
        public JsonResult GetSendingInvitationList(int UserID, int[] IDs = null)
        {

            var uvm = UMM.ServGetUserDetailByUserID(UserID);

            if (uvm != null && uvm.UserID > 0)
            {

                var lstSendingInvitation = UCM.SendingInvitationList(UserID, IDs);

                var list = new List<Dictionary<string, object>>();

                if (lstSendingInvitation != null && lstSendingInvitation.Count > 0)
                {
                    foreach (var item in lstSendingInvitation)
                    {
                        var dict = new Dictionary<string, object>();

                        dict["UserID"] = item.FKReceiverUserID;
                        dict["FirstName"] = item.ReceiverFirstName;
                        dict["LastName"] = item.ReceiverLastName;

                        if (!String.IsNullOrEmpty(item.ReceiverPicture))
                        {
                            dict["Picture"] = GlobalConstants.BaseUrl + "/Content/images/" + item.ReceiverPicture;
                        }
                        else
                        {
                            dict["Picture"] = "";
                        }
                        
                        dict["CreatedDatetime"] = item.CreatedDatetimeString;

                        list.Add(dict);
                    }

                    return Json(new { Result = true, Response = HttpStatusCode.OK, Data = list }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { Result = false, Response = HttpStatusCode.NoContent, Message = "No sending invitation found" }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(new { Result = false, Response = HttpStatusCode.NotFound, Message = "Please provide valid userid." }, JsonRequestBehavior.AllowGet);
            }

        }

        [HttpPost]
        [TokenAuthorization]
        public JsonResult GetContactList(int LoggedinUserID, int PageNo, int PageSize, string SearchKey = "", string sortbyvalue = "accepteddate", int[] IDs = null)
        {
            var uvm = UMM.ServGetUserDetailByUserID(LoggedinUserID);

            if (uvm != null && uvm.UserID > 0)
            {
                List<UserContactsViewModel> lstinv = new List<UserContactsViewModel>();

                lstinv = UCM.GetUserContactList(LoggedinUserID, Server.MapPath("~/Content/images"), PageNo, PageSize, SearchKey, sortbyvalue,IDs);

                if (lstinv != null && lstinv.Count > 0)
                {
                    return Json(new { Result = true, Response = HttpStatusCode.OK, Data = lstinv }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { Result = true, Response = HttpStatusCode.NoContent, Message = "No contact found" }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(new { Result = false, Response = HttpStatusCode.NotFound, Message = "Please provide valid logged in userid." }, JsonRequestBehavior.AllowGet);
            }

        }

        [HttpPost]
        [TokenAuthorization]
        public JsonResult AcceptOrRejectInvitation(int ContactID, bool IsAccepted)
        {
            UserContactsViewModel model = UCM.GetContactDetailsByContactID(ContactID);

            if (model != null && model.FKSenderUserID > 0 && !model.IsAccepted && !model.IsRejected && !model.IsDeleted)
            {
                string mode = string.Empty;

                if (IsAccepted)
                {
                    mode = "accepted";

                    UCM.SaveContactAccept(model.ContactID); // IF ACCEPTED THEN SAVE A REVERSE ENTRY IN USERCONTACTS TABLE

                }
                else
                {
                    mode = "rejected";
                }

                if (UCM.UpdateContactsReplyType(model.ContactID, mode))
                {
                    return Json(new { Result = true, ID= model.ContactID, Response = HttpStatusCode.OK, Message = "Invitation " + mode + " successfully." }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { Result = false, Response = HttpStatusCode.InternalServerError, Message = "Invitation accept/reject is failed." }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(new { Result = false, Response = HttpStatusCode.NotFound, Message = "Please provide valid contactid." }, JsonRequestBehavior.AllowGet);
            }

        }


        [HttpPost]
        [TokenAuthorization]
        public JsonResult DeleteContactByUserId(int ContactFromUserID, int ContactToUserId)
        {
            var uvm = UMM.ServGetUserDetailByUserID(ContactFromUserID);
            var contactuserid = UMM.ServGetUserDetailByUserID(ContactToUserId);

            if (uvm != null && uvm.UserID > 0 && contactuserid != null && contactuserid.UserID > 0)
            {
                if (UCM.DeleteContactByUserId(ContactFromUserID, ContactToUserId))
                {
                    return Json(new { Result = true, Response = HttpStatusCode.OK, Message = "Contact deleted successfully." }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { Result = false, Response = HttpStatusCode.ServiceUnavailable, Message = "Contact deleted failed." }, JsonRequestBehavior.AllowGet);
                }

            }
            else
            {
                return Json(new { Result = false, Response = HttpStatusCode.NotFound, Message = "Please provide valid UserID. " }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [TokenAuthorization]
        public JsonResult GetContactListByType(int UserID, int PageIndex=1, int PageSize=10, string SearchKey = "", string sortbyvalue = "accepteddate", int ListType = (int)SDGUtilities.ContactTypeList.UserContact, int[] IDs=null)
        {
            var uvm = UMM.ServGetUserDetailByUserID(UserID);

            if (uvm != null && uvm.UserID > 0)
            {
                List<UserContactsViewModel> lstinv = new List<UserContactsViewModel>();

                if (ListType == (int)SDGUtilities.ContactTypeList.UserContact)
                {
                    lstinv = UCM.GetUserContactList(UserID, Server.MapPath("~/Content/images"), PageIndex, PageSize, SearchKey, sortbyvalue, IDs);

                }
                else if (ListType == (int)SDGUtilities.ContactTypeList.PendingContact)
                {
                    lstinv = UCM.GetPendingInvitationList(UserID, Server.MapPath("~/Content/images"), SearchKey, IDs);
                }

                if (lstinv != null && lstinv.Count > 0)
                {
                    return Json(new { Result = true, Response = HttpStatusCode.OK, Data = lstinv }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { Result = true, Response = HttpStatusCode.NoContent, Message = "No contact found" }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(new { Result = false, Response = HttpStatusCode.NotFound, Message = "Please provide valid logged in userid." }, JsonRequestBehavior.AllowGet);
            }

        }



        #endregion

        #region[*****Divice Information******]

        [HttpPost]
        [TokenAuthorization]
        public JsonResult StoreDeviceInfo(DeviceInformationViewModel model)
        {
            int retrunDeviceInfoID = 0;

            if (model.UserID > 0)
            {
                if (DSM.SaveDeviceInfo(model, ref retrunDeviceInfoID))
                {

                    return Json(new { Result = true, Response = HttpStatusCode.OK, ID = retrunDeviceInfoID, Message = "Data stored successfully." }, JsonRequestBehavior.AllowGet);
                }

                else
                {
                    return Json(new { Result = false, Response = HttpStatusCode.InternalServerError, Message = "Data storing failed." }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(new { Result = false, Response = HttpStatusCode.InternalServerError, Message = "UserID does not exists." }, JsonRequestBehavior.AllowGet);
            }
            
        }

        [HttpPost]
        [TokenAuthorization]
        public ActionResult GetDeviceInfoList(int UserID, int PageIndex = 1, int PageSize = 10, int[] IDs = null)
        {

            if (UserID > 0)
            {
                var lstDeviceInfo = DSM.GetDeviceList(UserID, PageIndex, PageSize, IDs);

                if (lstDeviceInfo.Count > 0)
                {
                    return Json(new { Result = true, Response = HttpStatusCode.OK, Data = lstDeviceInfo }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { Result = true, Response = HttpStatusCode.NoContent, Data = "" }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(new { Result = false, Response = HttpStatusCode.NotFound, Message = "Please provide UserID." }, JsonRequestBehavior.AllowGet);
            }


        }



        #endregion


        private void RemoveModelStateItem(String data)
        {
            try
            {
                String[] items = data.Split(',');
                foreach (String item in items)
                {
                    ModelState.Remove(item);
                }
            }
            catch { }
        }

    }

}