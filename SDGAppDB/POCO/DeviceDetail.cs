using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SDGAppDB.POCO
{
    [Table("DeviceDetail")]
    public class DeviceDetail
    {
        [Key]
        public int DeviceID { get; set; }
        public String SerialNumber { get; set; }
        public String DeviceIMEI { get; set; }
        public bool IsActive { get; set; }
    }
}
