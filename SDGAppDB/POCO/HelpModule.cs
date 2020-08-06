using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SDGAppDB.POCO
{
    [Table("HelpModule")]
   public class HelpModule
    {
        [Key]
        public Int32 HelpModuleID { get; set; }
        public String Topic { get; set; }
    }
}
