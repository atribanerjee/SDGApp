using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SDGAppDB.POCO
{
    [Table("AttributeType")]
   public class AttributeType
    {
        [Key]
        public Int32 AttribteTypeID { get; set; }
        public String AttributeTypeName { get; set; }
        
    }
}
