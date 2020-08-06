using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SDGAppDB.POCO
{
    [Table("SleepDataDtls")]
   public class SleepDataDtls
    {
        [Key]
        public int ID { get; set; }

        [ForeignKey("SleepActivity")]
        public int FKSleepActivityID { get; set; }
        public SleepActivity SleepActivity { get; set; }

        [ForeignKey("SleepType")]
        public int FKSleepTypeID { get; set; }
        public SleepType SleepType { get; set; }

        public String StartTime { get; set; }
       

    }
}
