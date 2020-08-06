using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SDGAppDB.POCO
{
    [Table("WidgetStyle")]
    public class WidgetStyle
    {
        [Key]
        public int ID { get; set; }
        public int FkUserID { get; set; }
        public String WidgetName { get; set; }
        public Decimal Height { get; set; }
        public Decimal Width { get; set; }
        public int Position { get; set; }
        public DateTime CreatedDateTime { get; set; }

        public int FKWidgetId { get; set; }
        public int FKTabId { get; set; }

    }
}
