using System;
using System.Collections.Generic;

namespace FileUploadLib.Data.OldContext
{
    public partial class HeaderFooterItems
    {
        public int ItemId { get; set; }
        public int? PageId { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public string Type { get; set; }
        public string Position { get; set; }
        public bool? Publish { get; set; }
        public bool? Deleted { get; set; }
        public string WrittenBy { get; set; }
        public DateTime? WrittenDate { get; set; }
        public string EditedBy { get; set; }
        public DateTime? EditedDate { get; set; }

        public virtual Cmspages2 Page { get; set; }
    }
}
