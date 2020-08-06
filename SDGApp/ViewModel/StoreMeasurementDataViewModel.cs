using System;
using System.Collections.Generic;

namespace SDGApp.ViewModel
{
    public class StoreMeasurementDataViewModel
    {
        //public List<double> TimeElapsed { get; set; }
        public List<double> ECGElapsedTime { get; set; }
        public List<double> PPGElapsedTime { get; set; }
        public List<double> EcgValues { get; set; }
        public List<double> PpgValues { get; set; }
        public string SBP { get; set; }
        public string DBP { get; set; }
        public string HR { get; set; }
        public string DeviceID { get; set; }
        public string DeviceVersionNo { get; set; }
        public string DeviceFirmwareVersionNo { get; set; }
        public int UserID { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public string Calories { get; set; }
        public int? HRVDevice { get; set; }
    }

    //public class Demographics
    //{
    //    public int age { get; set; }
    //    public int height { get; set; }
    //    public int weight { get; set; }
    //    public string gender { get; set; }
    //}

    //public class Datum
    //{
    //    public string device_id { get; set; }
    //    public string schema { get; set; }
    //    public string timestamp { get; set; }
    //    public string username { get; set; }
    //    public Demographics demographics { get; set; }
    //    public int sys_manual { get; set; }
    //    public int dias_manual { get; set; }
    //    public int hr_manual { get; set; }
    //    public int o2_manual { get; set; }
    //    public int bg_manual { get; set; }
    //    public List<double> raw_times { get; set; }
    //    public List<double> raw_ecg { get; set; }
    //    public List<double> raw_ppg { get; set; }
    //}

    //public class RootObject
    //{
    //    public string model_id { get; set; }
    //    public List<Datum> data { get; set; }
    //}


    public class Demographic
    {
        public int age { get; set; }
        public string gender { get; set; }
        public int height { get; set; }
        public int weight { get; set; }
    }

    public class Datum
    {
        public Int64 id { get; set; }
        public string bg_manual { get; set; }
        public Demographic demographics { get; set; }
        public string device_id { get; set; }
        public string device_type { get; set; }
        public string dias_device { get; set; }
        public string dias_healthgauge { get; set; }
        public string dias_manual { get; set; }
        public string hr_device { get; set; }
        public string hr_manual { get; set; }
        public int? hrv_device { get; set; }
        public string o2_manual { get; set; }
        public List<string> raw_ecg { get; set; }
        public List<string> raw_ppg { get; set; }
        //public List<double> raw_times { get; set; }
        public List<string> ecg_elapsed_time { get; set; }
        public List<string> ppg_elapsed_time { get; set; }

        public String raw_ecg_array { get; set; }
        public String raw_ppg_array { get; set; }
        //public String raw_times_array { get; set; }
        public String ecg_elapsed_time_array { get; set; }
        public String ppg_elapsed_time_array { get; set; }
        public string schema { get; set; }
        public string sys_device { get; set; }
        public string sys_healthgauge { get; set; }
        public string sys_manual { get; set; }
        public string timestamp { get; set; }
        public string username { get; set; }
        public string model_id { get; set; }
        public int userID { get; set; }
    }

    public class RootObject
    {
        public int ID { get; set; }
        public DateTime? birthdate { get; set; }
        public List<Datum> data { get; set; }
    }



}