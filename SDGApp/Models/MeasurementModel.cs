using Newtonsoft.Json;
using SDGApp.ViewModel;
using SDGAppDB;
using SDGAppDB.POCO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace SDGApp.Models
{
    public class MeasurementModel : UserModel
    {
        public MeasurementViewModel GetLatestECGPPGMeasurementFromFile(int UserID)
        {
            MeasurementViewModel MVM = new MeasurementViewModel();
            RootObject model = new RootObject();

            try
            {
                using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
                {
                    var jsonfilename = (from um in db.UserMeasurement
                                        where um.FKUserId == UserID
                                        orderby um.ID descending
                                        select um.FileName).FirstOrDefault();

                    if (!string.IsNullOrEmpty(jsonfilename))
                    {
                        var splitjsonfilename = jsonfilename.Split('_');

                        String filePath = "~/Content/Measurement/" + UserID + "/" + splitjsonfilename[2] + "/" + jsonfilename;

                        string rootdir = System.Web.HttpContext.Current.Server.MapPath(filePath);

                        if (System.IO.File.Exists(rootdir))
                        {
                            string Jsonfileread = System.IO.File.ReadAllText(rootdir);

                            if (!String.IsNullOrEmpty(Jsonfileread))
                            {

                                model = JsonConvert.DeserializeObject<RootObject>(Jsonfileread);

                                if (model != null && model.data.FirstOrDefault().raw_ecg.Count > 0 && model.data.FirstOrDefault().raw_ppg.Count > 0)
                                {
                                    MVM.UserID = UserID;
                                    MVM.EcgValues = string.Join(",", model.data.FirstOrDefault().raw_ecg);
                                    MVM.PpgValues = string.Join(",", model.data.FirstOrDefault().raw_ppg);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception Ex)
            {
                WriteLog("SDGApp.Models.MeasurementModel - GetLatestECGPPGMeasurementFromFile", Ex.Message);
            }
            return MVM;
        }

        public MeasurementViewModel GetMeasurmentByFileID(int FileID,String Type)
        {
            MeasurementViewModel mesurmentModel = new MeasurementViewModel();
            try
            {
                using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
                {
                    if (FileID > 0)
                    {
                        var mesurmententity = (from um in db.UserMeasurement
                                               where um.ID == FileID
                                               select um).FirstOrDefault();

                        if (!string.IsNullOrEmpty(mesurmententity.FileName))
                        {
                            var splitjsonfilename = mesurmententity.FileName.Split('_');

                            String filePath = "~/Content/Measurement/" + mesurmententity.FKUserId + "/" + splitjsonfilename[2] + "/" + mesurmententity.FileName;

                            string rootdir = System.Web.HttpContext.Current.Server.MapPath(filePath);

                            if (System.IO.File.Exists(rootdir))
                            {
                                string Jsonfileread = System.IO.File.ReadAllText(rootdir);

                                if (!String.IsNullOrEmpty(Jsonfileread))
                                {
                                    RootObject model = new RootObject();
                                    model = JsonConvert.DeserializeObject<RootObject>(Jsonfileread);

                                    if (model != null && model.data.FirstOrDefault().raw_ecg.Count > 0 && model.data.FirstOrDefault().raw_ppg.Count > 0)
                                    {
                                        mesurmentModel.UserID = mesurmententity.FKUserId;
                                        if (Type == "ecg")
                                        {
                                            mesurmentModel.EcgValues = string.Join(",", model.data.FirstOrDefault().raw_ecg);
                                        }
                                        else if (Type == "ppg")
                                        {
                                            mesurmentModel.PpgValues = string.Join(",", model.data.FirstOrDefault().raw_ppg);
                                        }
                                        
                                        
                                    }
                                }
                            }
                        }
                    }

                }
            }
            catch (Exception Ex)
            {
                WriteLog("SDGApp.Models.MeasurementModel - GetMeasurmentByFileID", Ex.Message);
            }
            return mesurmentModel;
        }

        public List<MeasurementViewModel> GetHistoryList(int UserID, DateTime currentdate, string type)
        {
            List<MeasurementViewModel> _list = new List<MeasurementViewModel>();

            int Day = currentdate.Day;
            int Month = currentdate.Month;
            int Year = currentdate.Year;

            List<UserMeasurement> lstmesurmententity = new List<UserMeasurement>();

            if (UserID > 0)
            {
                try
                {
                    using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
                    {
                        if (type == "day")
                        {
                            lstmesurmententity = (from um in db.UserMeasurement
                                                  where um.FKUserId == UserID
                                                  && (um.CreatedDateTime.Day == Day && um.CreatedDateTime.Month == Month && um.CreatedDateTime.Year == Year)
                                                  orderby um.CreatedDateTime descending
                                                  select um).ToList();

                        }

                        else if (type == "month")
                        {

                            lstmesurmententity = (from um in db.UserMeasurement
                                                  where um.FKUserId == UserID
                                                  && (um.CreatedDateTime.Month == Month)
                                                  orderby um.CreatedDateTime descending
                                                  select um).ToList();

                        }

                        else if (type == "week")
                        {

                            int dayofweek = Convert.ToInt32(currentdate.DayOfWeek);

                            DateTime endday = currentdate.AddDays(1);


                            DateTime startdayofweek = currentdate.AddDays(-dayofweek);

                            lstmesurmententity = (from um in db.UserMeasurement
                                                  where um.FKUserId == UserID
                                                  && (um.CreatedDateTime >= startdayofweek && um.CreatedDateTime <= endday)
                                                  orderby um.CreatedDateTime descending
                                                  select um).ToList();

                        }

                        if (lstmesurmententity != null && lstmesurmententity.Count > 0)
                        {
                            foreach (var item in lstmesurmententity)
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
                                            RootObject model = new RootObject();
                                            MeasurementViewModel mesurmentmodel = new MeasurementViewModel();

                                            model = JsonConvert.DeserializeObject<RootObject>(Jsonfileread);

                                            if (model != null)
                                            {
                                                mesurmentmodel.ID = item.ID;
                                                mesurmentmodel.UserID = UserID;
                                                mesurmentmodel.CreatedDateTime = item.CreatedDateTime;
                                                mesurmentmodel.SBP = model.data.FirstOrDefault().sys_device;
                                                mesurmentmodel.DBP = model.data.FirstOrDefault().dias_device;
                                                mesurmentmodel.HR = model.data.FirstOrDefault().hr_device;
                                                mesurmentmodel.HRV1 = model.data.FirstOrDefault().hrv_device;
                                                mesurmentmodel.HRV2 = "";

                                                _list.Add(mesurmentmodel);
                                            }

                                        }

                                    }
                                }


                            }//end foreach
                        }//end if
                    }
                }
                catch (Exception Ex)
                {
                    WriteLog("SDGApp.Models.MeasurementModel - GetHistoryList", Ex.Message);
                }
            }

            return _list;
        }

        public MeasurementViewModel GetHRBPHrvByUserID(int UserID)
        {
            MeasurementViewModel measurmentModel = new MeasurementViewModel();
            try
            {
                using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
                {
                    if (UserID > 0)
                    {
                        var mesurmententity = (from um in db.UserMeasurement
                                               where um.FKUserId == UserID
                                               orderby um.ID descending
                                               select um).FirstOrDefault();

                        if (mesurmententity != null)
                        {
                            if (!string.IsNullOrEmpty(mesurmententity.FileName))
                            {
                                var splitjsonfilename = mesurmententity.FileName.Split('_');

                                String filePath = "~/Content/Measurement/" + mesurmententity.FKUserId + "/" + splitjsonfilename[2] + "/" + mesurmententity.FileName;

                                string rootdir = System.Web.HttpContext.Current.Server.MapPath(filePath);

                                if (System.IO.File.Exists(rootdir))
                                {
                                    string Jsonfileread = System.IO.File.ReadAllText(rootdir);

                                    if (!String.IsNullOrEmpty(Jsonfileread))
                                    {
                                        RootObject model = new RootObject();
                                        model = JsonConvert.DeserializeObject<RootObject>(Jsonfileread);

                                        if (model != null)
                                        {
                                            measurmentModel.UserID = mesurmententity.FKUserId;
                                            measurmentModel.HR = model.data.FirstOrDefault().hr_device;
                                            measurmentModel.BP= model.data.FirstOrDefault().sys_device + "/"+ model.data.FirstOrDefault().dias_device;
                                            measurmentModel.HRV =GetStringValue(model.data.FirstOrDefault().hrv_device);
                                            measurmentModel.Calories = "";


                                        }
                                    }
                                }
                            }
                        }
                        
                    }
                    
                }
            }
            catch (Exception Ex)
            {
                WriteLog("SDGApp.Models.MeasurementModel - GetHRBPHrvByUserID", Ex.Message);
            }
            return measurmentModel;
        }

        public MeasurementViewModel GetHRBPHrvByFileID(int FileID)
        {
            MeasurementViewModel measurmentModel = new MeasurementViewModel();
            try
            {
                using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
                {
                    if (FileID > 0)
                    {
                        var mesurmententity = (from um in db.UserMeasurement
                                               where um.ID == FileID
                                               orderby um.ID descending
                                               select um).FirstOrDefault();

                        if (mesurmententity != null)
                        {
                            if (!string.IsNullOrEmpty(mesurmententity.FileName))
                            {
                                var splitjsonfilename = mesurmententity.FileName.Split('_');

                                String filePath = "~/Content/Measurement/" + mesurmententity.FKUserId + "/" + splitjsonfilename[2] + "/" + mesurmententity.FileName;

                                string rootdir = System.Web.HttpContext.Current.Server.MapPath(filePath);

                                if (System.IO.File.Exists(rootdir))
                                {
                                    string Jsonfileread = System.IO.File.ReadAllText(rootdir);

                                    if (!String.IsNullOrEmpty(Jsonfileread))
                                    {
                                        RootObject model = new RootObject();
                                        model = JsonConvert.DeserializeObject<RootObject>(Jsonfileread);

                                        if (model != null)
                                        {
                                            measurmentModel.UserID = mesurmententity.FKUserId;
                                            measurmentModel.HR = model.data.FirstOrDefault().hr_device;
                                            measurmentModel.BP = model.data.FirstOrDefault().sys_device + "/" + model.data.FirstOrDefault().dias_device;
                                            measurmentModel.HRV = GetStringValue(model.data.FirstOrDefault().hrv_device);
                                            measurmentModel.Calories = "";


                                        }
                                    }
                                }
                            }
                        }

                    }

                }
            }
            catch (Exception Ex)
            {
                WriteLog("SDGApp.Models.MeasurementModel - GetHRBPHrvByFileID", Ex.Message);
            }
            return measurmentModel;
        }

        public String GetFilePathByFileID(int FileID, ref String FileName)
        {
            String MeaseurmentFilePath = String.Empty;
            try
            {
                using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
                {
                    var mesurmententity = (from um in db.UserMeasurement
                                           where um.ID == FileID
                                           select um).FirstOrDefault();

                    if(mesurmententity!=null && mesurmententity.ID > 0)
                    {
                        var splitjsonfilename = mesurmententity.FileName.Split('_');

                        String directoryPath = "~/Content/Measurement/" + mesurmententity.FKUserId + "/" + splitjsonfilename[2];

                        string rootfile = Path.Combine(System.Web.HttpContext.Current.Server.MapPath(directoryPath), mesurmententity.FileName);

                        if (System.IO.File.Exists(rootfile))
                        {
                            MeaseurmentFilePath = directoryPath;
                            FileName = mesurmententity.FileName;
                        }
                    }
                }
            }
            catch (Exception Ex)
            {

                WriteLog("SDGApp.Models.MeasurementModel - GetFilePathByFileID", Ex.Message);
            }

            return MeaseurmentFilePath;
        }
    }
}