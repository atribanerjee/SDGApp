using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace SDGAppDB.POCO
{
    [Table("WidgetMaster")]
    public class WidgetMaster
    {
        [Key]
        public int ID { get; set; }
        public String WidgetName { get; set; }

    }
}
