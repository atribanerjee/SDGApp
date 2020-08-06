using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SDGAppDB.POCO
{
    [Table("TagLabel")]
    public class TagLabel
    {
        [Key]
        public int ID { get; set; }
        public String LabelName { get; set; }
        public int MinRange { get; set; }
        public int MaxRange { get; set; }
        public int DefaultValue { get; set; }
        public int PrecisionDigit { get; set; }

        public string UnitName { get; set; }

        public string ImageName { get; set; }

        public String HasColorCode { get; set; }

        public int UserID { get; set; }

        public int FKTagLabelTypeID { get; set; }

        public DateTime CreatedDateTime { get; set; }
    }
}
