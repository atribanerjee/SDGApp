using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System;

namespace SDGAppDB.POCO
{
    [Table("UserType")]
   public class UserType
    {
        [Key]
        public int UserTypeID { get; set; }
        public String UserTypeName { get; set; }
    }
}
