using System;
using System.Collections.Generic;

namespace FileUploadLib.Data.OldContext
{
    public partial class Users
    {
        public Users()
        {
            UsersInRoles = new HashSet<UsersInRoles>();
        }

        public Guid ApplicationId { get; set; }
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public bool IsAnonymous { get; set; }
        public DateTime LastActivityDate { get; set; }

        public virtual Applications Application { get; set; }
        public virtual Memberships Memberships { get; set; }
        public virtual Profiles Profiles { get; set; }
        public virtual ICollection<UsersInRoles> UsersInRoles { get; set; }
    }
}
