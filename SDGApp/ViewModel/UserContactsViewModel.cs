using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SDGApp.ViewModel
{
    public class UserContactsViewModel
    {
        public int ContactID { get; set; }

        public int FKSenderUserID { get; set; }
        public int? FKReceiverUserID { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string SenderFirstName { get; set; }
        public string SenderLastName { get; set; }

        public string SenderEmail { get; set; }
        public string SenderPhone { get; set; }
        public string SenderPicture { get; set; }

        public string ReceiverFirstName { get; set; }
        public string ReceiverLastName { get; set; }
        public string ReceiverEmail { get; set; }
        public string ReceiverPhone { get; set; }
        public string ReceiverPicture { get; set; }

        public string Email { get; set; }
        public string Phone { get; set; }
        public string Picture { get; set; }

        public bool IsDeleted { get; set; }
        public string ContactGuid { get; set; }
        public bool IsAccepted { get; set; }
        public bool IsRejected { get; set; }
        public DateTime CreatedDatetime { get; set; }
        public String CreatedDatetimeString { get; set; }

        public Int32 PageNumber { get; set; }

        public Int32 PageSize { get; set; }

        public int TotalRecords { get; set; }

        public DateTime? AcceptedDate { get; set; }
    }
}