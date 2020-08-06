using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SDGAppDB.POCO
{
    [Table("RecognisedActivities")]
   public class RecognisedActivities
    {
        [Key]
        public int RecognisedActivitiesID { get; set; }
        public String RecognisedActivitiesName { get; set; }

    }
}
