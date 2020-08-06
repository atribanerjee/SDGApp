using SDGApp.ViewModel;
using SDGAppDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SDGApp.Models
{
    public class RoleModel : BaseModel
    {
        public bool AddNewRole(RoleViewModel model)
        {
            bool Result = false;
            try
            {
                using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
                {
                    var Entity = new SDGAppDB.POCO.Role();
                    Entity.RoleName = model.RoleName;
                    Entity.IsActive = true;
                    Entity.IsDeleted = false;
                    db.Role.Add(Entity);
                    db.SaveChanges();
                    Result = true;

                }
            }
            catch (Exception Ex)
            {
                WriteLog("SDGApp.Models.RoleModel - AddNewRole", Ex.Message);
            }

            return Result;
        }

        public RoleViewModel GetRoleDetailByID(int RoleID)
        {
            RoleViewModel model = new RoleViewModel();
            try
            {
                using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
                {
                    var Entity = db.Role.Find(RoleID);
                    if (Entity != null && Entity.RoleID > 0)
                    {
                        model.RoleId = Entity.RoleID;
                        model.RoleName = Entity.RoleName;
                        model.IsActive = Entity.IsActive;
                        model.IsDeleted = Entity.IsDeleted;
                    }
                }
            }
            catch (Exception Ex)
            {
                WriteLog("SDGApp.Models.RoleModel - GetRoleDetailByID", Ex.Message);
            }
            return model;
        }

        public bool UpdateRole(RoleViewModel model)
        {
            bool Result = false;
            try
            {
                using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
                {
                    var Entity = db.Role.Find(model.RoleId);
                    if (Entity != null && Entity.RoleID > 0)
                    {
                        Entity.RoleName = model.RoleName;
                        Entity.IsActive = model.IsActive;

                        db.Entry(Entity).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();

                        Result = true;
                    }
                }
            }
            catch (Exception Ex)
            {
                WriteLog("SDGApp.Models.RoleModel - GetRoleDetailByID", Ex.Message);
            }

            return Result;
        }

        public List<RoleViewModel> FetchRoleList(int PageSize, int pageNumber)
        {
            // UsersModel UM = new UsersModel();
            //int UserID = .GetLoggedInUserInfo().UserID;
            //int cid = UM.GetLoggedInUserInfo().CompanyID;
            List<RoleViewModel> lstchunk = new List<RoleViewModel>();
            List<RoleViewModel> lst = new List<RoleViewModel>();
            try
            {
                using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
                {

                    lst = (from R in db.Role
                           where !R.IsDeleted && R.IsActive

                           select new RoleViewModel
                           {
                               RoleId = R.RoleID,
                               RoleName = R.RoleName,
                               PageSize = PageSize,
                               PageNumber = pageNumber
                           }).Distinct().ToList();
                    lstchunk = lst.OrderByDescending(q => q.RoleId).Skip(SkipRecords(PageSize, pageNumber)).Take(PageSize).ToList();
                    lstchunk.ForEach(l => l.TotalRecords = lst.Count());
                }
            }
            catch (Exception Ex)
            {
                WriteLog("SDGApp.Models.RoleModel - FetchUserList", Ex.Message);
            }
            return lstchunk;
        }

        public bool DeleteRolebyID(int ID)
        {
            bool Result = false;
            try
            {
                using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
                {
                    var Entity = db.Role.Find(ID);
                    if (Entity != null && Entity.RoleID > 0)
                    {
                        Entity.IsDeleted = true;
                        db.Entry(Entity).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                        db.SaveChanges();

                        Result = true;
                    }
                }
            }
            catch (Exception Ex)
            {
                WriteLog("SDGApp.Models.RoleModel - DeleteStorebyID", Ex.Message);
            }

            return Result;
        }
    }
}