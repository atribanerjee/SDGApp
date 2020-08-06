using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System;

namespace SDGAppDB.POCO
{
    [Table("AttributeValueType")]
   public class AttributeValueType
    {
        [Key]
        public int TypeID { get; set; }
        public String TypeName { get; set; }
    }
}
