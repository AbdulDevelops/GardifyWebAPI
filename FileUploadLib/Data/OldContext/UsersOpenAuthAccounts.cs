using System;
using System.Collections.Generic;

namespace FileUploadLib.Data.OldContext
{
    public partial class UsersOpenAuthAccounts
    {
        public string ApplicationName { get; set; }
        public string ProviderName { get; set; }
        public string ProviderUserId { get; set; }
        public string ProviderUserName { get; set; }
        public string MembershipUserName { get; set; }
        public DateTime? LastUsedUtc { get; set; }

        public virtual UsersOpenAuthData UsersOpenAuthData { get; set; }
    }
}
