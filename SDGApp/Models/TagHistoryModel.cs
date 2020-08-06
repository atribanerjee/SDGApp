using SDGApp.ViewModel;
using SDGAppDB;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SDGApp.Models
{
    public class TagHistoryModel : UserModel
    {

        public List<TagsHistoryViewModel> GetTagHistoryList(DateTime currentdate, string type)        {            List<TagsHistoryViewModel> _list = new List<TagsHistoryViewModel>();

            int Day = currentdate.Day;            int Month = currentdate.Month;            int Year = currentdate.Year;            int UserID = GetLoggedInUserInfo().UserID;            if (UserID > 0)            {                try                {                    using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))                    {                        if (type == "day")                        {                            _list = (from tm in db.TagsMaster                                     join tl in db.TagLabel on tm.FKTagLabelID equals tl.ID
                                     where tm.FKUserID == UserID && tm.CreatedDateTime.Day == Day && tm.CreatedDateTime.Month == Month &&                                     tm.CreatedDateTime.Year == Year                                     select new TagsHistoryViewModel                                     {                                         ID = tm.ID,                                         FKTagLabelID = tm.FKTagLabelID,                                         TagValue = tm.TagValue,                                         Note = tm.Note,                                         TypeName = tl.LabelName,                                         CreatedDateTime = tm.CreatedDateTime

                                     }                                 ).OrderByDescending(x => x.CreatedDateTime).ToList();

                            _list.ForEach(l => l.CreatedDateTimeTimestamp = GetNotNullDateTimeValue(l.CreatedDateTime).ToString("MM-dd-yyyy HH:mm"));                        }                        else if (type == "month")                        {                            DateTime startdayofmonth = new DateTime(Year, Month, 1);                            DateTime enddayofmonth = currentdate.AddDays(1);                            int totaldays = System.DateTime.DaysInMonth(Year, Month);

                            SqlParameter[] Params =
                           {
                                    new SqlParameter("@STARTDATE",startdayofmonth.ToString("yyyy-MM-dd")),
                                    new SqlParameter("@ENDDATE",enddayofmonth.ToString("yyyy-MM-dd")),
                                    new SqlParameter("@NOOFDAYS",totaldays),
                                    new SqlParameter("@USERID",UserID)

                                };                            DataSet ds = SqlHelper.ExecuteDataset(GlobalConstants.DBConn(), CommandType.StoredProcedure, "USP_GETTAGHISTORY_WEEKMONTHWISE", Params);
                            if (ds != null && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                            {
                                _list = (from DataRow row in ds.Tables[0].Rows

                                         select new TagsHistoryViewModel()
                                         {
                                             CreatedDateTimeTimestamp = GetNotNullDateTimeValue(row["CREATEDDATE"]).ToString("MM-dd-yyyy HH:mm"),
                                             ID = GetIntegerValue(row["ID"]),
                                             TypeName = GetStringValue(row["LABELNAME"]),
                                             TagValue = GetIntegerValue(row["TAGVALUE"]),
                                             Note = GetStringValue(row["NOTE"]),

                                         }).OrderByDescending(x => x.CreatedDateTime).ToList();

                            }                        }                        else if (type == "week")                        {                            int dayofweek = Convert.ToInt32(currentdate.DayOfWeek);                            DateTime endday = currentdate.AddDays(1);                            DateTime startdayofweek = currentdate.AddDays(-dayofweek);                            SqlParameter[] Params =
                            {
                                    new SqlParameter("@STARTDATE",startdayofweek.ToString("yyyy-MM-dd")),
                                    new SqlParameter("@ENDDATE",endday.ToString("yyyy-MM-dd")),
                                     new SqlParameter("@NOOFDAYS",7),
                                    new SqlParameter("@USERID",UserID)

                                };                            DataSet ds = SqlHelper.ExecuteDataset(GlobalConstants.DBConn(), CommandType.StoredProcedure, "USP_GETTAGHISTORY_WEEKMONTHWISE", Params);
                            if (ds != null && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                            {
                                _list = (from DataRow row in ds.Tables[0].Rows

                                         select new TagsHistoryViewModel()
                                         {
                                             CreatedDateTimeTimestamp = GetNotNullDateTimeValue(row["CREATEDDATE"]).ToString("MM-dd-yyyy HH:mm"),
                                             ID = GetIntegerValue(row["ID"]),
                                             TypeName = GetStringValue(row["LABELNAME"]),
                                             TagValue = GetIntegerValue(row["TAGVALUE"]),
                                             Note = GetStringValue(row["NOTE"]),

                                         }).OrderByDescending(x => x.CreatedDateTime).ToList();

                            }                        }                    }                }                catch (Exception Ex)                {                    WriteLog("GetTagHistory List Problems", Ex.Message);                }            }            return _list;        }

        public List<TagsHistoryViewModel> GetAllTagHistoryList(DateTime currentdate, string type)        {            List<TagsHistoryViewModel> _list = new List<TagsHistoryViewModel>();

            int Day = currentdate.Day;            int Month = currentdate.Month;            int Year = currentdate.Year;
            try
            {
                using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
                {
                    if (type == "day")
                    {
                        _list = (from tm in db.TagsMaster
                                 join tl in db.TagLabel on tm.FKTagLabelID equals tl.ID
                                 where tm.CreatedDateTime.Day == Day && tm.CreatedDateTime.Month == Month &&
                                 tm.CreatedDateTime.Year == Year
                                 select new TagsHistoryViewModel
                                 {
                                     ID = tm.ID,
                                     FKTagLabelID = tm.FKTagLabelID,
                                     TagValue = tm.TagValue,
                                     Note = tm.Note,
                                     TypeName = tl.LabelName,
                                     CreatedDateTime = tm.CreatedDateTime,
                                     HasColorCode = tl.HasColorCode

                                 }
                             ).OrderByDescending(x => x.CreatedDateTime).ToList();

                        _list.ForEach(l => l.CreatedDateTimeTimestamp = GetNotNullDateTimeValue(l.CreatedDateTime).ToString("MM-dd-yyyy HH:mm"));

                    }
                    else if (type == "month")
                    {
                        DateTime startdayofmonth = new DateTime(Year, Month, 1);
                        DateTime enddayofmonth = currentdate.AddDays(1);

                        int totaldays = System.DateTime.DaysInMonth(Year, Month);

                        SqlParameter[] Params =
                       {
                                    new SqlParameter("@STARTDATE",startdayofmonth.ToString("yyyy-MM-dd")),
                                    new SqlParameter("@ENDDATE",enddayofmonth.ToString("yyyy-MM-dd")),
                                    new SqlParameter("@NOOFDAYS",totaldays)
                                    //new SqlParameter("@USERID",UserID)

                                };
                        DataSet ds = SqlHelper.ExecuteDataset(GlobalConstants.DBConn(), CommandType.StoredProcedure, "USP_GETTAGHISTORY_WEEKMONTHWISE", Params);
                        if (ds != null && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                        {
                            _list = (from DataRow row in ds.Tables[0].Rows

                                     select new TagsHistoryViewModel()
                                     {
                                         CreatedDateTimeTimestamp = GetNotNullDateTimeValue(row["CREATEDDATE"]).ToString("MM-dd-yyyy HH:mm"),
                                         ID = GetIntegerValue(row["ID"]),
                                         TypeName = GetStringValue(row["LABELNAME"]),
                                         TagValue = GetIntegerValue(row["TAGVALUE"]),
                                         Note = GetStringValue(row["NOTE"]),
                                         HasColorCode = GetStringValue(row["HASCOLORCODE"])

                                     }).OrderByDescending(x => x.CreatedDateTime).ToList();

                        }


                    }
                    else if (type == "week")
                    {

                        int dayofweek = Convert.ToInt32(currentdate.DayOfWeek);

                        DateTime endday = currentdate.AddDays(1);


                        DateTime startdayofweek = currentdate.AddDays(-dayofweek);

                        SqlParameter[] Params =
                        {
                                    new SqlParameter("@STARTDATE",startdayofweek.ToString("yyyy-MM-dd")),
                                    new SqlParameter("@ENDDATE",endday.ToString("yyyy-MM-dd")),
                                     new SqlParameter("@NOOFDAYS",7)
                                    //new SqlParameter("@USERID",UserID)

                                };
                        DataSet ds = SqlHelper.ExecuteDataset(GlobalConstants.DBConn(), CommandType.StoredProcedure, "USP_GETTAGHISTORY_WEEKMONTHWISE", Params);
                        if (ds != null && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                        {
                            _list = (from DataRow row in ds.Tables[0].Rows

                                     select new TagsHistoryViewModel()
                                     {
                                         CreatedDateTimeTimestamp = GetNotNullDateTimeValue(row["CREATEDDATE"]).ToString("MM-dd-yyyy HH:mm"),
                                         ID = GetIntegerValue(row["ID"]),
                                         TypeName = GetStringValue(row["LABELNAME"]),
                                         TagValue = GetIntegerValue(row["TAGVALUE"]),
                                         Note = GetStringValue(row["NOTE"]),
                                         HasColorCode = GetStringValue(row["HASCOLORCODE"])

                                     }).OrderByDescending(x => x.CreatedDateTime).ToList();

                        }


                    }
                }
            }
            catch (Exception Ex)
            {
                WriteLog("GetTagHistory List Problems", Ex.Message);
            }


            return _list;        }


        #region [ :: DROPDOWNLIST ]

        public List<SelectListItem> GetDDLTagLabelType(int UserID)
        {
            List<SelectListItem> _list = new List<SelectListItem>();

            try
            {
                using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
                {
                    _list = (from taglabel in db.TagLabel
                             where taglabel.UserID == UserID
                             || taglabel.UserID <= 0
                             select new SelectListItem { Text = taglabel.LabelName, Value = taglabel.ID.ToString() }
                             ).ToList();
                }
            }
            catch (Exception Ex)
            {
                WriteLog("SDGApp.Models.TagHistoryModel - GetDDLTagLabelType", Ex.Message);
            }

            return _list;
        }

        #endregion

        public bool AddNewTag(TagsHistoryViewModel model)
        {
            bool Result = false;

            int UserID = GetLoggedInUserInfo().UserID;

            try
            {
                using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
                {
                    var entity = new SDGAppDB.POCO.TagsMaster();
                    entity.FKTagLabelID = model.FKTagLabelID;
                    entity.TagValue = model.hdnTagValue;
                    entity.Note = model.Note;
                    entity.FKUserID = UserID;
                    entity.CreatedDateTime = model.CreatedDateTime;

                    db.TagsMaster.Add(entity);
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
                WriteLog("SDGApp.Models.TagHistoryModel - AddNewTag", Ex.Message);
            }


            return Result;
        }

        public TagsHistoryViewModel GetTagsHistoryDetailById(int Tagid)
        {
            TagsHistoryViewModel model = new TagsHistoryViewModel();
            try
            {
                using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
                {
                    var entity = (from tag in db.TagsMaster where tag.ID == Tagid select new { tag }).FirstOrDefault();
                    if (entity != null)
                    {
                        model.ID = entity.tag.ID;
                        model.FKTagLabelID = entity.tag.FKTagLabelID;
                        model.TagValue = entity.tag.TagValue;
                        model.Note = entity.tag.Note;
                        model.FKUserID = entity.tag.FKUserID;
                        model.CreatedDateTime = entity.tag.CreatedDateTime;
                    }
                }
            }
            catch (Exception Ex)
            {
                WriteLog("SDGApp.Models.TagLabesModel - GetTagsHistoryDetailById", Ex.Message);
            }
            return model;
        }

        public bool UpdateTagByID(TagsHistoryViewModel tvm)
        {
            bool Result = false;
            try
            {
                using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
                {
                    var entity = db.TagsMaster.Find(tvm.ID);

                    if (entity != null && entity.ID > 0)
                    {
                        entity.FKTagLabelID = tvm.FKTagLabelID;
                        entity.TagValue = tvm.hdnTagValue;
                        entity.Note = tvm.Note;
                        entity.CreatedDateTime = tvm.CreatedDateTime;

                        db.Entry(entity).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                        Result = true;
                    }
                }
            }
            catch (Exception Ex)
            {
                WriteLog("SDGApp.Models.TagHistoryModel - UpdateTagByID", Ex.Message);
            }
            return Result;

        }

        public List<TagsHistoryViewModel> GetAllTagRecordList(int UserID, int PageNumber, int PageSize, DateTime? FromDate = null, DateTime? ToDate = null, int[] IDs = null)
        {
            List<TagsHistoryViewModel> lstchunk = new List<TagsHistoryViewModel>();
            List<TagsHistoryViewModel> lst = new List<TagsHistoryViewModel>();
            try
            {
                using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
                {
                    if (FromDate != null && ToDate != null)
                    {
                        DateTime newdate = ToDate.Value.AddDays(1);

                        lst = (from tags in db.TagsMaster
                               join taglabel in db.TagLabel on tags.FKTagLabelID equals taglabel.ID
                               where tags.FKUserID == UserID
                                && (tags.CreatedDateTime >= FromDate && tags.CreatedDateTime <= newdate)
                               select new TagsHistoryViewModel
                               {
                                   ID = tags.ID,
                                   FKUserID = tags.FKUserID,
                                   FKTagLabelID = tags.FKTagLabelID,
                                   TypeName = taglabel.LabelName,
                                   TagValue = tags.TagValue,
                                   Note = tags.Note,
                                   CreatedDateTime = tags.CreatedDateTime,
                                   PageSize = PageSize,
                                   PageNumber = PageNumber,
                               }).ToList();
                    }
                    else
                    {
                        lst = (from tags in db.TagsMaster
                               join taglabel in db.TagLabel on tags.FKTagLabelID equals taglabel.ID
                               where tags.FKUserID == UserID
                               select new TagsHistoryViewModel
                               {
                                   ID = tags.ID,
                                   FKUserID = tags.FKUserID,
                                   FKTagLabelID = tags.FKTagLabelID,
                                   TypeName = taglabel.LabelName,
                                   TagValue = tags.TagValue,
                                   Note = tags.Note,
                                   CreatedDateTime = tags.CreatedDateTime,
                                   PageSize = PageSize,
                                   PageNumber = PageNumber,
                               }).ToList();
                    }

                    

                    if (lst.Count > 0)
                    {
                        if (IDs != null && IDs.Length > 0)
                        {
                            foreach (var item in IDs)
                            {
                                lst.Remove(lst.Single(s => s.ID == item)); // Remove IDs from List 
                            }

                        }

                        lstchunk = lst.OrderByDescending(q => q.ID).Skip(SkipRecords(PageSize, PageNumber)).Take(PageSize).ToList();
                        lstchunk.ForEach(l => l.TotalRecords = lst.Count());

                        lstchunk.ForEach(l => l.CreatedDateTimeTimestamp = l.CreatedDateTime.ToString("yyyy-MM-dd HH:mm:ss"));


                    }

                }

            }
            catch (Exception Ex)
            {
                WriteLog("SDGApp.Models.TagHistoryModel - GetAllTagRecordList", Ex.Message);
            }
            return lstchunk;
        }
    }
}