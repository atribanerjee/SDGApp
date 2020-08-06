using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SDGAppDB.POCO
{
    [Table("AttributeRuleLabel")]
    public class AttributeRuleLabel
    {
        [Key]
        public Int32 AttributeRuleLabelID { get; set; }
        public String AttributeRuleLabelText { get; set; }
    }
}
