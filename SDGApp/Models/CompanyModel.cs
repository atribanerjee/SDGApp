using SDGApp.ViewModel;
using SDGAppDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SDGApp.Models
{
    public class CompanyModel : BaseModel
    {

        public bool AddNewCompany(CompanyViewModel model)
        {
            bool Result = false;
            String rndCompanyCode = String.Empty;
            try
            {
                using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
                {
                    for (int i = 0; i < 1; i++)
                    {
                        rndCompanyCode = RandomString(9);
                        var existcompanycode = (from com in db.Company
                                                where com.CompanyCode.ToLower() == rndCompanyCode.ToLower()
                                                select com).FirstOrDefault();

                        if (existcompanycode == null)
                        {
                            var entity = new SDGAppDB.POCO.Company();

                            var existCompany = (from com in db.Company
                                                where com.CompanyName.ToLower() == model.CompanyName.ToLower()
                                                select com).FirstOrDefault();

                            if (existCompany == null)
                            {
                                entity.CompanyName = model.CompanyName;
                                entity.CompanyAddress = model.CompanyAddress;
                                entity.CompanyCode = rndCompanyCode;
                                entity.CreatedOn = DateTime.Now;
                                entity.IsActive = true;
                                entity.IsDeleted = false;
                                db.Company.Add(entity);
                                db.SaveChanges();

                                Result = true;
                            }
                            else
                            {
                                Result = false;
                            }
                        }
                        else
                        {
                            i--;
                        }
                    }

                }
            }
            catch (Exception Ex)
            {
                WriteLog("SDGApp.Models.CompanyModel - AddNewCompany", Ex.Message);
            }


            return Result;
        }

        public CompanyViewModel GetComanyDetailById(int companyid)
        {
            CompanyViewModel model = new CompanyViewModel();
            try
            {
                using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
                {
                    var entity = (from c in db.Company where c.CompanyID == companyid select new { c }).FirstOrDefault();
                    if (entity != null)
                    {
                        model.CompanyID = entity.c.CompanyID;
                        model.CompanyName = entity.c.CompanyName;
                        model.CompanyAddress = entity.c.CompanyAddress;
                        model.IsActive = entity.c.IsActive;
                        model.IsDeleted = entity.c.IsDeleted;
                    }
                }
            }
            catch (Exception Ex)
            {
                WriteLog("SDGApp.Models.CompanyModel - GetComanyDetailById", Ex.Message);
            }
            return model;
        }

        public bool UpdateCompanyProfile(CompanyViewModel cvm)
        {
            bool Result = false;
            try
            {
                using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
                {
                    var entity = db.Company.Find(cvm.CompanyID);

                    if (entity != null && entity.CompanyID > 0)
                    {
                        entity.CompanyName = cvm.CompanyName;
                        entity.CompanyAddress = cvm.CompanyAddress;
                        entity.IsActive = cvm.IsActive;
                        entity.IsDeleted = cvm.IsDeleted;

                        if (String.IsNullOrEmpty(entity.CompanyCode))
                        {
                            String rndStringcode = String.Empty;

                            for (int i = 0; i < 1; i++)
                            {
                                rndStringcode = RandomString(9);

                                var existCode = (from com in db.Company
                                                 where com.CompanyCode.ToLower() == rndStringcode.ToLower()
                                                 select com).FirstOrDefault();

                                if (existCode == null)
                                {
                                    entity.CompanyCode = rndStringcode;
                                }
                                else
                                {
                                    i--;
                                }
                                
                            }
                        }

                        db.Entry(entity).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                        Result = true;
                    }
                }
            }
            catch (Exception Ex)
            {
                WriteLog("SDGApp.Models.CompanyModel - GetComanyDetailById", Ex.Message);
            }
            return Result;

        }

        public List<CompanyViewModel> FetchCompanyList(int pageSize, int pageNumber, string SearchKey = "")
        {
            List<CompanyViewModel> lstchunk = new List<CompanyViewModel>();
            List<CompanyViewModel> lst = new List<CompanyViewModel>();
            try
            {
                using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
                {
                    if (string.IsNullOrEmpty(SearchKey))
                    {

                        lst = (from comp in db.Company
                               where comp.IsDeleted == false
                               && comp.CompanyName.ToLower() !="no company"
                               select new CompanyViewModel
                               {
                                   CompanyID = comp.CompanyID,
                                   CompanyName = comp.CompanyName,
                                   CompanyAddress = comp.CompanyAddress,
                                   CreatedOn = comp.CreatedOn,
                                   IsActive = comp.IsActive,
                                   IsDeleted = comp.IsDeleted,
                                   CompanyCode = comp.CompanyCode,
                                   PageSize = pageSize,
                                   PageNumber = pageNumber,
                               }).ToList();
                    }
                    else if (!string.IsNullOrEmpty(SearchKey) && SearchKey.Length > 0)
                    {
                        lst = (from comp in db.Company
                               where comp.IsDeleted == false &&
                               (comp.CompanyName.Trim().ToLower().Contains(SearchKey.Trim().ToLower())
                               || comp.CompanyAddress.Trim().ToLower().Contains(SearchKey.Trim().ToLower()))
                                && comp.CompanyName.ToLower() != "no company"
                               select new CompanyViewModel
                               {
                                   CompanyID = comp.CompanyID,
                                   CompanyName = comp.CompanyName,
                                   CompanyAddress = comp.CompanyAddress,
                                   CreatedOn = comp.CreatedOn,
                                   IsActive = comp.IsActive,
                                   IsDeleted = comp.IsDeleted,
                                   CompanyCode = comp.CompanyCode,
                                   PageSize = pageSize,
                                   PageNumber = pageNumber,
                               }).ToList();
                    }

                    if (lst != null && lst.Count > 0)
                    {
                        lstchunk = lst.OrderByDescending(q => q.CompanyID).Skip(SkipRecords(pageSize, pageNumber)).Take(pageSize).ToList();
                        lstchunk.ForEach(l => l.TotalRecords = lst.Count());
                    }

                }

            }
            catch (Exception Ex)
            {
                WriteLog("SDGApp.Models.CompanyModel - FetchCompanyList", Ex.Message);
            }
            return lstchunk;
        }

        public Boolean JoinOrganization(String CompanyCode, int UserId,int UserRoleId)
        {
            Boolean result = false;
            try
            {
                using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
                {
                    var existCompanycode = (from com in db.Company
                                            where com.CompanyCode.ToLower() == CompanyCode.ToLower()
                                            && com.IsActive 
                                            && !com.IsDeleted
                                            select com).FirstOrDefault();

                    if (existCompanycode != null && existCompanycode.CompanyID > 0)
                    {
                        var entity = (from comUsrRole in db.CompanyUserRole
                                      where comUsrRole.FKCompanyID == existCompanycode.CompanyID
                                      && comUsrRole.FKUserID == UserId
                                      select comUsrRole).FirstOrDefault();

                        if (entity != null && entity.CompanyUserRoleID > 0)
                        {
                            //Update data 

                            String rndStringcode = String.Empty;

                            for (int i = 0; i < 1; i++)
                            {
                                rndStringcode = RandomString(9);

                                var existUsercodethisOrg = (from comUsrRole in db.CompanyUserRole
                                                            where comUsrRole.UserCode.ToLower() == rndStringcode.ToLower()
                                                            && comUsrRole.FKCompanyID == existCompanycode.CompanyID
                                                            select comUsrRole).FirstOrDefault();
                                if (existUsercodethisOrg == null)
                                {
                                    entity.IsDefault = false;
                                    entity.UserCode = rndStringcode;
                                    entity.JoiningDate = DateTime.Now;

                                    db.Entry(entity).State = System.Data.Entity.EntityState.Modified;
                                    db.SaveChanges();
                                    result = true;
                                }
                                else
                                {
                                    i--;
                                }

                            }
                        }
                        else
                        {
                            //Inser New entry 
                            String rndStringcode = String.Empty;

                            for (int i = 0; i < 1; i++)
                            {
                                rndStringcode = RandomString(9);

                                var existUsercodethisOrg = (from comUsrRole in db.CompanyUserRole
                                                            where comUsrRole.UserCode.ToLower() == rndStringcode.ToLower()
                                                            && comUsrRole.FKCompanyID == existCompanycode.CompanyID
                                                            select comUsrRole).FirstOrDefault();

                                if (existUsercodethisOrg == null)
                                {
                                    var addcompanyuserrole = new SDGAppDB.POCO.CompanyUserRole();

                                    addcompanyuserrole.FKCompanyID = existCompanycode.CompanyID;
                                    addcompanyuserrole.FKUserID = UserId;
                                    addcompanyuserrole.FKRoleID = UserRoleId;
                                    addcompanyuserrole.IsDefault = false;
                                    addcompanyuserrole.UserCode = rndStringcode;
                                    addcompanyuserrole.JoiningDate = DateTime.Now;

                                    db.CompanyUserRole.Add(addcompanyuserrole);
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

                WriteLog("SDGApp.Models.CompanyModel - JoinOrganization", Ex.Message);
            }

            return result;
        }

        public List<UserOrganisationViewModel> GetOrganisationListByUserId(int UserId)
        {
            List<UserOrganisationViewModel> lst = new List<UserOrganisationViewModel>();

            try
            {
                using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
                {
                    

                    lst = (from compusrrole in db.CompanyUserRole
                           join comp in db.Company on compusrrole.FKCompanyID equals comp.CompanyID
                           where comp.IsDeleted == false
                           && compusrrole.FKUserID== UserId
                           && comp.CompanyName.ToLower() !="no company"
                           select new UserOrganisationViewModel
                           {
                               OrganisationName=comp.CompanyName,
                               UserCode= compusrrole.UserCode,
                               JoiningDate=compusrrole.JoiningDate
                           }).ToList();
                }

            }
            catch (Exception Ex)
            {
                WriteLog("SDGApp.Models.CompanyModel - GetOrganisationListByUserId", Ex.Message);

            }

            return lst;

        }

        public bool DeleteCompanyByCompanyID(int CompanyID)
        {
            bool Result = false;
            try
            {
                using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
                {
                    var entity = db.Company.Find(CompanyID);
                    if (entity != null)
                    {
                        entity.IsDeleted = true;
                        entity.IsActive = false;
                        db.Entry(entity).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();

                        Result = true;
                    }
                }
            }
            catch (Exception Ex)
            {
                WriteLog("SDGApp.Models.CompanyModel - DeleteCompanyByCompanyID", Ex.Message);
            }
            return Result;
        }

    }
}