using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SDGAppDB.POCO
{
    [Table("StateDetail")]

    public class StateDetail
    {
        [Key]
        public int StateID { get; set; }
        public String StateName { get; set; }

        [ForeignKey("Country")]
        public Int32 FKCountryID { get; set; }
        public Country Country { get; set; }
    }

}
