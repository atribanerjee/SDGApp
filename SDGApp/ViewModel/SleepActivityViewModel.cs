using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SDGApp.ViewModel
{
    public class SleepActivityViewModel
    {
        public int ID { get; set; }

        [DisplayName("User Id")]
        [Required(ErrorMessage = "UserId is required")]
        public int userid { get; set; }
        public String username { get; set; }
        public DateTime sleepDate { get; set; }
        public int? sleepTotalTime { get; set; }
        public decimal sleepDeepTime { get; set; }
        public decimal sleepLightTime { get; set; }
        public decimal sleepStayupTime { get; set; }
        public int? sleepWalkingNumber { get; set; }

        public String CreateDateTimeStamp { get; set; }

        public List<clsSleepData> SleepData { get; set; }

        public Int32 TotalRecords { get; set; }
    }

    public class clsSleepData
    {
        public int sleep_type { get; set; }
        public string startTime { get; set; }
    }
}