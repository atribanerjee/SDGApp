using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SDGAppDB.POCO
{
    [Table("CarePeople")]
    public class CarePeople
    {
        [Key]
        public int CarePeopleID { get; set; }

        public int CarePersonUserID { get; set; }
        public int RequestUserID { get; set; }
        public String Status { get; set; }
        public Boolean IsViewed { get; set; }
        public Boolean IsActive { get; set; }
        public Boolean IsDeleted { get; set; }
        public DateTime CreatedDateTime { get; set; }
    }
}
