using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SDGAppDB.POCO
{
    [Table("TagDetails")]
   public class TagDetails
    {
        [Key]
        public int TagDetailsID { get; set; }
        public String Prompt { get; set; }
        public String Min { get; set; }
        public String Max { get; set; }
        public String Choice { get; set; }

        [ForeignKey("Tags")]
        public Int32 FKTagID { get; set; }
        public Tags Tags { get; set; }

        [ForeignKey("AttributeValueType")]
        public Int32 FKTagTypeID { get; set; }
        public AttributeValueType AttributeValueType { get; set; }
    }
}
