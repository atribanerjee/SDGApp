using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SDGApp.ViewModel
{
    public class DeviceInformationViewModel
    {
        public int ID { get; set; }
        public int UserID { get; set; }
        public String DeviceName { get; set; }
        public String DeviceAddress { get; set; }
        public String BlockStatus { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public String CreateDateTimeStamp { get; set; }

    }
}