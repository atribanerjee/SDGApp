using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SDGApp.Helpers
{
    public class SDGUtilities
    {
        public enum UserRoleType
        {
            Administrator = 1,
            User = 2,
        }

        public enum MessageTypeList
        {
            Inbox = 1,
            Sent = 2,
            Bin = 3,
            Draft = 4
        }

        public enum SleepType
        {
            StayUp = 0,
            StartSleeping = 1,
            LightSleep = 2,
            DeepSleep = 3,
            SleepInterruption = 4,
            SleepComplete = 5

        }
        public enum WidGetList
        {
            CardioDayWise,
            CardioWeekWise,
            SleepWeekWise,
            ActivityWeekWise
        }

        public enum ContactTypeList
        {
            UserContact = 1,
            PendingContact = 2,
            ContactsInvitation = 3
        }

        public enum GetDataType
        {
            Download=1,

            Upload=2
        }

        public enum UserMessageReplyTypeList
        {
            Reply = 1,
            ReplyToAll = 2,
            Forward = 3
        }
    }
}