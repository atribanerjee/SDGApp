using SDGApp.ViewModel;
using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using SDGAppDB;
using SDGAppDB.POCO;
using System.Web;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using System.Data.SqlClient;
using System.Globalization;
using SDGApp.Helpers;

namespace SDGApp.Models
{
    public class UserModel : BaseModel
    {

        public bool CheckLoginInfo(UserViewModel UVM)
        {
            Boolean Result = false;

            try
            {
                using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))

                {
                    string EncryptedPwd = Encrypt(UVM.Password);
                    var model = (from u in db.User
                                 join ut in db.UserType on u.fkUserType equals ut.UserTypeID
                                 join cur in db.CompanyUserRole on u.UserID equals cur.FKUserID
                                 join r in db.Role on cur.FKRoleID equals r.RoleID
                                 where u.UserName == UVM.UserName && u.Password == EncryptedPwd
                                 && u.IsActive && !u.IsDeleted
                                 select new UserViewModel
                                 {
                                     UserID = u.UserID,
                                     FirstName = u.FirstName,
                                     LastName = u.LastName,
                                     Email = u.Email,
                                     RoleisActive = u.IsActive,
                                     UserRoleID = cur.FKRoleID,
                                     UserRole = r.RoleName,
                                     UserTypeID = u.fkUserType,
                                     UserType = ut.UserTypeName,
                                     CompanyID = cur.FKCompanyID,
                                     Picture = u.Picture
                                 }).FirstOrDefault();

                    if (model != null)
                    {
                        SetLoggedInUserInfo(model);
                        SaveUserCode(model.CompanyID, model.UserID);
                        Result = true;
                    }
                }
            }


