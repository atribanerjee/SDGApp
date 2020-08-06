using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SDGAppDB.POCO
{
    [Table("UserActivityDetails")]
   public class UserActivityDetails
    {
        [Key]
        public int UserActivityDetailsID { get; set; }
        public String Tag { get; set; }
        public String Measurement { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime EndDatetime { get; set; }


        [ForeignKey("User")]
        public Int32 FKUserID { get; set; }
        public User User { get; set; }

        [ForeignKey("PlannedActivities")]
        public Int32 FKPlannedActivitiesID { get; set; }
        public PlannedActivities PlannedActivities { get; set; }

        [ForeignKey("RecognisedActivities")]
        public Int32 FKRecognisedActivitiesID { get; set; }
        public RecognisedActivities RecognisedActivities { get; set; }

        [ForeignKey("DeviceDetail")]
        public Int32 FKDeviceID { get; set; }
        public DeviceDetail DeviceDetail { get; set; }
    }
}
