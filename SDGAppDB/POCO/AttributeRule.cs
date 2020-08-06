using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace SDGAppDB.POCO
{
    [Table("AttributeRule")]
   public class AttributeRule
    {
        [Key]
        public Int32 AttribteRuleID { get; set; }
        public String AttributeRuleValue { get; set; }

        [ForeignKey("AttributeRuleLabel")]
        public Int32 FKRuleLabelID { get; set; }
        public AttributeRuleLabel AttributeRuleLabel { get; set; }

        [ForeignKey("AttributeRuleType")]
        public Int32 FKRuleTypeID { get; set; }
        public AttributeRuleType AttributeRuleType { get; set; }
        [ForeignKey("AttributeType")]
        public Int32 FKAttributeTypeID { get; set; }
        public AttributeType AttributeType { get; set; }


    }
}
