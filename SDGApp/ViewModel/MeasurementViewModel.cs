using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SDGApp.ViewModel
{
    public class MeasurementViewModel
    {
        public Int64 ID { get; set; }
        public int UserID { get; set; }
        public string DeviceID { get; set; }
        public string DeviceVersionNo { get; set; }
        public string DeviceFirmwareVersionNo { get; set; }
        public String HR { get; set; }
        public String HRV { get; set; }
        public String SBP { get; set; }

        public String BP { get; set; }
        public String DBP { get; set; }
        public int? HRV1 { get; set; }
        public String HRV2 { get; set; }

        public String Calories { get; set; }
        public String EcgValues { get; set; }
        public String PpgValues { get; set; }
        public String ECGElapsedTime { get; set; }
        public String PPGElapsedTime { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public double TotalMillisecond { get; set; }
        public long AVGSBP { get; set; }
        public double AVGDBP { get; set; }
        public double AVGHR { get; set; }

        public double Difference { get; set; }

    }
}