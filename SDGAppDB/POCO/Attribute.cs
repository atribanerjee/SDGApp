using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SDGAppDB.POCO
{
    [Table("Attribute")]
  public  class Attribute
    {
        [Key]
        public Int32 AttributeID { get; set; }
        public String AttributeKey { get; set; }
        public String AttributeValue { get; set; }

        [ForeignKey("AttributeType")]
        public Int32 FKAttributeTypeID { get; set; }
        public AttributeType AttributeType { get; set; }

        public Int32 FKDataID { get; set; }
        public Int32 AttributeRuleLabelID { get; set; }
    }
}
