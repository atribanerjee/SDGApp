using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SDGAppDB.POCO
{
    [Table("UserMeasurement")]
    public class UserMeasurement
    {
        [Key]
        public Int64 ID { get; set; }

        public int FKUserId { get; set; }

        public String FileName { get; set; }
      
        public DateTime CreatedDateTime { get; set; }


    }
}
