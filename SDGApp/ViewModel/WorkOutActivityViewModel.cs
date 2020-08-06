using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SDGApp.ViewModel
{
    public class WorkOutActivityViewModel
    {
        public int ID { get; set; }
        public int FKUserID { get; set; }
        public int Steps { get; set; }
        public decimal KCal { get; set; }
        public decimal Mileage { get; set; }
        public decimal Completion { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public String CreatedDateTimeStamp { get; set; }
        public Int32 TotalRecords { get; set; }
    }
}