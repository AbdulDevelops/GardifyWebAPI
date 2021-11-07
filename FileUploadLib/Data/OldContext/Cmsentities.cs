using System;
using System.Collections.Generic;

namespace FileUploadLib.Data.OldContext
{
    public partial class Cmsentities
    {
        public int EntityId { get; set; }
        public int? DomainId { get; set; }
        public string EntityName { get; set; }
        public string Uiculture { get; set; }
        public string Culture { get; set; }
        public string MasterPage { get; set; }

        public virtual Cmsdomains Domain { get; set; }
    }
}
