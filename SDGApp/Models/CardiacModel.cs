using SDGApp.ViewModel;
using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using SDGAppDB;
using SDGAppDB.POCO;
using System.Web;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using System.Data.SqlClient;


namespace SDGApp.Models
{
    public class CardiacModel : UserModel
    {
        public List<CardiacViewModel> GetCardiacHistoryDtls(DateTime currentdate, string type, int UserID)
        {
            List<CardiacViewModel> _list = new List<CardiacViewModel>();

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
                            int avgcount = 0;
                            int totalSBP = 0;
                            int totalDBP = 0;
                            int totalHR = 0;

                            var mesurmententity = (from um in db.UserMeasurement
                                                   where um.FKUserId == UserID
                                                   && (um.CreatedDateTime.Day == Day && um.CreatedDateTime.Month == Month && um.CreatedDateTime.Year == Year)
                                                   orderby um.ID descending
                                                   select um).ToList();

                            if (mesurmententity != null && mesurmententity.Count > 0)
                            {
                                CardiacViewModel cardiacViewModel = new CardiacViewModel();

                                cardiacViewModel.CreatedDateTimeStamp = currentdate.ToString("MM-dd-yyyy");
                                cardiacViewModel.CreatedDateTime = currentdate;

                                foreach (var item in mesurmententity)
                                {

                                    if (!String.IsNullOrEmpty(item.FileName))
                                    {
                                        var splitjsonfilename = item.FileName.Split('_');

                                        String filePath = "~/Content/Measurement/" + UserID + "/" + splitjsonfilename[2] + "/" + item.FileName;

                                        string rootdir = System.Web.HttpContext.Current.Server.MapPath(filePath);

                                        if (System.IO.File.Exists(rootdir))
                                        {
                                            string Jsonfileread = System.IO.File.ReadAllText(rootdir);

                                            if (!String.IsNullOrEmpty(Jsonfileread))
                                            {
                                                avgcount++;

                                                RootObject model = new RootObject();

                                                model = JsonConvert.DeserializeObject<RootObject>(Jsonfileread);

                                                if (model != null)
                                                {
                                                    totalSBP = totalSBP + GetIntegerValue(model.data.FirstOrDefault().sys_device);
                                                    totalDBP = totalDBP + GetIntegerValue(model.data.FirstOrDefault().dias_device);
                                                    totalHR = totalHR + GetIntegerValue(model.data.FirstOrDefault().hr_device);

                                                    cardiacViewModel.AVGSBP = totalSBP / avgcount;
                                                    cardiacViewModel.AVGDBP = totalDBP / avgcount;
                                                    cardiacViewModel.AVGHR = totalHR / avgcount;
                                                    cardiacViewModel.HRV = "";

                                                }

                                            }

                                        }
                                    }

                                }//end foreach

                                _list.Add(cardiacViewModel);
                                
                            }
                        }

                        else if (type == "month")
                        {


                            var _lstentity = (from um in db.UserMeasurement
                                                   where um.FKUserId == UserID
                                                   && (um.CreatedDateTime.Month == currentdate.Month)
                                                   //orderby um.CreatedDateTime descending
                                                   select um).GroupBy(m=> System.Data.Entity.DbFunctions.TruncateTime(m.CreatedDateTime)).ToList();

                            foreach (var lstmesurmentdtls in _lstentity)
                            {

                                CardiacViewModel cardiacViewModel = new CardiacViewModel();

                                int avgcount = 0;
                                int totalSBP = 0;
                                int totalDBP = 0;
                                int totalHR = 0;

                                foreach (var item in lstmesurmentdtls)
                                {

                                    if (!String.IsNullOrEmpty(item.FileName))
                                    {
                                        var splitjsonfilename = item.FileName.Split('_');

                                        String filePath = "~/Content/Measurement/" + UserID + "/" + splitjsonfilename[2] + "/" + item.FileName;

                                        string rootdir = System.Web.HttpContext.Current.Server.MapPath(filePath);

                                        if (System.IO.File.Exists(rootdir))
                                        {
                                            string Jsonfileread = System.IO.File.ReadAllText(rootdir);

                                            if (!String.IsNullOrEmpty(Jsonfileread))
                                            {
                                                avgcount++;

                                                RootObject model = new RootObject();

                                                model = JsonConvert.DeserializeObject<RootObject>(Jsonfileread);

                                                if (model != null)
                                                {

                                                    totalSBP = totalSBP + GetIntegerValue(model.data.FirstOrDefault().sys_device);
                                                    totalDBP = totalDBP + GetIntegerValue(model.data.FirstOrDefault().dias_device);
                                                    totalHR = totalHR + GetIntegerValue(model.data.FirstOrDefault().hr_device);

                                                    cardiacViewModel.AVGSBP = totalSBP / avgcount;
                                                    cardiacViewModel.AVGDBP = totalDBP / avgcount;
                                                    cardiacViewModel.AVGHR = totalHR / avgcount;
                                                    cardiacViewModel.HRV = "";

                                                }


                                            }

                                        }

                                        cardiacViewModel.CreatedDateTimeStamp = item.CreatedDateTime.ToString("MM-dd-yyyy");
                                        cardiacViewModel.CreatedDateTime = item.CreatedDateTime;
                                    }

                                }

                               if(cardiacViewModel.CreatedDateTime != null)
                                {
                                   
                                    _list.Add(cardiacViewModel);
                                }
                                
                            }
                          

                        }

