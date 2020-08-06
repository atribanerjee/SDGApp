using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SDGApp.ViewModel
{
    public class UserMessageViewModel
    {
        public Int32 MessageID { get; set; }

        public int SenderUserID { get; set; }

        public int ReceiverUserID { get; set; }

        public int FkMessageTypeID { get; set; }

        public String MessageTypeName { get; set; }

        public string MessageFrom { get; set; }


        [Required(ErrorMessage = "Enter Message To")]
        [Display(Name = "To")]
        public String MessageTo { get; set; }

        [Display(Name = "Cc")]
        public String MessageCc { get; set; }

        [Required(ErrorMessage = "Enter Subject")]
        [Display(Name = "Subject")]
        public String MessageSubject { get; set; }

        //[Required(ErrorMessage = "Enter Body")]
        [Display(Name = "Body")]
        public String MessageBody { get; set; }


        public DateTime CreatedDateTime { get; set; }

        public bool IsViewed { get; set; }

        public bool IsDeleted { get; set; }

        public Int32 TotalRecords { get; set; }

        public Int32 PageFrom { get; set; }

        public Int32 PageTo { get; set; }

        public String UserEmailTo { get; set; }
        public String UserEmailCc { get; set; }

        public int LoginUserID { get; set; }
        public String LoginUserEmail { get; set; }

        public string SenderUserName { get; set; }
        public string ReceiverUserName { get; set; }
        public List<string> lstemilto { get; set; }
        public List<string> lstemilCc { get; set; }

        public List<HttpPostedFileBase> FileAttachments { get; set; }
        public List<FileViewModel> AttachmentFiles { get; set; }
        public String Filesids { get; set; }

        public string MessageCreatedDateTime { get; set; }

        public string MsgResponseType { get; set; }

        public List<UserMessageViewModel> MessageTree { get; set; }
      
        public int? ReplyMessageID { get; set; }
        public int? FKReplyTypeID { get; set; }

        public int? FKMessageID { get; set; }
        public List<int> MessageReturnIDs { get; set; }

        public string UserNameCc { get; set; }

        public String ReplyMessageTo { get; set; }

        public string UserFile { get; set; }
        public string FileExtension { get; set; }

        public string RequestType { get; set; }

        public int MsgResponseTypeID { get; set; }

        public int ReplyMessageCount { get; set; }

        public String ParentGUIID { get; set; }

        public int LastInboxMessageID { get; set; }

    }

    public class FileViewModel
    {
        public int FileID { get; set; }
        public String FileName { get; set; }
    }

    public class ReplyConversationCount
    {
        public int MessageID { get; set; }

        public int Count { get; set; }
    }
}