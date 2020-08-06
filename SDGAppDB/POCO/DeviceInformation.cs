using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDGAppDB.POCO
{
    [Table("DeviceInformation")]
    public class DeviceInformation
    {
        [Key]
        public int ID { get; set; }
        public int UserID { get; set; }
        public String DeviceName { get; set; }
        public String DeviceAddress { get; set; }
        public String BlockStatus { get; set; }
        public DateTime CreatedDateTime { get; set; }

    }
}
