using Newtonsoft.Json;
using SDGApp.ViewModel;
using SDGAppDB;
using SDGAppDB.POCO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SDGApp.Models
{

    public class DeviceServicesModel : BaseModel
    {
        public UserViewModel CheckLogInService(UserViewModel model)
        {
            UserViewModel uvm = new UserViewModel();

            try
            {
                using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
                {
                    string EncryptedPwd = Encrypt(model.Password);

                    uvm = (from u in db.User
                           where u.UserName.Trim().ToLower() == model.UserName.Trim().ToLower() && u.Password == EncryptedPwd
                           && u.IsActive && !u.IsDeleted
                           select new UserViewModel
                           {
                               UserID = u.UserID,
                               FirstName = u.FirstName,
                               LastName = u.LastName,
                               //Address = u.Address,
                               Email = u.Email,
                               IsActive = u.IsActive,
                               IsDelete = u.IsDeleted,
                               Gender = u.Gender,
                               Height = (u.Height).ToString(),
                               Weight = (u.Weight).ToString(),
                               SkinType = u.SkinType,
                               MobileNo = u.Mobile,
                               UserName = u.UserName,
                               Phone = u.Phone,
                               Race = u.Race,
                               DateOfBirth = u.DOB != null ? u.DOB.ToString() : "",
                               ResearchName = u.ResearchersName,
                               UnitNameID = u.fkUnit ?? 1,
                               Picture = (string.IsNullOrEmpty(u.Picture) ? "" : GlobalConstants.BaseUrl + "/Content/images/" + u.Picture)

                           }).FirstOrDefault();

                    if (uvm != null && !String.IsNullOrEmpty(uvm.DateOfBirth))
                    {
                        uvm.DateOfBirth = Convert.ToDateTime(uvm.DateOfBirth).ToString("MMM dd yyyy");
                    }
                }

            }
            catch (Exception Ex)
            {
                WriteLog("SDGApp.Models.DeviceServicesModel - CheckLogInService", Ex.Message);
            }

            return uvm;


        }

        public UserViewModel GetUserDetailByServiceUserId(int UserID)
        {
            UserViewModel model = new UserViewModel();
            try
            {
                using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
                {
                    var entity = db.User.Find(UserID);

                    if (entity != null)
                    {
                        model.UserID = entity.UserID;
                        model.FirstName = entity.FirstName;
                        model.LastName = entity.LastName;
                        //model.Address = entity.Address;
                        //    model.LoginID = entity.LoginID;
                        model.IsActive = entity.IsActive;
                        model.IsDelete = entity.IsDeleted;

                        model.Email = entity.Email;  // added

                    }
                }
            }
            catch (Exception Ex)
            {
                WriteLog("SDGApp.Models.DeviceServicesModel - GetUserDetailByServiceUserId", Ex.Message);
            }
            return model;
        }

        public bool CheckUserName(string userName)
        {
            bool result = false;
            try
            {
                using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
                {
                    var UVM = (from u in db.User
                               where u.UserName.ToLower() == userName.Trim().ToLower() && !u.IsDeleted && u.IsActive
                               select new UserViewModel
                               {
                                   UserID = u.UserID

                               }).FirstOrDefault();

                    if (UVM != null)
                    {
                        result = true;
                    }
                }
            }
            catch (Exception Ex)
            {
                WriteLog("SDGApp.Models.DeviceServicesModel - CheckUserName", Ex.Message);
            }
            return result;
        }

        public bool CheckEmailID(string EmailID)
        {
            bool result = false;
            try
            {
                using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
                {
                    var UVM = (from u in db.User
                               where u.Email.ToLower() == EmailID.Trim().ToLower() && !u.IsDeleted && u.IsActive
                               select new UserViewModel
                               {
                                   UserID = u.UserID

                               }).FirstOrDefault();

                    if (UVM != null)
                    {
                        result = true;
                    }
                }
            }
            catch (Exception Ex)
            {
                WriteLog("SDGApp.Models.DeviceServicesModel - CheckEmailID", Ex.Message);
            }
            return result;
        }

        public bool AddNewUser(AppUserRegisterViewModel model, ref Int64 ID)
        {
            bool Result = false;

            try
            {
                using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
                {
                    var entity = new SDGAppDB.POCO.User();
                    entity.FirstName = model.FirstName;
                    entity.LastName = model.LastName;
                    entity.Password = Encrypt(model.Password);
                    entity.Email = model.Email;
                    entity.UserName = model.UserName;

                    entity.FKCountryID = 231; // 231-United State
                    //entity.FKCityID = 3; //
                    entity.FKStateID = 3890; // 3890- Alabama



                    entity.IsActive = true;
                    entity.IsDeleted = false;
                    entity.CreateDateTime = DateTime.Now;

                    entity.fkUserType = 3; // 3- Device user

                    db.User.Add(entity);
                    db.SaveChanges();

                    ID = entity.UserID;

                    if (ID > 0)
                    {
                        // Add at Company user role

                        var CompanyUserRoleEntity = new CompanyUserRole();

                        var companydtls = (from com in db.Company
                                           where com.CompanyName.ToLower() == "no company"
                                           select com).FirstOrDefault();

                        if (companydtls != null && companydtls.CompanyID > 0)
                        {
                            CompanyUserRoleEntity.FKCompanyID = companydtls.CompanyID;
                            CompanyUserRoleEntity.FKRoleID = 2;
                            CompanyUserRoleEntity.FKUserID = entity.UserID;

                            CompanyUserRoleEntity.JoiningDate = DateTime.Now;
                            db.CompanyUserRole.Add(CompanyUserRoleEntity);
                            db.SaveChanges();
                        }
                        else
                        {
                            CompanyUserRoleEntity.FKCompanyID = 3;
                            CompanyUserRoleEntity.FKRoleID = 2;
                            CompanyUserRoleEntity.FKUserID = entity.UserID;
                            CompanyUserRoleEntity.JoiningDate = DateTime.Now;
                            db.CompanyUserRole.Add(CompanyUserRoleEntity);
                            db.SaveChanges();
                        }

                        Result = true;
                    }
                }
            }
            catch (Exception Ex)
            {
                WriteLog("SDGApp.Models.DeviceServicesModel - AddNewUser", Ex.Message);
            }

            return Result;
        }

        public UserViewModel GetUserDetailsByEmailAndSetNewGuID(string email, string guID)
        {
            UserViewModel model = new UserViewModel();

            try
            {
                using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
                {
                    var UVM = db.User.Where(x => x.Email == email).FirstOrDefault();

                    if (UVM != null)
                    {
                        UVM.GuID = guID;
                        UVM.GuIDIsActive = true;
                        UVM.UserID = UVM.UserID;
                        UVM.FirstName = UVM.FirstName;
                        db.Entry(UVM).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                        // result = true;


                        model.GuID = guID;
                        model.GuIDIsActive = true;
                        model.UserID = UVM.UserID;
                        model.FirstName = UVM.FirstName;

                    }


                }

            }
            catch (Exception Ex)
            {
                WriteLog("SDGApp.Models.DeviceServicesModel - GetUserDetailsByEmailAndSetNewGuID", Ex.Message);
            }
            return model;
        }

        public UserViewModel GetUserDetailByGUID(string guid)
        {
            UserViewModel model = new UserViewModel();

            try
            {
                using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
                {
                    model = (from u in db.User
                             where u.GuID.Trim().ToLower() == guid.Trim().ToLower() && u.GuIDIsActive
                             select new UserViewModel
                             {
                                 UserID = u.UserID,
                                 FirstName = u.FirstName,
                                 LastName = u.LastName,
                                 //Address = u.Address,
                                 Email = u.Email
                             }).FirstOrDefault();

                }


                //IF NEEDED FILL MORE

            }
            catch (Exception Ex)
            {
                WriteLog("SDGApp.Models.DeviceServicesModel - GetUserDetailByGUID", Ex.Message);
            }
            return model;
        }


        public UserViewModel GetUserDetailId(int UserID)
        {
            UserViewModel model = new UserViewModel();

            try
            {
                using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
                {
                    model = (from u in db.User
                             join cur in db.CompanyUserRole on u.UserID equals cur.FKUserID
                             where u.UserID == UserID && u.IsActive == true && u.IsDeleted == false
                             select new UserViewModel
                             {
                                 UserID = u.UserID,
                                 FirstName = u.FirstName,
                                 LastName = u.LastName,
                                 MobileNo = u.Mobile,
                                 UserName = u.UserName,
                                 Email = u.Email,
                                 IsActive = u.IsActive,
                                 Company = u.Company,
                                 Gender = u.Gender,
                                 Height = (u.Height).ToString(),
                                 Weight = (u.Weight).ToString(),
                                 Race = u.Race,
                                 UserRoleID = cur.FKRoleID,
                                 UserTypeID = u.fkUserType,
                                 DateOfBirth = u.DOB != null ? u.DOB.ToString() : "",
                                 GeoLocation = u.Geolocation,
                                 ResearchName = u.ResearchersName,
                                 Institution = u.ResearchersInstitution,
                                 NotesHere = u.ResearchersNotes,
                                 SkinType = u.SkinType,
                                 GuID = u.GuID,
                                 GuIDIsActive = true,
                                 Picture = (string.IsNullOrEmpty(u.Picture) ? "" : GlobalConstants.BaseUrl + "/Content/images/" + u.Picture),
                                 UnitNameID = u.fkUnit != null ? (int)u.fkUnit : 1

                             }).FirstOrDefault();

                    if (!String.IsNullOrEmpty(model.DateOfBirth))
                    {
                        model.DateOfBirth = Convert.ToDateTime(model.DateOfBirth).ToString("MMM dd yyyy");
                    }
                }
            }

            catch (Exception Ex)
            {
                WriteLog("SDGApp.Models.DeviceServicesModel - GetUserDetailByUserId", Ex.Message);
            }
            return model;
        }


        public bool UserProfileUpdate(AppUserProfileViewModel model)
        {

            bool result = false;
            try
            {
                using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
                {
                    var UVM = db.User.Where(x => x.UserID == model.UserID).FirstOrDefault();

                    if (UVM != null)
                    {
                        UVM.FirstName = model.FirstName;
                        UVM.LastName = model.LastName;
                        UVM.Gender = model.Gender;

                        UVM.Height = GetDecimalPlaceValue(model.Height);
                        UVM.Weight = GetDecimalPlaceValue(model.Weight);

                        UVM.DOB = GetDateTimeValue(model.DateOfBirth);
                        UVM.SkinType = model.SkinType;

                        if (model.IsUpdate)
                        {
                            UVM.Picture = string.IsNullOrEmpty(model.UserImage) ? "" : model.UserImage;
                        }


                        UVM.fkUnit = GetIntegerValue(model.UnitID);

                        db.Entry(UVM).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();

                        result = true;
                    }
                }
            }
            catch (Exception Ex)
            {
                WriteLog("SDGApp.Models.DeviceServicesModel - UserProfileUpdate", Ex.Message);
            }

            return result;
        }

        public Boolean InsertActivityDtls(MotionRecordsViewModel model, ref int ActivityID)
        {
            Boolean result = false;

            try
            {
                using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
                {

                    if (model.userid > 0)
                    {
                        if (model.motionDate != null)
                        {

                            String strDate = model.motionDate.ToString("yyyy-MM-dd");
                            int Day = GetIntegerValue(strDate.Split('-')[2]);
                            int Month = GetIntegerValue(strDate.Split('-')[1]);
                            int Year = GetIntegerValue(strDate.Split('-')[0]);

                            var dateentity = (from w in db.WorkOutActivity
                                              where w.CreatedDateTime.Day == Day && w.CreatedDateTime.Month == Month &&
                                               w.CreatedDateTime.Year == Year
                                               && w.FKUserID == model.userid
                                              select new { w.ID }).FirstOrDefault();

                            //UPDATE
                            if (dateentity != null && dateentity.ID > 0)
                            {
                                var UPentity = db.WorkOutActivity.Find(dateentity.ID);
                                if (UPentity != null)
                                {
                                    UPentity.Steps = model.motionSteps;
                                    UPentity.KCal = model.motionCalorie;
                                    UPentity.Mileage = model.motionDistance;
                                    UPentity.CreatedDateTime = model.motionDate;


                                    db.Entry(UPentity).State = System.Data.Entity.EntityState.Modified;
                                    db.SaveChanges();
                                    ActivityID = UPentity.ID;
                                    result = true;
                                }
                            }
                            else
                            {
                                var insertentity = new SDGAppDB.POCO.WorkOutActivity();
                                insertentity.Steps = model.motionSteps;
                                insertentity.KCal = model.motionCalorie;
                                insertentity.Mileage = model.motionDistance;
                                insertentity.FKUserID = model.userid;
                                insertentity.CreatedDateTime = model.motionDate;


                                db.WorkOutActivity.Add(insertentity);
                                db.SaveChanges();

                                ActivityID = insertentity.ID;
                                result = true;

                            }
                        }

                    }

                }
            }
            catch (Exception Ex)
            {
            }

            return result;
        }

        public Boolean InsertSleepRecord(SleepActivityViewModel model, ref int SleepentityID)
        {
            Boolean result = false;
            try
            {
                using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
                {

                    if (model.userid > 0)
                    {

                        if (model.sleepDate != null)
                        {
                            String strDate = model.sleepDate.ToString("yyyy-MM-dd");
                            int Day = GetIntegerValue(strDate.Split('-')[2]);
                            int Month = GetIntegerValue(strDate.Split('-')[1]);
                            int Year = GetIntegerValue(strDate.Split('-')[0]);

                            var dateentity = (from s in db.SleepActivity
                                              where s.CreatedDateTime.Day == Day && s.CreatedDateTime.Month == Month &&
                                               s.CreatedDateTime.Year == Year
                                               && s.FKUserID == model.userid
                                              select new { s.ID }).FirstOrDefault();

                            //UPDATE
                            if (dateentity != null && dateentity.ID > 0)
                            {
                                var UPentity = db.SleepActivity.Find(dateentity.ID);

                                if (UPentity != null)
                                {
                                    UPentity.DeepSleepHour = model.sleepDeepTime;
                                    UPentity.LightSleepHour = model.sleepLightTime;
                                    UPentity.StayUPHour = model.sleepStayupTime;
                                    UPentity.CreatedDateTime = model.sleepDate;
                                    UPentity.SleepTotalTime = model.sleepTotalTime;
                                    UPentity.SleepWalkingNumber = model.sleepWalkingNumber;

                                    db.Entry(UPentity).State = System.Data.Entity.EntityState.Modified;
                                    db.SaveChanges();

                                    SleepentityID = UPentity.ID;


                                    if (model.SleepData != null)
                                    {
                                        //Remove sleepdatadtl records

                                        var list = db.SleepDataDtls.Where(sd => sd.FKSleepActivityID == UPentity.ID).ToList();
                                        // Use Remove Range function to delete all records at once
                                        db.SleepDataDtls.RemoveRange(list);
                                        // Save changes
                                        db.SaveChanges();

                                        //Insert Into SleepDataDtls Table 
                                        var entity = new SDGAppDB.POCO.SleepDataDtls();

                                        foreach (var item in model.SleepData)
                                        {
                                            entity.FKSleepTypeID = item.sleep_type;
                                            entity.StartTime = item.startTime;

                                            entity.FKSleepActivityID = UPentity.ID;

                                            db.SleepDataDtls.Add(entity);
                                            db.SaveChanges();

                                        }
                                    }


                                    result = true;
                                }
                            }
                            else//INSERT
                            {
                                //Insert Into SleepActivity Table

                                var insertentity = new SDGAppDB.POCO.SleepActivity();
                                insertentity.DeepSleepHour = model.sleepDeepTime;
                                insertentity.LightSleepHour = model.sleepLightTime;
                                insertentity.StayUPHour = model.sleepStayupTime;
                                insertentity.FKUserID = model.userid;
                                insertentity.CreatedDateTime = model.sleepDate == null ? DateTime.Now : model.sleepDate;
                                insertentity.SleepTotalTime = model.sleepTotalTime;
                                insertentity.SleepWalkingNumber = model.sleepWalkingNumber;

                                db.SleepActivity.Add(insertentity);
                                db.SaveChanges();


                                //Insert Into SleepDataDtls Table 

                                if (model.SleepData != null)
                                {
                                    var entity = new SDGAppDB.POCO.SleepDataDtls();

                                    foreach (var item in model.SleepData)
                                    {
                                        entity.FKSleepTypeID = item.sleep_type;
                                        entity.StartTime = item.startTime;

                                        entity.FKSleepActivityID = insertentity.ID;

                                        db.SleepDataDtls.Add(entity);
                                        db.SaveChanges();

                                    }
                                }
                                result = true;
                            }

                        }
                    }

                }

            }
            catch (Exception Ex)
            {

            }
            return result;
        }

        public bool ProfileUpdate(ProfileUpdateViewModel model)
        {
            bool result = false;
            try
            {
                using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
                {
                    var UVM = db.User.Where(x => x.UserID == model.userId).FirstOrDefault();

                    if (UVM != null)
                    {
                        UVM.Picture = model.Picture;


                        UVM.FirstName = model.firstName;
                        UVM.LastName = model.lastName;
                        UVM.Zip = model.Zip;
                        UVM.Address = model.Address;
                        UVM.Mobile = model.MobileNo;
                        UVM.UserName = model.userName;
                        UVM.Email = model.Email;
                        UVM.Phone = model.Phone;
                        UVM.Gender = model.gender;

                        var CountryEntity = db.Country.Where(x => x.CountryID == model.CountryID).FirstOrDefault();
                        if (CountryEntity != null)
                        {
                            UVM.FKCountryID = model.CountryID;
                        }

                        UVM.CityName = model.CityName;

                        var StateEntity = db.StateDetail.Where(x => x.StateID == model.State).FirstOrDefault();
                        if (StateEntity != null)
                        {
                            UVM.FKStateID = model.State;
                        }

                        UVM.Height = GetDecimalPlaceValue(model.height);
                        UVM.Weight = GetDecimalPlaceValue(model.weight);

                        var UserTypeEntity = db.UserType.Where(x => x.UserTypeID == model.UserTypeID).FirstOrDefault();
                        if (UserTypeEntity != null)
                        {
                            UVM.fkUserType = model.UserTypeID;
                        }

                        UVM.DOB = GetDateTimeValue(model.dateOfBirth);
                        UVM.Geolocation = model.GeoLocation;
                        UVM.ResearchersInstitution = model.Institution;
                        UVM.SkinType = model.skin;



                        db.Entry(UVM).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();

                        var UVMRol = db.UserRole.Where(x => x.FKUserID == model.userId).FirstOrDefault();
                        if (UVMRol != null)
                        {
                            UVMRol.FKRoleID = model.UserRoleID;

                            db.Entry(UVMRol).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();
                        }

                        result = true;

                    }
                }
            }
            catch (Exception Ex)
            {
                WriteLog("SDGApp.Models.DeviceServicesModel - ProfileUpdate", Ex.Message);
            }

            return result;
        }

        public DateTime UnixTimeToDateTime(object unixtime)
        {
            System.DateTime dtDateTime;
            try
            {
                if (unixtime != null)
                {
                    BaseModel BM = new BaseModel();
                    long unixtime1 = BM.GetLontValue(unixtime);

                    dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
                    dtDateTime = dtDateTime.AddMilliseconds(unixtime1).ToLocalTime();
                }
                else
                {
                    dtDateTime = DateTime.Now;
                }
            }
            catch (Exception Ex)
            {

                dtDateTime = DateTime.Now;
                WriteLog("SDGApp.Models.DeviceServicesModel - UnixTimeToDateTime", Ex.Message);
            }


            return dtDateTime;
        }


        public ThirdPartyMeasurmentViewModel GetMeasurmentDtlsByUserID(String PublicKey, String UserCode, DateTime FromDate, DateTime ToDate, int PageIndex = 1, int PageSize = 10)
        {

            List<SleepActivityViewModel> lstSleep = new List<SleepActivityViewModel>();
            List<WorkActivityModel> lstActivity = new List<WorkActivityModel>();

            ThirdPartyMeasurmentViewModel MesurmentDtls = new ThirdPartyMeasurmentViewModel();
            List<CardiacDtls> lstcardiacDtls = new List<CardiacDtls>();
            try
            {
                using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
                {
                    var CompanyID = (from ut in db.UserThirdPartyAPIKey
                                     where ut.APIKeyValue.Trim().ToLower() == PublicKey.Trim().ToLower() && ut.IsActive == true && ut.IsDeleted == false
                                     select ut.FKCompanyID).FirstOrDefault();
                    if (CompanyID > 0)
                    {

                        var UserId = (from c in db.CompanyUserRole
                                      where c.UserCode.Trim().ToLower() == UserCode.Trim().ToLower() && c.FKCompanyID == CompanyID
                                      select c.FKUserID).FirstOrDefault();

                        if (UserId > 0)
                        {
                            DateTime EndDate = ToDate.AddDays(1);

                            var jsonfilename = (from um in db.UserMeasurement
                                                where um.FKUserId == UserId
                                                orderby um.ID descending
                                                select um.FileName).ToList();


                            // var WorkActivityDtls = 

                            SleepModel sleepModel = new SleepModel();
                            WorkActivityModel workActivityModel = new WorkActivityModel();

                            var sleepDtls = sleepModel.GetSleepActivityList(UserId, PageIndex, PageSize, FromDate, ToDate);

                            var WorkActivityDtls = workActivityModel.GetWorkActivityList(UserId, PageIndex, PageSize, FromDate, ToDate);


                            if (jsonfilename != null && jsonfilename.Count > 0)
                            {

                                foreach (var item in jsonfilename)
                                {

                                    var splitjsonfilename = item.Split('_');

                                    String filePath = "~/Content/Measurement/" + UserId + "/" + splitjsonfilename[2] + "/" + item;

                                    string rootdir = System.Web.HttpContext.Current.Server.MapPath(filePath);

                                    if (System.IO.File.Exists(rootdir))
                                    {
                                        string Jsonfileread = System.IO.File.ReadAllText(rootdir);

                                        if (!String.IsNullOrEmpty(Jsonfileread))
                                        {

                                            CardiacDtls CardiacModel = new CardiacDtls();
                                            RootObject model = new RootObject();

                                            model = JsonConvert.DeserializeObject<RootObject>(Jsonfileread);

                                            if (model != null)
                                            {

                                                CardiacModel.SBP = GetStringValue(model.data.FirstOrDefault().sys_device);
                                                CardiacModel.DBP = GetStringValue(model.data.FirstOrDefault().dias_device);
                                                CardiacModel.HR = GetStringValue(model.data.FirstOrDefault().hr_device);
                                                CardiacModel.HRV = GetStringValue(model.data.FirstOrDefault().hrv_device);

                                                DateTime DateInfo = UnixTimeToDateTime(model.data.FirstOrDefault().timestamp);
                                                CardiacModel.CreatedDateTime = DateInfo.ToString("yyyy-MM-dd HH:mm:ss");
                                            }

                                            //Add Third Party mesurment into list
                                            lstcardiacDtls.Add(CardiacModel);
                                        }

                                    }

                                }//end foreach
                            }

                            MesurmentDtls.Sleep = sleepDtls;

                            MesurmentDtls.Activity = WorkActivityDtls;

                            MesurmentDtls.Cardiac = lstcardiacDtls;

                        }

                    }
                }
            }
            catch (Exception Ex)
            {
                WriteLog("SDGApp.Models.DeviceServicesModel - GetEstimateDtlsByUserID", Ex.Message);
            }

            return MesurmentDtls;

        }

        public List<RootObject> GetMeasurmentDtlsList(int UserID, int PageNumber, int PageSize, DateTime? FromDate = null, DateTime? ToDate = null, int[] IDs = null)
        {
            RootObject model = new RootObject();
            List<RootObject> lstRootObject = new List<RootObject>();

            List<String> lstjsonfilename = new List<string>();

            List<UserMeasurementViewModel> lst = new List<UserMeasurementViewModel>();


            try
            {
                using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
                {
                    if (UserID > 0)
                    {
                        if (FromDate != null && ToDate != null)
                        {
                            DateTime newdate = ToDate.Value.AddDays(1);

                            lst = (from um in db.UserMeasurement
                                   where um.FKUserId == UserID && !String.IsNullOrEmpty(um.FileName)
                                   && (um.CreatedDateTime >= FromDate && um.CreatedDateTime <= newdate)
                                   orderby um.ID descending
                                   select new UserMeasurementViewModel
                                   {
                                       ID = um.ID,
                                       FKUserId = um.FKUserId,
                                       FileName = um.FileName,
                                       CreatedDateTime = um.CreatedDateTime
                                   }).ToList();


                        }
                        else
                        {
                            lst = (from um in db.UserMeasurement
                                   where um.FKUserId == UserID && !String.IsNullOrEmpty(um.FileName)
                                   orderby um.ID descending
                                   select new UserMeasurementViewModel
                                   {
                                       ID = um.ID,
                                       FKUserId = um.FKUserId,
                                       FileName = um.FileName,
                                       CreatedDateTime = um.CreatedDateTime
                                   }).ToList();
                        }



                        if (lst != null && lst.Count > 0)
                        {
                            if (IDs != null && IDs.Length > 0)
                            {
                                foreach (var item in IDs)
                                {
                                    lst.Remove(lst.Single(s => s.ID == item)); // Remove IDs from List 
                                }

                            }

                            // Skip and Tack recods
                            lst = lst.Skip(SkipRecords(PageSize, PageNumber)).Take(PageSize).ToList();

                            foreach (var item in lst)
                            {
                                if (!String.IsNullOrEmpty(item.FileName))
                                {
                                    var splitjsonfilename = item.FileName.Split('_');

                                    String filePath = "~/Content/Measurement/" + UserID + "/" + splitjsonfilename[2] + "/" + item.FileName;

                                    string rootdir = System.Web.HttpContext.Current.Server.MapPath(filePath);

                                    if (System.IO.File.Exists(rootdir))
                                    {
                                        string Jsonfileread = System.IO.File.ReadAllText(rootdir);

                                        if (!String.IsNullOrEmpty(Jsonfileread))
                                        {

                                            model = JsonConvert.DeserializeObject<RootObject>(Jsonfileread);

                                            model.ID = GetIntegerValue(splitjsonfilename[0]);

                                            lstRootObject.Add(model);
                                        }

                                    }
                                }

                            }
                        }

                    }

                }

            }
            catch (Exception Ex)
            {
                WriteLog("SDGApp.Models.DeviceServicesModel - GetMeasurmentDtlsList", Ex.Message);
            }
            return lstRootObject;
        }

        public Boolean StoreUserMeasurementDtls(RootObject model, ref List<Int64> MeaseurmentID)
        {
            Boolean result = false;

            try
            {
                using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
                {
                    foreach (var item in model.data)
                    {
                        var userEntity = (from u in db.User where u.UserID == item.userID && u.IsActive && !u.IsDeleted select u).FirstOrDefault();

                        if (userEntity != null && userEntity.UserID > 0)
                        {
                            var entity = new SDGAppDB.POCO.UserMeasurement();

                            entity.FKUserId = userEntity.UserID;
                            entity.CreatedDateTime = UnixTimeToDateTime(item.timestamp);

                            db.UserMeasurement.Add(entity);
                            db.SaveChanges();

                            if (entity.ID > 0)
                            {

                                String jsonfilename = entity.ID + "_" + entity.FKUserId + "_" + entity.CreatedDateTime.ToString("yyyy-MM-dd") + "_Measurement.json";

                                if (WriteUserMeasurement(entity.FKUserId, jsonfilename, entity.CreatedDateTime.ToString("yyyy-MM-dd"), model))
                                {
                                    var Usermeassurment = db.UserMeasurement.Find(entity.ID);
                                    if (Usermeassurment != null)
                                    {
                                        Usermeassurment.FileName = jsonfilename;

                                        db.Entry(Usermeassurment).State = System.Data.Entity.EntityState.Modified;
                                        db.SaveChanges();

                                        MeaseurmentID.Add(entity.ID);
                                        result = true;
                                    }

                                }
                                else
                                {
                                    //Mesurment Json file writing failed
                                    String subject = "Mesurment Data Storing Failed";
                                    SendMailFailReport(subject, "Failed to write data unknown reason");
                                }
                            }

                        }
                    }

                }
            }
            catch (Exception Ex)
            {
                //Mesurment Json file writing failed
                String subject = "Mesurment Data Storing Failed";
                SendMailFailReport(subject, Ex.Message);

                //WriteLog("SDGApp.Models.DeviceServicesModel - StoreUserMeasurementDtls", Ex.Message);
            }
            return result;
        }


        #region [ :: TAG HISTORY DETAILS ]

        public bool UpdateTagByID(TagsHistServiceViewModel tvm)
        {
            bool Result = false;
            try
            {
                using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
                {
                    var entity = db.TagsMaster.Find(tvm.ID);

                    if (entity != null && entity.ID > 0)
                    {
                        var taglabelEntity = (from taglabel in db.TagLabel
                                              where taglabel.LabelName.ToLower().Contains(tvm.type.ToLower())
                                              select new { taglabel }).FirstOrDefault();

                        if (taglabelEntity != null && taglabelEntity.taglabel.ID > 0)
                        {
                            entity.FKTagLabelID = taglabelEntity.taglabel.ID;
                        }


                        entity.TagValue = tvm.value;
                        entity.Note = tvm.note;
                        entity.CreatedDateTime = GetNotNullDateTimeValue(tvm.date + " " + tvm.time);

                        db.Entry(entity).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                        Result = true;
                    }
                }
            }
            catch (Exception Ex)
            {
                WriteLog("SDGApp.Models.DeviceServicesModel - UpdateTagByID", Ex.Message);
            }
            return Result;

        }

        public bool AddNewTag(TagsHistServiceViewModel model, ref int ID)
        {
            bool Result = false;

            try
            {
                using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
                {
                    var entity = new SDGAppDB.POCO.TagsMaster();

                    var taglabelEntity = (from taglabel in db.TagLabel
                                          where taglabel.LabelName.ToLower().Contains(model.type.ToLower())
                                          select new { taglabel }).FirstOrDefault();

                    if (taglabelEntity != null && taglabelEntity.taglabel.ID > 0)
                    {
                        entity.FKTagLabelID = taglabelEntity.taglabel.ID;
                    }

                    entity.TagValue = model.value;
                    entity.Note = model.note;

                    var userEntity = db.User.Find(model.userId);
                    if (userEntity != null && userEntity.UserID > 0)
                    {
                        entity.FKUserID = userEntity.UserID;
                    }

                    entity.CreatedDateTime = GetNotNullDateTimeValue(model.date + " " + model.time);

                    db.TagsMaster.Add(entity);
                    db.SaveChanges();

                    if (entity.ID > 0)
                    {
                        var lastid = entity.ID;
                        model.ID = lastid;
                        ID = entity.ID;
                    }

                    Result = true;
                }
            }
            catch (Exception Ex)
            {
                WriteLog("SDGApp.Models.DeviceServicesModel - AddNewTag", Ex.Message);
            }


            return Result;
        }

        public bool EditTag(TagsHistServiceViewModel model)
        {
            bool Result = false;

            try
            {
                using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
                {
                    var entity = db.TagsMaster.Find(model.ID);

                    if (entity != null && entity.ID > 0)
                    {
                        var taglabelEntity = (from taglabel in db.TagLabel
                                              where taglabel.LabelName.ToLower().Contains(model.type.ToLower())
                                              select new { taglabel }).FirstOrDefault();

                        if (taglabelEntity != null && taglabelEntity.taglabel.ID > 0)
                        {
                            entity.FKTagLabelID = taglabelEntity.taglabel.ID;
                        }

                        entity.TagValue = model.value;
                        entity.Note = model.note;

                        var userEntity = db.User.Find(model.userId);
                        if (userEntity != null && userEntity.UserID > 0)
                        {
                            entity.FKUserID = userEntity.UserID;
                        }

                        entity.CreatedDateTime = GetNotNullDateTimeValue(model.date + " " + model.time);


                        db.Entry(entity).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                        Result = true;
                    }
                }
            }
            catch (Exception Ex)
            {
                WriteLog("SDGApp.Models.DeviceServicesModel - EditTag", Ex.Message);
            }


            return Result;
        }

        public bool AddNewLabel(TagLabelViewModel model)
        {
            bool Result = false;
            try
            {
                using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
                {
                    var entity = new SDGAppDB.POCO.TagLabel();
                    entity.LabelName = model.LabelName;
                    entity.MaxRange = model.MaxRange;
                    entity.MinRange = model.MinRange;
                    entity.DefaultValue = model.DefaultValue;
                    entity.PrecisionDigit = model.PrecisionDigit;

                    db.TagLabel.Add(entity);
                    db.SaveChanges();

                    if (entity.ID > 0)
                    {
                        var lastid = entity.ID;
                        model.ID = lastid;
                    }


                    Result = true;
                }
            }
            catch (Exception Ex)
            {
                WriteLog("SDGApp.Models.DeviceServicesModel - AddNewLabel", Ex.Message);
            }


            return Result;
        }

        public bool DuplicateTagLabel(string labelname, int UserID, ref int TagLabelID)
        {
            bool Result = false;
            TagLabelViewModel model = new TagLabelViewModel();
            try
            {
                using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
                {
                    var entity = (from taglabel in db.TagLabel
                                  where taglabel.LabelName.ToLower().Equals(labelname.ToLower())
                                  && taglabel.UserID == UserID
                                  select taglabel).FirstOrDefault();

                    if (entity != null && entity.ID > 0)
                    {
                        Result = true;
                        TagLabelID = entity.ID;
                    }
                    //if (entityCount != 0 && entityCount > 0)
                    //{
                    //    Result = true;

                    //}

                }
            }
            catch (Exception Ex)
            {
                WriteLog("SDGApp.Models.DeviceServicesModel - DuplicateTagLabel", Ex.Message);
            }
            return Result;
        }



        #endregion


        #region [ThirdPartyAPIController method]

        public Boolean ActivateKey(String Email, String PublicKey, String ConfirmationCode)
        {
            Boolean result = false;

            try
            {
                using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
                {
                    var CompanyID = (from utpa in db.UserThirdPartyAPIKey
                                     where utpa.APIKeyValue == PublicKey
                                  && !utpa.IsActive
                                  && !utpa.IsDeleted
                                     select utpa.FKCompanyID).FirstOrDefault();

                    if (CompanyID > 0)
                    {
                        var existkey = (from utp in db.UserThirdPartyAPIKey
                                        where utp.APIKeyValue.ToLower().Trim() == PublicKey.ToLower().Trim()
                                        && utp.ConfimationCode.ToLower().Trim() == ConfirmationCode.ToLower().Trim()
                                        && utp.FKCompanyID == CompanyID
                                        && !utp.IsActive
                                        && !utp.IsDeleted
                                        select utp).FirstOrDefault();

                        if (existkey != null && existkey.UserThirdPartyAPIKeyID > 0)
                        {
                            existkey.IsActive = true;
                            db.Entry(existkey).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();
                            result = true;
                        }
                    }

                }

            }
            catch (Exception Ex)
            {
                WriteLog("SDGApp.Models.DeviceServicesModel - ActivateKey", Ex.Message);
            }

            return result;
        }

        public Boolean CheckAPIAccessKey(String PublicKey)
        {
            Boolean result = false;
            try
            {
                using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
                {
                    var existentity = (from pk in db.UserThirdPartyAPIKey
                                       where pk.APIKeyValue.ToLower().Trim() == PublicKey.Trim().ToLower()
                                       && !pk.IsDeleted && pk.IsActive
                                       select pk).FirstOrDefault();

                    if (existentity != null && existentity.UserThirdPartyAPIKeyID > 0)
                    {
                        result = true;
                    }
                }
            }
            catch (Exception Ex)
            {
                WriteLog("SDGApp.Models.DeviceServicesModel - CheckAPIAccessKey", Ex.Message);
            }

            return result;
        }

        #endregion


        #region[*****Divice Information******]

        public Boolean SaveDeviceInfo(DeviceInformationViewModel model, ref int retrunDeviceInfoID)
        {
            Boolean result = false;
            try
            {
                using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
                {

                    var entity = new SDGAppDB.POCO.DeviceInformation();
                    entity.UserID = model.UserID;
                    entity.DeviceName = model.DeviceName;
                    entity.DeviceAddress = model.DeviceAddress;
                    entity.BlockStatus = model.BlockStatus;
                    entity.CreatedDateTime = DateTime.Now;

                    db.DeviceInformation.Add(entity);
                    db.SaveChanges();

                    if (entity.ID > 0)
                    {
                        result = true;
                        retrunDeviceInfoID = entity.ID;
                    }
                }
            }
            catch (Exception Ex)
            {
                WriteLog("SDGApp.Models.DeviceServicesModel - SaveDeviceInfo", Ex.Message);
            }
            return result;
        }

        public List<DeviceInformationViewModel> GetDeviceList(int UserID, int PageNumber, int PageSize, int[] IDs = null)
        {
            List<DeviceInformationViewModel> lstchunk = new List<DeviceInformationViewModel>();
            List<DeviceInformationViewModel> lst = new List<DeviceInformationViewModel>();
            try
            {
                using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
                {

                    lst = (from dinfo in db.DeviceInformation
                           where dinfo.UserID == UserID
                           select new DeviceInformationViewModel
                           {
                               ID = dinfo.ID,
                               UserID = dinfo.UserID,
                               DeviceName = dinfo.DeviceName,
                               DeviceAddress = dinfo.DeviceAddress,
                               BlockStatus = dinfo.BlockStatus,
                               CreatedDateTime = dinfo.CreatedDateTime
                           }).ToList();


                    if (lst.Count > 0)
                    {
                        if (IDs != null && IDs.Length > 0)
                        {
                            lst.RemoveAll(x => IDs.Contains(x.ID));

                            //foreach (var item in IDs)
                            //{
                            //    lst.Remove(lst.Single(s => s.ID == item)); // Remove IDs from List 
                            //}

                        }

                        lstchunk = lst.OrderByDescending(q => q.ID).Skip(SkipRecords(PageSize, PageNumber)).Take(PageSize).ToList();

                        lstchunk.ForEach(l => l.CreateDateTimeStamp = l.CreatedDateTime.ToString("yyyy-MM-dd HH:mm:ss"));
                    }

                }

            }
            catch (Exception Ex)
            {
                WriteLog("SDGApp.Models.TagHistoryModel - GetSleepActivityList", Ex.Message);
            }
            return lstchunk;
        }

        #endregion

    }
}