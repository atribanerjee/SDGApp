using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDGAppDB.POCO
{
    [Table("TabMaster")]
    public class TabMaster
    {
        public int ID { get; set; }

        public int FKUserId { get; set; }

        public String TabName { get; set; }

        public DateTime CreatedDateTime { get; set; }
    }
}
