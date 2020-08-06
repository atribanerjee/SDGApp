using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SDGAppDB.POCO
{

    [Table("SkinType")]
    public class SkinType
    {
        [Key]
        public int ID { get; set; }
        public String UUID { get; set; }
        public String BPSystolic { get; set; }
        public String BPDiastolic { get; set; }
        public String HeartRate { get; set; }
        public String PuseTransitTime { get; set; }
        public String SDNN { get; set; }

        public String RMSDD { get; set; }
        public String RRIntervals { get; set; }
        public String DeviceID { get; set; }
        public DateTime CreatedDateTime { get; set; }

        [ForeignKey("User")]
        public Int32 FKUserID { get; set; }
        public User User { get; set; }
    }
}
