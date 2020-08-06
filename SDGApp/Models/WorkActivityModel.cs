using System.Data;
using System.Collections.Generic;
using SDGApp.ViewModel;
using System;
using System.Linq;
using SDGAppDB;

namespace SDGApp.Models
{
    public class WorkActivityModel : UserModel
    {
        public List<WorkOutActivityViewModel> GetWorkActivity(DateTime currentdate, string type, int UserID)
        {
            List<WorkOutActivityViewModel> _list = new List<WorkOutActivityViewModel>();

            int Day = currentdate.Day;
            int Month = currentdate.Month;
            int Year = currentdate.Year;

            if (UserID > 0)
            {
                try
                {

                    using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
                    {
                        if (type == "day")
                        {
                            _list = (from w in db.WorkOutActivity
                                     where w.FKUserID == UserID && w.CreatedDateTime.Day == Day && w.CreatedDateTime.Month == Month &&
                                     w.CreatedDateTime.Year == Year
                                     select new WorkOutActivityViewModel
                                     {
                                         CreatedDateTime = w.CreatedDateTime,
                                         Steps = w.Steps,
                                         KCal = w.KCal,
                                         Mileage = w.Mileage,
                                         Completion = w.Completion
                                     }
                                 ).ToList();

                            _list.ForEach(l => l.CreatedDateTimeStamp = GetNotNullDateTimeValue(l.CreatedDateTime).ToString("MM-dd-yyyy"));
                        }
                        else if (type == "month")
                        {
                            _list = (from w in db.WorkOutActivity
                                     where w.FKUserID == UserID && w.CreatedDateTime.Month == Month
                                     select new WorkOutActivityViewModel
                                     {
                                         CreatedDateTime = w.CreatedDateTime,
                                         Steps = w.Steps,
                                         KCal = w.KCal,
                                         Mileage = w.Mileage,
                                         Completion = w.Completion
                                     }
                                 ).ToList();

                            _list.ForEach(l => l.CreatedDateTimeStamp = GetNotNullDateTimeValue(l.CreatedDateTime).ToString("MM-dd-yyyy"));
                        }
                        else if (type == "week")
                        {

                            int dayofweek = Convert.ToInt32(currentdate.DayOfWeek);

                            DateTime endday = currentdate.AddDays(1);


                            DateTime startdayofweek = currentdate.AddDays(-dayofweek);

                            _list = (from w in db.WorkOutActivity
                                     where w.FKUserID == UserID && w.CreatedDateTime >= startdayofweek && w.CreatedDateTime < endday
                                     select new WorkOutActivityViewModel
                                     {
                                         CreatedDateTime = w.CreatedDateTime,
                                         Steps = w.Steps,
                                         KCal = w.KCal,
                                         Mileage = w.Mileage,
                                         Completion = w.Completion
                                     }
                                  ).ToList();

                            _list.ForEach(l => l.CreatedDateTimeStamp = GetNotNullDateTimeValue(l.CreatedDateTime).ToString("MM-dd-yyyy"));
                        }
                    }
                }
                catch (Exception Ex)
                {
                    WriteLog("SDGApp.Models.UserModel - GetWorkActivityForBarChart", Ex.Message);
                }
            }

            return _list;
        }

        public List<WorkOutActivityViewModel> GetWorkActivityList(int UserID, int PageNumber, int PageSize, DateTime? FromDate = null, DateTime? ToDate = null, int[] IDs = null)
        {
            List<WorkOutActivityViewModel> lstchunk = new List<WorkOutActivityViewModel>();
            List<WorkOutActivityViewModel> lst = new List<WorkOutActivityViewModel>();
            try
            {
                using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
                {
                    if (FromDate != null && ToDate != null)
                    {
                        DateTime newdate = ToDate.Value.AddDays(1);


                        lst = (from w in db.WorkOutActivity
                               where w.FKUserID == UserID
                               && (w.CreatedDateTime >= FromDate && w.CreatedDateTime <= newdate)
                               select new WorkOutActivityViewModel
                               {
                                   ID=w.ID,
                                   FKUserID=w.FKUserID,
                                   Steps = w.Steps,
                                   KCal = w.KCal,
                                   Mileage = w.Mileage,
                                   Completion = w.Completion,
                                   CreatedDateTime = w.CreatedDateTime
                               }).ToList();
                    }
                    else
                    {
                        lst = (from w in db.WorkOutActivity
                               where w.FKUserID == UserID
                               select new WorkOutActivityViewModel
                               {
                                   ID = w.ID,
                                   FKUserID = w.FKUserID,
                                   Steps = w.Steps,
                                   KCal = w.KCal,
                                   Mileage = w.Mileage,
                                   Completion = w.Completion,
                                   CreatedDateTime = w.CreatedDateTime
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

                        lstchunk.ForEach(l => l.CreatedDateTimeStamp = l.CreatedDateTime.ToString("yyyy-MM-dd HH:mm:ss"));

                    }

                }

            }
            catch (Exception Ex)
            {
                WriteLog("SDGApp.Models.TagHistoryModel - GetAllTagList", Ex.Message);
            }
            return lstchunk;
        }
    }
}