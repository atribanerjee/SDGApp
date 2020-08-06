using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SDGAppDB.POCO
{
    [Table("UserMessage")]
    public class UserMessage
    {
        [Key]
        public int MessageID { get; set; }

        public int SenderUserID { get; set; }

        public int ReceiverUserID { get; set; }

        [ForeignKey("UserMessageType")]
        public int FkMessageTypeID { get; set; }
        public UserMessageType UserMessageType { get; set; }

        public string MessageFrom { get; set; }

        public string MessageTo { get; set; }

        public string MessageCC { get; set; }

        public string MessageSubject { get; set; }

        public string MessageBody { get; set; }

        public bool IsViewed { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime CreatedDateTime { get; set; }

        public int? FKMessageID { get; set; }
        public int? ReplyMessageID { get; set; }
        public int? FKReplyTypeID { get; set; }

        public String ParentGUIID { get; set; }

    }
}
