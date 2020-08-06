using SDGApp.ViewModel;
using SDGAppDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SDGApp.Models
{
    public class CarePeopleModel : UserModel
    {

        public Boolean AddRequestForCare(CarePeopleViewModel model)
        {
            Boolean Result = false;

            try
            {
                using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
                {
                    var Entity = new SDGAppDB.POCO.CarePeople();

                    Entity.RequestUserID = model.RequestUserID;
                    Entity.Status = model.Status;
                    Entity.IsActive = true;
                    Entity.IsDeleted = false;
                    Entity.CreatedDateTime = DateTime.Now;

                    db.CarePeople.Add(Entity);
                    db.SaveChanges();
                    Result = true;
                }
            }
            catch (Exception Ex)
            {


            }
            return Result;
        }

        public List<CarePeopleViewModel> RequestForCareList(int UserID, int PageIndex = 1, int PageSize = 10)
        {
            List<CarePeopleViewModel> lst = new List<CarePeopleViewModel>();

            try
            {
                using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
                {
                    lst = (from CP in db.CarePeople
                           join u in db.User on CP.RequestUserID equals u.UserID
                           where CP.CarePersonUserID == UserID
                           && !CP.IsDeleted && CP.IsActive && CP.IsViewed
                           select new CarePeopleViewModel
                           {
                               CarePeopleID = CP.CarePeopleID,
                               RequestUserID = CP.RequestUserID,
                               FirstName = u.FirstName,
                               LastName = u.LastName,
                               Email = u.Email,
                               Phone = u.Phone,
                               Address = u.Address,
                               Status = CP.Status,
                               IsActive = CP.IsActive,
                               IsDeleted = CP.IsDeleted,
                               CreatedDateTime = CP.CreatedDateTime
                           }).OrderByDescending(x => x.CarePeopleID).ToList();

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

        public Boolean CheckViewingPermission(int ViewingDtlUserID, int LoginUserID)
        {
            Boolean Result = false;
            try
            {
                using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
                {
                    if (ViewingDtlUserID > 0 && LoginUserID > 0)
                    {

                        var entity = (from cp in db.CarePeople
                                      where cp.CarePersonUserID == LoginUserID
                                      && cp.RequestUserID == ViewingDtlUserID
                                      select cp).FirstOrDefault();

                        if (entity != null && entity.CarePeopleID > 0)
                        {
                            if (entity.IsViewed)
                            {
                                Result = true;
                            }
                            else
                            {
                                Result = false;
                            }
                        }

                    }
                }

            }
            catch (Exception Ex)
            {


            }

            return Result;
        }
    }
}