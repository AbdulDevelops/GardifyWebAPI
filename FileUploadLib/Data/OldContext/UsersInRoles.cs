using System;
using System.Collections.Generic;

namespace FileUploadLib.Data.OldContext
{
    public partial class UsersInRoles
    {
        public Guid UserId { get; set; }
        public Guid RoleId { get; set; }

        public virtual Roles Role { get; set; }
        public virtual Users User { get; set; }
    }
}
