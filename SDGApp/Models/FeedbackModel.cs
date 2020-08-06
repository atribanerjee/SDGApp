using SDGApp.ViewModel;
using SDGAppDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SDGApp.Models
{
  
    public class FeedbackModel:BaseModel
    {
        UserModel UM;
        HelpModel HM;
        public FeedbackModel()
        {
            UM = new UserModel();
            HM = new HelpModel();
        }
        public bool AddNewFeedback(FeedbackViewModel model)
        {
            bool result = false;
            try
            {
                using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
                {
                    var entity = new SDGAppDB.POCO.UserFeedBack();
                    entity.FKHelpMOduleID = model.FKHelpModuleID;
                    //   entity.FKUserID = GetIntegerValue(GetSessionValue("LoggedInUserID"));
                    entity.FKUserID = UM.GetLoggedInUserInfo().UserID;
                    //entity.FeedbackRating = model.FeedbackRating;
                    entity.FeedbackContent = model.FeedbackContent;

                    entity.FKCompanyID = UM.GetLoggedInUserInfo().CompanyID;

                    db.UserFeedBack.Add(entity);
                    db.SaveChanges();

                    result = true;
                }
            }
            catch (Exception Ex)
            {
                WriteLog("SDGApp.Models.FeedbackModel - AddNewFeedback", Ex.Message);
            }
            return result;
        }

        public FeedbackViewModel GetFeedbackDetailByID(int FeedbackID)
        {
            FeedbackViewModel model = new FeedbackViewModel();
            try
            {
                using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
                {
                    int DefaultCompanyID = UM.GetLoggedInUserInfo().CompanyID;
                    //var entity = db.UserFeedback.Find(model.ID);
                    var entity = db.UserFeedBack.Where(fd => fd.UserFeedbackID == FeedbackID && fd.FKCompanyID == DefaultCompanyID).FirstOrDefault();


                    if (entity != null && entity.UserFeedbackID > 0)
                    {
                        model.ID = entity.UserFeedbackID;
                        model.FKHelpModuleID = entity.FKHelpMOduleID;
                        model.FKUserID = entity.FKUserID;
                        //model.FeedbackRating = entity.FeedbackRating;
                        model.FeedbackContent = entity.FeedbackContent;
                        model.FKCompanyID = entity.FKCompanyID;
                    }
                }

                var UserEntity = UM.GetUserDetailByUserID(model.FKUserID);

                if (UserEntity != null)
                {
                    model.UserFullName = UserEntity.FirstName + " " + UserEntity.LastName;
                }

            }
            catch (Exception Ex)
            {
                WriteLog("SDGApp.Models.FeedbackModel - GetFeedbackDetailByID", Ex.Message);
            }
            return model;
        }

        public bool UpdateFeedback(FeedbackViewModel model)
        {
            bool Result = false;
            try
            {
                using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
                {
                    var entity = db.UserFeedBack.Find(model.ID);

                    if (entity != null && entity.UserFeedbackID > 0)
                    {
                        entity.FKHelpMOduleID = model.FKHelpModuleID;
                        //entity.FKUserID = model.FKUserID;
                        //entity.FeedbackRating = model.FeedbackRating;
                        entity.FeedbackContent = model.FeedbackContent;

                        db.Entry(entity).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();

                        Result = true;
                    }
                }
            }
            catch (Exception Ex)
            {
                WriteLog("SDGApp.Models.FeedbackModel - UpdateFeedback", Ex.Message);
            }

            return Result;
        }

        public List<FeedbackViewModel> GetFeedbackList(int CompanyID, int UserID, int pageSize, int pageNumber, string SearchKey = "")
        {
            List<FeedbackViewModel> lst = new List<FeedbackViewModel>();
            List<FeedbackViewModel> lstchunk = new List<FeedbackViewModel>();

            try
            {
                using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
                {
                    if (string.IsNullOrEmpty(SearchKey))
                    {
                        lst = (from fd in db.UserFeedBack
                               join hm in db.HelpModule on fd.FKHelpMOduleID equals hm.HelpModuleID
                               join Cur in db.CompanyUserRole on fd.FKUserID equals Cur.FKUserID
                               join u in db.User on fd.FKUserID equals u.UserID

                               where fd.FKUserID == UserID && fd.FKCompanyID == CompanyID && !u.IsDeleted && !fd.IsDeleted
                               select new FeedbackViewModel
                               {
                                   ID = fd.UserFeedbackID,
                                   Topic = hm.Topic,
                                   //FeedbackRating = fd.FeedbackRating,
                                   FeedbackContent = fd.FeedbackContent,
                                   FKUserID = u.UserID,
                                   UserFullName = u.FirstName + " " + u.LastName,
                                   PageSize = pageSize,
                                   PageNumber = pageNumber
                               }).Distinct().ToList();
                    }
                    else if (!string.IsNullOrEmpty(SearchKey) && SearchKey.Length > 0)
                    {
                        lst = (from fd in db.UserFeedBack
                               join hm in db.HelpModule on fd.FKHelpMOduleID equals hm.HelpModuleID
                               join Cur in db.CompanyUserRole on fd.FKUserID equals Cur.FKUserID
                               join u in db.User on fd.FKUserID equals u.UserID

                               where fd.FKUserID == UserID && fd.FKCompanyID == CompanyID && !u.IsDeleted && !fd.IsDeleted
                               && (hm.Topic.Trim().ToLower().Contains(SearchKey.Trim().ToLower()))

                               select new FeedbackViewModel
                               {
                                   ID = fd.UserFeedbackID,
                                   Topic = hm.Topic,
                                   //FeedbackRating = fd.FeedbackRating,
                                   FeedbackContent = fd.FeedbackContent,
                                   FKUserID = u.UserID,
                                   UserFullName = u.FirstName + " " + u.LastName,
                                   PageSize = pageSize,
                                   PageNumber = pageNumber
                               }).Distinct().ToList();
                    }
                }
                lstchunk = lst.OrderByDescending(q => q.ID).Skip(SkipRecords(pageSize, pageNumber)).Take(pageSize).ToList();
                lstchunk.ForEach(l => l.TotalRecords = lst.Count());
            }
            catch (Exception Ex)
            {
                WriteLog("SDGApp.Models.FeedbackModel - GetFeedbackList", Ex.Message);
            }
            return lstchunk;
        }


        public bool DeleteFeedbackByID(int FeedbackID)
        {
            bool result = false;
            try
            {
                using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
                {
                    var entity = db.UserFeedBack.Find(FeedbackID);

                    if (entity != null && entity.UserFeedbackID > 0)
                    {
                        entity.IsDeleted = true;
                        db.Entry(entity).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                        result = true;
                    }
                }
            }
            catch (Exception Ex)
            {
                WriteLog("SDGApp.Models.FeedbackModel - DeleteFeedbackID", Ex.Message);
            }
            return result;
        }
    }
}