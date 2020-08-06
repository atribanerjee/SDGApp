using SDGApp.ViewModel;
using SDGAppDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SDGApp.Models
{
    public class HelpModel : BaseModel
    {
        #region :: 1. List

        public List<HelpViewModel> GetHelpList(HelpViewModel model, int pageSize, int pageNumber, string SearchKey = "")
        {
            List<HelpViewModel> lst = new List<HelpViewModel>();
            List<HelpViewModel> lstchunk = new List<HelpViewModel>();
            try
            {
                using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
                {
                    if (string.IsNullOrEmpty(SearchKey))
                    {

                        lst = (from h in db.HelpContent
                               join hm in db.HelpModule on h.FkTopicID equals hm.HelpModuleID
                               select new HelpViewModel
                               {
                                   HelpID = h.HelpContentID,
                                   Topic = hm.Topic,
                                   HelpText = h.HelpText,
                                   FromRow = model.FromRow,
                                   ToRow = model.ToRow,
                                   PageNumber = pageNumber,
                                   PageSize = pageSize
                               }).ToList();
                    }
                    else if (!string.IsNullOrEmpty(SearchKey) && SearchKey.Length > 0)
                    {
                        lst = (from h in db.HelpContent
                               join hm in db.HelpModule on h.FkTopicID equals hm.HelpModuleID
                               where (hm.Topic.Trim().ToLower().Contains(SearchKey.Trim().ToLower()))
                               select new HelpViewModel
                               {
                                   HelpID = h.HelpContentID,
                                   Topic = hm.Topic,
                                   HelpText = h.HelpText,
                                   FromRow = model.FromRow,
                                   ToRow = model.ToRow,
                                   PageNumber = pageNumber,
                                   PageSize = pageSize
                               }).ToList();
                    }
                }
                lstchunk = lst.OrderByDescending(q => q.HelpID).Skip(SkipRecords(pageSize, pageNumber)).Take(pageSize).ToList();
                lstchunk.ForEach(l => l.TotalRecords = lst.Count());
            }
            catch (Exception Ex)
            {
                WriteLog("SDGApp.Models.HelpModel - GetHelpList", Ex.Message);
            }
            return lstchunk;
        }




        public HelpViewModel GetHelpByID(Int32 HelpID)
        {
            HelpViewModel model = new HelpViewModel();
            try
            {
                using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
                {
                    var entity = (from h in db.HelpContent
                                  join hm in db.HelpModule on h.FkTopicID equals hm.HelpModuleID
                                  where h.HelpContentID == HelpID
                                  select new HelpViewModel
                                  {
                                      HelpID = h.HelpContentID,
                                      Topic = hm.Topic,
                                      HelpText = h.HelpText,
                                      FromRow = model.FromRow,
                                      ToRow = model.ToRow
                                  }).FirstOrDefault();

                    if (entity != null && entity.HelpID > 0)
                    {
                        model.HelpID = entity.HelpID;
                        model.Topic = entity.Topic;
                        model.HelpText = entity.HelpText;
                    }
                }
            }
            catch (Exception Ex)
            {
                WriteLog("SDGApp.Models.HelpModel - GetHelpByID", Ex.Message);
            }
            return model;
        }

        #endregion

        #region :: 4. Edit

        public bool UpdateHelp(HelpViewModel model)
        {
            bool Result = false;
            try
            {
                using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
                {
                    var entity = db.HelpContent.Find(model.HelpID);

                    if (entity != null && entity.HelpContentID > 0)
                    {
                        entity.HelpContentID = model.HelpID;
                        //entity.Topic = model.Topic;
                        entity.HelpText = model.HelpText;

                        db.Entry(entity).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();


                        Result = true;
                    }
                }
            }
            catch (Exception Ex)
            {
                WriteLog("SDGApp.Models.HelpModel - UpdateHelp", Ex.Message);
            }

            return Result;
        }

        #endregion

        #region :: 5. Delete

        public bool DeleteHelpID(int HelpID)
        {
            bool result = false;
            try
            {
                using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
                {
                    var entity = db.HelpContent.Find(HelpID);

                    if (entity != null && entity.HelpContentID > 0)
                    {
                        db.Entry(entity).State = System.Data.Entity.EntityState.Deleted;
                        db.SaveChanges();
                        result = true;
                    }
                }
            }
            catch (Exception Ex)
            {
                WriteLog("SDGApp.Models.HelpModel - DeleteHelpID", Ex.Message);
            }
            return result;
        }

        #endregion

        public List<SelectListItem> GetTopicDDL()
        {
            List<SelectListItem> _list = new List<SelectListItem>();

            try
            {
                using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
                {
                    _list = (from hm in db.HelpModule
                             select new SelectListItem { Text = hm.Topic, Value = hm.HelpModuleID.ToString() }
                             ).ToList();
                }
            }
            catch (Exception Ex)
            {
                WriteLog("SDGApp.Models.HelpModel - GetTopicDDL", Ex.Message);
            }

            return _list;
        }
        //public void SaveHelp(HelpViewModel model)
        //{
        //    try
        //    {
        //        if (model.HelpID <= 0)
        //        {
        //            using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
        //            {
        //                var entity = new SDGAppDB.POCO.HelpContents();
        //                //entity.HelpID = model.HelpID;
        //                entity.FKTopicID = model.FKTopicID;
        //                entity.HelpText = model.HelpText;

        //                db.HelpContents.Add(entity);
        //                db.SaveChanges();
        //            }
        //        }
        //        else
        //        {
        //            UpdateHelp(model);
        //        }
        //    }
        //    catch (Exception Ex)
        //    {
        //        WriteLog("SDGApp.Models.HelpModel - SaveHelp", Ex.Message);
        //    }
        //}

        //public bool CheckHelpTopic(int FKTopicID)
        //{
        //    bool Result = false;
        //    try
        //    {
        //        using (SRSDBContext db = new SRSDBContext(GlobalConstants.DBConn()))
        //        {
        //            var entity = db.HelpContents.Where(x => x.FKTopicID == FKTopicID).FirstOrDefault();

        //            if (entity != null)
        //            {
        //                Result = true;
        //            }
        //        }
        //    }
        //    catch (Exception Ex)
        //    {
        //        WriteLog("SDGApp.Models.HelpModel - CheckHelpTopic", Ex.Message);
        //    }

        //    return Result;
        //}
        public string GetHelpSessionValue()
        {
            string strValue = GetStringValue(GetSessionValue("HelpPageAction"));

            return strValue;
        }

        public HelpViewModel GetRelatedHelp(string TopicText)
        {
            HelpViewModel model = new HelpViewModel();
            try
            {
                using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
                {
                    var entity = (from h in db.HelpContent
                                  join hm in db.HelpModule on h.FkTopicID equals hm.HelpModuleID
                                  where hm.Topic.Contains(TopicText)
                                  select new HelpViewModel
                                  {
                                      Topic = hm.Topic,
                                      HelpText = h.HelpText
                                  });

                    if (entity != null)
                    {
                        foreach (var item in entity)
                        {
                            model.Topic = item.Topic;
                            model.HelpText = item.HelpText;
                        }
                    }
                }
            }
            catch (Exception Ex)
            {
                WriteLog("HelpModel - GetHelpByID", Ex.Message);
            }
            return model;
        }

        #region :: Set Session Get Session Values to Action Controller

        public void SetHelpSessionValue(string KeyValue)
        {
            SetSessionValue("HelpPageAction", KeyValue); // set session value for user view.
        }

        public void DeleteHelpSessionValue()
        {
            SetSessionValue("HelpPageAction", null);
        }

        #endregion

    }
}