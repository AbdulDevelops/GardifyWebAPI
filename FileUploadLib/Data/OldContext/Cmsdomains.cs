using System;
using System.Collections.Generic;

namespace FileUploadLib.Data.OldContext
{
    public partial class Cmsdomains
    {
        public Cmsdomains()
        {
            Cmsentities = new HashSet<Cmsentities>();
        }

        public int DomainId { get; set; }
        public string Domain { get; set; }
        public string LocalDomain { get; set; }

        public virtual ICollection<Cmsentities> Cmsentities { get; set; }
    }
}
