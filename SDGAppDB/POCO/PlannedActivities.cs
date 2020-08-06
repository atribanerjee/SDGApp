using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace SDGAppDB.POCO
{
    [Table("PlannedActivities")]
   public class PlannedActivities
    {
        [Key]
        public int PlannedActivitiesID { get; set; }
        public String PlannedActivitiesName { get; set; }
        public DateTime DefaultDateTime { get; set; }
        public bool IsDeleted { get; set; }
    }
}
