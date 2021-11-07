using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GardifyModels.Models
{
    public class PushSubscription: _BaseEntity
    {
        public Guid UserId { get; set; }
        public string EndPoint { get; set; }
        public string Auth { get; set; }
        public string P256dh { get; set; }
    }
}