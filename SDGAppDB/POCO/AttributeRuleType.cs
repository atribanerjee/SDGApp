using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SDGAppDB.POCO
{
    [Table("AttributeRuleType")]
   public class AttributeRuleType
    {
        [Key]
        public Int32 AttributeRuleTypeID { get; set; }
        public String AttributeRuleTypeText { get; set; }
    }
}
