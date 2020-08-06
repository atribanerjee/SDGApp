using SDGApp.ViewModel;
using SDGAppDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SDGApp.Models
{
    public class UpdateAppModel : UserModel
    {

        public bool SaveUpdateApp(int userid, UpdateAppViewModel model, string apptype)
        {
            Boolean result = false;
            try
            {
                using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
                {
                    var Entitydtls = new SDGAppDB.POCO.AppDetails();

                    Entitydtls.FKUserID = userid;

                    if (apptype.Trim().ToLower().Contains("ios"))
                    {
                        var entitytype = (from type in db.AppType
                                          where type.AppTypeName.Trim().ToLower().Contains("ios")
                                          select type).FirstOrDefault();

                        if (entitytype != null && entitytype.ID > 0)
                        {
                            Entitydtls.FKAppTypeID = entitytype.ID;
                            Entitydtls.AppURL = model.AppiosURL;
                            Entitydtls.AppVersion = model.AppiosVersion;
                        }
                    }
                    else if (apptype.Trim().ToLower().Contains("android"))
                    {
                        var entitytype = (from type in db.AppType
                                          where type.AppTypeName.Trim().ToLower().Contains("android")
                                          select type).FirstOrDefault();

                        if (entitytype != null && entitytype.ID > 0)
                        {
                            Entitydtls.FKAppTypeID = entitytype.ID;
                            Entitydtls.AppURL = model.AppAndroidURL;
                            Entitydtls.AppVersion = model.AppAndroidVersion;
                        }
                    }


                    Entitydtls.UploadDateTime = DateTime.Now;

                    Entitydtls.IsActive = true;
                    Entitydtls.IsDelete = false;


                    db.AppDetails.Add(Entitydtls);
                    db.SaveChanges();

                    result = true;

                }
            }
            catch (Exception Ex)
            {


            }

            return result;
        }
        
        public List<UpdateAppViewModel> FetchAppListByType(int AppType, int PageSize, int pageNumber)
        {

            List<UpdateAppViewModel> lstchunk = new List<UpdateAppViewModel>();
            List<UpdateAppViewModel> lst = new List<UpdateAppViewModel>();
            try
            {
                using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
                {
                    if (AppType == 1)
                    {
                        lst = (from app in db.AppDetails
                               where app.FKAppTypeID == AppType && app.IsDelete == false && app.IsActive == true
                               
                               select new UpdateAppViewModel
                               {
                                   ID = app.ID,
                                   AppAndroidURL = app.AppURL,
                                   AppAndroidVersion = app.AppVersion
                                   
                               }).Distinct().OrderByDescending(q => q.ID).ToList();
                    }
                    else
                    {
                        lst = (from app in db.AppDetails
                               where app.FKAppTypeID == AppType && app.IsDelete == false && app.IsActive == true
                              
                               select new UpdateAppViewModel
                               {
                                   ID = app.ID,
                                   FKAppTypeID = app.FKAppTypeID,
                                   AppiosURL = app.AppURL,
                                   AppiosVersion = app.AppVersion

                               }).Distinct().OrderByDescending(q => q.ID).ToList();

                    }
                                      
                }
            }
            catch (Exception Ex)
            {
                WriteLog("SDGApp.Models.UpdateAppModel - FetchAppListByType", Ex.Message);
            }
            return lst;
        }

        public bool DeleteAppbyID(int ID)
        {
            bool Result = false;
            try
            {
                using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
                {
                    var Entity = db.AppDetails.Find(ID);
                    if (Entity != null && Entity.ID > 0)
                    {
                        Entity.IsDelete = true;

                        db.Entry(Entity).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                        db.SaveChanges();

                        Result = true;
                    }
                }
            }
            catch (Exception Ex)
            {
                WriteLog("SDGApp.Models.UpdateAppModel - DeleteAppbyID", Ex.Message);
            }

            return Result;
        }


    }
}