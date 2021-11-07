using System;
using System.Collections.Generic;

namespace FileUploadLib.Data.OldContext
{
    public partial class UsersOpenAuthData
    {
        public UsersOpenAuthData()
        {
            UsersOpenAuthAccounts = new HashSet<UsersOpenAuthAccounts>();
        }

        public string ApplicationName { get; set; }
        public string MembershipUserName { get; set; }
        public bool HasLocalPassword { get; set; }

        public virtual ICollection<UsersOpenAuthAccounts> UsersOpenAuthAccounts { get; set; }
    }
}
