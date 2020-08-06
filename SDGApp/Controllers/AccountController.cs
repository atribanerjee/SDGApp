using SDGApp.Helpers;
using SDGApp.Models;
using SDGApp.ViewModel;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;

namespace SDGApp.Controllers
{
    public class AccountController : Controller
    {
        UserModel UM;
        MailHelper MH;
        BaseModel BM;
        ServiceModel SM;
        AttributeRuleModel ARM;
        DeviceServicesModel DM;
        public AccountController()
        {
            UM = new UserModel();
            MH = new MailHelper();
            BM = new BaseModel();
            SM = new ServiceModel();
            ARM = new AttributeRuleModel();
            DM = new DeviceServicesModel();
        }

        public ActionResult Index()
        {
            UserViewModel UVM = new UserViewModel();

            if (!String.IsNullOrEmpty(Convert.ToString(TempData["SuccessMessage"])))
            {
                ViewBag.SuccessMessage = Convert.ToString(TempData["SuccessMessage"]);
            }
            if (!String.IsNullOrEmpty(Convert.ToString(TempData["InfoMessage"])))
            {
                ViewBag.InfoMessage = Convert.ToString(TempData["InfoMessage"]);
            }
            if (!String.IsNullOrEmpty(Convert.ToString(TempData["ErrorMessage"])))
            {
                ViewBag.ErrorMessage = Convert.ToString(TempData["ErrorMessage"]);
            }

            return View(UVM);
        }
        // GET: Account
        [HttpGet]
        public ActionResult Login(string ReturnUrl)
        {

            UserViewModel Uvm = new UserViewModel();


            if (!String.IsNullOrEmpty(Convert.ToString(TempData["SuccessMessage"])))
            {
                ViewBag.SuccessMessage = Convert.ToString(TempData["SuccessMessage"]);
            }
            else
            {
                ViewBag.ErrorMessage = Convert.ToString(TempData["ErrorMessage"]);
            }

            if (Request.Cookies["SDGAppLogin"] != null)
            {
                Uvm.Email = Request.Cookies["SDGAppLogin"].Values["Email"];

                Uvm.RememberMe = true;
            }
            else
            {
                Uvm.Email = String.Empty;
                Uvm.RememberMe = false;
            }

            if (!String.IsNullOrEmpty(ReturnUrl))
            {
                Uvm.ReturnUrl = ReturnUrl;
            }


            return View(Uvm);
        }

