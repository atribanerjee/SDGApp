using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SDGAppDB.POCO
{
    [Table("UserRole")]
   public class UserRole
    {
        [Key]
        public int UserRoleID { get; set; }


        [ForeignKey("User")]
        public Int32 FKUserID { get; set; }
        public User User { get; set; }

        [ForeignKey("Role")]
        public Int32 FKRoleID { get; set; }
        public Role Role { get; set; }
    }
}
