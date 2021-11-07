using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GardifyModels.Models
{
    public class TempEmail: _BaseEntity
    {
        public string UserId { get; set; }
        public string Email { get; set; }
    }
}