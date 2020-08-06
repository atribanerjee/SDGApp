using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SDGAppDB.POCO
{
    [Table("AppDetails")]
    public class AppDetails
    {
        [Key]
        public int ID { get; set; }        
        public int FKUserID { get; set; }
        public int FKAppTypeID { get; set; }
        public String AppURL { get; set; }
        public String AppVersion { get; set; }
        public DateTime? UploadDateTime { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsDelete { get; set; }

    }
}
