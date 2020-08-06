using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDGAppDB.POCO
{
    [Table("UserMessageType")]
    public class UserMessageType
    {
        [Key]
        public int ID { get; set; }

        public int MessageType { get; set; }
       
    }
}
