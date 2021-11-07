using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GardifyModels.Models
{
    public class UserContact: _BaseEntity
    {
        public Guid ContactUserId { get; set; }

        public Guid UserId { get; set; }
    }

    public class UserContactMessage: _BaseEntity
    {
        public Guid SenderId { get; set; }
        public Guid ReceiverId { get; set; }
        public string Message { get; set; }

    }


    public class UserContactAddViewModel
    {
        public string Username { get; set; }
        
    }
}