using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SDGApp.ViewModel
{
    public class CardiacViewModel
    {
        
        public String HR { get; set; }
        public String HRV { get; set; }
        public String SBP { get; set; }

        public String BP { get; set; }
        public String DBP { get; set; }
        public String HRV1 { get; set; }
        public String HRV2 { get; set; }
        
        public DateTime CreatedDateTime { get; set; }
        public long AVGSBP { get; set; }
        public double AVGDBP { get; set; }
        public double AVGHR { get; set; }

        public String SBPValues { get; set; }
        public String DBPValues { get; set; }
        public String HRValues { get; set; }

        public String DateValues { get; set; }

        public String CreatedDateTimeStamp { get; set; }
    }
}