        [HttpPost]
        public ActionResult Login(UserViewModel model)
        {
            RemoveModelStateItem("FirstName,LastName,State,City,Zip,Address,Email,NewPassword,ConfirmPassword");
            if (ModelState.IsValid)
            {
                if (UM.CheckLoginInfo(model))
                {
                    if (model.RememberMe)
                    {
                        HttpCookie cookie = new HttpCookie("SDGAppLogin");
                        cookie.Values.Add("Email", model.Email);
                        cookie.Expires = DateTime.Now.AddHours(24);
                        Response.Cookies.Add(cookie);
                    }
                    else
                    {
                        if (Request.Cookies["SDGAppLogin"] != null)
                        {
                            Response.Cookies["SDGAppLogin"].Expires = DateTime.Now.AddDays(-1);
                        }
                    }

                    if (String.IsNullOrEmpty(model.ReturnUrl))
                    {
                        return RedirectToAction("Index", "Dashboard");
                    }
                    else
                    {
                        return Redirect(model.ReturnUrl.Replace("_", "/"));
                    }
                }
                else
                {
                    TempData["ErrorMessage"] = "Invalid username or password";
                    return RedirectToAction("Login", "Account");
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Please input username & password";
            }

            // return View("Login",model);
            return RedirectToAction("Login", "Account");
        }

        [HttpPost]
        public ActionResult Register(UserViewModel model)
        {

            model.CountryID = 231; // 231-United State
            model.State = 3890;// 3890- Alabama

            RemoveModelStateItem("Password,Zip,Address,State");

            if (ModelState.IsValid)
            {
                if (!UM.CheckEmailID(model.Email) && !UM.CheckUserName(model.UserName) && model.RememberMe)
                {
                    AppUserRegisterViewModel appmodel = new AppUserRegisterViewModel();

                    appmodel.FirstName = model.FirstName;
                    appmodel.LastName = model.LastName;
                    appmodel.UserName = model.UserName;
                    appmodel.Email = model.Email;
                    appmodel.Phone = model.Phone;
                    appmodel.Password = model.NewPassword;



                    Int64 ID = 0;
                    if (DM.AddNewUser(appmodel, ref ID))
                    {
                        TempData["SuccessMessage"] = "You have registered successfully";
                        return RedirectToAction("Login", "Account");
                    }
                }
                else
                {
                    model.GenderNameList = UM.GetGenderList();
                    model.UserRoleList = UM.GetRoleList();
                    model.UserTypeList = UM.GetUserTypeList();
                    model.UserSkinTypeList = UM.GetUserSkinTypeList();
                    model.UserCompanyList = UM.GetCompanyList();
                    model.UnitNameList = UM.GetUnitNameList();

                    TempData["ErrorMessage"] = "Email or User Name already exists. Please try another.";
                    return RedirectToAction("Login", "Account");
                }

            }

            model.GenderNameList = UM.GetGenderList();
            model.UserRoleList = UM.GetRoleList();
            model.UserTypeList = UM.GetUserTypeList();
            model.UserSkinTypeList = UM.GetUserSkinTypeList();
            model.UserCompanyList = UM.GetCompanyList();
            model.UnitNameList = UM.GetUnitNameList();

            TempData["ErrorMessage"] = "Please Input required fields.";
            return RedirectToAction("Login", "Account");

        }

        [HttpGet]
        public ActionResult ForgetPassword()
        {
            UserViewModel UVM = new UserViewModel();
            return PartialView("_ForgetPassword", UVM);
        }

        [HttpPost]
        public ActionResult ForgetPassword(UserViewModel UVM)
        {
            RemoveModelStateItem("FirstName,LastName,State,City,Zip,Address,Password,NewPassword,ConfirmPassword,UserName");
            if (ModelState.IsValid)
            {

                Guid guid = Guid.NewGuid();
                UVM.GuID = guid.ToString();

                UserViewModel model = UM.GetUserDetailsByEmailAndSetNewGuID(UVM.Email, UVM.GuID);

                if (model.UserID > 0)
                {
                    UM.SaveGuID(UVM);
                    Dictionary<string, string> objDict = new Dictionary<string, string>();
                    objDict.Add("Pseudo", model.FirstName);
                    objDict.Add("Year", DateTime.Now.Year.ToString());

                    objDict.Add("ActivationUrl", GlobalConstants.BaseUrl + "/Account/ResetPassword?ID=" + UVM.GuID);
                    MH.SendEmail("Reset Password Requested", UVM.Email, "ForgotPassword.html", objDict);
                    TempData["SuccessMessage"] = "A reset password link has been sent to your registered email. Please check your email.";
                    return RedirectToAction("Login", "Account");
                }
                else
                {
                    TempData["ErrorMessage"] = "User Does not exist";
                    return RedirectToAction("Login", "Account");
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Invalid Email.";
            }

            return RedirectToAction("Login", "Account");
        }

        [HttpGet]
        public ActionResult ResetPassword(string ID)//guid
        {
            // UserModel um = new UserModel();
            UserViewModel uvm = new UserViewModel();
            uvm = UM.GetUserDetailByGUID(ID);
            if (!string.IsNullOrEmpty(uvm.FirstName))
            {

                return View(uvm);
            }
            else
            {
                TempData["ErrorMessage"] = "Invalid Url.";
                return RedirectToAction("Login", "Account");
            }
        }

        [HttpPost]
        public ActionResult ResetPassword(UserViewModel model)
        {
            RemoveModelStateItem("LastName,Password,State,City,Zip,Address,UserName");
            if (ModelState.IsValid)
            {
                if (UM.UpdatepasswordforUser(model.UserID, model.NewPassword))
                {
                    Dictionary<string, string> objDict = new Dictionary<string, string>();
                    objDict.Add("Pseudo", model.FirstName);
                    objDict.Add("Year", DateTime.Now.Year.ToString());

                    MH.SendEmail("Password Changed Successfully", model.Email, "ChangePasswordSuccessfully.html", objDict);
                    TempData["SuccessMessage"] = "Your Password has successfully changed";

                    return RedirectToAction("Login", "Account");
                }
                else
                {
                    ViewBag.ErrorMessage = "Reset Password failed";
                    return View(model);
                }
            }
            else
            {
                return View(model);
            }

        }

        [HttpGet]
        public ActionResult ForgetUserID()
        {
            UserViewModel UVM = new UserViewModel();
            return PartialView("_ForgetUserID", UVM);
        }

        [HttpPost]
        public ActionResult ForgetUserID(UserViewModel UVM)
        {
            // bool forgotpasswordorid = id;
            RemoveModelStateItem("FirstName,LastName,State,City,Zip,Address,Password,NewPassword,ConfirmPassword,UserName");
            if (ModelState.IsValid)
            {
                //// Captcha

                //var response = Request["g-recaptcha-response"];
                //string secretkey = SDGApp.GlobalConstants.RecaptchaPrivatekey;
                //var client = new WebClient();
                //var result = client.DownloadString(String.Format("https://www.google.com/recaptcha/api/siteverify?secret={0}&response={1}", secretkey, response));
                //var obj = Newtonsoft.Json.Linq.JObject.Parse(result);
                //var status = (bool)obj.SelectToken("success");
                //ViewBag.Message = status ? "Google recaptcha validation success" : "Google recptcha validation false";


                Guid guid = Guid.NewGuid();
                UVM.GuID = guid.ToString();

                UserViewModel model = UM.GetUserDetailsByEmailAndSetNewGuID(UVM.Email, UVM.GuID);

                if (model.UserID > 0)
                {
                    UM.SaveGuID(UVM);
                    Dictionary<string, string> objDict = new Dictionary<string, string>();
                    objDict.Add("Pseudo", model.FirstName);
                    objDict.Add("UserName", (model.UserName).ToString());
                    objDict.Add("Year", DateTime.Now.Year.ToString());

                    MH.SendEmail("Here is your User Name", UVM.Email, "ForgotUserName.html", objDict);
                    TempData["SuccessMessage"] = "A User name has been sent to your registered email. Please check your email.";
                    return RedirectToAction("Login", "Account");


                }
                else
                {
                    TempData["ErrorMessage"] = "Invalid email.";
                    return RedirectToAction("Login", "Account");
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Invalid email.";
            }
            // return PartialView("_ForgetPassword", UVM);
            return RedirectToAction("Login", "Account");
        }

        [UserAuthorization]
        [HttpGet]
        public ActionResult MyProfile()
        {
            Int32 LoggedInUserID = UM.GetLoggedInUserInfo().UserID;

            UserViewModel uvm = UM.GetUserDetailByUserID(LoggedInUserID);

            uvm.UserRole = UM.GetLoggedInUserInfo().UserRole;
            uvm.GenderNameList = UM.GetGenderList();
            uvm.UserSkinTypeList = UM.GetUserSkinTypeList();
            uvm.UnitNameList = UM.GetUnitNameList();

            return View(uvm);
        }

        [UserAuthorization]
        [HttpPost]
        public ActionResult MyProfile(UserViewModel model, string skincolor, HttpPostedFileBase userfile)
        {
            RemoveModelStateItem("Email,Password,NewPassword,ConfirmPassword,UserName,Zip,Address");
            if (ModelState.IsValid)
            {
                #region [ CODE FOR CROPPED PROFILE IMAGE ]

                string base64 = Request.Form["imgCropped"];
                if (!String.IsNullOrEmpty(base64))
                {
                    byte[] bytes = Convert.FromBase64String(base64.Split(',')[1]);
                    //Convert the Byte Array to Base64 Encoded string.
                    string base64String = Convert.ToBase64String(bytes, 0, bytes.Length);

                    //***Save Base64 Encoded string as Image File***//

                    //Convert Base64 Encoded string to Byte Array.
                    byte[] imageBytes = Convert.FromBase64String(base64String);

                    //Save the Byte Array as Image File.
                    string ImageName = Guid.NewGuid().ToString() + ".png";
                    string filephysicalPath = HttpContext.Server.MapPath("~/Content/images/" + ImageName);

                    System.IO.MemoryStream ms = new System.IO.MemoryStream(imageBytes, 0, imageBytes.Length);

                    // Convert byte[] to Image
                    ms.Write(imageBytes, 0, imageBytes.Length);
                    System.Drawing.Image image = System.Drawing.Image.FromStream(ms, true);
                    image.Save(filephysicalPath, System.Drawing.Imaging.ImageFormat.Png);

                    model.Picture = ImageName;  // CROPPED PROFILE IMAGE SET TO MODEL .
                }
               

                #endregion


                //TO UPDATE THE PROFILE
                if (UM.ProfileUpdateDtls(model, skincolor, userfile))
                {

                    TempData["SuccessMessage"] = "Your profile updated successfully.";
                    return RedirectToAction("Index", "Dashboard");
                }
                else
                {
                    ViewBag.ErrorMessage = "Profile update failed";

                    model.CountryNameList = UM.GetCountryList();

                    model.StateNameList = UM.GetDDLSateList(model.CountryID);

                    model.UserSkinTypeList = UM.GetUserSkinTypeList();
                    model.GenderNameList = UM.GetGenderList();
                    model.UnitNameList = UM.GetUnitNameList();

                    return View(model);
                }
            }
            model.CountryNameList = UM.GetCountryList();

            model.StateNameList = UM.GetDDLSateList(model.CountryID);

            model.UserSkinTypeList = UM.GetUserSkinTypeList();

            model.UnitNameList = UM.GetUnitNameList();

            model.GenderNameList = UM.GetGenderList();
            return View(model);
        }

        [UserAuthorization]
        public ActionResult UserDetails()
        {
            UserViewModel model = new UserViewModel();
            model = UM.GetLoggedInUserInfo();
            return PartialView("_UserDetails", model);
        }

        [HttpGet]
        public JsonResult GetStates(int CountryID)
        {
            UserModel um = new UserModel();
            List<State> lststate = new List<State>();
            lststate = um.GetSateList(CountryID);
            return Json(lststate, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetCity(int StateID)
        {
            UserModel um = new UserModel();
            List<City> lstcity = new List<City>();
            lstcity = um.GetCityList(StateID);
            return Json(lstcity, JsonRequestBehavior.AllowGet);
        }

        [UserAuthorization]
        [HttpGet]
        public ActionResult ChangePassword()
        {
            //HERE I ASSUMED THAT USER IS SAVED IN SESSION["USERID"]
            UserViewModel uvm = new UserViewModel();
            uvm.UserID = UM.GetLoggedInUserInfo().UserID;
            return View(uvm);
        }

        [UserAuthorization]
        [HttpPost]
        public ActionResult ChangePassword(UserViewModel uvm)
        {
            RemoveModelStateItem("FirstName,LastName,Email,State,City,Address,UserName,Zip");
            if (ModelState.IsValid)
            {
                //CHECK FOR PASSWORD EXISTS AND THEN CHANGE
                if (UM.ChangeExistingPassword(uvm.UserID, uvm.Password, uvm.ConfirmPassword))
                {
                    UserViewModel model = UM.GetUserDetailByUserID(uvm.UserID);

                    Dictionary<string, string> objDict = new Dictionary<string, string>();
                    objDict.Add("Pseudo", model.FirstName);
                    objDict.Add("Year", DateTime.Now.Year.ToString());

                    MH.SendEmail("Password Changed Successfully", model.Email, "ChangePasswordSuccessfully.html", objDict);
                    TempData["SuccessMessage"] = "Your Password has successfully changed";
                    return RedirectToAction("Index", "Dashboard");
                }
                else
                {
                    ViewBag.ErrorMessage = "Your current password is wrong.";
                    return View(uvm);
                }
            }
            else
            {
                return View(uvm);
            }
        }

        [UserAuthorization]
        public ActionResult LogOut()
        {
            Session.Abandon();
            return RedirectToAction("Login", "Account");
        }

        [UserAuthorization]
        public ActionResult Settings()
        {

            Int32 LoggedInUserID = UM.GetLoggedInUserInfo().UserID;

            UserViewModel uvm = UM.GetUserDetailByUserID(LoggedInUserID);

            uvm.CountryNameList = UM.GetCountryList();

            uvm.StateNameList = UM.GetDDLSateList(uvm.CountryID);

            // uvm.CityNameList = UM.GetDDLCityList(uvm.State);
            uvm.GenderNameList = UM.GetGenderList();

            uvm.UserSkinTypeList = UM.GetUserSkinTypeList();

            uvm.UnitNameList = UM.GetUnitNameList();

            return PartialView("_Settings", uvm);
        }

        [UserAuthorization]
        [HttpPost]
        public ActionResult Settings(UserViewModel model, string skincolor, HttpPostedFileBase file, string[] txtAttrLabel, string[] txtAttrValue)
        {
            RemoveModelStateItem("Email,Password,NewPassword,ConfirmPassword,UserName");
            if (ModelState.IsValid)
            {
                //TO UPDATE THE PROFILE
                if (UM.UserProfileUpdate(model, skincolor, file, txtAttrLabel, txtAttrValue))
                {
                    //  UM.SetLoggedInUserInfo(model);
                    TempData["SuccessMessage"] = "Your profile updated successfully.";
                    return RedirectToAction("Index", "Dashboard");
                }
                else
                {
                    ViewBag.ErrorMessage = "Profile update failed";

                    model.CountryNameList = UM.GetCountryList();

                    model.StateNameList = UM.GetDDLSateList(model.CountryID);

                    // model.CityNameList = UM.GetDDLCityList(model.State);

                    model.GenderNameList = UM.GetGenderList();

                    model.UnitNameList = UM.GetUnitNameList();

                    return PartialView("_Settings", model);
                }
            }
            model.CountryNameList = UM.GetCountryList();

            model.StateNameList = UM.GetDDLSateList(model.CountryID);

            //  model.CityNameList = UM.GetDDLCityList(model.State);

            model.GenderNameList = UM.GetGenderList();
            return PartialView("_Settings", model);
        }

        [UserAuthorization]
        public ActionResult Contacts()
        {

            Int32 LoggedInUserID = UM.GetLoggedInUserInfo().UserID;

            UserViewModel model = UM.GetUserDetailByUserID(LoggedInUserID);

            model.CountryNameList = UM.GetCountryList();

            model.StateNameList = UM.GetDDLSateList(model.CountryID);

            // model.CityNameList = UM.GetDDLCityList(model.State);

            return PartialView("_Contacts", model);
        }

        [UserAuthorization]
        public ActionResult ManageUser()
        {
            List<string> lstBreadcrumb = new List<string>();
            lstBreadcrumb.Add("Dashboard/Index");
            lstBreadcrumb.Add("Dashboard");
            lstBreadcrumb.Add("Manage User");
            ViewBag.lstbdcomb = lstBreadcrumb;

            if (BM.GetIntegerValue(BM.GetSessionValue("LoggedInUserRoleID")) == (int)SDGApp.Helpers.SDGUtilities.UserRoleType.Administrator)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Dashboard");
            }
        }

        public ActionResult UserList(int PageNumber, int PageSize, string SearchValue = "")
        {
            if (BM.GetIntegerValue(BM.GetSessionValue("LoggedInUserRoleID")) == BM.GetIntegerValue(SDGApp.Helpers.SDGUtilities.UserRoleType.Administrator))
            {
                UserViewModel UVM = new UserViewModel();
                UVM.IsActive = true;
                UVM.PageNumber = PageNumber;
                UVM.PageSize = PageSize;
                UVM.SearchValue = SearchValue;

                if (!String.IsNullOrEmpty(SearchValue))
                {
                    ViewBag.SearchValue = SearchValue;
                }

                return PartialView("_UserList", UM.GetAllUsersList(UVM));
            }
            else
            {
                return RedirectToAction("Index", "Dashboard");
            }
        }


        //public ActionResult UserContactList()
        //{
        //    List<UserDtlsViewModel> userlist = new List<UserDtlsViewModel>();

        //    if (BM.GetIntegerValue(BM.GetSessionValue("LoggedInUserRoleID")) == BM.GetIntegerValue(SDGApp.Helpers.SDGUtilities.UserRoleType.Administrator))
        //    {
        //        userlist = UM.GetAllUserForContactList();

        //        return PartialView("_ContactSideBar", userlist);
        //    }
        //    else
        //    {
        //        return PartialView("_ContactSideBar", userlist);
        //    }
        //}


        [UserAuthorization]
        [HttpGet]
        public ActionResult Edit(int ID)
        {

            List<string> lstBreadcrumb = new List<string>();
            lstBreadcrumb.Add("Account/ManageUser");
            lstBreadcrumb.Add("Manage User");
            lstBreadcrumb.Add("Edit User");
            ViewBag.lstbdcomb = lstBreadcrumb;

            if (BM.GetIntegerValue(BM.GetSessionValue("LoggedInUserRoleID")) == (int)SDGApp.Helpers.SDGUtilities.UserRoleType.Administrator)
            {
                UserViewModel uvm = new UserViewModel();

                uvm = UM.GetUserDetailByUserID(ID);

                uvm.GenderNameList = UM.GetGenderList();

                uvm.UserRoleList = UM.GetRoleList();

                uvm.UserTypeList = UM.GetUserTypeList();

                uvm.UserSkinTypeList = UM.GetUserSkinTypeList();

                uvm.DDLAttributeLabel = ARM.GetAttributeLabelByAttrType(6);
                uvm.UserCompanyList = UM.GetCompanyList();

                uvm.UnitNameList = UM.GetUnitNameList();

                return View(uvm);
            }
            else
            {
                return RedirectToAction("Index", "Dashboard");
            }
        }

        [UserAuthorization]
        [HttpPost]
        public ActionResult Edit(UserViewModel model, string skincolor, HttpPostedFileBase file, string[] txtAttrLabel, string[] txtAttrValue)
        {

            RemoveModelStateItem("Email,Password,NewPassword,ConfirmPassword,UserName,State");
            if (ModelState.IsValid)
            {
                //TO UPDATE THE PROFILE
                if (UM.UserProfileUpdate(model, skincolor, file, txtAttrLabel, txtAttrValue))
                {
                    ViewBag.SuccessMessage = "Your profile has updated successfully by admin.";
                    return RedirectToAction("ManageUser", "Account");
                }
                else
                {
                    ViewBag.ErrorMessage = "Profile update failed";

                    model.GenderNameList = UM.GetGenderList();
                    model.UserRoleList = UM.GetRoleList();
                    model.UserTypeList = UM.GetUserTypeList();
                    model.UserCompanyList = UM.GetCompanyList();
                    model.UnitNameList = UM.GetUnitNameList();

                    return View(model);
                }
            }

            model.GenderNameList = UM.GetGenderList();
            model.UserRoleList = UM.GetRoleList();
            model.UserTypeList = UM.GetUserTypeList();
            model.UserCompanyList = UM.GetCompanyList();
            model.UnitNameList = UM.GetUnitNameList();

            return View(model);
        }

        [UserAuthorization]
        [HttpPost]
        public JsonResult DeleteUser(int UserId)
        {
            if (UM.DeleteUserByUserID(UserId))
            {
                return Json(new { Result = true, Message = "User deleted successfully." }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Result = false, Message = "User deleted unsuccessful." }, JsonRequestBehavior.AllowGet);
            }

        }

        [UserAuthorization]
        [HttpGet]
        public ActionResult Add()
        {
            List<string> lstBreadcrumb = new List<string>();
            lstBreadcrumb.Add("Account/ManageUser");
            lstBreadcrumb.Add("Manage User");
            lstBreadcrumb.Add("Add User");
            ViewBag.lstbdcomb = lstBreadcrumb;

            UserViewModel uvm = new UserViewModel();

            uvm.GenderNameList = UM.GetGenderList();
            uvm.UserRoleList = UM.GetRoleList();
            uvm.UserTypeList = UM.GetUserTypeList();
            uvm.UserSkinTypeList = UM.GetUserSkinTypeList();
            uvm.UserCompanyList = UM.GetCompanyList();
            uvm.UnitNameList = UM.GetUnitNameList();

            return View(uvm);
        }

        [UserAuthorization]
        [HttpPost]
        public ActionResult Add(UserViewModel model, string SkinType, HttpPostedFileBase file, string[] txtAttrLabel, string[] txtAttrValue)
        {

            model.CountryID = 231; // 231-United State
            model.State = 3890;// 3890- Alabama

            RemoveModelStateItem("Password,Zip,Address,State");

            if (ModelState.IsValid)
            {
                if (!UM.CheckEmailID(model.Email) && !UM.CheckUserName(model.UserName))
                {

                    if (UM.AddNewUser(model, SkinType, file, txtAttrLabel, txtAttrValue))
                    {
                        ViewBag.SuccessMessage = "Your profile has updated successfully by admin.";
                        return RedirectToAction("ManageUser", "Account");
                    }
                }
                else
                {
                    model.GenderNameList = UM.GetGenderList();
                    model.UserRoleList = UM.GetRoleList();
                    model.UserTypeList = UM.GetUserTypeList();
                    model.UserSkinTypeList = UM.GetUserSkinTypeList();
                    model.UserCompanyList = UM.GetCompanyList();
                    model.UnitNameList = UM.GetUnitNameList();
                    ViewBag.ErrorMessage = "Email or User Name already exists. Please try another.";
                    return View(model);
                }

            }

            model.GenderNameList = UM.GetGenderList();
            model.UserRoleList = UM.GetRoleList();
            model.UserTypeList = UM.GetUserTypeList();
            model.UserSkinTypeList = UM.GetUserSkinTypeList();
            model.UserCompanyList = UM.GetCompanyList();
            model.UnitNameList = UM.GetUnitNameList();
            ViewBag.ErrorMessage = "Please Input required fields.";
            return View(model);

        }

        [UserAuthorization]
        [HttpPost]
        public JsonResult CheckDuplicateEmail(String Email, Int32 UserID)
        {
            if (UM.CheckDuplicateEmail(Email.Trim(), UserID))
            {
                return Json(new { Result = true }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Result = false }, JsonRequestBehavior.AllowGet);
            }
        }

        [UserAuthorization]
        [HttpPost]
        public JsonResult CheckDuplicateUserName(String UserName, Int32 UserID)
        {
            if (UM.CheckDuplicateUserName(UserName.Trim(), UserID))
            {
                return Json(new { Result = true }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Result = false }, JsonRequestBehavior.AllowGet);
            }
        }

        // Get Blood Pressure Report from Service
        public JsonResult GetBloodPresSurReport()
        {
            UM.GetBloodResult();
            return Json(new { }, JsonRequestBehavior.AllowGet);

        }

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