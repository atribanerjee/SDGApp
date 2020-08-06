using SDGApp.ViewModel;
using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using SDGAppDB;
using SDGAppDB.POCO;
using System.Globalization;
using System.Web;
using System.Data.Entity.SqlServer;
using System.Net;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using System.Data.SqlClient;

namespace SDGApp.Models
{
    public class SleepModel : UserModel
    {

        public List<SleepActivityViewModel> GetSleepActivity(DateTime currentdate, string type, int UserID)
        {

            List<SleepActivityViewModel> _list = new List<SleepActivityViewModel>();

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
                            var entitySleep = (from s in db.SleepActivity
                                               where s.FKUserID == UserID && s.CreatedDateTime.Day == Day
                                               && s.CreatedDateTime.Month == Month && s.CreatedDateTime.Year == Year
                                               select new { s }).FirstOrDefault();


                            if (entitySleep != null && entitySleep.s.ID > 0)
                            {
                                var Entityslp = (db.SleepDataDtls.Where(x => x.FKSleepActivityID == entitySleep.s.ID).ToList());

                                List<clsSleepData> _sleepData = new List<clsSleepData>();

                                if (Entityslp != null)
                                {
                                    foreach (var item in Entityslp)
                                    {
                                        _sleepData.Add(new clsSleepData { sleep_type = item.FKSleepTypeID, startTime = item.StartTime });
                                    }
                                }

                                if (_sleepData != null)
                                {
                                    _list.Add(new SleepActivityViewModel
                                    {
                                        ID = entitySleep.s.ID,
                                        userid = entitySleep.s.FKUserID,


                                        sleepDate = entitySleep.s.CreatedDateTime,
                                        CreateDateTimeStamp = GetNotNullDateTimeValue(entitySleep.s.CreatedDateTime).ToString("MM-dd-yyyy  HH:mm"),
                                        sleepTotalTime = entitySleep.s.SleepTotalTime,
                                        sleepDeepTime = entitySleep.s.DeepSleepHour,
                                        sleepLightTime = entitySleep.s.LightSleepHour,
                                        sleepStayupTime = entitySleep.s.StayUPHour,
                                        sleepWalkingNumber = entitySleep.s.SleepWalkingNumber,
                                        SleepData = _sleepData // list
                                    });
                                }
                            }

                        }
                        else if (type == "month")
                        {
                            _list = (from s in db.SleepActivity
                                     where s.FKUserID == UserID && s.CreatedDateTime.Month == Month

                                     select new SleepActivityViewModel
                                     {
                                         sleepDate = s.CreatedDateTime,
                                         sleepDeepTime = s.DeepSleepHour,
                                         sleepLightTime = s.LightSleepHour,
                                         sleepStayupTime = s.StayUPHour,
                                         sleepTotalTime = s.SleepTotalTime
                                     }
                                 ).ToList();

                            _list.ForEach(l => l.CreateDateTimeStamp = GetNotNullDateTimeValue(l.sleepDate).ToString("MM-dd-yyyy  HH:mm"));

                        }
                        else if (type == "week")
                        {

                            int dayofweek = Convert.ToInt32(currentdate.DayOfWeek);

                            DateTime endday = currentdate.AddDays(1);

                            DateTime startdayofweek = currentdate.AddDays(-dayofweek);

                            _list = (from s in db.SleepActivity
                                     where s.FKUserID == UserID && s.CreatedDateTime >= startdayofweek && s.CreatedDateTime <= endday
                                     select new SleepActivityViewModel
                                     {
                                         sleepDate = s.CreatedDateTime,
                                         sleepDeepTime = s.DeepSleepHour,
                                         sleepLightTime = s.LightSleepHour,
                                         sleepStayupTime = s.StayUPHour,
                                         sleepTotalTime = s.SleepTotalTime
                                     }
                                  ).ToList();

                            _list.ForEach(l => l.CreateDateTimeStamp = GetNotNullDateTimeValue(l.sleepDate).ToString("MM-dd-yyyy  HH:mm"));
                        }
                    }
                }
                catch (Exception Ex)
                {
                    WriteLog("SDGApp.Models.SleepModel - GetSleepActivity", Ex.Message);
                }
            }

            return _list;
        }

        public List<SleepActivityViewModel> GetSleepActivityList(int UserID, int PageNumber, int PageSize, DateTime? FromDate = null, DateTime? ToDate = null, int[] IDs = null)
        {
            List<SleepActivityViewModel> lstchunk = new List<SleepActivityViewModel>();
            List<SleepActivityViewModel> lst = new List<SleepActivityViewModel>();
            try
            {
                using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
                {
                    if (FromDate != null && ToDate != null)
                    {
                        DateTime newdate = ToDate.Value.AddDays(1);

                        lst = (from s in db.SleepActivity
                               where s.FKUserID == UserID
                               && (s.CreatedDateTime >= FromDate && s.CreatedDateTime <= newdate)
                               select new SleepActivityViewModel
                               {
                                   ID = s.ID,
                                   userid = s.FKUserID,
                                   sleepDeepTime = s.DeepSleepHour,
                                   sleepLightTime = s.LightSleepHour,
                                   sleepStayupTime = s.StayUPHour,
                                   sleepTotalTime = s.SleepTotalTime,
                                   sleepWalkingNumber = s.SleepWalkingNumber,
                                   sleepDate = s.CreatedDateTime,
                                   SleepData = (from sd in db.SleepDataDtls
                                                where sd.FKSleepActivityID == s.ID
                                                select new clsSleepData
                                                {
                                                    sleep_type = sd.FKSleepTypeID,
                                                    startTime = sd.StartTime
                                                }).ToList()
                               }).ToList();
                    }
                    else
                    {

                        lst = (from s in db.SleepActivity
                               where s.FKUserID == UserID
                               select new SleepActivityViewModel
                               {
                                   ID = s.ID,
                                   userid = s.FKUserID,
                                   sleepDeepTime = s.DeepSleepHour,
                                   sleepLightTime = s.LightSleepHour,
                                   sleepStayupTime = s.StayUPHour,
                                   sleepTotalTime = s.SleepTotalTime,
                                   sleepWalkingNumber = s.SleepWalkingNumber,
                                   sleepDate = s.CreatedDateTime,
                                   SleepData = (from sd in db.SleepDataDtls
                                                where sd.FKSleepActivityID == s.ID
                                                select new clsSleepData
                                                {
                                                    sleep_type = sd.FKSleepTypeID,
                                                    startTime = sd.StartTime
                                                }).ToList()
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

                        lstchunk.ForEach(l => l.CreateDateTimeStamp = l.sleepDate.ToString("yyyy-MM-dd HH:mm:ss"));
                    }

                }

            }
            catch (Exception Ex)
            {
                WriteLog("SDGApp.Models.TagHistoryModel - GetSleepActivityList", Ex.Message);
            }
            return lstchunk;
        }
    }
}