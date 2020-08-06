using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SDGApp.ViewModel
{
    public class ThirdPartyMeasurmentViewModel
    {
        public List<CardiacDtls> Cardiac { get; set; }

        public List<WorkOutActivityViewModel> Activity { get; set; }

        public List<SleepActivityViewModel> Sleep { get; set; }

    }

    public class CardiacDtls
    {
        public String SBP { get; set; }

        public String DBP { get; set; }

        public String HR { get; set; }

        public String HRV { get; set; }

        public String CreatedDateTime { get; set; }
    }
}