                        else if (type == "week")
                        {
                            int dayofweek = Convert.ToInt32(currentdate.DayOfWeek);
                            DateTime endday = currentdate.AddDays(1);
                            DateTime startdayofweek = currentdate.AddDays(-dayofweek);


                            var _lstentity = (from um in db.UserMeasurement
                                              where um.FKUserId == UserID
                                              && (um.CreatedDateTime >= startdayofweek && um.CreatedDateTime<=endday)
                                              select um).GroupBy(m => System.Data.Entity.DbFunctions.TruncateTime(m.CreatedDateTime)).ToList();

                            foreach (var lstmesurmentdtls in _lstentity)
                            {

                                CardiacViewModel cardiacViewModel = new CardiacViewModel();

                                int avgcount = 0;
                                int totalSBP = 0;
                                int totalDBP = 0;
                                int totalHR = 0;


                                foreach (var item in lstmesurmentdtls)
                                {

                                    if (!String.IsNullOrEmpty(item.FileName))
                                    {
                                        var splitjsonfilename = item.FileName.Split('_');

                                        String filePath = "~/Content/Measurement/" + UserID + "/" + splitjsonfilename[2] + "/" + item.FileName;

                                        string rootdir = System.Web.HttpContext.Current.Server.MapPath(filePath);

                                        if (System.IO.File.Exists(rootdir))
                                        {
                                            string Jsonfileread = System.IO.File.ReadAllText(rootdir);

                                            if (!String.IsNullOrEmpty(Jsonfileread))
                                            {
                                                avgcount++;

                                                RootObject model = new RootObject();

                                                model = JsonConvert.DeserializeObject<RootObject>(Jsonfileread);

                                                if (model != null)
                                                {

                                                    totalSBP = totalSBP + GetIntegerValue(model.data.FirstOrDefault().sys_device);
                                                    totalDBP = totalDBP + GetIntegerValue(model.data.FirstOrDefault().dias_device);
                                                    totalHR = totalHR + GetIntegerValue(model.data.FirstOrDefault().hr_device);

                                                    cardiacViewModel.AVGSBP = totalSBP / avgcount;
                                                    cardiacViewModel.AVGDBP = totalDBP / avgcount;
                                                    cardiacViewModel.AVGHR = totalHR / avgcount;
                                                    cardiacViewModel.HRV = "";


                                                    cardiacViewModel.CreatedDateTimeStamp = item.CreatedDateTime.ToString("MM-dd-yyyy");
                                                    cardiacViewModel.CreatedDateTime = item.CreatedDateTime;
                                                }
                                            }

                                        }

                                        cardiacViewModel.CreatedDateTimeStamp = item.CreatedDateTime.ToString("MM-dd-yyyy");
                                        cardiacViewModel.CreatedDateTime = item.CreatedDateTime;
                                    }
                                    
                                }

                                if (GetNotNullDateTimeValue(cardiacViewModel.CreatedDateTime) != null)
                                {
                                    _list.Add(cardiacViewModel);
                                }

                            }
                        }
                    }
                }
                catch (Exception Ex)
                {
                    WriteLog("SDGApp.Models.UserModel - GetCardiacHistoryDtls", Ex.Message);
                }
            }

            return _list;
        }

        public DateTime UnixTimeToDateTime(object unixtime)
        {
            System.DateTime dtDateTime;
            try
            {
                if (unixtime != null)
                {
                    BaseModel BM = new BaseModel();
                    long unixtime1 = BM.GetLontValue(unixtime);

                    dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
                    dtDateTime = dtDateTime.AddMilliseconds(unixtime1).ToLocalTime();
                }
                else
                {
                    dtDateTime = DateTime.Now;
                }
            }
            catch (Exception Ex)
            {

                dtDateTime = DateTime.Now;
                WriteLog("SDGApp.Models.DeviceServicesModel - UnixTimeToDateTime", Ex.Message);
            }


            return dtDateTime;
        }
    }
}