            catch (Exception Ex)
            {
                WriteLog("SDGApp.Models.UserModel - CheckLoginInfo", Ex.Message);

            }
            return Result;
        }

        public Boolean SaveUserCode(int CompanyID, int UserID)
        {
            Boolean result = false;

            try
            {
                using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
                {
                    if (CompanyID > 0 && UserID > 0)
                    {
                        var existUsercode = (from comuserrole in db.CompanyUserRole
                                             where comuserrole.FKCompanyID == CompanyID
                                             && comuserrole.FKUserID == UserID
                                             && String.IsNullOrEmpty(comuserrole.UserCode)
                                             select comuserrole).FirstOrDefault();

                        if (existUsercode != null && existUsercode.CompanyUserRoleID > 0)
                        {
                            String rndStringcode = String.Empty;

                            for (int i = 0; i < 1; i++)
                            {
                                rndStringcode = RandomString(9);

                                var existUsercodethisOrg = (from comUsrRole in db.CompanyUserRole
                                                            where comUsrRole.UserCode.ToLower() == rndStringcode.ToLower()
                                                            && comUsrRole.FKCompanyID == CompanyID
                                                            select comUsrRole).FirstOrDefault();

                                if (existUsercodethisOrg == null)
                                {

                                    existUsercode.UserCode = rndStringcode;
                                    existUsercode.JoiningDate = DateTime.Now;

                                    db.Entry(existUsercode).State = System.Data.Entity.EntityState.Modified;
                                    db.SaveChanges();
                                    result = true;
                                }
                                else
                                {
                                    i--;
                                }
                            }
                        }


                    }
                }
            }
            catch (Exception Ex)
            {

                WriteLog("SDGApp.Models.UserModel - SaveUserCode", Ex.Message);
            }

            return result;
        }

        public UserViewModel SaveGuID(UserViewModel UVM)
        {
            //bool user = false;
            UserViewModel model = new UserViewModel();
            try
            {

                using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
                {
                    var mail = db.User.Where(x => x.Email == UVM.Email).FirstOrDefault();
                    if (mail != null)
                    {
                        model.GuID = UVM.GuID;
                        model.GuIDIsActive = true;
                        model.Email = UVM.Email;
                        db.Entry(mail).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                        // result = true;
                    }

                }

            }
            catch (Exception Ex)
            {
                WriteLog("SDGApp.Models.UserModel - SaveGuId", Ex.Message);
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
                                 Email = u.Email,
                                 Password = u.Password,
                                 IsActive = u.IsActive
                             }).FirstOrDefault();

                }


                //IF NEEDED FILL MORE

            }
            catch (Exception Ex)
            {
                WriteLog("SDGApp.Models.UserModel - GetUserDetailByGUID", Ex.Message);
            }
            return model;
        }

        public String RegisterNewUser(string firstName, string lastName, int countryID, int stateID, int cityID, string zipCode, string address, string email, string password, string gender, decimal height, decimal weight, string race, bool isAdmin, string socialSecurityNo, string phoneNumber, string mobileNumber, string companyName, string facebookUrl, string twitterUrl, string googlePlusUrl, string flickrUrl, string youtubeUrl, string skypeID)
        {
            String retresult = "";
            try
            {
                int RoleID = GetIntegerValue(SDGApp.Helpers.SDGUtilities.UserRoleType.User);

                if (isAdmin)
                {
                    RoleID = GetIntegerValue(SDGApp.Helpers.SDGUtilities.UserRoleType.Administrator);
                }

                if (SqlHelper.ExecuteNonQuery(
                    GlobalConstants.DBConn(),
                    "USP_INSERT_USER",
                    firstName,
                    lastName,
                    stateID,
                    cityID,
                    zipCode,
                    address,
                    email,
                    socialSecurityNo,
                    1,
                    firstName,
                    password,
                    "",
                    0,
                    mobileNumber,
                    countryID,
                    companyName,
                    facebookUrl,
                    twitterUrl,
                    googlePlusUrl,
                    flickrUrl,
                    youtubeUrl,
                    phoneNumber,
                    skypeID,
                    gender,
                    height,
                    weight,
                    race,
                    RoleID
                    ) > 0)
                {
                    retresult = "Success";
                }
            }
            catch (Exception Ex)
            {
                WriteLog("SDGApp.Models.UserModel - RegisterNewUser", Ex.Message);
                retresult = "Failed ( Exception - " + Ex.Message + " )";
            }
            return retresult;
        }



        public Boolean UpdatepasswordforUser(int userid, string password)
        {
            Boolean retresult = false;
            try
            {
                using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
                {
                    var Entity = db.User.Find(userid);
                    if (Entity != null)
                    {
                        //Entity.Password = password;
                        Entity.Password = Encrypt(password);
                        Entity.GuID = null;
                        Entity.GuIDIsActive = false;
                        db.Entry(Entity).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                        retresult = true;
                    }
                }
            }
            catch (Exception Ex)
            {
                WriteLog("SDGApp.Models.UserModel - UpdatepasswordforUser", Ex.Message);
            }
            return retresult;
        }

        public UserViewModel GetUserDetailByUserID(int UserID)
        {

            UserViewModel model = new UserViewModel();
            try
            {

                using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
                {
                    model = (from u in db.User
                             join cur in db.CompanyUserRole on u.UserID equals cur.FKUserID
                             where u.UserID == UserID 
                             && u.IsActive == true && u.IsDeleted == false 
                             select new UserViewModel
                             {
                                 UserID = u.UserID,
                                 FirstName = u.FirstName,
                                 LastName = u.LastName,
                                 UserName = u.UserName,
                                 Email = u.Email,
                                 IsActive = u.IsActive,
                                 Gender = u.Gender,
                                 Height = (u.Height).ToString(),
                                 Weight = (u.Weight).ToString(),
                                 Race = u.Race,
                                 UserRoleID = cur.FKRoleID,
                                 UserTypeID = u.fkUserType,
                                 DateOfBirth = (u.DOB).ToString(),
                                 GeoLocation = u.Geolocation,
                                 ResearchName = u.ResearchersName,
                                 Picture = u.Picture,
                                 Institution = u.ResearchersInstitution,
                                 NotesHere = u.ResearchersNotes,
                                 SkinType = u.SkinType,
                                 GuID = u.GuID,
                                 GuIDIsActive = true,
                                 CompanyID = cur.FKCompanyID,
                                 UnitNameID = u.fkUnit ?? 1

                             }).FirstOrDefault();

                    if (model != null && model.UserID > 0 && !String.IsNullOrEmpty(model.DateOfBirth))
                    {
                        model.DateOfBirth = Convert.ToDateTime(model.DateOfBirth).ToShortDateString();
                    }

                    var entityAttribute = (from a in db.Attribute
                                           where a.FKAttributeTypeID == 6 && a.FKDataID == UserID
                                           select new { a }).ToList();
                    if (entityAttribute != null && entityAttribute.Count > 0)
                    {
                        String[] lbl = new String[entityAttribute.Count];
                        String[] val = new String[entityAttribute.Count];

                        // AttribteRule Label ID

                        string[] attvalues = new string[entityAttribute.Count];
                        int[] attlabelid = new int[entityAttribute.Count];
                        int[] atttypeid = new int[entityAttribute.Count];

                        for (int i = 0; i < entityAttribute.Count; i++)
                        {
                            lbl[i] = entityAttribute[i].a.AttributeKey;
                            val[i] = entityAttribute[i].a.AttributeValue;
                            int attributerulalabelid = entityAttribute[i].a.AttributeRuleLabelID;
                            attlabelid[i] = attributerulalabelid;
                            var entityAttributeRule = (from ar in db.AttributeRule
                                                       where ar.FKRuleLabelID == attributerulalabelid
                                                       select new { ar }).FirstOrDefault();
                            if (entityAttributeRule == null)
                            {
                                atttypeid[i] = 0;
                                attvalues[i] = null;
                            }
                            else
                            {
                                atttypeid[i] = entityAttributeRule.ar.FKRuleTypeID;
                                attvalues[i] = entityAttributeRule.ar.AttributeRuleValue;
                            }
                        }
                        if (model != null && model.UserID > 0)
                        {
                            model.txtAttrLabel = lbl;
                            model.txtAttrValue = val;
                            model.AttributeRuleLabelID = attlabelid;
                            model.AttributeRuleTypeID = atttypeid;
                            model.AttributeRuleValue = attvalues;
                        }
                    }

                }
            }

            catch (Exception Ex)
            {
                WriteLog("SDGApp.Models.UserModel - GetUserDetailByUserId", Ex.Message);
            }
            return model;
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
                        model.UserID = UVM.UserID;
                        model.UserName = UVM.UserName;
                        model.FirstName = UVM.FirstName;
                        db.Entry(UVM).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                        // result = true;
                    }
                }

            }
            catch (Exception Ex)
            {
                WriteLog("SDGApp.Models.UserModel - GetUserDetailsByEmailAndSetNewGuID", Ex.Message);
            }
            return model;
        }


        public bool ProfileUpdateDtls(UserViewModel model, string skincolor, HttpPostedFileBase file)
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
                        UVM.Company = model.Company;
                        UVM.Gender = model.Gender;
                        UVM.Height = GetDecimalPlaceValue(model.Height);
                        UVM.Weight = GetDecimalPlaceValue(model.Weight);
                        UVM.fkUnit = model.UnitNameID;

                        if (!String.IsNullOrEmpty(skincolor))
                        {
                            UVM.SkinType = skincolor;
                        }

                        if (file != null && file.ContentLength != 0)
                        {
                            string ImageName = System.IO.Path.GetFileName(file.FileName);
                            //UVM.Picture = ImageName;
                            string filephysicalPath = HttpContext.Current.Server.MapPath("~/Content/images/" + ImageName);
                            //file.SaveAs(filephysicalPath);
                        }


                        UVM.Picture = model.Picture;  // CROPPED PROFILE IMAGE SAVED IN DB.


                        db.Entry(UVM).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();

                        //IF User update profile then change session value if changed
                        if (model.UserID == GetLoggedInUserInfo().UserID)
                        {

                            var userentity = (from u in db.User
                                              join ur in db.UserRole on u.UserID equals ur.FKUserID
                                              join r in db.Role on ur.FKRoleID equals r.RoleID
                                              join ut in db.UserType on u.fkUserType equals ut.UserTypeID
                                              join cur in db.CompanyUserRole on u.UserID equals cur.FKUserID
                                              where u.UserID == UVM.UserID
                                              && u.IsActive == true
                                              select new UserViewModel
                                              {
                                                  UserID = u.UserID,
                                                  FirstName = u.FirstName,
                                                  LastName = u.LastName,
                                                  Email = u.Email,
                                                  RoleisActive = u.IsActive,
                                                  UserRoleID = ur.FKRoleID,
                                                  UserRole = r.RoleName,
                                                  UserTypeID = u.fkUserType,
                                                  UserType = ut.UserTypeName,
                                                  CompanyID = cur.FKCompanyID,
                                                  Picture = u.Picture
                                              }).FirstOrDefault();

                            if (userentity != null)
                            {
                                SetLoggedInUserInfo(userentity);

                            }
                        }

                        result = true;



                    }
                }
            }




            catch (Exception Ex)
            {
                WriteLog("SDGApp.Models.UserModel - UserProfileUpdate", Ex.Message);
            }

            return result;
        }


        public bool UserProfileUpdate(UserViewModel model, string skincolor, HttpPostedFileBase file, string[] txtAttrLabel, string[] txtAttrValue)
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
                        UVM.UserName = model.UserName;
                        UVM.Email = model.Email;
                        UVM.Gender = model.Gender;
                        UVM.Height = GetDecimalPlaceValue(model.Height);
                        UVM.Weight = GetDecimalPlaceValue(model.Weight);
                        UVM.Race = model.Race;
                        UVM.fkUserType = model.UserTypeID;

                        UVM.Geolocation = model.GeoLocation;
                        UVM.ResearchersName = model.ResearchName;
                        UVM.ResearchersInstitution = model.Institution;
                        UVM.ResearchersNotes = model.ResearchName;
                        UVM.fkUnit = model.UnitNameID;
                        if (model.DateOfBirth != null)
                        {
                            UVM.DOB = GetDateTimeValue(model.DateOfBirth);
                        }

                        if (!String.IsNullOrEmpty(skincolor))
                        {
                            UVM.SkinType = skincolor;
                        }

                        if (file != null && file.ContentLength != 0)
                        {
                            string ImageName = System.IO.Path.GetFileName(file.FileName);
                            UVM.Picture = ImageName;
                            string filephysicalPath = HttpContext.Current.Server.MapPath("~/Content/images/" + ImageName);
                            file.SaveAs(filephysicalPath);
                        }


                        db.Entry(UVM).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();



                        // Select all the records to be deleted
                        var entityCompanyUserRoles = db.CompanyUserRole.Where(x => x.FKUserID == model.UserID).ToList();

                        // Use Remove Range function to delete all records at once
                        db.CompanyUserRole.RemoveRange(entityCompanyUserRoles);

                        // Save changes
                        db.SaveChanges();


                        //Add Company user role

                        var CompanyUserRoleEntity = new CompanyUserRole();
                        CompanyUserRoleEntity.FKCompanyID = model.CompanyID;
                        CompanyUserRoleEntity.FKRoleID = model.UserRoleID;
                        CompanyUserRoleEntity.FKUserID = model.UserID;
                        db.CompanyUserRole.Add(CompanyUserRoleEntity);
                        db.SaveChanges();

                        //End

                        // Delete existing data and add new data

                        var _list = (from a in db.Attribute
                                     where a.FKAttributeTypeID == 6 && a.FKDataID == model.UserID
                                     select new { a }).ToList();

                        if (_list != null && _list.Count > 0)
                        {
                            foreach (var item in _list)
                            {
                                var entityAttr = db.Attribute.Find(item.a.AttributeID);

                                if (entityAttr != null && entityAttr.AttributeID > 0)
                                {
                                    db.Entry(entityAttr).State = System.Data.Entity.EntityState.Deleted;
                                    db.SaveChanges();
                                }
                            }
                        }
                        if (txtAttrLabel != null && txtAttrValue != null)
                        {
                            for (int i = 0; i < txtAttrLabel.Length; i++)
                            {
                                if (!String.IsNullOrEmpty(txtAttrLabel[i]) && !String.IsNullOrEmpty(txtAttrValue[i]))
                                {
                                    var entityAttribute = new SDGAppDB.POCO.Attribute();
                                    var entityAttributeRuleLabel = db.AttributeRuleLabel.Find(GetIntegerValue(txtAttrLabel[i]));
                                    entityAttribute.AttributeKey = entityAttributeRuleLabel.AttributeRuleLabelText;

                                    entityAttribute.AttributeValue = txtAttrValue[i];
                                    entityAttribute.FKAttributeTypeID = 6;
                                    entityAttribute.FKDataID = model.UserID;
                                    entityAttribute.AttributeRuleLabelID = GetIntegerValue(txtAttrLabel[i]);

                                    db.Attribute.Add(entityAttribute);
                                    db.SaveChanges();
                                }
                            }
                        }


                        //IF User update profile then change session value if changed
                        if (model.UserID == GetLoggedInUserInfo().UserID)
                        {
                            var userentity = (from u in db.User
                                              join ut in db.UserType on u.fkUserType equals ut.UserTypeID
                                              join cur in db.CompanyUserRole on u.UserID equals cur.FKUserID
                                              join r in db.Role on cur.FKRoleID equals r.RoleID
                                              where u.UserID == UVM.UserID
                                              && u.IsActive == true
                                              select new UserViewModel
                                              {
                                                  UserID = u.UserID,
                                                  FirstName = u.FirstName,
                                                  LastName = u.LastName,
                                                  Email = u.Email,
                                                  RoleisActive = u.IsActive,
                                                  UserRoleID = cur.FKRoleID,
                                                  UserRole = r.RoleName,
                                                  UserTypeID = u.fkUserType,
                                                  UserType = ut.UserTypeName,
                                                  CompanyID = cur.FKCompanyID,
                                                  Picture = u.Picture
                                              }).FirstOrDefault();

                            if (userentity != null)
                            {
                                SetLoggedInUserInfo(userentity);

                            }

                        }


                        result = true;



                    }
                }
            }




            catch (Exception Ex)
            {
                WriteLog("SDGApp.Models.UserModel - UserProfileUpdate", Ex.Message);
            }

            return result;
        }


        public bool ChangeExistingPassword(int userid, string oldpassword, string newpassword)
        {
            bool result = false;
            String encryptOldPassword = Encrypt(oldpassword);
            try
            {

                using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
                {
                    var UserEntity = db.User.Where(x => x.UserID == userid && x.Password == encryptOldPassword).FirstOrDefault();

                    if (UserEntity != null)
                    {
                        UserEntity.Password = Encrypt(newpassword);

                        db.Entry(UserEntity).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();

                        result = true;
                    }
                }

            }
            catch (Exception Ex)
            {
                WriteLog("SDGApp.Models.UserModel - ChangExistingPassword", Ex.Message);
            }
            return result;
        }


        public List<SelectListItem> GetCountryList()
        {
            List<SelectListItem> _list = new List<SelectListItem>();

            try
            {
                using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
                {
                    _list = (from c in db.Country
                             select new SelectListItem { Text = c.CountryName, Value = c.CountryID.ToString() }
                             ).ToList();
                }
            }
            catch (Exception Ex)
            {
                WriteLog("SDGApp.Models.GetCountryList - GetCompanyDDL", Ex.Message);
            }

            return _list;
        }

        


        public List<SelectListItem> GetDDLSateList(Int32 CountryID)
        {

            List<SelectListItem> _list = new List<SelectListItem>();
            try
            {
                using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
                {
                    _list = (from sd in db.StateDetail
                             where sd.FKCountryID == CountryID
                             select new SelectListItem { Text = sd.StateName, Value = sd.StateID.ToString() }
                             ).ToList();
                }
            }
            catch (Exception Ex)
            {
                WriteLog("SDGApp.Models.UserModel - GetDDLSateList", Ex.Message);
            }
            return _list;
        }


        public List<SelectListItem> GetDDLCityList(int StateID)
        {

            List<SelectListItem> _list = new List<SelectListItem>();
            try
            {
                using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
                {
                    _list = (from c in db.CityDetail
                             where c.FKStateID == StateID
                             select new SelectListItem { Text = c.CityName, Value = c.CityID.ToString() }
                             ).ToList();
                }
            }
            catch (Exception Ex)
            {
                WriteLog("SDGApp.Models.UserModel - GetDDLCityList", Ex.Message);
            }
            return _list;
        }

        public List<UserViewModel> GetAllUsersList(UserViewModel UVM)
        {
            List<UserViewModel> _List = new List<UserViewModel>();
            DataSet DS = null;
            try
            {
                if (string.IsNullOrEmpty(UVM.SearchValue))
                {
                    using (DS = new DataSet())
                    {
                        DS = SqlHelper.ExecuteDataset(GlobalConstants.DBConn(), "USP_GetAllUsersList", UVM.PageNumber, UVM.PageSize);
                        if (DS != null && DS.Tables[0].Rows.Count > 0)
                        {
                            foreach (DataRow DR in DS.Tables[0].Rows)
                            {
                                UserViewModel model = new UserViewModel();

                                model.UserID = GetIntegerValue(DR["UserID"]);
                                model.FirstName = GetStringValue(DR["FirstName"]);
                                model.LastName = GetStringValue(DR["LastName"]);
                                model.Email = GetStringValue(DR["Email"]);
                                model.Company = GetStringValue(DR["Company"]);
                                model.TotalRecords = GetIntegerValue(DS.Tables[1].Rows[0]["TotalCount"]);
                                model.PageSize = UVM.PageSize;
                                model.PageNumber = UVM.PageNumber;
                                model.Gender = GetStringValue(DR["Gender"]);
                                model.Height = GetStringValue(DR["Height"]);
                                model.Weight = GetStringValue(DR["Weight"]);
                                model.Race = GetStringValue(DR["Race"]);

                                _List.Add(model);
                            }
                        }
                    }

                }
                else if (!string.IsNullOrEmpty(UVM.SearchValue) && UVM.SearchValue.Length > 0)
                {
                    var SearchKey = UVM.SearchValue;
                    var pageNumber = UVM.PageNumber;
                    var PageSize = UVM.PageSize;

                    List<UserViewModel> lstchunk = new List<UserViewModel>();
                    List<UserViewModel> lst = new List<UserViewModel>();

                    using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
                    {
                        if (SearchKey.Length >= 0)
                        {
                            var lstResult = (from u in db.User
                                             where (!u.IsDeleted && u.IsActive)
                                             &&
                                             (u.FirstName.Trim().ToLower().Contains(SearchKey.Trim().ToLower())
                                              || u.LastName.Trim().ToLower().Contains(SearchKey.Trim().ToLower())
                                              || u.Email.Trim().ToLower().Contains(SearchKey.Trim().ToLower())
                                              || u.Mobile.Trim().ToLower().Contains(SearchKey.Trim().ToLower()))
                                             select new UserViewModel
                                             {
                                                 UserID = u.UserID,
                                                 FirstName = u.FirstName,
                                                 LastName = u.LastName,
                                                 Email = u.Email,
                                                 MobileNo = u.Mobile,
                                                 Company = u.Company,
                                                 Gender = u.Gender,
                                                 //Height = u.Height,
                                                 //Weight = u.Weight,
                                                 Race = u.Race,
                                                 PageSize = PageSize,
                                                 PageNumber = pageNumber
                                             }).Distinct().ToList();

                            lstchunk.AddRange(lstResult);
                            lst.AddRange(lstchunk);
                        }
                        lstchunk = lst.OrderByDescending(q => q.UserID).Skip(SkipRecords(PageSize, pageNumber)).Take(PageSize).ToList();
                        lstchunk.ForEach(l => l.TotalRecords = lst.Count());
                    }

                    _List = lstchunk;
                }
            }
            catch (Exception Ex)
            {
                WriteLog("SDGApp.Models.UserModel - GetAllUsersList", Ex.Message);
            }
            return _List;
        }


        public Boolean CheckDuplicateEmail(String Email, Int32 UserID)
        {
            Boolean Result = false;
            DataSet DS = null;
            try
            {
                using (DS = new DataSet())
                {
                    DS = SqlHelper.ExecuteDataset(GlobalConstants.DBConn(), "USP_CHECK_DUPLICATE_EMAIL", Email, UserID);

                    if (DS != null && DS.Tables[0] != null && DS.Tables[0].Rows.Count > 0)
                    {
                        Result = true;
                    }
                }
            }
            catch (Exception Ex)
            {
                WriteLog("SDGApp.Models.UserModel - CheckDuplicateEmail", Ex.Message);
            }
            return Result;
        }

        public Boolean CheckDuplicateUserName(String UserName, Int32 UserID)
        {
            Boolean Result = false;
            DataSet DS = null;
            try
            {
                using (DS = new DataSet())
                {
                    DS = SqlHelper.ExecuteDataset(GlobalConstants.DBConn(), "USP_CHECK_DUPLICATE_USERNAME", UserName, UserID);

                    if (DS != null && DS.Tables[0] != null && DS.Tables[0].Rows.Count > 0)
                    {
                        Result = true;
                    }
                }
            }
            catch (Exception Ex)
            {
                WriteLog("SDGApp.Models.UserModel - CheckDuplicateUserName", Ex.Message);
            }
            return Result;
        }

        public List<State> GetSateList(Int32 CountryID)
        {
            DataSet dsState = null;
            List<State> _list = new List<State>();
            try
            {
                using (dsState = new DataSet())
                {
                    dsState = SqlHelper.ExecuteDataset(GlobalConstants.DBConn(), "USP_GetAllStateByCountry", CountryID);
                    _list = dsState.Tables[0].AsEnumerable().Select(dataRow => new State { StateName = dataRow.Field<string>("StateName"), ID = dataRow.Field<Int32>("StateID") }).ToList();
                }
            }
            catch (Exception Ex)
            {
                WriteLog("SDGApp.Models.UserModel - GetSateList", Ex.Message);
            }
            return _list;
        }

        public List<City> GetCityList(int StateID)
        {
            DataSet dsCity = null;
            List<City> _list = new List<City>();
            try
            {
                using (dsCity = new DataSet())
                {
                    dsCity = SqlHelper.ExecuteDataset(GlobalConstants.DBConn(), "USP_GetAllCityByState", StateID);
                    _list = dsCity.Tables[0].AsEnumerable().Select(dataRow => new City { CityName = dataRow.Field<string>("CityName"), ID = dataRow.Field<Int32>("CityID") }).ToList();
                }
            }
            catch (Exception Ex)
            {
                WriteLog("SDGApp.Models.UserModel - GetCityList", Ex.Message);
            }
            return _list;
        }

        public List<SelectListItem> GetRoleList()
        {
            DataSet dsRole = null;
            List<SelectListItem> _list = new List<SelectListItem>();
            try
            {
                using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
                {
                    _list = (from r in db.Role
                             select new SelectListItem { Text = r.RoleName, Value = r.RoleID.ToString() }
                             ).ToList();
                }
            }
            catch (Exception Ex)
            {
                WriteLog("SDGApp.Models.UserModel - GetRoleList", Ex.Message);
            }
            return _list;
        }


        public List<SelectListItem> GetUserTypeList()
        {

            List<SelectListItem> _list = new List<SelectListItem>();
            try
            {
                using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
                {
                    _list = (from ut in db.UserType

                             select new SelectListItem { Text = ut.UserTypeName, Value = ut.UserTypeID.ToString() }
                             ).ToList();
                }
            }
            catch (Exception Ex)
            {
                WriteLog("SDGApp.Models.UserModel - GetRoleList", Ex.Message);
            }
            return _list;
        }

        public Boolean SetLoggedInUserInfo(UserViewModel model)
        {
            Boolean Result = false;
            try
            {
                SetSessionValue("LoggedInUserID", model.UserID);
                SetSessionValue("LoggedInUserFirstName", model.FirstName);
                SetSessionValue("LoggedInUserLastName", model.LastName);
                SetSessionValue("LoggedInUserFullName", model.FirstName + " " + model.LastName);
                SetSessionValue("LoggedInUserEmail", model.Email);
                SetSessionValue("LoggedInUserRoleIsAdmin", model.RoleisActive);
                SetSessionValue("LoggedInUserRole", model.UserRole);
                SetSessionValue("LoggedInUserRoleID", model.UserRoleID);
                SetSessionValue("LoggedInUserType", model.UserType);
                SetSessionValue("LoggedInUserTypeID", model.UserTypeID);
                SetSessionValue("LoggedInCompanyID", model.CompanyID);
                SetSessionValue("LoggedInUserPicture", model.Picture);
                Result = true;
            }
            catch (Exception Ex)
            {
                WriteLog("SDGApp.Models.UserModel - SetLoggedInUserInfo", Ex.Message);
            }
            return Result;
        }

        public UserViewModel GetLoggedInUserInfo()
        {
            UserViewModel model = new UserViewModel();
            try
            {
                model.UserID = GetIntegerValue(GetSessionValue("LoggedInUserID"));
                model.FirstName = GetStringValue(GetSessionValue("LoggedInUserFirstName"));
                model.LastName = GetStringValue(GetSessionValue("LoggedInUserLastName"));
                model.Email = GetStringValue(GetSessionValue("LoggedInUserEmail"));
                model.UserName = GetStringValue(GetSessionValue("LoggedInUserFirstName")) + " " + GetStringValue(GetSessionValue("LoggedInUserLastName"));
                model.UserRole = GetStringValue(GetSessionValue("LoggedInUserRole"));
                model.UserRoleID = GetIntegerValue(GetSessionValue("LoggedInUserRoleID"));
                model.UserType = GetStringValue(GetSessionValue("LoggedInUserType"));
                model.UserTypeID = GetIntegerValue(GetSessionValue("LoggedInUserTypeID"));
                model.CompanyID = GetIntegerValue(GetSessionValue("LoggedInCompanyID"));
                model.Picture = GetStringValue(GetSessionValue("LoggedInUserPicture"));
            }
            catch (Exception Ex)
            {
                WriteLog("SDGApp.Models.UserModel - GetLoggedInUserInfo", Ex.Message);
            }
            return model;
        }

        public List<SelectListItem> GetUserDDL()
        {
            List<SelectListItem> _list = new List<SelectListItem>();
            try
            {
                using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
                {
                    _list = (from u in db.User
                             where u.IsActive && !u.IsDeleted
                             orderby u.UserID descending
                             select new SelectListItem { Text = u.FirstName + " " + u.LastName + "/" + u.UserName, Value = u.UserID.ToString() }
                             ).ToList();
                }
            }
            catch (Exception Ex)
            {
                WriteLog("SDGApp.Models.UserModel - GetUserDDL", Ex.Message);
            }
            return _list;
        }

        public bool DeleteUserByUserID(int UserID)
        {
            bool Result = false;
            try
            {
                int Val = SqlHelper.ExecuteNonQuery(GlobalConstants.DBConn(), "USP_DeleteUserByUserID", UserID);
                if (Val > 0)
                {
                    Result = true;
                }
            }
            catch (Exception Ex)
            {
                WriteLog("SDGApp.Models.UserModel - DeleteUserByUserID", Ex.Message);
            }
            return Result;
        }

        public List<SelectListItem> GetGenderList()
        {


            List<SelectListItem> _list = new List<SelectListItem>();
            try
            {
                _list.Add(new SelectListItem()
                {
                    Value = "M",
                    Text = "Male"
                });
                _list.Add(new SelectListItem()
                {
                    Value = "F",
                    Text = "Female"
                });

                _list.Add(new SelectListItem()
                {
                    Value = "O",
                    Text = "Others"
                });

            }
            catch (Exception Ex)
            {
                WriteLog("SDGApp.Models.UserModel - GetGenderList", Ex.Message);
            }
            return _list;
        }

        public List<SelectListItem> GetUnitNameList()
        {
            List<SelectListItem> _list = new List<SelectListItem>();
            try
            {
                using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
                {
                    _list = (from u in db.Unit

                             select new SelectListItem { Text = u.UnitName, Value = u.Id.ToString() }
                             ).ToList();
                }

            }
            catch (Exception)
            {

                throw;
            }
            return _list;
        }

        // Skin Type test
        public List<SelectListItem> GetUserSkinTypeList()
        {


            List<SelectListItem> _list = new List<SelectListItem>();
            try
            {
                _list.Add(new SelectListItem()
                {
                    Value = "#8d5524",
                    Text = "#8d5524"
                });
                _list.Add(new SelectListItem()
                {
                    Value = "#c68642",
                    Text = "#c68642"
                });
                _list.Add(new SelectListItem()
                {
                    Value = "#e0ac69",
                    Text = "#e0ac69"
                });
                _list.Add(new SelectListItem()
                {
                    Value = "#f1c27d",
                    Text = "#f1c27d"
                });
                _list.Add(new SelectListItem()
                {
                    Value = "#ffdbac",
                    Text = "#ffdbac"

                });



            }
            catch (Exception Ex)
            {
                WriteLog("SDGApp.Models.UserModel - GetGenderList", Ex.Message);
            }

            return _list;
        }

        public bool AddNewUser(UserViewModel model, string skincolor, HttpPostedFileBase file, String[] txtAttrLabel, String[] txtAttrValue)
        {
            bool Result = false;

            try
            {
                using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
                {
                    var entity = new SDGAppDB.POCO.User();
                    entity.FirstName = model.FirstName;
                    entity.LastName = model.LastName;
                    entity.Password = Encrypt(model.ConfirmPassword);
                    entity.Email = model.Email;
                    entity.UserName = model.UserName;
                    entity.FKCountryID = model.CountryID;
                    entity.FKStateID = model.State;
                    entity.Company = model.Company;
                    entity.Gender = model.Gender;
                    entity.Height = GetDecimelValue(model.Height);
                    entity.Weight = GetDecimelValue(model.Weight);
                    entity.Race = model.Race;
                    entity.fkUserType = model.UserTypeID;
                    entity.fkUnit = model.UnitNameID;
                    entity.IsActive = true;
                    entity.IsDeleted = false;
                    entity.CreateDateTime = DateTime.Now;

                    if (!string.IsNullOrEmpty(model.DateOfBirth))
                    {
                        entity.DOB = GetDateTimeValue(model.DateOfBirth);
                    }
                    else
                    {
                        entity.DOB = null;
                    }

                    entity.Geolocation = model.GeoLocation;
                    entity.ResearchersName = model.ResearchName;
                    entity.ResearchersInstitution = model.Institution;
                    entity.ResearchersNotes = model.NotesHere;
                    if (model.UserTypeID == 3)
                    {
                        entity.SkinType = skincolor;
                    }
                    else
                    {
                        entity.SkinType = null;
                    }

                    if (file != null && file.ContentLength != 0)
                    {
                        string ImageName = System.IO.Path.GetFileName(file.FileName);
                        entity.Picture = ImageName;
                        string filephysicalPath = HttpContext.Current.Server.MapPath("~/Content/images/" + ImageName);
                        file.SaveAs(filephysicalPath);
                    }

                    db.User.Add(entity);
                    db.SaveChanges();

                    // Add at Company user role

                    var CompanyUserRoleEntity = new CompanyUserRole();
                    CompanyUserRoleEntity.FKCompanyID = model.CompanyID;
                    CompanyUserRoleEntity.FKRoleID = model.UserRoleID;
                    CompanyUserRoleEntity.FKUserID = entity.UserID;

                    db.CompanyUserRole.Add(CompanyUserRoleEntity);
                    db.SaveChanges();

                    //User Code Entry
                    SaveUserCode(model.CompanyID, entity.UserID);


                    // Save Attribute Rule

                    if (txtAttrLabel != null && txtAttrValue != null)
                    {
                        for (int i = 0; i < txtAttrLabel.Length; i++)
                        {
                            if (!String.IsNullOrEmpty(txtAttrLabel[i]) && !String.IsNullOrEmpty(txtAttrValue[i]))
                            {
                                var entityattribute = new SDGAppDB.POCO.Attribute();
                                var Entityattributerulelabel = db.AttributeRuleLabel.Find(GetIntegerValue(txtAttrLabel[i]));
                                entityattribute.AttributeKey = Entityattributerulelabel.AttributeRuleLabelText;
                                entityattribute.AttributeValue = txtAttrValue[i];
                                entityattribute.FKAttributeTypeID = 6;
                                entityattribute.FKDataID = entity.UserID;
                                entityattribute.AttributeRuleLabelID = GetIntegerValue(txtAttrLabel[i]);
                                db.Attribute.Add(entityattribute);
                                db.SaveChanges();

                            }
                        }
                    }
                    Result = true;
                }
            }
            catch (Exception Ex)
            {
                WriteLog("SDGApp.Models.UserModel - UserProfileUpdate", Ex.Message);
            }


            return Result;
        }

        public List<SelectListItem> GetCompanyList()
        {

            List<SelectListItem> _list = new List<SelectListItem>();
            try
            {
                using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
                {
                    _list = (from c in db.Company
                             where c.IsDeleted == false && c.CompanyName.ToLower().Trim() != "no company"
                             select new SelectListItem { Text = c.CompanyName, Value = c.CompanyID.ToString() }
                             ).ToList();
                }
            }
            catch (Exception Ex)
            {
                WriteLog("SDGApp.Models.UserModel - GetCompanyList", Ex.Message);
            }
            return _list;
        }

        // Get bllod presure report from  Service

        public void GetBloodResult()
        {
            //   UserViewModel uvm = new UserViewModel();
            try
            {

                string url = GlobalConstants.BaseUrlBloodPresurwe;

                HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
                String ResponseData = String.Empty;

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                var stream = response.GetResponseStream();
                var sr = new StreamReader(stream);
                ResponseData = sr.ReadToEnd();

                // Test

                //  String CheckData = @"{'Email': 'james@example.com','Active': true,'CreatedDateTime': '2013-01-20T00:00:00Z'}";

                //   String CheckData= "{ \"BPDiastolic\":\"Jignesh\",\"HeartRate\":\"Trivedi\" }";
                //End

                UserViewModel model = JsonConvert.DeserializeObject<UserViewModel>(ResponseData);

                // Save value in database

                using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
                {
                    var Entity = new SDGAppDB.POCO.BPDetails();
                    Entity.BPDiastolic = model.UUID;
                    Entity.BPSystolic = model.BPSystolic;
                    Entity.BPDiastolic = model.BPDiastolic;
                    Entity.HeartRate = model.HeartRate;
                    Entity.PuseTransitTime = model.PuseTransitTime;
                    Entity.SDNN = model.SDNN;
                    Entity.RMSDD = model.RMSDD;
                    Entity.RRIntervals = model.RRIntervals;
                    Entity.CreatedDateTime = model.CreatedDateTime;
                    db.BPDetails.Add(Entity);
                    db.SaveChanges();
                }


            }


            catch (Exception Ex)
            {
            }
        }

        //public MeasurementViewModel GetLatestMeasurement(int userID)
        //{
        //    MeasurementViewModel model = new MeasurementViewModel();
        //    try
        //    {
        //        using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
        //        {
        //            model = (from e in db.EPBProfile
        //                     where e.FKUserID == userID
        //                     orderby e.ID descending
        //                     select new MeasurementViewModel
        //                     {
        //                         ID = e.ID,
        //                         UserID = e.FKUserID,
        //                         HR = e.HeartRate,
        //                         HRV = e.HRVDevice.ToString(),
        //                         BP = e.BPSystolic + "/" + e.BPDiastolic,
        //                         Calories = e.Calorie,
        //                         EcgValues = e.ECGValues,
        //                         PpgValues = e.PPGValues,
        //                         ECGElapsedTime = e.ECGElapsedTime,
        //                         PPGElapsedTime = e.PPGElapsedTime,
        //                         CreatedDateTime = e.CreatedDateTime
        //                     }).FirstOrDefault();


        //        }
        //    }
        //    catch (Exception Ex)
        //    {
        //        WriteLog("SDGApp.Models.UserModel - GetLatestMeasurement", Ex.Message);
        //    }
        //    return model;
        //}

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
                WriteLog("SDGApp.Models.UserModel - CheckEmail", Ex.Message);
            }
            return result;
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
                WriteLog("SDGApp.Models.UserModel - CheckUserName", Ex.Message);
            }
            return result;
        }

        //public List<UserDtlsViewModel> GetAllUserForContactList()
        //{
        //   List<UserDtlsViewModel> list = new List<UserDtlsViewModel>();

        //    try
        //    {

        //        using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
        //        {
        //            list = (from u in db.User
        //                     where u.IsActive == true && u.IsDeleted == false
        //                     select new UserDtlsViewModel
        //                     {
        //                        UserID = u.UserID,
        //                        FullName=u.FirstName+ " " + u.LastName,
        //                        UserName=u.UserName,
        //                        UserImage=u.Picture,
        //                        IsActive=u.IsActive

        //                     }).ToList();

        //        }
        //    }

        //    catch (Exception Ex)
        //    {
        //        WriteLog("SDGApp.Models.UserModel - GetAllUserForContactList", Ex.Message);
        //    }


        //    return list;
        //}



    }
}
