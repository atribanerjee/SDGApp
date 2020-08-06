using SDGAppDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SDGApp.Models
{
    public class ServiceModel : BaseModel
    {
        //public void SaveValueFromPhonDevice( int[] elapsetime,
        //    double[] ECGValues, double[] PPGValue, string systolicBloodPresure,
        //    string diastolicBloosPresure, String heartrate, String DeviceID,
        //    String DeviceversionNumber, String firmwareversionnumber, int fkUserID)
        //{
        //    try
        //    {
        //        string Elapesed = "";
        //        string Ecg = "";
        //        String Ppg = "";
        //        if (elapsetime.Length > 0)
        //        {
        //            for (var i = 0; i < elapsetime.Length; i++)
        //            {
        //                Elapesed = Elapesed + elapsetime[i] + ",";
        //            }
        //        }

        //        if (ECGValues.Length > 0)
        //        {
        //            for (var i = 0; i < ECGValues.Length; i++)
        //            {
        //                Ecg = Ecg + ECGValues[i] + ",";
        //            }
        //        }
        //        if (PPGValue.Length > 0)
        //        {
        //            for (var i = 0; i < PPGValue.Length; i++)
        //            {
        //                Ppg = Ppg + PPGValue[i] + ",";
        //            }

        //        }


        //        using (SDGAppDBContext db = new SDGAppDBContext(GlobalConstants.DBConn()))
        //        {
        //            var entity = new SDGAppDB.POCO.EPBProfile();
        //            entity.CreatedDateTime = DateTime.Now;
        //            entity.ECGElapsedTime = Elapesed;
        //            entity.ECGValues = Ecg;
        //            entity.PPGValues = Ppg;
        //            entity.BPSystolic = systolicBloodPresure;
        //            entity.BPDiastolic = diastolicBloosPresure;
        //            entity.HeartRate = heartrate;
        //            entity.DeviceID = DeviceID;
        //            entity.DeviceVersionNo = DeviceversionNumber;
        //            entity.DeviceFirmwareVersionNo = firmwareversionnumber;
        //            entity.FKUserID = fkUserID;
        //            db.EPBProfile.Add(entity);
        //            db.SaveChanges();
        //        }

        //    }
        //    catch (Exception Ex)
        //    {
        //        WriteLog("SDGApp.Models.ServiceModel - SaveValueFromPhonDevice", Ex.Message);
        //    }
        //}
    }
}