using System;
using System.Collections.Generic;

namespace FileUploadLib.Data.OldContext
{
    public partial class Roles
    {
        public Roles()
        {
            UsersInRoles = new HashSet<UsersInRoles>();
        }

        public Guid ApplicationId { get; set; }
        public Guid RoleId { get; set; }
        public string RoleName { get; set; }
        public string Description { get; set; }

        public virtual Applications Application { get; set; }
        public virtual ICollection<UsersInRoles> UsersInRoles { get; set; }
    }
}
