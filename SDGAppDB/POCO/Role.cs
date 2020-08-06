using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace SDGAppDB.POCO
{
    [Table("Role")]
   public class Role
    {
        [Key]
        public int RoleID { get; set; }
        public String RoleName { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
    }
}
