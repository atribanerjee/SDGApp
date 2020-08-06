using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SDGAppDB.POCO
{
    [Table("HelpContent")]
   public class HelpContent
    {
        [Key]
        public Int32 HelpContentID { get; set; }
        public String HelpText { get; set; }
        public Int32 FkTopicID { get; set; }
    }
}
