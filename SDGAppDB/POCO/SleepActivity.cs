using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SDGAppDB.POCO
{
    [Table("SleepActivity")]
    public class SleepActivity
    {
        [Key]
        public int ID { get; set; }

        [ForeignKey("User")]
        public int FKUserID { get; set; }
        public User User { get; set; }
        public decimal DeepSleepHour { get; set; }
        public decimal LightSleepHour { get; set; }
        public decimal StayUPHour { get; set; }
        public DateTime CreatedDateTime { get; set; }

        public int? SleepTotalTime { get; set; }
        public int? SleepWalkingNumber { get; set; }

        

    }
}
