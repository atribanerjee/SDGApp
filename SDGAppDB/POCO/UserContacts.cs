using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SDGAppDB.POCO
{
    [Table("UserContacts")]
    public class UserContacts
    {
        [Key]
        public int ContactID { get; set; }

        public int FKSenderUserID { get; set; }
        public int? FKReceiverUserID { get; set; }
        public bool IsDeleted { get; set; }
        public string ContactGuid { get; set; }
        public bool IsAccepted { get; set; }
        public bool IsRejected { get; set; }
        public DateTime CreatedDatetime { get; set; }
        public DateTime? AcceptedDate { get; set; }

    }
}
