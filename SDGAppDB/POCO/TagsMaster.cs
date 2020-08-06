using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SDGAppDB.POCO
{
    [Table("TagsMaster")]
    public class TagsMaster
    {
        [Key]
        public int ID { get; set; }

        [ForeignKey("TagLabel")]
        public Int32 FKTagLabelID { get; set; }
        public User TagLabel { get; set; }

       
        public int TagValue { get; set; }
        public string Note { get; set; }

        [ForeignKey("User")]
        public Int32 FKUserID { get; set; }
        public User User { get; set; }

        public DateTime CreatedDateTime { get; set; }

        public bool IsUpdate { get; set; }
        public string TagImage { get; set; }
    }
}
