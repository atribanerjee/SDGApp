using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SDGAppDB.POCO
{
    [Table("MessageAttachment")]
    public class MessageAttachment
    {
        [Key]
        public Int32 ID { get; set; }
        public int FKMessageID { get; set; }
        public String FileName { get; set; }

    }
}
