using System;

namespace SDGApp.ViewModel
{
    public class PendingInvitationViewModel
    {
        public Int32 ContactID { get; set; }
        public Int32 SenderUserID { get; set; }
        public String SenderFirstName { get; set; }
        public String SenderLastName { get; set; }
        public String SenderEmail { get; set; }
        public String SenderPhone { get; set; }
        public String SenderPicture { get; set; }
    }
}