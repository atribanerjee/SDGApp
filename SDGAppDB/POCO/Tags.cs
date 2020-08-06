using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SDGAppDB.POCO
{
    [Table("Tags")]
   public class Tags
    {
        [Key]
        public int TagID { get; set; }
        public String TagName { get; set; }
        public String Description { get; set; }
        public bool IsDeleted { get; set; }

        [ForeignKey("User")]
        public Int32 FKUserID { get; set; }
        public User User { get; set; }
    }
}
