using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SDGApp.ViewModel
{
    public class UserMeasurementViewModel
    {
        public Int64 ID { get; set; }

        public int FKUserId { get; set; }

        public String FileName { get; set; }

        public DateTime CreatedDateTime { get; set; }
    }
}