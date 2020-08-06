using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SDGAppDB.POCO
{
    [Table("TagCategories")]
   public class TagCategories
    {
        [Key]
        public int TagCategoryID { get; set; }
        public String Prompt { get; set; }
        public String TagCategoryValue { get; set; }
        public bool IsDeleted { get; set; }

        [ForeignKey("Tags")]
        public Int32 FKTagID { get; set; }
        public Tags Tags { get; set; }

        [ForeignKey("User")]
        public Int32 FKUserID { get; set; }
        public User User { get; set; }

        [ForeignKey("TagDetails")]
        public Int32 FKTagDetailsID { get; set; }
        public TagDetails TagDetails { get; set; }
    }
}
