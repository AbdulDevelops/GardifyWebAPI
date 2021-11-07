using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GardifyModels.Models
{
    public class PayoneRequest : _BaseEntity
    {
        public string RequestMerchantId { get; set; }
        public string RequestPortalId { get; set; }
        public string RequestKey { get; set; }
        public string RequestAPIVersion { get; set; }
        public string RequestMode { get; set; }
        public string RequestEncoding { get; set; }
    }
}