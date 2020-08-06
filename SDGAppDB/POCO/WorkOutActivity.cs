using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SDGAppDB.POCO
{
    [Table("WorkOutActivity")]
    public class WorkOutActivity
    {
        [Key]
        public int ID { get; set; }

        [ForeignKey("User")]
        public int FKUserID { get; set; }
        public User User { get; set; }

        public int Steps { get; set; }
        public decimal KCal { get; set; }
        public decimal Mileage { get; set; }
        public decimal Completion { get; set; }
        public DateTime CreatedDateTime { get; set; }

    }
}
