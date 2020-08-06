using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SDGAppDB.POCO
{
    [Table("CityDetail")]
   public class CityDetail
    {
        [Key]
        public int CityID { get; set; }
        public String CityName { get; set; }

        [ForeignKey("StateDetail")]
        public Int32 FKStateID { get; set; }
        public StateDetail StateDetail { get; set; }
    }
}
