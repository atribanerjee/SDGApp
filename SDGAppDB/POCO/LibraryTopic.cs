using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDGAppDB.POCO
{
    [Table("LibraryTopic")]
    public class LibraryTopic
    {
        [Key]
        public int TopicID { get; set; }
        public string TopicName { get; set; }
        public string TopicDescription { get; set; }
        public bool IsActive { get; set; }
        public bool IsDelete { get; set; }
        public int FKCompanyID { get; set; }
        public int OrderID { get; set; }
        public string TopicWidth { get; set; }
    }
}
