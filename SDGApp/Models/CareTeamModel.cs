using System;
using SDGApp.ViewModel;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SDGAppDB;

namespace SDGApp.Models
{
    public class CareTeamModel : UserModel
    {
        public List<MessageSearchViewModel> GetCarePeopleList(int LogedInUserID, string prefix)
        {
            List<MessageSearchViewModel> lst = new List<MessageSearchViewModel>();

            try
            {
                using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
                {

                    lst = (from u in db.User
                           where (u.FirstName.Trim().ToLower().StartsWith(prefix.Trim().ToLower())
                           || u.LastName.Trim().ToLower().StartsWith(prefix.Trim().ToLower())
                           || u.Email.Trim().ToLower().StartsWith(prefix.Trim().ToLower())
                           )
                           && u.UserID != LogedInUserID
                           && u.UserID !=(from cp in db.CarePeople where cp.RequestUserID==LogedInUserID && cp.CarePersonUserID==u.UserID && !cp.IsDeleted select cp.CarePersonUserID).FirstOrDefault()
                           && u.IsActive
                           && !u.IsDeleted
                           select new MessageSearchViewModel
                           {
                               UserID = u.UserID,
                               Name = u.FirstName + " " + u.LastName,
                               Picture = (!string.IsNullOrEmpty(u.Picture) ? "/Content/images/" + u.Picture : "/Content/Latest/images/no-image-profile-male-sm.png"),
                               Email = u.Email
                           }).ToList();
                   
                }

            }
            catch (Exception Ex)
            {
                WriteLog("SDGApp.Models.CareTeamModel - GetCarePeopleList", Ex.Message);
            }
            return lst;
        }

        public Boolean SendRequestToCarePerson(int LoginUserID, int SenderUserID)
        {
            Boolean Result = false;

            try
            {
                using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
                {
                    if (LoginUserID > 0 && SenderUserID > 0)
                    {
                        var entity = new SDGAppDB.POCO.CarePeople();

                        entity.CarePersonUserID = SenderUserID;
                        entity.RequestUserID = LoginUserID;
                        entity.IsViewed = false;
                        entity.IsActive = true;
                        entity.IsDeleted = false;
                        entity.CreatedDateTime = DateTime.Now;

                        db.CarePeople.Add(entity);
                        db.SaveChanges();

                        Result = true;
                    }
                }

            }
            catch (Exception Ex)
            {

                WriteLog("SDGApp.Models.CareTeamModel - SendRequestToCarePerson", Ex.Message);
            }

            return Result;
        }

        public Boolean ChangeViewingPermission(int CarePeopleID, int chkBoxVal)
        {
            Boolean Result = false;

            try
            {
                using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
                {
                    if (CarePeopleID > 0)
                    {
                        var entity = db.CarePeople.Find(CarePeopleID);

                        if (entity != null && entity.CarePeopleID > 0)
                        {
                            if (chkBoxVal == 1)
                            {
                                entity.IsViewed = true;
                            }
                            else
                            {
                                entity.IsViewed = false;
                            }

                            db.Entry(entity).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();
                            Result = true;
                        }
                    }
                }
            }
            catch (Exception Ex)
            {

                WriteLog("SDGApp.Models.CareTeamModel - ChangeViewingPermission", Ex.Message);
            }

            return Result;
        }

        public List<CareTeamViewModel> GetListCareTeam(int UserID)
        {
            List<CareTeamViewModel> lst = new List<CareTeamViewModel>();

            try
            {
                using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
                {
                    lst = (from cp in db.CarePeople
                           join u in db.User on cp.CarePersonUserID equals u.UserID
                           where cp.RequestUserID == UserID
                           && !cp.IsDeleted
                           select new CareTeamViewModel
                           {
                               CarePeopleID = cp.CarePeopleID,
                               FirstName = u.FirstName,
                               LastName = u.LastName,
                               Email = u.Email,
                               Phone = u.Phone,
                               IsViewed = cp.IsViewed,
                               CreatedDateTime = cp.CreatedDateTime

                           }).ToList();

                    if (lst != null && lst.Count > 0)
                    {
                        lst.ForEach(l => l.CreatedDateTimeStamp = l.CreatedDateTime.ToString("MM-dd-yyyy"));
                    }
                }
            }
            catch (Exception Ex)
            {


            }

            return lst;
        }

        public Boolean DeleteCarePerson(int CarePeopleID)
        {
            Boolean Result = false;
            try
            {
                using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
                {
                    if (CarePeopleID > 0)
                    {
                        var entity = db.CarePeople.Find(CarePeopleID);

                        if (entity != null && entity.CarePeopleID > 0)
                        {
                            entity.IsDeleted = true;

                            db.Entry(entity).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();
                            Result = true;
                        }
                    }
                }

            }
            catch (Exception Ex)
            {

                throw;
            }
            return Result;
        }


    }
}