using System;

namespace SDGApp.ViewModel
{
    public class ThirdPartyViewModel
    {
        public Int64 UserThirdPartyAPIKeyID { get; set; }
        public Int64 FKCompanyID { get; set; }
        public String APIKeyName { get; set; }
        public String APIKeyID { get; set; }
        public String APIKeyValue { get; set; }
        public String ConfimationCode { get; set; }
        public Boolean IsActive { get; set; }
        public Boolean IsDeleted { get; set; }
    }
}